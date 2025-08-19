// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.Rope`1
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

[Serializable]
public sealed class Rope<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, ICloneable
{
  internal RopeNode<T> root;
  [NonSerialized]
  private volatile ImmutableStack<Rope<T>.RopeCacheEntry> lastUsedNodeStack;

  internal Rope(RopeNode<T> root) => this.root = root;

  public Rope() => this.root = RopeNode<T>.emptyRopeNode;

  public Rope(IEnumerable<T> input)
  {
    switch (input)
    {
      case null:
        throw new ArgumentNullException(nameof (input));
      case Rope<T> rope:
        rope.root.Publish();
        this.root = rope.root;
        break;
      case string text:
        ((Rope<char>) this).root = CharRope.InitFromString(text);
        break;
      default:
        T[] array = Rope<T>.ToArray(input);
        this.root = RopeNode<T>.CreateFromArray(array, 0, array.Length);
        break;
    }
  }

  public Rope(T[] array, int arrayIndex, int count)
  {
    Rope<T>.VerifyArrayWithRange(array, arrayIndex, count);
    this.root = RopeNode<T>.CreateFromArray(array, arrayIndex, count);
  }

  public Rope(int length, Func<Rope<T>> initializer)
  {
    if (initializer == null)
      throw new ArgumentNullException(nameof (initializer));
    if (length < 0)
      throw new ArgumentOutOfRangeException(nameof (length), (object) length, "Length must not be negative");
    if (length == 0)
      this.root = RopeNode<T>.emptyRopeNode;
    else
      this.root = (RopeNode<T>) new FunctionNode<T>(length, initializer);
  }

  private static T[] ToArray(IEnumerable<T> input)
  {
    return input is T[] objArray ? objArray : input.ToArray<T>();
  }

  public Rope<T> Clone()
  {
    this.root.Publish();
    return new Rope<T>(this.root);
  }

  object ICloneable.Clone() => (object) this.Clone();

  public void Clear()
  {
    this.root = RopeNode<T>.emptyRopeNode;
    this.OnChanged();
  }

  public int Length => this.root.length;

  public int Count => this.root.length;

