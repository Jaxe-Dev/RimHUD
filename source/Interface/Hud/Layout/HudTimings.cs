using System.Collections.Generic;
using System.Diagnostics;
using RimHUD.Configuration;
using RimHUD.Interface.Hud.Layers;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Hud.Layout
{
  public static class HudTimings
  {
    private static readonly Dictionary<BaseLayer, Timing> Layers = new Dictionary<BaseLayer, Timing>();

    public static void Restart()
    {
      foreach (var timing in Layers.Values) { timing.Reset(); }
    }

    public static void Add(BaseLayer layer) => Layers[layer] = new Timing();

    public static void Remove(BaseLayer layer) => Layers.Remove(layer);

    public static Timing Update(BaseLayer layer)
    {
      if (!Mod.DevMode) { return null; }

      return Layers.TryGetValue(layer, out var timing) ? timing : null;
    }

    public class Timing
    {
      private static readonly TextStyle TimingsTextStyle = new TextStyle(nameof(TimingsTextStyle), Theme.SmallTextStyle, -3, -3, -3, 100, 100, 100);

      private static readonly Color ColorBackground = Color.black.ToTransparent(0.7f);
      private static readonly Color ColorForeground = Color.cyan.ToTransparent(0.2f);

      private readonly Stopwatch _stopwatch = new Stopwatch();

      private double _average;
      private long _max;

      public void Reset()
      {
        _stopwatch.Stop();
        _average = 0;
        _max = 0;
      }

      public void Start() => _stopwatch.Restart();

      public void Finish(Rect rect, bool isPanel = false)
      {
        _average = (int)(_average == 0 ? _stopwatch.ElapsedMilliseconds : (_average + _stopwatch.ElapsedMilliseconds) / 2);
        if (_stopwatch.ElapsedMilliseconds > _max) { _max = _stopwatch.ElapsedMilliseconds; }

        if (isPanel) { WidgetsPlus.DrawText(rect, $"[[Max={_max},Avg={_average},Now={_stopwatch.ElapsedMilliseconds} ms]", Theme.RegularTextStyle, Color.cyan, TextAnchor.MiddleCenter); }
        else
        {
          Verse.Widgets.DrawBoxSolidWithOutline(rect, ColorBackground, ColorForeground);
          WidgetsPlus.DrawText(rect, $"[M={_max},A={_average},N={_stopwatch.ElapsedMilliseconds}]", TimingsTextStyle, Color.yellow, TextAnchor.MiddleCenter);
        }
      }
    }
  }
}
