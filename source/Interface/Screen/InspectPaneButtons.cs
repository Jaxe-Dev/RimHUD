using System.Linq;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Integration.Multiplayer;
using RimHUD.Interface.Hud.Tooltips;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Steam;

namespace RimHUD.Interface.Screen
{
  public static class InspectPaneButtons
  {
    public static void Draw(Rect bounds, IInspectPane pane, ref float offset)
    {
      if (pane.ShouldShowSelectNextInCellButton)
      {
        var selectOverlappingNextRect = GetRowRect(bounds, ref offset);
        if (Widgets.ButtonImage(selectOverlappingNextRect, TexButton.SelectOverlappingNext)) { pane.SelectNextInCell(); }

        if (SteamDeck.IsSteamDeckInNonKeyboardMode) { TooltipHandler.TipRegionByKey(selectOverlappingNextRect, "SelectNextInSquareTipController"); }
        else { TooltipHandler.TipRegionByKey(selectOverlappingNextRect, "SelectNextInSquareTip", KeyBindingDefOf.SelectNextInCell!.MainKeyLabel); }
      }

      if (Find.Selector!.NumSelected is not 1) { return; }

      var selected = Find.Selector.SingleSelectedThing;
      if (selected is null) { return; }

      var infoCardRect = GetRowRect(bounds, ref offset);
      Widgets.InfoCardButton(infoCardRect.x, infoCardRect.y, selected);

      if (selected is not Pawn pawn) { return; }

      if (AllowXenotype(pawn)) { DrawXenotype(pawn, GetRowRect(bounds, ref offset)); }

      if (AllowFaction(pawn)) { FactionUIUtility.DrawFactionIconWithTooltip(GetRowRect(bounds, ref offset), pawn.Faction); }

      if (AllowIdeo(pawn)) { DrawIdeoligion(pawn, GetRowRect(bounds, ref offset)); }

      if (!pawn.IsPlayerControlled()) { return; }

      offset += GUIPlus.SmallPadding;

      if (AllowResponse(pawn)) { HostilityResponseModeUtility.DrawResponseButton(GetRowRect(bounds, ref offset), pawn, false); }

      if (AllowMedical(pawn)) { DrawMedical(pawn, GetRowRect(bounds, ref offset)); }
      if (AllowSelfTend(pawn)) { DrawSelfTend(pawn, GetRowRect(bounds, ref offset)); }

      if (AllowRename(pawn)) { RenameUIUtility.DrawRenameButton(GetRowRect(bounds, ref offset, GenUI.SmallIconSize + GUIPlus.SmallPadding, GenUI.SmallIconSize + GUIPlus.SmallPadding), pawn); }
    }

    private static Rect GetRowRect(Rect bounds, ref float offset, float width = GenUI.SmallIconSize, float height = GenUI.SmallIconSize)
    {
      offset += width;
      var rect = new Rect(bounds.xMax - offset, bounds.GetCenteredY(height), width, height);
      offset += GUIPlus.SmallPadding;

      return rect;
    }

    private static void DrawSelfTend(Pawn pawn, Rect rect)
    {
      var canDoctor = !pawn.WorkTypeIsDisabled(WorkTypeDefOf.Doctor);

      var selfTend = pawn.playerSettings!.selfTend;
      selfTend = WidgetsPlus.DrawToggle(rect, selfTend, () => GetSelfTendTooltip(pawn, canDoctor), canDoctor, TexturesPlus.SelfTendOnIcon, TexturesPlus.SelfTendOffIcon);
      if (selfTend != pawn.playerSettings.selfTend) { Mod_Multiplayer.SetSelfTend(pawn, selfTend); }
    }

