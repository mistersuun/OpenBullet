// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.DocumentExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Browser;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Dom;

public static class DocumentExtensions
{
  internal static void ForEachRange(
    this Document document,
    Predicate<Range> condition,
    Action<Range> action)
  {
    foreach (Range attachedReference in document.GetAttachedReferences<Range>())
    {
      if (condition(attachedReference))
        action(attachedReference);
    }
  }

  public static TElement CreateElement<TElement>(this IDocument document) where TElement : IElement
  {
    if (document == null)
      throw new ArgumentNullException(nameof (document));
    Type type = ((IEnumerable<Type>) typeof (BrowsingContext).GetAssembly().GetTypes()).Where<Type>((Func<Type, bool>) (m => m.Implements<TElement>())).FirstOrDefault<Type>((Func<Type, bool>) (m => !m.IsAbstractClass()));
    if (type != (Type) null)
    {
      foreach (ConstructorInfo constructorInfo in (IEnumerable<ConstructorInfo>) ((IEnumerable<ConstructorInfo>) type.GetConstructors()).OrderBy<ConstructorInfo, int>((Func<ConstructorInfo, int>) (m => m.GetParameters().Length)))
      {
        ParameterInfo[] parameters1 = constructorInfo.GetParameters();
        object[] parameters2 = new object[parameters1.Length];
        for (int index = 0; index < parameters1.Length; ++index)
        {
          bool flag = parameters1[index].ParameterType == typeof (Document);
          parameters2[index] = flag ? (object) document : parameters1[index].DefaultValue;
        }
        object obj = constructorInfo.Invoke(parameters2);
        if (obj != null)
        {
          TElement externalNode = (TElement) obj;
          if (externalNode is Element element)
            element.SetupElement();
          document.Adopt((INode) externalNode);
          return externalNode;
        }
      }
    }
    throw new ArgumentException("No element could be created for the provided interface.");
  }

  public static void AdoptNode(this IDocument document, INode node)
  {
    if (!(node is Node node1))
      throw new DomException(DomError.NotSupported);
    node1.Parent?.RemoveChild(node1, false);
    node1.Owner = document as Document;
  }

  internal static void QueueTask(this Document document, Action action)
  {
    document.Loop.Enqueue(action);
  }

  internal static Task QueueTaskAsync(this Document document, Action<CancellationToken> action)
  {
    return (Task) document.Loop.EnqueueAsync<bool>((Func<CancellationToken, bool>) (_ =>
    {
      action(_);
      return true;
    }));
  }

  internal static Task<T> QueueTaskAsync<T>(this Document document, Func<CancellationToken, T> func)
  {
    return document.Loop.EnqueueAsync<T>(func);
  }

  internal static void QueueMutation(this Document document, MutationRecord record)
  {
    MutationObserver[] array = document.Mutations.Observers.ToArray<MutationObserver>();
    if (array.Length == 0)
      return;
    IEnumerable<INode> inclusiveAncestors = record.Target.GetInclusiveAncestors();
    for (int index = 0; index < array.Length; ++index)
    {
      MutationObserver mutationObserver = array[index];
      bool? nullable = new bool?();
      foreach (INode node in inclusiveAncestors)
      {
        MutationObserver.MutationOptions mutationOptions = mutationObserver.ResolveOptions(node);
        if (!mutationOptions.IsInvalid && (node == record.Target || mutationOptions.IsObservingSubtree) && (!record.IsAttribute || mutationOptions.IsObservingAttributes) && (!record.IsAttribute || mutationOptions.AttributeFilters == null || mutationOptions.AttributeFilters.Contains<string>(record.AttributeName) && record.AttributeNamespace == null) && (!record.IsCharacterData || mutationOptions.IsObservingCharacterData) && (!record.IsChildList || mutationOptions.IsObservingChildNodes) && (!nullable.HasValue || nullable.Value))
          nullable = new bool?(record.IsAttribute && !mutationOptions.IsExaminingOldAttributeValue || record.IsCharacterData && !mutationOptions.IsExaminingOldCharacterData);
      }
      if (nullable.HasValue)
        mutationObserver.Enqueue(record.Copy(nullable.Value));
    }
    document.PerformMicrotaskCheckpoint();
  }

  internal static void AddTransientObserver(this Document document, INode node)
  {
    IEnumerable<INode> ancestors = node.GetAncestors();
    IEnumerable<MutationObserver> observers = document.Mutations.Observers;
    foreach (INode ancestor in ancestors)
    {
      foreach (MutationObserver mutationObserver in observers)
        mutationObserver.AddTransient(ancestor, node);
    }
  }

  internal static void ApplyManifest(this Document document)
  {
    if (!document.IsInBrowsingContext || !(document.DocumentElement is IHtmlHtmlElement documentElement))
      return;
    string manifest = documentElement.Manifest;
    Predicate<string> predicate = (Predicate<string>) (str => false);
    if (string.IsNullOrEmpty(manifest))
      return;
    int num = predicate(manifest) ? 1 : 0;
  }

  internal static void PerformMicrotaskCheckpoint(this Document document)
  {
    document.Mutations.ScheduleCallback();
  }

  internal static void ProvideStableState(this Document document)
  {
  }

  public static IEnumerable<Task> GetScriptDownloads(this IDocument document)
  {
    return document.Context.GetDownloads<HtmlScriptElement>();
  }

  public static IEnumerable<Task> GetStyleSheetDownloads(this IDocument document)
  {
    return document.Context.GetDownloads<HtmlLinkElement>();
  }

  public static async Task WaitForReadyAsync(this IDocument document)
  {
    ConfiguredTaskAwaitable configuredTaskAwaitable = Task.WhenAll(document.GetScriptDownloads().ToArray<Task>()).ConfigureAwait(false);
    await configuredTaskAwaitable;
    configuredTaskAwaitable = Task.WhenAll(document.GetStyleSheetDownloads().ToArray<Task>()).ConfigureAwait(false);
    await configuredTaskAwaitable;
  }

  public static IEnumerable<IDownload> GetDownloads(this IDocument document)
  {
    if (document == null)
      throw new ArgumentNullException(nameof (document));
    return document.All.OfType<ILoadableElement>().Select<ILoadableElement, IDownload>((Func<ILoadableElement, IDownload>) (m => m.CurrentDownload)).Where<IDownload>((Func<IDownload, bool>) (m => m != null));
  }
}
