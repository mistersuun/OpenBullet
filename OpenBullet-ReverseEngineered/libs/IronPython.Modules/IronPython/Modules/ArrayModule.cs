// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.ArrayModule
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace IronPython.Modules;

public static class ArrayModule
{
  public const string __doc__ = "Provides arrays for native data types.  These can be used for compact storage or native interop via ctypes";
  public static readonly PythonType ArrayType = DynamicHelpers.GetPythonTypeFromType(typeof (ArrayModule.array));

  [PythonType]
  public class array : 
    IPythonArray,
    IList<object>,
    ICollection<object>,
    IEnumerable<object>,
    IEnumerable,
    IWeakReferenceable,
    ICollection,
    ICodeFormattable,
    IStructuralEquatable
  {
    private ArrayModule.array.ArrayData _data;
    private readonly char _typeCode;
    private WeakRefTracker _tracker;
    public const object __hash__ = null;

    public array([BytesConversion] string type, [Optional] object initializer)
    {
      this._typeCode = type != null && type.Length == 1 ? type[0] : throw PythonOps.TypeError("expected character, got {0}", (object) PythonTypeOps.GetName((object) type));
      this._data = ArrayModule.array.CreateData(this._typeCode);
      if (initializer == Missing.Value)
        return;
      this.extend(initializer);
    }

    private array(char typeCode, ArrayModule.array.ArrayData data)
    {
      this._typeCode = typeCode;
      this._data = data;
    }

    private static ArrayModule.array.ArrayData CreateData(char typecode)
    {
      switch (typecode)
      {
        case 'B':
          return (ArrayModule.array.ArrayData) new ArrayModule.array.ArrayData<byte>();
        case 'H':
          return (ArrayModule.array.ArrayData) new ArrayModule.array.ArrayData<ushort>();
        case 'I':
        case 'L':
          return (ArrayModule.array.ArrayData) new ArrayModule.array.ArrayData<uint>();
        case 'b':
          return (ArrayModule.array.ArrayData) new ArrayModule.array.ArrayData<sbyte>();
        case 'c':
          return (ArrayModule.array.ArrayData) new ArrayModule.array.ArrayData<char>();
        case 'd':
          return (ArrayModule.array.ArrayData) new ArrayModule.array.ArrayData<double>();
        case 'f':
          return (ArrayModule.array.ArrayData) new ArrayModule.array.ArrayData<float>();
        case 'h':
          return (ArrayModule.array.ArrayData) new ArrayModule.array.ArrayData<short>();
        case 'i':
        case 'l':
          return (ArrayModule.array.ArrayData) new ArrayModule.array.ArrayData<int>();
        case 'u':
          return (ArrayModule.array.ArrayData) new ArrayModule.array.ArrayData<char>();
        default:
          throw PythonOps.ValueError("Bad type code (expected one of 'c', 'b', 'B', 'u', 'H', 'h', 'i', 'I', 'l', 'L', 'f', 'd')");
      }
    }

    [SpecialName]
    public ArrayModule.array InPlaceAdd(ArrayModule.array other)
    {
      if (this.typecode != other.typecode)
        throw PythonOps.TypeError("cannot add different typecodes");
      if (other._data.Length != 0)
        this.extend((object) other);
      return this;
    }

    public static ArrayModule.array operator +(ArrayModule.array self, ArrayModule.array other)
    {
      if (self.typecode != other.typecode)
        throw PythonOps.TypeError("cannot add different typecodes");
      ArrayModule.array array = new ArrayModule.array(self.typecode, (object) Missing.Value);
      foreach (object iterable in (IEnumerable<object>) self)
        array.append(iterable);
      foreach (object iterable in (IEnumerable<object>) other)
        array.append(iterable);
      return array;
    }

    [SpecialName]
    public ArrayModule.array InPlaceMultiply(int value)
    {
      if (value <= 0)
      {
        this._data.Clear();
      }
      else
      {
        IronPython.Runtime.List iterable = this.tolist();
        for (int index = 0; index < value - 1; ++index)
          this.extend((object) iterable);
      }
      return this;
    }

    public static ArrayModule.array operator *(ArrayModule.array array, int value)
    {
      if ((BigInteger) value * (BigInteger) array.__len__() * (BigInteger) array.itemsize > (long) int.MaxValue)
        throw PythonOps.MemoryError("");
      return value <= 0 ? new ArrayModule.array(array.typecode, (object) Missing.Value) : new ArrayModule.array(array._typeCode, array._data.Multiply(value));
    }

    public static ArrayModule.array operator *(ArrayModule.array array, BigInteger value)
    {
      int ret;
      if (!value.AsInt32(out ret))
        throw PythonOps.OverflowError("cannot fit 'long' into an index-sized integer");
      if (value * (BigInteger) array.__len__() * (BigInteger) array.itemsize > (long) int.MaxValue)
        throw PythonOps.MemoryError("");
      return array * ret;
    }

    public static ArrayModule.array operator *(int value, ArrayModule.array array) => array * value;

    public static ArrayModule.array operator *(BigInteger value, ArrayModule.array array)
    {
      return array * value;
    }

    public void append(object iterable) => this._data.Append(iterable);

    internal IntPtr GetArrayAddress() => this._data.GetAddress();

    public PythonTuple buffer_info()
    {
      return PythonTuple.MakeTuple(this._data.GetAddress().ToPython(), (object) this._data.Length);
    }

    public void byteswap()
    {
      MemoryStream stream = this.ToStream();
      byte[] buffer = new byte[stream.Length];
      stream.Read(buffer, 0, buffer.Length);
      byte[] numArray = new byte[this.itemsize];
      for (int index1 = 0; index1 < buffer.Length; index1 += this.itemsize)
      {
        for (int index2 = 0; index2 < this.itemsize; ++index2)
          numArray[index2] = buffer[index1 + index2];
        for (int index3 = 0; index3 < this.itemsize; ++index3)
          buffer[index1 + index3] = numArray[this.itemsize - (index3 + 1)];
      }
      this._data.Clear();
      this.FromStream((Stream) new MemoryStream(buffer));
    }

    public int count(object x) => x == null ? 0 : this._data.Count(x);

    public void extend(object iterable)
    {
      switch (iterable)
      {
        case ArrayModule.array array:
          if (this.typecode != array.typecode)
            throw PythonOps.TypeError("cannot extend with different typecode");
          int length = array._data.Length;
          for (int index = 0; index < length; ++index)
            this._data.Append(array._data.GetData(index));
          break;
        case string s when this._typeCode != 'u':
          this.fromstring(s);
          break;
        case Bytes b:
          this.fromstring(b);
          break;
        case PythonBuffer buf:
          this.fromstring(buf);
          break;
        default:
          IEnumerator enumerator = PythonOps.GetEnumerator(iterable);
          while (enumerator.MoveNext())
            this.append(enumerator.Current);
          break;
      }
    }

    public void fromlist(object iterable)
    {
      IEnumerator enumerator = PythonOps.GetEnumerator(iterable);
      List<object> iterable1 = new List<object>();
      while (enumerator.MoveNext())
      {
        if (!this._data.CanStore(enumerator.Current))
          throw PythonOps.TypeError("expected {0}, got {1}", (object) DynamicHelpers.GetPythonTypeFromType(this._data.StorageType).Name, (object) DynamicHelpers.GetPythonType(enumerator.Current).Name);
        iterable1.Add(enumerator.Current);
      }
      this.extend((object) iterable1);
    }

    public void fromfile(PythonFile f, int n)
    {
      int size = n * this.itemsize;
      string s = f.read(size);
      if (s.Length < size)
        throw PythonOps.EofError("file not large enough");
      this.fromstring(s);
    }

    public void fromstring([NotNull] Bytes b)
    {
      if (b.Count % this.itemsize != 0)
        throw PythonOps.ValueError("string length not a multiple of itemsize");
      this.FromStream((Stream) new MemoryStream(b._bytes, false));
    }

    public void fromstring([NotNull] string s)
    {
      if (s.Length % this.itemsize != 0)
        throw PythonOps.ValueError("string length not a multiple of itemsize");
      byte[] buffer = new byte[s.Length];
      for (int index = 0; index < buffer.Length; ++index)
        buffer[index] = checked ((byte) s[index]);
      this.FromStream((Stream) new MemoryStream(buffer));
    }

    public void fromstring([NotNull] PythonBuffer buf)
    {
      if (buf.Size % this.itemsize != 0)
        throw PythonOps.ValueError("string length not a multiple of itemsize");
      this.FromStream((Stream) new MemoryStream(buf.byteCache, false));
    }

    public void fromunicode(CodeContext context, string s)
    {
      if (s == null)
        throw PythonOps.TypeError("expected string");
      if (this._typeCode != 'u')
        throw PythonOps.ValueError("fromunicode() may only be called on type 'u' arrays");
      ArrayModule.array.ArrayData<char> data = (ArrayModule.array.ArrayData<char>) this._data;
      data.EnsureSize(data.Length + s.Length);
      for (int index = 0; index < s.Length; ++index)
        data.Data[index + data.Length] = s[index];
      data.Length += s.Length;
    }

    public int index(object x)
    {
      int num = x != null ? this._data.Index(x) : throw PythonOps.ValueError("got None, expected value");
      return num != -1 ? num : throw PythonOps.ValueError("x not found");
    }

    public void insert(int i, object x)
    {
      if (i > this._data.Length)
        i = this._data.Length;
      if (i < 0)
        i = this._data.Length + i;
      if (i < 0)
        i = 0;
      this._data.Insert(i, x);
    }

    public int itemsize
    {
      get
      {
        switch (this._typeCode)
        {
          case 'B':
          case 'b':
          case 'c':
          case 'p':
          case 's':
          case 'x':
            return 1;
          case 'H':
          case 'h':
          case 'u':
            return 2;
          case 'I':
          case 'L':
          case 'f':
          case 'i':
          case 'l':
            return 4;
          case 'P':
            return IntPtr.Size;
          case 'Q':
          case 'd':
          case 'q':
            return 8;
          default:
            return 0;
        }
      }
    }

    public object pop() => this.pop(-1);

    public object pop(int i)
    {
      i = PythonOps.FixIndex(i, this._data.Length);
      object data = this._data.GetData(i);
      this._data.RemoveAt(i);
      return data;
    }

    [Python3Warning("array.read() not supported in 3.x; use array.fromfile()")]
    public void read(PythonFile f, int n) => this.fromfile(f, n);

    public void remove(object value)
    {
      if (value == null)
        throw PythonOps.ValueError("got None, expected value");
      this._data.Remove(value);
    }

    public void reverse()
    {
      for (int x = 0; x < this._data.Length / 2; ++x)
        this._data.Swap(x, this._data.Length - (x + 1));
    }

    public virtual object this[int index]
    {
      get
      {
        object data = this._data.GetData(PythonOps.FixIndex(index, this._data.Length));
        switch (this._typeCode)
        {
          case 'B':
            return (object) (int) (byte) data;
          case 'H':
            return (object) (int) (ushort) data;
          case 'I':
            return (object) (BigInteger) (uint) data;
          case 'L':
            return (object) (BigInteger) (uint) data;
          case 'b':
            return (object) (int) (sbyte) data;
          case 'c':
          case 'u':
            return (object) new string((char) data, 1);
          case 'd':
            return data;
          case 'f':
            return (object) (double) (float) data;
          case 'h':
            return (object) (int) (short) data;
          case 'i':
            return data;
          case 'l':
            return data;
          default:
            throw PythonOps.ValueError("Bad type code (expected one of 'c', 'b', 'B', 'u', 'H', 'h', 'i', 'I', 'l', 'L', 'f', 'd')");
        }
      }
      set => this._data.SetData(PythonOps.FixIndex(index, this._data.Length), value);
    }

    internal byte[] RawGetItem(int index)
    {
      MemoryStream output = new MemoryStream();
      BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
      switch (this._typeCode)
      {
        case 'B':
          binaryWriter.Write((byte) this._data.GetData(index));
          break;
        case 'H':
          binaryWriter.Write((ushort) this._data.GetData(index));
          break;
        case 'I':
        case 'L':
          binaryWriter.Write((uint) this._data.GetData(index));
          break;
        case 'b':
          binaryWriter.Write((sbyte) this._data.GetData(index));
          break;
        case 'c':
          binaryWriter.Write((byte) (char) this._data.GetData(index));
          break;
        case 'd':
          binaryWriter.Write((double) this._data.GetData(index));
          break;
        case 'f':
          binaryWriter.Write((float) this._data.GetData(index));
          break;
        case 'h':
          binaryWriter.Write((short) this._data.GetData(index));
          break;
        case 'i':
        case 'l':
          binaryWriter.Write((int) this._data.GetData(index));
          break;
        case 'u':
          binaryWriter.Write((char) this._data.GetData(index));
          break;
      }
      return output.ToArray();
    }

    public void __delitem__(int index)
    {
      this._data.RemoveAt(PythonOps.FixIndex(index, this._data.Length));
    }

    public void __delitem__(IronPython.Runtime.Slice slice)
    {
      if (slice == null)
        throw PythonOps.TypeError("expected Slice, got None");
      int ostart;
      int ostop;
      int ostep;
      slice.indices(this._data.Length, out ostart, out ostop, out ostep);
      if (ostep > 0 && ostart >= ostop || ostep < 0 && ostart <= ostop)
        return;
      switch (ostep)
      {
        case -1:
          int index1 = ostop + 1;
          int index2 = ostart + 1;
          while (index2 < this._data.Length)
          {
            this._data.SetData(index1, this._data.GetData(index2));
            ++index2;
            ++index1;
          }
          for (int index3 = 0; index3 < ostop - ostart; ++index3)
            this._data.RemoveAt(this._data.Length - 1);
          break;
        case 1:
          int index4 = ostart;
          int index5 = ostop;
          while (index5 < this._data.Length)
          {
            this._data.SetData(index4, this._data.GetData(index5));
            ++index5;
            ++index4;
          }
          for (int index6 = 0; index6 < ostop - ostart; ++index6)
            this._data.RemoveAt(this._data.Length - 1);
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
          int index7 = num3 = ostart;
          int num4 = num3;
          int num5;
          for (num5 = num3; num5 < ostop && index7 < ostop; ++index7)
          {
            if (index7 != num4)
              this._data.SetData(num5++, this._data.GetData(index7));
            else
              num4 += ostep;
          }
          while (ostop < this._data.Length)
            this._data.SetData(num5++, this._data.GetData(ostop++));
          while (this._data.Length > num5)
            this._data.RemoveAt(this._data.Length - 1);
          break;
      }
    }

    public object this[IronPython.Runtime.Slice index]
    {
      get
      {
        if (index == null)
          throw PythonOps.TypeError("expected Slice, got None");
        int ostart;
        int ostop;
        int ostep;
        index.indices(this._data.Length, out ostart, out ostop, out ostep);
        ArrayModule.array array = new ArrayModule.array(new string(this._typeCode, 1), (object) Missing.Value);
        if (ostep < 0)
        {
          for (int index1 = ostart; index1 > ostop; index1 += ostep)
            array._data.Append(this._data.GetData(index1));
        }
        else
        {
          for (int index2 = ostart; index2 < ostop; index2 += ostep)
            array._data.Append(this._data.GetData(index2));
        }
        return (object) array;
      }
      set
      {
        if (index == null)
          throw PythonOps.TypeError("expected Slice, got None");
        this.CheckSliceAssignType(value);
        if (index.step != null)
        {
          if (value == this)
            value = (object) this.tolist();
          index.DoSliceAssign(new IronPython.Runtime.Slice.SliceAssign(this.SliceAssign), this._data.Length, value);
        }
        else
        {
          int ostart;
          int ostop;
          index.indices(this._data.Length, out ostart, out ostop, out int _);
          if (ostop < ostart)
            ostop = ostart;
          this.SliceNoStep(value, ostart, ostop);
        }
      }
    }

    private void CheckSliceAssignType(object value)
    {
      if (!(value is ArrayModule.array array))
        throw PythonOps.TypeError("can only assign array (not \"{0}\") to array slice", (object) PythonTypeOps.GetName(value));
      if (array != null && (int) array._typeCode != (int) this._typeCode)
        throw PythonOps.TypeError("bad argument type for built-in operation");
    }

    private void SliceNoStep(object value, int start, int stop)
    {
      IEnumerator enumerator = PythonOps.GetEnumerator(value);
      ArrayModule.array.ArrayData data = ArrayModule.array.CreateData(this._typeCode);
      for (int index = 0; index < start; ++index)
        data.Append(this._data.GetData(index));
      while (enumerator.MoveNext())
        data.Append(enumerator.Current);
      for (int index = Math.Max(stop, start); index < this._data.Length; ++index)
        data.Append(this._data.GetData(index));
      this._data = data;
    }

    public object __getslice__(object start, object stop) => this[new IronPython.Runtime.Slice(start, stop)];

    public void __setslice__(int start, int stop, object value)
    {
      this.CheckSliceAssignType(value);
      IronPython.Runtime.Slice.FixSliceArguments(this._data.Length, ref start, ref stop);
      this.SliceNoStep(value, start, stop);
    }

    public void __delslice__(object start, object stop) => this.__delitem__(new IronPython.Runtime.Slice(start, stop));

    public PythonTuple __reduce__()
    {
      return PythonOps.MakeTuple((object) DynamicHelpers.GetPythonType((object) this), (object) PythonOps.MakeTuple((object) this.typecode, (object) this.tolist()), null);
    }

    public ArrayModule.array __copy__() => new ArrayModule.array(this.typecode, (object) this);

    public ArrayModule.array __deepcopy__(ArrayModule.array arg) => arg.__copy__();

    public PythonTuple __reduce_ex__(int version) => this.__reduce__();

    public PythonTuple __reduce_ex__() => this.__reduce__();

    private void SliceAssign(int index, object value) => this._data.SetData(index, value);

    public void tofile(PythonFile f) => f.write(this.tostring());

    public IronPython.Runtime.List tolist()
    {
      IronPython.Runtime.List list = new IronPython.Runtime.List();
      for (int index = 0; index < this._data.Length; ++index)
        list.AddNoLock(this[index]);
      return list;
    }

    public string tostring()
    {
      Stream stream = (Stream) this.ToStream();
      byte[] buffer = new byte[stream.Length];
      stream.Read(buffer, 0, (int) stream.Length);
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < buffer.Length; ++index)
        stringBuilder.Append((char) buffer[index]);
      return stringBuilder.ToString();
    }

