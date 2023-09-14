using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Screen
{
  public static class InspectPaneLog
  {
    private const int LogLinesMax = 300;

    private static Vector2 _scrollPosition = Vector2.zero;

    private static List<ITab_Pawn_Log_Utility.LogLineDisplayable>? _log;
    private static ITab_Pawn_Log_Utility.LogDrawData _logDrawData = new();

    private static int _lastBattleTick = -1;
    private static int _lastPlayTick = -1;

    private static Pawn? _pawn;

    public static void Draw(Pawn pawn, Rect rect)
    {
      if (_log is null || _lastBattleTick != pawn.records!.LastBattleTick || _lastPlayTick != Find.PlayLog!.LastTick || _pawn != pawn)
      {
        ClearCache();
        _log = ITab_Pawn_Log_Utility.GenerateLogLinesFor(pawn, true, true, true, LogLinesMax).ToList();
        _lastPlayTick = Find.PlayLog!.LastTick;
        _lastBattleTick = pawn.records!.LastBattleTick;
        _pawn = pawn;
      }

      var width = rect.width - WidgetsPlus.ScrollbarWidth;
      var height = _log.Sum(line => line.GetHeight(rect.width));

      if (height <= 0f) { return; }

      var scrollRect = new Rect(0f, 0f, rect.width - WidgetsPlus.ScrollbarWidth, height);

      _logDrawData.StartNewDraw();

      Widgets.BeginScrollView(rect, ref _scrollPosition, scrollRect);

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
    }
  }
}
