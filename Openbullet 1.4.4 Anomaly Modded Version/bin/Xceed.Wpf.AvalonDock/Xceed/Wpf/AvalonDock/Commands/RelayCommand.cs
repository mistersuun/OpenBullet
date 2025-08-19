// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Commands.RelayCommand
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Windows.Input;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Commands;

internal class RelayCommand : ICommand
{
  private readonly Action<object> _execute;
  private readonly Predicate<object> _canExecute;

  public RelayCommand(Action<object> execute)
    : this(execute, (Predicate<object>) null)
  {
  }

  public RelayCommand(Action<object> execute, Predicate<object> canExecute)
  {
    this._execute = execute != null ? execute : throw new ArgumentNullException(nameof (execute));
    this._canExecute = canExecute;
  }

  public bool CanExecute(object parameter)
  {
    return this._canExecute == null || this._canExecute(parameter);
  }

  public event EventHandler CanExecuteChanged
  {
    add => CommandManager.RequerySuggested += value;
    remove => CommandManager.RequerySuggested -= value;
  }

  public void Execute(object parameter) => this._execute(parameter);
}
