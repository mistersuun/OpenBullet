// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Primitives.DateTimeUpDownBase`1
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

#nullable disable
namespace Xceed.Wpf.Toolkit.Primitives;

public abstract class DateTimeUpDownBase<T> : UpDownBase<T>
{
  internal List<DateTimeInfo> _dateTimeInfoList = new List<DateTimeInfo>();
  internal DateTimeInfo _selectedDateTimeInfo;
  internal bool _fireSelectionChangedEvent = true;
  internal bool _processTextChanged = true;
  public static readonly DependencyProperty CurrentDateTimePartProperty = DependencyProperty.Register(nameof (CurrentDateTimePart), typeof (DateTimePart), typeof (DateTimeUpDownBase<T>), (PropertyMetadata) new UIPropertyMetadata((object) DateTimePart.Other, new PropertyChangedCallback(DateTimeUpDownBase<T>.OnCurrentDateTimePartChanged)));
  public static readonly DependencyProperty StepProperty = DependencyProperty.Register(nameof (Step), typeof (int), typeof (DateTimeUpDownBase<T>), (PropertyMetadata) new UIPropertyMetadata((object) 1, new PropertyChangedCallback(DateTimeUpDownBase<T>.OnStepChanged)));

  public DateTimePart CurrentDateTimePart
  {
    get => (DateTimePart) this.GetValue(DateTimeUpDownBase<T>.CurrentDateTimePartProperty);
    set => this.SetValue(DateTimeUpDownBase<T>.CurrentDateTimePartProperty, (object) value);
  }

  private static void OnCurrentDateTimePartChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is DateTimeUpDownBase<T> dateTimeUpDownBase))
      return;
    dateTimeUpDownBase.OnCurrentDateTimePartChanged((DateTimePart) e.OldValue, (DateTimePart) e.NewValue);
  }

  protected virtual void OnCurrentDateTimePartChanged(DateTimePart oldValue, DateTimePart newValue)
  {
    this.Select(this.GetDateTimeInfo(newValue));
  }

  public int Step
  {
    get => (int) this.GetValue(DateTimeUpDownBase<T>.StepProperty);
    set => this.SetValue(DateTimeUpDownBase<T>.StepProperty, (object) value);
  }

  private static void OnStepChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is DateTimeUpDownBase<T> dateTimeUpDownBase))
      return;
    dateTimeUpDownBase.OnStepChanged((int) e.OldValue, (int) e.NewValue);
  }

  protected virtual void OnStepChanged(int oldValue, int newValue)
  {
  }

  internal DateTimeUpDownBase()
  {
    this.InitializeDateTimeInfoList(this.Value);
    this.Loaded += new RoutedEventHandler(this.DateTimeUpDownBase_Loaded);
  }

  public override void OnApplyTemplate()
  {
    if (this.TextBox != null)
      this.TextBox.SelectionChanged -= new RoutedEventHandler(this.TextBox_SelectionChanged);
    base.OnApplyTemplate();
    if (this.TextBox == null)
      return;
    this.TextBox.SelectionChanged += new RoutedEventHandler(this.TextBox_SelectionChanged);
  }

  protected override void OnPreviewKeyDown(KeyEventArgs e)
  {
    int startPosition = this._selectedDateTimeInfo != null ? this._selectedDateTimeInfo.StartPosition : 0;
    int length = this._selectedDateTimeInfo != null ? this._selectedDateTimeInfo.Length : 0;
    switch (e.Key)
    {
      case Key.Return:
        if (this.IsReadOnly)
          return;
        this._fireSelectionChangedEvent = false;
        BindingOperations.GetBindingExpression((DependencyObject) this.TextBox, TextBox.TextProperty).UpdateSource();
        this._fireSelectionChangedEvent = true;
        return;
      case Key.Left:
        if (this.IsCurrentValueValid())
        {
          this.PerformKeyboardSelection(startPosition > 0 ? startPosition - 1 : 0);
          e.Handled = true;
        }
        this._fireSelectionChangedEvent = false;
        break;
      case Key.Right:
        if (this.IsCurrentValueValid())
        {
          this.PerformKeyboardSelection(startPosition + length);
          e.Handled = true;
        }
        this._fireSelectionChangedEvent = false;
        break;
      case Key.Add:
        if (this.AllowSpin && !this.IsReadOnly)
        {
          this.DoIncrement();
          e.Handled = true;
        }
        this._fireSelectionChangedEvent = false;
        break;
      case Key.Subtract:
        if (this.AllowSpin && !this.IsReadOnly)
        {
          this.DoDecrement();
          e.Handled = true;
        }
        this._fireSelectionChangedEvent = false;
        break;
      default:
        this._fireSelectionChangedEvent = false;
        break;
    }
    base.OnPreviewKeyDown(e);
  }

  private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
  {
    if (this._fireSelectionChangedEvent)
      this.PerformMouseSelection();
    else
      this._fireSelectionChangedEvent = true;
  }

  private void DateTimeUpDownBase_Loaded(object sender, RoutedEventArgs e) => this.InitSelection();

  protected virtual void InitializeDateTimeInfoList(T value)
  {
  }

  protected virtual bool IsCurrentValueValid() => true;

  protected virtual void PerformMouseSelection()
  {
    DateTimeInfo dateTimeInfo = this.GetDateTimeInfo(this.TextBox.SelectionStart);
    if (this.TextBox is MaskedTextBox && dateTimeInfo != null && dateTimeInfo.Type == DateTimePart.Other)
      this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Delegate) (() => this.Select(this.GetDateTimeInfo(dateTimeInfo.StartPosition + dateTimeInfo.Length))));
    else
      this.Select(dateTimeInfo);
  }

  protected virtual bool IsLowerThan(T value1, T value2) => false;

  protected virtual bool IsGreaterThan(T value1, T value2) => false;

  internal DateTimeInfo GetDateTimeInfo(int selectionStart)
  {
    return this._dateTimeInfoList.FirstOrDefault<DateTimeInfo>((Func<DateTimeInfo, bool>) (info => info.StartPosition <= selectionStart && selectionStart < info.StartPosition + info.Length));
  }

  internal DateTimeInfo GetDateTimeInfo(DateTimePart part)
  {
    return this._dateTimeInfoList.FirstOrDefault<DateTimeInfo>((Func<DateTimeInfo, bool>) (info => info.Type == part));
  }

  internal void Select(DateTimeInfo info)
  {
    if (info == null || info.Equals((object) this._selectedDateTimeInfo) || this.TextBox == null || string.IsNullOrEmpty(this.TextBox.Text))
      return;
    this._fireSelectionChangedEvent = false;
    this.TextBox.Select(info.StartPosition, info.Length);
    this._fireSelectionChangedEvent = true;
    this._selectedDateTimeInfo = info;
    this.SetCurrentValue(DateTimeUpDownBase<T>.CurrentDateTimePartProperty, (object) info.Type);
  }

  internal T CoerceValueMinMax(T value)
  {
    if (this.IsLowerThan(value, this.Minimum))
      return this.Minimum;
    return this.IsGreaterThan(value, this.Maximum) ? this.Maximum : value;
  }

  internal void ValidateDefaultMinMax(T value)
  {
    if (object.Equals((object) value, (object) this.DefaultValue))
      return;
    if (this.IsLowerThan(value, this.Minimum))
      throw new ArgumentOutOfRangeException("Minimum", $"Value must be greater than MinValue of {this.Minimum}");
    if (this.IsGreaterThan(value, this.Maximum))
      throw new ArgumentOutOfRangeException("Maximum", $"Value must be less than MaxValue of {this.Maximum}");
  }

  internal T GetClippedMinMaxValue(T value)
  {
    if (this.IsGreaterThan(value, this.Maximum))
      return this.Maximum;
    return this.IsLowerThan(value, this.Minimum) ? this.Minimum : value;
  }

  protected internal virtual void PerformKeyboardSelection(int nextSelectionStart)
  {
    this.TextBox.Focus();
    if (!this.UpdateValueOnEnterKey)
      this.CommitInput();
    int startPosition = this._selectedDateTimeInfo != null ? this._selectedDateTimeInfo.StartPosition : 0;
    if (nextSelectionStart - startPosition > 0)
      this.Select(this.GetNextDateTimeInfo(nextSelectionStart));
    else
      this.Select(this.GetPreviousDateTimeInfo(nextSelectionStart - 1));
  }

  private DateTimeInfo GetNextDateTimeInfo(int nextSelectionStart)
  {
    DateTimeInfo objA = this.GetDateTimeInfo(nextSelectionStart) ?? this._dateTimeInfoList.First<DateTimeInfo>();
    DateTimeInfo objB = objA;
    while (objA.Type == DateTimePart.Other)
    {
      objA = this.GetDateTimeInfo(objA.StartPosition + objA.Length) ?? this._dateTimeInfoList.First<DateTimeInfo>();
      if (object.Equals((object) objA, (object) objB))
        throw new InvalidOperationException("Couldn't find a valid DateTimeInfo.");
    }
    return objA;
  }

  private DateTimeInfo GetPreviousDateTimeInfo(int previousSelectionStart)
  {
    DateTimeInfo objA = this.GetDateTimeInfo(previousSelectionStart);
    if (objA == null && this._dateTimeInfoList.Count > 0)
      objA = this._dateTimeInfoList.Last<DateTimeInfo>();
    DateTimeInfo objB = objA;
    while (objA != null && objA.Type == DateTimePart.Other)
    {
      objA = this.GetDateTimeInfo(objA.StartPosition - 1) ?? this._dateTimeInfoList.Last<DateTimeInfo>();
      if (object.Equals((object) objA, (object) objB))
        throw new InvalidOperationException("Couldn't find a valid DateTimeInfo.");
    }
    return objA;
  }

  private void InitSelection()
  {
    if (this._selectedDateTimeInfo != null)
      return;
    this.Select(this.CurrentDateTimePart != DateTimePart.Other ? this.GetDateTimeInfo(this.CurrentDateTimePart) : this.GetDateTimeInfo(0));
  }
}
