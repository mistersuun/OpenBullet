// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.PrettyMarkupFormatter
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Text;
using System.Linq;

#nullable disable
namespace AngleSharp.Html;

public class PrettyMarkupFormatter : IMarkupFormatter
{
  private string _indentString;
  private string _newLineString;
  private int _indentCount;

  public PrettyMarkupFormatter()
  {
    this._indentCount = 0;
    this._indentString = "\t";
    this._newLineString = "\n";
  }

  public string Indentation
  {
    get => this._indentString;
    set => this._indentString = value;
  }

  public string NewLine
  {
    get => this._newLineString;
    set => this._newLineString = value;
  }

  string IMarkupFormatter.Comment(IComment comment)
  {
    return this.IndentBefore() + HtmlMarkupFormatter.Instance.Comment(comment);
  }

  string IMarkupFormatter.Doctype(IDocumentType doctype)
  {
    string str = string.Empty;
    if (doctype.ParentElement != null)
      str = this.IndentBefore();
    return str + HtmlMarkupFormatter.Instance.Doctype(doctype) + this.NewLine;
  }

  string IMarkupFormatter.Processing(IProcessingInstruction processing)
  {
    return this.IndentBefore() + HtmlMarkupFormatter.Instance.Processing(processing);
  }

  string IMarkupFormatter.Text(ICharacterData text)
  {
    string data = text.Data;
    string str = string.Empty;
    string content = data.Replace('\n', ' ');
    if (!(text.NextSibling is ICharacterData))
      content = content.TrimEnd();
    if (content.Length > 0 && !(text.PreviousSibling is ICharacterData) && content[0].IsSpaceCharacter())
    {
      content = content.TrimStart();
      str = this.IndentBefore();
    }
    return str + HtmlMarkupFormatter.EscapeText(content);
  }

  string IMarkupFormatter.OpenTag(IElement element, bool selfClosing)
  {
    string str = string.Empty;
    IText previousSibling = element.PreviousSibling as IText;
    if (element.ParentElement != null && (previousSibling == null || PrettyMarkupFormatter.EndsWithSpace((ICharacterData) previousSibling)))
      str = this.IndentBefore();
    ++this._indentCount;
    return str + HtmlMarkupFormatter.Instance.OpenTag(element, selfClosing);
  }

  string IMarkupFormatter.CloseTag(IElement element, bool selfClosing)
  {
    --this._indentCount;
    string str = string.Empty;
    IText lastChild = element.LastChild as IText;
    if (element.HasChildNodes && (lastChild == null || PrettyMarkupFormatter.EndsWithSpace((ICharacterData) lastChild)))
      str = this.IndentBefore();
    return str + HtmlMarkupFormatter.Instance.CloseTag(element, selfClosing);
  }

  string IMarkupFormatter.Attribute(IAttr attribute)
  {
    return HtmlMarkupFormatter.Instance.Attribute(attribute);
  }

  private static bool EndsWithSpace(ICharacterData text)
  {
    string data = text.Data;
    return data.Length > 0 && data[data.Length - 1].IsSpaceCharacter();
  }

  private string IndentBefore()
  {
    return this._newLineString + string.Join(string.Empty, Enumerable.Repeat<string>(this._indentString, this._indentCount));
  }
}
