// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Parser.CssSelectorConstructor
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#nullable disable
namespace AngleSharp.Css.Parser;

internal sealed class CssSelectorConstructor
{
  private static readonly Dictionary<string, Func<CssSelectorConstructor, CssSelectorConstructor.FunctionState>> pseudoClassFunctions = new Dictionary<string, Func<CssSelectorConstructor, CssSelectorConstructor.FunctionState>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
  {
    {
      PseudoClassNames.NthChild,
      (Func<CssSelectorConstructor, CssSelectorConstructor.FunctionState>) (ctx => (CssSelectorConstructor.FunctionState) new CssSelectorConstructor.ChildFunctionState((Func<int, int, ISelector, ISelector>) ((step, offset, kind) => (ISelector) new FirstChildSelector(step, offset, kind)), ctx))
    },
    {
      PseudoClassNames.NthLastChild,
      (Func<CssSelectorConstructor, CssSelectorConstructor.FunctionState>) (ctx => (CssSelectorConstructor.FunctionState) new CssSelectorConstructor.ChildFunctionState((Func<int, int, ISelector, ISelector>) ((step, offset, kind) => (ISelector) new LastChildSelector(step, offset, kind)), ctx))
    },
    {
      PseudoClassNames.NthOfType,
      (Func<CssSelectorConstructor, CssSelectorConstructor.FunctionState>) (ctx => (CssSelectorConstructor.FunctionState) new CssSelectorConstructor.ChildFunctionState((Func<int, int, ISelector, ISelector>) ((step, offset, kind) => (ISelector) new FirstTypeSelector(step, offset, kind)), ctx, false))
    },
    {
      PseudoClassNames.NthLastOfType,
      (Func<CssSelectorConstructor, CssSelectorConstructor.FunctionState>) (ctx => (CssSelectorConstructor.FunctionState) new CssSelectorConstructor.ChildFunctionState((Func<int, int, ISelector, ISelector>) ((step, offset, kind) => (ISelector) new LastTypeSelector(step, offset, kind)), ctx, false))
    },
    {
      PseudoClassNames.NthColumn,
      (Func<CssSelectorConstructor, CssSelectorConstructor.FunctionState>) (ctx => (CssSelectorConstructor.FunctionState) new CssSelectorConstructor.ChildFunctionState((Func<int, int, ISelector, ISelector>) ((step, offset, kind) => (ISelector) new FirstColumnSelector(step, offset, kind)), ctx, false))
    },
    {
      PseudoClassNames.NthLastColumn,
      (Func<CssSelectorConstructor, CssSelectorConstructor.FunctionState>) (ctx => (CssSelectorConstructor.FunctionState) new CssSelectorConstructor.ChildFunctionState((Func<int, int, ISelector, ISelector>) ((step, offset, kind) => (ISelector) new LastColumnSelector(step, offset, kind)), ctx, false))
    },
    {
      PseudoClassNames.Not,
      (Func<CssSelectorConstructor, CssSelectorConstructor.FunctionState>) (ctx => (CssSelectorConstructor.FunctionState) new CssSelectorConstructor.NotFunctionState(ctx))
    },
    {
      PseudoClassNames.Dir,
      (Func<CssSelectorConstructor, CssSelectorConstructor.FunctionState>) (ctx => (CssSelectorConstructor.FunctionState) new CssSelectorConstructor.DirFunctionState())
    },
    {
      PseudoClassNames.Lang,
      (Func<CssSelectorConstructor, CssSelectorConstructor.FunctionState>) (ctx => (CssSelectorConstructor.FunctionState) new CssSelectorConstructor.LangFunctionState())
    },
    {
      PseudoClassNames.Contains,
      (Func<CssSelectorConstructor, CssSelectorConstructor.FunctionState>) (ctx => (CssSelectorConstructor.FunctionState) new CssSelectorConstructor.ContainsFunctionState())
    },
    {
      PseudoClassNames.Has,
      (Func<CssSelectorConstructor, CssSelectorConstructor.FunctionState>) (ctx => (CssSelectorConstructor.FunctionState) new CssSelectorConstructor.HasFunctionState(ctx))
    },
    {
      PseudoClassNames.Matches,
      (Func<CssSelectorConstructor, CssSelectorConstructor.FunctionState>) (ctx => (CssSelectorConstructor.FunctionState) new CssSelectorConstructor.MatchesFunctionState(ctx))
    },
    {
      PseudoClassNames.HostContext,
      (Func<CssSelectorConstructor, CssSelectorConstructor.FunctionState>) (ctx => (CssSelectorConstructor.FunctionState) new CssSelectorConstructor.HostContextFunctionState(ctx))
    }
  };
  private readonly CssTokenizer _tokenizer;
  private readonly Stack<CssCombinator> _combinators;
  private readonly IAttributeSelectorFactory _attributeSelector;
  private readonly IPseudoElementSelectorFactory _pseudoElementSelector;
  private readonly IPseudoClassSelectorFactory _pseudoClassSelector;
  private CssSelectorConstructor.State _state;
  private ISelector _temp;
  private ListSelector _group;
  private ComplexSelector _complex;
  private string _attrName;
  private string _attrValue;
  private bool _attrInsensitive;
  private string _attrOp;
  private string _attrNs;
  private bool _valid;
  private bool _nested;
  private bool _ready;
  private CssSelectorConstructor.FunctionState _function;
  private bool _invoked;

