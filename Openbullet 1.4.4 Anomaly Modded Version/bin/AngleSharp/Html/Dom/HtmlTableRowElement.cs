// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlTableRowElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Text;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlTableRowElement(Document owner, string prefix = null) : 
  HtmlElement(owner, TagNames.Tr, prefix, NodeFlags.Special | NodeFlags.ImplicitelyClosed),
  IHtmlTableRowElement,
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
  private HtmlCollection<IHtmlTableCellElement> _cells;

  public HorizontalAlignment Align
  {
    get
    {
      return this.GetOwnAttribute(AttributeNames.Align).ToEnum<HorizontalAlignment>(HorizontalAlignment.Left);
    }
    set => this.SetOwnAttribute(AttributeNames.Align, value.ToString());
  }

  public VerticalAlignment VAlign
  {
    get
    {
      return this.GetOwnAttribute(AttributeNames.Valign).ToEnum<VerticalAlignment>(VerticalAlignment.Middle);
    }
    set => this.SetOwnAttribute(AttributeNames.Valign, value.ToString());
  }

  public string BgColor
  {
    get => this.GetOwnAttribute(AttributeNames.BgColor);
    set => this.SetOwnAttribute(AttributeNames.BgColor, value);
  }

  public IHtmlCollection<IHtmlTableCellElement> Cells
  {
    get
    {
      return (IHtmlCollection<IHtmlTableCellElement>) this._cells ?? (IHtmlCollection<IHtmlTableCellElement>) (this._cells = new HtmlCollection<IHtmlTableCellElement>((INode) this, false));
    }
  }

  public int Index
  {
    get
    {
      IHtmlTableElement ancestor = this.GetAncestor<IHtmlTableElement>();
      return ancestor == null ? -1 : ((IEnumerable<INode>) ancestor.Rows).Index((INode) this);
    }
  }

  public int IndexInSection
  {
    get
    {
      return !(this.ParentElement is IHtmlTableSectionElement parentElement) ? this.Index : ((IEnumerable<INode>) parentElement.Rows).Index((INode) this);
    }
  }

  public IHtmlTableCellElement InsertCellAt(int index = -1, TableCellKind tableCellKind = TableCellKind.Td)
  {
    IHtmlCollection<IHtmlTableCellElement> cells = this.Cells;
    IHtmlTableCellElement element = this.Owner.CreateElement(tableCellKind == TableCellKind.Td ? TagNames.Td : TagNames.Th) as IHtmlTableCellElement;
    if (index >= 0 && index < cells.Length)
      this.InsertBefore((INode) element, (INode) cells[index]);
    else
      this.AppendChild((INode) element);
    return element;
  }

  public void RemoveCellAt(int index)
  {
    IHtmlCollection<IHtmlTableCellElement> cells = this.Cells;
    if (index < 0)
      index = cells.Length + index;
    if (index < 0 || index >= cells.Length)
      return;
    cells[index].Remove();
  }
}
