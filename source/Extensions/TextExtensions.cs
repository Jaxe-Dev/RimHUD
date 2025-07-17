using System;
using System.Text;
using RimHUD.Configuration;
using RimHUD.Interface;
using RimHUD.Interface.Hud.Models;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimHUD.Extensions;

public static class TextExtensions
{
  public static string Italic(this string? self) => $"<i>{self}</i>";
  public static string Bold(this string self) => $"<b>{self}</b>";
  public static string ColorizeHex(this string self, string hex) => $"<color=#{hex}>{self}</color>";
  public static string ColorizeByDefMod(this string text, Def? def) => def?.modContentPack?.IsOfficialMod ?? true ? text : text.Colorize(Theme.ExternalModColor);

  public static string Size(this string self, int size) => $"<size={size}>{self}</size>";
  public static string SmallSize(this string self) => self.Size(WidgetsPlus.SmallFontSize);

  public static string? WithDefault(this string? self, string? @default) => self.NullOrWhitespace() ? @default : self;
  public static string? WithValue(this string? self, string? value) => self.NullOrWhitespace() || value.NullOrWhitespace() ? null : $"{self}: {value}";

  public static string ToStringTrimmed(this StringBuilder self) => self.ToString().TrimEndNewlines()!.Trim();
  public static string? ToStringTrimmedOrNull(this StringBuilder self) => self.Length > 0 ? self.ToStringTrimmed() : null;

  public static int? ToInt(this string self) => int.TryParse(self, out var result) ? result : null;
  public static bool? ToBool(this string self) => bool.TryParse(self, out var result) ? result : null;
  public static T? ToEnum<T>(this string self) where T : struct => Enum.TryParse<T>(self, out var result) ? result : null;
  public static string ToDecimalString(this int self, int remainder) => !Theme.ShowDecimals.Value ? self.ToString().Bold() : $"{self.ToString().Bold()}.{(remainder >= 100 ? "99" : remainder.ToString("D2")).Size(Theme.SmallTextStyle.ActualSize)}";
  public static int ToPercentageInt(this float self) => Mathf.RoundToInt(self * 100f);
  public static float ToPercentageFloat(this int self) => self / 100f;

  public static string FlattenWithSeparator(this string text, string separator)
  {
    if (text.NullOrWhitespace()) { return text; }
    text = text.Replace("\r", string.Empty);

    return string.Join(separator, text.Split('\n', StringSplitOptions.RemoveEmptyEntries));
  }

  public static void AppendLineIfNotEmpty(this StringBuilder self, string? text)
  {
    if (!text.NullOrWhitespace()) { self.AppendLine(text); }
  }

  public static void AppendValue(this StringBuilder self, string? label, string? value)
  {
    if (!label.NullOrWhitespace() && !value.NullOrWhitespace()) { self.AppendLine(label.WithValue(value)); }
  }

  public static void AppendStatLine(this StringBuilder self, StatDef def)
  {
    if (def.Worker?.IsDisabledFor(Active.Pawn) ?? true) { return; }
    self.AppendValue(def.LabelCap, def.ValueToString(Active.Pawn.GetStatValue(def)));
  }

  public static void AppendNeedLine(this StringBuilder self, Pawn pawn, NeedDef def)
  {
    if (pawn.needs is null) { return; }
    self.AppendValue(def.LabelCap, pawn.needs.TryGetNeed(def)?.CurLevelPercentage.ToStringPercent());
  }

  public static bool NullOrWhitespace(this string? self) => string.IsNullOrWhiteSpace(self);
  public static bool NullOrWhitespace(this TaggedString self) => self.RawText.NullOrWhitespace();
}
