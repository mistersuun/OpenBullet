// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.MarshalOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

#nullable disable
namespace IronPython.Runtime.Operations;

public class MarshalOps
{
  public static byte[] GetBytes(object o, int version)
  {
    MarshalOps.MarshalWriter marshalWriter = new MarshalOps.MarshalWriter(version);
    marshalWriter.WriteObject(o);
    return marshalWriter.GetBytes();
  }

  public static object GetObject(IEnumerator<byte> bytes)
  {
    return new MarshalOps.MarshalReader(bytes).ReadObject();
  }

  private class MarshalWriter
  {
    private readonly List<byte> _bytes;
    private readonly int _version;
    private readonly Dictionary<string, int> _strings;

    public MarshalWriter(int version)
    {
      this._bytes = new List<byte>();
      this._version = version;
      if (this._version <= 0)
        return;
      this._strings = new Dictionary<string, int>();
    }

    public void WriteObject(object o)
    {
      List<object> reprInfinite = PythonOps.GetReprInfinite();
      int index = !reprInfinite.Contains(o) ? reprInfinite.Count : throw PythonOps.ValueError("Marshaled data contains infinite cycle");
      reprInfinite.Add(o);
      try
      {
        if (o == null)
          this._bytes.Add((byte) 78);
        else if (o == ScriptingRuntimeHelpers.True)
          this._bytes.Add((byte) 84);
        else if (o == ScriptingRuntimeHelpers.False)
        {
          this._bytes.Add((byte) 70);
        }
        else
        {
          switch (o)
          {
            case string _:
              this.WriteString(o as string);
              break;
            case int val3:
              this.WriteInt(val3);
              break;
            case float f1:
              this.WriteFloat(f1);
              break;
            case double f2:
              this.WriteFloat(f2);
              break;
            case long l:
              this.WriteLong(l);
              break;
            default:
              if (o.GetType() == typeof (List))
              {
                this.WriteList(o);
                break;
              }
              if (o.GetType() == typeof (PythonDictionary))
              {
                this.WriteDict(o);
                break;
              }
              if (o.GetType() == typeof (PythonTuple))
              {
                this.WriteTuple(o);
                break;
              }
              if (o.GetType() == typeof (SetCollection))
              {
                this.WriteSet(o);
                break;
              }
              if (o.GetType() == typeof (FrozenSetCollection))
              {
                this.WriteFrozenSet(o);
                break;
              }
              switch (o)
              {
                case BigInteger val1:
                  this.WriteInteger(val1);
                  return;
                case Complex val2:
                  this.WriteComplex(val2);
                  return;
                case PythonBuffer _:
                  this.WriteBuffer((PythonBuffer) o);
                  return;
                default:
                  if (o != PythonExceptions.StopIteration)
                    throw PythonOps.ValueError("unmarshallable object");
                  this.WriteStopIteration();
                  return;
              }
          }
        }
      }
      finally
      {
        reprInfinite.RemoveAt(index);
      }
    }

    private void WriteFloat(float f)
    {
      if (this._version > 1)
      {
        this._bytes.Add((byte) 103);
        this._bytes.AddRange((IEnumerable<byte>) BitConverter.GetBytes((double) f));
      }
      else
      {
        this._bytes.Add((byte) 102);
        this.WriteDoubleString((double) f);
      }
    }

    private void WriteFloat(double f)
    {
      if (this._version > 1)
      {
        this._bytes.Add((byte) 103);
        this._bytes.AddRange((IEnumerable<byte>) BitConverter.GetBytes(f));
      }
      else
      {
        this._bytes.Add((byte) 102);
        this.WriteDoubleString(f);
      }
    }

    private void WriteDoubleString(double d)
    {
      string str = DoubleOps.__repr__(DefaultContext.Default, d);
      this._bytes.Add((byte) str.Length);
      for (int index = 0; index < str.Length; ++index)
        this._bytes.Add((byte) str[index]);
    }