    public string tounicode(CodeContext context)
    {
      if (this._typeCode != 'u')
        throw PythonOps.ValueError("only 'u' arrays can be converted to unicode");
      return new string(((ArrayModule.array.ArrayData<char>) this._data).Data, 0, this._data.Length);
    }

    [Python3Warning("array.write() not supported in 3.x; use array.tofile()")]
    public void write(PythonFile f) => this.tofile(f);

    public string typecode => ScriptingRuntimeHelpers.CharToString(this._typeCode);

    internal MemoryStream ToStream()
    {
      MemoryStream ms = new MemoryStream();
      this.ToStream((Stream) ms);
      ms.Seek(0L, SeekOrigin.Begin);
      return ms;
    }

    internal void ToStream(Stream ms)
    {
      BinaryWriter binaryWriter = new BinaryWriter(ms, Encoding.Unicode);
      for (int index = 0; index < this._data.Length; ++index)
      {
        switch (this._typeCode)
        {
          case 'B':
            binaryWriter.Write((byte) this._data.GetData(index));
            break;
          case 'H':
            binaryWriter.Write((ushort) this._data.GetData(index));
            break;
          case 'I':
          case 'L':
            binaryWriter.Write((uint) this._data.GetData(index));
            break;
          case 'b':
            binaryWriter.Write((sbyte) this._data.GetData(index));
            break;
          case 'c':
            binaryWriter.Write((byte) (char) this._data.GetData(index));
            break;
          case 'd':
            binaryWriter.Write((double) this._data.GetData(index));
            break;
          case 'f':
            binaryWriter.Write((float) this._data.GetData(index));
            break;
          case 'h':
            binaryWriter.Write((short) this._data.GetData(index));
            break;
          case 'i':
          case 'l':
            binaryWriter.Write((int) this._data.GetData(index));
            break;
          case 'u':
            binaryWriter.Write((char) this._data.GetData(index));
            break;
        }
      }
    }

