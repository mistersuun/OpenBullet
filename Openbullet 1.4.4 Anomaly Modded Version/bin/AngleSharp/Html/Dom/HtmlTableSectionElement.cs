// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlTableSectionElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlTableSectionElement(Document owner, string name = null, string prefix = null) : 
  HtmlElement(owner, name ?? TagNames.Tbody, prefix, NodeFlags.Special | NodeFlags.ImplicitelyClosed | NodeFlags.HtmlTableSectionScoped),
  IHtmlTableSectionElement,
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
  private HtmlCollection<IHtmlTableRowElement> _rows;

  public HorizontalAlignment Align
  {
    get
    {
      return this.GetOwnAttribute(AttributeNames.Align).ToEnum<HorizontalAlignment>(HorizontalAlignment.Center);
    }
    set => this.SetOwnAttribute(AttributeNames.Align, value.ToString());
  }

  public IHtmlCollection<IHtmlTableRowElement> Rows
  {
    get
    {
      return (IHtmlCollection<IHtmlTableRowElement>) this._rows ?? (IHtmlCollection<IHtmlTableRowElement>) (this._rows = new HtmlCollection<IHtmlTableRowElement>((INode) this, false));
    }
  }

  public VerticalAlignment VAlign
  {
    get
    {
      return this.GetOwnAttribute(AttributeNames.Valign).ToEnum<VerticalAlignment>(VerticalAlignment.Middle);
    }
    set => this.SetOwnAttribute(AttributeNames.Valign, value.ToString());
  }

  public IHtmlTableRowElement InsertRowAt(int index = -1)
  {
    IHtmlCollection<IHtmlTableRowElement> rows = this.Rows;
    IHtmlTableRowElement element = this.Owner.CreateElement(TagNames.Tr) as IHtmlTableRowElement;
    if (index >= 0 && index < rows.Length)
      this.InsertBefore((INode) element, (INode) rows[index]);
    else
      this.AppendChild((INode) element);
    return element;
  }

  public void RemoveRowAt(int index)
  {
    IHtmlCollection<IHtmlTableRowElement> rows = this.Rows;
    if (index < 0 || index >= rows.Length)
      return;
    rows[index].Remove();
  }
}
