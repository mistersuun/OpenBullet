// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.LayoutPositionableGroup`1
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Xml;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

[Serializable]
public abstract class LayoutPositionableGroup<T> : 
  LayoutGroup<T>,
  ILayoutPositionableElement,
  ILayoutElement,
  INotifyPropertyChanged,
  INotifyPropertyChanging,
  ILayoutElementForFloatingWindow,
  ILayoutPositionableElementWithActualSize
  where T : class, ILayoutElement
{
  private static GridLengthConverter _gridLengthConverter = new GridLengthConverter();
  private GridLength _dockWidth = new GridLength(1.0, GridUnitType.Star);
  private GridLength _dockHeight = new GridLength(1.0, GridUnitType.Star);
  private bool _allowDuplicateContent = true;
  private bool _canRepositionItems = true;
  private double _dockMinWidth = 25.0;
  private double _dockMinHeight = 25.0;
  private double _floatingWidth;
  private double _floatingHeight;
  private double _floatingLeft;
  private double _floatingTop;
  private bool _isMaximized;
  [NonSerialized]
  private double _actualWidth;
  [NonSerialized]
  private double _actualHeight;

  public GridLength DockWidth
  {
    get => this._dockWidth;
    set
    {
      if (!(this.DockWidth != value))
        return;
      this.RaisePropertyChanging(nameof (DockWidth));
      this._dockWidth = value;
      this.RaisePropertyChanged(nameof (DockWidth));
      this.OnDockWidthChanged();
    }
  }

  public GridLength DockHeight
  {
    get => this._dockHeight;
    set
    {
      if (!(this.DockHeight != value))
        return;
      this.RaisePropertyChanging(nameof (DockHeight));
      this._dockHeight = value;
      this.RaisePropertyChanged(nameof (DockHeight));
      this.OnDockHeightChanged();
    }
  }

  public bool AllowDuplicateContent
  {
    get => this._allowDuplicateContent;
    set
    {
      if (this._allowDuplicateContent == value)
        return;
      this.RaisePropertyChanging(nameof (AllowDuplicateContent));
      this._allowDuplicateContent = value;
      this.RaisePropertyChanged(nameof (AllowDuplicateContent));
    }
  }

  public bool CanRepositionItems
  {
    get => this._canRepositionItems;
    set
    {
      if (this._canRepositionItems == value)
        return;
      this.RaisePropertyChanging(nameof (CanRepositionItems));
      this._canRepositionItems = value;
      this.RaisePropertyChanged(nameof (CanRepositionItems));
    }
  }

  public double DockMinWidth
  {
    get => this._dockMinWidth;
    set
    {
      if (this._dockMinWidth == value)
        return;
      MathHelper.AssertIsPositiveOrZero(value);
      this.RaisePropertyChanging(nameof (DockMinWidth));
      this._dockMinWidth = value;
      this.RaisePropertyChanged(nameof (DockMinWidth));
    }
  }

  public double DockMinHeight
  {
    get => this._dockMinHeight;
    set
    {
      if (this._dockMinHeight == value)
        return;
      MathHelper.AssertIsPositiveOrZero(value);
      this.RaisePropertyChanging(nameof (DockMinHeight));
      this._dockMinHeight = value;
      this.RaisePropertyChanged(nameof (DockMinHeight));
    }
  }

  public double FloatingWidth
  {
    get => this._floatingWidth;
    set
    {
      if (this._floatingWidth == value)
        return;
      this.RaisePropertyChanging(nameof (FloatingWidth));
      this._floatingWidth = value;
      this.RaisePropertyChanged(nameof (FloatingWidth));
    }
  }

  public double FloatingHeight
  {
    get => this._floatingHeight;
    set
    {
      if (this._floatingHeight == value)
        return;
      this.RaisePropertyChanging(nameof (FloatingHeight));
      this._floatingHeight = value;
      this.RaisePropertyChanged(nameof (FloatingHeight));
    }
  }

  public double FloatingLeft
  {
    get => this._floatingLeft;
    set
    {
      if (this._floatingLeft == value)
        return;
      this.RaisePropertyChanging(nameof (FloatingLeft));
      this._floatingLeft = value;
      this.RaisePropertyChanged(nameof (FloatingLeft));
    }
  }

  public double FloatingTop
  {
    get => this._floatingTop;
    set
    {
      if (this._floatingTop == value)
        return;
      this.RaisePropertyChanging(nameof (FloatingTop));
      this._floatingTop = value;
      this.RaisePropertyChanged(nameof (FloatingTop));
    }
  }

  public bool IsMaximized
  {
    get => this._isMaximized;
    set
    {
      if (this._isMaximized == value)
        return;
      this._isMaximized = value;
      this.RaisePropertyChanged(nameof (IsMaximized));
    }
  }

  double ILayoutPositionableElementWithActualSize.ActualWidth
  {
    get => this._actualWidth;
    set => this._actualWidth = value;
  }

  double ILayoutPositionableElementWithActualSize.ActualHeight
  {
    get => this._actualHeight;
    set => this._actualHeight = value;
  }

  public override void WriteXml(XmlWriter writer)
  {
    GridLength gridLength;
    if (this.DockWidth.Value == 1.0)
    {
      gridLength = this.DockWidth;
      if (gridLength.IsStar)
        goto label_3;
    }
    writer.WriteAttributeString("DockWidth", LayoutPositionableGroup<T>._gridLengthConverter.ConvertToInvariantString((object) this.DockWidth));
label_3:
    gridLength = this.DockHeight;
    if (gridLength.Value == 1.0)
    {
      gridLength = this.DockHeight;
      if (gridLength.IsStar)
        goto label_6;
    }
    writer.WriteAttributeString("DockHeight", LayoutPositionableGroup<T>._gridLengthConverter.ConvertToInvariantString((object) this.DockHeight));
label_6:
    if (this.DockMinWidth != 25.0)
      writer.WriteAttributeString("DocMinWidth", this.DockMinWidth.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (this.DockMinHeight != 25.0)
      writer.WriteAttributeString("DockMinHeight", this.DockMinHeight.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (this.FloatingWidth != 0.0)
      writer.WriteAttributeString("FloatingWidth", this.FloatingWidth.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (this.FloatingHeight != 0.0)
      writer.WriteAttributeString("FloatingHeight", this.FloatingHeight.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (this.FloatingLeft != 0.0)
      writer.WriteAttributeString("FloatingLeft", this.FloatingLeft.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (this.FloatingTop != 0.0)
      writer.WriteAttributeString("FloatingTop", this.FloatingTop.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (this.IsMaximized)
      writer.WriteAttributeString("IsMaximized", this.IsMaximized.ToString());
    base.WriteXml(writer);
  }

  public override void ReadXml(XmlReader reader)
  {
    if (reader.MoveToAttribute("DockWidth"))
      this._dockWidth = (GridLength) LayoutPositionableGroup<T>._gridLengthConverter.ConvertFromInvariantString(reader.Value);
    if (reader.MoveToAttribute("DockHeight"))
      this._dockHeight = (GridLength) LayoutPositionableGroup<T>._gridLengthConverter.ConvertFromInvariantString(reader.Value);
    if (reader.MoveToAttribute("DocMinWidth"))
      this._dockMinWidth = double.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    if (reader.MoveToAttribute("DocMinHeight"))
      this._dockMinHeight = double.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    if (reader.MoveToAttribute("FloatingWidth"))
      this._floatingWidth = double.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    if (reader.MoveToAttribute("FloatingHeight"))
      this._floatingHeight = double.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    if (reader.MoveToAttribute("FloatingLeft"))
      this._floatingLeft = double.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    if (reader.MoveToAttribute("FloatingTop"))
      this._floatingTop = double.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    if (reader.MoveToAttribute("IsMaximized"))
      this._isMaximized = bool.Parse(reader.Value);
    base.ReadXml(reader);
  }

  protected virtual void OnDockWidthChanged()
  {
  }

  protected virtual void OnDockHeightChanged()
  {
  }
}
