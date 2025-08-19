// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Primitives.InputBase
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.Primitives;

public abstract class InputBase : Control
{
  public static readonly DependencyProperty AllowTextInputProperty = DependencyProperty.Register(nameof (AllowTextInput), typeof (bool), typeof (InputBase), (PropertyMetadata) new UIPropertyMetadata((object) true, new PropertyChangedCallback(InputBase.OnAllowTextInputChanged)));
  public static readonly DependencyProperty CultureInfoProperty = DependencyProperty.Register(nameof (CultureInfo), typeof (CultureInfo), typeof (InputBase), (PropertyMetadata) new UIPropertyMetadata((object) CultureInfo.CurrentCulture, new PropertyChangedCallback(InputBase.OnCultureInfoChanged)));
  public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof (IsReadOnly), typeof (bool), typeof (InputBase), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(InputBase.OnReadOnlyChanged)));
  public static readonly DependencyProperty IsUndoEnabledProperty = DependencyProperty.Register(nameof (IsUndoEnabled), typeof (bool), typeof (InputBase), (PropertyMetadata) new UIPropertyMetadata((object) true, new PropertyChangedCallback(InputBase.OnIsUndoEnabledChanged)));
  public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof (Text), typeof (string), typeof (InputBase), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(InputBase.OnTextChanged), (CoerceValueCallback) null, false, UpdateSourceTrigger.LostFocus));
  public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(nameof (TextAlignment), typeof (TextAlignment), typeof (InputBase), (PropertyMetadata) new UIPropertyMetadata((object) TextAlignment.Left));
  public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(nameof (Watermark), typeof (object), typeof (InputBase), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty WatermarkTemplateProperty = DependencyProperty.Register(nameof (WatermarkTemplate), typeof (DataTemplate), typeof (InputBase), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));

  public bool AllowTextInput
  {
    get => (bool) this.GetValue(InputBase.AllowTextInputProperty);
    set => this.SetValue(InputBase.AllowTextInputProperty, (object) value);
  }

  private static void OnAllowTextInputChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is InputBase inputBase))
      return;
    inputBase.OnAllowTextInputChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnAllowTextInputChanged(bool oldValue, bool newValue)
  {
  }

  public CultureInfo CultureInfo
  {
    get => (CultureInfo) this.GetValue(InputBase.CultureInfoProperty);
    set => this.SetValue(InputBase.CultureInfoProperty, (object) value);
  }

  private static void OnCultureInfoChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is InputBase inputBase))
      return;
    inputBase.OnCultureInfoChanged((CultureInfo) e.OldValue, (CultureInfo) e.NewValue);
  }

  protected virtual void OnCultureInfoChanged(CultureInfo oldValue, CultureInfo newValue)
  {
  }

  public bool IsReadOnly
  {
    get => (bool) this.GetValue(InputBase.IsReadOnlyProperty);
    set => this.SetValue(InputBase.IsReadOnlyProperty, (object) value);
  }

  private static void OnReadOnlyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is InputBase inputBase))
      return;
    inputBase.OnReadOnlyChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnReadOnlyChanged(bool oldValue, bool newValue)
  {
  }

  public bool IsUndoEnabled
  {
    get => (bool) this.GetValue(InputBase.IsUndoEnabledProperty);
    set => this.SetValue(InputBase.IsUndoEnabledProperty, (object) value);
  }

  private static void OnIsUndoEnabledChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is InputBase inputBase))
      return;
    inputBase.OnIsUndoEnabledChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIsUndoEnabledChanged(bool oldValue, bool newValue)
  {
  }

  public string Text
  {
    get => (string) this.GetValue(InputBase.TextProperty);
    set => this.SetValue(InputBase.TextProperty, (object) value);
  }

  private static void OnTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is InputBase inputBase))
      return;
    inputBase.OnTextChanged((string) e.OldValue, (string) e.NewValue);
  }

  protected virtual void OnTextChanged(string oldValue, string newValue)
  {
  }

  public TextAlignment TextAlignment
  {
    get => (TextAlignment) this.GetValue(InputBase.TextAlignmentProperty);
    set => this.SetValue(InputBase.TextAlignmentProperty, (object) value);
  }

  public object Watermark
  {
    get => this.GetValue(InputBase.WatermarkProperty);
    set => this.SetValue(InputBase.WatermarkProperty, value);
  }

  public DataTemplate WatermarkTemplate
  {
    get => (DataTemplate) this.GetValue(InputBase.WatermarkTemplateProperty);
    set => this.SetValue(InputBase.WatermarkTemplateProperty, (object) value);
  }
}
