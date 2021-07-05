using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimHUD.Data;
using RimHUD.Data.Configuration;
using RimHUD.Data.Integration;
using RimHUD.Data.Models;
using RimHUD.Interface.HUD;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    internal static class InspectPanePlus
    {
        private const float ButtonSize = 24f;

        private static Vector2 _scrollPosition = Vector2.zero;

        private static List<ITab_Pawn_Log_Utility.LogLineDisplayable> _log;
        private static ITab_Pawn_Log_Utility.LogDrawData _logDrawData = new ITab_Pawn_Log_Utility.LogDrawData();

        private static int _lastBattleTick = -1;
        private static int _lastPlayTick = -1;
        private static Pawn _pawn;

        private static readonly Dictionary<InspectTabBase, int> TabButtonWidths = new Dictionary<InspectTabBase, int>();

        public static void OnGUI(Rect inRect, IInspectPane pane)
        {
            Theme.CheckFontChange();

            var model = PawnModel.Selected;

            pane.RecentHeight = Theme.InspectPaneHeight.Value - 35f;

            if (model == null) { return; }
            if (!pane.AnythingSelected) { return; }

            var rect = inRect.ContractedBy(12f);
            rect.yMin -= 4f;
            rect.yMax += 6f;

            try
            {
                GUI.BeginGroup(rect.ExpandedBy(1f));

                var lineEndWidth = 0f;

                if (pane.ShouldShowSelectNextInCellButton)
                {
                    var selectOverlappingNextRect = new Rect(rect.width - ButtonSize, 0f, ButtonSize, ButtonSize);
                    if (Widgets.ButtonImage(selectOverlappingNextRect, Textures.SelectOverlappingNext)) { pane.SelectNextInCell(); }
                    lineEndWidth += ButtonSize;
                    TooltipHandler.TipRegion(selectOverlappingNextRect, "SelectNextInSquareTip".Translate(KeyBindingDefOf.SelectNextInCell.MainKeyLabel));
                }

                DrawButtons(rect, ref lineEndWidth);

                var previousAnchor = Text.Anchor;
                Text.Anchor = TextAnchor.UpperLeft;
                GUIPlus.SetFont(GameFont.Medium);
                GUIPlus.SetColor(model.FactionRelationColor);

                var labelRect = new Rect(0f, 0f, rect.width - lineEndWidth, Text.LineHeight);
                var label = model.Name;

                Widgets.Label(labelRect, label);
                if (Widgets.ButtonInvisible(labelRect)) { ToggleSocialTab(); }

                GUIPlus.ResetFont();
                GUIPlus.ResetColor();
                Text.Anchor = previousAnchor;
                GUIPlus.DrawTooltip(labelRect, model.BioTooltip, false);

                if (!pane.ShouldShowPaneContents) { return; }

                var contentRect = rect.AtZero();
                contentRect.yMin += 26f;
                DrawContent(contentRect, model, null);
            }
            catch (Exception exception) { Mod.HandleError(exception); }
            finally { GUI.EndGroup(); }
        }

        public static void DrawContent(Rect rect, PawnModel model, Pawn pawn)
        {
            if (pawn == null)
            {
                if (model == null) { throw new Mod.Exception("Both model and pawn are null"); }

                pawn = model.Base;
            }

            Text.Font = GameFont.Small;

            if (Theme.HudDocked.Value) { Hud.DrawDocked(rect, model); }
            else if (Theme.InspectPaneTabAddLog.Value) { DrawLog(pawn, rect); }
        }

        public static bool DrawTabs(IInspectPane pane)
        {
            if (pane.CurTabs == null)
            {
                Log.Warning("NO CURTABS");
                return false;
            }

            try
            {
                var y = pane.PaneTopY - 30f;
                var x = InspectPaneUtility.PaneWidthFor(pane) - Theme.InspectPaneTabWidth.Value;

                var width = 0f;
                var isSelected = false;

                foreach (var tab in pane.CurTabs)
                {
                    if (!tab.IsVisible) { continue; }

                    int buttonWidth;
                    if (TabButtonWidths.ContainsKey(tab)) { buttonWidth = TabButtonWidths[tab]; }
                    else
                    {
                        buttonWidth = Traverse.Create(tab).Field<int>("tabButtonWidth")?.Value ?? 0;
                        if (buttonWidth <= 0) { buttonWidth = Theme.InspectPaneTabWidth.Value; }
                        TabButtonWidths[tab] = buttonWidth;
                    }

                    var rect = new Rect(x, y, buttonWidth, 30f);
                    width = x;

                    Text.Font = GameFont.Small;

                    if (Widgets.ButtonText(rect, tab.labelKey.Translate())) { InterfaceToggleTab(tab, pane); }

                    var isOpenTab = tab.GetType() == pane.OpenTabType;
                    if (!isOpenTab && !tab.TutorHighlightTagClosed.NullOrEmpty()) { UIHighlighter.HighlightOpportunity(rect, tab.TutorHighlightTagClosed); }

                    if (isOpenTab)
                    {
                        tab.DoTabGUI();
                        pane.RecentHeight = 700f;
                        isSelected = true;
                    }

                    x -= Theme.InspectPaneTabWidth.Value;
                }
                if (!isSelected) { return false; }

                GUI.DrawTexture(new Rect(0f, y, width, 30f), Textures.InspectTabButtonFill);
            }
            catch (Exception exception) { Mod.HandleError(exception); }
            return false;
        }

        private static void DrawButtons(Rect rect, ref float lineEndWidth)
        {
            if (Find.Selector.NumSelected != 1) { return; }
            var selected = Find.Selector.SingleSelectedThing;
            if (selected == null) { return; }

            lineEndWidth += ButtonSize;
            Widgets.InfoCardButton(rect.width - lineEndWidth, 0f, selected);

            if (!(selected is Pawn pawn) || !PlayerControlled(pawn)) { return; }

            if (pawn.playerSettings.UsesConfigurableHostilityResponse)
            {
                lineEndWidth += ButtonSize;
                HostilityResponseModeUtility.DrawResponseButton(new Rect(rect.width - lineEndWidth, 0f, ButtonSize, ButtonSize), pawn, false);
                lineEndWidth += GUIPlus.SmallPadding;
            }

            lineEndWidth += ButtonSize;
            var careRect = new Rect(rect.width - lineEndWidth, 0f, ButtonSize, ButtonSize);
            MedicalCareUtility.MedicalCareSelectButton(careRect, pawn);
            GUIPlus.DrawTooltip(careRect, new TipSignal(() => Lang.Get("InspectPane.MedicalCare", pawn.KindLabel, pawn.playerSettings.medCare.GetLabel()), GUIPlus.TooltipId), true);
            lineEndWidth += GUIPlus.SmallPadding;

            if (!pawn.IsColonist) { return; }

            lineEndWidth += ButtonSize;

            var canDoctor = !pawn.WorkTypeIsDisabled(WorkTypeDefOf.Doctor);
            var canDoctorPriority = pawn.workSettings == null || pawn.workSettings?.GetPriority(WorkTypeDefOf.Doctor) > 0;

            var selfTendRect = new Rect(rect.width - lineEndWidth, 0f, ButtonSize, ButtonSize);
            var selfTendTip = "SelfTendTip".Translate(Faction.OfPlayer.def.pawnsPlural, 0.7f.ToStringPercent()).CapitalizeFirst();

            if (!canDoctor) { selfTendTip += "\n\n" + "MessageCannotSelfTendEver".Translate(pawn.LabelShort, pawn); }
            else if (!canDoctorPriority) { selfTendTip += "\n\n" + "MessageSelfTendUnsatisfied".Translate(pawn.LabelShort, pawn); }

            GUIPlus.SetFont(GameFont.Tiny);
            var selfTend = pawn.playerSettings.selfTend;
            selfTend = GUIPlus.DrawToggle(selfTendRect, selfTend, new TipSignal(() => selfTendTip, GUIPlus.TooltipId), canDoctor, Textures.SelfTendOnIcon, Textures.SelfTendOffIcon);
            if (selfTend != pawn.playerSettings.selfTend) { Mod_Multiplayer.SetSelfTend(pawn, selfTend); }
            GUIPlus.ResetFont();

            lineEndWidth += GUIPlus.SmallPadding;
        }

        private static bool PlayerControlled(Pawn pawn) => !pawn.Dead && pawn.playerSettings != null && ((pawn.Faction?.IsPlayer ?? false) || (pawn.HostFaction?.IsPlayer ?? false));

        private static void DrawLog(Pawn pawn, Rect rect)
        {
            if (_log == null || _lastBattleTick != pawn.records.LastBattleTick || _lastPlayTick != Find.PlayLog.LastTick || _pawn != pawn)
            {
                ClearCache();
                _log = ITab_Pawn_Log_Utility.GenerateLogLinesFor(pawn, true, true, true).ToList();
                _lastPlayTick = Find.PlayLog.LastTick;
                _lastBattleTick = pawn.records.LastBattleTick;
                _pawn = pawn;
            }

            var width = rect.width - GUIPlus.ScrollbarWidth;
            var height = _log.Sum(line => line.GetHeight(rect.width));

            if (height <= 0f) { return; }

            var viewRect = new Rect(0f, 0f, rect.width - GUIPlus.ScrollbarWidth, height);

            _logDrawData.StartNewDraw();

            Widgets.BeginScrollView(rect, ref _scrollPosition, viewRect);

            var y = 0f;
            foreach (var line in _log.OfType<ITab_Pawn_Log_Utility.LogLineDisplayableLog>())
            {
                line.Draw(y, width, _logDrawData);
                y += line.GetHeight(width);
            }

            Widgets.EndScrollView();
        }

        public static void ClearCache()
        {
            _log = null;
            _pawn = null;
            _lastBattleTick = -1;
            _lastPlayTick = -1;
            _logDrawData = new ITab_Pawn_Log_Utility.LogDrawData();
            _scrollPosition = Vector2.zero;
            TabButtonWidths.Clear();
        }

        private static void InterfaceToggleTab(InspectTabBase tab, IInspectPane pane) => Access.Method_RimWorld_InspectPaneUtility_InterfaceToggleTab.Invoke(null, tab, pane);

        private static void ToggleTab(Type tabType)
        {
            var pane = (MainTabWindow_Inspect) MainButtonDefOf.Inspect.TabWindow;
            var tab = (from t in pane.CurTabs where tabType.IsInstanceOfType(t) select t).FirstOrDefault();
            if (tab == null) { return; }

            if (Find.MainTabsRoot.OpenTab != MainButtonDefOf.Inspect) { Find.MainTabsRoot.SetCurrentTab(MainButtonDefOf.Inspect); }

            Access.Method_RimWorld_InspectPaneUtility_ToggleTab.Invoke(null, tab, pane);
        }

        public static void ToggleBioTab() => ToggleTab(typeof(ITab_Pawn_Character));
        public static void ToggleGearTab() => ToggleTab(typeof(ITab_Pawn_Gear));
        public static void ToggleHealthTab() => ToggleTab(typeof(ITab_Pawn_Health));
        public static void ToggleNeedsTab() => ToggleTab(typeof(ITab_Pawn_Needs));
        public static void ToggleSocialTab() => ToggleTab(typeof(ITab_Pawn_Social));
        public static void ToggleTrainingTab() => ToggleTab(typeof(ITab_Pawn_Training));
    }
}
