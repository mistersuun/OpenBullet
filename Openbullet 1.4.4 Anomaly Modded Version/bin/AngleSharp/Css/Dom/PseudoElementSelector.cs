// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.PseudoElementSelector
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using System;

#nullable disable
namespace AngleSharp.Css.Dom;

internal sealed class PseudoElementSelector : ISelector
{
  private readonly Predicate<IElement> _action;
  private readonly string _pseudoElement;

  public PseudoElementSelector(Predicate<IElement> action, string pseudoElement)
  {
    this._action = action;
    this._pseudoElement = pseudoElement;
  }

  public Priority Specificity => Priority.OneTag;

  public string Text => PseudoElementNames.Separator + this._pseudoElement;

  public void Accept(ISelectorVisitor visitor) => visitor.PseudoElement(this._pseudoElement);

  public bool Match(IElement element, IElement scope) => this._action(element);
}
