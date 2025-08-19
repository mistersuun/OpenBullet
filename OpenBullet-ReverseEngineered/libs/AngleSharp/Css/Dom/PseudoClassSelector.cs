// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.PseudoClassSelector
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using System;

#nullable disable
namespace AngleSharp.Css.Dom;

internal sealed class PseudoClassSelector : ISelector
{
  private readonly Predicate<IElement> _action;
  private readonly string _pseudoClass;

  public PseudoClassSelector(Predicate<IElement> action, string pseudoClass)
  {
    this._action = action;
    this._pseudoClass = pseudoClass;
  }

  public Priority Specificity => Priority.OneClass;

  public string Text => PseudoClassNames.Separator + this._pseudoClass;

  public void Accept(ISelectorVisitor visitor) => visitor.PseudoClass(this._pseudoClass);

  public bool Match(IElement element, IElement scope) => this._action(element);
}
