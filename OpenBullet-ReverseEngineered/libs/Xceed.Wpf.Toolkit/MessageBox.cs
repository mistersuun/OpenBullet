// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.MessageBox
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[TemplateVisualState(Name = "OK", GroupName = "MessageBoxButtonsGroup")]
[TemplateVisualState(Name = "OKCancel", GroupName = "MessageBoxButtonsGroup")]
[TemplateVisualState(Name = "YesNo", GroupName = "MessageBoxButtonsGroup")]
[TemplateVisualState(Name = "YesNoCancel", GroupName = "MessageBoxButtonsGroup")]
[TemplatePart(Name = "PART_CancelButton", Type = typeof (Button))]
[TemplatePart(Name = "PART_NoButton", Type = typeof (Button))]
[TemplatePart(Name = "PART_OkButton", Type = typeof (Button))]
[TemplatePart(Name = "PART_YesButton", Type = typeof (Button))]
[TemplatePart(Name = "PART_WindowControl", Type = typeof (WindowControl))]
public class MessageBox : WindowControl
{
  private const string PART_CancelButton = "PART_CancelButton";
  private const string PART_NoButton = "PART_NoButton";
  private const string PART_OkButton = "PART_OkButton";
  private const string PART_YesButton = "PART_YesButton";
  private const string PART_CloseButton = "PART_CloseButton";
  private const string PART_WindowControl = "PART_WindowControl";
  private MessageBoxButton _button;
  private MessageBoxResult _defaultResult;
  private MessageBoxResult _dialogResult;
  private Window _owner;
  private IntPtr _ownerHandle;
  private WindowControl _windowControl;
  public static readonly DependencyProperty ButtonRegionBackgroundProperty = DependencyProperty.Register(nameof (ButtonRegionBackground), typeof (Brush), typeof (MessageBox), new PropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty CancelButtonContentProperty = DependencyProperty.Register(nameof (CancelButtonContent), typeof (object), typeof (MessageBox), (PropertyMetadata) new UIPropertyMetadata((object) "Cancel"));
  public static readonly DependencyProperty CancelButtonStyleProperty = DependencyProperty.Register(nameof (CancelButtonStyle), typeof (Style), typeof (MessageBox), new PropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof (ImageSource), typeof (ImageSource), typeof (MessageBox), (PropertyMetadata) new UIPropertyMetadata((object) null));
  public static readonly DependencyProperty OkButtonContentProperty = DependencyProperty.Register(nameof (OkButtonContent), typeof (object), typeof (MessageBox), (PropertyMetadata) new UIPropertyMetadata((object) "OK"));
  public static readonly DependencyProperty OkButtonStyleProperty = DependencyProperty.Register(nameof (OkButtonStyle), typeof (Style), typeof (MessageBox), new PropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty NoButtonContentProperty = DependencyProperty.Register(nameof (NoButtonContent), typeof (object), typeof (MessageBox), (PropertyMetadata) new UIPropertyMetadata((object) "No"));
  public static readonly DependencyProperty NoButtonStyleProperty = DependencyProperty.Register(nameof (NoButtonStyle), typeof (Style), typeof (MessageBox), new PropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof (Text), typeof (string), typeof (MessageBox), (PropertyMetadata) new UIPropertyMetadata((object) string.Empty));
  public static readonly DependencyProperty YesButtonContentProperty = DependencyProperty.Register(nameof (YesButtonContent), typeof (object), typeof (MessageBox), (PropertyMetadata) new UIPropertyMetadata((object) "Yes"));
  public static readonly DependencyProperty YesButtonStyleProperty = DependencyProperty.Register(nameof (YesButtonStyle), typeof (Style), typeof (MessageBox), new PropertyMetadata((PropertyChangedCallback) null));

  static MessageBox()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (MessageBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (MessageBox)));
  }

