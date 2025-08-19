// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.ExtensibleString
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using System.Collections;

#nullable disable
namespace IronPython.Runtime.Operations;

public class ExtensibleString : Extensible<string>, ICodeFormattable, IStructuralEquatable
{
  public ExtensibleString()
    : base(string.Empty)
  {
  }

  public ExtensibleString(string self)
    : base(self)
  {
  }

  public override string ToString() => this.Value;

  public virtual string __repr__(CodeContext context) => StringOps.Quote(this.Value);

  [return: MaybeNotImplemented]
  public object __eq__(object other)
  {
    switch (other)
    {
      case string _:
      case ExtensibleString _:
      case Bytes _:
        return ScriptingRuntimeHelpers.BooleanToObject(this.EqualsWorker(other));
      default:
        return (object) NotImplementedType.Value;
    }
  }

  [return: MaybeNotImplemented]
  public object __ne__(object other)
  {
    switch (other)
    {
      case string _:
      case ExtensibleString _:
      case Bytes _:
        return ScriptingRuntimeHelpers.BooleanToObject(!this.EqualsWorker(other));
      default:
        return (object) NotImplementedType.Value;
    }
  }

  int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
  {
    if (comparer is PythonContext.PythonEqualityComparer)
      return this.GetHashCode();
    return ((IStructuralEquatable) PythonTuple.MakeTuple((object) this.Value.ToCharArray())).GetHashCode(comparer);
  }

  bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
  {
    if (comparer is PythonContext.PythonEqualityComparer)
      return this.EqualsWorker(other);
    switch (other)
    {
      case ExtensibleString extensibleString:
        return this.EqualsWorker(extensibleString.Value, comparer);
      case string other1:
        return this.EqualsWorker(other1, comparer);
      case Bytes bytes:
        return this.EqualsWorker(bytes.ToString(), comparer);
      default:
        return false;
    }
  }

  private bool EqualsWorker(object other)
  {
    switch (other)
    {
      case null:
        return false;
      case ExtensibleString extensibleString:
        return this.Value == extensibleString.Value;
      case string str:
        return this.Value == str;
      case Bytes bytes:
        return this.Value == bytes.ToString();
      default:
        return false;
    }
  }

  private bool EqualsWorker(string other, IEqualityComparer comparer)
  {
    if (this.Value.Length != other.Length)
      return false;
    if (this.Value.Length == 0)
      return true;
    for (int index = 0; index < this.Value.Length; ++index)
    {
      if (!comparer.Equals((object) this.Value[index], (object) other[index]))
        return false;
    }
    return true;
  }

  public virtual object this[int index]
  {
    get => (object) ScriptingRuntimeHelpers.CharToString(this.Value[index]);
  }

  public object this[Slice slice] => (object) StringOps.GetItem(this.Value, slice);

  public object __getslice__(int start, int stop)
  {
    return (object) StringOps.__getslice__(this.Value, start, stop);
  }

  public virtual int __len__() => this.Value.Length;

  public virtual bool __contains__(object value)
  {
    switch (value)
    {
      case string _:
        return this.Value.Contains((string) value);
      case ExtensibleString _:
        return this.Value.Contains(((Extensible<string>) value).Value);
      case Bytes _:
        return this.Value.Contains(value.ToString());
      default:
        throw PythonOps.TypeErrorForBadInstance("expected string, got {0}", value);
    }
  }
}
