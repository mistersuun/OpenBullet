// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.AvalonEditCommands
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System.Windows.Input;

#nullable disable
namespace ICSharpCode.AvalonEdit;

public static class AvalonEditCommands
{
  public static readonly RoutedCommand ToggleOverstrike = new RoutedCommand(nameof (ToggleOverstrike), typeof (TextEditor), new InputGestureCollection()
  {
    (InputGesture) new KeyGesture(Key.Insert)
  });
  public static readonly RoutedCommand DeleteLine = new RoutedCommand(nameof (DeleteLine), typeof (TextEditor), new InputGestureCollection()
  {
    (InputGesture) new KeyGesture(Key.D, ModifierKeys.Control)
  });
  public static readonly RoutedCommand RemoveLeadingWhitespace = new RoutedCommand(nameof (RemoveLeadingWhitespace), typeof (TextEditor));
  public static readonly RoutedCommand RemoveTrailingWhitespace = new RoutedCommand(nameof (RemoveTrailingWhitespace), typeof (TextEditor));
  public static readonly RoutedCommand ConvertToUppercase = new RoutedCommand(nameof (ConvertToUppercase), typeof (TextEditor));
  public static readonly RoutedCommand ConvertToLowercase = new RoutedCommand(nameof (ConvertToLowercase), typeof (TextEditor));
  public static readonly RoutedCommand ConvertToTitleCase = new RoutedCommand(nameof (ConvertToTitleCase), typeof (TextEditor));
  public static readonly RoutedCommand InvertCase = new RoutedCommand(nameof (InvertCase), typeof (TextEditor));
  public static readonly RoutedCommand ConvertTabsToSpaces = new RoutedCommand(nameof (ConvertTabsToSpaces), typeof (TextEditor));
  public static readonly RoutedCommand ConvertSpacesToTabs = new RoutedCommand(nameof (ConvertSpacesToTabs), typeof (TextEditor));
  public static readonly RoutedCommand ConvertLeadingTabsToSpaces = new RoutedCommand(nameof (ConvertLeadingTabsToSpaces), typeof (TextEditor));
  public static readonly RoutedCommand ConvertLeadingSpacesToTabs = new RoutedCommand(nameof (ConvertLeadingSpacesToTabs), typeof (TextEditor));
  public static readonly RoutedCommand IndentSelection = new RoutedCommand(nameof (IndentSelection), typeof (TextEditor), new InputGestureCollection()
  {
    (InputGesture) new KeyGesture(Key.I, ModifierKeys.Control)
  });
}