    internal byte[] ToByteArray()
    {
      if (!(this._data is ArrayModule.array.ArrayData<byte> data))
        return this.ToStream().ToArray();
      byte[] destinationArray = new byte[data.Length];
      Array.Copy((Array) data.Data, (Array) destinationArray, data.Length);
      return destinationArray;
    }

    internal void Clear() => this._data = ArrayModule.array.CreateData(this._typeCode);

    internal void FromStream(Stream ms)
    {
      BinaryReader br = new BinaryReader(ms);
      if (this._data is ArrayModule.array.ArrayData<byte> data)
      {
        int length = (int) ms.Length;
        data.EnsureSize(data.Length + length);
        Array.Copy((Array) br.ReadBytes(length), 0, (Array) data.Data, data.Length, length);
        data.Length += length;
      }
      else
      {
        for (int index = 0; (long) index < ms.Length / (long) this.itemsize; ++index)
        {
          object obj;
          switch (this._typeCode)
          {
            case 'B':
              obj = (object) br.ReadByte();
              break;
            case 'H':
              obj = (object) br.ReadUInt16();
              break;
            case 'I':
              obj = (object) br.ReadUInt32();
              break;
            case 'L':
              obj = (object) br.ReadUInt32();
              break;
            case 'b':
              obj = (object) (sbyte) br.ReadByte();
              break;
            case 'c':
              obj = (object) (char) br.ReadByte();
              break;
            case 'd':
              obj = (object) br.ReadDouble();
              break;
            case 'f':
              obj = (object) br.ReadSingle();
              break;
            case 'h':
              obj = (object) br.ReadInt16();
              break;
            case 'i':
              obj = (object) br.ReadInt32();
              break;
            case 'l':
              obj = (object) br.ReadInt32();
              break;
            case 'u':
              obj = ArrayModule.array.ReadBinaryChar(br);
              break;
            default:
              throw new InvalidOperationException();
          }
          this._data.Append(obj);
        }
      }
    }

