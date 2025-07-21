using System;
using System.Collections.Generic;
using RimHUD.Engine;
using RimHUD.Interface.Hud.Layout;
using RimHUD.Interface.Hud.Models;
using RimHUD.Interface.Hud.Models.Bars;
using RimHUD.Interface.Hud.Models.Values;
using RimHUD.Interface.Hud.Widgets;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud;

public sealed class HudWidget
{
  private delegate IWidget? BuilderDelegate(HudArgs args);

  public string Id { get; }
  public Def? Def { get; }

  private readonly Type? _defType;

  private readonly BuilderDelegate _builder;
  public LayoutElement LayoutElement { get; }

  private readonly Dictionary<string, bool> _validDefNames = new();

  private HudWidget(string id, BuilderDelegate builder, Def? def = null, Type? defType = null)
  {
    Id = id;
    _builder = builder;

    Def = def;
    _defType = defType;

    LayoutElement = new LayoutElement(LayoutElementType.Widget, Id, Def);
  }

  public static HudWidget Blank() => new(BlankWidget.TypeName, static _ => BlankWidget.Collapsed);
  public static HudWidget Separator() => new(SeparatorWidget.TypeName, static _ => new SeparatorWidget());

  public static HudWidget FromModel<T>(string id, Def? def = null) where T : BaseModel, new() => new(id, static args => new T().Build(args), def);
  public static HudWidget FromEitherModel<T, TFallback>(string id, bool condition) where T : BaseModel, new() where TFallback : BaseModel, new() => condition ? FromModel<T>(id) : FromModel<TFallback>(id);
  public static HudWidget FromSkillModel(string id, SkillDef def) => new(id, args => new SkillValue(def).Build(args), def);
  public static HudWidget FromTrainableModel(string id, TrainableDef def) => new(id, args => new TrainableValue(def).Build(args), def);

  public static HudWidget FromNeedDef() => new(HudContent.NeedDefType, static args => args.GetDef<NeedDef>() is { } def ? new NeedBar(def).Build(args) : null, defType: typeof(NeedDef));
  public static HudWidget FromSkillDef() => new(HudContent.SkillDefType, static args => args.GetDef<SkillDef>() is { } def ? new SkillValue(def).Build(args) : null, defType: typeof(SkillDef));
  public static HudWidget FromTrainableDef() => new(HudContent.TrainableDefType, static args => args.GetDef<TrainableDef>() is { } def ? new TrainableValue(def).Build(args) : null, defType: typeof(TrainableDef));
  public static HudWidget FromStatDef() => new(HudContent.StatDefType, static args => args.GetDef<StatDef>() is { } def ? new StatValue(def).Build(args) : null, defType: typeof(StatDef));
  public static HudWidget FromRecordDef() => new(HudContent.RecordDefType, static args => args.GetDef<RecordDef>() is { } def ? new RecordValue(def).Build(args) : null, defType: typeof(RecordDef));

  public static HudWidget FromCustomWidgetDef() => new(HudContent.ExternalWidgetType, static args => args.GetDef<CustomWidgetDef>(), defType: typeof(CustomWidgetDef));
  public static HudWidget FromCustomValueDef() => new(HudContent.ExternalValueType, static args => args.GetDefAndBuild<CustomValueDef>(), defType: typeof(CustomValueDef));
  public static HudWidget FromCustomBarDef() => new(HudContent.ExternalBarType, static args => args.GetDefAndBuild<CustomBarDef>(), defType: typeof(CustomBarDef));
  public static HudWidget FromCustomSelectorDef() => new(HudContent.ExternalSelectorType, static args => args.GetDefAndBuild<CustomSelectorDef>(), defType: typeof(CustomSelectorDef));
  public static HudWidget FromCustomNeedDef() => new(HudContent.ExternalNeedType, static args => args.GetDefAndBuild<CustomNeedDef>(), defType: typeof(CustomNeedDef));

  public IWidget Build(HudArgs args)
  {
    try { return _builder.Invoke(args) ?? BlankWidget.Collapsed; }
    catch (Exception exception)
    {
      Report.HandleError(exception);
      return BlankWidget.Collapsed;
    }
  }

  public bool DefNameIsValid(string? defName)
  {
    if (_defType is null) { return true; }
    if (defName is null) { return false; }

    if (_validDefNames.TryGetValue(defName, out var value)) { return value; }

    var def = GenDefDatabase.GetDef(_defType, defName, false);
    return _validDefNames[defName] = def is not null && (def is not ExternalWidgetDef externalWidgetDef || externalWidgetDef.Initialized);
  }
}
