// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.WeakHashtable
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Collections;
using System.Security.Permissions;

#nullable disable
namespace Microsoft.Windows.Controls;

[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
internal sealed class WeakHashtable : Hashtable
{
  private static IEqualityComparer _comparer = (IEqualityComparer) new WeakHashtable.WeakKeyComparer();
  private long _lastGlobalMem;
  private int _lastHashCount;

  internal WeakHashtable()
    : base(WeakHashtable._comparer)
  {
  }

  public override void Clear() => base.Clear();

  public override void Remove(object key) => base.Remove(key);

  public void SetWeak(object key, object value)
  {
    this.ScavengeKeys();
    this[(object) new WeakHashtable.EqualityWeakReference(key)] = value;
  }

  private void ScavengeKeys()
  {
    int count = this.Count;
    if (count == 0)
      return;
    if (this._lastHashCount == 0)
    {
      this._lastHashCount = count;
    }
    else
    {
      long totalMemory = GC.GetTotalMemory(false);
      if (this._lastGlobalMem == 0L)
      {
        this._lastGlobalMem = totalMemory;
      }
      else
      {
        float num1 = (float) (totalMemory - this._lastGlobalMem) / (float) this._lastGlobalMem;
        float num2 = (float) (count - this._lastHashCount) / (float) this._lastHashCount;
        if ((double) num1 < 0.0 && (double) num2 >= 0.0)
        {
          ArrayList arrayList = (ArrayList) null;
          foreach (object key in (IEnumerable) this.Keys)
          {
            if (key is WeakHashtable.EqualityWeakReference equalityWeakReference && !equalityWeakReference.IsAlive)
            {
              if (arrayList == null)
                arrayList = new ArrayList();
              arrayList.Add((object) equalityWeakReference);
            }
          }
          if (arrayList != null)
          {
            foreach (object key in arrayList)
              this.Remove(key);
          }
        }
        this._lastGlobalMem = totalMemory;
        this._lastHashCount = count;
      }
    }
  }

  private class WeakKeyComparer : IEqualityComparer
  {
    bool IEqualityComparer.Equals(object x, object y)
    {
      if (x == null)
        return y == null;
      if (y == null || x.GetHashCode() != y.GetHashCode())
        return false;
      WeakHashtable.EqualityWeakReference equalityWeakReference1 = x as WeakHashtable.EqualityWeakReference;
      WeakHashtable.EqualityWeakReference equalityWeakReference2 = y as WeakHashtable.EqualityWeakReference;
      if (equalityWeakReference1 != null && equalityWeakReference2 != null && !equalityWeakReference2.IsAlive && !equalityWeakReference1.IsAlive)
        return true;
      if (equalityWeakReference1 != null)
        x = equalityWeakReference1.Target;
      if (equalityWeakReference2 != null)
        y = equalityWeakReference2.Target;
      return object.ReferenceEquals(x, y);
    }

    int IEqualityComparer.GetHashCode(object obj) => obj.GetHashCode();
  }

  private sealed class EqualityWeakReference
  {
    private int _hashCode;
    private WeakReference _weakRef;

    internal EqualityWeakReference(object o)
    {
      this._weakRef = new WeakReference(o);
      this._hashCode = o.GetHashCode();
    }

    public bool IsAlive => this._weakRef.IsAlive;

    public object Target => this._weakRef.Target;

    public override bool Equals(object o)
    {
      return o != null && o.GetHashCode() == this._hashCode && (o == this || object.ReferenceEquals(o, this.Target));
    }

    public override int GetHashCode() => this._hashCode;
  }
}
