// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.ImmutableStack`1
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

[Serializable]
public sealed class ImmutableStack<T> : IEnumerable<T>, IEnumerable
{
  public static readonly ImmutableStack<T> Empty = new ImmutableStack<T>();
  private readonly T value;
  private readonly ImmutableStack<T> next;

  private ImmutableStack()
  {
  }

  private ImmutableStack(T value, ImmutableStack<T> next)
  {
    this.value = value;
    this.next = next;
  }

  public ImmutableStack<T> Push(T item) => new ImmutableStack<T>(item, this);

  public T Peek()
  {
    if (this.IsEmpty)
      throw new InvalidOperationException("Operation not valid on empty stack.");
    return this.value;
  }

  public T PeekOrDefault() => this.value;

  public ImmutableStack<T> Pop()
  {
    if (this.IsEmpty)
      throw new InvalidOperationException("Operation not valid on empty stack.");
    return this.next;
  }

  public bool IsEmpty => this.next == null;

  public IEnumerator<T> GetEnumerator()
  {
    for (ImmutableStack<T> t = this; !t.IsEmpty; t = t.next)
      yield return t.value;
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder("[Stack");
    foreach (T obj in this)
    {
      stringBuilder.Append(' ');
      stringBuilder.Append((object) obj);
    }
    stringBuilder.Append(']');
    return stringBuilder.ToString();
  }
}
