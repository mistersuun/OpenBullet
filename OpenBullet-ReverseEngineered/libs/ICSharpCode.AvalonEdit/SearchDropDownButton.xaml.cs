// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Search.DropDownButton
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Windows;
using System.Windows.Controls.Primitives;

#nullable disable
namespace ICSharpCode.AvalonEdit.Search;

public partial class DropDownButton : ButtonBase
{
  public static readonly DependencyProperty DropDownContentProperty = DependencyProperty.Register(nameof (DropDownContent), typeof (Popup), typeof (DropDownButton), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
  protected static readonly DependencyPropertyKey IsDropDownContentOpenPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsDropDownContentOpen), typeof (bool), typeof (DropDownButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty IsDropDownContentOpenProperty = DropDownButton.IsDropDownContentOpenPropertyKey.DependencyProperty;

  static DropDownButton()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DropDownButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DropDownButton)));
  }

  public Popup DropDownContent
  {
    get => (Popup) this.GetValue(DropDownButton.DropDownContentProperty);
    set => this.SetValue(DropDownButton.DropDownContentProperty, (object) value);
  }

  public bool IsDropDownContentOpen
  {
    get => (bool) this.GetValue(DropDownButton.IsDropDownContentOpenProperty);
    protected set => this.SetValue(DropDownButton.IsDropDownContentOpenPropertyKey, (object) value);
  }

  protected override void OnClick()
  {
    if (this.DropDownContent == null || this.IsDropDownContentOpen)
      return;
    this.DropDownContent.Placement = PlacementMode.Bottom;
    this.DropDownContent.PlacementTarget = (UIElement) this;
    this.DropDownContent.IsOpen = true;
    this.DropDownContent.Closed += new EventHandler(this.DropDownContent_Closed);
    this.IsDropDownContentOpen = true;
  }

  private void DropDownContent_Closed(object sender, EventArgs e)
  {
    ((Popup) sender).Closed -= new EventHandler(this.DropDownContent_Closed);
    this.IsDropDownContentOpen = false;
  }
}