    private void WriteInteger(BigInteger val)
    {
      this._bytes.Add((byte) 108);
      int val1 = 0;
      int num1;
      if (val < BigInteger.Zero)
      {
        val *= (BigInteger) -1;
        num1 = -1;
      }
      else
        num1 = 1;
      List<byte> collection = new List<byte>();
      while (val != BigInteger.Zero)
      {
        int num2 = (int) (val & (BigInteger) (int) short.MaxValue);
        val >>= 15;
        collection.Add((byte) (num2 & (int) byte.MaxValue));
        collection.Add((byte) (num2 >> 8 & (int) byte.MaxValue));
        val1 += num1;
      }
      this.WriteInt32(val1);
      this._bytes.AddRange((IEnumerable<byte>) collection);
    }

    private void WriteBuffer(PythonBuffer b)
    {
      this._bytes.Add((byte) 115);
      List<byte> collection = new List<byte>();
      for (int s1 = 0; s1 < b.Size; ++s1)
      {
        if (b[(object) s1] is string)
        {
          string s2 = b[(object) s1] as string;
          byte[] bytes1 = Encoding.UTF8.GetBytes(s2);
          if (bytes1.Length != s2.Length)
          {
            collection.AddRange((IEnumerable<byte>) bytes1);
          }
          else
          {
            byte[] bytes2 = PythonAsciiEncoding.Instance.GetBytes(s2);
            collection.AddRange((IEnumerable<byte>) bytes2);
          }
        }
        else
          collection.Add((byte) b[(object) s1]);
      }
      this.WriteInt32(collection.Count);
      this._bytes.AddRange((IEnumerable<byte>) collection);
    }

    private void WriteLong(long l)
    {
      this._bytes.Add((byte) 73);
      for (int index = 0; index < 8; ++index)
      {
        this._bytes.Add((byte) ((ulong) l & (ulong) byte.MaxValue));
        l >>= 8;
      }
    }

    private void WriteComplex(Complex val)
    {
      this._bytes.Add((byte) 120);
      this.WriteDoubleString(val.Real);
      this.WriteDoubleString(val.Imaginary());
    }

    private void WriteStopIteration() => this._bytes.Add((byte) 83);

    private void WriteInt(int val)
    {
      this._bytes.Add((byte) 105);
      this.WriteInt32(val);
    }

    private void WriteInt32(int val)
    {
      this._bytes.Add((byte) (val & (int) byte.MaxValue));
      this._bytes.Add((byte) (val >> 8 & (int) byte.MaxValue));
      this._bytes.Add((byte) (val >> 16 /*0x10*/ & (int) byte.MaxValue));
      this._bytes.Add((byte) (val >> 24 & (int) byte.MaxValue));
    }

    private void WriteString(string s)
    {
      byte[] bytes1 = Encoding.UTF8.GetBytes(s);
      if (bytes1.Length != s.Length)
      {
        this._bytes.Add((byte) 117);
        this.WriteInt32(bytes1.Length);
        for (int index = 0; index < bytes1.Length; ++index)
          this._bytes.Add(bytes1[index]);
      }
      else
      {
        int val;
        if (this._strings != null && this._strings.TryGetValue(s, out val))
        {
          this._bytes.Add((byte) 82);
          this.WriteInt32(val);
        }
        else
        {
          byte[] bytes2 = PythonAsciiEncoding.Instance.GetBytes(s);
          if (this._strings != null)
            this._bytes.Add((byte) 116);
          else
            this._bytes.Add((byte) 115);
          this.WriteInt32(bytes2.Length);
          for (int index = 0; index < bytes2.Length; ++index)
            this._bytes.Add(bytes2[index]);
          if (this._strings == null)
            return;
          this._strings[s] = this._strings.Count;
        }
      }
    }

    private void WriteList(object o)
    {
      List list = o as List;
      this._bytes.Add((byte) 91);
      this.WriteInt32(list.__len__());
      for (int index = 0; index < list.__len__(); ++index)
        this.WriteObject(list[index]);
    }

    private void WriteDict(object o)
    {
      PythonDictionary pythonDictionary = o as PythonDictionary;
      this._bytes.Add((byte) 123);
      IEnumerator<KeyValuePair<object, object>> enumerator = pythonDictionary.GetEnumerator();
      while (enumerator.MoveNext())
      {
        this.WriteObject(enumerator.Current.Key);
        this.WriteObject(enumerator.Current.Value);
      }
      this._bytes.Add((byte) 48 /*0x30*/);
    }

