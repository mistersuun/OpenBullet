// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.CharRope
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.IO;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

public static class CharRope
{
  public static Rope<char> Create(string text)
  {
    return text != null ? new Rope<char>(CharRope.InitFromString(text)) : throw new ArgumentNullException(nameof (text));
  }

  public static string ToString(this Rope<char> rope, int startIndex, int length)
  {
    if (rope == null)
      throw new ArgumentNullException(nameof (rope));
    if (length == 0)
      return string.Empty;
    char[] array = new char[length];
    rope.CopyTo(startIndex, array, 0, length);
    return new string(array);
  }

  public static void WriteTo(this Rope<char> rope, TextWriter output, int startIndex, int length)
  {
    if (rope == null)
      throw new ArgumentNullException(nameof (rope));
    if (output == null)
      throw new ArgumentNullException(nameof (output));
    rope.VerifyRange(startIndex, length);
    rope.root.WriteTo(startIndex, output, length);
  }

  public static void AddText(this Rope<char> rope, string text)
  {
    rope.InsertText(rope.Length, text);
  }

  public static void InsertText(this Rope<char> rope, int index, string text)
  {
    if (rope == null)
      throw new ArgumentNullException(nameof (rope));
    rope.InsertRange(index, text.ToCharArray(), 0, text.Length);
  }

  internal static RopeNode<char> InitFromString(string text)
  {
    if (text.Length == 0)
      return RopeNode<char>.emptyRopeNode;
    RopeNode<char> nodes = RopeNode<char>.CreateNodes(text.Length);
    CharRope.FillNode(nodes, text, 0);
    return nodes;
  }

  private static void FillNode(RopeNode<char> node, string text, int start)
  {
    if (node.contents != null)
    {
      text.CopyTo(start, node.contents, 0, node.length);
    }
    else
    {
      CharRope.FillNode(node.left, text, start);
      CharRope.FillNode(node.right, text, start + node.left.length);
    }
  }

  internal static void WriteTo(this RopeNode<char> node, int index, TextWriter output, int count)
  {
    if (node.height == (byte) 0)
    {
      if (node.contents == null)
        node.GetContentNode().WriteTo(index, output, count);
      else
        output.Write(node.contents, index, count);
    }
    else if (index + count <= node.left.length)
      node.left.WriteTo(index, output, count);
    else if (index >= node.left.length)
    {
      node.right.WriteTo(index - node.left.length, output, count);
    }
    else
    {
      int count1 = node.left.length - index;
      node.left.WriteTo(index, output, count1);
      node.right.WriteTo(0, output, count - count1);
    }
  }

  public static int IndexOfAny(this Rope<char> rope, char[] anyOf, int startIndex, int length)
  {
    if (rope == null)
      throw new ArgumentNullException(nameof (rope));
    if (anyOf == null)
      throw new ArgumentNullException(nameof (anyOf));
    rope.VerifyRange(startIndex, length);
    while (length > 0)
    {
      Rope<char>.RopeCacheEntry ropeCacheEntry = rope.FindNodeUsingCache(startIndex).PeekOrDefault();
      char[] contents = ropeCacheEntry.node.contents;
      int num1 = startIndex - ropeCacheEntry.nodeStartIndex;
      int num2 = Math.Min(ropeCacheEntry.node.length, num1 + length);
      for (int index = startIndex - ropeCacheEntry.nodeStartIndex; index < num2; ++index)
      {
        char ch1 = contents[index];
        foreach (char ch2 in anyOf)
        {
          if ((int) ch1 == (int) ch2)
            return ropeCacheEntry.nodeStartIndex + index;
        }
      }
      length -= num2 - num1;
      startIndex = ropeCacheEntry.nodeStartIndex + num2;
    }
    return -1;
  }

  public static int IndexOf(
    this Rope<char> rope,
    string searchText,
    int startIndex,
    int length,
    StringComparison comparisonType)
  {
    if (rope == null)
      throw new ArgumentNullException(nameof (rope));
    if (searchText == null)
      throw new ArgumentNullException(nameof (searchText));
    rope.VerifyRange(startIndex, length);
    int num = rope.ToString(startIndex, length).IndexOf(searchText, comparisonType);
    return num < 0 ? -1 : num + startIndex;
  }

  public static int LastIndexOf(
    this Rope<char> rope,
    string searchText,
    int startIndex,
    int length,
    StringComparison comparisonType)
  {
    if (rope == null)
      throw new ArgumentNullException(nameof (rope));
    if (searchText == null)
      throw new ArgumentNullException(nameof (searchText));
    rope.VerifyRange(startIndex, length);
    int num = rope.ToString(startIndex, length).LastIndexOf(searchText, comparisonType);
    return num < 0 ? -1 : num + startIndex;
  }
}
