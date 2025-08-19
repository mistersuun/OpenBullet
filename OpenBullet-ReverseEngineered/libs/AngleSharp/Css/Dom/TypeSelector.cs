// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.TypeSelector
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Css.Dom;

internal sealed class TypeSelector : ISelector
{
  private readonly string _type;

  public TypeSelector(string type) => this._type = type;

  public Priority Specificity => Priority.OneTag;

  public string Text => this._type;

  public void Accept(ISelectorVisitor visitor) => visitor.Type(this._type);

  public bool Match(IElement element, IElement scope) => this._type.Isi(element.LocalName);
}
