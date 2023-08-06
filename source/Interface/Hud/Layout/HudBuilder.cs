using System.Text;
using RimHUD.Configuration;
using RimHUD.Engine;
using RimHUD.Extensions;
using RimHUD.Interface.Dialog;
using RimHUD.Interface.Hud.Layers;
using RimHUD.Interface.Hud.Models;
using RimHUD.Interface.Hud.Widgets;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud.Layout
{
  public static class HudBuilder
  {
    private const string NeedId = "Need";
    private const string SkillId = "Skill";
    private const string TrainingId = "Training";
    private const string StatId = "Stat";
    private const string RecordId = "Record";

    public static bool IsValidType(string id) => HudContent.Widgets.ContainsKey(id) || id == HudContent.StatTypeName || id == HudContent.RecordTypeName || id == HudContent.NeedTypeName || id == HudContent.SkillTypeName || id == HudContent.TrainingTypeName;

    public static IWidget GetWidget(PawnModel owner, string id, string defName, string variant)
    {
      switch (id)
      {
        case NeedId:
          return GetNeedWidget(owner, defName, variant);
        case SkillId:
          return GetSkillWidget(owner, defName);
        case TrainingId:
          return GetTrainingWidget(owner, defName);
        case StatId:
          return GetStatWidget(owner, defName);
        case RecordId:
          return GetRecordWidget(owner, defName);
      }
      if (!HudContent.Widgets.TryGetValue(id, out var getter)) { return MissingWidget.Get(id, defName); }

      return getter.Invoke(owner, new object[] { variant }) ?? BlankWidget.GetEmpty;
    }

    private static IWidget GetNeedWidget(PawnModel owner, string defName, string variant)
    {
      var def = DefDatabase<NeedDef>.GetNamed(defName, false);

      if (def != null) { return (IWidget)BarWidget.FromModel(new NeedModel(owner, def), Theme.RegularTextStyle, variant) ?? BlankWidget.GetEmpty; }

      Mod.Warning($"Invalid HUD Widget, Need def '{defName}' not found, resetting layout to default");
      RequiredReset();
      return MissingWidget.Get(NeedId, defName);
    }

    private static IWidget GetSkillWidget(PawnModel owner, string defName)
    {
      var def = DefDatabase<SkillDef>.GetNamed(defName, false);
      if (def != null) { return (IWidget)ValueWidget.FromModel(new SkillModel(owner, def), Theme.RegularTextStyle) ?? BlankWidget.GetEmpty; }

      Mod.Warning($"Invalid HUD Widget, Skill def '{defName}' not found, resetting layout to default");
      RequiredReset();
      return MissingWidget.Get(SkillId, defName);
    }

    private static IWidget GetTrainingWidget(PawnModel owner, string defName)
    {
      var def = DefDatabase<TrainableDef>.GetNamed(defName, false);
      if (def != null) { return (IWidget)ValueWidget.FromModel(new TrainingModel(owner, def), Theme.RegularTextStyle) ?? BlankWidget.GetEmpty; }

      Mod.Warning($"Invalid HUD Widget, Trainable def '{defName}' not found, resetting layout to default");
      RequiredReset();
      return MissingWidget.Get(TrainingId, defName);
    }

    private static IWidget GetStatWidget(PawnModel owner, string defName)
    {
      var def = DefDatabase<StatDef>.GetNamed(defName, false);
      if (def != null)
      {
        try
        {
          if (def.Worker?.IsDisabledFor(owner.Base) ?? true) { return BlankWidget.GetEmpty; }

          var text = $"{def.GetLabelCap()}: {def.ValueToString(owner.Base.GetStatValue(def))}";

          return (IWidget)ValueWidget.FromModel(TextModel.Create(text), Theme.RegularTextStyle) ?? BlankWidget.GetEmpty;
        }
        catch { return BlankWidget.GetEmpty; }
      }

      Mod.Warning($"Invalid HUD Widget, Stat def '{defName}' not found, resetting layout to default");
      RequiredReset();

      return MissingWidget.Get(StatId, defName);
    }

    private static IWidget GetRecordWidget(PawnModel owner, string defName)
    {
      var def = DefDatabase<RecordDef>.GetNamed(defName, false);
      if (def != null)
      {
        var text = $"{def.LabelCap}: {(def.type == RecordType.Time ? owner.Base.records.GetAsInt(def).ToStringTicksToPeriod() : owner.Base.records.GetValue(def).ToString("0.##"))}";
        return (IWidget)ValueWidget.FromModel(TextModel.Create(text), Theme.RegularTextStyle) ?? BlankWidget.GetEmpty;
      }

      Mod.Warning($"Invalid HUD Widget, Record def '{defName}' not found, resetting layout to default");
      RequiredReset();
      return MissingWidget.Get(RecordId, defName);
    }

    private static void RequiredReset()
    {
      Dialog_Alert.Open(Lang.Get("Interface.Alert.InvalidLayout"));
      LayoutLayer.LoadDefaultAndSave();
    }

    public static void BuildStatString(Pawn pawn, StringBuilder builder, StatDef def) => builder.TryAppendLine(GetStatString(pawn, def));

    private static string GetStatString(Thing thing, StatDef def)
    {
      try
      {
        if (def.Worker?.IsDisabledFor(thing) ?? true) { return null; }

        return $"{def.LabelCap}: {def.ValueToString(thing.GetStatValue(def))}";
      }
      catch { return null; }
    }
  }
}
