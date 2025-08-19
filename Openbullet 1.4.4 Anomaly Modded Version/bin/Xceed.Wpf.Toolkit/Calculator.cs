// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Calculator
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[TemplatePart(Name = "PART_CalculatorButtonPanel", Type = typeof (ContentControl))]
public class Calculator : Control
{
  private const string PART_CalculatorButtonPanel = "PART_CalculatorButtonPanel";
  private ContentControl _buttonPanel;
  private bool _showNewNumber = true;
  private Decimal _previousValue;
  private Calculator.Operation _lastOperation = Calculator.Operation.None;
  private readonly Dictionary<Button, DispatcherTimer> _timers = new Dictionary<Button, DispatcherTimer>();
  public static readonly DependencyProperty CalculatorButtonPanelTemplateProperty = DependencyProperty.Register(nameof (CalculatorButtonPanelTemplate), typeof (ControlTemplate), typeof (Calculator), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty CalculatorButtonTypeProperty = DependencyProperty.RegisterAttached("CalculatorButtonType", typeof (Calculator.CalculatorButtonType), typeof (Calculator), (PropertyMetadata) new UIPropertyMetadata((object) Calculator.CalculatorButtonType.None, new PropertyChangedCallback(Calculator.OnCalculatorButtonTypeChanged)));
  public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(nameof (DisplayText), typeof (string), typeof (Calculator), (PropertyMetadata) new UIPropertyMetadata((object) "0", new PropertyChangedCallback(Calculator.OnDisplayTextChanged)));
  public static readonly DependencyProperty MemoryProperty = DependencyProperty.Register(nameof (Memory), typeof (Decimal), typeof (Calculator), (PropertyMetadata) new UIPropertyMetadata((object) 0M));
  public static readonly DependencyProperty PrecisionProperty = DependencyProperty.Register(nameof (Precision), typeof (int), typeof (Calculator), (PropertyMetadata) new UIPropertyMetadata((object) 6));
  public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof (Value), typeof (Decimal?), typeof (Calculator), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0M, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(Calculator.OnValueChanged)));
  public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof (RoutedPropertyChangedEventHandler<object>), typeof (Calculator));

  public ControlTemplate CalculatorButtonPanelTemplate
  {
    get => (ControlTemplate) this.GetValue(Calculator.CalculatorButtonPanelTemplateProperty);
    set => this.SetValue(Calculator.CalculatorButtonPanelTemplateProperty, (object) value);
  }

  public static Calculator.CalculatorButtonType GetCalculatorButtonType(DependencyObject target)
  {
    return (Calculator.CalculatorButtonType) target.GetValue(Calculator.CalculatorButtonTypeProperty);
  }

  public static void SetCalculatorButtonType(
    DependencyObject target,
    Calculator.CalculatorButtonType value)
  {
    target.SetValue(Calculator.CalculatorButtonTypeProperty, (object) value);
  }

  private static void OnCalculatorButtonTypeChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    Calculator.OnCalculatorButtonTypeChanged(o, (Calculator.CalculatorButtonType) e.OldValue, (Calculator.CalculatorButtonType) e.NewValue);
  }

  private static void OnCalculatorButtonTypeChanged(
    DependencyObject o,
    Calculator.CalculatorButtonType oldValue,
    Calculator.CalculatorButtonType newValue)
  {
    Button button = o as Button;
    button.CommandParameter = (object) newValue;
    if (button.Content != null)
      return;
    button.Content = (object) CalculatorUtilities.GetCalculatorButtonContent(newValue);
  }

  public string DisplayText
  {
    get => (string) this.GetValue(Calculator.DisplayTextProperty);
    set => this.SetValue(Calculator.DisplayTextProperty, (object) value);
  }

  private static void OnDisplayTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is Calculator calculator))
      return;
    calculator.OnDisplayTextChanged((string) e.OldValue, (string) e.NewValue);
  }

  protected virtual void OnDisplayTextChanged(string oldValue, string newValue)
  {
  }

  public Decimal Memory
  {
    get => (Decimal) this.GetValue(Calculator.MemoryProperty);
    set => this.SetValue(Calculator.MemoryProperty, (object) value);
  }

  public int Precision
  {
    get => (int) this.GetValue(Calculator.PrecisionProperty);
    set => this.SetValue(Calculator.PrecisionProperty, (object) value);
  }

  public Decimal? Value
  {
    get => (Decimal?) this.GetValue(Calculator.ValueProperty);
    set => this.SetValue(Calculator.ValueProperty, (object) value);
  }

  private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is Calculator calculator))
      return;
    calculator.OnValueChanged((Decimal?) e.OldValue, (Decimal?) e.NewValue);
  }

  protected virtual void OnValueChanged(Decimal? oldValue, Decimal? newValue)
  {
    this.SetDisplayText(newValue);
    RoutedPropertyChangedEventArgs<object> e = new RoutedPropertyChangedEventArgs<object>((object) oldValue, (object) newValue);
    e.RoutedEvent = Calculator.ValueChangedEvent;
    this.RaiseEvent((RoutedEventArgs) e);
  }

  static Calculator()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (Calculator), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (Calculator)));
  }

  public Calculator()
  {
    this.CommandBindings.Add(new CommandBinding((ICommand) CalculatorCommands.CalculatorButtonClick, new ExecutedRoutedEventHandler(this.ExecuteCalculatorButtonClick)));
    this.AddHandler(UIElement.MouseDownEvent, (Delegate) new MouseButtonEventHandler(this.Calculator_OnMouseDown), true);
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this._buttonPanel = this.GetTemplateChild("PART_CalculatorButtonPanel") as ContentControl;
  }

  protected override void OnTextInput(TextCompositionEventArgs e)
  {
    Calculator.CalculatorButtonType buttonTypeFromText = CalculatorUtilities.GetCalculatorButtonTypeFromText(e.Text);
    if (buttonTypeFromText == Calculator.CalculatorButtonType.None)
      return;
    this.SimulateCalculatorButtonClick(buttonTypeFromText);
    this.ProcessCalculatorButton(buttonTypeFromText);
  }

  private void Calculator_OnMouseDown(object sender, MouseButtonEventArgs e)
  {
    if (this.IsFocused)
      return;
    this.Focus();
    e.Handled = true;
  }

  private void Timer_Tick(object sender, EventArgs e)
  {
    DispatcherTimer timer = (DispatcherTimer) sender;
    timer.Stop();
    timer.Tick -= new EventHandler(this.Timer_Tick);
    if (!this._timers.ContainsValue(timer))
      return;
    Button button = this._timers.Where<KeyValuePair<Button, DispatcherTimer>>((Func<KeyValuePair<Button, DispatcherTimer>, bool>) (x => x.Value == timer)).Select<KeyValuePair<Button, DispatcherTimer>, Button>((Func<KeyValuePair<Button, DispatcherTimer>, Button>) (x => x.Key)).FirstOrDefault<Button>();
    if (button == null)
      return;
    VisualStateManager.GoToState((FrameworkElement) button, button.IsMouseOver ? "MouseOver" : "Normal", true);
    this._timers.Remove(button);
  }

  internal void InitializeToValue(Decimal? value)
  {
    this._previousValue = 0M;
    this._lastOperation = Calculator.Operation.None;
    this._showNewNumber = true;
    this.Value = value;
    this.SetDisplayText(value);
  }

  private void Calculate()
  {
    if (this._lastOperation == Calculator.Operation.None)
      return;
    try
    {
      this.Value = new Decimal?(Decimal.Round(this.CalculateValue(this._lastOperation), this.Precision));
      this.SetDisplayText(this.Value);
    }
    catch
    {
      this.Value = new Decimal?();
      this.DisplayText = "ERROR";
    }
  }

  private void SetDisplayText(Decimal? newValue)
  {
    if (newValue.HasValue && newValue.Value != 0M)
      this.DisplayText = newValue.ToString();
    else
      this.DisplayText = "0";
  }

  private void Calculate(Calculator.Operation newOperation)
  {
    if (!this._showNewNumber)
      this.Calculate();
    this._lastOperation = newOperation;
  }

  private void Calculate(Calculator.Operation currentOperation, Calculator.Operation newOperation)
  {
    this._lastOperation = currentOperation;
    this.Calculate();
    this._lastOperation = newOperation;
  }

  private Decimal CalculateValue(Calculator.Operation operation)
  {
    Decimal num1 = CalculatorUtilities.ParseDecimal(this.DisplayText);
    Decimal num2;
    switch (operation)
    {
      case Calculator.Operation.Add:
        num2 = CalculatorUtilities.Add(this._previousValue, num1);
        break;
      case Calculator.Operation.Subtract:
        num2 = CalculatorUtilities.Subtract(this._previousValue, num1);
        break;
      case Calculator.Operation.Divide:
        num2 = CalculatorUtilities.Divide(this._previousValue, num1);
        break;
      case Calculator.Operation.Multiply:
        num2 = CalculatorUtilities.Multiply(this._previousValue, num1);
        break;
      case Calculator.Operation.Sqrt:
        num2 = CalculatorUtilities.SquareRoot(num1);
        break;
      case Calculator.Operation.Fraction:
        num2 = CalculatorUtilities.Fraction(num1);
        break;
      case Calculator.Operation.Negate:
        num2 = CalculatorUtilities.Negate(num1);
        break;
      default:
        num2 = 0M;
        break;
    }
    return num2;
  }

  private void ProcessBackKey()
  {
    string str;
    if (this.DisplayText.Length > 1 && (this.DisplayText.Length != 2 || this.DisplayText[0] != '-'))
    {
      str = this.DisplayText.Remove(this.DisplayText.Length - 1, 1);
    }
    else
    {
      str = "0";
      this._showNewNumber = true;
    }
    this.DisplayText = str;
  }

  private void ProcessCalculatorButton(Calculator.CalculatorButtonType buttonType)
  {
    if (CalculatorUtilities.IsDigit(buttonType))
      this.ProcessDigitKey(buttonType);
    else if (CalculatorUtilities.IsMemory(buttonType))
      this.ProcessMemoryKey(buttonType);
    else
      this.ProcessOperationKey(buttonType);
  }

  private void ProcessDigitKey(Calculator.CalculatorButtonType buttonType)
  {
    this.DisplayText = !this._showNewNumber ? this.DisplayText + CalculatorUtilities.GetCalculatorButtonContent(buttonType) : CalculatorUtilities.GetCalculatorButtonContent(buttonType);
    this._showNewNumber = false;
  }

  private void ProcessMemoryKey(Calculator.CalculatorButtonType buttonType)
  {
    Decimal num = CalculatorUtilities.ParseDecimal(this.DisplayText);
    this._showNewNumber = true;
    switch (buttonType)
    {
      case Calculator.CalculatorButtonType.MAdd:
        this.Memory += num;
        break;
      case Calculator.CalculatorButtonType.MC:
        this.Memory = 0M;
        break;
      case Calculator.CalculatorButtonType.MR:
        this.DisplayText = this.Memory.ToString();
        this._showNewNumber = false;
        break;
      case Calculator.CalculatorButtonType.MS:
        this.Memory = num;
        break;
      case Calculator.CalculatorButtonType.MSub:
        this.Memory -= num;
        break;
    }
  }

  private void ProcessOperationKey(Calculator.CalculatorButtonType buttonType)
  {
    switch (buttonType)
    {
      case Calculator.CalculatorButtonType.Add:
        this.Calculate(Calculator.Operation.Add);
        break;
      case Calculator.CalculatorButtonType.Back:
        this.ProcessBackKey();
        return;
      case Calculator.CalculatorButtonType.Cancel:
        this.DisplayText = this._previousValue.ToString();
        this._lastOperation = Calculator.Operation.None;
        this._showNewNumber = true;
        return;
      case Calculator.CalculatorButtonType.Clear:
        this.Calculate(Calculator.Operation.Clear, Calculator.Operation.None);
        break;
      case Calculator.CalculatorButtonType.Divide:
        this.Calculate(Calculator.Operation.Divide);
        break;
      case Calculator.CalculatorButtonType.Equal:
        this.Calculate(Calculator.Operation.None);
        break;
      case Calculator.CalculatorButtonType.Fraction:
        this.Calculate(Calculator.Operation.Fraction, Calculator.Operation.None);
        break;
      case Calculator.CalculatorButtonType.Multiply:
        this.Calculate(Calculator.Operation.Multiply);
        break;
      case Calculator.CalculatorButtonType.Negate:
        this.Calculate(Calculator.Operation.Negate, Calculator.Operation.None);
        break;
      case Calculator.CalculatorButtonType.Percent:
        if (this._lastOperation != Calculator.Operation.None)
        {
          this.DisplayText = CalculatorUtilities.Percent(this._previousValue, CalculatorUtilities.ParseDecimal(this.DisplayText)).ToString();
          return;
        }
        this.DisplayText = "0";
        this._showNewNumber = true;
        return;
      case Calculator.CalculatorButtonType.Sqrt:
        this.Calculate(Calculator.Operation.Sqrt, Calculator.Operation.None);
        break;
      case Calculator.CalculatorButtonType.Subtract:
        this.Calculate(Calculator.Operation.Subtract);
        break;
    }
    Decimal.TryParse(this.DisplayText, out this._previousValue);
    this._showNewNumber = true;
  }

  private void SimulateCalculatorButtonClick(Calculator.CalculatorButtonType buttonType)
  {
    Button calculatorButtonType = CalculatorUtilities.FindButtonByCalculatorButtonType((DependencyObject) this._buttonPanel, buttonType);
    if (calculatorButtonType == null)
      return;
    VisualStateManager.GoToState((FrameworkElement) calculatorButtonType, "Pressed", true);
    DispatcherTimer dispatcherTimer;
    if (this._timers.ContainsKey(calculatorButtonType))
    {
      dispatcherTimer = this._timers[calculatorButtonType];
      dispatcherTimer.Stop();
    }
    else
    {
      dispatcherTimer = new DispatcherTimer();
      dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100.0);
      dispatcherTimer.Tick += new EventHandler(this.Timer_Tick);
      this._timers.Add(calculatorButtonType, dispatcherTimer);
    }
    dispatcherTimer.Start();
  }

  public event RoutedPropertyChangedEventHandler<object> ValueChanged
  {
    add => this.AddHandler(Calculator.ValueChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(Calculator.ValueChangedEvent, (Delegate) value);
  }

  private void ExecuteCalculatorButtonClick(object sender, ExecutedRoutedEventArgs e)
  {
    this.ProcessCalculatorButton((Calculator.CalculatorButtonType) e.Parameter);
  }

  public enum CalculatorButtonType
  {
    Add,
    Back,
    Cancel,
    Clear,
    Decimal,
    Divide,
    Eight,
    Equal,
    Five,
    Four,
    Fraction,
    MAdd,
    MC,
    MR,
    MS,
    MSub,
    Multiply,
    Negate,
    Nine,
    None,
    One,
    Percent,
    Seven,
    Six,
    Sqrt,
    Subtract,
    Three,
    Two,
    Zero,
  }

  public enum Operation
  {
    Add,
    Subtract,
    Divide,
    Multiply,
    Percent,
    Sqrt,
    Fraction,
    None,
    Clear,
    Negate,
  }
}