    private void WriteTuple(object o)
    {
      PythonTuple pythonTuple = o as PythonTuple;
      this._bytes.Add((byte) 40);
      this.WriteInt32(pythonTuple.__len__());
      for (int index = 0; index < pythonTuple.__len__(); ++index)
        this.WriteObject(pythonTuple[index]);
    }

    private void WriteSet(object set)
    {
      SetCollection setCollection = set as SetCollection;
      this._bytes.Add((byte) 60);
      this.WriteInt32(setCollection.__len__());
      foreach (object o in (IEnumerable<object>) setCollection)
        this.WriteObject(o);
    }

    private void WriteFrozenSet(object set)
    {
      FrozenSetCollection frozenSetCollection = set as FrozenSetCollection;
      this._bytes.Add((byte) 62);
      this.WriteInt32(frozenSetCollection.__len__());
      foreach (object o in (IEnumerable<object>) frozenSetCollection)
        this.WriteObject(o);
    }

    public byte[] GetBytes() => this._bytes.ToArray();
  }

  private class MarshalReader
  {
    private IEnumerator<byte> _myBytes;
    private Stack<MarshalOps.MarshalReader.ProcStack> _stack;
    private readonly Dictionary<int, string> _strings;
    private object _result;

    public MarshalReader(IEnumerator<byte> bytes)
    {
      this._myBytes = bytes;
      this._strings = new Dictionary<int, string>();
    }

    public object ReadObject()
    {
      while (this._myBytes.MoveNext())
      {
        switch ((char) this._myBytes.Current)
        {
          case '(':
            this.PushStack(MarshalOps.MarshalReader.StackType.Tuple);
            break;
          case '0':
            if (this._stack == null || this._stack.Count == 0)
              throw PythonOps.ValueError("bad marshal data");
            this._stack.Peek().StackCount = 0;
            break;
          case '<':
            this.PushStack(MarshalOps.MarshalReader.StackType.Set);
            break;
          case '>':
            this.PushStack(MarshalOps.MarshalReader.StackType.FrozenSet);
            break;
          case '[':
            this.PushStack(MarshalOps.MarshalReader.StackType.List);
            break;
          case '{':
            this.PushStack(MarshalOps.MarshalReader.StackType.Dict);
            break;
          default:
            object res = this.YieldSimple();
            if (this._stack == null)
              return res;
            do
            {
              res = this.UpdateStack(res);
            }
            while (res != null && this._stack.Count > 0);
            if (this._stack.Count == 0)
              return this._result;
            continue;
        }
        if (this._stack != null && this._stack.Count > 0 && this._stack.Peek().StackCount == 0)
        {
          MarshalOps.MarshalReader.ProcStack procStack = this._stack.Pop();
          object obj = procStack.StackObj;
          if (procStack.StackType == MarshalOps.MarshalReader.StackType.Tuple)
            obj = (object) PythonTuple.Make(obj);
          else if (procStack.StackType == MarshalOps.MarshalReader.StackType.FrozenSet)
            obj = (object) FrozenSetCollection.Make(TypeCache.FrozenSet, obj);
          if (this._stack.Count > 0)
          {
            do
            {
              obj = this.UpdateStack(obj);
            }
            while (obj != null && this._stack.Count > 0);
            if (this._stack.Count == 0)
              break;
          }
          else
          {
            this._result = obj;
            break;
          }
        }
      }
      return this._result;
    }

