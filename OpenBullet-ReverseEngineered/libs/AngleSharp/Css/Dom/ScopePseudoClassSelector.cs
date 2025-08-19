// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.ScopePseudoClassSelector
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;

#nullable disable
namespace AngleSharp.Css.Dom;

internal sealed class ScopePseudoClassSelector : ISelector
{
  public static readonly ISelector Instance = (ISelector) new ScopePseudoClassSelector();

  private ScopePseudoClassSelector()
  {
  }

  public Priority Specificity => Priority.OneClass;

  public string Text => PseudoClassNames.Separator + PseudoClassNames.Scope;

  public void Accept(ISelectorVisitor visitor) => visitor.PseudoClass(PseudoClassNames.Scope);

  public bool Match(IElement element, IElement scope)
  {
    IElement element1 = scope ?? element.Owner.DocumentElement;
    return element == element1;
  }
}