  public CssSelectorConstructor(
    CssTokenizer tokenizer,
    IAttributeSelectorFactory attributeSelector,
    IPseudoClassSelectorFactory pseudoClassSelector,
    IPseudoElementSelectorFactory pseudoElementSelector,
    bool invoked = false)
  {
    this._tokenizer = tokenizer;
    this._invoked = invoked;
    this._combinators = new Stack<CssCombinator>();
    this._attributeSelector = attributeSelector;
    this._pseudoClassSelector = pseudoClassSelector;
    this._pseudoElementSelector = pseudoElementSelector;
    this._attrOp = string.Empty;
    this._attrInsensitive = false;
    this._state = CssSelectorConstructor.State.Data;
    this._valid = true;
    this._ready = true;
  }

  public bool IsValid => this._invoked && this._valid && this._ready;

  public bool IsNested => this._nested;

  public ISelector Parse()
  {
    for (CssSelectorToken token = this._tokenizer.Get(); token.Type != CssTokenType.EndOfFile; token = this._tokenizer.Get())
      this.Apply(token);
    return this.GetResult();
  }

  private ISelector GetResult()
  {
    if (!this.IsValid)
      return (ISelector) null;
    if (this._complex != null)
    {
      this._complex.ConcludeSelector(this._temp);
      this._temp = (ISelector) this._complex;
      this._complex = (ComplexSelector) null;
    }
    if (this._group == null || this._group.Length == 0)
      return this._temp ?? AllSelector.Instance;
    if (this._temp == null && this._group.Length == 1)
      return this._group[0];
    if (this._temp != null)
    {
      this._group.Add(this._temp);
      this._temp = (ISelector) null;
    }
    return (ISelector) this._group;
  }

  private void Apply(CssSelectorToken token)
  {
    this._invoked = true;
    switch (this._state)
    {
      case CssSelectorConstructor.State.Data:
        this.OnData(token);
        break;
      case CssSelectorConstructor.State.Function:
        this.OnFunctionState(token);
        break;
      case CssSelectorConstructor.State.Attribute:
        this.OnAttribute(token);
        break;
      case CssSelectorConstructor.State.AttributeOperator:
        this.OnAttributeOperator(token);
        break;
      case CssSelectorConstructor.State.AttributeValue:
        this.OnAttributeValue(token);
        break;
      case CssSelectorConstructor.State.AttributeEnd:
        this.OnAttributeEnd(token);
        break;
      case CssSelectorConstructor.State.PseudoClass:
        this.OnPseudoClass(token);
        break;
      case CssSelectorConstructor.State.PseudoElement:
        this.OnPseudoElement(token);
        break;
      default:
        this._valid = false;
        break;
    }
  }

