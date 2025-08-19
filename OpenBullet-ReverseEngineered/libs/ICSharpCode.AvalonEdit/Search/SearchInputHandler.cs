// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Search.SearchInputHandler
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Threading;

#nullable disable
namespace ICSharpCode.AvalonEdit.Search;

public class SearchInputHandler : TextAreaInputHandler
{
  private SearchPanel panel;

  [Obsolete("Use SearchPanel.Install instead")]
  public SearchInputHandler(TextArea textArea)
    : base(textArea)
  {
    this.RegisterCommands(this.CommandBindings);
    this.panel = SearchPanel.Install(textArea);
  }

  internal SearchInputHandler(TextArea textArea, SearchPanel panel)
    : base(textArea)
  {
    this.RegisterCommands(this.CommandBindings);
    this.panel = panel;
  }

  internal void RegisterGlobalCommands(CommandBindingCollection commandBindings)
  {
    commandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.Find, new ExecutedRoutedEventHandler(this.ExecuteFind)));
    commandBindings.Add(new CommandBinding((ICommand) SearchCommands.FindNext, new ExecutedRoutedEventHandler(this.ExecuteFindNext), new CanExecuteRoutedEventHandler(this.CanExecuteWithOpenSearchPanel)));
    commandBindings.Add(new CommandBinding((ICommand) SearchCommands.FindPrevious, new ExecutedRoutedEventHandler(this.ExecuteFindPrevious), new CanExecuteRoutedEventHandler(this.CanExecuteWithOpenSearchPanel)));
  }

  private void RegisterCommands(ICollection<CommandBinding> commandBindings)
  {
    commandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.Find, new ExecutedRoutedEventHandler(this.ExecuteFind)));
    commandBindings.Add(new CommandBinding((ICommand) SearchCommands.FindNext, new ExecutedRoutedEventHandler(this.ExecuteFindNext), new CanExecuteRoutedEventHandler(this.CanExecuteWithOpenSearchPanel)));
    commandBindings.Add(new CommandBinding((ICommand) SearchCommands.FindPrevious, new ExecutedRoutedEventHandler(this.ExecuteFindPrevious), new CanExecuteRoutedEventHandler(this.CanExecuteWithOpenSearchPanel)));
    commandBindings.Add(new CommandBinding((ICommand) SearchCommands.CloseSearchPanel, new ExecutedRoutedEventHandler(this.ExecuteCloseSearchPanel), new CanExecuteRoutedEventHandler(this.CanExecuteWithOpenSearchPanel)));
  }

  private void ExecuteFind(object sender, ExecutedRoutedEventArgs e)
  {
    this.panel.Open();
    if (!this.TextArea.Selection.IsEmpty && !this.TextArea.Selection.IsMultiline)
      this.panel.SearchPattern = this.TextArea.Selection.GetText();
    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, (Delegate) (() => this.panel.Reactivate()));
  }

  private void CanExecuteWithOpenSearchPanel(object sender, CanExecuteRoutedEventArgs e)
  {
    if (this.panel.IsClosed)
    {
      e.CanExecute = false;
      e.ContinueRouting = true;
    }
    else
    {
      e.CanExecute = true;
      e.Handled = true;
    }
  }

  private void ExecuteFindNext(object sender, ExecutedRoutedEventArgs e)
  {
    if (this.panel.IsClosed)
      return;
    this.panel.FindNext();
    e.Handled = true;
  }

  private void ExecuteFindPrevious(object sender, ExecutedRoutedEventArgs e)
  {
    if (this.panel.IsClosed)
      return;
    this.panel.FindPrevious();
    e.Handled = true;
  }

  private void ExecuteCloseSearchPanel(object sender, ExecutedRoutedEventArgs e)
  {
    if (this.panel.IsClosed)
      return;
    this.panel.Close();
    e.Handled = true;
  }

  public event EventHandler<SearchOptionsChangedEventArgs> SearchOptionsChanged
  {
    add => this.panel.SearchOptionsChanged += value;
    remove => this.panel.SearchOptionsChanged -= value;
  }
}
