// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.IdDispenser
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public static class IdDispenser
{
  private static readonly IEqualityComparer<object> _comparer = (IEqualityComparer<object>) new IdDispenser.WrapperComparer();
  private static Dictionary<object, object> _hashtable = new Dictionary<object, object>(IdDispenser._comparer);
  private static readonly object _synchObject = new object();
  private static long _currentId = 42;
  private static long _cleanupId;
  private static int _cleanupGC;

  public static object GetObject(long id)
  {
    lock (IdDispenser._synchObject)
    {
      foreach (IdDispenser.Wrapper key in IdDispenser._hashtable.Keys)
      {
        if (key.Target != null && key.Id == id)
          return key.Target;
      }
      return (object) null;
    }
  }

  public static long GetId(object o)
  {
    if (o == null)
      return 0;
    lock (IdDispenser._synchObject)
    {
      object obj;
      if (IdDispenser._hashtable.TryGetValue(o, out obj))
        return ((IdDispenser.Wrapper) obj).Id;
      long uniqueId = checked (++IdDispenser._currentId);
      if (uniqueId - IdDispenser._cleanupId > (long) (1234 + IdDispenser._hashtable.Count / 2))
      {
        int num = GC.CollectionCount(0);
        if (num != IdDispenser._cleanupGC)
        {
          IdDispenser.Cleanup();
          IdDispenser._cleanupId = uniqueId;
          IdDispenser._cleanupGC = num;
        }
        else
          IdDispenser._cleanupId += 1234L;
      }
      IdDispenser.Wrapper key = new IdDispenser.Wrapper(o, uniqueId);
      IdDispenser._hashtable[(object) key] = (object) key;
      return uniqueId;
    }
  }

  private static void Cleanup()
  {
    int num1 = 0;
    int num2 = 0;
    foreach (IdDispenser.Wrapper key in IdDispenser._hashtable.Keys)
    {
      if (key.Target != null)
        ++num1;
      else
        ++num2;
    }
    if (num2 <= num1 / 4)
      return;
    Dictionary<object, object> dictionary = new Dictionary<object, object>(num1 + num1 / 4, IdDispenser._comparer);
    foreach (IdDispenser.Wrapper key in IdDispenser._hashtable.Keys)
    {
      if (key.Target != null)
        dictionary[(object) key] = (object) key;
    }
    IdDispenser._hashtable = dictionary;
  }

  private sealed class Wrapper
  {
    private readonly WeakReference _weakReference;
    private readonly int _hashCode;

    public Wrapper(object obj, long uniqueId)
    {
      this._weakReference = new WeakReference(obj, true);
      this._hashCode = obj == null ? 0 : ReferenceEqualityComparer<object>.Instance.GetHashCode(obj);
      this.Id = uniqueId;
    }

    public long Id { get; }

    public object Target => this._weakReference.Target;

    public override int GetHashCode() => this._hashCode;
  }

  private sealed class WrapperComparer : IEqualityComparer<object>
  {
    bool IEqualityComparer<object>.Equals(object x, object y)
    {
      if (x is IdDispenser.Wrapper wrapper1)
        x = wrapper1.Target;
      if (y is IdDispenser.Wrapper wrapper2)
        y = wrapper2.Target;
      return x == y;
    }

    int IEqualityComparer<object>.GetHashCode(object obj)
    {
      return obj is IdDispenser.Wrapper wrapper ? wrapper.GetHashCode() : IdDispenser.WrapperComparer.GetHashCodeWorker(obj);
    }

    private static int GetHashCodeWorker(object o)
    {
      return o == null ? 0 : ReferenceEqualityComparer<object>.Instance.GetHashCode(o);
    }
  }
}
