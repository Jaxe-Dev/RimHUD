using System.Collections.Generic;
using System.Diagnostics;
using HarmonyLib;
using RimHUD.Configuration;
using RimHUD.Configuration.Settings;
using RimHUD.Interface.Hud.Layers;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud
{
  public static class HudTimings
  {
    private static readonly Dictionary<BaseLayer, Timing> Layers = new();

    public static void Restart() => Layers.Values.Do(static timing => timing.Reset());

    public static void Add(BaseLayer layer) => Layers[layer] = new Timing();

    public static void Remove(BaseLayer layer) => Layers.Remove(layer);

    public static Timing? Update(BaseLayer layer)
    {
      if (!Mod.DevMode) { return null; }

      return Layers.TryGetValue(layer, out var timing) ? timing : null;
    }

    public sealed class Timing
    {
      private static readonly TextStyle TimingsTextStyle = new(null, Theme.SmallTextStyle, -3, -3, -3, 100, 100, 100);

      private static readonly Color ColorBackground = Color.black.ToTransparent(0.7f);
      private static readonly Color ColorForeground = Color.cyan.ToTransparent(0.2f);

      private readonly Stopwatch _stopwatch = new();

      private double _max;

      public void Reset()
      {
        _stopwatch.Stop();
        _max = 0;
      }

      public void Start() => _stopwatch.Restart();

      public void Finish(Rect rect, bool isPanel = false)
      {
        var value = (int)_stopwatch.ElapsedMilliseconds;
        if (value > _max) { _max = value; }

        if (isPanel) { WidgetsPlus.DrawText(rect, $"[[Max={_max}, Now={value} ms]", Theme.RegularTextStyle, Color.cyan, TextAnchor.MiddleCenter); }
        else
        {
          Verse.Widgets.DrawBoxSolidWithOutline(rect, ColorBackground, ColorForeground);
          WidgetsPlus.DrawText(rect, $"[M={_max}, N={value}]", TimingsTextStyle, Color.yellow, TextAnchor.MiddleCenter);
        }
      }
    }
  }
}
