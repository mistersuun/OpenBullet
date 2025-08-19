// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Wizard
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Xceed.Wpf.Toolkit.Core;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class Wizard : ItemsControl
{
  public static readonly DependencyProperty BackButtonContentProperty = DependencyProperty.Register(nameof (BackButtonContent), typeof (object), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) "< Back"));
  public static readonly DependencyProperty BackButtonVisibilityProperty = DependencyProperty.Register(nameof (BackButtonVisibility), typeof (Visibility), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) Visibility.Visible));
  public static readonly DependencyProperty CanCancelProperty = DependencyProperty.Register(nameof (CanCancel), typeof (bool), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty CancelButtonClosesWindowProperty = DependencyProperty.Register(nameof (CancelButtonClosesWindow), typeof (bool), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty CancelButtonContentProperty = DependencyProperty.Register(nameof (CancelButtonContent), typeof (object), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) "Cancel"));
  public static readonly DependencyProperty CancelButtonVisibilityProperty = DependencyProperty.Register(nameof (CancelButtonVisibility), typeof (Visibility), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) Visibility.Visible));
  public static readonly DependencyProperty CanFinishProperty = DependencyProperty.Register(nameof (CanFinish), typeof (bool), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty CanHelpProperty = DependencyProperty.Register(nameof (CanHelp), typeof (bool), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty CanSelectNextPageProperty = DependencyProperty.Register(nameof (CanSelectNextPage), typeof (bool), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty CanSelectPreviousPageProperty = DependencyProperty.Register(nameof (CanSelectPreviousPage), typeof (bool), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty CurrentPageProperty = DependencyProperty.Register(nameof (CurrentPage), typeof (WizardPage), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(Wizard.OnCurrentPageChanged)));
  public static readonly DependencyProperty ExteriorPanelMinWidthProperty = DependencyProperty.Register(nameof (ExteriorPanelMinWidth), typeof (double), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) 165.0));
  public static readonly DependencyProperty FinishButtonClosesWindowProperty = DependencyProperty.Register(nameof (FinishButtonClosesWindow), typeof (bool), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty FinishButtonContentProperty = DependencyProperty.Register(nameof (FinishButtonContent), typeof (object), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) "Finish"));
  public static readonly DependencyProperty FinishButtonVisibilityProperty = DependencyProperty.Register(nameof (FinishButtonVisibility), typeof (Visibility), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) Visibility.Collapsed));
  public static readonly DependencyProperty HelpButtonContentProperty = DependencyProperty.Register(nameof (HelpButtonContent), typeof (object), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) "Help"));
  public static readonly DependencyProperty HelpButtonVisibilityProperty = DependencyProperty.Register(nameof (HelpButtonVisibility), typeof (Visibility), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) Visibility.Visible));
  public static readonly DependencyProperty NextButtonContentProperty = DependencyProperty.Register(nameof (NextButtonContent), typeof (object), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) "Next >"));
  public static readonly DependencyProperty NextButtonVisibilityProperty = DependencyProperty.Register(nameof (NextButtonVisibility), typeof (Visibility), typeof (Wizard), (PropertyMetadata) new UIPropertyMetadata((object) Visibility.Visible));
  public static readonly RoutedEvent CancelEvent = EventManager.RegisterRoutedEvent("Cancel", RoutingStrategy.Bubble, typeof (EventHandler), typeof (Wizard));
  public static readonly RoutedEvent PageChangedEvent = EventManager.RegisterRoutedEvent("PageChanged", RoutingStrategy.Bubble, typeof (EventHandler), typeof (Wizard));
  public static readonly RoutedEvent FinishEvent = EventManager.RegisterRoutedEvent("Finish", RoutingStrategy.Bubble, typeof (CancelRoutedEventHandler), typeof (Wizard));
  public static readonly RoutedEvent HelpEvent = EventManager.RegisterRoutedEvent("Help", RoutingStrategy.Bubble, typeof (EventHandler), typeof (Wizard));
  public static readonly RoutedEvent NextEvent = EventManager.RegisterRoutedEvent("Next", RoutingStrategy.Bubble, typeof (Wizard.NextRoutedEventHandler), typeof (Wizard));
  public static readonly RoutedEvent PreviousEvent = EventManager.RegisterRoutedEvent("Previous", RoutingStrategy.Bubble, typeof (Wizard.PreviousRoutedEventHandler), typeof (Wizard));

  public object BackButtonContent
  {
    get => this.GetValue(Wizard.BackButtonContentProperty);
    set => this.SetValue(Wizard.BackButtonContentProperty, value);
  }

  public Visibility BackButtonVisibility
  {
    get => (Visibility) this.GetValue(Wizard.BackButtonVisibilityProperty);
    set => this.SetValue(Wizard.BackButtonVisibilityProperty, (object) value);
  }

  public bool CanCancel
  {
    get => (bool) this.GetValue(Wizard.CanCancelProperty);
    set => this.SetValue(Wizard.CanCancelProperty, (object) value);
  }

  public bool CancelButtonClosesWindow
  {
    get => (bool) this.GetValue(Wizard.CancelButtonClosesWindowProperty);
    set => this.SetValue(Wizard.CancelButtonClosesWindowProperty, (object) value);
  }

  public object CancelButtonContent
  {
    get => this.GetValue(Wizard.CancelButtonContentProperty);
    set => this.SetValue(Wizard.CancelButtonContentProperty, value);
  }

  public Visibility CancelButtonVisibility
  {
    get => (Visibility) this.GetValue(Wizard.CancelButtonVisibilityProperty);
    set => this.SetValue(Wizard.CancelButtonVisibilityProperty, (object) value);
  }

  public bool CanFinish
  {
    get => (bool) this.GetValue(Wizard.CanFinishProperty);
    set => this.SetValue(Wizard.CanFinishProperty, (object) value);
  }

  public bool CanHelp
  {
    get => (bool) this.GetValue(Wizard.CanHelpProperty);
    set => this.SetValue(Wizard.CanHelpProperty, (object) value);
  }

  public bool CanSelectNextPage
  {
    get => (bool) this.GetValue(Wizard.CanSelectNextPageProperty);
    set => this.SetValue(Wizard.CanSelectNextPageProperty, (object) value);
  }

  public bool CanSelectPreviousPage
  {
    get => (bool) this.GetValue(Wizard.CanSelectPreviousPageProperty);
    set => this.SetValue(Wizard.CanSelectPreviousPageProperty, (object) value);
  }

  public WizardPage CurrentPage
  {
    get => (WizardPage) this.GetValue(Wizard.CurrentPageProperty);
    set => this.SetValue(Wizard.CurrentPageProperty, (object) value);
  }

  private static void OnCurrentPageChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is Wizard wizard))
      return;
    wizard.OnCurrentPageChanged((WizardPage) e.OldValue, (WizardPage) e.NewValue);
  }

  protected virtual void OnCurrentPageChanged(WizardPage oldValue, WizardPage newValue)
  {
    this.RaiseRoutedEvent(Wizard.PageChangedEvent);
  }

  public double ExteriorPanelMinWidth
  {
    get => (double) this.GetValue(Wizard.ExteriorPanelMinWidthProperty);
    set => this.SetValue(Wizard.ExteriorPanelMinWidthProperty, (object) value);
  }

  public bool FinishButtonClosesWindow
  {
    get => (bool) this.GetValue(Wizard.FinishButtonClosesWindowProperty);
    set => this.SetValue(Wizard.FinishButtonClosesWindowProperty, (object) value);
  }

  public object FinishButtonContent
  {
    get => this.GetValue(Wizard.FinishButtonContentProperty);
    set => this.SetValue(Wizard.FinishButtonContentProperty, value);
  }

  public Visibility FinishButtonVisibility
  {
    get => (Visibility) this.GetValue(Wizard.FinishButtonVisibilityProperty);
    set => this.SetValue(Wizard.FinishButtonVisibilityProperty, (object) value);
  }

  public object HelpButtonContent
  {
    get => this.GetValue(Wizard.HelpButtonContentProperty);
    set => this.SetValue(Wizard.HelpButtonContentProperty, value);
  }

  public Visibility HelpButtonVisibility
  {
    get => (Visibility) this.GetValue(Wizard.HelpButtonVisibilityProperty);
    set => this.SetValue(Wizard.HelpButtonVisibilityProperty, (object) value);
  }

  public object NextButtonContent
  {
    get => this.GetValue(Wizard.NextButtonContentProperty);
    set => this.SetValue(Wizard.NextButtonContentProperty, value);
  }

  public Visibility NextButtonVisibility
  {
    get => (Visibility) this.GetValue(Wizard.NextButtonVisibilityProperty);
    set => this.SetValue(Wizard.NextButtonVisibilityProperty, (object) value);
  }

  static Wizard()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (Wizard), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (Wizard)));
  }

  public Wizard()
  {
    this.CommandBindings.Add(new CommandBinding((ICommand) WizardCommands.Cancel, new ExecutedRoutedEventHandler(this.ExecuteCancelWizard), new CanExecuteRoutedEventHandler(this.CanExecuteCancelWizard)));
    this.CommandBindings.Add(new CommandBinding((ICommand) WizardCommands.Finish, new ExecutedRoutedEventHandler(this.ExecuteFinishWizard), new CanExecuteRoutedEventHandler(this.CanExecuteFinishWizard)));
    this.CommandBindings.Add(new CommandBinding((ICommand) WizardCommands.Help, new ExecutedRoutedEventHandler(this.ExecuteRequestHelp), new CanExecuteRoutedEventHandler(this.CanExecuteRequestHelp)));
    this.CommandBindings.Add(new CommandBinding((ICommand) WizardCommands.NextPage, new ExecutedRoutedEventHandler(this.ExecuteSelectNextPage), new CanExecuteRoutedEventHandler(this.CanExecuteSelectNextPage)));
    this.CommandBindings.Add(new CommandBinding((ICommand) WizardCommands.PreviousPage, new ExecutedRoutedEventHandler(this.ExecuteSelectPreviousPage), new CanExecuteRoutedEventHandler(this.CanExecuteSelectPreviousPage)));
  }

  protected override DependencyObject GetContainerForItemOverride()
  {
    return (DependencyObject) new WizardPage();
  }

  protected override bool IsItemItsOwnContainerOverride(object item) => item is WizardPage;

  protected override void OnInitialized(EventArgs e)
  {
    base.OnInitialized(e);
    if (this.Items.Count <= 0 || this.CurrentPage != null)
      return;
    this.CurrentPage = this.Items[0] as WizardPage;
  }

  protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
  {
    base.OnItemsChanged(e);
    foreach (object obj in (IEnumerable) this.Items)
    {
      if (!(obj is WizardPage))
        throw new NotSupportedException("Wizard should only contain WizardPages.");
    }
    if (this.Items.Count <= 0 || this.CurrentPage != null)
      return;
    this.CurrentPage = this.Items[0] as WizardPage;
  }

  protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
  {
    base.OnPropertyChanged(e);
    if (!(e.Property.Name == "CanSelectNextPage") && !(e.Property.Name == "CanHelp") && !(e.Property.Name == "CanFinish") && !(e.Property.Name == "CanCancel") && !(e.Property.Name == "CanSelectPreviousPage"))
      return;
    CommandManager.InvalidateRequerySuggested();
  }

  private void ExecuteCancelWizard(object sender, ExecutedRoutedEventArgs e)
  {
    this.RaiseRoutedEvent(Wizard.CancelEvent);
    if (!this.CancelButtonClosesWindow)
      return;
    this.CloseParentWindow(false);
  }

  private void CanExecuteCancelWizard(object sender, CanExecuteRoutedEventArgs e)
  {
    if (this.CurrentPage == null)
      return;
    if (this.CurrentPage.CanCancel.HasValue)
      e.CanExecute = this.CurrentPage.CanCancel.Value;
    else
      e.CanExecute = this.CanCancel;
  }

  private void ExecuteFinishWizard(object sender, ExecutedRoutedEventArgs e)
  {
    CancelRoutedEventArgs e1 = new CancelRoutedEventArgs(Wizard.FinishEvent);
    this.RaiseEvent((RoutedEventArgs) e1);
    if (e1.Cancel || !this.FinishButtonClosesWindow)
      return;
    this.CloseParentWindow(true);
  }

  private void CanExecuteFinishWizard(object sender, CanExecuteRoutedEventArgs e)
  {
    if (this.CurrentPage == null)
      return;
    if (this.CurrentPage.CanFinish.HasValue)
      e.CanExecute = this.CurrentPage.CanFinish.Value;
    else
      e.CanExecute = this.CanFinish;
  }

  private void ExecuteRequestHelp(object sender, ExecutedRoutedEventArgs e)
  {
    this.RaiseRoutedEvent(Wizard.HelpEvent);
  }

  private void CanExecuteRequestHelp(object sender, CanExecuteRoutedEventArgs e)
  {
    if (this.CurrentPage == null)
      return;
    if (this.CurrentPage.CanHelp.HasValue)
      e.CanExecute = this.CurrentPage.CanHelp.Value;
    else
      e.CanExecute = this.CanHelp;
  }

  private void ExecuteSelectNextPage(object sender, ExecutedRoutedEventArgs e)
  {
    WizardPage wizardPage = (WizardPage) null;
    if (this.CurrentPage != null)
    {
      CancelRoutedEventArgs e1 = new CancelRoutedEventArgs(Wizard.NextEvent);
      this.RaiseEvent((RoutedEventArgs) e1);
      if (e1.Cancel)
        return;
      if (this.CurrentPage.NextPage != null)
      {
        wizardPage = this.CurrentPage.NextPage;
      }
      else
      {
        int index = this.Items.IndexOf((object) this.CurrentPage) + 1;
        if (index < this.Items.Count)
          wizardPage = this.Items[index] as WizardPage;
      }
    }
    this.CurrentPage = wizardPage;
  }

  private void CanExecuteSelectNextPage(object sender, CanExecuteRoutedEventArgs e)
  {
    if (this.CurrentPage == null)
      return;
    if (this.CurrentPage.CanSelectNextPage.HasValue)
    {
      if (!this.CurrentPage.CanSelectNextPage.Value)
        return;
      e.CanExecute = this.NextPageExists();
    }
    else
    {
      if (!this.CanSelectNextPage)
        return;
      e.CanExecute = this.NextPageExists();
    }
  }

  private void ExecuteSelectPreviousPage(object sender, ExecutedRoutedEventArgs e)
  {
    WizardPage wizardPage = (WizardPage) null;
    if (this.CurrentPage != null)
    {
      CancelRoutedEventArgs e1 = new CancelRoutedEventArgs(Wizard.PreviousEvent);
      this.RaiseEvent((RoutedEventArgs) e1);
      if (e1.Cancel)
        return;
      if (this.CurrentPage.PreviousPage != null)
      {
        wizardPage = this.CurrentPage.PreviousPage;
      }
      else
      {
        int index = this.Items.IndexOf((object) this.CurrentPage) - 1;
        if (index >= 0 && index < this.Items.Count)
          wizardPage = this.Items[index] as WizardPage;
      }
    }
    this.CurrentPage = wizardPage;
  }

  private void CanExecuteSelectPreviousPage(object sender, CanExecuteRoutedEventArgs e)
  {
    if (this.CurrentPage == null)
      return;
    if (this.CurrentPage.CanSelectPreviousPage.HasValue)
    {
      if (!this.CurrentPage.CanSelectPreviousPage.Value)
        return;
      e.CanExecute = this.PreviousPageExists();
    }
    else
    {
      if (!this.CanSelectPreviousPage)
        return;
      e.CanExecute = this.PreviousPageExists();
    }
  }

  public event RoutedEventHandler Cancel
  {
    add => this.AddHandler(Wizard.CancelEvent, (Delegate) value);
    remove => this.RemoveHandler(Wizard.CancelEvent, (Delegate) value);
  }

  public event RoutedEventHandler PageChanged
  {
    add => this.AddHandler(Wizard.PageChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(Wizard.PageChangedEvent, (Delegate) value);
  }

  public event CancelRoutedEventHandler Finish
  {
    add => this.AddHandler(Wizard.FinishEvent, (Delegate) value);
    remove => this.RemoveHandler(Wizard.FinishEvent, (Delegate) value);
  }

  public event RoutedEventHandler Help
  {
    add => this.AddHandler(Wizard.HelpEvent, (Delegate) value);
    remove => this.RemoveHandler(Wizard.HelpEvent, (Delegate) value);
  }

  public event Wizard.NextRoutedEventHandler Next
  {
    add => this.AddHandler(Wizard.NextEvent, (Delegate) value);
    remove => this.RemoveHandler(Wizard.NextEvent, (Delegate) value);
  }

  public event Wizard.PreviousRoutedEventHandler Previous
  {
    add => this.AddHandler(Wizard.PreviousEvent, (Delegate) value);
    remove => this.RemoveHandler(Wizard.PreviousEvent, (Delegate) value);
  }

  private void CloseParentWindow(bool dialogResult)
  {
    Window window = Window.GetWindow((DependencyObject) this);
    if (window == null)
      return;
    if (ComponentDispatcher.IsThreadModal)
      window.DialogResult = new bool?(dialogResult);
    window.Close();
  }

  private bool NextPageExists()
  {
    bool flag = false;
    if (this.CurrentPage.NextPage != null)
      flag = true;
    else if (this.Items.IndexOf((object) this.CurrentPage) + 1 < this.Items.Count)
      flag = true;
    return flag;
  }

  private bool PreviousPageExists()
  {
    bool flag = false;
    if (this.CurrentPage.PreviousPage != null)
    {
      flag = true;
    }
    else
    {
      int num = this.Items.IndexOf((object) this.CurrentPage) - 1;
      if (num >= 0 && num < this.Items.Count)
        flag = true;
    }
    return flag;
  }

  private void RaiseRoutedEvent(RoutedEvent routedEvent)
  {
    this.RaiseEvent(new RoutedEventArgs(routedEvent, (object) this));
  }

  public delegate void NextRoutedEventHandler(object sender, CancelRoutedEventArgs e);

  public delegate void PreviousRoutedEventHandler(object sender, CancelRoutedEventArgs e);
}
