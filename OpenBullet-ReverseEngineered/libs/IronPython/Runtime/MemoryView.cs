// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.MemoryView
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System.Collections.Generic;
using System.Numerics;

#nullable disable
namespace IronPython.Runtime;

[PythonType("memoryview")]
public sealed class MemoryView : ICodeFormattable
{
  private readonly IBufferProtocol _buffer;
  private readonly int _start;
  private readonly int? _end;
  public const object __hash__ = null;

  public MemoryView(IBufferProtocol obj) => this._buffer = obj;

  private MemoryView(IBufferProtocol obj, int start, int? end)
    : this(obj)
  {
    this._start = start;
    this._end = end;
  }

  public MemoryView(MemoryView obj)
  {
    this._buffer = obj._buffer;
    this._start = obj._start;
    this._end = obj._end;
  }

  public MemoryView(PythonBuffer obj)
  {
    this._buffer = (IBufferProtocol) new IListOfBytesBufferProtocol((IList<byte>) obj);
  }

  public MemoryView(string obj) => this._buffer = (IBufferProtocol) new StringBufferProtocol(obj);

  public int __len__()
  {
    return this._end.HasValue ? this._end.Value - this._start : this._buffer.ItemCount;
  }

  public string format => this._buffer.Format;

  public BigInteger itemsize => this._buffer.ItemSize;

  public BigInteger ndim => this._buffer.NumberDimensions;

  public bool @readonly => this._buffer.ReadOnly;

  public PythonTuple shape
  {
    get
    {
      IList<BigInteger> shape = this._buffer.GetShape(this._start, this._end);
      return shape == null ? (PythonTuple) null : new PythonTuple((object) shape);
    }
  }

  public PythonTuple strides => this._buffer.Strides;

  public object suboffsets => this._buffer.SubOffsets;

  public Bytes tobytes() => this._buffer.ToBytes(this._start, this._end);

  public List tolist() => this._buffer.ToList(this._start, this._end);

  public object this[int index]
  {
    get
    {
      index = PythonOps.FixIndex(index, this.__len__());
      return (object) this._buffer.GetItem(index + this._start);
    }
    set
    {
      if (this._buffer.ReadOnly)
        throw PythonOps.TypeError("cannot modify read-only memory");
      index = PythonOps.FixIndex(index, this.__len__());
      this._buffer.SetItem(index + this._start, value);
    }
  }

  public void __delitem__(int index)
  {
    if (this._buffer.ReadOnly)
      throw PythonOps.TypeError("cannot modify read-only memory");
    throw PythonOps.TypeError("cannot delete memory");
  }

  public void __delitem__([NotNull] Slice slice)
  {
    if (this._buffer.ReadOnly)
      throw PythonOps.TypeError("cannot modify read-only memory");
    throw PythonOps.TypeError("cannot delete memory");
  }

  public object this[[NotNull] Slice slice]
  {
    get
    {
      int start;
      int stop;
      MemoryView.FixSlice(slice, this.__len__(), out start, out stop);
      return (object) new MemoryView(this._buffer, this._start + start, new int?(this._start + stop));
    }
    set
    {
      if (this._buffer.ReadOnly)
        throw PythonOps.TypeError("cannot modify read-only memory");
      int start;
      int stop;
      MemoryView.FixSlice(slice, this.__len__(), out start, out stop);
      int num = PythonOps.Length(value);
      if (stop - start != num)
        throw PythonOps.ValueError("cannot resize memory view");
      this._buffer.SetSlice(new Slice((object) (this._start + start), (object) (this._start + stop)), value);
    }
  }

  public static void FixSlice(Slice slice, int len, out int start, out int stop)
  {
    if (slice.step != null)
      throw PythonOps.NotImplementedError("");
    slice.indices(len, out start, out stop, out int _);
    if (stop >= start)
      return;
    stop = start;
  }

  public static bool operator >(MemoryView self, IBufferProtocol other)
  {
    return self > new MemoryView(other);
  }

  public static bool operator >(IBufferProtocol self, MemoryView other)
  {
    return new MemoryView(self) > other;
  }

  public static bool operator >(MemoryView self, MemoryView other)
  {
    if ((object) self == null)
      return other != null;
    return (object) other == null || self.tobytes() > other.tobytes();
  }

  public static bool operator <(MemoryView self, MemoryView other)
  {
    if ((object) self == null)
      return (object) other == null;
    return (object) other != null && self.tobytes() < other.tobytes();
  }

  public static bool operator <(MemoryView self, IBufferProtocol other)
  {
    return self < new MemoryView(other);
  }

  public static bool operator <(IBufferProtocol self, MemoryView other)
  {
    return new MemoryView(self) < other;
  }

  public static bool operator >=(MemoryView self, MemoryView other)
  {
    if ((object) self == null)
      return (object) other == null;
    return (object) other != null && self.tobytes() >= other.tobytes();
  }

  public static bool operator >=(MemoryView self, IBufferProtocol other)
  {
    return self >= new MemoryView(other);
  }

  public static bool operator >=(IBufferProtocol self, MemoryView other)
  {
    return new MemoryView(self) >= other;
  }

  public static bool operator <=(MemoryView self, MemoryView other)
  {
    if ((object) self == null)
      return other != null;
    return (object) other == null || self.tobytes() <= other.tobytes();
  }

  public static bool operator <=(MemoryView self, IBufferProtocol other)
  {
    return self <= new MemoryView(other);
  }

  public static bool operator <=(IBufferProtocol self, MemoryView other)
  {
    return new MemoryView(self) <= other;
  }

  public static bool operator ==(MemoryView self, MemoryView other)
  {
    if ((object) self == null)
      return (object) other == null;
    return (object) other != null && self.tobytes().Equals((object) other.tobytes());
  }

  public static bool operator ==(MemoryView self, IBufferProtocol other)
  {
    return self == new MemoryView(other);
  }

  public static bool operator ==(IBufferProtocol self, MemoryView other)
  {
    return new MemoryView(self) == other;
  }

  public static bool operator !=(MemoryView self, MemoryView other)
  {
    if ((object) self == null)
      return other != null;
    return (object) other == null || !self.tobytes().Equals((object) other.tobytes());
  }

  public static bool operator !=(MemoryView self, IBufferProtocol other)
  {
    return self != new MemoryView(other);
  }

  public static bool operator !=(IBufferProtocol self, MemoryView other)
  {
    return new MemoryView(self) != other;
  }

  public override bool Equals(object obj)
  {
    MemoryView memoryView = obj as MemoryView;
    return (object) memoryView != null && this == memoryView;
  }

  public override int GetHashCode() => base.GetHashCode();

  public string __repr__(CodeContext context) => $"<memory at {PythonOps.Id((object) this)}>";
}