  private void OnData(CssSelectorToken token)
  {
    switch (token.Type)
    {
      case CssTokenType.Hash:
        this.Insert((ISelector) new IdSelector(token.Data));
        this._ready = true;
        break;
      case CssTokenType.Class:
        this.Insert((ISelector) new ClassSelector(token.Data));
        this._ready = true;
        break;
      case CssTokenType.Ident:
        this.Insert((ISelector) new TypeSelector(token.Data));
        this._ready = true;
        break;
      case CssTokenType.Column:
        this.Insert(CssCombinator.Column);
        this._ready = false;
        break;
      case CssTokenType.Descendent:
        this.Insert(CssCombinator.Descendent);
        this._ready = false;
        break;
      case CssTokenType.Deep:
        this.Insert(CssCombinator.Deep);
        this._ready = false;
        break;
      case CssTokenType.Delim:
        this.OnDelim(token);
        break;
      case CssTokenType.SquareBracketOpen:
        this._attrName = (string) null;
        this._attrValue = (string) null;
        this._attrOp = string.Empty;
        this._attrNs = (string) null;
        this._state = CssSelectorConstructor.State.Attribute;
        this._ready = false;
        break;
      case CssTokenType.Colon:
        this._state = CssSelectorConstructor.State.PseudoClass;
        this._ready = false;
        break;
      case CssTokenType.Comma:
        this.InsertOr();
        this._ready = false;
        break;
      case CssTokenType.Whitespace:
        this.Insert(CssCombinator.Descendent);
        break;
      default:
        this._valid = false;
        break;
    }
  }

  private void OnAttribute(CssSelectorToken token)
  {
    if (token.Type == CssTokenType.Whitespace)
      return;
    if (token.Type == CssTokenType.Ident || token.Type == CssTokenType.String)
    {
      this._state = CssSelectorConstructor.State.AttributeOperator;
      this._attrName = token.Data;
    }
    else if (token.Type == CssTokenType.Delim && token.Data.Is(CombinatorSymbols.Pipe))
    {
      this._state = CssSelectorConstructor.State.Attribute;
      this._attrNs = string.Empty;
    }
    else if (token.Type == CssTokenType.Delim && token.Data.Is("*"))
    {
      this._state = CssSelectorConstructor.State.AttributeOperator;
      this._attrName = "*";
    }
    else
    {
      this._state = CssSelectorConstructor.State.Data;
      this._valid = false;
    }
  }

  private void OnAttributeOperator(CssSelectorToken token)
  {
    if (token.Type == CssTokenType.Whitespace)
      return;
    if (token.Type == CssTokenType.SquareBracketClose)
    {
      this._state = CssSelectorConstructor.State.AttributeValue;
      this.OnAttributeEnd(token);
    }
    else if (token.Type == CssTokenType.Match || token.Type == CssTokenType.Delim)
    {
      this._state = CssSelectorConstructor.State.AttributeValue;
      this._attrOp = token.Data;
      if (!(this._attrOp == CombinatorSymbols.Pipe))
        return;
      this._attrNs = this._attrName;
      this._attrName = (string) null;
      this._attrOp = string.Empty;
      this._state = CssSelectorConstructor.State.Attribute;
    }
    else
    {
      this._state = CssSelectorConstructor.State.AttributeEnd;
      this._valid = false;
    }
  }

  private void OnAttributeValue(CssSelectorToken token)
  {
    if (token.Type == CssTokenType.Whitespace)
      return;
    if (token.Type == CssTokenType.Ident || token.Type == CssTokenType.String || token.Type == CssTokenType.Number)
    {
      this._state = CssSelectorConstructor.State.AttributeEnd;
      this._attrValue = token.Data;
    }
    else
    {
      this._state = CssSelectorConstructor.State.Data;
      this._valid = false;
    }
  }

  private void OnAttributeEnd(CssSelectorToken token)
  {
    if (!this._attrInsensitive && token.Type == CssTokenType.Ident && token.Data == "i")
    {
      this._attrInsensitive = true;
    }
    else
    {
      if (token.Type == CssTokenType.Whitespace)
        return;
      this._state = CssSelectorConstructor.State.Data;
      this._ready = true;
      if (token.Type == CssTokenType.SquareBracketClose)
      {
        ISelector selector = this._attributeSelector.Create(this._attrOp, this._attrName, this._attrValue, this._attrNs, this._attrInsensitive);
        this._attrInsensitive = false;
        this.Insert(selector);
      }
      else
        this._valid = false;
    }
  }