    internal void FromStream(Stream ms, int index)
    {
      BinaryReader br = new BinaryReader(ms);
      for (int index1 = index; (long) index1 < ms.Length / (long) this.itemsize + (long) index; ++index1)
      {
        object obj;
        switch (this._typeCode)
        {
          case 'B':
            obj = (object) br.ReadByte();
            break;
          case 'H':
            obj = (object) br.ReadUInt16();
            break;
          case 'I':
            obj = (object) br.ReadUInt32();
            break;
          case 'L':
            obj = (object) br.ReadUInt32();
            break;
          case 'b':
            obj = (object) (sbyte) br.ReadByte();
            break;
          case 'c':
            obj = (object) (char) br.ReadByte();
            break;
          case 'd':
            obj = (object) br.ReadDouble();
            break;
          case 'f':
            obj = (object) br.ReadSingle();
            break;
          case 'h':
            obj = (object) br.ReadInt16();
            break;
          case 'i':
            obj = (object) br.ReadInt32();
            break;
          case 'l':
            obj = (object) br.ReadInt32();
            break;
          case 'u':
            obj = ArrayModule.array.ReadBinaryChar(br);
            break;
          default:
            throw new InvalidOperationException();
        }
        this._data.SetData(index1, obj);
      }
    }

    internal long FromStream(Stream ms, int index, int nbytes)
    {
      BinaryReader br = new BinaryReader(ms);
      if (nbytes <= 0)
        return 0;
      int num = Math.Min((int) (ms.Length - ms.Position), nbytes);
      for (int index1 = index; index1 < num / this.itemsize + index; ++index1)
      {
        object obj;
        switch (this._typeCode)
        {
          case 'B':
            obj = (object) br.ReadByte();
            break;
          case 'H':
            obj = (object) br.ReadUInt16();
            break;
          case 'I':
            obj = (object) br.ReadUInt32();
            break;
          case 'L':
            obj = (object) br.ReadUInt32();
            break;
          case 'b':
            obj = (object) (sbyte) br.ReadByte();
            break;
          case 'c':
            obj = (object) (char) br.ReadByte();
            break;
          case 'd':
            obj = (object) br.ReadDouble();
            break;
          case 'f':
            obj = (object) br.ReadSingle();
            break;
          case 'h':
            obj = (object) br.ReadInt16();
            break;
          case 'i':
            obj = (object) br.ReadInt32();
            break;
          case 'l':
            obj = (object) br.ReadInt32();
            break;
          case 'u':
            obj = ArrayModule.array.ReadBinaryChar(br);
            break;
          default:
            throw new InvalidOperationException();
        }
        this._data.SetData(index1, obj);
      }
      if (num % this.itemsize > 0)
      {
        byte[] bytes = this.ToBytes(num / this.itemsize + index);
        for (int index2 = 0; index2 < num % this.itemsize; ++index2)
          bytes[index2] = br.ReadByte();
        this._data.SetData(num / this.itemsize + index, this.FromBytes(bytes));
      }
      return (long) num;
    }

