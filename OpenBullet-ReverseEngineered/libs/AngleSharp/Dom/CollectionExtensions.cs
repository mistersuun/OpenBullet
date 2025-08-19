// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.CollectionExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Dom;

public static class CollectionExtensions
{
  public static IEnumerable<T> GetNodes<T>(this INode parent, bool deep = true, Func<T, bool> predicate = null) where T : class, INode
  {
    predicate = predicate ?? (Func<T, bool>) (m => true);
    return !deep ? parent.GetDescendendElements<T>(predicate) : parent.GetAllNodes<T>(predicate);
  }

  public static IElement GetElementById(this INodeList children, string id)
  {
    for (int index = 0; index < children.Length; ++index)
    {
      if (children[index] is IElement child)
      {
        if (child.Id.Is(id))
          return child;
        IElement elementById = child.ChildNodes.GetElementById(id);
        if (elementById != null)
          return elementById;
      }
    }
    return (IElement) null;
  }

  public static void GetElementsByName(this INodeList children, string name, List<IElement> result)
  {
    for (int index = 0; index < children.Length; ++index)
    {
      if (children[index] is IElement child)
      {
        if (child.GetAttribute((string) null, AttributeNames.Name).Is(name))
          result.Add(child);
        child.ChildNodes.GetElementsByName(name, result);
      }
    }
  }

  public static bool Accepts(this FilterSettings filter, INode node)
  {
    switch (node.NodeType)
    {
      case NodeType.Element:
        return (filter & FilterSettings.Element) == FilterSettings.Element;
      case NodeType.Attribute:
        return (filter & FilterSettings.Attribute) == FilterSettings.Attribute;
      case NodeType.Text:
        return (filter & FilterSettings.Text) == FilterSettings.Text;
      case NodeType.CharacterData:
        return (filter & FilterSettings.CharacterData) == FilterSettings.CharacterData;
      case NodeType.EntityReference:
        return (filter & FilterSettings.EntityReference) == FilterSettings.EntityReference;
      case NodeType.Entity:
        return (filter & FilterSettings.Entity) == FilterSettings.Entity;
      case NodeType.ProcessingInstruction:
        return (filter & FilterSettings.ProcessingInstruction) == FilterSettings.ProcessingInstruction;
      case NodeType.Comment:
        return (filter & FilterSettings.Comment) == FilterSettings.Comment;
      case NodeType.Document:
        return (filter & FilterSettings.Document) == FilterSettings.Document;
      case NodeType.DocumentType:
        return (filter & FilterSettings.DocumentType) == FilterSettings.DocumentType;
      case NodeType.DocumentFragment:
        return (filter & FilterSettings.DocumentFragment) == FilterSettings.DocumentFragment;
      case NodeType.Notation:
        return (filter & FilterSettings.Notation) == FilterSettings.Notation;
      default:
        return filter == FilterSettings.All;
    }
  }

  public static T GetElementById<T>(this IEnumerable<T> elements, string id) where T : class, IElement
  {
    foreach (T element in elements)
    {
      if (element.Id.Is(id))
        return element;
    }
    foreach (T element in elements)
    {
      if (element.GetAttribute((string) null, AttributeNames.Name).Is(id))
        return element;
    }
    return default (T);
  }

  private static IEnumerable<T> GetAllNodes<T>(this INode parent, Func<T, bool> predicate) where T : class, INode
  {
    return new NodeEnumerable(parent).OfType<T>().Where<T>(predicate);
  }

  private static IEnumerable<T> GetDescendendElements<T>(this INode parent, Func<T, bool> predicate) where T : class, INode
  {
    for (int i = 0; i < parent.ChildNodes.Length; ++i)
    {
      if (parent.ChildNodes[i] is T childNode && predicate(childNode))
        yield return childNode;
    }
  }
}
