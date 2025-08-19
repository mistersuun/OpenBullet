// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonBuffer
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Runtime;

[PythonType("buffer")]
[DontMapGetMemberNamesToDir]
public sealed class PythonBuffer : 
  ICodeFormattable,
  IDynamicMetaObjectProvider,
  IList<byte>,
  ICollection<byte>,
  IEnumerable<byte>,
  IEnumerable
{
  internal object _object;
  private int _offset;
  private readonly CodeContext _context;
  private byte[] _objectByteCache;

  [Python3Warning("buffer() not supported in 3.x")]
  public PythonBuffer(CodeContext context, object @object, int offset = 0, int size = -1)
  {
    PythonOps.Warn3k(context, "buffer() not supported in 3.x");
    if (!this.InitBufferObject(@object, offset, size))
      throw PythonOps.TypeError("expected buffer object");
    this._context = context;
  }

  private bool InitBufferObject(object o, int offset, int size)
  {
    if (offset < 0)
      throw PythonOps.ValueError("offset must be zero or positive");
    if (size < -1)
      throw PythonOps.ValueError("size must be zero or positive");
    int num;
    switch (o)
    {
      case PythonBuffer pythonBuffer:
        o = pythonBuffer._object;
        num = pythonBuffer.Size;
        break;
      case string str:
        num = str.Length;
        break;
      case Bytes _:
        num = ((Bytes) o).Count;
        break;
      case ByteArray _:
        num = ((ByteArray) o).Count;
        break;
      case Array _:
      case IPythonArray _:
        if (o is Array array)
        {
          Type elementType = array.GetType().GetElementType();
          if (!elementType.IsPrimitive && elementType != typeof (string))
            return false;
          num = array.Length;
          break;
        }
        num = ((ICollection<object>) o).Count;
        break;
      case IPythonBufferable _:
        num = ((IPythonBufferable) o).Size;
        this._object = o;
        break;
      default:
        return false;
    }
    this.Size = size >= num - offset || size == -1 ? num - offset : size;
    this._object = o;
    this._offset = offset;
    return true;
  }

  public override string ToString()
  {
    object selectedRange = this.GetSelectedRange();
    switch (selectedRange)
    {
      case Bytes _:
        return ((IList<byte>) selectedRange).MakeString();
      case ByteArray _:
        return ((IList<byte>) selectedRange).MakeString();
      case IPythonBufferable _:
        return ((IList<byte>) this.GetSelectedRange()).MakeString();
      case byte[] _:
        return ((IList<byte>) (byte[]) this.GetSelectedRange()).MakeString();
      default:
        return selectedRange.ToString();
    }
  }

  public int __cmp__([NotNull] PythonBuffer other)
  {
    return this == other ? 0 : PythonOps.Compare((object) this.ToString(), (object) other.ToString());
  }

  [PythonHidden(new PlatformID[] {})]
  public override bool Equals(object obj) => obj is PythonBuffer other && this.__cmp__(other) == 0;

  public override int GetHashCode()
  {
    return this._object.GetHashCode() ^ this._offset ^ (this.Size << 16 /*0x10*/ | this.Size >> 16 /*0x10*/);
  }

  private Slice GetSlice()
  {
    object stop = (object) null;
    if (this.Size >= 0)
      stop = (object) (this._offset + this.Size);
    return new Slice((object) this._offset, stop);
  }

  public object __getslice__(object start, object stop) => this[(object) new Slice(start, stop)];

  private static Exception ReadOnlyError() => PythonOps.TypeError("buffer is read-only");

  public object __setslice__(object start, object stop, object value)
  {
    throw PythonBuffer.ReadOnlyError();
  }

  public void __delitem__(int index) => throw PythonBuffer.ReadOnlyError();

  public void __delslice__(object start, object stop) => throw PythonBuffer.ReadOnlyError();

  public object this[object s]
  {
    get => PythonOps.GetIndex(this._context, this.GetSelectedRange(), s);
    set => throw PythonBuffer.ReadOnlyError();
  }

  private object GetSelectedRange()
  {
    if (this._object is IPythonArray pythonArray)
      return (object) pythonArray.tostring();
    if (this._object is ByteArray byteArray)
      return (object) new Bytes((IList<byte>) byteArray[this.GetSlice()]);
    return this._object is IPythonBufferable pythonBufferable ? (object) new Bytes((IList<byte>) pythonBufferable.GetBytes(this._offset, this.Size)) : PythonOps.GetIndex(this._context, this._object, (object) this.GetSlice());
  }

  public static object operator +(PythonBuffer a, PythonBuffer b)
  {
    return a._context.LanguageContext.Operation(PythonOperationKind.Add, PythonOps.GetIndex(a._context, a._object, (object) a.GetSlice()), PythonOps.GetIndex(a._context, b._object, (object) b.GetSlice()));
  }

  public static object operator +(PythonBuffer a, string b) => (object) (a.ToString() + b);

  public static object operator *(PythonBuffer b, int n)
  {
    return b._context.LanguageContext.Operation(PythonOperationKind.Multiply, PythonOps.GetIndex(b._context, b._object, (object) b.GetSlice()), (object) n);
  }

  public static object operator *(int n, PythonBuffer b)
  {
    return b._context.LanguageContext.Operation(PythonOperationKind.Multiply, PythonOps.GetIndex(b._context, b._object, (object) b.GetSlice()), (object) n);
  }

  public int __len__() => Math.Max(this.Size, 0);

  internal int Size { get; private set; }

  public string __repr__(CodeContext context)
  {
    return $"<read-only buffer for 0x{PythonOps.Id(this._object):X16}, size {this.Size}, offset {this._offset} at 0x{PythonOps.Id((object) this):X16}>";
  }

  DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
  {
    return (DynamicMetaObject) new PythonBuffer.BufferMeta(parameter, BindingRestrictions.Empty, (object) this);
  }

  internal byte[] byteCache
  {
    get
    {
      return this._objectByteCache ?? (this._objectByteCache = PythonOps.ConvertBufferToByteArray(this));
    }
  }

  [PythonHidden(new PlatformID[] {})]
  int IList<byte>.IndexOf(byte item)
  {
    for (int index = 0; index < this.byteCache.Length; ++index)
    {
      if ((int) this.byteCache[index] == (int) item)
        return index;
    }
    return -1;
  }

  [PythonHidden(new PlatformID[] {})]
  void IList<byte>.Insert(int index, byte item) => throw PythonBuffer.ReadOnlyError();

  [PythonHidden(new PlatformID[] {})]
  void IList<byte>.RemoveAt(int index) => throw PythonBuffer.ReadOnlyError();

  byte IList<byte>.this[int index]
  {
    [PythonHidden(new PlatformID[] {})] get => this.byteCache[index];
    [PythonHidden(new PlatformID[] {})] set => throw PythonBuffer.ReadOnlyError();
  }

  [PythonHidden(new PlatformID[] {})]
  IEnumerator IEnumerable.GetEnumerator() => this.byteCache.GetEnumerator();

  [PythonHidden(new PlatformID[] {})]
  IEnumerator<byte> IEnumerable<byte>.GetEnumerator()
  {
    return ((IEnumerable<byte>) this.byteCache).GetEnumerator();
  }

  [PythonHidden(new PlatformID[] {})]
  void ICollection<byte>.Add(byte item) => throw PythonBuffer.ReadOnlyError();

  [PythonHidden(new PlatformID[] {})]
  void ICollection<byte>.Clear() => throw PythonBuffer.ReadOnlyError();

  [PythonHidden(new PlatformID[] {})]
  bool ICollection<byte>.Contains(byte item) => ((IList<byte>) this).IndexOf(item) != -1;

  [PythonHidden(new PlatformID[] {})]
  void ICollection<byte>.CopyTo(byte[] array, int arrayIndex)
  {
    this.byteCache.CopyTo((Array) array, arrayIndex);
  }

  [PythonHidden(new PlatformID[] {})]
  bool ICollection<byte>.Remove(byte item) => throw PythonBuffer.ReadOnlyError();

  int ICollection<byte>.Count
  {
    [PythonHidden(new PlatformID[] {})] get => this.byteCache.Length;
  }

  bool ICollection<byte>.IsReadOnly
  {
    [PythonHidden(new PlatformID[] {})] get => true;
  }

  private class BufferMeta(Expression expr, BindingRestrictions restrictions, object value) : 
    DynamicMetaObject(expr, restrictions, value),
    IComConvertible
  {
    DynamicMetaObject IComConvertible.GetComMetaObject()
    {
      return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("ConvertBufferToByteArray"), Utils.Convert(this.Expression, typeof (PythonBuffer))), BindingRestrictions.Empty);
    }
  }
}
