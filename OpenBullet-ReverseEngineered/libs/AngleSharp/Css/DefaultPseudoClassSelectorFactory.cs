// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.DefaultPseudoClassSelectorFactory
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using AngleSharp.Text;
using System;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Css;

public class DefaultPseudoClassSelectorFactory : IPseudoClassSelectorFactory
{
  private readonly Dictionary<string, ISelector> _selectors = new Dictionary<string, ISelector>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
  {
    {
      PseudoClassNames.Root,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.Owner.DocumentElement == el), PseudoClassNames.Root)
    },
    {
      PseudoClassNames.Scope,
      ScopePseudoClassSelector.Instance
    },
    {
      PseudoClassNames.OnlyType,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsOnlyOfType()), PseudoClassNames.OnlyType)
    },
    {
      PseudoClassNames.FirstOfType,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsFirstOfType()), PseudoClassNames.FirstOfType)
    },
    {
      PseudoClassNames.LastOfType,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsLastOfType()), PseudoClassNames.LastOfType)
    },
    {
      PseudoClassNames.OnlyChild,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsOnlyChild()), PseudoClassNames.OnlyChild)
    },
    {
      PseudoClassNames.FirstChild,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsFirstChild()), PseudoClassNames.FirstChild)
    },
    {
      PseudoClassNames.LastChild,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsLastChild()), PseudoClassNames.LastChild)
    },
    {
      PseudoClassNames.Empty,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.ChildElementCount == 0 && el.TextContent.Is(string.Empty)), PseudoClassNames.Empty)
    },
    {
      PseudoClassNames.AnyLink,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsLink() || el.IsVisited()), PseudoClassNames.AnyLink)
    },
    {
      PseudoClassNames.Link,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsLink()), PseudoClassNames.Link)
    },
    {
      PseudoClassNames.Visited,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsVisited()), PseudoClassNames.Visited)
    },
    {
      PseudoClassNames.Active,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsActive()), PseudoClassNames.Active)
    },
    {
      PseudoClassNames.Hover,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsHovered()), PseudoClassNames.Hover)
    },
    {
      PseudoClassNames.Focus,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsFocused), PseudoClassNames.Focus)
    },
    {
      PseudoClassNames.Target,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsTarget()), PseudoClassNames.Target)
    },
    {
      PseudoClassNames.Enabled,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsEnabled()), PseudoClassNames.Enabled)
    },
    {
      PseudoClassNames.Disabled,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsDisabled()), PseudoClassNames.Disabled)
    },
    {
      PseudoClassNames.Default,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsDefault()), PseudoClassNames.Default)
    },
    {
      PseudoClassNames.Checked,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsChecked()), PseudoClassNames.Checked)
    },
    {
      PseudoClassNames.Indeterminate,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsIndeterminate()), PseudoClassNames.Indeterminate)
    },
    {
      PseudoClassNames.PlaceholderShown,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsPlaceholderShown()), PseudoClassNames.PlaceholderShown)
    },
    {
      PseudoClassNames.Unchecked,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsUnchecked()), PseudoClassNames.Unchecked)
    },
    {
      PseudoClassNames.Valid,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsValid()), PseudoClassNames.Valid)
    },
    {
      PseudoClassNames.Invalid,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsInvalid()), PseudoClassNames.Invalid)
    },
    {
      PseudoClassNames.Required,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsRequired()), PseudoClassNames.Required)
    },
    {
      PseudoClassNames.ReadOnly,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsReadOnly()), PseudoClassNames.ReadOnly)
    },
    {
      PseudoClassNames.ReadWrite,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsEditable()), PseudoClassNames.ReadWrite)
    },
    {
      PseudoClassNames.InRange,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsInRange()), PseudoClassNames.InRange)
    },
    {
      PseudoClassNames.OutOfRange,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsOutOfRange()), PseudoClassNames.OutOfRange)
    },
    {
      PseudoClassNames.Optional,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsOptional()), PseudoClassNames.Optional)
    },
    {
      PseudoClassNames.Shadow,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsShadow()), PseudoClassNames.Shadow)
    },
    {
      PseudoElementNames.Before,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsPseudo(PseudoElementNames.Before)), PseudoElementNames.Before)
    },
    {
      PseudoElementNames.After,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.IsPseudo(PseudoElementNames.After)), PseudoElementNames.After)
    },
    {
      PseudoElementNames.FirstLine,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.HasChildNodes && el.ChildNodes[0].NodeType == NodeType.Text), PseudoElementNames.FirstLine)
    },
    {
      PseudoElementNames.FirstLetter,
      (ISelector) new PseudoClassSelector((Predicate<IElement>) (el => el.HasChildNodes && el.ChildNodes[0].NodeType == NodeType.Text && el.ChildNodes[0].TextContent.Length > 0), PseudoElementNames.FirstLetter)
    }
  };

  public void Register(string name, ISelector selector) => this._selectors.Add(name, selector);

  public ISelector Unregister(string name)
  {
    ISelector selector;
    if (this._selectors.TryGetValue(name, out selector))
      this._selectors.Remove(name);
    return selector;
  }

  protected virtual ISelector CreateDefault(string name) => (ISelector) null;

  public ISelector Create(string name)
  {
    ISelector selector;
    return this._selectors.TryGetValue(name, out selector) ? selector : this.CreateDefault(name);
  }
}
