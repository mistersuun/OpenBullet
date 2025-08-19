// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Search.SearchCommands
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System.Windows.Input;

#nullable disable
namespace ICSharpCode.AvalonEdit.Search;

public static class SearchCommands
{
  public static readonly RoutedCommand FindNext = new RoutedCommand(nameof (FindNext), typeof (SearchPanel), new InputGestureCollection()
  {
    (InputGesture) new KeyGesture(Key.F3)
  });
  public static readonly RoutedCommand FindPrevious = new RoutedCommand(nameof (FindPrevious), typeof (SearchPanel), new InputGestureCollection()
  {
    (InputGesture) new KeyGesture(Key.F3, ModifierKeys.Shift)
  });
  public static readonly RoutedCommand CloseSearchPanel = new RoutedCommand(nameof (CloseSearchPanel), typeof (SearchPanel), new InputGestureCollection()
  {
    (InputGesture) new KeyGesture(Key.Escape)
  });
}
