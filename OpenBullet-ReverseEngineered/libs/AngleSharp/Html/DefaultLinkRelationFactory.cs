// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.DefaultLinkRelationFactory
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom;
using AngleSharp.Html.LinkRels;
using System;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Html;

public class DefaultLinkRelationFactory : ILinkRelationFactory
{
  private readonly Dictionary<string, DefaultLinkRelationFactory.Creator> _creators = new Dictionary<string, DefaultLinkRelationFactory.Creator>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
  {
    {
      LinkRelNames.StyleSheet,
      (DefaultLinkRelationFactory.Creator) (link => (BaseLinkRelation) new StyleSheetLinkRelation(link))
    },
    {
      LinkRelNames.Import,
      (DefaultLinkRelationFactory.Creator) (link => (BaseLinkRelation) new ImportLinkRelation(link))
    }
  };

  public void Register(string rel, DefaultLinkRelationFactory.Creator creator)
  {
    this._creators.Add(rel, creator);
  }

  public DefaultLinkRelationFactory.Creator Unregister(string rel)
  {
    DefaultLinkRelationFactory.Creator creator;
    if (this._creators.TryGetValue(rel, out creator))
      this._creators.Remove(rel);
    return creator;
  }

  protected virtual BaseLinkRelation CreateDefault(IHtmlLinkElement link, string rel)
  {
    return (BaseLinkRelation) null;
  }

  public BaseLinkRelation Create(IHtmlLinkElement link, string rel)
  {
    DefaultLinkRelationFactory.Creator creator;
    return rel != null && this._creators.TryGetValue(rel, out creator) ? creator(link) : this.CreateDefault(link, rel);
  }

  public delegate BaseLinkRelation Creator(IHtmlLinkElement link);
}
