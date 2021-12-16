using System.Collections.Generic;
using System.Diagnostics;
using RimHUD.Data.Configuration;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.HUD
{
  internal static class HudTimings
  {
    private static readonly Dictionary<HudComponent, Timing> Components = new Dictionary<HudComponent, Timing>();

    public static void Restart()
    {
      foreach (var component in Components.Values) { component.Reset(); }
    }

    public static void Add(HudComponent component) => Components[component] = new Timing();

    public static void Remove(HudComponent component) => Components.Remove(component);

    public static Timing Update(HudComponent component)
    {
      if (!Mod.DevMode) { return null; }

      return Components.TryGetValue(component, out var timing) ? timing : null;
    }

    public class Timing
    {
      private static readonly Color ColorBackground = Color.black.ToTransparent(0.4f);
      private static readonly Color ColorForeground = Color.cyan.ToTransparent(0.2f);
      private static readonly TextStyle MiniTextStyle = new TextStyle("Mini Text Style (Dev)", Theme.SmallTextStyle, -3, -3, -3, 100, 100, 100);

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
        _average = (int) (_average == 0 ? _stopwatch.ElapsedMilliseconds : (_average + _stopwatch.ElapsedMilliseconds) / 2);
        if (_stopwatch.ElapsedMilliseconds > _max) { _max = _stopwatch.ElapsedMilliseconds; }

        if (isPanel) { GUIPlus.DrawText(rect, $"[[Max={_max},Avg={_average},Now={_stopwatch.ElapsedMilliseconds} ms]", Color.cyan, Theme.RegularTextStyle, TextAnchor.MiddleCenter); }
        else
        {
          Widgets.DrawBoxSolidWithOutline(rect, ColorBackground, ColorForeground);
          GUIPlus.DrawText(rect, $"[M={_max},A={_average},N={_stopwatch.ElapsedMilliseconds}]", Color.yellow, MiniTextStyle, TextAnchor.MiddleCenter);
        }
      }
    }
  }
}