    private static string GetSelfTendTooltip(Pawn pawn, bool canDoctor)
    {
      var canDoctorPriority = pawn.workSettings is null || pawn.workSettings?.GetPriority(WorkTypeDefOf.Doctor) > 0;
      var selfTendTip = "AllowSelfTend".Translate(Faction.OfPlayer!.def!.pawnsPlural, 0.7f.ToStringPercent()).CapitalizeFirst();

      return !canDoctor ? $"{selfTendTip}\n\n{"MessageCannotSelfTendEver".Translate(pawn.LabelShort, pawn)}" : $"{selfTendTip}{(canDoctorPriority ? null : $"\n\n{"MessageSelfTendUnsatisfied".Translate(pawn.LabelShort, pawn)}")}";
    }

    private static void DrawMedical(Pawn pawn, Rect rect)
    {
      MedicalCareUtility.MedicalCareSelectButton(rect, pawn);
      TooltipsPlus.DrawSimple(rect, Lang.Get("Model.Health.MedicalCare", pawn.KindLabel, pawn.playerSettings!.medCare.GetLabel()));
    }

    private static void DrawIdeoligion(Pawn pawn, Rect rect)
    {
      IdeoUIUtility.DoIdeoIcon(rect, pawn.Ideo, false, () => IdeoUIUtility.OpenIdeoInfo(pawn.Ideo));
      if (!Mouse.IsOver(rect)) { return; }

      var name = pawn.ideo!.Ideo!.name.Colorize(ColoredText.TipSectionTitleColor);
      var certainty = "Certainty".Translate().CapitalizeFirst().Resolve().WithValue(pawn.ideo.Certainty.ToStringPercent());
      var previous = pawn.ideo.PreviousIdeos.Any() ? $"\n\n{"Formerly".Translate().CapitalizeFirst()}: \n{(from x in pawn.ideo.PreviousIdeos select x.name).ToLineList("  - ")}" : null;

      TooltipsPlus.DrawSimple(rect, $"{name}\n{certainty}{previous}");
    }

    private static void DrawXenotype(Pawn pawn, Rect rect)
    {
      if (Mouse.IsOver(rect)) { Widgets.DrawHighlight(rect); }

      if (Widgets.ButtonImage(rect, pawn.genes!.XenotypeIcon)) { InspectPaneUtility.OpenTab(typeof(ITab_Genes)); }

      if (!Mouse.IsOver(rect)) { return; }

      var tooltip = "Xenotype".Translate().CapitalizeFirst().Resolve().WithValue(pawn.genes.XenotypeLabelCap).Colorize(ColoredText.TipSectionTitleColor);
      var stage = pawn.ageTracker?.CurLifeStage?.LabelCap.ToString();

      if (stage is not null) { tooltip += $"\n{Lang.Get("Model.Bio.Lifestage", stage)}"; }
      if (!pawn.genes.Xenotype!.description.NullOrEmpty()) { tooltip += $"\n\n{pawn.genes.Xenotype.description}"; }

      TooltipHandler.TipRegion(rect, tooltip);
    }

    private static bool AllowXenotype(Pawn pawn) => ModsConfig.BiotechActive && pawn.genes?.Xenotype is not null;
    private static bool AllowFaction(Pawn pawn) => Theme.ShowFactionIcon.Value && pawn.Faction is not null && !pawn.Faction.def!.FactionIcon.NullOrBad();
    private static bool AllowIdeo(Pawn pawn) => Theme.ShowIdeoligionIcon.Value && ModsConfig.IdeologyActive && pawn.Ideo is not null;
    private static bool AllowResponse(Pawn pawn) => pawn.playerSettings!.UsesConfigurableHostilityResponse;
    private static bool AllowMedical(Pawn pawn) => pawn.RaceProps?.IsFlesh ?? false;
    private static bool AllowSelfTend(Pawn pawn) => pawn.IsColonist;
    private static bool AllowRename(Pawn pawn) => (pawn.Faction == Faction.OfPlayer && (pawn.RaceProps?.Animal ?? false) && pawn.RaceProps.hideTrainingTab) || (ModsConfig.BiotechActive && pawn.IsColonyMech);
  }
}
