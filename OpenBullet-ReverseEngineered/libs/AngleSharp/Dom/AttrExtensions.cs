// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.AttrExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Dom;

public static class AttrExtensions
{
  public static bool SameAs(this INamedNodeMap sourceAttributes, INamedNodeMap targetAttributes)
  {
    if (sourceAttributes.Length != targetAttributes.Length)
      return false;
    foreach (IAttr sourceAttribute in (IEnumerable<IAttr>) sourceAttributes)
    {
      bool flag = false;
      foreach (IAttr targetAttribute in (IEnumerable<IAttr>) targetAttributes)
      {
        flag = sourceAttribute.GetHashCode() == targetAttribute.GetHashCode();
        if (flag)
          break;
      }
      if (!flag)
        return false;
    }
    return true;
  }

  public static INamedNodeMap Clear(this INamedNodeMap attributes)
  {
    if (attributes == null)
      throw new ArgumentNullException(nameof (attributes));
    while (attributes.Length > 0)
    {
      string name = attributes[attributes.Length - 1].Name;
      attributes.RemoveNamedItem(name);
    }
    return attributes;
  }
}
