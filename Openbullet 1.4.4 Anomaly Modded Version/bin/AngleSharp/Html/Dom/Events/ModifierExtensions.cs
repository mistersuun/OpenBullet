// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.Events.ModifierExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

#nullable disable
namespace AngleSharp.Html.Dom.Events;

internal static class ModifierExtensions
{
  public static bool IsCtrlPressed(this string modifierList) => false;

  public static bool IsMetaPressed(this string modifierList) => false;

  public static bool IsShiftPressed(this string modifierList) => false;

  public static bool IsAltPressed(this string modifierList) => false;

  public static bool ContainsKey(this string modifierList, string key)
  {
    return modifierList.Contains(key);
  }
}
