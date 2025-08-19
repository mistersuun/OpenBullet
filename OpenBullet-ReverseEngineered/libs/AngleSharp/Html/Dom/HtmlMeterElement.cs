// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlMeterElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Text;
using System;
using System.Globalization;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlMeterElement : 
  HtmlElement,
  IHtmlMeterElement,
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers,
  ILabelabelElement
{
  private readonly NodeList _labels;

  public HtmlMeterElement(Document owner, string prefix = null)
    : base(owner, TagNames.Meter, prefix)
  {
    this._labels = new NodeList();
  }

  public INodeList Labels => (INodeList) this._labels;

  public double Value
  {
    get
    {
      return this.GetOwnAttribute(AttributeNames.Value).ToDouble().Constraint(this.Minimum, this.Maximum);
    }
    set
    {
      this.SetOwnAttribute(AttributeNames.Value, value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
    }
  }

  public double Maximum
  {
    get
    {
      return this.GetOwnAttribute(AttributeNames.Max).ToDouble(1.0).Constraint(this.Minimum, double.PositiveInfinity);
    }
    set
    {
      this.SetOwnAttribute(AttributeNames.Max, value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
    }
  }

  public double Minimum
  {
    get => this.GetOwnAttribute(AttributeNames.Min).ToDouble();
    set
    {
      this.SetOwnAttribute(AttributeNames.Min, value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
    }
  }

  public double Low
  {
    get
    {
      return this.GetOwnAttribute(AttributeNames.Low).ToDouble(this.Minimum).Constraint(this.Minimum, this.Maximum);
    }
    set
    {
      this.SetOwnAttribute(AttributeNames.Low, value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
    }
  }

  public double High
  {
    get
    {
      return this.GetOwnAttribute(AttributeNames.High).ToDouble(this.Maximum).Constraint(this.Low, this.Maximum);
    }
    set
    {
      this.SetOwnAttribute(AttributeNames.High, value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
    }
  }

  public double Optimum
  {
    get
    {
      return this.GetOwnAttribute(AttributeNames.Optimum).ToDouble((this.Maximum + this.Minimum) * 0.5).Constraint(this.Minimum, this.Maximum);
    }
    set
    {
      this.SetOwnAttribute(AttributeNames.Optimum, value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
    }
  }
}