  public void InsertRange(int index, Rope<T> newElements)
  {
    if (index < 0 || index > this.Length)
      throw new ArgumentOutOfRangeException(nameof (index), (object) index, "0 <= index <= " + this.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (newElements == null)
      throw new ArgumentNullException(nameof (newElements));
    newElements.root.Publish();
    this.root = this.root.Insert(index, newElements.root);
    this.OnChanged();
  }

  public void InsertRange(int index, IEnumerable<T> newElements)
  {
    if (newElements == null)
      throw new ArgumentNullException(nameof (newElements));
    if (newElements is Rope<T> newElements1)
    {
      this.InsertRange(index, newElements1);
    }
    else
    {
      T[] array = Rope<T>.ToArray(newElements);
      this.InsertRange(index, array, 0, array.Length);
    }
  }

  public void InsertRange(int index, T[] array, int arrayIndex, int count)
  {
    if (index < 0 || index > this.Length)
      throw new ArgumentOutOfRangeException(nameof (index), (object) index, "0 <= index <= " + this.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    Rope<T>.VerifyArrayWithRange(array, arrayIndex, count);
    if (count <= 0)
      return;
    this.root = this.root.Insert(index, array, arrayIndex, count);
    this.OnChanged();
  }

  public void AddRange(IEnumerable<T> newElements) => this.InsertRange(this.Length, newElements);

  public void AddRange(Rope<T> newElements) => this.InsertRange(this.Length, newElements);

  public void AddRange(T[] array, int arrayIndex, int count)
  {
    this.InsertRange(this.Length, array, arrayIndex, count);
  }

  public void RemoveRange(int index, int count)
  {
    this.VerifyRange(index, count);
    if (count <= 0)
      return;
    this.root = this.root.RemoveRange(index, count);
    this.OnChanged();
  }

  public void SetRange(int index, T[] array, int arrayIndex, int count)
  {
    this.VerifyRange(index, count);
    Rope<T>.VerifyArrayWithRange(array, arrayIndex, count);
    if (count <= 0)
      return;
    this.root = this.root.StoreElements(index, array, arrayIndex, count);
    this.OnChanged();
  }

  public Rope<T> GetRange(int index, int count)
  {
    this.VerifyRange(index, count);
    Rope<T> range = this.Clone();
    int index1 = index + count;
    range.RemoveRange(index1, range.Length - index1);
    range.RemoveRange(0, index);
    return range;
  }

  public static Rope<T> Concat(Rope<T> left, Rope<T> right)
  {
    if (left == null)
      throw new ArgumentNullException(nameof (left));
    if (right == null)
      throw new ArgumentNullException(nameof (right));
    left.root.Publish();
    right.root.Publish();
    return new Rope<T>(RopeNode<T>.Concat(left.root, right.root));
  }

  public static Rope<T> Concat(params Rope<T>[] ropes)
  {
    if (ropes == null)
      throw new ArgumentNullException(nameof (ropes));
    Rope<T> rope1 = new Rope<T>();
    foreach (Rope<T> rope2 in ropes)
      rope1.AddRange(rope2);
    return rope1;
  }

  internal void OnChanged()
  {
    this.lastUsedNodeStack = (ImmutableStack<Rope<T>.RopeCacheEntry>) null;
  }

  public T this[int index]
  {
    get
    {
      Rope<T>.RopeCacheEntry ropeCacheEntry = (uint) index < (uint) this.Length ? this.FindNodeUsingCache(index).PeekOrDefault() : throw new ArgumentOutOfRangeException(nameof (index), (object) index, "0 <= index < " + this.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return ropeCacheEntry.node.contents[index - ropeCacheEntry.nodeStartIndex];
    }
    set
    {
      if (index < 0 || index >= this.Length)
        throw new ArgumentOutOfRangeException(nameof (index), (object) index, "0 <= index < " + this.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.root = this.root.SetElement(index, value);
      this.OnChanged();
    }
  }

  internal ImmutableStack<Rope<T>.RopeCacheEntry> FindNodeUsingCache(int index)
  {
    ImmutableStack<Rope<T>.RopeCacheEntry> nodeUsingCache = this.lastUsedNodeStack;
    ImmutableStack<Rope<T>.RopeCacheEntry> immutableStack = nodeUsingCache;
    if (nodeUsingCache == null)
      nodeUsingCache = ImmutableStack<Rope<T>.RopeCacheEntry>.Empty.Push(new Rope<T>.RopeCacheEntry(this.root, 0));
    while (!nodeUsingCache.PeekOrDefault().IsInside(index))
      nodeUsingCache = nodeUsingCache.Pop();
    while (true)
    {
      Rope<T>.RopeCacheEntry ropeCacheEntry = nodeUsingCache.PeekOrDefault();
      if (ropeCacheEntry.node.height == (byte) 0)
      {
        if (ropeCacheEntry.node.contents == null)
          ropeCacheEntry = new Rope<T>.RopeCacheEntry(ropeCacheEntry.node.GetContentNode(), ropeCacheEntry.nodeStartIndex);
        if (ropeCacheEntry.node.contents != null)
          break;
      }
      nodeUsingCache = index - ropeCacheEntry.nodeStartIndex < ropeCacheEntry.node.left.length ? nodeUsingCache.Push(new Rope<T>.RopeCacheEntry(ropeCacheEntry.node.left, ropeCacheEntry.nodeStartIndex)) : nodeUsingCache.Push(new Rope<T>.RopeCacheEntry(ropeCacheEntry.node.right, ropeCacheEntry.nodeStartIndex + ropeCacheEntry.node.left.length));
    }
    if (immutableStack != nodeUsingCache)
      this.lastUsedNodeStack = nodeUsingCache;
    return nodeUsingCache;
  }

  internal void VerifyRange(int startIndex, int length)
  {
    if (startIndex < 0 || startIndex > this.Length)
      throw new ArgumentOutOfRangeException(nameof (startIndex), (object) startIndex, "0 <= startIndex <= " + this.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (length < 0 || startIndex + length > this.Length)
      throw new ArgumentOutOfRangeException(nameof (length), (object) length, $"0 <= length, startIndex({(object) startIndex})+length <= {this.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture)}");
  }

  internal static void VerifyArrayWithRange(T[] array, int arrayIndex, int count)
  {
    if (array == null)
      throw new ArgumentNullException(nameof (array));
    if (arrayIndex < 0 || arrayIndex > array.Length)
      throw new ArgumentOutOfRangeException("startIndex", (object) arrayIndex, "0 <= arrayIndex <= " + array.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (count < 0 || arrayIndex + count > array.Length)
      throw new ArgumentOutOfRangeException(nameof (count), (object) count, $"0 <= length, arrayIndex({(object) arrayIndex})+count <= {array.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture)}");
  }

  public override string ToString()
  {
    if (this is Rope<char> rope)
      return rope.ToString(0, this.Length);
    StringBuilder stringBuilder = new StringBuilder();
    foreach (T obj in this)
    {
      if (stringBuilder.Length == 0)
        stringBuilder.Append('{');
      else
        stringBuilder.Append(", ");
      stringBuilder.Append(obj.ToString());
    }
    stringBuilder.Append('}');
    return stringBuilder.ToString();
  }

  internal string GetTreeAsString() => "Not available in release build.";

  bool ICollection<T>.IsReadOnly => false;

  public int IndexOf(T item) => this.IndexOf(item, 0, this.Length);

  public int IndexOf(T item, int startIndex, int count)
  {
    this.VerifyRange(startIndex, count);
    while (count > 0)
    {
      Rope<T>.RopeCacheEntry ropeCacheEntry = this.FindNodeUsingCache(startIndex).PeekOrDefault();
      T[] contents = ropeCacheEntry.node.contents;
      int startIndex1 = startIndex - ropeCacheEntry.nodeStartIndex;
      int num1 = Math.Min(ropeCacheEntry.node.length, startIndex1 + count);
      int num2 = Array.IndexOf<T>(contents, item, startIndex1, num1 - startIndex1);
      if (num2 >= 0)
        return ropeCacheEntry.nodeStartIndex + num2;
      count -= num1 - startIndex1;
      startIndex = ropeCacheEntry.nodeStartIndex + num1;
    }
    return -1;
  }

  public int LastIndexOf(T item) => this.LastIndexOf(item, 0, this.Length);

  public int LastIndexOf(T item, int startIndex, int count)
  {
    this.VerifyRange(startIndex, count);
    EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
    for (int index = startIndex + count - 1; index >= startIndex; --index)
    {
      if (equalityComparer.Equals(this[index], item))
        return index;
    }
    return -1;
  }

  public void Insert(int index, T item)
  {
    this.InsertRange(index, new T[1]{ item }, 0, 1);
  }

  public void RemoveAt(int index) => this.RemoveRange(index, 1);

  public void Add(T item)
  {
    this.InsertRange(this.Length, new T[1]{ item }, 0, 1);
  }

  public bool Contains(T item) => this.IndexOf(item) >= 0;

  public void CopyTo(T[] array, int arrayIndex) => this.CopyTo(0, array, arrayIndex, this.Length);

  public void CopyTo(int index, T[] array, int arrayIndex, int count)
  {
    this.VerifyRange(index, count);
    Rope<T>.VerifyArrayWithRange(array, arrayIndex, count);
    this.root.CopyTo(index, array, arrayIndex, count);
  }

  public bool Remove(T item)
  {
    int index = this.IndexOf(item);
    if (index < 0)
      return false;
    this.RemoveAt(index);
    return true;
  }

  public IEnumerator<T> GetEnumerator()
  {
    this.root.Publish();
    return Rope<T>.Enumerate(this.root);
  }

  public T[] ToArray()
  {
    T[] array = new T[this.Length];
    this.root.CopyTo(0, array, 0, array.Length);
    return array;
  }

  public T[] ToArray(int startIndex, int count)
  {
    this.VerifyRange(startIndex, count);
    T[] array = new T[count];
    this.CopyTo(startIndex, array, 0, count);
    return array;
  }

  private static IEnumerator<T> Enumerate(RopeNode<T> node)
  {
    Stack<RopeNode<T>> stack = new Stack<RopeNode<T>>();
    for (; node != null; node = stack.Count <= 0 ? (RopeNode<T>) null : stack.Pop())
    {
      while (node.contents == null)
      {
        if (node.height == (byte) 0)
        {
          node = node.GetContentNode();
        }
        else
        {
          stack.Push(node.right);
          node = node.left;
        }
      }
      for (int i = 0; i < node.length; ++i)
        yield return node.contents[i];
    }
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  internal struct RopeCacheEntry
  {
    internal readonly RopeNode<T> node;
    internal readonly int nodeStartIndex;

    internal RopeCacheEntry(RopeNode<T> node, int nodeStartOffset)
    {
      this.node = node;
      this.nodeStartIndex = nodeStartOffset;
    }

    internal bool IsInside(int offset)
    {
      return offset >= this.nodeStartIndex && offset < this.nodeStartIndex + this.node.length;
    }
  }
}
