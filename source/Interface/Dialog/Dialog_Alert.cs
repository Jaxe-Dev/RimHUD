using System;
using RimHUD.Engine;
using RimHUD.Extensions;
using UnityEngine;
using Verse;

namespace RimHUD.Interface.Dialog;

public sealed class Dialog_Alert : Window
{
  public override Vector2 InitialSize { get; }

  private readonly string _message;
  private readonly Buttons _buttons;
  private readonly Action? _onAccept;
  private readonly Action? _onCancel;

  private bool _isAccepted;

  private Dialog_Alert(string message, Buttons buttons = Buttons.Ok, Action? onAccept = null, Action? onCancel = null)
  {
    doCloseButton = false;
    closeOnAccept = true;
    closeOnClickedOutside = false;
    absorbInputAroundWindow = true;
    draggable = true;

    _message = message;
    _buttons = buttons;
    _onAccept = onAccept;
    _onCancel = onCancel;

    GUIPlus.SetWrap(true);
    GUIPlus.SetFont(GameFont.Small);
    InitialSize = new Vector2(400f, 80f + Text.CalcHeight(_message, 364f));
    GUIPlus.ResetFont();
    GUIPlus.ResetWrap();
  }

  public static void Open(string message, Buttons buttons = Buttons.Ok, Action? onAccept = null, Action? onCancel = null) => Find.WindowStack!.Add(new Dialog_Alert(message, buttons, onAccept, onCancel));

  public override void DoWindowContents(Rect rect)
  {
    var listing = new Listing_Standard();
    var vGrid = rect.GetVGrid(4f, -1f, WidgetsPlus.ButtonHeight);

    listing.Begin(vGrid[1]);
    listing.Label(_message);
    listing.End();

    var hGrid = vGrid[2].GetHGrid(4f, 100f, -1f);

    listing.Begin(_buttons is Buttons.Ok ? vGrid[2] : hGrid[1]);

    if (listing.ButtonText(_buttons is Buttons.YesNo ? Lang.Get("Interface.Button.Yes") : Lang.Get("Interface.Button.OK")))
    {
      _isAccepted = true;
      _onAccept?.Invoke();
      Close();
    }

    listing.End();

    if (_buttons is Buttons.Ok) { return; }

    listing.Begin(hGrid[2]);
    if (listing.ButtonText(_buttons is Buttons.YesNo ? Lang.Get("Interface.Button.No") : Lang.Get("Interface.Button.Cancel"))) { Close(); }
    listing.End();
  }

  public override void Close(bool doCloseSound = true)
  {
    if (!_isAccepted) { _onCancel?.Invoke(); }
    base.Close(doCloseSound);
  }

  public enum Buttons
  {
    Ok,
    YesNo
  }
}
