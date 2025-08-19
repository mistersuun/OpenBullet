// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.DefaultAttributeSelectorFactory
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Css.Dom;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Css;

public class DefaultAttributeSelectorFactory : IAttributeSelectorFactory
{
  private readonly Dictionary<string, DefaultAttributeSelectorFactory.Creator> _creators = new Dictionary<string, DefaultAttributeSelectorFactory.Creator>()
  {
    {
      CombinatorSymbols.Exactly,
      (DefaultAttributeSelectorFactory.Creator) ((name, value, prefix, mode) => (ISelector) new AttrMatchSelector(name, value, prefix, mode))
    },
    {
      CombinatorSymbols.InList,
      (DefaultAttributeSelectorFactory.Creator) ((name, value, prefix, mode) => (ISelector) new AttrInListSelector(name, value, prefix, mode))
    },
    {
      CombinatorSymbols.InToken,
      (DefaultAttributeSelectorFactory.Creator) ((name, value, prefix, mode) => (ISelector) new AttrInTokenSelector(name, value, prefix, mode))
    },
    {
      CombinatorSymbols.Begins,
      (DefaultAttributeSelectorFactory.Creator) ((name, value, prefix, mode) => (ISelector) new AttrStartsWithSelector(name, value, prefix, mode))
    },
    {
      CombinatorSymbols.Ends,
      (DefaultAttributeSelectorFactory.Creator) ((name, value, prefix, mode) => (ISelector) new AttrEndsWithSelector(name, value, prefix, mode))
    },
    {
      CombinatorSymbols.InText,
      (DefaultAttributeSelectorFactory.Creator) ((name, value, prefix, mode) => (ISelector) new AttrContainsSelector(name, value, prefix, mode))
    },
    {
      CombinatorSymbols.Unlike,
      (DefaultAttributeSelectorFactory.Creator) ((name, value, prefix, mode) => (ISelector) new AttrNotMatchSelector(name, value, prefix, mode))
    }
  };

  public void Register(string combinator, DefaultAttributeSelectorFactory.Creator creator)
  {
    this._creators.Add(combinator, creator);
  }

  public DefaultAttributeSelectorFactory.Creator Unregister(string combinator)
  {
    DefaultAttributeSelectorFactory.Creator creator;
    if (this._creators.TryGetValue(combinator, out creator))
      this._creators.Remove(combinator);
    return creator;
  }

  protected virtual ISelector CreateDefault(
    string name,
    string value,
    string prefix,
    bool insensitive)
  {
    return (ISelector) new AttrAvailableSelector(name, prefix);
  }

  public ISelector Create(
    string combinator,
    string name,
    string value,
    string prefix,
    bool insensitive)
  {
    DefaultAttributeSelectorFactory.Creator creator;
    return this._creators.TryGetValue(combinator, out creator) ? creator(name, value, prefix, insensitive) : this.CreateDefault(name, value, prefix, insensitive);
  }

  public delegate ISelector Creator(string name, string value, string prefix, bool insensitive);
}
