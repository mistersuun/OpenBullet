// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlTableElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlTableElement(Document owner, string prefix = null) : 
  HtmlElement(owner, TagNames.Table, prefix, NodeFlags.Special | NodeFlags.Scoped | NodeFlags.HtmlTableSectionScoped | NodeFlags.HtmlTableScoped),
  IHtmlTableElement,
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers
{
  private HtmlCollection<IHtmlTableSectionElement> _bodies;
  private HtmlCollection<IHtmlTableRowElement> _rows;

  public IHtmlTableCaptionElement Caption
  {
    get
    {
      return this.ChildNodes.OfType<IHtmlTableCaptionElement>().FirstOrDefault<IHtmlTableCaptionElement>((Func<IHtmlTableCaptionElement, bool>) (m => m.LocalName.Is(TagNames.Caption)));
    }
    set
    {
      this.DeleteCaption();
      this.InsertChild(0, (INode) value);
    }
  }

  public IHtmlTableSectionElement Head
  {
    get
    {
      return this.ChildNodes.OfType<IHtmlTableSectionElement>().FirstOrDefault<IHtmlTableSectionElement>((Func<IHtmlTableSectionElement, bool>) (m => m.LocalName.Is(TagNames.Thead)));
    }
    set
    {
      this.DeleteHead();
      this.AppendChild((INode) value);
    }
  }

  public IHtmlCollection<IHtmlTableSectionElement> Bodies
  {
    get
    {
      return (IHtmlCollection<IHtmlTableSectionElement>) this._bodies ?? (IHtmlCollection<IHtmlTableSectionElement>) (this._bodies = new HtmlCollection<IHtmlTableSectionElement>((INode) this, false, (Func<IHtmlTableSectionElement, bool>) (m => m.LocalName.Is(TagNames.Tbody))));
    }
  }

  public IHtmlTableSectionElement Foot
  {
    get
    {
      return this.ChildNodes.OfType<IHtmlTableSectionElement>().FirstOrDefault<IHtmlTableSectionElement>((Func<IHtmlTableSectionElement, bool>) (m => m.LocalName.Is(TagNames.Tfoot)));
    }
    set
    {
      this.DeleteFoot();
      this.AppendChild((INode) value);
    }
  }

  public IEnumerable<IHtmlTableRowElement> AllRows
  {
    get
    {
      HtmlTableElement htmlTableElement = this;
      IEnumerable<IHtmlTableSectionElement> tableSectionElements = htmlTableElement.ChildNodes.OfType<IHtmlTableSectionElement>().Where<IHtmlTableSectionElement>((Func<IHtmlTableSectionElement, bool>) (m => m.LocalName.Is(TagNames.Thead)));
      IEnumerable<IHtmlTableSectionElement> foots = htmlTableElement.ChildNodes.OfType<IHtmlTableSectionElement>().Where<IHtmlTableSectionElement>((Func<IHtmlTableSectionElement, bool>) (m => m.LocalName.Is(TagNames.Tfoot)));
      foreach (IHtmlTableSectionElement tableSectionElement in tableSectionElements)
      {
        foreach (IHtmlTableRowElement row in (IEnumerable<IHtmlTableRowElement>) tableSectionElement.Rows)
          yield return row;
      }
      foreach (INode childNode in htmlTableElement.ChildNodes)
      {
        if (childNode is IHtmlTableSectionElement)
        {
          IHtmlTableSectionElement tableSectionElement = (IHtmlTableSectionElement) childNode;
          if (tableSectionElement.LocalName.Is(TagNames.Tbody))
          {
            foreach (IHtmlTableRowElement row in (IEnumerable<IHtmlTableRowElement>) tableSectionElement.Rows)
              yield return row;
          }
        }
        else if (childNode is IHtmlTableRowElement)
          yield return (IHtmlTableRowElement) childNode;
      }
      foreach (IHtmlTableSectionElement tableSectionElement in foots)
      {
        foreach (IHtmlTableRowElement row in (IEnumerable<IHtmlTableRowElement>) tableSectionElement.Rows)
          yield return row;
      }
    }
  }

  public IHtmlCollection<IHtmlTableRowElement> Rows
  {
    get
    {
      return (IHtmlCollection<IHtmlTableRowElement>) this._rows ?? (IHtmlCollection<IHtmlTableRowElement>) (this._rows = new HtmlCollection<IHtmlTableRowElement>(this.AllRows));
    }
  }

  public HorizontalAlignment Align
  {
    get
    {
      return this.GetOwnAttribute(AttributeNames.Align).ToEnum<HorizontalAlignment>(HorizontalAlignment.Left);
    }
    set => this.SetOwnAttribute(AttributeNames.Align, value.ToString());
  }

  public string BgColor
  {
    get => this.GetOwnAttribute(AttributeNames.BgColor);
    set => this.SetOwnAttribute(AttributeNames.BgColor, value);
  }

  public uint Border
  {
    get => this.GetOwnAttribute(AttributeNames.Border).ToInteger(0U);
    set => this.SetOwnAttribute(AttributeNames.Border, value.ToString());
  }

  public int CellPadding
  {
    get => this.GetOwnAttribute(AttributeNames.CellPadding).ToInteger(0);
    set => this.SetOwnAttribute(AttributeNames.CellPadding, value.ToString());
  }

  public int CellSpacing
  {
    get => this.GetOwnAttribute(AttributeNames.CellSpacing).ToInteger(0);
    set => this.SetOwnAttribute(AttributeNames.CellSpacing, value.ToString());
  }

  public TableFrames Frame
  {
    get => this.GetOwnAttribute(AttributeNames.Frame).ToEnum<TableFrames>(TableFrames.Void);
    set => this.SetOwnAttribute(AttributeNames.Frame, value.ToString());
  }

  public TableRules Rules
  {
    get => this.GetOwnAttribute(AttributeNames.Rules).ToEnum<TableRules>(TableRules.All);
    set => this.SetOwnAttribute(AttributeNames.Rules, value.ToString());
  }

  public string Summary
  {
    get => this.GetOwnAttribute(AttributeNames.Summary);
    set => this.SetOwnAttribute(AttributeNames.Summary, value);
  }

  public string Width
  {
    get => this.GetOwnAttribute(AttributeNames.Width);
    set => this.SetOwnAttribute(AttributeNames.Width, value);
  }

  public IHtmlTableRowElement InsertRowAt(int index = -1)
  {
    IHtmlCollection<IHtmlTableRowElement> rows = this.Rows;
    IHtmlTableRowElement element = this.Owner.CreateElement(TagNames.Tr) as IHtmlTableRowElement;
    if (index >= 0 && index < rows.Length)
    {
      IHtmlTableRowElement referenceElement = rows[index];
      referenceElement.ParentElement.InsertBefore((INode) element, (INode) referenceElement);
    }
    else if (rows.Length == 0)
    {
      IHtmlCollection<IHtmlTableSectionElement> bodies = this.Bodies;
      if (bodies.Length == 0)
        this.AppendChild((INode) this.Owner.CreateElement(TagNames.Tbody));
      bodies[bodies.Length - 1].AppendChild((INode) element);
    }
    else
      rows[rows.Length - 1].ParentElement.AppendChild((INode) element);
    return element;
  }

  public void RemoveRowAt(int index)
  {
    IHtmlCollection<IHtmlTableRowElement> rows = this.Rows;
    if (index < 0 || index >= rows.Length)
      return;
    rows[index].Remove();
  }

  public IHtmlTableSectionElement CreateHead()
  {
    IHtmlTableSectionElement child = this.Head;
    if (child == null)
    {
      child = this.Owner.CreateElement(TagNames.Thead) as IHtmlTableSectionElement;
      this.AppendChild((INode) child);
    }
    return child;
  }

  public IHtmlTableSectionElement CreateBody()
  {
    IHtmlTableSectionElement node = this.Bodies.LastOrDefault<IHtmlTableSectionElement>();
    IHtmlTableSectionElement element = this.Owner.CreateElement(TagNames.Tbody) as IHtmlTableSectionElement;
    int length = this.ChildNodes.Length;
    int index = node != null ? node.Index() + 1 : length;
    if (index == length)
      this.AppendChild((INode) element);
    else
      this.InsertChild(index, (INode) element);
    return element;
  }

  public void DeleteHead() => this.Head?.Remove();

  public IHtmlTableSectionElement CreateFoot()
  {
    IHtmlTableSectionElement child = this.Foot;
    if (child == null)
    {
      child = this.Owner.CreateElement(TagNames.Tfoot) as IHtmlTableSectionElement;
      this.AppendChild((INode) child);
    }
    return child;
  }

  public void DeleteFoot() => this.Foot?.Remove();

  public IHtmlTableCaptionElement CreateCaption()
  {
    IHtmlTableCaptionElement child = this.Caption;
    if (child == null)
    {
      child = this.Owner.CreateElement(TagNames.Caption) as IHtmlTableCaptionElement;
      this.InsertChild(0, (INode) child);
    }
    return child;
  }

  public void DeleteCaption() => this.Caption?.Remove();
}
