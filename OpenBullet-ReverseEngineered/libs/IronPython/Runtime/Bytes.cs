// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Bytes
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
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.InteropServices;

#nullable disable
namespace IronPython.Runtime;

[PythonType("bytes")]
public class Bytes : 
  IList<byte>,
  ICollection<byte>,
  IEnumerable<byte>,
  IEnumerable,
  ICodeFormattable,
  IExpressionSerializable,
  IBufferProtocol
{
  internal byte[] _bytes;
  internal static Bytes Empty = new Bytes();

  public Bytes() => this._bytes = new byte[0];

  public Bytes([BytesConversion, NotNull] IList<byte> bytes)
  {
    this._bytes = ArrayUtils.ToArray<byte>((ICollection<byte>) bytes);
  }

  public Bytes([NotNull] List bytes)
  {
    this._bytes = ByteOps.GetBytes((ICollection) bytes, new Func<object, byte>(ByteOps.GetByteListOk)).ToArray();
  }

  public Bytes(int size) => this._bytes = new byte[size];

  private Bytes(byte[] bytes) => this._bytes = bytes;

  public Bytes(CodeContext context, [NotNull] string unicode, [NotNull] string encoding)
  {
    this._bytes = StringOps.encode(context, unicode, (object) encoding, "strict").MakeByteArray();
  }

  internal static Bytes Make(byte[] bytes) => new Bytes(bytes);

  public Bytes capitalize()
  {
    return this.Count == 0 ? this : new Bytes((IList<byte>) ((IList<byte>) this._bytes).Capitalize());
  }

  public Bytes center(int width) => this.center(width, " ");

  public Bytes center(int width, [NotNull] string fillchar)
  {
    List<byte> bytes = ((IList<byte>) this._bytes).TryCenter(width, (int) fillchar.ToByte(nameof (center), 2));
    return bytes == null ? this : new Bytes((IList<byte>) bytes);
  }

  public Bytes center(int width, [BytesConversion] IList<byte> fillchar)
  {
    List<byte> bytes = ((IList<byte>) this._bytes).TryCenter(width, (int) fillchar.ToByte(nameof (center), 2));
    return bytes == null ? this : new Bytes((IList<byte>) bytes);
  }

  public ByteArray center(int width, List fillchar)
  {
    throw PythonOps.TypeError("center() argument 2 must be byte, not list");
  }

  public int count([BytesConversion] IList<byte> sub) => this.count(sub, 0, this.Count);

  public int count([BytesConversion] IList<byte> sub, int start)
  {
    return this.count(sub, start, this.Count);
  }

  public int count([BytesConversion] IList<byte> ssub, int start, int end)
  {
    return ((IList<byte>) this._bytes).CountOf(ssub, start, end);
  }

  public int count(List sub) => throw PythonOps.TypeError("expected bytes or bytearray, got list");

  public int count(List sub, int start)
  {
    throw PythonOps.TypeError("expected bytes or bytearray, got list");
  }

  public int count(List ssub, int start, int end)
  {
    throw PythonOps.TypeError("expected bytes or bytearray, got list");
  }

  public string decode(CodeContext context, [Optional] object encoding, [NotNull] string errors = "strict")
  {
    return StringOps.decode(context, ((IList<byte>) this._bytes).MakeString(), encoding, errors);
  }

  public bool endswith([BytesConversion] IList<byte> suffix)
  {
    return ((IList<byte>) this._bytes).EndsWith(suffix);
  }

  public bool endswith([BytesConversion] IList<byte> suffix, int start)
  {
    return ((IList<byte>) this._bytes).EndsWith(suffix, start);
  }

  public bool endswith([BytesConversion] IList<byte> suffix, int start, int end)
  {
    return ((IList<byte>) this._bytes).EndsWith(suffix, start, end);
  }

  public bool endswith(List suffix)
  {
    throw PythonOps.TypeError("expected bytes or bytearray, got list");
  }

  public bool endswith(List suffix, int start)
  {
    throw PythonOps.TypeError("expected bytes or bytearray, got list");
  }

  public bool endswith(List suffix, int start, int end)
  {
    throw PythonOps.TypeError("expected bytes or bytearray, got list");
  }

  public bool endswith(PythonTuple suffix) => ((IList<byte>) this._bytes).EndsWith(suffix);

  public bool endswith(PythonTuple suffix, int start)
  {
    return ((IList<byte>) this._bytes).EndsWith(suffix, start);
  }

  public bool endswith(PythonTuple suffix, int start, int end)
  {
    return ((IList<byte>) this._bytes).EndsWith(suffix, start, end);
  }

  public Bytes expandtabs() => this.expandtabs(8);

  public Bytes expandtabs(int tabsize)
  {
    return new Bytes((IList<byte>) ((IList<byte>) this._bytes).ExpandTabs(tabsize));
  }

  public int find([BytesConversion] IList<byte> sub) => ((IList<byte>) this._bytes).Find(sub);

  public int find([BytesConversion] IList<byte> sub, int? start)
  {
    return ((IList<byte>) this._bytes).Find(sub, start);
  }

  public int find([BytesConversion] IList<byte> sub, int? start, int? end)
  {
    return ((IList<byte>) this._bytes).Find(sub, start, end);
  }

  public static Bytes fromhex(string @string)
  {
    return new Bytes(IListOfByteOps.FromHex(@string).ToArray());
  }

  public int index([BytesConversion] IList<byte> item)
  {
    return this.index(item, new int?(0), new int?(this.Count));
  }

  public int index([BytesConversion] IList<byte> item, int? start)
  {
    return this.index(item, start, new int?(this.Count));
  }

  public int index([BytesConversion] IList<byte> item, int? start, int? stop)
  {
    int num = this.find(item, start, stop);
    return num != -1 ? num : throw PythonOps.ValueError("bytes.index(item): item not in bytes");
  }

  public bool isalnum() => ((IList<byte>) this._bytes).IsAlphaNumeric();

  public bool isalpha() => ((IList<byte>) this._bytes).IsLetter();

  public bool isdigit() => ((IList<byte>) this._bytes).IsDigit();

  public bool islower() => ((IList<byte>) this._bytes).IsLower();

  public bool isspace() => ((IList<byte>) this._bytes).IsWhiteSpace();

  public bool istitle() => ((IList<byte>) this._bytes).IsTitle();

  public bool isupper() => ((IList<byte>) this._bytes).IsUpper();

  public Bytes join(object sequence)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(sequence);
    if (!enumerator.MoveNext())
      return Bytes.Empty;
    object current = enumerator.Current;
    if (!enumerator.MoveNext())
      return Bytes.JoinOne(current);
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
    return new Bytes((IList<byte>) byteList);
  }

  public Bytes join([NotNull] List sequence)
  {
    if (sequence.__len__() == 0)
      return new Bytes();
    if (sequence.__len__() == 1)
      return Bytes.JoinOne(sequence[0]);
    List<byte> byteList = new List<byte>();
    ByteOps.AppendJoin(sequence._data[0], 0, byteList);
    for (int index = 1; index < sequence._size; ++index)
    {
      byteList.AddRange((IEnumerable<byte>) this);
      ByteOps.AppendJoin(sequence._data[index], index, byteList);
    }
    return new Bytes((IList<byte>) byteList);
  }

  public Bytes ljust(int width) => this.ljust(width, (byte) 32 /*0x20*/);

  public Bytes ljust(int width, [NotNull] string fillchar)
  {
    return this.ljust(width, fillchar.ToByte(nameof (ljust), 2));
  }

  public Bytes ljust(int width, [BytesConversion] IList<byte> fillchar)
  {
    return this.ljust(width, fillchar.ToByte(nameof (ljust), 2));
  }

  private Bytes ljust(int width, byte fillchar)
  {
    int num = width - this.Count;
    if (num <= 0)
      return this;
    List<byte> bytes = new List<byte>(width);
    bytes.AddRange((IEnumerable<byte>) this._bytes);
    for (int index = 0; index < num; ++index)
      bytes.Add(fillchar);
    return new Bytes((IList<byte>) bytes);
  }

  public Bytes lower() => new Bytes((IList<byte>) ((IList<byte>) this._bytes).ToLower());

  public Bytes lstrip()
  {
    List<byte> bytes = ((IList<byte>) this._bytes).LeftStrip();
    return bytes == null ? this : new Bytes((IList<byte>) bytes);
  }

  public Bytes lstrip([BytesConversion] IList<byte> bytes)
  {
    lock (this)
    {
      List<byte> bytes1 = ((IList<byte>) this._bytes).LeftStrip(bytes);
      return bytes1 == null ? this : new Bytes((IList<byte>) bytes1);
    }
  }

  public PythonTuple partition([BytesConversion] IList<byte> sep)
  {
    if (sep == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    if (sep.Count == 0)
      throw PythonOps.ValueError("empty separator");
    object[] o = new object[3]
    {
      (object) Bytes.Empty,
      (object) Bytes.Empty,
      (object) Bytes.Empty
    };
    if (this.Count != 0)
    {
      int len = this.find(sep);
      if (len == -1)
      {
        o[0] = (object) this;
      }
      else
      {
        o[0] = (object) new Bytes((IList<byte>) ((IList<byte>) this._bytes).Substring(0, len));
        o[1] = (object) sep;
        o[2] = (object) new Bytes((IList<byte>) ((IList<byte>) this._bytes).Substring(len + sep.Count, this.Count - len - sep.Count));
      }
    }
    return new PythonTuple((object) o);
  }

  public Bytes replace([BytesConversion] IList<byte> old, [BytesConversion] IList<byte> @new)
  {
    if (old == null)
      throw PythonOps.TypeError("expected bytes or bytearray, got NoneType");
    return this.replace(old, @new, this._bytes.Length);
  }

  public Bytes replace([BytesConversion] IList<byte> old, [BytesConversion] IList<byte> @new, int count)
  {
    if (old == null)
      throw PythonOps.TypeError("expected bytes or bytearray, got NoneType");
    return count == 0 ? this : new Bytes((IList<byte>) ((IList<byte>) this._bytes).Replace(old, @new, count));
  }

  public int rfind([BytesConversion] IList<byte> sub)
  {
    return this.rfind(sub, new int?(0), new int?(this.Count));
  }

  public int rfind([BytesConversion] IList<byte> sub, int? start)
  {
    return this.rfind(sub, start, new int?(this.Count));
  }

  public int rfind([BytesConversion] IList<byte> sub, int? start, int? end)
  {
    return ((IList<byte>) this._bytes).ReverseFind(sub, start, end);
  }

  public int rindex([BytesConversion] IList<byte> sub)
  {
    return this.rindex(sub, new int?(0), new int?(this.Count));
  }

  public int rindex([BytesConversion] IList<byte> sub, int? start)
  {
    return this.rindex(sub, start, new int?(this.Count));
  }

  public int rindex([BytesConversion] IList<byte> sub, int? start, int? end)
  {
    int num = this.rfind(sub, start, end);
    return num != -1 ? num : throw PythonOps.ValueError("substring {0} not found in {1}", (object) sub, (object) this);
  }

  public Bytes rjust(int width) => this.rjust(width, (byte) 32 /*0x20*/);

  public Bytes rjust(int width, [NotNull] string fillchar)
  {
    return this.rjust(width, fillchar.ToByte(nameof (rjust), 2));
  }

  public Bytes rjust(int width, [BytesConversion] IList<byte> fillchar)
  {
    return this.rjust(width, fillchar.ToByte(nameof (rjust), 2));
  }

  private Bytes rjust(int width, byte fillchar)
  {
    int num = width - this.Count;
    if (num <= 0)
      return this;
    List<byte> bytes = new List<byte>(width);
    for (int index = 0; index < num; ++index)
      bytes.Add(fillchar);
    bytes.AddRange((IEnumerable<byte>) this._bytes);
    return new Bytes((IList<byte>) bytes);
  }

  public PythonTuple rpartition([BytesConversion] IList<byte> sep)
  {
    if (sep == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    if (sep.Count == 0)
      throw PythonOps.ValueError("empty separator");
    object[] o = new object[3]
    {
      (object) Bytes.Empty,
      (object) Bytes.Empty,
      (object) Bytes.Empty
    };
    if (this.Count != 0)
    {
      int len = this.rfind(sep);
      if (len == -1)
      {
        o[2] = (object) this;
      }
      else
      {
        o[0] = (object) new Bytes((IList<byte>) ((IList<byte>) this._bytes).Substring(0, len));
        o[1] = (object) sep;
        o[2] = (object) new Bytes((IList<byte>) ((IList<byte>) this._bytes).Substring(len + sep.Count, this.Count - len - sep.Count));
      }
    }
    return new PythonTuple((object) o);
  }

  public List rsplit()
  {
    return ((IList<byte>) this._bytes).SplitInternal((IList<byte>) null, -1, (Func<List<byte>, object>) (x => (object) new Bytes((IList<byte>) x)));
  }

  public List rsplit([BytesConversion] IList<byte> sep) => this.rsplit(sep, -1);

  public List rsplit([BytesConversion] IList<byte> sep, int maxsplit)
  {
    return ((IList<byte>) this._bytes).RightSplit(sep, maxsplit, (Func<IList<byte>, IList<byte>>) (x => (IList<byte>) new Bytes((IList<byte>) new List<byte>((IEnumerable<byte>) x))));
  }

  public Bytes rstrip()
  {
    List<byte> bytes = ((IList<byte>) this._bytes).RightStrip();
    return bytes == null ? this : new Bytes((IList<byte>) bytes);
  }

  public Bytes rstrip([BytesConversion] IList<byte> bytes)
  {
    lock (this)
    {
      List<byte> bytes1 = ((IList<byte>) this._bytes).RightStrip(bytes);
      return bytes1 == null ? this : new Bytes((IList<byte>) bytes1);
    }
  }

  public List split()
  {
    return ((IList<byte>) this._bytes).SplitInternal((IList<byte>) null, -1, (Func<List<byte>, object>) (x => (object) new Bytes((IList<byte>) x)));
  }

  public List split([BytesConversion] IList<byte> sep) => this.split(sep, -1);

  public List split([BytesConversion] IList<byte> sep, int maxsplit)
  {
    return ((IList<byte>) this._bytes).Split(sep, maxsplit, (Func<List<byte>, object>) (x => (object) new Bytes((IList<byte>) x)));
  }

  public List splitlines() => this.splitlines(false);

  public List splitlines(bool keepends)
  {
    return ((IList<byte>) this._bytes).SplitLines(keepends, (Func<List<byte>, object>) (x => (object) new Bytes((IList<byte>) x)));
  }

  public bool startswith([BytesConversion] IList<byte> prefix)
  {
    return ((IList<byte>) this._bytes).StartsWith(prefix);
  }

  public bool startswith([BytesConversion] IList<byte> prefix, int start)
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
    return ((IList<byte>) this._bytes).Substring(start).StartsWith(prefix);
  }

  public bool startswith([BytesConversion] IList<byte> prefix, int start, int end)
  {
    return ((IList<byte>) this._bytes).StartsWith(prefix, start, end);
  }

  public bool startswith(PythonTuple prefix) => ((IList<byte>) this._bytes).StartsWith(prefix);

  public bool startswith(PythonTuple prefix, int start)
  {
    return ((IList<byte>) this._bytes).StartsWith(prefix, start);
  }

  public bool startswith(PythonTuple prefix, int start, int end)
  {
    return ((IList<byte>) this._bytes).StartsWith(prefix, start, end);
  }

  public Bytes strip()
  {
    List<byte> bytes = ((IList<byte>) this._bytes).Strip();
    return bytes == null ? this : new Bytes((IList<byte>) bytes);
  }

  public Bytes strip([BytesConversion] IList<byte> chars)
  {
    lock (this)
    {
      List<byte> bytes = ((IList<byte>) this._bytes).Strip(chars);
      return bytes == null ? this : new Bytes((IList<byte>) bytes);
    }
  }

  public Bytes swapcase() => new Bytes((IList<byte>) ((IList<byte>) this._bytes).SwapCase());

  public Bytes title()
  {
    lock (this)
    {
      List<byte> byteList = ((IList<byte>) this._bytes).Title();
      return byteList == null ? this : new Bytes(byteList.ToArray());
    }
  }

  public Bytes translate([BytesConversion] IList<byte> table)
  {
    if (table == null)
      return this;
    if (table.Count != 256 /*0x0100*/)
      throw PythonOps.ValueError("translation table must be 256 characters long");
    return this.Count == 0 ? this : new Bytes((IList<byte>) ((IList<byte>) this._bytes).Translate(table, (IList<byte>) null));
  }

  public Bytes translate([BytesConversion] IList<byte> table, [BytesConversion] IList<byte> deletechars)
  {
    if (deletechars == null)
      throw PythonOps.TypeError("expected bytes or bytearray, got None");
    return this.Count == 0 ? this : new Bytes((IList<byte>) ((IList<byte>) this._bytes).Translate(table, deletechars));
  }

  public Bytes upper() => new Bytes((IList<byte>) ((IList<byte>) this._bytes).ToUpper());

  public Bytes zfill(int width)
  {
    int spaces = width - this.Count;
    return spaces <= 0 ? this : new Bytes((IList<byte>) ((IList<byte>) this._bytes).ZeroFill(width, spaces));
  }

  public bool __contains__([BytesConversion] IList<byte> bytes) => this.IndexOf(bytes, 0) != -1;

  public bool __contains__(CodeContext context, int value)
  {
    if (!context.LanguageContext.PythonOptions.Python30)
      throw PythonOps.TypeError("'in <bytes>' requires string or bytes as left operand, not int");
    return this.IndexOf(value.ToByteChecked()) != -1;
  }

  public bool __contains__(CodeContext context, object value)
  {
    if (value is Extensible<string>)
      return this.__contains__((IList<byte>) PythonOps.MakeBytes(((Extensible<string>) value).Value.MakeByteArray()));
    if (!context.LanguageContext.PythonOptions.Python30)
      throw PythonOps.TypeError("'in <bytes>' requires string or bytes as left operand, not {0}", (object) PythonTypeOps.GetName(value));
    switch (value)
    {
      case Extensible<int> _:
        return this.IndexOf(((Extensible<int>) value).Value.ToByteChecked()) != -1;
      case BigInteger bigInteger:
        return this.IndexOf(bigInteger.ToByteChecked()) != -1;
      case Extensible<BigInteger> _:
        return this.IndexOf(((Extensible<BigInteger>) value).Value.ToByteChecked()) != -1;
      default:
        throw PythonOps.TypeError("Type {0} doesn't support the buffer API", (object) PythonTypeOps.GetOldName(value));
    }
  }

  public PythonTuple __reduce__(CodeContext context)
  {
    return PythonTuple.MakeTuple((object) DynamicHelpers.GetPythonType((object) this), (object) PythonTuple.MakeTuple((object) this.MakeString(), (object) "latin-1"), this.GetType() == typeof (Bytes) ? (object) null : ObjectOps.ReduceProtocol0(context, (object) this)[2]);
  }

  public virtual string __repr__(CodeContext context) => ((IList<byte>) this._bytes).BytesRepr();

  public override string ToString() => this.MakeString();

  public static Bytes operator +(Bytes self, Bytes other)
  {
    List<byte> bytes = self != null ? new List<byte>((IEnumerable<byte>) self._bytes) : throw PythonOps.TypeError("expected bytes, got None");
    bytes.AddRange((IEnumerable<byte>) other._bytes);
    return new Bytes((IList<byte>) bytes);
  }

  public static ByteArray operator +(Bytes self, ByteArray other)
  {
    List<byte> bytes = new List<byte>((IEnumerable<byte>) self._bytes);
    lock (other)
      bytes.AddRange((IEnumerable<byte>) other);
    return new ByteArray(bytes);
  }

  public static string operator +(Bytes self, string other) => self.ToString() + other;

  public static string operator +(string other, Bytes self) => other + self.ToString();

  public static Bytes operator *(Bytes x, int y)
  {
    return y == 1 ? x : new Bytes((IList<byte>) ((IList<byte>) x._bytes).Multiply(y));
  }

  public static Bytes operator *(int x, Bytes y) => y * x;

  public static bool operator >(Bytes x, Bytes y)
  {
    return y == null || ((IList<byte>) x._bytes).Compare((IList<byte>) y._bytes) > 0;
  }

  public static bool operator <(Bytes x, Bytes y)
  {
    return y != null && ((IList<byte>) x._bytes).Compare((IList<byte>) y._bytes) < 0;
  }

  public static bool operator >=(Bytes x, Bytes y)
  {
    return y == null || ((IList<byte>) x._bytes).Compare((IList<byte>) y._bytes) >= 0;
  }

  public static bool operator <=(Bytes x, Bytes y)
  {
    return y != null && ((IList<byte>) x._bytes).Compare((IList<byte>) y._bytes) <= 0;
  }

  public object this[CodeContext context, int index]
  {
    get
    {
      byte num = this._bytes[PythonOps.FixIndex(index, this._bytes.Length)];
      if (context.LanguageContext.PythonOptions.Python30)
        return (object) (int) num;
      return (object) new Bytes(new byte[1]{ num });
    }
    [PythonHidden(new PlatformID[] {})] set => throw new InvalidOperationException();
  }

  public object this[CodeContext context, BigInteger index]
  {
    get
    {
      int ret;
      if (index.AsInt32(out ret))
        return this[context, ret];
      throw PythonOps.IndexError("cannot fit long in index");
    }
  }

  public Bytes this[Slice slice]
  {
    get
    {
      List<byte> byteList = ((IList<byte>) this._bytes).Slice(slice);
      return byteList == null ? Bytes.Empty : new Bytes(byteList.ToArray());
    }
  }

  [PythonHidden(new PlatformID[] {})]
  public byte[] ToByteArray()
  {
    byte[] byteArray = (byte[]) null;
    if (this._bytes != null)
    {
      byteArray = new byte[this._bytes.Length];
      this._bytes.CopyTo((Array) byteArray, 0);
    }
    return byteArray;
  }

  [PythonHidden(new PlatformID[] {})]
  public byte[] GetUnsafeByteArray() => this._bytes;

  private static Bytes JoinOne(object curVal)
  {
    switch (curVal)
    {
      case IList<byte> _:
        return curVal is Bytes bytes ? bytes : new Bytes(curVal as IList<byte>);
      case string _:
        return PythonOps.MakeBytes(((string) curVal).MakeByteArray());
      default:
        throw PythonOps.TypeError("can only join an iterable of bytes");
    }
  }

  internal static Bytes Concat(IList<Bytes> list, int length)
  {
    byte[] numArray = new byte[length];
    int destinationIndex = 0;
    for (int index = 0; index < list.Count; ++index)
    {
      Array.Copy((Array) list[index]._bytes, 0, (Array) numArray, destinationIndex, list[index]._bytes.Length);
      destinationIndex += list[index]._bytes.Length;
    }
    return new Bytes(numArray);
  }

  [PythonHidden(new PlatformID[] {})]
  public int IndexOf(byte item)
  {
    for (int index = 0; index < this._bytes.Length; ++index)
    {
      if ((int) this._bytes[index] == (int) item)
        return index;
    }
    return -1;
  }

  [PythonHidden(new PlatformID[] {})]
  public void Insert(int index, byte item) => throw new InvalidOperationException();

  [PythonHidden(new PlatformID[] {})]
  public void RemoveAt(int index) => throw new InvalidOperationException();

  byte IList<byte>.this[int index]
  {
    get => this._bytes[index];
    set => throw new InvalidOperationException();
  }

  [PythonHidden(new PlatformID[] {})]
  public void Add(byte item) => throw new InvalidOperationException();

  [PythonHidden(new PlatformID[] {})]
  public void Clear() => throw new InvalidOperationException();

  [PythonHidden(new PlatformID[] {})]
  public bool Contains(byte item) => ((ICollection<byte>) this._bytes).Contains(item);

  [PythonHidden(new PlatformID[] {})]
  public void CopyTo(byte[] array, int arrayIndex) => this._bytes.CopyTo((Array) array, arrayIndex);

  public int Count
  {
    [PythonHidden(new PlatformID[] {})] get => this._bytes.Length;
  }

  public bool IsReadOnly
  {
    [PythonHidden(new PlatformID[] {})] get => true;
  }

  [PythonHidden(new PlatformID[] {})]
  public bool Remove(byte item) => throw new InvalidOperationException();

  [PythonHidden(new PlatformID[] {})]
  public IEnumerator<byte> GetEnumerator() => ((IEnumerable<byte>) this._bytes).GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => this._bytes.GetEnumerator();

  public override bool Equals(object obj)
  {
    switch (obj)
    {
      case IList<byte> other:
        return ((IList<byte>) this._bytes).Compare(other) == 0;
      case Extensible<string> extensible:
        pattern_1 = extensible.Value;
        break;
    }
    return pattern_1 != null && this.ToString() == pattern_1;
  }

  public override int GetHashCode() => this.ToString().GetHashCode();

  Expression IExpressionSerializable.CreateExpression()
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeBytes"), (Expression) Expression.NewArrayInit(typeof (byte), (Expression[]) ArrayUtils.ConvertAll<byte, ConstantExpression>(this._bytes, (Func<byte, ConstantExpression>) (b => Expression.Constant((object) b)))));
  }

  Bytes IBufferProtocol.GetItem(int index)
  {
    return new Bytes(new byte[1]
    {
      this._bytes[PythonOps.FixIndex(index, this._bytes.Length)]
    });
  }

  void IBufferProtocol.SetItem(int index, object value) => throw new InvalidOperationException();

  void IBufferProtocol.SetSlice(Slice index, object value) => throw new InvalidOperationException();

  int IBufferProtocol.ItemCount => this._bytes.Length;

  string IBufferProtocol.Format => "B";

  BigInteger IBufferProtocol.ItemSize => (BigInteger) 1;

  BigInteger IBufferProtocol.NumberDimensions => (BigInteger) 1;

  bool IBufferProtocol.ReadOnly => true;

  IList<BigInteger> IBufferProtocol.GetShape(int start, int? end)
  {
    return end.HasValue ? (IList<BigInteger>) new BigInteger[1]
    {
      (BigInteger) end.Value - (BigInteger) start
    } : (IList<BigInteger>) new BigInteger[1]
    {
      (BigInteger) this._bytes.Length - (BigInteger) start
    };
  }

  PythonTuple IBufferProtocol.Strides => PythonTuple.MakeTuple((object) 1);

  object IBufferProtocol.SubOffsets => (object) null;

  Bytes IBufferProtocol.ToBytes(int start, int? end)
  {
    return start == 0 && !end.HasValue ? this : this[new Slice((object) start, (object) end)];
  }

  List IBufferProtocol.ToList(int start, int? end)
  {
    List<byte> byteList = ((IList<byte>) this._bytes).Slice(new Slice((object) start, (object) end));
    return byteList == null ? new List() : new List((ICollection) byteList.ToArray());
  }
}
