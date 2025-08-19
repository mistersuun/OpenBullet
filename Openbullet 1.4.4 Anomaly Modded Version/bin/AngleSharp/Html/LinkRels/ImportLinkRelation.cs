// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.LinkRels.ImportLinkRelation
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using AngleSharp.Io.Processors;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Html.LinkRels;

internal class ImportLinkRelation(IHtmlLinkElement link) : BaseLinkRelation(link, (IRequestProcessor) new DocumentRequestProcessor(link?.Owner.Context))
{
  private static readonly ConditionalWeakTable<IDocument, ImportLinkRelation.ImportList> ImportLists = new ConditionalWeakTable<IDocument, ImportLinkRelation.ImportList>();
  private bool _async;

  public IDocument Import
  {
    get
    {
      return !(this.Processor is DocumentRequestProcessor processor) ? (IDocument) null : processor.ChildDocument;
    }
  }

  public bool IsAsync => this._async;

  public override Task LoadAsync()
  {
    IHtmlLinkElement link = this.Link;
    IDocument owner = link.Owner;
    ImportLinkRelation.ImportList importList = ImportLinkRelation.ImportLists.GetOrCreateValue(owner);
    Url url = this.Url;
    IRequestProcessor processor = this.Processor;
    ImportLinkRelation.ImportEntry importEntry1 = new ImportLinkRelation.ImportEntry()
    {
      Relation = this,
      IsCycle = url != null && ImportLinkRelation.CheckCycle(owner, url)
    };
    ImportLinkRelation.ImportEntry importEntry2 = importEntry1;
    importList.Add(importEntry2);
    if (url == null || importEntry1.IsCycle)
      return Task.CompletedTask;
    ResourceRequest requestFor = link.CreateRequestFor(url);
    this._async = link.HasAttribute(AttributeNames.Async);
    return processor?.ProcessAsync(requestFor);
  }

  private static bool CheckCycle(IDocument document, Url location)
  {
    ImportLinkRelation.ImportList importList;
    for (IDocument importAncestor = document.ImportAncestor; importAncestor != null && ImportLinkRelation.ImportLists.TryGetValue(importAncestor, out importList); importAncestor = importAncestor.ImportAncestor)
    {
      if (importList.Contains(location))
        return true;
    }
    return false;
  }

  private sealed class ImportList
  {
    private readonly List<ImportLinkRelation.ImportEntry> _list;

    public ImportList() => this._list = new List<ImportLinkRelation.ImportEntry>();

    public bool Contains(Url location)
    {
      for (int index = 0; index < this._list.Count; ++index)
      {
        Url url = this._list[index].Relation.Url;
        if (url != null && url.Equals(location))
          return true;
      }
      return false;
    }

    public void Add(ImportLinkRelation.ImportEntry item) => this._list.Add(item);

    public void Remove(ImportLinkRelation.ImportEntry item) => this._list.Remove(item);
  }

  private struct ImportEntry
  {
    public ImportLinkRelation Relation;
    public bool IsCycle;
  }
}