  private void OnPseudoClass(CssSelectorToken token)
  {
    this._state = CssSelectorConstructor.State.Data;
    this._ready = true;
    if (token.Type == CssTokenType.Colon)
    {
      this._state = CssSelectorConstructor.State.PseudoElement;
    }
    else
    {
      if (token.Type == CssTokenType.Function)
      {
        Func<CssSelectorConstructor, CssSelectorConstructor.FunctionState> func;
        if (CssSelectorConstructor.pseudoClassFunctions.TryGetValue(token.Data, out func))
        {
          this._state = CssSelectorConstructor.State.Function;
          this._function = func(this);
          this._ready = false;
          return;
        }
      }
      else if (token.Type == CssTokenType.Ident)
      {
        ISelector selector = this._pseudoClassSelector.Create(token.Data);
        if (selector != null)
        {
          this.Insert(selector);
          return;
        }
      }
      this._valid = false;
    }
  }

  private void OnPseudoElement(CssSelectorToken token)
  {
    this._state = CssSelectorConstructor.State.Data;
    this._ready = true;
    if (token.Type == CssTokenType.Ident)
    {
      ISelector selector = this._pseudoElementSelector.Create(token.Data);
      if (selector != null)
      {
        this._valid = this._valid && !this._nested;
        this.Insert(selector);
        return;
      }
    }
    this._valid = false;
  }

  private void InsertOr()
  {
    if (this._temp == null)
      return;
    if (this._group == null)
      this._group = new ListSelector();
    if (this._complex != null)
    {
      this._complex.ConcludeSelector(this._temp);
      this._group.Add((ISelector) this._complex);
      this._complex = (ComplexSelector) null;
    }
    else
      this._group.Add(this._temp);
    this._temp = (ISelector) null;
  }

  private void Insert(ISelector selector)
  {
    if (this._temp != null)
    {
      if (this._combinators.Count == 0)
      {
        if (!(this._temp is CompoundSelector compoundSelector1))
        {
          CompoundSelector compoundSelector = new CompoundSelector();
          compoundSelector.Add(this._temp);
          compoundSelector1 = compoundSelector;
        }
        compoundSelector1.Add(selector);
        this._temp = (ISelector) compoundSelector1;
      }
      else
      {
        if (this._complex == null)
          this._complex = new ComplexSelector();
        this._complex.AppendSelector(this._temp, this.GetCombinator());
        this._temp = selector;
      }
    }
    else
    {
      this._combinators.Clear();
      this._temp = selector;
    }
  }

  private CssCombinator GetCombinator()
  {
    while (this._combinators.Count > 1 && this._combinators.Peek() == CssCombinator.Descendent)
      this._combinators.Pop();
    if (this._combinators.Count <= 1)
      return this._combinators.Pop();
    CssCombinator combinator = this._combinators.Pop();
    while (this._combinators.Count > 0)
      this._valid = this._combinators.Pop() == CssCombinator.Descendent && this._valid;
    return combinator;
  }

  private void Insert(CssCombinator cssCombinator) => this._combinators.Push(cssCombinator);

  private void OnDelim(CssSelectorToken token)
  {
    switch (token.Data[0])
    {
      case '*':
        this.Insert(AllSelector.Instance);
        this._ready = true;
        break;
      case '+':
        this.Insert(CssCombinator.AdjacentSibling);
        this._ready = false;
        break;
      case ',':
        this.InsertOr();
        this._ready = false;
        break;
      case '>':
        this.Insert(CssCombinator.Child);
        this._ready = false;
        break;
      case '|':
        if (this._combinators.Count > 0 && this._combinators.Peek() == CssCombinator.Descendent)
          this.Insert((ISelector) new TypeSelector(string.Empty));
        this.Insert(CssCombinator.Namespace);
        this._ready = false;
        break;
      case '~':
        this.Insert(CssCombinator.Sibling);
        this._ready = false;
        break;
      default:
        this._valid = false;
        break;
    }
  }

  private void OnFunctionState(CssSelectorToken token)
  {
    if (!this._function.Finished(token))
      return;
    ISelector selector = this._function.Produce();
    if (this._nested && this._function is CssSelectorConstructor.NotFunctionState)
      selector = (ISelector) null;
    this._function = (CssSelectorConstructor.FunctionState) null;
    this._state = CssSelectorConstructor.State.Data;
    this._ready = true;
    if (selector != null)
      this.Insert(selector);
    else
      this._valid = false;
  }

