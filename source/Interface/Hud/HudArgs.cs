using System.Xml.Linq;
using RimHUD.Extensions;
using RimHUD.Interface.Hud.Layers;
using RimHUD.Interface.Hud.Models;
using RimHUD.Interface.Hud.Widgets;
using RimWorld;
using Verse;

namespace RimHUD.Interface.Hud;

public sealed class HudArgs
{
  private const string TargetsAttribute = "Targets";
  private const string FillHeightAttribute = "FillHeight";

  private const string DefNameAttribute = "DefName";

  private const string BarColorStyleAttribute = "BarColorStyle";

  public LayerTarget Targets { get; set; } = LayerTarget.All;
  public bool FillHeight { get; set; }

  public string? DefName { get; set; }

  public BarColorStyle? BarColorStyle { get; set; }

  public HudArgs()
  { }

  public HudArgs(XElement xml)
  {
    foreach (var attribute in xml.Attributes())
    {
      var key = attribute.Name.ToString();
      var value = attribute.Value;

      switch (key)
      {
        case TargetsAttribute:
          Targets = LayerTargetUtility.FromId(value);
          break;
        case FillHeightAttribute:
          FillHeight = value.ToBool() ?? false;
          break;
        case DefNameAttribute:
          DefName = value;
          break;
        case BarColorStyleAttribute:
          BarColorStyle = value.ToEnum<BarColorStyle>();
          break;
      }
    }
  }

  public T? GetDef<T>() where T : Def => DefName is null ? null : DefDatabase<T>.GetNamed(DefName, false);
  public IWidget? GetDefAndBuild<T>() where T : Def, IModel => GetDef<T>()?.Build(this);

  public Def? GetDefFromLayerId(string id)
  {
    return DefName is null
      ? null
      : id switch
      {
        HudContent.NeedDefType => GetDef<NeedDef>(),
        HudContent.SkillDefType => GetDef<SkillDef>(),
        HudContent.TrainableDefType => GetDef<TrainableDef>(),
        HudContent.StatDefType => GetDef<StatDef>(),
        HudContent.RecordDefType => GetDef<RecordDef>(),
        HudContent.ExternalWidgetType => GetDef<CustomWidgetDef>(),
        HudContent.ExternalValueType => GetDef<CustomValueDef>(),
        HudContent.ExternalBarType => GetDef<CustomBarDef>(),
        HudContent.ExternalNeedType => GetDef<CustomNeedDef>(),
        _ => null
      };
  }

  public XElement ToXml(XElement xml)
  {
    xml.AddAttribute(TargetsAttribute, Targets.ToId());

    if (xml.Name != LayoutLayer.RootName) { xml.AddAttribute(FillHeightAttribute, FillHeight); }
    if (HudContent.IsDefType(xml.Name.ToString())) { xml.AddAttribute(DefNameAttribute, DefName); }

    xml.AddAttribute(BarColorStyleAttribute, BarColorStyle);
    return xml;
  }
}
