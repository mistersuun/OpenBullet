// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.DateElement
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public sealed class DateElement : IComparable<DateElement>
{
  internal Rect PlacementRectangle;
  private readonly int _originalIndex;
  private readonly DateTime _date;
  private readonly DateTime _dateEnd;
  private readonly UIElement _element;

  public DateTime Date => this._date;

  public DateTime DateEnd => this._dateEnd;

  public UIElement Element => this._element;

  internal DateElement(UIElement element, DateTime date, DateTime dateEnd)
    : this(element, date, dateEnd, -1)
  {
  }

  internal DateElement(UIElement element, DateTime date, DateTime dateEnd, int originalIndex)
  {
    this._element = element;
    this._date = date;
    this._dateEnd = dateEnd;
    this._originalIndex = originalIndex;
  }

  public override string ToString()
  {
    if (!(this.Element is FrameworkElement element))
      return base.ToString();
    return element.Tag != null ? element.Tag.ToString() : element.Name;
  }

  public int CompareTo(DateElement d)
  {
    int num = this.Date.CompareTo(d.Date);
    if (num != 0)
      return num;
    if (this._originalIndex < 0)
      return -this.DateEnd.CompareTo(d.DateEnd);
    return this._originalIndex >= d._originalIndex ? 1 : -1;
  }
}
