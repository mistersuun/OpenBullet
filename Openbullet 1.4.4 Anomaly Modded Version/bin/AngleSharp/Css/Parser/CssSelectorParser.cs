// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Parser.CssSelectorParser
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Css.Dom;
using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Css.Parser;

public class CssSelectorParser : ICssSelectorParser
{
  private readonly IAttributeSelectorFactory _attribute;
  private readonly IPseudoClassSelectorFactory _pseudoClass;
  private readonly IPseudoElementSelectorFactory _pseudoElement;

  public CssSelectorParser()
    : this((IBrowsingContext) null)
  {
  }

  internal CssSelectorParser(IBrowsingContext context)
  {
    if (context == null)
      context = BrowsingContext.NewFrom<ICssSelectorParser>((ICssSelectorParser) this);
    this._attribute = context.GetFactory<IAttributeSelectorFactory>();
    this._pseudoClass = context.GetFactory<IPseudoClassSelectorFactory>();
    this._pseudoElement = context.GetFactory<IPseudoElementSelectorFactory>();
  }

  public ISelector ParseSelector(string selectorText)
  {
    return new CssSelectorConstructor(new CssTokenizer(new StringSource(selectorText)), this._attribute, this._pseudoClass, this._pseudoElement).Parse();
  }
}
