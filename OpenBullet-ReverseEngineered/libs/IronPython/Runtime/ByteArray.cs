// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ByteArray
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable disable
namespace IronPython.Runtime;

[PythonType("bytearray")]
public class ByteArray : 
  IList<byte>,
  ICollection<byte>,
  IEnumerable<byte>,
  IEnumerable,
  ICodeFormattable,
  IBufferProtocol
{
  internal List<byte> _bytes;
  public const object __hash__ = null;

  public ByteArray() => this._bytes = new List<byte>(0);

  internal ByteArray(List<byte> bytes) => this._bytes = bytes;

  internal ByteArray(byte[] bytes) => this._bytes = new List<byte>((IEnumerable<byte>) bytes);

  public void __init__() => this._bytes = new List<byte>();

  public void __init__(int source)
  {
    this._bytes = new List<byte>(source);
    for (int index = 0; index < source; ++index)
      this._bytes.Add((byte) 0);
  }

  public void __init__(BigInteger source) => this.__init__((int) source);

  public void __init__([NotNull] IList<byte> source)
  {
    this._bytes = new List<byte>((IEnumerable<byte>) source);
  }

  public void __init__(object source) => this.__init__(ByteArray.GetBytes(source));

  public void __init__(CodeContext context, string source, string encoding, string errors = "strict")
  {
    this._bytes = new List<byte>((IEnumerable<byte>) StringOps.encode(context, source, (object) encoding, errors).MakeByteArray());
  }

  public void append(int item)
  {
    lock (this)
      this._bytes.Add(item.ToByteChecked());
  }

  public void append(object item)
  {
    lock (this)
      this._bytes.Add(ByteArray.GetByte(item));
  }

  public void extend([NotNull] IEnumerable<byte> seq)
  {
    using (new OrderedLocker((object) this, (object) seq))
      this._bytes.AddRange(seq);
  }

  public void extend(object seq) => this.extend((IEnumerable<byte>) ByteArray.GetBytes(seq));

  public void insert(int index, int value)
  {
    lock (this)
    {
      if (index >= this.Count)
      {
        this.append(value);
      }
      else
      {
        index = PythonOps.FixSliceIndex(index, this.Count);
        this._bytes.Insert(index, value.ToByteChecked());
      }
    }
  }

  public void insert(int index, object value)
  {
    this.insert(index, Converter.ConvertToIndex(value));
  }

  public int pop()
  {
    lock (this)
    {
      if (this.Count == 0)
        throw PythonOps.IndexError("pop off of empty bytearray");
      int num = (int) this._bytes[this._bytes.Count - 1];
      this._bytes.RemoveAt(this._bytes.Count - 1);
      return num;
    }
  }

  public int pop(int index)
  {
    lock (this)
    {
      index = this.Count != 0 ? PythonOps.FixIndex(index, this.Count) : throw PythonOps.IndexError("pop off of empty bytearray");
      int num = (int) this._bytes[index];
      this._bytes.RemoveAt(index);
      return num;
    }
  }

  public void remove(int value)
  {
    lock (this)
      this._bytes.RemoveAt(this._bytes.IndexOfByte((int) value.ToByteChecked(), 0, this._bytes.Count));
  }

  public void remove(object value)
  {
    lock (this)
    {
      if (value is ByteArray)
        throw PythonOps.TypeError("an integer or string of size 1 is required");
      this._bytes.RemoveAt(this._bytes.IndexOfByte((int) ByteArray.GetByte(value), 0, this._bytes.Count));
    }
  }

  public void reverse()
  {
    lock (this)
    {
      List<byte> byteList = new List<byte>();
      for (int index = this._bytes.Count - 1; index >= 0; --index)
        byteList.Add(this._bytes[index]);
      this._bytes = byteList;
    }
  }

  [SpecialName]
  public ByteArray InPlaceAdd(ByteArray other)
  {
    using (new OrderedLocker((object) this, (object) other))
    {
      this._bytes.AddRange((IEnumerable<byte>) other._bytes);
      return this;
    }
  }

  [SpecialName]
  public ByteArray InPlaceAdd(Bytes other)
  {
    lock (this)
    {
      this._bytes.AddRange((IEnumerable<byte>) other);
      return this;
    }
  }

  [SpecialName]
  public ByteArray InPlaceAdd(MemoryView other)
  {
    lock (this)
    {
      this._bytes.AddRange((IEnumerable<byte>) other.tobytes());
      return this;
    }
  }

  [SpecialName]
  public ByteArray InPlaceMultiply(int len)
  {
    lock (this)
    {
      this._bytes = (this * len)._bytes;
      return this;
    }
  }

  public ByteArray capitalize()
  {
    lock (this)
      return new ByteArray(this._bytes.Capitalize());
  }

  public ByteArray center(int width) => this.center(width, " ");

  public ByteArray center(int width, [NotNull] string fillchar)
  {
    lock (this)
    {
      List<byte> bytes = this._bytes.TryCenter(width, (int) fillchar.ToByte(nameof (center), 2));
      return bytes == null ? this.CopyThis() : new ByteArray(bytes);
    }
  }

  public ByteArray center(int width, [BytesConversion] IList<byte> fillchar)
  {
    lock (this)
    {
      List<byte> bytes = this._bytes.TryCenter(width, (int) fillchar.ToByte(nameof (center), 2));
      return bytes == null ? this.CopyThis() : new ByteArray(bytes);
    }
  }

  public int count([BytesConversion] IList<byte> sub) => this.count(sub, 0, this._bytes.Count);

  public int count([BytesConversion] IList<byte> sub, int start)
  {
    return this.count(sub, start, this._bytes.Count);
  }

  public int count([BytesConversion] IList<byte> ssub, int start, int end)
  {
    lock (this)
      return this._bytes.CountOf(ssub, start, end);
  }

  public string decode(CodeContext context, [Optional] object encoding, string errors = "strict")
  {
    return StringOps.decode(context, this._bytes.MakeString(), encoding, errors);
  }

  public bool endswith([BytesConversion] IList<byte> suffix)
  {
    lock (this)
      return this._bytes.EndsWith(suffix);
  }

  public bool endswith([BytesConversion] IList<byte> suffix, int start)
  {
    lock (this)
      return this._bytes.EndsWith(suffix, start);
  }

  public bool endswith([BytesConversion] IList<byte> suffix, int start, int end)
  {
    lock (this)
      return this._bytes.EndsWith(suffix, start, end);
  }

  public bool endswith(PythonTuple suffix)
  {
    lock (this)
      return this._bytes.EndsWith(suffix);
  }

  public bool endswith(PythonTuple suffix, int start)
  {
    lock (this)
      return this._bytes.EndsWith(suffix, start);
  }

  public bool endswith(PythonTuple suffix, int start, int end)
  {
    lock (this)
      return this._bytes.EndsWith(suffix, start, end);
  }

  public ByteArray expandtabs() => this.expandtabs(8);

  public ByteArray expandtabs(int tabsize)
  {
    lock (this)
      return new ByteArray(this._bytes.ExpandTabs(tabsize));
  }

  public int find([BytesConversion] IList<byte> sub)
  {
    lock (this)
      return this._bytes.Find(sub);
  }

  public int find([BytesConversion] IList<byte> sub, int start)
  {
    lock (this)
      return this._bytes.Find(sub, new int?(start));
  }

  public int find([BytesConversion] IList<byte> sub, int start, int end)
  {
    lock (this)
      return this._bytes.Find(sub, new int?(start), new int?(end));
  }

  public static ByteArray fromhex(string @string) => new ByteArray(IListOfByteOps.FromHex(@string));

  public int index([BytesConversion] IList<byte> item) => this.index(item, 0, this._bytes.Count);

  public int index([BytesConversion] IList<byte> item, int start)
  {
    return this.index(item, start, this._bytes.Count);
  }

  public int index([BytesConversion] IList<byte> item, int start, int stop)
  {
    lock (this)
    {
      int num = this.find(item, start, stop);
      return num != -1 ? num : throw PythonOps.ValueError("bytearray.index(item): item not in bytearray");
    }
  }

  public bool isalnum()
  {
    lock (this)
      return this._bytes.IsAlphaNumeric();
  }

  public bool isalpha()
  {
    lock (this)
      return this._bytes.IsLetter();
  }

  public bool isdigit()
  {
    lock (this)
      return this._bytes.IsDigit();
  }

  public bool islower()
  {
    lock (this)
      return this._bytes.IsLower();
  }

  public bool isspace()
  {
    lock (this)
      return this._bytes.IsWhiteSpace();
  }

  public bool istitle()
  {
    lock (this)
      return this._bytes.IsTitle();
  }

  public bool isupper()
  {
    lock (this)
      return this._bytes.IsUpper();
  }

  public ByteArray join(object sequence)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(sequence);
    if (!enumerator.MoveNext())
      return new ByteArray();
    object current = enumerator.Current;
    if (!enumerator.MoveNext())
      return ByteArray.JoinOne(current);
    List<byte> byteList = new List<byte>();
    ByteOps.AppendJoin(current, 0, byteList);
    int index = 1;
    do
    {
      byteList.AddRange((IEnumerable<byte>) this);
      ByteOps.AppendJoin(enumerator.Current, index, byteList);
      ++index;
    }
    while (enumerator.MoveNext());
    return new ByteArray(byteList);
  }

  public ByteArray join([NotNull] List sequence)
  {
    if (sequence.__len__() == 0)
      return new ByteArray();
    lock (this)
    {
      if (sequence.__len__() == 1)
        return ByteArray.JoinOne(sequence[0]);
      List<byte> byteList = new List<byte>();
      ByteOps.AppendJoin(sequence._data[0], 0, byteList);
      for (int index = 1; index < sequence._size; ++index)
      {
        byteList.AddRange((IEnumerable<byte>) this);
        ByteOps.AppendJoin(sequence._data[index], index, byteList);
      }
      return new ByteArray(byteList);
    }
  }

  public ByteArray ljust(int width) => this.ljust(width, (byte) 32 /*0x20*/);

  public ByteArray ljust(int width, [NotNull] string fillchar)
  {
    return this.ljust(width, fillchar.ToByte(nameof (ljust), 2));
  }

  public ByteArray ljust(int width, IList<byte> fillchar)
  {
    return this.ljust(width, fillchar.ToByte(nameof (ljust), 2));
  }

  private ByteArray ljust(int width, byte fillchar)
  {
    lock (this)
    {
      int num = width - this._bytes.Count;
      List<byte> bytes = new List<byte>(width);
      bytes.AddRange((IEnumerable<byte>) this._bytes);
      for (int index = 0; index < num; ++index)
        bytes.Add(fillchar);
      return new ByteArray(bytes);
    }
  }

  public ByteArray lower()
  {
    lock (this)
      return new ByteArray(this._bytes.ToLower());
  }

  public ByteArray lstrip()
  {
    lock (this)
    {
      List<byte> bytes = this._bytes.LeftStrip();
      return bytes == null ? this.CopyThis() : new ByteArray(bytes);
    }
  }

  public ByteArray lstrip([BytesConversionNoString] IList<byte> bytes)
  {
    lock (this)
    {
      List<byte> bytes1 = this._bytes.LeftStrip(bytes);
      return bytes1 == null ? this.CopyThis() : new ByteArray(bytes1);
    }
  }

  public PythonTuple partition(IList<byte> sep)
  {
    if (sep == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    if (sep.Count == 0)
      throw PythonOps.ValueError("empty separator");
    object[] o = new object[3]
    {
      (object) new ByteArray(),
      (object) new ByteArray(),
      (object) new ByteArray()
    };
    if (this._bytes.Count != 0)
    {
      int len = this.find(sep);
      if (len == -1)
      {
        o[0] = (object) this.CopyThis();
      }
      else
      {
        o[0] = (object) new ByteArray(this._bytes.Substring(0, len));
        o[1] = (object) sep;
        o[2] = (object) new ByteArray(this._bytes.Substring(len + sep.Count, this._bytes.Count - len - sep.Count));
      }
    }
    return new PythonTuple((object) o);
  }

  public PythonTuple partition([NotNull] List sep)
  {
    return this.partition(ByteArray.GetBytes((object) sep));
  }

  public ByteArray replace([BytesConversion] IList<byte> old, [BytesConversion] IList<byte> @new)
  {
    if (old == null)
      throw PythonOps.TypeError("expected bytes or bytearray, got NoneType");
    return this.replace(old, @new, this._bytes.Count);
  }

  public ByteArray replace([BytesConversion] IList<byte> old, [BytesConversion] IList<byte> @new, int count)
  {
    if (old == null)
      throw PythonOps.TypeError("expected bytes or bytearray, got NoneType");
    return count == 0 ? this.CopyThis() : new ByteArray(this._bytes.Replace(old, @new, count));
  }

  public int rfind([BytesConversion] IList<byte> sub) => this.rfind(sub, 0, this._bytes.Count);

  public int rfind([BytesConversion] IList<byte> sub, int start)
  {
    return this.rfind(sub, start, this._bytes.Count);
  }

  public int rfind([BytesConversion] IList<byte> sub, int start, int end)
  {
    lock (this)
      return this._bytes.ReverseFind(sub, new int?(start), new int?(end));
  }

  public int rindex([BytesConversion] IList<byte> sub) => this.rindex(sub, 0, this._bytes.Count);

  public int rindex([BytesConversion] IList<byte> sub, int start)
  {
    return this.rindex(sub, start, this._bytes.Count);
  }

  public int rindex([BytesConversion] IList<byte> sub, int start, int end)
  {
    int num = this.rfind(sub, start, end);
    return num != -1 ? num : throw PythonOps.ValueError("substring {0} not found in {1}", (object) sub, (object) this);
  }

  public ByteArray rjust(int width) => this.rjust(width, 32 /*0x20*/);

  public ByteArray rjust(int width, [NotNull] string fillchar)
  {
    return this.rjust(width, (int) fillchar.ToByte(nameof (rjust), 2));
  }

  public ByteArray rjust(int width, [BytesConversion] IList<byte> fillchar)
  {
    return this.rjust(width, (int) fillchar.ToByte(nameof (rjust), 2));
  }

  private ByteArray rjust(int width, int fillchar)
  {
    byte byteChecked = fillchar.ToByteChecked();
    lock (this)
    {
      int num = width - this._bytes.Count;
      if (num <= 0)
        return this.CopyThis();
      List<byte> bytes = new List<byte>(width);
      for (int index = 0; index < num; ++index)
        bytes.Add(byteChecked);
      bytes.AddRange((IEnumerable<byte>) this._bytes);
      return new ByteArray(bytes);
    }
  }

  public PythonTuple rpartition(IList<byte> sep)
  {
    if (sep == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    if (sep.Count == 0)
      throw PythonOps.ValueError("empty separator");
    lock (this)
    {
      object[] o = new object[3]
      {
        (object) new ByteArray(),
        (object) new ByteArray(),
        (object) new ByteArray()
      };
      if (this._bytes.Count != 0)
      {
        int len = this.rfind(sep);
        if (len == -1)
        {
          o[2] = (object) this.CopyThis();
        }
        else
        {
          o[0] = (object) new ByteArray(this._bytes.Substring(0, len));
          o[1] = (object) new ByteArray(new List<byte>((IEnumerable<byte>) sep));
          o[2] = (object) new ByteArray(this._bytes.Substring(len + sep.Count, this.Count - len - sep.Count));
        }
      }
      return new PythonTuple((object) o);
    }
  }

  public PythonTuple rpartition([NotNull] List sep)
  {
    return this.rpartition(ByteArray.GetBytes((object) sep));
  }

  public List rsplit()
  {
    lock (this)
      return this._bytes.SplitInternal((IList<byte>) null, -1, (Func<List<byte>, object>) (x => (object) new ByteArray(x)));
  }

  public List rsplit([BytesConversionNoString] IList<byte> sep) => this.rsplit(sep, -1);

  public List rsplit([BytesConversionNoString] IList<byte> sep, int maxsplit)
  {
    return this._bytes.RightSplit(sep, maxsplit, (Func<IList<byte>, IList<byte>>) (x => (IList<byte>) new ByteArray(new List<byte>((IEnumerable<byte>) x))));
  }

  public ByteArray rstrip()
  {
    lock (this)
    {
      List<byte> bytes = this._bytes.RightStrip();
      return bytes == null ? this.CopyThis() : new ByteArray(bytes);
    }
  }

  public ByteArray rstrip([BytesConversionNoString] IList<byte> bytes)
  {
    lock (this)
    {
      List<byte> bytes1 = this._bytes.RightStrip(bytes);
      return bytes1 == null ? this.CopyThis() : new ByteArray(bytes1);
    }
  }

  public List split()
  {
    lock (this)
      return this._bytes.SplitInternal((IList<byte>) null, -1, (Func<List<byte>, object>) (x => (object) new ByteArray(x)));
  }

  public List split([BytesConversionNoString] IList<byte> sep) => this.split(sep, -1);

  public List split([BytesConversionNoString] IList<byte> sep, int maxsplit)
  {
    lock (this)
      return this._bytes.Split(sep, maxsplit, (Func<List<byte>, object>) (x => (object) new ByteArray(x)));
  }

  public List splitlines() => this.splitlines(false);

  public List splitlines(bool keepends)
  {
    lock (this)
      return this._bytes.SplitLines(keepends, (Func<List<byte>, object>) (x => (object) new ByteArray(x)));
  }

  public bool startswith([BytesConversion] IList<byte> prefix)
  {
    lock (this)
      return this._bytes.StartsWith(prefix);
  }

  public bool startswith([BytesConversion] IList<byte> prefix, int start)
  {
    lock (this)
    {
      int count = this.Count;
      if (start > count)
        return false;
      if (start < 0)
      {
        start += count;
        if (start < 0)
          start = 0;
      }
      return this._bytes.Substring(start).StartsWith(prefix);
    }
  }

  public bool startswith([BytesConversion] IList<byte> prefix, int start, int end)
  {
    lock (this)
      return this._bytes.StartsWith(prefix, start, end);
  }

  public bool startswith(PythonTuple prefix)
  {
    lock (this)
      return this._bytes.StartsWith(prefix);
  }

  public bool startswith(PythonTuple prefix, int start)
  {
    lock (this)
      return this._bytes.StartsWith(prefix, start);
  }

  public bool startswith(PythonTuple prefix, int start, int end)
  {
    lock (this)
      return this._bytes.StartsWith(prefix, start, end);
  }

  public ByteArray strip()
  {
    lock (this)
    {
      List<byte> bytes = this._bytes.Strip();
      return bytes == null ? this.CopyThis() : new ByteArray(bytes);
    }
  }

  public ByteArray strip([BytesConversionNoString] IList<byte> chars)
  {
    lock (this)
    {
      List<byte> bytes = this._bytes.Strip(chars);
      return bytes == null ? this.CopyThis() : new ByteArray(bytes);
    }
  }

  public ByteArray swapcase()
  {
    lock (this)
      return new ByteArray(this._bytes.SwapCase());
  }

  public ByteArray title()
  {
    lock (this)
    {
      List<byte> bytes = this._bytes.Title();
      return bytes == null ? this.CopyThis() : new ByteArray(bytes);
    }
  }

  public ByteArray translate([BytesConversion] IList<byte> table)
  {
    lock (this)
    {
      if (table != null)
      {
        if (table.Count != 256 /*0x0100*/)
          throw PythonOps.ValueError("translation table must be 256 characters long");
        if (this.Count == 0)
          return this.CopyThis();
      }
      return new ByteArray(this._bytes.Translate(table, (IList<byte>) null));
    }
  }

  public ByteArray translate([BytesConversion] IList<byte> table, [BytesConversion] IList<byte> deletechars)
  {
    if (table == null && deletechars == null)
      throw PythonOps.TypeError("expected bytearray or bytes, got NoneType");
    if (deletechars == null)
      throw PythonOps.TypeError("expected bytes or bytearray, got None");
    lock (this)
      return new ByteArray(this._bytes.Translate(table, deletechars));
  }

  public ByteArray upper()
  {
    lock (this)
      return new ByteArray(this._bytes.ToUpper());
  }

  public ByteArray zfill(int width)
  {
    lock (this)
    {
      int spaces = width - this.Count;
      return spaces <= 0 ? this.CopyThis() : new ByteArray(this._bytes.ZeroFill(width, spaces));
    }
  }

  public int __alloc__() => this._bytes.Count == 0 ? 0 : this._bytes.Count + 1;

  public bool __contains__([BytesConversionNoString] IList<byte> bytes)
  {
    return this.IndexOf(bytes, 0) != -1;
  }

  public bool __contains__(int value) => this.IndexOf(value.ToByteChecked()) != -1;

  public bool __contains__(CodeContext context, object value)
  {
    switch (value)
    {
      case Extensible<int> _:
        return this.IndexOf(((Extensible<int>) value).Value.ToByteChecked()) != -1;
      case BigInteger bigInteger:
        return this.IndexOf(bigInteger.ToByteChecked()) != -1;
      case Extensible<BigInteger> _:
        return this.IndexOf(((Extensible<BigInteger>) value).Value.ToByteChecked()) != -1;
      default:
        throw PythonOps.TypeError("Type {0} doesn't support the buffer API", context.LanguageContext.PythonOptions.Python30 ? (object) PythonTypeOps.GetOldName(value) : (object) PythonTypeOps.GetName(value));
    }
  }

  public PythonTuple __reduce__(CodeContext context)
  {
    return PythonTuple.MakeTuple((object) DynamicHelpers.GetPythonType((object) this), (object) PythonTuple.MakeTuple((object) this.MakeString(), (object) "latin-1"), this.GetType() == typeof (ByteArray) ? (object) null : ObjectOps.ReduceProtocol0(context, (object) this)[2]);
  }

  public virtual string __repr__(CodeContext context)
  {
    lock (this)
      return $"bytearray({this._bytes.BytesRepr()})";
  }

  public static ByteArray operator +(ByteArray self, ByteArray other)
  {
    if (self == null)
      throw PythonOps.TypeError("expected ByteArray, got None");
    List<byte> bytes;
    lock (self)
      bytes = new List<byte>((IEnumerable<byte>) self._bytes);
    lock (other)
      bytes.AddRange((IEnumerable<byte>) other._bytes);
    return new ByteArray(bytes);
  }

  public static ByteArray operator +(ByteArray self, Bytes other)
  {
    List<byte> bytes;
    lock (self)
      bytes = new List<byte>((IEnumerable<byte>) self._bytes);
    bytes.AddRange((IEnumerable<byte>) other);
    return new ByteArray(bytes);
  }

  public static ByteArray operator +(ByteArray self, MemoryView other)
  {
    List<byte> bytes;
    lock (self)
      bytes = new List<byte>((IEnumerable<byte>) self._bytes);
    bytes.AddRange((IEnumerable<byte>) other.tobytes());
    return new ByteArray(bytes);
  }

  public static ByteArray operator +(ByteArray self, string other)
  {
    List<byte> bytes1;
    lock (self)
      bytes1 = new List<byte>((IEnumerable<byte>) self._bytes);
    byte[] bytes2;
    if (!other.TryMakeByteArray(out bytes2))
      throw PythonOps.TypeError("can't concat unicode to bytearray");
    bytes1.AddRange((IEnumerable<byte>) bytes2);
    return new ByteArray(bytes1);
  }

  public static ByteArray operator *(ByteArray x, int y)
  {
    lock (x)
      return y == 1 ? x.CopyThis() : new ByteArray(x._bytes.Multiply(y));
  }

  public static ByteArray operator *(int x, ByteArray y) => y * x;

  public static bool operator >(ByteArray x, ByteArray y)
  {
    if (y == null)
      return true;
    using (new OrderedLocker((object) x, (object) y))
      return x._bytes.Compare((IList<byte>) y._bytes) > 0;
  }

  public static bool operator <(ByteArray x, ByteArray y)
  {
    if (y == null)
      return false;
    using (new OrderedLocker((object) x, (object) y))
      return x._bytes.Compare((IList<byte>) y._bytes) < 0;
  }

  public static bool operator >=(ByteArray x, ByteArray y)
  {
    if (y == null)
      return true;
    using (new OrderedLocker((object) x, (object) y))
      return x._bytes.Compare((IList<byte>) y._bytes) >= 0;
  }

  public static bool operator <=(ByteArray x, ByteArray y)
  {
    if (y == null)
      return false;
    using (new OrderedLocker((object) x, (object) y))
      return x._bytes.Compare((IList<byte>) y._bytes) <= 0;
  }

  public static bool operator >(ByteArray x, Bytes y)
  {
    if (y == null)
      return true;
    lock (x)
      return x._bytes.Compare((IList<byte>) y) > 0;
  }

  public static bool operator <(ByteArray x, Bytes y)
  {
    if (y == null)
      return false;
    lock (x)
      return x._bytes.Compare((IList<byte>) y) < 0;
  }

  public static bool operator >=(ByteArray x, Bytes y)
  {
    if (y == null)
      return true;
    lock (x)
      return x._bytes.Compare((IList<byte>) y) >= 0;
  }

  public static bool operator <=(ByteArray x, Bytes y)
  {
    if (y == null)
      return false;
    lock (x)
      return x._bytes.Compare((IList<byte>) y) <= 0;
  }

  public object this[int index]
  {
    get
    {
      lock (this)
        return ScriptingRuntimeHelpers.Int32ToObject((int) this._bytes[PythonOps.FixIndex(index, this._bytes.Count)]);
    }
    set
    {
      lock (this)
        this._bytes[PythonOps.FixIndex(index, this._bytes.Count)] = ByteArray.GetByte(value);
    }
  }

  public object this[BigInteger index]
  {
    get
    {
      int ret;
      if (index.AsInt32(out ret))
        return this[ret];
      throw PythonOps.IndexError("cannot fit long in index");
    }
    set
    {
      int ret;
      if (!index.AsInt32(out ret))
        throw PythonOps.IndexError("cannot fit long in index");
      this[ret] = value;
    }
  }

  public object this[Slice slice]
  {
    get
    {
      lock (this)
      {
        List<byte> bytes = this._bytes.Slice(slice);
        return bytes == null ? (object) new ByteArray() : (object) new ByteArray(bytes);
      }
    }
    set
    {
      if (slice == null)
        throw PythonOps.TypeError("bytearray indices must be integer or slice, not None");
      if (!(value is IList<byte> byteList1))
      {
        int? nullable1 = new int?();
        if (value is int num1)
          nullable1 = new int?(num1);
        else if (value is Extensible<int>)
        {
          nullable1 = new int?(((Extensible<int>) value).Value);
        }
        else
        {
          int ret;
          if (value is BigInteger self && self.AsInt32(out ret))
            nullable1 = new int?(ret);
        }
        if (nullable1.HasValue)
        {
          List<byte> byteList = new List<byte>();
          byteList.Capacity = nullable1.Value;
          int num2 = 0;
          while (true)
          {
            int num3 = num2;
            int? nullable2 = nullable1;
            int valueOrDefault = nullable2.GetValueOrDefault();
            if (num3 < valueOrDefault & nullable2.HasValue)
            {
              byteList.Add((byte) 0);
              ++num2;
            }
            else
              break;
          }
          byteList1 = (IList<byte>) byteList;
        }
        else
        {
          IEnumerator enumerator = PythonOps.GetEnumerator(value);
          byteList1 = (IList<byte>) new List<byte>();
          while (enumerator.MoveNext())
            byteList1.Add(ByteArray.GetByte(enumerator.Current));
        }
      }
      lock (this)
      {
        if (slice.step != null)
        {
          if (this == byteList1)
            value = (object) this.CopyThis();
          else if (byteList1.Count == 0)
          {
            this.DeleteItem(slice);
            return;
          }
          IList<byte> bytes = ByteArray.GetBytes(value);
          int ostart;
          int ostop;
          int ostep;
          slice.indices(this._bytes.Count, out ostart, out ostop, out ostep);
          int num = (ostep > 0 ? ostop - ostart + ostep - 1 : ostop - ostart + ostep + 1) / ostep;
          if (byteList1.Count < num)
            throw PythonOps.ValueError("too few items in the enumerator. need {0} have {1}", (object) num, (object) bytes.Count);
          int index1 = 0;
          int index2 = ostart;
          while (index1 < bytes.Count)
          {
            if (index1 >= num)
            {
              if (index2 == this._bytes.Count)
                this._bytes.Add(bytes[index1]);
              else
                this._bytes.Insert(index2, bytes[index1]);
            }
            else
              this._bytes[index2] = bytes[index1];
            ++index1;
            index2 += ostep;
          }
        }
        else
        {
          int ostart;
          int ostop;
          slice.indices(this._bytes.Count, out ostart, out ostop, out int _);
          this.SliceNoStep(ostart, ostop, byteList1);
        }
      }
    }
  }

  [SpecialName]
  public void DeleteItem(int index)
  {
    this._bytes.RemoveAt(PythonOps.FixIndex(index, this._bytes.Count));
  }

  [SpecialName]
  public void DeleteItem(Slice slice)
  {
    if (slice == null)
      throw PythonOps.TypeError("list indices must be integers or slices");
    lock (this)
    {
      int ostart;
      int ostop;
      int ostep;
      slice.indices(this._bytes.Count, out ostart, out ostop, out ostep);
      if (ostep > 0 && ostart >= ostop || ostep < 0 && ostart <= ostop)
        return;
      switch (ostep)
      {
        case -1:
          int index1 = ostop + 1;
          int index2 = ostart + 1;
          while (index2 < this._bytes.Count)
          {
            this._bytes[index1] = this._bytes[index2];
            ++index2;
            ++index1;
          }
          this._bytes.RemoveRange(index1, ostart - ostop);
          break;
        case 1:
          int index3 = ostart;
          int index4 = ostop;
          while (index4 < this._bytes.Count)
          {
            this._bytes[index3] = this._bytes[index4];
            ++index4;
            ++index3;
          }
          this._bytes.RemoveRange(index3, ostop - ostart);
          break;
        default:
          if (ostep < 0)
          {
            int num1 = ostart;
            while (num1 > ostop)
              num1 += ostep;
            int num2 = num1 - ostep;
            ostop = ostart + 1;
            ostart = num2;
            ostep = -ostep;
          }
          int num3;
          int index5 = num3 = ostart;
          int num4 = num3;
          int index6;
          for (index6 = num3; index6 < ostop && index5 < ostop; ++index5)
          {
            if (index5 != num4)
              this._bytes[index6++] = this._bytes[index5];
            else
              num4 += ostep;
          }
          while (ostop < this._bytes.Count)
            this._bytes[index6++] = this._bytes[ostop++];
          this._bytes.RemoveRange(index6, this._bytes.Count - index6);
          break;
      }
    }
  }

  private static ByteArray JoinOne(object curVal)
  {
    return curVal is IList<byte> ? new ByteArray(new List<byte>((IEnumerable<byte>) (curVal as IList<byte>))) : throw PythonOps.TypeError("can only join an iterable of bytes");
  }

  private ByteArray CopyThis() => new ByteArray(new List<byte>((IEnumerable<byte>) this._bytes));

  private void SliceNoStep(int start, int stop, IList<byte> value)
  {
    IList<byte> bytes = ByteArray.GetBytes((object) value);
    lock (this)
    {
      if (start > stop)
      {
        List<byte> byteList = new List<byte>(this.Count + bytes.Count);
        int index1;
        for (index1 = 0; index1 < start; ++index1)
          byteList.Add(this._bytes[index1]);
        for (int index2 = 0; index2 < bytes.Count; ++index2)
          byteList.Add(bytes[index2]);
        for (; index1 < this.Count; ++index1)
          byteList.Add(this._bytes[index1]);
        this._bytes = byteList;
      }
      else if (stop - start == bytes.Count)
      {
        for (int index = 0; index < bytes.Count; ++index)
          this._bytes[index + start] = bytes[index];
      }
      else
      {
        List<byte> byteList = new List<byte>(this.Count - (stop - start) + bytes.Count);
        for (int index = 0; index < start; ++index)
          byteList.Add(this._bytes[index]);
        for (int index = 0; index < bytes.Count; ++index)
          byteList.Add(bytes[index]);
        for (int index = stop; index < this.Count; ++index)
          byteList.Add(this._bytes[index]);
        this._bytes = byteList;
      }
    }
  }

  private static byte GetByte(object value)
  {
    switch (value)
    {
      case double _:
      case Extensible<double> _:
      case float _:
        throw PythonOps.TypeError("an integer or string of size 1 is required");
      default:
        return ByteOps.GetByteListOk(value);
    }
  }

  private static IList<byte> GetBytes(object value)
  {
    if (!(value is ListGenericWrapper<byte>) && value is IList<byte>)
      return (IList<byte>) value;
    switch (value)
    {
      case string s:
        return (IList<byte>) s.MakeByteArray();
      case Extensible<string> extensible:
        return (IList<byte>) extensible.Value.MakeByteArray();
      default:
        List<byte> bytes = new List<byte>();
        IEnumerator enumerator = PythonOps.GetEnumerator(value);
        while (enumerator.MoveNext())
          bytes.Add(ByteArray.GetByte(enumerator.Current));
        return (IList<byte>) bytes;
    }
  }

  [PythonHidden(new PlatformID[] {})]
  public int IndexOf(byte item)
  {
    lock (this)
      return this._bytes.IndexOf(item);
  }

  [PythonHidden(new PlatformID[] {})]
  public void Insert(int index, byte item) => this._bytes.Insert(index, item);

  [PythonHidden(new PlatformID[] {})]
  public void RemoveAt(int index) => this._bytes.RemoveAt(index);

  byte IList<byte>.this[int index]
  {
    get => this._bytes[index];
    set => this._bytes[index] = value;
  }

  [PythonHidden(new PlatformID[] {})]
  public void Add(byte item)
  {
    lock (this)
      this._bytes.Add(item);
  }

  [PythonHidden(new PlatformID[] {})]
  public void Clear()
  {
    lock (this)
      this._bytes.Clear();
  }

  [PythonHidden(new PlatformID[] {})]
  public bool Contains(byte item)
  {
    lock (this)
      return this._bytes.Contains(item);
  }

  [PythonHidden(new PlatformID[] {})]
  public void CopyTo(byte[] array, int arrayIndex)
  {
    lock (this)
      this._bytes.CopyTo(array, arrayIndex);
  }

  public int Count
  {
    [PythonHidden(new PlatformID[] {})] get
    {
      lock (this)
        return this._bytes.Count;
    }
  }

  public bool IsReadOnly
  {
    [PythonHidden(new PlatformID[] {})] get => false;
  }

  [PythonHidden(new PlatformID[] {})]
  public bool Remove(byte item)
  {
    lock (this)
      return this._bytes.Remove(item);
  }

  public IEnumerator __iter__() => PythonOps.BytesIntEnumerator((IList<byte>) this).Key;

  [PythonHidden(new PlatformID[] {})]
  public IEnumerator<byte> GetEnumerator() => (IEnumerator<byte>) this._bytes.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator()
  {
    foreach (int num in this._bytes)
      yield return (object) num;
  }

  public override int GetHashCode()
  {
    return PythonTuple.MakeTuple((object) this._bytes.ToArray()).GetHashCode();
  }

  public override bool Equals(object other)
  {
    IList<byte> byteList;
    switch (other)
    {
      case string _:
        byteList = (IList<byte>) PythonOps.MakeBytes(((string) other).MakeByteArray());
        break;
      case Extensible<string> _:
        byteList = (IList<byte>) PythonOps.MakeBytes(((Extensible<string>) other).Value.MakeByteArray());
        break;
      default:
        byteList = other as IList<byte>;
        break;
    }
    if (byteList == null || this.Count != byteList.Count)
      return false;
    if (this.Count == 0)
      return true;
    using (new OrderedLocker((object) this, other))
    {
      for (int index = 0; index < this.Count; ++index)
      {
        if ((int) this._bytes[index] != (int) byteList[index])
          return false;
      }
    }
    return true;
  }

  public override string ToString() => this._bytes.MakeString();

  Bytes IBufferProtocol.GetItem(int index)
  {
    lock (this)
      return new Bytes((IList<byte>) new byte[1]
      {
        this._bytes[PythonOps.FixIndex(index, this._bytes.Count)]
      });
  }

  void IBufferProtocol.SetItem(int index, object value) => this[index] = value;

  void IBufferProtocol.SetSlice(Slice index, object value) => this[index] = value;

  int IBufferProtocol.ItemCount => this._bytes.Count;

  string IBufferProtocol.Format => "B";

  BigInteger IBufferProtocol.ItemSize => (BigInteger) 1;

  BigInteger IBufferProtocol.NumberDimensions => (BigInteger) 1;

  bool IBufferProtocol.ReadOnly => false;

  IList<BigInteger> IBufferProtocol.GetShape(int start, int? end)
  {
    return end.HasValue ? (IList<BigInteger>) new BigInteger[1]
    {
      (BigInteger) end.Value - (BigInteger) start
    } : (IList<BigInteger>) new BigInteger[1]
    {
      (BigInteger) this._bytes.Count - (BigInteger) start
    };
  }

  PythonTuple IBufferProtocol.Strides => PythonTuple.MakeTuple((object) 1);

  object IBufferProtocol.SubOffsets => (object) null;

  Bytes IBufferProtocol.ToBytes(int start, int? end)
  {
    return start == 0 && !end.HasValue ? new Bytes((IList<byte>) this) : new Bytes((IList<byte>) this[new Slice((object) start, (object) end)]);
  }

  List IBufferProtocol.ToList(int start, int? end)
  {
    List<byte> byteList = this._bytes.Slice(new Slice((object) start, (object) end));
    return byteList == null ? new List() : new List((ICollection) byteList.ToArray());
  }
}