    private void PushStack(MarshalOps.MarshalReader.StackType type)
    {
      MarshalOps.MarshalReader.ProcStack procStack = new MarshalOps.MarshalReader.ProcStack();
      procStack.StackType = type;
      switch (type)
      {
        case MarshalOps.MarshalReader.StackType.Tuple:
          procStack.StackCount = this.ReadInt32();
          procStack.StackObj = (object) new List<object>(procStack.StackCount);
          break;
        case MarshalOps.MarshalReader.StackType.Dict:
          procStack.StackObj = (object) new PythonDictionary();
          procStack.StackCount = -1;
          break;
        case MarshalOps.MarshalReader.StackType.List:
          procStack.StackObj = (object) new List();
          procStack.StackCount = this.ReadInt32();
          break;
        case MarshalOps.MarshalReader.StackType.Set:
          procStack.StackObj = (object) new SetCollection();
          procStack.StackCount = this.ReadInt32();
          break;
        case MarshalOps.MarshalReader.StackType.FrozenSet:
          procStack.StackCount = this.ReadInt32();
          procStack.StackObj = (object) new List<object>(procStack.StackCount);
          break;
      }
      if (this._stack == null)
        this._stack = new Stack<MarshalOps.MarshalReader.ProcStack>();
      this._stack.Push(procStack);
    }

    private object UpdateStack(object res)
    {
      MarshalOps.MarshalReader.ProcStack procStack = this._stack.Peek();
      switch (procStack.StackType)
      {
        case MarshalOps.MarshalReader.StackType.Tuple:
          List<object> stackObj1 = procStack.StackObj as List<object>;
          stackObj1.Add(res);
          --procStack.StackCount;
          if (procStack.StackCount == 0)
          {
            this._stack.Pop();
            object obj = (object) PythonTuple.Make((object) stackObj1);
            if (this._stack.Count == 0)
              this._result = obj;
            return obj;
          }
          break;
        case MarshalOps.MarshalReader.StackType.Dict:
          PythonDictionary stackObj2 = procStack.StackObj as PythonDictionary;
          if (procStack.HaveKey)
          {
            stackObj2[procStack.Key] = res;
            procStack.HaveKey = false;
            break;
          }
          procStack.HaveKey = true;
          procStack.Key = res;
          break;
        case MarshalOps.MarshalReader.StackType.List:
          List stackObj3 = procStack.StackObj as List;
          stackObj3.AddNoLock(res);
          --procStack.StackCount;
          if (procStack.StackCount == 0)
          {
            this._stack.Pop();
            if (this._stack.Count == 0)
              this._result = (object) stackObj3;
            return (object) stackObj3;
          }
          break;
        case MarshalOps.MarshalReader.StackType.Set:
          SetCollection stackObj4 = procStack.StackObj as SetCollection;
          stackObj4.add(res);
          --procStack.StackCount;
          if (procStack.StackCount == 0)
          {
            this._stack.Pop();
            if (this._stack.Count == 0)
              this._result = (object) stackObj4;
            return (object) stackObj4;
          }
          break;
        case MarshalOps.MarshalReader.StackType.FrozenSet:
          List<object> stackObj5 = procStack.StackObj as List<object>;
          stackObj5.Add(res);
          --procStack.StackCount;
          if (procStack.StackCount == 0)
          {
            this._stack.Pop();
            object obj = (object) FrozenSetCollection.Make(TypeCache.FrozenSet, (object) stackObj5);
            if (this._stack.Count == 0)
              this._result = obj;
            return obj;
          }
          break;
      }
      return (object) null;
    }

    private object YieldSimple()
    {
      switch ((char) this._myBytes.Current)
      {
        case 'F':
          return ScriptingRuntimeHelpers.False;
        case 'I':
          return this.ReadLong();
        case 'N':
          return (object) null;
        case 'R':
          return (object) this._strings[this.ReadInt32()];
        case 'S':
          return (object) PythonExceptions.StopIteration;
        case 'T':
          return ScriptingRuntimeHelpers.True;
        case 'f':
          return this.ReadFloat();
        case 'g':
          return this.ReadBinaryFloat();
        case 'i':
          return this.ReadInt();
        case 'l':
          return this.ReadBigInteger();
        case 's':
          return this.ReadBuffer();
        case 't':
          return this.ReadAsciiString();
        case 'u':
          return this.ReadUnicodeString();
        case 'x':
          return this.ReadComplex();
        default:
          throw PythonOps.ValueError("bad marshal data");
      }
    }