    private static object ReadBinaryChar(BinaryReader br)
    {
      byte num = br.ReadByte();
      return (object) (char) ((uint) br.ReadByte() << 8 | (uint) num);
    }

    private byte[] ToBytes(int index)
    {
      switch (this._typeCode)
      {
        case 'B':
          return new byte[1]
          {
            (byte) this._data.GetData(index)
          };
        case 'H':
          return BitConverter.GetBytes((ushort) this._data.GetData(index));
        case 'I':
        case 'L':
          return BitConverter.GetBytes((uint) this._data.GetData(index));
        case 'b':
          return new byte[1]
          {
            (byte) (sbyte) this._data.GetData(index)
          };
        case 'c':
          return new byte[1]
          {
            (byte) (char) this._data.GetData(index)
          };
        case 'd':
          return BitConverter.GetBytes((double) this._data.GetData(index));
        case 'f':
          return BitConverter.GetBytes((float) this._data.GetData(index));
        case 'h':
          return BitConverter.GetBytes((short) this._data.GetData(index));
        case 'i':
        case 'l':
          return BitConverter.GetBytes((int) this._data.GetData(index));
        case 'u':
          return BitConverter.GetBytes((char) this._data.GetData(index));
        default:
          throw PythonOps.ValueError("Bad type code (expected one of 'c', 'b', 'B', 'u', 'H', 'h', 'i', 'I', 'l', 'L', 'f', 'd')");
      }
    }

