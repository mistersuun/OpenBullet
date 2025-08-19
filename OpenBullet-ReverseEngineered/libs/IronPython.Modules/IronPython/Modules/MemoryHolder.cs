// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.MemoryHolder
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Modules;

internal sealed class MemoryHolder : CriticalFinalizerObject
{
  private readonly IntPtr _data;
  private readonly bool _ownsData;
  private readonly int _size;
  private PythonDictionary _objects;
  private readonly MemoryHolder _parent;

  [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
  public MemoryHolder(int size)
  {
    RuntimeHelpers.PrepareConstrainedRegions();
    try
    {
    }
    finally
    {
      this._size = size;
      this._data = NativeFunctions.Calloc(new IntPtr(size));
      if (this._data == IntPtr.Zero)
      {
        GC.SuppressFinalize((object) this);
        throw new OutOfMemoryException();
      }
      this._ownsData = true;
    }
  }

  public MemoryHolder(IntPtr data, int size)
  {
    GC.SuppressFinalize((object) this);
    this._data = data;
    this._size = size;
  }

  public MemoryHolder(IntPtr data, int size, MemoryHolder parent)
  {
    GC.SuppressFinalize((object) this);
    this._data = data;
    this._parent = parent;
    this._objects = parent._objects;
    this._size = size;
  }

  public IntPtr UnsafeAddress => this._data;

  public int Size => this._size;

  public PythonDictionary Objects
  {
    get => this._objects;
    set => this._objects = value;
  }

  internal PythonDictionary EnsureObjects()
  {
    if (this._objects == null)
      Interlocked.CompareExchange<PythonDictionary>(ref this._objects, new PythonDictionary(), (PythonDictionary) null);
    return this._objects;
  }

  internal void AddObject(object key, object value) => this.EnsureObjects()[key] = value;

  private short Swap(short val)
  {
    return (short) (((int) (ushort) val & 65280) >> 8 | ((int) (ushort) val & (int) byte.MaxValue) << 8);
  }

  private int Swap(int val)
  {
    val = val >>> 16 /*0x10*/ | val << 16 /*0x10*/;
    return (val & -16711936 /*0xFF00FF00*/) >>> 8 | (val & 16711935) << 8;
  }

  private long Swap(long val)
  {
    val = val >>> 32 /*0x20*/ | val << 32 /*0x20*/;
    val = (val & -281470681808896L /*0xFFFF0000FFFF0000*/) >>> 16 /*0x10*/ | (val & 281470681808895L) << 16 /*0x10*/;
    return (val & -71777214294589696L /*0xFF00FF00FF00FF00*/) >>> 8 | (val & 71777214294589695L) << 8;
  }

  public byte ReadByte(int offset)
  {
    int num = (int) Marshal.ReadByte(this._data, offset);
    GC.KeepAlive((object) this);
    return (byte) num;
  }

  public short ReadInt16(int offset, bool swap = false)
  {
    short val = Marshal.ReadInt16(this._data, offset);
    GC.KeepAlive((object) this);
    if (swap)
      val = this.Swap(val);
    return val;
  }

  public int ReadInt32(int offset, bool swap = false)
  {
    int val = Marshal.ReadInt32(this._data, offset);
    GC.KeepAlive((object) this);
    if (swap)
      val = this.Swap(val);
    return val;
  }

  public long ReadInt64(int offset, bool swap = false)
  {
    long val = Marshal.ReadInt64(this._data, offset);
    GC.KeepAlive((object) this);
    if (swap)
      val = this.Swap(val);
    return val;
  }

  public IntPtr ReadIntPtr(int offset)
  {
    IntPtr num = Marshal.ReadIntPtr(this._data, offset);
    GC.KeepAlive((object) this);
    return num;
  }

  public MemoryHolder ReadMemoryHolder(int offset)
  {
    return new MemoryHolder(Marshal.ReadIntPtr(this._data, offset), IntPtr.Size, this);
  }

  internal string ReadAnsiString(int offset)
  {
    try
    {
      return Marshal.PtrToStringAnsi(this._data.Add(offset));
    }
    finally
    {
      GC.KeepAlive((object) this);
    }
  }

  internal string ReadUnicodeString(int offset)
  {
    try
    {
      return Marshal.PtrToStringUni(this._data.Add(offset));
    }
    finally
    {
      GC.KeepAlive((object) this);
    }
  }

  internal string ReadAnsiString(int offset, int length)
  {
    try
    {
      return MemoryHolder.ReadAnsiString(this._data, offset, length);
    }
    finally
    {
      GC.KeepAlive((object) this);
    }
  }

  internal static string ReadAnsiString(IntPtr addr, int offset, int length)
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (checked (offset + length) < int.MaxValue)
    {
      for (int index = 0; index < length; ++index)
        stringBuilder.Append((char) Marshal.ReadByte(addr, offset + index));
    }
    return stringBuilder.ToString();
  }