  private CssSelectorConstructor CreateChild()
  {
    return new CssSelectorConstructor(this._tokenizer, this._attributeSelector, this._pseudoClassSelector, this._pseudoElementSelector, true);
  }

  private enum State : byte
  {
    Data,
    Function,
    Attribute,
    AttributeOperator,
    AttributeValue,
    AttributeEnd,
    PseudoClass,
    PseudoElement,
  }

  private abstract class FunctionState
  {
    public bool Finished(CssSelectorToken token) => this.OnToken(token);

    public abstract ISelector Produce();

    protected abstract bool OnToken(CssSelectorToken token);
  }

  private sealed class NotFunctionState : CssSelectorConstructor.FunctionState
  {
    private readonly CssSelectorConstructor _selector;

    public NotFunctionState(CssSelectorConstructor parent)
    {
      this._selector = parent.CreateChild();
      this._selector._nested = true;
    }

    protected override bool OnToken(CssSelectorToken token)
    {
      if (token.Type == CssTokenType.RoundBracketClose && this._selector._state == CssSelectorConstructor.State.Data)
        return true;
      this._selector.Apply(token);
      return false;
    }

    public override ISelector Produce()
    {
      int num = this._selector.IsValid ? 1 : 0;
      ISelector sel = this._selector.GetResult();
      if (num == 0)
        return (ISelector) null;
      return (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => !sel.Match(el)), PseudoClassNames.Not.CssFunction(sel.Text));
    }
  }

  private sealed class HasFunctionState : CssSelectorConstructor.FunctionState
  {
    private readonly CssSelectorConstructor _nested;
    private bool _firstToken = true;
    private bool _matchSiblings;

    public HasFunctionState(CssSelectorConstructor parent) => this._nested = parent.CreateChild();

    protected override bool OnToken(CssSelectorToken token)
    {
      if (token.Type == CssTokenType.RoundBracketClose && this._nested._state == CssSelectorConstructor.State.Data)
        return true;
      if (this._firstToken && token.Type == CssTokenType.Delim)
      {
        this._nested.Insert(ScopePseudoClassSelector.Instance);
        this._nested.Apply(CssSelectorToken.Whitespace);
        this._matchSiblings = true;
      }
      this._firstToken = false;
      this._nested.Apply(token);
      return false;
    }

    public override ISelector Produce()
    {
      bool isValid = this._nested.IsValid;
      ISelector sel = this._nested.GetResult();
      string text = sel.Text;
      bool matchSiblings = this._matchSiblings || text.Contains(":" + PseudoClassNames.Scope);
      if (!isValid)
        return (ISelector) null;
      return (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => sel.MatchAny((!matchSiblings ? (IEnumerable<IElement>) el.Children : (IEnumerable<IElement>) el.ParentElement?.Children) ?? Enumerable.Empty<IElement>(), el) != null), PseudoClassNames.Has.CssFunction(text));
    }
  }

  private sealed class MatchesFunctionState : CssSelectorConstructor.FunctionState
  {
    private readonly CssSelectorConstructor _selector;

    public MatchesFunctionState(CssSelectorConstructor parent)
    {
      this._selector = parent.CreateChild();
    }

    protected override bool OnToken(CssSelectorToken token)
    {
      if (token.Type == CssTokenType.RoundBracketClose && this._selector._state == CssSelectorConstructor.State.Data)
        return true;
      this._selector.Apply(token);
      return false;
    }

    public override ISelector Produce()
    {
      int num = this._selector.IsValid ? 1 : 0;
      ISelector sel = this._selector.GetResult();
      if (num == 0)
        return (ISelector) null;
      return (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => sel.Match(el)), PseudoClassNames.Matches.CssFunction(sel.Text));
    }
  }

  private sealed class DirFunctionState : CssSelectorConstructor.FunctionState
  {
    private bool _valid;
    private string _value;

    public DirFunctionState()
    {
      this._valid = true;
      this._value = (string) null;
    }

    protected override bool OnToken(CssSelectorToken token)
    {
      if (token.Type == CssTokenType.Ident)
      {
        this._value = token.Data;
      }
      else
      {
        if (token.Type == CssTokenType.RoundBracketClose)
          return true;
        if (token.Type != CssTokenType.Whitespace)
          this._valid = false;
      }
      return false;
    }

    public override ISelector Produce()
    {
      return this._valid && this._value != null ? (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el is IHtmlElement && this._value.Isi(((IHtmlElement) el).Direction)), PseudoClassNames.Dir.CssFunction(this._value)) : (ISelector) null;
    }
  }

  private sealed class LangFunctionState : CssSelectorConstructor.FunctionState
  {
    private bool valid;
    private string value;

    public LangFunctionState()
    {
      this.valid = true;
      this.value = (string) null;
    }

    protected override bool OnToken(CssSelectorToken token)
    {
      if (token.Type == CssTokenType.Ident)
      {
        this.value = token.Data;
      }
      else
      {
        if (token.Type == CssTokenType.RoundBracketClose)
          return true;
        if (token.Type != CssTokenType.Whitespace)
          this.valid = false;
      }
      return false;
    }

    public override ISelector Produce()
    {
      return this.valid && this.value != null ? (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el is IHtmlElement && ((IHtmlElement) el).Language.StartsWith(this.value, StringComparison.OrdinalIgnoreCase)), PseudoClassNames.Lang.CssFunction(this.value)) : (ISelector) null;
    }
  }

  private sealed class ContainsFunctionState : CssSelectorConstructor.FunctionState
  {
    private bool _valid;
    private string _value;

    public ContainsFunctionState()
    {
      this._valid = true;
      this._value = (string) null;
    }

    protected override bool OnToken(CssSelectorToken token)
    {
      if (token.Type == CssTokenType.Ident || token.Type == CssTokenType.String)
      {
        this._value = token.Data;
      }
      else
      {
        if (token.Type == CssTokenType.RoundBracketClose)
          return true;
        if (token.Type != CssTokenType.Whitespace)
          this._valid = false;
      }
      return false;
    }

    public override ISelector Produce()
    {
      return this._valid && this._value != null ? (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.TextContent.Contains(this._value)), PseudoClassNames.Contains.CssFunction(this._value)) : (ISelector) null;
    }
  }

  private sealed class HostContextFunctionState : CssSelectorConstructor.FunctionState
  {
    private readonly CssSelectorConstructor _selector;

    public HostContextFunctionState(CssSelectorConstructor parent)
    {
      this._selector = parent.CreateChild();
    }

    protected override bool OnToken(CssSelectorToken token)
    {
      if (token.Type == CssTokenType.RoundBracketClose && this._selector._state == CssSelectorConstructor.State.Data)
        return true;
      this._selector.Apply(token);
      return false;
    }

    public override ISelector Produce()
    {
      int num = this._selector.IsValid ? 1 : 0;
      ISelector sel = this._selector.GetResult();
      if (num == 0)
        return (ISelector) null;
      return (ISelector) new PseudoClassSelector((Predicate<IElement>) (el =>
      {
        for (IElement element = el.Parent is IShadowRoot parent2 ? parent2.Host : (IElement) null; element != null; element = element.ParentElement)
        {
          if (sel.Match(element))
            return true;
        }
        return false;
      }), PseudoClassNames.HostContext.CssFunction(sel.Text));
    }
  }

  private sealed class ChildFunctionState : CssSelectorConstructor.FunctionState
  {
    private readonly CssSelectorConstructor _parent;
    private bool _valid;
    private int _step;
    private int _offset;
    private int _sign;
    private CssSelectorConstructor.ChildFunctionState.ParseState _state;
    private CssSelectorConstructor _nested;
    private bool _allowOf;
    private Func<int, int, ISelector, ISelector> _creator;

    public ChildFunctionState(
      Func<int, int, ISelector, ISelector> creator,
      CssSelectorConstructor parent,
      bool withOptionalSelector = true)
    {
      this._creator = creator;
      this._parent = parent;
      this._allowOf = withOptionalSelector;
      this._valid = true;
      this._sign = 1;
      this._state = CssSelectorConstructor.ChildFunctionState.ParseState.Initial;
    }

    public override ISelector Produce()
    {
      return (!this._valid ? 1 : (this._nested == null ? 0 : (!this._nested.IsValid ? 1 : 0))) == 0 ? this._creator(this._step, this._offset, this._nested?.GetResult() ?? AllSelector.Instance) : (ISelector) null;
    }

    protected override bool OnToken(CssSelectorToken token)
    {
      switch (this._state)
      {
        case CssSelectorConstructor.ChildFunctionState.ParseState.Initial:
          return this.OnInitial(token);
        case CssSelectorConstructor.ChildFunctionState.ParseState.AfterInitialSign:
          return this.OnAfterInitialSign(token);
        case CssSelectorConstructor.ChildFunctionState.ParseState.Offset:
          return this.OnOffset(token);
        case CssSelectorConstructor.ChildFunctionState.ParseState.BeforeOf:
          return this.OnBeforeOf(token);
        default:
          return this.OnAfter(token);
      }
    }

    private bool OnAfterInitialSign(CssSelectorToken token)
    {
      if (token.Type == CssTokenType.Number)
        return this.OnOffset(token);
      if (token.Type == CssTokenType.Dimension)
      {
        string s = token.Data.Remove(token.Data.Length - 1);
        this._valid = this._valid && token.Data.EndsWith("n", StringComparison.OrdinalIgnoreCase) && int.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out this._step);
        this._step *= this._sign;
        this._sign = 1;
        this._state = CssSelectorConstructor.ChildFunctionState.ParseState.Offset;
        return false;
      }
      if (token.Type == CssTokenType.Ident && token.Data.Isi("n"))
      {
        this._step = this._sign;
        this._sign = 1;
        this._state = CssSelectorConstructor.ChildFunctionState.ParseState.Offset;
        return false;
      }
      if (this._state == CssSelectorConstructor.ChildFunctionState.ParseState.Initial && token.Type == CssTokenType.Ident && token.Data.Isi("-n"))
      {
        this._step = -1;
        this._state = CssSelectorConstructor.ChildFunctionState.ParseState.Offset;
        return false;
      }
      this._valid = false;
      return token.Type == CssTokenType.RoundBracketClose;
    }

    private bool OnAfter(CssSelectorToken token)
    {
      if (token.Type == CssTokenType.RoundBracketClose && this._nested._state == CssSelectorConstructor.State.Data)
        return true;
      this._nested.Apply(token);
      return false;
    }

    private bool OnBeforeOf(CssSelectorToken token)
    {
      if (token.Type == CssTokenType.Whitespace)
        return false;
      if (token.Data.Isi(Keywords.Of))
      {
        this._valid = this._allowOf;
        this._state = CssSelectorConstructor.ChildFunctionState.ParseState.AfterOf;
        this._nested = this._parent.CreateChild();
        return false;
      }
      if (token.Type == CssTokenType.RoundBracketClose)
        return true;
      this._valid = false;
      return false;
    }

    private bool OnOffset(CssSelectorToken token)
    {
      if (token.Type == CssTokenType.Whitespace)
        return false;
      if (token.Type != CssTokenType.Number)
        return this.OnBeforeOf(token);
      this._valid = this._valid && int.TryParse(token.Data, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out this._offset);
      this._offset *= this._sign;
      this._state = CssSelectorConstructor.ChildFunctionState.ParseState.BeforeOf;
      return false;
    }

    private bool OnInitial(CssSelectorToken token)
    {
      if (token.Type == CssTokenType.Whitespace)
        return false;
      if (token.Data.Isi(Keywords.Odd))
      {
        this._state = CssSelectorConstructor.ChildFunctionState.ParseState.BeforeOf;
        this._step = 2;
        this._offset = 1;
        return false;
      }
      if (token.Data.Isi(Keywords.Even))
      {
        this._state = CssSelectorConstructor.ChildFunctionState.ParseState.BeforeOf;
        this._step = 2;
        this._offset = 0;
        return false;
      }
      if (token.Type != CssTokenType.Delim || !token.Data.IsOneOf("+", "-"))
        return this.OnAfterInitialSign(token);
      this._sign = token.Data == "-" ? -1 : 1;
      this._state = CssSelectorConstructor.ChildFunctionState.ParseState.AfterInitialSign;
      return false;
    }

    private enum ParseState : byte
    {
      Initial,
      AfterInitialSign,
      Offset,
      BeforeOf,
      AfterOf,
    }
  }
}