    private object FromBytes(byte[] bytes)
    {
      switch (this._typeCode)
      {
        case 'B':
          return (object) bytes[0];
        case 'H':
          return (object) BitConverter.ToUInt16(bytes, 0);
        case 'I':
        case 'L':
          return (object) BitConverter.ToUInt32(bytes, 0);
        case 'b':
          return (object) (sbyte) bytes[0];
        case 'c':
          return (object) (char) bytes[0];
        case 'd':
          return (object) BitConverter.ToDouble(bytes, 0);
        case 'f':
          return (object) BitConverter.ToSingle(bytes, 0);
        case 'h':
          return (object) BitConverter.ToInt16(bytes, 0);
        case 'i':
        case 'l':
          return (object) BitConverter.ToInt32(bytes, 0);
        case 'u':
          return (object) BitConverter.ToChar(bytes, 0);
        default:
          throw PythonOps.ValueError("Bad type code (expected one of 'c', 'b', 'B', 'u', 'H', 'h', 'i', 'I', 'l', 'L', 'f', 'd')");
      }
    }

    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
    {
      IStructuralEquatable structuralEquatable;
      switch (this._typeCode)
      {
        case 'B':
          structuralEquatable = (IStructuralEquatable) PythonTuple.MakeTuple((object) ((ArrayModule.array.ArrayData<byte>) this._data).Data);
          break;
        case 'H':
          structuralEquatable = (IStructuralEquatable) PythonTuple.MakeTuple((object) ((ArrayModule.array.ArrayData<ushort>) this._data).Data);
          break;
        case 'I':
        case 'L':
          structuralEquatable = (IStructuralEquatable) PythonTuple.MakeTuple((object) ((ArrayModule.array.ArrayData<uint>) this._data).Data);
          break;
        case 'b':
          structuralEquatable = (IStructuralEquatable) PythonTuple.MakeTuple((object) ((ArrayModule.array.ArrayData<sbyte>) this._data).Data);
          break;
        case 'c':
          structuralEquatable = (IStructuralEquatable) PythonTuple.MakeTuple((object) ((ArrayModule.array.ArrayData<char>) this._data).Data);
          break;
        case 'd':
          structuralEquatable = (IStructuralEquatable) PythonTuple.MakeTuple((object) ((ArrayModule.array.ArrayData<double>) this._data).Data);
          break;
        case 'f':
          structuralEquatable = (IStructuralEquatable) PythonTuple.MakeTuple((object) ((ArrayModule.array.ArrayData<float>) this._data).Data);
          break;
        case 'h':
          structuralEquatable = (IStructuralEquatable) PythonTuple.MakeTuple((object) ((ArrayModule.array.ArrayData<short>) this._data).Data);
          break;
        case 'i':
        case 'l':
          structuralEquatable = (IStructuralEquatable) PythonTuple.MakeTuple((object) ((ArrayModule.array.ArrayData<int>) this._data).Data);
          break;
        case 'u':
          structuralEquatable = (IStructuralEquatable) PythonTuple.MakeTuple((object) ((ArrayModule.array.ArrayData<char>) this._data).Data);
          break;
        default:
          throw PythonOps.ValueError("Bad type code (expected one of 'c', 'b', 'B', 'u', 'H', 'h', 'i', 'I', 'l', 'L', 'f', 'd')");
      }
      return structuralEquatable.GetHashCode(comparer);
    }

    bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
    {
      if (!(other is ArrayModule.array array) || this._data.Length != array._data.Length)
        return false;
      for (int index = 0; index < this._data.Length; ++index)
      {
        if (!comparer.Equals(this._data.GetData(index), array._data.GetData(index)))
          return false;
      }
      return true;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      for (int i = 0; i < this._data.Length; ++i)
        yield return this._data.GetData(i);
    }

    public virtual string __repr__(CodeContext context)
    {
      string str = $"array('{this.typecode.ToString()}'";
      if (this._data.Length == 0)
        return str + ")";
      StringBuilder stringBuilder = new StringBuilder(str);
      if (this._typeCode == 'c' || this._typeCode == 'u')
      {
        char quote = '\'';
        string s = new string(((ArrayModule.array.ArrayData<char>) this._data).Data, 0, this._data.Length);
        if (s.IndexOf('\'') != -1 && s.IndexOf('"') == -1)
          quote = '"';
        if (this._typeCode == 'u')
          stringBuilder.Append(", u");
        else
          stringBuilder.Append(", ");
        stringBuilder.Append(quote);
        bool isUnicode = false;
        stringBuilder.Append(StringOps.ReprEncode(s, quote, ref isUnicode));
        stringBuilder.Append(quote);
        stringBuilder.Append(")");
      }
      else
      {
        stringBuilder.Append(", [");
        for (int index = 0; index < this._data.Length; ++index)
        {
          if (index > 0)
            stringBuilder.Append(", ");
          stringBuilder.Append(PythonOps.Repr(context, this[index]));
        }
        stringBuilder.Append("])");
      }
      return stringBuilder.ToString();
    }

    WeakRefTracker IWeakReferenceable.GetWeakRef() => this._tracker;

    bool IWeakReferenceable.SetWeakRef(WeakRefTracker value)
    {
      this._tracker = value;
      return true;
    }

    void IWeakReferenceable.SetFinalizer(WeakRefTracker value) => this._tracker = value;

    public int __len__() => this._data.Length;

    public bool __contains__(object value) => this._data.Index(value) != -1;

    private bool TryCompare(object other, out int res)
    {
      if (!(other is ArrayModule.array array) || array.typecode != this.typecode)
      {
        res = 0;
        return false;
      }
      if (array._data.Length != this._data.Length)
      {
        res = this._data.Length - array._data.Length;
      }
      else
      {
        res = 0;
        for (int index = 0; index < array._data.Length && res == 0; ++index)
          res = PythonOps.Compare(this._data.GetData(index), array._data.GetData(index));
      }
      return true;
    }

    public static object operator >(ArrayModule.array self, object other)
    {
      int res;
      return !self.TryCompare(other, out res) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(res > 0);
    }

    public static object operator <(ArrayModule.array self, object other)
    {
      int res;
      return !self.TryCompare(other, out res) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(res < 0);
    }

    public static object operator >=(ArrayModule.array self, object other)
    {
      int res;
      return !self.TryCompare(other, out res) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(res >= 0);
    }

    public static object operator <=(ArrayModule.array self, object other)
    {
      int res;
      return !self.TryCompare(other, out res) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(res <= 0);
    }

    void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();

    int ICollection.Count => this.__len__();

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => (object) this;

    int IList<object>.IndexOf(object item) => this._data.Index(item);

    void IList<object>.Insert(int index, object item) => this.insert(index, item);

    void IList<object>.RemoveAt(int index) => this.__delitem__(index);

    void ICollection<object>.Add(object item) => this.append(item);

    void ICollection<object>.Clear() => this.__delitem__(new IronPython.Runtime.Slice((object) null, (object) null));

    bool ICollection<object>.Contains(object item) => this.__contains__(item);

