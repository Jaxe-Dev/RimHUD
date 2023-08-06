using System.Linq;
using RimHUD.Configuration;
using RimHUD.Extensions;
using UnityEngine;

namespace RimHUD.Interface.Dialog
{
  public class TabManager
  {
    private const float TabPadding = 4f;

    private readonly Tab[] _tabs;
    private Tab _selected;

    private readonly float _tabWidth;
    private readonly float _tabHeight;

    public TabManager(float tabWidth, float tabHeight, params Tab[] tabs)
    {
      _tabWidth = tabWidth;
      _tabHeight = tabHeight;
      _tabs = tabs.Where(tab => tab != null).ToArray();

      if (_tabs.Length == 0) { return; }
      _selected = Persistent.HasCredits ? _tabs.Last() : _tabs.First();
    }

    public void SelectType<T>() where T : Tab => _selected = _tabs.First(tab => tab.GetType() == typeof(T)) ?? _selected;

    public void Reset()
    {
      foreach (var tab in _tabs) { tab.Reset(); }
    }

    public void Draw(Rect rect)
    {
      if (_tabs.Length == 0) { return; }

      var vGrid = rect.GetVGrid(TabPadding, _tabHeight, -1f);
      var hGrid = vGrid[1].GetHGrid(TabPadding, Enumerable.Repeat(_tabWidth, _tabs.Length).ToArray());

      for (var index = 0; index < _tabs.Length; index++)
      {
        var tab = _tabs[index];
        GUIPlus.SetColor(tab == _selected ? Theme.ButtonSelectedColor : (Color?)null);
        if (WidgetsPlus.DrawButton(hGrid[index + 1], tab.Label, tab.Tooltip, enabled: tab.Enabled)) { _selected = tab; }
        GUIPlus.ResetColor();
      }

      _selected.Draw(vGrid[2]);
    }
  }
}
