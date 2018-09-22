using System;
using System.Collections.Generic;
using System.Linq;
using RimHUD.Data;
using RimHUD.Integration;
using RimHUD.Patch;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface
{
    internal static class InspectPanePlus
    {
        private const float ButtonSize = 24f;
        private const float ButtonPadding = 4f;
        private const float SelfTendWidth = 80f;
        private const float SelfTendHeight = 20f;

        private const float WidgetRowGap = 6f;
        private const int BarsCount = 4;
        private const float BarHeight = 16f;

        private static Vector2 _scrollPosition = Vector2.zero;

        private static List<ITab_Pawn_Log_Utility.LogLineDisplayable> _log;
        private static ITab_Pawn_Log_Utility.LogDrawData _logDrawData = new ITab_Pawn_Log_Utility.LogDrawData();

        private static int _lastBattleTick = -1;
        private static int _lastPlayTick = -1;
        private static Pawn _pawn;

        private static void InterfaceToggleTab(InspectTabBase tab, IInspectPane pane) => Access.Method_RimWorld_InspectPaneUtility_InterfaceToggleTab.Invoke(null, new object[] { tab, pane });

        public static void OnGUI(Rect inRect, IInspectPane pane)
        {
            var model = PawnModel.Selected;

            pane.RecentHeight = 165f;

            if (model == null) { return; }
            if (!pane.AnythingSelected) { return; }

            var rect = inRect.ContractedBy(12f);
            rect.yMin -= 4f;
            rect.yMax += 6f;

            GUI.BeginGroup(rect);

            var lineEndWidth = 0f;

            if (pane.ShouldShowSelectNextInCellButton)
            {
                var selectOverlappingNextRect = new Rect(rect.width - ButtonSize, 0f, ButtonSize, ButtonSize);
                if (Widgets.ButtonImage(selectOverlappingNextRect, Textures.SelectOverlappingNextTex)) { pane.SelectNextInCell(); }
                lineEndWidth += 24f;
                TooltipHandler.TipRegion(selectOverlappingNextRect, "SelectNextInSquareTip".Translate(KeyBindingDefOf.SelectNextInCell.MainKeyLabel));
            }

            pane.DoInspectPaneButtons(rect, ref lineEndWidth);

            var labelRect = new Rect(0f, 0f, rect.width - lineEndWidth, 50f);
            var label = model.Name;

            labelRect.width += 300f;

            var previousAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.UpperLeft;
            GUIPlus.SetFont(GameFont.Medium);
            GUIPlus.SetColor(model.FactionRelationColor);
            Widgets.Label(labelRect, label);
            GUIPlus.ResetFont();
            GUIPlus.ResetColor();
            Text.Anchor = previousAnchor;

            if (!pane.ShouldShowPaneContents) { return; }

            var contentRect = rect.AtZero();
            contentRect.yMin += 26f;
            DrawContent(contentRect, model, null);
            GUI.EndGroup();
        }

        public static void DrawContent(Rect rect, PawnModel model, Pawn pawn)
        {
            if (pawn == null)
            {
                if (model == null) { throw new Mod.Exception("Both model and pawn are null"); }

                pawn = model.Base;
            }

            Text.Font = GameFont.Small;

            GUI.BeginGroup(rect);

            var y = 3f;
            var barWidth = Mathf.RoundToInt((rect.width - (WidgetRowGap * (BarsCount - 1))) / BarsCount);

            var row = new WidgetRow(0f, y);

            if (CanHaveRules(pawn))
            {
                DrawOutfit(row, pawn, barWidth);
                DrawRules(row, pawn, barWidth);
                DrawTimetableSetting(row, pawn, barWidth);
                DrawAreaAllowed(row, pawn, barWidth);
            }

            y += 18f;

            var contentRect = rect.AtZero();
            contentRect.yMin = y + 4f;

            if (Theme.HudDocked.Value) { HudDocked.OnGUI(model, contentRect); }
            else if (Theme.InspectPaneTabAddLog.Value) { DrawLog(pawn, contentRect); }

            GUI.EndGroup();
        }

        public static bool DrawTabs(IInspectPane pane)
        {
            try
            {
                var y = pane.PaneTopY - 30f;
                var x = InspectPaneUtility.PaneWidthFor(pane) - Theme.InspectPaneTabWidth.Value;

                var width = 0f;
                var isSelected = false;

                foreach (var curTab in pane.CurTabs)
                {
                    if (!curTab.IsVisible) { continue; }

                    var rect = new Rect(x, y, Theme.InspectPaneTabWidth.Value, 30f);
                    width = x;

                    Text.Font = GameFont.Small;

                    if (Widgets.ButtonText(rect, curTab.labelKey.Translate())) { InterfaceToggleTab(curTab, pane); }

                    var isOpenTab = curTab.GetType() == pane.OpenTabType;
                    if (!isOpenTab && !curTab.TutorHighlightTagClosed.NullOrEmpty()) { UIHighlighter.HighlightOpportunity(rect, curTab.TutorHighlightTagClosed); }

                    if (isOpenTab)
                    {
                        curTab.DoTabGUI();
                        pane.RecentHeight = 700f;
                        isSelected = true;
                    }

                    x -= Theme.InspectPaneTabWidth.Value;
                }
                if (!isSelected) { return false; }

                GUI.DrawTexture(new Rect(0.0f, y, width, 30f), Textures.InspectTabButtonFillTex);
            }
            catch (Exception ex) { Log.ErrorOnce(ex.ToString(), 742783); }

            return false;
        }

        public static void DrawSettingsButtons(Rect rect, ref float lineEndWidth)
        {
            if ((Find.Selector.NumSelected != 1) || !(Find.Selector.SingleSelectedThing is Pawn pawn) || !CanHaveRules(pawn)) { return; }

            lineEndWidth += ButtonPadding;

            var careRect = new Rect(rect.width - lineEndWidth - ButtonSize - ButtonPadding, 0f, ButtonSize, ButtonSize);
            MedicalCareUtility.MedicalCareSelectButton(careRect, pawn);
            GUIPlus.DrawTooltip(careRect, Lang.Get("InspectPane.MedicalCare", pawn.KindLabel, pawn.playerSettings.medCare.GetLabel()), true);

            lineEndWidth += ButtonSize + ButtonPadding;

            if (!pawn.IsColonist || pawn.Dead) { return; }

            var canDoctor = !pawn.story.WorkTypeIsDisabled(WorkTypeDefOf.Doctor);
            var canDoctorPriority = pawn.workSettings.GetPriority(WorkTypeDefOf.Doctor) > 0;

            var selfTendRect = new Rect(rect.width - lineEndWidth - ButtonPadding - SelfTendWidth, 0f + ((ButtonSize - SelfTendHeight) / 2f), SelfTendWidth, SelfTendHeight);

            var selfTendTip = "SelfTendTip".Translate(Faction.OfPlayer.def.pawnsPlural, 0.7f.ToStringPercent()).CapitalizeFirst();

            if (!canDoctor) { selfTendTip += "\n\n" + "MessageCannotSelfTendEver".Translate(pawn.LabelShort, pawn); }
            else if (!canDoctorPriority) { selfTendTip += "\n\n" + "MessageSelfTendUnsatisfied".Translate(pawn.LabelShort, pawn); }

            GUIPlus.SetFont(GameFont.Tiny);
            var previousAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleRight;
            pawn.playerSettings.selfTend = GUIPlus.DrawCheckbox(selfTendRect, "SelfTend".Translate(), pawn.playerSettings.selfTend, selfTendTip, canDoctor, SelfTendHeight);
            Text.Anchor = previousAnchor;
            GUIPlus.ResetFont();

            lineEndWidth += SelfTendWidth;
        }

        private static void DrawOutfit(WidgetRow row, Pawn pawn, float barWidth)
        {
            if (!pawn.IsColonistPlayerControlled || (pawn.outfits?.CurrentOutfit == null)) { return; }
            row.Gap(WidgetRowGap);

            var name = Lang.Get("InspectPane.OutfitFormat", pawn.outfits.CurrentOutfit.label);

            var rect = row.FillableBar(barWidth, BarHeight, 1f, name, BaseContent.GreyTex);
            if (Mouse.IsOver(rect))
            {
                var border = rect.ContractedBy(-1f);
                Widgets.DrawBox(border);
            }

            if (Widgets.ButtonInvisible(rect)) { DrawOutfitFloatMenu(pawn); }
        }

        private static void DrawOutfitFloatMenu(Pawn pawn)
        {
            var options = from outfit in Current.Game.outfitDatabase.AllOutfits select new FloatMenuOption(outfit.label, () => pawn.outfits.CurrentOutfit = outfit);
            Find.WindowStack.Add(new FloatMenu(options.ToList()));
        }

        private static bool CanHaveRules(Pawn pawn) => !pawn.Dead && ((pawn.Faction?.IsPlayer ?? false) || (pawn.HostFaction?.IsPlayer ?? false));

        private static void DrawRules(WidgetRow row, Pawn pawn, float barWidth)
        {
            if (!Theme.InspectPaneTabAddPawnRules.Value) { return; }

            var isLoaded = PawnRules.IsLoaded;
            var name = isLoaded ? PawnRules.GetRulesInfo(pawn) : null;

            row.Gap(WidgetRowGap);

            var label = isLoaded ? Lang.Get("InspectPane.PawnRules.RuleNameFormat", name) : Lang.Get("InspectPane.RulesDisabled");

            var rect = row.FillableBar(barWidth, BarHeight, 1f, label, BaseContent.GreyTex);
            if (Mouse.IsOver(rect))
            {
                var border = rect.ContractedBy(-1f);
                Widgets.DrawBox(border);
            }

            if (!Widgets.ButtonInvisible(rect)) { return; }

            if (isLoaded) { PawnRules.OpenRulesDialog(pawn); }
            else { Dialog_Alert.Open(PawnRules.RequiredAlert, Dialog_Alert.Buttons.YesNo, () => Application.OpenURL(PawnRules.Url)); }
        }

        private static void DrawTimetableSetting(WidgetRow row, Pawn pawn, float barWidth)
        {
            if (!pawn.IsColonistPlayerControlled || (pawn.timetable == null)) { return; }
            row.Gap(WidgetRowGap);

            var rect = row.FillableBar(barWidth, BarHeight, 1f, Lang.Get("InspectPane.TimetableFormat", pawn.timetable.CurrentAssignment.LabelCap), pawn.timetable.CurrentAssignment.ColorTexture);
            if (Mouse.IsOver(rect))
            {
                var border = rect.ContractedBy(-1f);
                Widgets.DrawBox(border);
            }

            if (Widgets.ButtonInvisible(rect)) { Find.MainTabsRoot.SetCurrentTab(DefDatabase<MainButtonDef>.GetNamed("Restrict")); }
        }

        private static void DrawAreaAllowed(WidgetRow row, Pawn pawn, float barWidth)
        {
            if (!pawn.IsColonistPlayerControlled || (pawn.playerSettings == null) || !pawn.playerSettings.RespectsAllowedArea) { return; }
            row.Gap(WidgetRowGap);

            var hasRestrictedArea = pawn.playerSettings?.EffectiveAreaRestriction != null;
            var fillTex = hasRestrictedArea ? pawn.playerSettings.EffectiveAreaRestriction.ColorTexture : BaseContent.GreyTex;
            var rect = row.FillableBar(barWidth, BarHeight, 1f, Lang.Get("InspectPane.AreaFormat", AreaUtility.AreaAllowedLabel(pawn)), fillTex);

            if (Mouse.IsOver(rect))
            {
                if (hasRestrictedArea) { pawn.playerSettings.EffectiveAreaRestriction.MarkForDraw(); }
                var border = rect.ContractedBy(-1f);
                Widgets.DrawBox(border);
            }
            if (Widgets.ButtonInvisible(rect)) { AreaUtility.MakeAllowedAreaListFloatMenu(area => pawn.playerSettings.AreaRestriction = area, true, true, pawn.Map); }
        }

        private static void DrawLog(Pawn pawn, Rect rect)
        {
            if ((_log == null) || (_lastBattleTick != pawn.records.LastBattleTick) || (_lastPlayTick != Find.PlayLog.LastTick) || (_pawn != pawn))
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
            foreach (var line in _log)
            {
                if (!(line is ITab_Pawn_Log_Utility.LogLineDisplayableLog)) { continue; }

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
        }
    }
}