    void ICollection<object>.CopyTo(object[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    int ICollection<object>.Count => this.__len__();

    bool ICollection<object>.IsReadOnly => false;

    bool ICollection<object>.Remove(object item)
    {
      try
      {
        this.remove(item);
        return true;
      }
      catch (ArgumentException ex)
      {
        return false;
      }
    }

    IEnumerator<object> IEnumerable<object>.GetEnumerator()
    {
      for (int i = 0; i < this._data.Length; ++i)
        yield return this._data.GetData(i);
    }

    private abstract class ArrayData
    {
      public abstract void SetData(int index, object value);

      public abstract object GetData(int index);

      public abstract void Append(object value);

      public abstract int Count(object value);

      public abstract bool CanStore(object value);

      public abstract Type StorageType { get; }

      public abstract int Index(object value);

      public abstract void Insert(int index, object value);

      public abstract void Remove(object value);

      public abstract void RemoveAt(int index);

      public abstract int Length { get; set; }

      public abstract void Swap(int x, int y);

      public abstract void Clear();

      public abstract IntPtr GetAddress();

      public abstract ArrayModule.array.ArrayData Multiply(int count);
    }

    private class ArrayData<T> : ArrayModule.array.ArrayData
    {
      private T[] _data;
      private int _count;
      private GCHandle? _dataHandle;

      public ArrayData()
      {
        GC.SuppressFinalize((object) this);
        this._data = new T[8];
      }

      private ArrayData(int size)
      {
        GC.SuppressFinalize((object) this);
        this._data = new T[size];
        this._count = size;
      }

      ~ArrayData() => this._dataHandle.Value.Free();

      public T[] Data
      {
        get => this._data;
        set => this._data = value;
      }

      public override object GetData(int index) => (object) this._data[index];

      public override void SetData(int index, object value)
      {
        this._data[index] = ArrayModule.array.ArrayData<T>.GetValue(value);
      }

      private static T GetValue(object value)
      {
        if (!(value is T))
        {
          object result;
          if (!Converter.TryConvert(value, typeof (T), out result))
          {
            if (value != null && typeof (T).IsPrimitive && typeof (T) != typeof (char))
              throw PythonOps.OverflowError("couldn't convert {1} to {0}", (object) DynamicHelpers.GetPythonTypeFromType(typeof (T)).Name, (object) DynamicHelpers.GetPythonType(value).Name);
            throw PythonOps.TypeError("expected {0}, got {1}", (object) DynamicHelpers.GetPythonTypeFromType(typeof (T)).Name, (object) DynamicHelpers.GetPythonType(value).Name);
          }
          value = result;
        }
        return (T) value;
      }

      public override void Append(object value)
      {
        this.EnsureSize(this._count + 1);
        this._data[this._count++] = ArrayModule.array.ArrayData<T>.GetValue(value);
      }

      public void EnsureSize(int size)
      {
        if (this._data.Length >= size)
          return;
        int length = this._data.Length;
        while (length < size)
          length *= 2;
        Array.Resize<T>(ref this._data, length);
        if (!this._dataHandle.HasValue)
          return;
        this._dataHandle.Value.Free();
        this._dataHandle = new GCHandle?();
        GC.SuppressFinalize((object) this);
      }

      public override int Count(object value)
      {
        T obj = ArrayModule.array.ArrayData<T>.GetValue(value);
        int num = 0;
        for (int index = 0; index < this._count; ++index)
        {
          if (this._data[index].Equals((object) obj))
            ++num;
        }
        return num;
      }

      public override void Insert(int index, object value)
      {
        this.EnsureSize(this._count + 1);
        if (index < this._count)
          Array.Copy((Array) this._data, index, (Array) this._data, index + 1, this._count - index);
        this._data[index] = ArrayModule.array.ArrayData<T>.GetValue(value);
        ++this._count;
      }

      public override int Index(object value)
      {
        T obj = ArrayModule.array.ArrayData<T>.GetValue(value);
        for (int index = 0; index < this._count; ++index)
        {
          if (this._data[index].Equals((object) obj))
            return index;
        }
        return -1;
      }

      public override void Remove(object value)
      {
        T obj = ArrayModule.array.ArrayData<T>.GetValue(value);
        for (int index = 0; index < this._count; ++index)
        {
          if (this._data[index].Equals((object) obj))
          {
            this.RemoveAt(index);
            return;
          }
        }
        throw PythonOps.ValueError("couldn't find value to remove");
      }

      public override void RemoveAt(int index)
      {
        --this._count;
        if (index >= this._count)
          return;
        Array.Copy((Array) this._data, index + 1, (Array) this._data, index, this._count - index);
      }

      public override void Swap(int x, int y)
      {
        T obj = this._data[x];
        this._data[x] = this._data[y];
        this._data[y] = obj;
      }

      public override int Length
      {
        get => this._count;
        set => this._count = value;
      }

      public override void Clear() => this._count = 0;

      public override bool CanStore(object value)
      {
        return value is T || Converter.TryConvert(value, typeof (T), out object _);
      }

      public override Type StorageType => typeof (T);

      public override IntPtr GetAddress()
      {
        if (!this._dataHandle.HasValue)
        {
          this._dataHandle = new GCHandle?(GCHandle.Alloc((object) this._data, GCHandleType.Pinned));
          GC.ReRegisterForFinalize((object) this);
        }
        return this._dataHandle.Value.AddrOfPinnedObject();
      }

      public override ArrayModule.array.ArrayData Multiply(int count)
      {
        ArrayModule.array.ArrayData<T> arrayData = new ArrayModule.array.ArrayData<T>(count * this._count);
        if (count != 0)
        {
          Array.Copy((Array) this._data, (Array) arrayData._data, this._count);
          int num = count * this._count;
          int count1 = this._count;
          int count2 = this._count;
          while (count2 < num)
          {
            Array.Copy((Array) arrayData._data, 0, (Array) arrayData._data, count2, Math.Min(count1, num - count2));
            count2 += count1;
            count1 *= 2;
          }
        }
        return (ArrayModule.array.ArrayData) arrayData;
      }
    }
  }
}
