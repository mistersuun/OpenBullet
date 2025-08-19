// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.NamespaceSelector
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;

#nullable disable
namespace AngleSharp.Css.Dom;

internal sealed class NamespaceSelector : ISelector
{
  private readonly string _prefix;

  public NamespaceSelector(string prefix) => this._prefix = prefix;

  public Priority Specificity => Priority.Zero;

  public string Text => this._prefix;

  public bool Match(IElement element, IElement scope) => element.MatchesCssNamespace(this._prefix);

  public void Accept(ISelectorVisitor visitor) => visitor.Type(this._prefix);
}