  public MessageBox()
  {
    this.Visibility = Visibility.Collapsed;
    this.InitHandlers();
    this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.MessageBox_IsVisibleChanged);
  }

  protected Window Container => this.Parent as Window;

  public Brush ButtonRegionBackground
  {
    get => (Brush) this.GetValue(MessageBox.ButtonRegionBackgroundProperty);
    set => this.SetValue(MessageBox.ButtonRegionBackgroundProperty, (object) value);
  }

  public object CancelButtonContent
  {
    get => this.GetValue(MessageBox.CancelButtonContentProperty);
    set => this.SetValue(MessageBox.CancelButtonContentProperty, value);
  }

  public Style CancelButtonStyle
  {
    get => (Style) this.GetValue(MessageBox.CancelButtonStyleProperty);
    set => this.SetValue(MessageBox.CancelButtonStyleProperty, (object) value);
  }

  public ImageSource ImageSource
  {
    get => (ImageSource) this.GetValue(MessageBox.ImageSourceProperty);
    set => this.SetValue(MessageBox.ImageSourceProperty, (object) value);
  }

  public object OkButtonContent
  {
    get => this.GetValue(MessageBox.OkButtonContentProperty);
    set => this.SetValue(MessageBox.OkButtonContentProperty, value);
  }

  public Style OkButtonStyle
  {
    get => (Style) this.GetValue(MessageBox.OkButtonStyleProperty);
    set => this.SetValue(MessageBox.OkButtonStyleProperty, (object) value);
  }

  public MessageBoxResult MessageBoxResult => this._dialogResult;

  public object NoButtonContent
  {
    get => this.GetValue(MessageBox.NoButtonContentProperty);
    set => this.SetValue(MessageBox.NoButtonContentProperty, value);
  }

  public Style NoButtonStyle
  {
    get => (Style) this.GetValue(MessageBox.NoButtonStyleProperty);
    set => this.SetValue(MessageBox.NoButtonStyleProperty, (object) value);
  }

  public string Text
  {
    get => (string) this.GetValue(MessageBox.TextProperty);
    set => this.SetValue(MessageBox.TextProperty, (object) value);
  }

  public object YesButtonContent
  {
    get => this.GetValue(MessageBox.YesButtonContentProperty);
    set => this.SetValue(MessageBox.YesButtonContentProperty, value);
  }

  public Style YesButtonStyle
  {
    get => (Style) this.GetValue(MessageBox.YesButtonStyleProperty);
    set => this.SetValue(MessageBox.YesButtonStyleProperty, (object) value);
  }

  internal override bool AllowPublicIsActiveChange => false;

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    if (this._windowControl != null)
    {
      this._windowControl.HeaderDragDelta -= (DragDeltaEventHandler) ((o, e) => this.OnHeaderDragDelta(e));
      this._windowControl.HeaderIconDoubleClicked -= (MouseButtonEventHandler) ((o, e) => this.OnHeaderIconDoubleClicked(e));
      this._windowControl.CloseButtonClicked -= (RoutedEventHandler) ((o, e) => this.OnCloseButtonClicked(e));
    }
    this._windowControl = this.GetTemplateChild("PART_WindowControl") as WindowControl;
    if (this._windowControl != null)
    {
      this._windowControl.HeaderDragDelta += (DragDeltaEventHandler) ((o, e) => this.OnHeaderDragDelta(e));
      this._windowControl.HeaderIconDoubleClicked += (MouseButtonEventHandler) ((o, e) => this.OnHeaderIconDoubleClicked(e));
      this._windowControl.CloseButtonClicked += (RoutedEventHandler) ((o, e) => this.OnCloseButtonClicked(e));
    }
    this.UpdateBlockMouseInputsPanel();
    this.ChangeVisualState(this._button.ToString(), true);
    Button messageBoxButton1 = this.GetMessageBoxButton("PART_CloseButton");
    if (messageBoxButton1 != null)
      messageBoxButton1.IsEnabled = !object.Equals((object) this._button, (object) MessageBoxButton.YesNo);
    Button messageBoxButton2 = this.GetMessageBoxButton("PART_OkButton");
    if (messageBoxButton2 != null)
      messageBoxButton2.IsCancel = object.Equals((object) this._button, (object) MessageBoxButton.OK);
    this.SetDefaultResult();
  }

  protected override void OnPreviewKeyDown(KeyEventArgs e)
  {
    base.OnPreviewKeyDown(e);
    if (!Keyboard.IsKeyDown(Key.LeftAlt) && !Keyboard.IsKeyDown(Key.RightAlt))
      return;
    e.Handled = true;
  }

  protected override object OnCoerceCloseButtonVisibility(Visibility newValue)
  {
    return newValue == Visibility.Visible ? (object) newValue : throw new InvalidOperationException("Close button on MessageBox is always Visible.");
  }

  protected override object OnCoerceWindowStyle(WindowStyle newValue)
  {
    return newValue == WindowStyle.SingleBorderWindow ? (object) newValue : throw new InvalidOperationException("Window style on MessageBox is not available.");
  }

  internal override void UpdateBlockMouseInputsPanel()
  {
    if (this._windowControl == null)
      return;
    this._windowControl.IsBlockMouseInputsPanelActive = this.IsBlockMouseInputsPanelActive;
  }

  public static MessageBoxResult Show(string messageText)
  {
    return MessageBox.Show(messageText, string.Empty, MessageBoxButton.OK, (Style) null);
  }

  public static MessageBoxResult Show(Window owner, string messageText)
  {
    return MessageBox.Show(owner, messageText, string.Empty, MessageBoxButton.OK, (Style) null);
  }

  public static MessageBoxResult Show(string messageText, string caption)
  {
    return MessageBox.Show(messageText, caption, MessageBoxButton.OK, (Style) null);
  }

  public static MessageBoxResult Show(Window owner, string messageText, string caption)
  {
    return MessageBox.Show(owner, messageText, caption, (Style) null);
  }

  public static MessageBoxResult Show(
    Window owner,
    string messageText,
    string caption,
    Style messageBoxStyle)
  {
    return MessageBox.Show(owner, messageText, caption, MessageBoxButton.OK, messageBoxStyle);
  }

  public static MessageBoxResult Show(string messageText, string caption, MessageBoxButton button)
  {
    return MessageBox.Show(messageText, caption, button, (Style) null);
  }

  public static MessageBoxResult Show(
    string messageText,
    string caption,
    MessageBoxButton button,
    Style messageBoxStyle)
  {
    return MessageBox.ShowCore((Window) null, IntPtr.Zero, messageText, caption, button, MessageBoxImage.None, MessageBoxResult.None, messageBoxStyle);
  }

  public static MessageBoxResult Show(
    Window owner,
    string messageText,
    string caption,
    MessageBoxButton button)
  {
    return MessageBox.Show(owner, messageText, caption, button, (Style) null);
  }

  public static MessageBoxResult Show(
    Window owner,
    string messageText,
    string caption,
    MessageBoxButton button,
    Style messageBoxStyle)
  {
    return MessageBox.ShowCore(owner, IntPtr.Zero, messageText, caption, button, MessageBoxImage.None, MessageBoxResult.None, messageBoxStyle);
  }

  public static MessageBoxResult Show(
    string messageText,
    string caption,
    MessageBoxButton button,
    MessageBoxImage icon)
  {
    return MessageBox.Show(messageText, caption, button, icon, (Style) null);
  }

  public static MessageBoxResult Show(
    string messageText,
    string caption,
    MessageBoxButton button,
    MessageBoxImage icon,
    Style messageBoxStyle)
  {
    return MessageBox.ShowCore((Window) null, IntPtr.Zero, messageText, caption, button, icon, MessageBoxResult.None, messageBoxStyle);
  }

  public static MessageBoxResult Show(
    Window owner,
    string messageText,
    string caption,
    MessageBoxButton button,
    MessageBoxImage icon)
  {
    return MessageBox.Show(owner, messageText, caption, button, icon, (Style) null);
  }

  public static MessageBoxResult Show(
    Window owner,
    string messageText,
    string caption,
    MessageBoxButton button,
    MessageBoxImage icon,
    Style messageBoxStyle)
  {
    return MessageBox.ShowCore(owner, IntPtr.Zero, messageText, caption, button, icon, MessageBoxResult.None, messageBoxStyle);
  }

  public static MessageBoxResult Show(
    string messageText,
    string caption,
    MessageBoxButton button,
    MessageBoxImage icon,
    MessageBoxResult defaultResult)
  {
    return MessageBox.Show(messageText, caption, button, icon, defaultResult, (Style) null);
  }

  public static MessageBoxResult Show(
    string messageText,
    string caption,
    MessageBoxButton button,
    MessageBoxImage icon,
    MessageBoxResult defaultResult,
    Style messageBoxStyle)
  {
    return MessageBox.ShowCore((Window) null, IntPtr.Zero, messageText, caption, button, icon, defaultResult, messageBoxStyle);
  }

  public static MessageBoxResult Show(
    Window owner,
    string messageText,
    string caption,
    MessageBoxButton button,
    MessageBoxImage icon,
    MessageBoxResult defaultResult)
  {
    return MessageBox.Show(owner, messageText, caption, button, icon, defaultResult, (Style) null);
  }

  public static MessageBoxResult Show(
    Window owner,
    string messageText,
    string caption,
    MessageBoxButton button,
    MessageBoxImage icon,
    MessageBoxResult defaultResult,
    Style messageBoxStyle)
  {
    return MessageBox.ShowCore(owner, IntPtr.Zero, messageText, caption, button, icon, defaultResult, messageBoxStyle);
  }

  public static MessageBoxResult Show(IntPtr ownerWindowHandle, string messageText)
  {
    return MessageBox.Show(ownerWindowHandle, messageText, string.Empty, MessageBoxButton.OK, (Style) null);
  }

  public static MessageBoxResult Show(IntPtr ownerWindowHandle, string messageText, string caption)
  {
    return MessageBox.Show(ownerWindowHandle, messageText, caption, (Style) null);
  }

  public static MessageBoxResult Show(
    IntPtr ownerWindowHandle,
    string messageText,
    string caption,
    Style messageBoxStyle)
  {
    return MessageBox.Show(ownerWindowHandle, messageText, caption, MessageBoxButton.OK, messageBoxStyle);
  }

  public static MessageBoxResult Show(
    IntPtr ownerWindowHandle,
    string messageText,
    string caption,
    MessageBoxButton button)
  {
    return MessageBox.Show(ownerWindowHandle, messageText, caption, button, (Style) null);
  }

  public static MessageBoxResult Show(
    IntPtr ownerWindowHandle,
    string messageText,
    string caption,
    MessageBoxButton button,
    Style messageBoxStyle)
  {
    return MessageBox.ShowCore((Window) null, ownerWindowHandle, messageText, caption, button, MessageBoxImage.None, MessageBoxResult.None, messageBoxStyle);
  }

  public static MessageBoxResult Show(
    IntPtr ownerWindowHandle,
    string messageText,
    string caption,
    MessageBoxButton button,
    MessageBoxImage icon)
  {
    return MessageBox.Show(ownerWindowHandle, messageText, caption, button, icon, (Style) null);
  }

  public static MessageBoxResult Show(
    IntPtr ownerWindowHandle,
    string messageText,
    string caption,
    MessageBoxButton button,
    MessageBoxImage icon,
    Style messageBoxStyle)
  {
    return MessageBox.ShowCore((Window) null, ownerWindowHandle, messageText, caption, button, icon, MessageBoxResult.None, messageBoxStyle);
  }

  public static MessageBoxResult Show(
    IntPtr ownerWindowHandle,
    string messageText,
    string caption,
    MessageBoxButton button,
    MessageBoxImage icon,
    MessageBoxResult defaultResult)
  {
    return MessageBox.Show(ownerWindowHandle, messageText, caption, button, icon, defaultResult, (Style) null);
  }

  public static MessageBoxResult Show(
    IntPtr ownerWindowHandle,
    string messageText,
    string caption,
    MessageBoxButton button,
    MessageBoxImage icon,
    MessageBoxResult defaultResult,
    Style messageBoxStyle)
  {
    return MessageBox.ShowCore((Window) null, ownerWindowHandle, messageText, caption, button, icon, defaultResult, messageBoxStyle);
  }

  public void ShowMessageBox()
  {
    if (this.Container != null || this.Parent == null)
      throw new InvalidOperationException("This method is not intended to be called while displaying a MessageBox outside of a WindowContainer. Use ShowDialog() instead in that case.");
    if (!(this.Parent is WindowContainer))
      throw new InvalidOperationException("The MessageBox instance is not intended to be displayed in a container other than a WindowContainer.");
    this._dialogResult = MessageBoxResult.None;
    this.Visibility = Visibility.Visible;
  }

  public void ShowMessageBox(string messageText)
  {
    this.ShowMessageBoxCore(messageText, string.Empty, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None);
  }

  public void ShowMessageBox(string messageText, string caption)
  {
    this.ShowMessageBoxCore(messageText, caption, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None);
  }

  public void ShowMessageBox(string messageText, string caption, MessageBoxButton button)
  {
    this.ShowMessageBoxCore(messageText, caption, button, MessageBoxImage.None, MessageBoxResult.None);
  }

  public void ShowMessageBox(
    string messageText,
    string caption,
    MessageBoxButton button,
    MessageBoxImage icon)
  {
    this.ShowMessageBoxCore(messageText, caption, button, icon, MessageBoxResult.None);
  }

  public void ShowMessageBox(
    string messageText,
    string caption,
    MessageBoxButton button,
    MessageBoxImage icon,
    MessageBoxResult defaultResult)
  {
    this.ShowMessageBoxCore(messageText, caption, button, icon, defaultResult);
  }

  public bool? ShowDialog()
  {
    if (this.Parent != null)
      throw new InvalidOperationException("This method is not intended to be called while displaying a Message Box inside a WindowContainer. Use 'ShowMessageBox()' instead.");
    this._dialogResult = MessageBoxResult.None;
    this.Visibility = Visibility.Visible;
    this.CreateContainer();
    return this.Container.ShowDialog();
  }

  protected void InitializeMessageBox(
    Window owner,
    IntPtr ownerHandle,
    string text,
    string caption,
    MessageBoxButton button,
    MessageBoxImage image,
    MessageBoxResult defaultResult)
  {
    this.Text = text;
    this.Caption = caption;
    this._button = button;
    this._defaultResult = defaultResult;
    this._owner = owner;
    this._ownerHandle = ownerHandle;
    this.SetImageSource(image);
  }

  protected void ChangeVisualState(string name, bool useTransitions)
  {
    VisualStateManager.GoToState((FrameworkElement) this, name, useTransitions);
  }

  private bool IsCurrentWindow(object windowtoTest)
  {
    return object.Equals((object) this._windowControl, windowtoTest);
  }

  private void Close()
  {
    if (this.Container != null)
      this.Container.Close();
    else
      this.OnClose();
  }

  private void SetDefaultResult()
  {
    Button fromDefaultResult = this.GetDefaultButtonFromDefaultResult();
    if (fromDefaultResult == null)
      return;
    fromDefaultResult.IsDefault = true;
    fromDefaultResult.Focus();
  }

  private Button GetDefaultButtonFromDefaultResult()
  {
    Button fromDefaultResult = (Button) null;
    switch (this._defaultResult)
    {
      case MessageBoxResult.None:
        fromDefaultResult = this.GetDefaultButton();
        break;
      case MessageBoxResult.OK:
        fromDefaultResult = this.GetMessageBoxButton("PART_OkButton");
        break;
      case MessageBoxResult.Cancel:
        fromDefaultResult = this.GetMessageBoxButton("PART_CancelButton");
        break;
      case MessageBoxResult.Yes:
        fromDefaultResult = this.GetMessageBoxButton("PART_YesButton");
        break;
      case MessageBoxResult.No:
        fromDefaultResult = this.GetMessageBoxButton("PART_NoButton");
        break;
    }
    return fromDefaultResult;
  }

  private Button GetDefaultButton()
  {
    Button defaultButton = (Button) null;
    switch (this._button)
    {
      case MessageBoxButton.OK:
      case MessageBoxButton.OKCancel:
        defaultButton = this.GetMessageBoxButton("PART_OkButton");
        break;
      case MessageBoxButton.YesNoCancel:
      case MessageBoxButton.YesNo:
        defaultButton = this.GetMessageBoxButton("PART_YesButton");
        break;
    }
    return defaultButton;
  }

  private Button GetMessageBoxButton(string name) => this.GetTemplateChild(name) as Button;

  private void ShowMessageBoxCore(
    string messageText,
    string caption,
    MessageBoxButton button,
    MessageBoxImage icon,
    MessageBoxResult defaultResult)
  {
    this.InitializeMessageBox((Window) null, IntPtr.Zero, messageText, caption, button, icon, defaultResult);
    this.ShowMessageBox();
  }

  private void InitHandlers()
  {
    this.AddHandler(ButtonBase.ClickEvent, (Delegate) new RoutedEventHandler(this.Button_Click));
    this.CommandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.Copy, new ExecutedRoutedEventHandler(this.ExecuteCopy)));
  }

  private static MessageBoxResult ShowCore(
    Window owner,
    IntPtr ownerHandle,
    string messageText,
    string caption,
    MessageBoxButton button,
    MessageBoxImage icon,
    MessageBoxResult defaultResult,
    Style messageBoxStyle)
  {
    if (BrowserInteropHelper.IsBrowserHosted)
      throw new InvalidOperationException("Static methods for MessageBoxes are not available in XBAP. Use the instance ShowMessageBox methods instead.");
    if (owner != null && ownerHandle != IntPtr.Zero)
      throw new NotSupportedException("The owner of a MessageBox can't be both a Window and a WindowHandle.");
    MessageBox messageBox = new MessageBox();
    messageBox.InitializeMessageBox(owner, ownerHandle, messageText, caption, button, icon, defaultResult);
    if (messageBoxStyle != null)
      messageBox.Style = messageBoxStyle;
    messageBox.ShowDialog();
    return messageBox.MessageBoxResult;
  }

  private static Window ComputeOwnerWindow()
  {
    Window result = (Window) null;
    if (Application.Current != null)
    {
      if (Application.Current.Dispatcher.CheckAccess())
        result = MessageBox.ComputeOwnerWindowCore();
      else
        Application.Current.Dispatcher.BeginInvoke((Delegate) (() => result = MessageBox.ComputeOwnerWindowCore()));
    }
    return result;
  }

  private static Window ComputeOwnerWindowCore()
  {
    Window ownerWindowCore = (Window) null;
    if (Application.Current != null)
    {
      foreach (Window window in Application.Current.Windows)
      {
        if (window.IsActive)
        {
          ownerWindowCore = window;
          break;
        }
      }
    }
    return ownerWindowCore;
  }

  private void SetImageSource(MessageBoxImage image)
  {
    string empty = string.Empty;
    string str;
    switch (image)
    {
      case MessageBoxImage.None:
        return;
      case MessageBoxImage.Hand:
        str = "Error48.png";
        break;
      case MessageBoxImage.Question:
        str = "Question48.png";
        break;
      case MessageBoxImage.Exclamation:
        str = "Warning48.png";
        break;
      case MessageBoxImage.Asterisk:
        str = "Information48.png";
        break;
      default:
        return;
    }
    this.ImageSource = (ImageSource) new BitmapImage(new Uri($"/Xceed.Wpf.Toolkit;component/MessageBox/Icons/{str}", UriKind.RelativeOrAbsolute));
  }

  private Window CreateContainer()
  {
    Window window = new Window();
    window.AllowsTransparency = true;
    window.Background = (Brush) Brushes.Transparent;
    window.Content = (object) this;
    if (this._ownerHandle != IntPtr.Zero)
    {
      new WindowInteropHelper(window).Owner = this._ownerHandle;
      window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
    }
    else
    {
      window.Owner = this._owner ?? MessageBox.ComputeOwnerWindow();
      window.WindowStartupLocation = window.Owner == null ? System.Windows.WindowStartupLocation.CenterScreen : System.Windows.WindowStartupLocation.CenterOwner;
    }
    window.ShowInTaskbar = false;
    window.SizeToContent = SizeToContent.WidthAndHeight;
    window.ResizeMode = ResizeMode.NoResize;
    window.WindowStyle = WindowStyle.None;
    window.Closed += new EventHandler(this.OnContainerClosed);
    return window;
  }

  protected virtual void OnHeaderDragDelta(DragDeltaEventArgs e)
  {
    if (!this.IsCurrentWindow(e.OriginalSource))
      return;
    e.Handled = true;
    DragDeltaEventArgs e1 = new DragDeltaEventArgs(e.HorizontalChange, e.VerticalChange);
    e1.RoutedEvent = WindowControl.HeaderDragDeltaEvent;
    e1.Source = (object) this;
    this.RaiseEvent((RoutedEventArgs) e1);
    if (e1.Handled)
      return;
    if (this.Container == null)
    {
      this.Left = this.FlowDirection != FlowDirection.RightToLeft ? this.Left + e.HorizontalChange : this.Left - e.HorizontalChange;
      this.Top += e.VerticalChange;
    }
    else
    {
      this.Container.Left = this.FlowDirection != FlowDirection.RightToLeft ? this.Container.Left + e.HorizontalChange : this.Container.Left - e.HorizontalChange;
      this.Container.Top += e.VerticalChange;
    }
  }

  protected virtual void OnHeaderIconDoubleClicked(MouseButtonEventArgs e)
  {
    if (!this.IsCurrentWindow(e.OriginalSource))
      return;
    e.Handled = true;
    MouseButtonEventArgs e1 = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
    e1.RoutedEvent = WindowControl.HeaderIconDoubleClickedEvent;
    e1.Source = (object) this;
    this.RaiseEvent((RoutedEventArgs) e1);
    if (e1.Handled)
      return;
    this.Close();
  }

  protected virtual void OnCloseButtonClicked(RoutedEventArgs e)
  {
    if (!this.IsCurrentWindow(e.OriginalSource))
      return;
    e.Handled = true;
    this._dialogResult = object.Equals((object) this._button, (object) MessageBoxButton.OK) ? MessageBoxResult.OK : MessageBoxResult.Cancel;
    RoutedEventArgs e1 = new RoutedEventArgs(WindowControl.CloseButtonClickedEvent, (object) this);
    this.RaiseEvent(e1);
    if (e1.Handled)
      return;
    this.Close();
  }

  private void Button_Click(object sender, RoutedEventArgs e)
  {
    if (!(e.OriginalSource is Button originalSource))
      return;
    switch (originalSource.Name)
    {
      case "PART_NoButton":
        this._dialogResult = MessageBoxResult.No;
        this.Close();
        break;
      case "PART_YesButton":
        this._dialogResult = MessageBoxResult.Yes;
        this.Close();
        break;
      case "PART_CancelButton":
        this._dialogResult = MessageBoxResult.Cancel;
        this.Close();
        break;
      case "PART_OkButton":
        this._dialogResult = MessageBoxResult.OK;
        this.Close();
        break;
    }
    e.Handled = true;
  }

  private void OnContainerClosed(object sender, EventArgs e)
  {
    this.Container.Closed -= new EventHandler(this.OnContainerClosed);
    this.Container.Content = (object) null;
    this.OnClose();
  }

  private void OnClose()
  {
    this.Visibility = Visibility.Collapsed;
    this.OnClosed(EventArgs.Empty);
  }

  private void MessageBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
  {
    if (!(bool) e.NewValue)
      return;
    this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Delegate) (() => this.GetDefaultButtonFromDefaultResult()?.Focus()));
  }

  public event EventHandler Closed;

  protected virtual void OnClosed(EventArgs e)
  {
    if (this.Closed == null)
      return;
    this.Closed((object) this, e);
  }

  private void ExecuteCopy(object sender, ExecutedRoutedEventArgs e)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("---------------------------");
    stringBuilder.AppendLine();
    stringBuilder.Append(this.Caption);
    stringBuilder.AppendLine();
    stringBuilder.Append("---------------------------");
    stringBuilder.AppendLine();
    stringBuilder.Append(this.Text);
    stringBuilder.AppendLine();
    stringBuilder.Append("---------------------------");
    stringBuilder.AppendLine();
    switch (this._button)
    {
      case MessageBoxButton.OK:
        stringBuilder.Append(this.OkButtonContent.ToString());
        break;
      case MessageBoxButton.OKCancel:
        stringBuilder.Append($"{this.OkButtonContent}     {this.CancelButtonContent}");
        break;
      case MessageBoxButton.YesNoCancel:
        stringBuilder.Append($"{this.YesButtonContent}     {this.NoButtonContent}     {this.CancelButtonContent}");
        break;
      case MessageBoxButton.YesNo:
        stringBuilder.Append($"{this.YesButtonContent}     {this.NoButtonContent}");
        break;
    }
    stringBuilder.AppendLine();
    stringBuilder.Append("---------------------------");
    try
    {
      new UIPermission(UIPermissionClipboard.AllClipboard).Demand();
      Clipboard.SetText(stringBuilder.ToString());
    }
    catch (SecurityException ex)
    {
      throw new SecurityException();
    }
  }

  private delegate Window ComputeOwnerWindowCoreDelegate();
}
