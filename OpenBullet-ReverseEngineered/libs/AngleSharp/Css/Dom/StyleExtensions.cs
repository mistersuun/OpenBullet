// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.StyleExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Css.Dom;

public static class StyleExtensions
{
  public static IEnumerable<string> GetAllStyleSheetSets(this IStyleSheetList sheets)
  {
    List<string> existing = new List<string>();
    foreach (IStyleSheet sheet in (IEnumerable<IStyleSheet>) sheets)
    {
      string title = sheet.Title;
      if (!string.IsNullOrEmpty(title) && !existing.Contains(title))
      {
        existing.Add(title);
        yield return title;
      }
    }
  }

  public static IEnumerable<string> GetEnabledStyleSheetSets(this IStyleSheetList sheets)
  {
    List<string> second = new List<string>();
    foreach (IStyleSheet sheet in (IEnumerable<IStyleSheet>) sheets)
    {
      string title = sheet.Title;
      if (!string.IsNullOrEmpty(title) && !second.Contains(title) && sheet.IsDisabled)
        second.Add(title);
    }
    return sheets.GetAllStyleSheetSets().Except<string>((IEnumerable<string>) second);
  }

  public static void EnableStyleSheetSet(this IStyleSheetList sheets, string name)
  {
    foreach (IStyleSheet sheet in (IEnumerable<IStyleSheet>) sheets)
    {
      string title = sheet.Title;
      if (!string.IsNullOrEmpty(title))
        sheet.IsDisabled = title != name;
    }
  }

  public static IStyleSheetList CreateStyleSheets(this INode parent)
  {
    return (IStyleSheetList) new StyleSheetList(parent.GetStyleSheets());
  }

  public static IStringList CreateStyleSheetSets(this INode parent)
  {
    return (IStringList) new StringList(parent.GetStyleSheets().Select<IStyleSheet, string>((Func<IStyleSheet, string>) (m => m.Title)).Where<string>((Func<string, bool>) (m => m != null)));
  }

  public static IEnumerable<IStyleSheet> GetStyleSheets(this INode parent)
  {
    foreach (INode childNode in (IEnumerable<INode>) parent.ChildNodes)
    {
      if (childNode.NodeType == NodeType.Element)
      {
        if (childNode is ILinkStyle linkStyle)
        {
          IStyleSheet sheet = linkStyle.Sheet;
          if (sheet != null && !sheet.IsDisabled)
            yield return sheet;
        }
        else
        {
          foreach (IStyleSheet styleSheet in childNode.GetStyleSheets())
            yield return styleSheet;
        }
      }
    }
  }

  public static string LocateNamespace(this IStyleSheetList sheets, string prefix)
  {
    string str = (string) null;
    int length = sheets.Length;
    for (int index = 0; index < length && str == null; ++index)
      str = sheets[index].LocateNamespace(prefix);
    return str;
  }
}
