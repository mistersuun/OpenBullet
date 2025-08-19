// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.WizardPage
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class WizardPage : ContentControl
{
  public static readonly DependencyProperty BackButtonVisibilityProperty = DependencyProperty.Register(nameof (BackButtonVisibility), typeof (WizardPageButtonVisibility), typeof (WizardPage), (PropertyMetadata) new UIPropertyMetadata((object) WizardPageButtonVisibility.Inherit));
  public static readonly DependencyProperty CanCancelProperty = DependencyProperty.Register(nameof (CanCancel), typeof (bool?), typeof (WizardPage), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty CancelButtonVisibilityProperty = DependencyProperty.Register(nameof (CancelButtonVisibility), typeof (WizardPageButtonVisibility), typeof (WizardPage), (PropertyMetadata) new UIPropertyMetadata((object) WizardPageButtonVisibility.Inherit));
  public static readonly DependencyProperty CanFinishProperty = DependencyProperty.Register(nameof (CanFinish), typeof (bool?), typeof (WizardPage), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty CanHelpProperty = DependencyProperty.Register(nameof (CanHelp), typeof (bool?), typeof (WizardPage), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty CanSelectNextPageProperty = DependencyProperty.Register(nameof (CanSelectNextPage), typeof (bool?), typeof (WizardPage), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty CanSelectPreviousPageProperty = DependencyProperty.Register(nameof (CanSelectPreviousPage), typeof (bool?), typeof (WizardPage), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof (Description), typeof (string), typeof (WizardPage));
  public static readonly DependencyProperty ExteriorPanelBackgroundProperty = DependencyProperty.Register(nameof (ExteriorPanelBackground), typeof (Brush), typeof (WizardPage), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty ExteriorPanelContentProperty = DependencyProperty.Register(nameof (ExteriorPanelContent), typeof (object), typeof (WizardPage), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty FinishButtonVisibilityProperty = DependencyProperty.Register(nameof (FinishButtonVisibility), typeof (WizardPageButtonVisibility), typeof (WizardPage), (PropertyMetadata) new UIPropertyMetadata((object) WizardPageButtonVisibility.Inherit));
  public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.Register(nameof (HeaderBackground), typeof (Brush), typeof (WizardPage), (PropertyMetadata) new UIPropertyMetadata((object) Brushes.White));
  public static readonly DependencyProperty HeaderImageProperty = DependencyProperty.Register(nameof (HeaderImage), typeof (ImageSource), typeof (WizardPage), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty HelpButtonVisibilityProperty = DependencyProperty.Register(nameof (HelpButtonVisibility), typeof (WizardPageButtonVisibility), typeof (WizardPage), (PropertyMetadata) new UIPropertyMetadata((object) WizardPageButtonVisibility.Inherit));
  public static readonly DependencyProperty NextButtonVisibilityProperty = DependencyProperty.Register(nameof (NextButtonVisibility), typeof (WizardPageButtonVisibility), typeof (WizardPage), (PropertyMetadata) new UIPropertyMetadata((object) WizardPageButtonVisibility.Inherit));
  public static readonly DependencyProperty NextPageProperty = DependencyProperty.Register(nameof (NextPage), typeof (WizardPage), typeof (WizardPage), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty PageTypeProperty = DependencyProperty.Register(nameof (PageType), typeof (WizardPageType), typeof (WizardPage), (PropertyMetadata) new UIPropertyMetadata((object) WizardPageType.Exterior));
  public static readonly DependencyProperty PreviousPageProperty = DependencyProperty.Register(nameof (PreviousPage), typeof (WizardPage), typeof (WizardPage), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof (Title), typeof (string), typeof (WizardPage));
  public static readonly RoutedEvent EnterEvent = EventManager.RegisterRoutedEvent("Enter", RoutingStrategy.Bubble, typeof (EventHandler), typeof (WizardPage));
  public static readonly RoutedEvent LeaveEvent = EventManager.RegisterRoutedEvent("Leave", RoutingStrategy.Bubble, typeof (EventHandler), typeof (WizardPage));

  public WizardPageButtonVisibility BackButtonVisibility
  {
    get => (WizardPageButtonVisibility) this.GetValue(WizardPage.BackButtonVisibilityProperty);
    set => this.SetValue(WizardPage.BackButtonVisibilityProperty, (object) value);
  }

  public bool? CanCancel
  {
    get => (bool?) this.GetValue(WizardPage.CanCancelProperty);
    set => this.SetValue(WizardPage.CanCancelProperty, (object) value);
  }

  public WizardPageButtonVisibility CancelButtonVisibility
  {
    get => (WizardPageButtonVisibility) this.GetValue(WizardPage.CancelButtonVisibilityProperty);
    set => this.SetValue(WizardPage.CancelButtonVisibilityProperty, (object) value);
  }

  public bool? CanFinish
  {
    get => (bool?) this.GetValue(WizardPage.CanFinishProperty);
    set => this.SetValue(WizardPage.CanFinishProperty, (object) value);
  }

  public bool? CanHelp
  {
    get => (bool?) this.GetValue(WizardPage.CanHelpProperty);
    set => this.SetValue(WizardPage.CanHelpProperty, (object) value);
  }

  public bool? CanSelectNextPage
  {
    get => (bool?) this.GetValue(WizardPage.CanSelectNextPageProperty);
    set => this.SetValue(WizardPage.CanSelectNextPageProperty, (object) value);
  }

  public bool? CanSelectPreviousPage
  {
    get => (bool?) this.GetValue(WizardPage.CanSelectPreviousPageProperty);
    set => this.SetValue(WizardPage.CanSelectPreviousPageProperty, (object) value);
  }

  public string Description
  {
    get => (string) this.GetValue(WizardPage.DescriptionProperty);
    set => this.SetValue(WizardPage.DescriptionProperty, (object) value);
  }

  public Brush ExteriorPanelBackground
  {
    get => (Brush) this.GetValue(WizardPage.ExteriorPanelBackgroundProperty);
    set => this.SetValue(WizardPage.ExteriorPanelBackgroundProperty, (object) value);
  }

  public object ExteriorPanelContent
  {
    get => this.GetValue(WizardPage.ExteriorPanelContentProperty);
    set => this.SetValue(WizardPage.ExteriorPanelContentProperty, value);
  }

  public WizardPageButtonVisibility FinishButtonVisibility
  {
    get => (WizardPageButtonVisibility) this.GetValue(WizardPage.FinishButtonVisibilityProperty);
    set => this.SetValue(WizardPage.FinishButtonVisibilityProperty, (object) value);
  }

  public Brush HeaderBackground
  {
    get => (Brush) this.GetValue(WizardPage.HeaderBackgroundProperty);
    set => this.SetValue(WizardPage.HeaderBackgroundProperty, (object) value);
  }

  public ImageSource HeaderImage
  {
    get => (ImageSource) this.GetValue(WizardPage.HeaderImageProperty);
    set => this.SetValue(WizardPage.HeaderImageProperty, (object) value);
  }

  public WizardPageButtonVisibility HelpButtonVisibility
  {
    get => (WizardPageButtonVisibility) this.GetValue(WizardPage.HelpButtonVisibilityProperty);
    set => this.SetValue(WizardPage.HelpButtonVisibilityProperty, (object) value);
  }

  public WizardPageButtonVisibility NextButtonVisibility
  {
    get => (WizardPageButtonVisibility) this.GetValue(WizardPage.NextButtonVisibilityProperty);
    set => this.SetValue(WizardPage.NextButtonVisibilityProperty, (object) value);
  }

  public WizardPage NextPage
  {
    get => (WizardPage) this.GetValue(WizardPage.NextPageProperty);
    set => this.SetValue(WizardPage.NextPageProperty, (object) value);
  }

  public WizardPageType PageType
  {
    get => (WizardPageType) this.GetValue(WizardPage.PageTypeProperty);
    set => this.SetValue(WizardPage.PageTypeProperty, (object) value);
  }

  public WizardPage PreviousPage
  {
    get => (WizardPage) this.GetValue(WizardPage.PreviousPageProperty);
    set => this.SetValue(WizardPage.PreviousPageProperty, (object) value);
  }

  public string Title
  {
    get => (string) this.GetValue(WizardPage.TitleProperty);
    set => this.SetValue(WizardPage.TitleProperty, (object) value);
  }

  static WizardPage()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (WizardPage), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (WizardPage)));
  }

  public WizardPage()
  {
    this.Loaded += new RoutedEventHandler(this.WizardPage_Loaded);
    this.Unloaded += new RoutedEventHandler(this.WizardPage_Unloaded);
  }

  private void WizardPage_Unloaded(object sender, RoutedEventArgs e)
  {
    this.RaiseEvent(new RoutedEventArgs(WizardPage.LeaveEvent, (object) this));
  }

  private void WizardPage_Loaded(object sender, RoutedEventArgs e)
  {
    if (!this.IsVisible)
      return;
    this.RaiseEvent(new RoutedEventArgs(WizardPage.EnterEvent, (object) this));
  }

  protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
  {
    base.OnPropertyChanged(e);
    if (!(e.Property.Name == "CanSelectNextPage") && !(e.Property.Name == "CanHelp") && !(e.Property.Name == "CanFinish") && !(e.Property.Name == "CanCancel") && !(e.Property.Name == "CanSelectPreviousPage"))
      return;
    CommandManager.InvalidateRequerySuggested();
  }

  public event RoutedEventHandler Enter
  {
    add => this.AddHandler(WizardPage.EnterEvent, (Delegate) value);
    remove => this.RemoveHandler(WizardPage.EnterEvent, (Delegate) value);
  }

  public event RoutedEventHandler Leave
  {
    add => this.AddHandler(WizardPage.LeaveEvent, (Delegate) value);
    remove => this.RemoveHandler(WizardPage.LeaveEvent, (Delegate) value);
  }
}