    private byte[] ReadBytes(int len)
    {
      byte[] numArray = new byte[len];
      for (int index = 0; index < len; ++index)
        numArray[index] = this._myBytes.MoveNext() ? this._myBytes.Current : throw PythonOps.ValueError("bad marshal data");
      return numArray;
    }

    private int ReadInt32()
    {
      byte[] numArray = this.ReadBytes(4);
      return (int) numArray[0] | (int) numArray[1] << 8 | (int) numArray[2] << 16 /*0x10*/ | (int) numArray[3] << 24;
    }

    private double ReadFloatStr()
    {
      this.MoveNext();
      string s = this.DecodeString(PythonAsciiEncoding.Instance, this.ReadBytes((int) this._myBytes.Current));
      double num = 0.0;
      ref double local = ref num;
      return double.TryParse(s, out local) ? num : 0.0;
    }

    private void MoveNext()
    {
      if (!this._myBytes.MoveNext())
        throw PythonOps.EofError("EOF read where object expected");
    }

    private string DecodeString(Encoding enc, byte[] bytes)
    {
      return enc.GetString(bytes, 0, bytes.Length);
    }

    private object ReadInt()
    {
      return ScriptingRuntimeHelpers.Int32ToObject(BitConverter.ToInt32(new byte[4]
      {
        this.ReadIntPart(),
        this.ReadIntPart(),
        this.ReadIntPart(),
        this.ReadIntPart()
      }, 0));
    }

    private byte ReadIntPart() => !this._myBytes.MoveNext() ? byte.MaxValue : this._myBytes.Current;

    private object ReadFloat() => (object) this.ReadFloatStr();

    private object ReadBinaryFloat() => (object) BitConverter.ToDouble(this.ReadBytes(8), 0);

    private object ReadAsciiString()
    {
      string str = this.DecodeString(PythonAsciiEncoding.Instance, this.ReadBytes(this.ReadInt32()));
      this._strings[this._strings.Count] = str;
      return (object) str;
    }

    private object ReadUnicodeString()
    {
      return (object) this.DecodeString(Encoding.UTF8, this.ReadBytes(this.ReadInt32()));
    }

    private object ReadComplex() => (object) new Complex(this.ReadFloatStr(), this.ReadFloatStr());

    private object ReadBuffer()
    {
      return (object) this.DecodeString(Encoding.UTF8, this.ReadBytes(this.ReadInt32()));
    }

    private object ReadLong()
    {
      byte[] numArray = this.ReadBytes(8);
      long num = 0;
      for (int index = 0; index < 8; ++index)
        num |= (long) numArray[index] << index * 8;
      return (object) num;
    }

    private object ReadBigInteger()
    {
      int num1 = this.ReadInt32();
      if (num1 == 0)
        return (object) BigInteger.Zero;
      int num2 = 1;
      if (num1 < 0)
      {
        num2 = -1;
        num1 *= -1;
      }
      int len = num1 * 2;
      byte[] numArray1 = this.ReadBytes(len);
      byte[] numArray2 = new byte[numArray1.Length];
      int index1 = 0;
      int index2 = 0;
      int num3 = 0;
      int index3;
      for (; index1 < len; index1 = index3 + 1)
      {
        numArray2[index2] = num3 != 0 ? (byte) ((int) numArray1[index1] >> num3 | (int) numArray1[index1 + 1] << 8 - num3) : numArray1[index1];
        ++index2;
        index3 = index1 + 1;
        if (num3 == 7)
        {
          num3 = 0;
        }
        else
        {
          numArray2[index2] = index3 >= len - 1 ? (byte) ((uint) numArray1[index3] >> num3) : (byte) ((int) numArray1[index3] >> num3 | (int) numArray1[index3 + 1] << 7 - num3);
          ++index2;
          ++num3;
        }
      }
      BigInteger bigInteger = new BigInteger(numArray2);
      return (object) (num2 < 0 ? -bigInteger : bigInteger);
    }

    private enum StackType
    {
      Tuple,
      Dict,
      List,
      Set,
      FrozenSet,
    }

    private class ProcStack
    {
      public MarshalOps.MarshalReader.StackType StackType;
      public object StackObj;
      public int StackCount;
      public bool HaveKey;
      public object Key;
    }
  }
}
