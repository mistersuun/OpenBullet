// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.WizardCommands
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows.Input;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public static class WizardCommands
{
  private static RoutedCommand _cancelCommand = new RoutedCommand();
  private static RoutedCommand _finishCommand = new RoutedCommand();
  private static RoutedCommand _helpCommand = new RoutedCommand();
  private static RoutedCommand _nextPageCommand = new RoutedCommand();
  private static RoutedCommand _previousPageCommand = new RoutedCommand();
  private static RoutedCommand _selectPageCommand = new RoutedCommand();

  public static RoutedCommand Cancel => WizardCommands._cancelCommand;

  public static RoutedCommand Finish => WizardCommands._finishCommand;

  public static RoutedCommand Help => WizardCommands._helpCommand;

  public static RoutedCommand NextPage => WizardCommands._nextPageCommand;

  public static RoutedCommand PreviousPage => WizardCommands._previousPageCommand;

  public static RoutedCommand SelectPage => WizardCommands._selectPageCommand;
}