  internal static string ReadAnsiString(IntPtr addr, int offset)
  {
    StringBuilder stringBuilder = new StringBuilder();
    while (true)
    {
      IntPtr ptr = addr;
      int ofs = offset++;
      byte num;
      if ((num = Marshal.ReadByte(ptr, ofs)) != (byte) 0)
        stringBuilder.Append((char) num);
      else
        break;
    }
    return stringBuilder.ToString();
  }

  internal string ReadUnicodeString(int offset, int length)
  {
    try
    {
      return Marshal.PtrToStringUni(this._data.Add(offset), length);
    }
    finally
    {
      GC.KeepAlive((object) this);
    }
  }

  public void WriteByte(int offset, byte value)
  {
    Marshal.WriteByte(this._data, offset, value);
    GC.KeepAlive((object) this);
  }

  public void WriteInt16(int offset, short value, bool swap = false)
  {
    Marshal.WriteInt16(this._data, offset, swap ? this.Swap(value) : value);
    GC.KeepAlive((object) this);
  }

  public void WriteInt32(int offset, int value, bool swap = false)
  {
    Marshal.WriteInt32(this._data, offset, swap ? this.Swap(value) : value);
    GC.KeepAlive((object) this);
  }

  public void WriteInt64(int offset, long value, bool swap = false)
  {
    Marshal.WriteInt64(this._data, offset, swap ? this.Swap(value) : value);
    GC.KeepAlive((object) this);
  }

  public void WriteIntPtr(int offset, IntPtr value)
  {
    Marshal.WriteIntPtr(this._data, offset, value);
    GC.KeepAlive((object) this);
  }

  public void WriteIntPtr(int offset, MemoryHolder address)
  {
    Marshal.WriteIntPtr(this._data, offset, address.UnsafeAddress);
    GC.KeepAlive((object) this);
    GC.KeepAlive((object) address);
  }

  public void CopyFrom(IntPtr source, IntPtr size)
  {
    NativeFunctions.CopyMemory(this._data, source, size);
    GC.KeepAlive((object) this);
  }

  internal void WriteUnicodeString(int offset, string value)
  {
    for (int index = 0; index < value.Length; ++index)
      this.WriteInt16(checked (offset + index * 2), (short) value[index]);
  }

  internal void WriteAnsiString(int offset, string value)
  {
    for (int index = 0; index < value.Length; ++index)
      this.WriteByte(checked (offset + index), (byte) value[index]);
  }

  public MemoryHolder GetSubBlock(int offset)
  {
    return new MemoryHolder(this._data.Add(offset), this._size - offset, this);
  }

  public void CopyTo(MemoryHolder destAddress, int writeOffset, int size)
  {
    NativeFunctions.CopyMemory(destAddress._data.Add(writeOffset), this._data, new IntPtr(size));
    GC.KeepAlive((object) destAddress);
    GC.KeepAlive((object) this);
  }

  [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
  ~MemoryHolder()
  {
    if (!this._ownsData)
      return;
    Marshal.FreeHGlobal(this._data);
  }
}
