// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.IdSelector
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Css.Dom;

internal sealed class IdSelector : ISelector
{
  private readonly string _id;

  public IdSelector(string id) => this._id = id;

  public Priority Specificity => Priority.OneId;

  public string Text => "#" + this._id;

  public void Accept(ISelectorVisitor visitor) => visitor.Id(this._id);

  public bool Match(IElement element, IElement scope) => element.Id.Is(this._id);
}
