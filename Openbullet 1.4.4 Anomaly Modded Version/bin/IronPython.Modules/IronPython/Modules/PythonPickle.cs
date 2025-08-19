// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonPickle
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace IronPython.Modules;

public static class PythonPickle
{
  public const string __doc__ = "Fast object serialization/deserialization.\n\nDifferences from CPython:\n - does not implement the undocumented fast mode\n";
  private static readonly PythonStruct.Struct _float64 = PythonStruct.Struct.Create(">d");
  private const int highestProtocol = 2;
  public const string __version__ = "1.71";
  public const string format_version = "2.0";
  private const string Newline = "\n";

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    context.EnsureModuleException((object) "PickleError", dict, "PickleError", "cPickle");
    context.EnsureModuleException((object) "PicklingError", dict, "PicklingError", "cPickle");
    context.EnsureModuleException((object) "UnpicklingError", dict, "UnpicklingError", "cPickle");
    context.EnsureModuleException((object) "UnpickleableError", dict, "UnpickleableError", "cPickle");
    context.EnsureModuleException((object) "BadPickleGet", dict, "BadPickleGet", "cPickle");
    dict[(object) "__builtins__"] = (object) context.BuiltinModuleInstance;
    dict[(object) "compatible_formats"] = (object) PythonOps.MakeList((object) "1.0", (object) "1.1", (object) "1.2", (object) "1.3", (object) "2.0");
  }

  public static int HIGHEST_PROTOCOL => 2;

  [Documentation("dump(obj, file, protocol=0) -> None\n\nPickle obj and write the result to file.\n\nSee documentation for Pickler() for a description the file, protocol, and\n(deprecated) bin parameters.")]
  public static void dump(
    CodeContext context,
    object obj,
    object file,
    object protocol = null,
    object bin = null)
  {
    new PythonPickle.PicklerObject(context, file, protocol, bin).dump(context, obj);
  }

  [Documentation("dumps(obj, protocol=0) -> pickle string\n\nPickle obj and return the result as a string.\n\nSee the documentation for Pickler() for a description of the protocol and\n(deprecated) bin parameters.")]
  public static string dumps(CodeContext context, object obj, object protocol = null, object bin = null)
  {
    PythonPickle.StringBuilderOutput file = new PythonPickle.StringBuilderOutput();
    new PythonPickle.PicklerObject(context, (object) file, protocol, bin).dump(context, obj);
    return file.GetString();
  }

  [Documentation("load(file) -> unpickled object\n\nRead pickle data from the open file object and return the corresponding\nunpickled object. Data after the first pickle found is ignored, but the file\ncursor is not reset, so if a file objects contains multiple pickles, then\nload() may be called multiple times to unpickle them.\n\nfile: an object (such as an open file or a StringIO) with read(num_chars) and\n    readline() methods that return strings\n\nload() automatically determines if the pickle data was written in binary or\ntext mode.")]
  public static object load(CodeContext context, object file)
  {
    return new PythonPickle.UnpicklerObject(context, file).load(context);
  }

  [Documentation("loads(string) -> unpickled object\n\nRead a pickle object from a string, unpickle it, and return the resulting\nreconstructed object. Characters in the string beyond the end of the first\npickle are ignored.")]
  public static object loads(CodeContext context, [BytesConversion] string @string)
  {
    return new PythonPickle.UnpicklerObject(context, (PythonPickle.FileInput) new PythonPickle.PythonStringInput(@string)).load(context);
  }

  public static PythonPickle.PicklerObject Pickler(
    CodeContext context,
    object file = null,
    object protocol = null,
    object bin = null)
  {
    return new PythonPickle.PicklerObject(context, file, protocol, bin);
  }

  public static PythonPickle.UnpicklerObject Unpickler(CodeContext context, object file)
  {
    return new PythonPickle.UnpicklerObject(context, file);
  }

  private static PythonType PicklingError(CodeContext context)
  {
    return (PythonType) context.LanguageContext.GetModuleState((object) nameof (PicklingError));
  }

  private static PythonType PickleError(CodeContext context)
  {
    return (PythonType) context.LanguageContext.GetModuleState((object) nameof (PickleError));
  }

  private static PythonType UnpicklingError(CodeContext context)
  {
    return (PythonType) context.LanguageContext.GetModuleState((object) nameof (UnpicklingError));
  }

  private static PythonType BadPickleGet(CodeContext context)
  {
    return (PythonType) context.LanguageContext.GetModuleState((object) nameof (BadPickleGet));
  }

  internal abstract class FileInput
  {
    public abstract string Read(CodeContext context, int size);

    public abstract string ReadLine(CodeContext context);

    public virtual string ReadLineNoNewLine(CodeContext context)
    {
      string str = this.ReadLine(context);
      return str.Substring(0, str.Length - 1);
    }

    public virtual char ReadChar(CodeContext context)
    {
      string str = this.Read(context, 1);
      return str.Length >= 1 ? str[0] : throw PythonOps.EofError("unexpected EOF while unpickling");
    }

    public virtual int ReadInt(CodeContext context)
    {
      return (int) this.ReadChar(context) | (int) this.ReadChar(context) << 8 | (int) this.ReadChar(context) << 16 /*0x10*/ | (int) this.ReadChar(context) << 24;
    }
  }

  internal abstract class FileOutput
  {
    private readonly char[] int32chars = new char[4];

    public abstract void Write(CodeContext context, string data);

    public virtual void Write(CodeContext context, int data)
    {
      this.int32chars[0] = (char) (data & (int) byte.MaxValue);
      this.int32chars[1] = (char) (data >> 8 & (int) byte.MaxValue);
      this.int32chars[2] = (char) (data >> 16 /*0x10*/ & (int) byte.MaxValue);
      this.int32chars[3] = (char) (data >> 24 & (int) byte.MaxValue);
      this.Write(context, new string(this.int32chars));
    }

    public virtual void Write(CodeContext context, char data)
    {
      this.Write(context, ScriptingRuntimeHelpers.CharToString(data));
    }
  }

  private class PythonFileInput : PythonPickle.FileInput
  {
    private object _readMethod;
    private object _readLineMethod;

    public PythonFileInput(CodeContext context, object file)
    {
      if (!PythonOps.TryGetBoundAttr(context, file, "read", out this._readMethod) || !PythonOps.IsCallable(context, this._readMethod) || !PythonOps.TryGetBoundAttr(context, file, "readline", out this._readLineMethod) || !PythonOps.IsCallable(context, this._readLineMethod))
        throw PythonOps.TypeError("argument must have callable 'read' and 'readline' attributes");
    }

    public override string Read(CodeContext context, int size)
    {
      return Converter.ConvertToString(PythonCalls.Call(context, this._readMethod, (object) size));
    }

    public override string ReadLine(CodeContext context)
    {
      return Converter.ConvertToString(PythonCalls.Call(context, this._readLineMethod));
    }
  }

  internal class PythonStringInput : PythonPickle.FileInput
  {
    private readonly string _data;
    private int _offset;

    public PythonStringInput(string data) => this._data = data;

    public override string Read(CodeContext context, int size)
    {
      string str = this._data.Substring(this._offset, size);
      this._offset += size;
      return str;
    }

    public override string ReadLine(CodeContext context) => this.ReadLineWorker(true);

    public override string ReadLineNoNewLine(CodeContext context) => this.ReadLineWorker(false);

    public override char ReadChar(CodeContext context)
    {
      if (this._offset < this._data.Length)
        return this._data[this._offset++];
      throw PythonOps.EofError("unexpected EOF while unpickling");
    }

    public override int ReadInt(CodeContext context)
    {
      if (this._offset + 4 > this._data.Length)
        throw PythonOps.EofError("unexpected EOF while unpickling");
      int num = (int) this._data[this._offset] | (int) this._data[this._offset + 1] << 8 | (int) this._data[this._offset + 2] << 16 /*0x10*/ | (int) this._data[this._offset + 3] << 24;
      this._offset += 4;
      return num;
    }

    private string ReadLineWorker(bool includeNewLine)
    {
      for (int offset = this._offset; offset < this._data.Length; ++offset)
      {
        if (this._data[offset] == '\n')
        {
          string str = this._data.Substring(this._offset, offset - this._offset + (includeNewLine ? 1 : 0));
          this._offset = offset + 1;
          return str;
        }
      }
      string str1 = this._data.Substring(this._offset);
      this._offset = this._data.Length;
      return str1;
    }
  }

  private class PythonFileLikeOutput : PythonPickle.FileOutput
  {
    private object _writeMethod;

    public PythonFileLikeOutput(CodeContext context, object file)
    {
      if (!PythonOps.TryGetBoundAttr(context, file, "write", out this._writeMethod) || !PythonOps.IsCallable(context, this._writeMethod))
        throw PythonOps.TypeError("argument must have callable 'write' attribute");
    }

    public override void Write(CodeContext context, string data)
    {
      PythonCalls.Call(context, this._writeMethod, (object) data);
    }
  }

  private class PythonFileOutput : PythonPickle.FileOutput
  {
    private readonly PythonFile _file;

    public PythonFileOutput(PythonFile file) => this._file = file;

    public override void Write(CodeContext context, string data) => this._file.write(data);
  }

  private class StringBuilderOutput : PythonPickle.FileOutput
  {
    private readonly StringBuilder _builder = new StringBuilder(4096 /*0x1000*/);

    public string GetString() => this._builder.ToString();

    public override void Write(CodeContext context, char data) => this._builder.Append(data);

    public override void Write(CodeContext context, int data)
    {
      this._builder.Append((char) (data & (int) byte.MaxValue));
      this._builder.Append((char) (data >> 8 & (int) byte.MaxValue));
      this._builder.Append((char) (data >> 16 /*0x10*/ & (int) byte.MaxValue));
      this._builder.Append((char) (data >> 24 & (int) byte.MaxValue));
    }

    public override void Write(CodeContext context, string data) => this._builder.Append(data);
  }

  private class PythonReadableFileOutput : PythonPickle.PythonFileLikeOutput
  {
    private object _getValueMethod;

    public PythonReadableFileOutput(CodeContext context, object file)
      : base(context, file)
    {
      if (!PythonOps.TryGetBoundAttr(context, file, "getvalue", out this._getValueMethod) || !PythonOps.IsCallable(context, this._getValueMethod))
        throw PythonOps.TypeError("argument must have callable 'getvalue' attribute");
    }

    public object GetValue(CodeContext context) => PythonCalls.Call(context, this._getValueMethod);
  }

  internal static class Opcode
  {
    public const char Append = 'a';
    public const char Appends = 'e';
    public const char BinFloat = 'G';
    public const char BinGet = 'h';
    public const char BinInt = 'J';
    public const char BinInt1 = 'K';
    public const char BinInt2 = 'M';
    public const char BinPersid = 'Q';
    public const char BinPut = 'q';
    public const char BinString = 'T';
    public const char BinUnicode = 'X';
    public const char Build = 'b';
    public const char Dict = 'd';
    public const char Dup = '2';
    public const char EmptyDict = '}';
    public const char EmptyList = ']';
    public const char EmptyTuple = ')';
    public const char Ext1 = '\u0082';
    public const char Ext2 = '\u0083';
    public const char Ext4 = '\u0084';
    public const char Float = 'F';
    public const char Get = 'g';
    public const char Global = 'c';
    public const char Inst = 'i';
    public const char Int = 'I';
    public const char List = 'l';
    public const char Long = 'L';
    public const char Long1 = '\u008A';
    public const char Long4 = '\u008B';
    public const char LongBinGet = 'j';
    public const char LongBinPut = 'r';
    public const char Mark = '(';
    public const char NewFalse = '\u0089';
    public const char NewObj = '\u0081';
    public const char NewTrue = '\u0088';
    public const char NoneValue = 'N';
    public const char Obj = 'o';
    public const char PersId = 'P';
    public const char Pop = '0';
    public const char PopMark = '1';
    public const char Proto = '\u0080';
    public const char Put = 'p';
    public const char Reduce = 'R';
    public const char SetItem = 's';
    public const char SetItems = 'u';
    public const char ShortBinstring = 'U';
    public const char Stop = '.';
    public const char String = 'S';
    public const char Tuple = 't';
    public const char Tuple1 = '\u0085';
    public const char Tuple2 = '\u0086';
    public const char Tuple3 = '\u0087';
    public const char Unicode = 'V';
  }

  [Documentation("Pickler(file, protocol=0) -> Pickler object\n\nA Pickler object serializes Python objects to a pickle bytecode stream, which\ncan then be converted back into equivalent objects using an Unpickler.\n\nfile: an object (such as an open file) that has a write(string) method.\nprotocol: if omitted, protocol 0 is used. If HIGHEST_PROTOCOL or a negative\n    number, the highest available protocol is used.\nbin: (deprecated; use protocol instead) for backwards compability, a 'bin'\n    keyword parameter is supported. When protocol is specified it is ignored.\n    If protocol is not specified, then protocol 0 is used if bin is false, and\n    protocol 1 is used if bin is true.")]
  [PythonType("Pickler")]
  [PythonHidden(new PlatformID[] {})]
  public class PicklerObject
  {
    private const char LowestPrintableChar = ' ';
    private const char HighestPrintableChar = '~';
    private static readonly Dictionary<Type, PythonPickle.PicklerObject.PickleFunction> _dispatchTable;
    private int _batchSize = 1000;
    private PythonPickle.FileOutput _file;
    private int _protocol;
    private PythonDictionary _memo;
    private Dictionary<object, int> _privMemo;
    private object _persist_id;
    private static readonly BigInteger MaxInt = new BigInteger(int.MaxValue);
    private static readonly BigInteger MinInt = new BigInteger(int.MinValue);

    static PicklerObject()
    {
      PythonPickle.PicklerObject._dispatchTable = new Dictionary<Type, PythonPickle.PicklerObject.PickleFunction>();
      PythonPickle.PicklerObject._dispatchTable[typeof (PythonDictionary)] = new PythonPickle.PicklerObject.PickleFunction(PythonPickle.PicklerObject.SaveDict);
      PythonPickle.PicklerObject._dispatchTable[typeof (PythonTuple)] = new PythonPickle.PicklerObject.PickleFunction(PythonPickle.PicklerObject.SaveTuple);
      PythonPickle.PicklerObject._dispatchTable[typeof (IronPython.Runtime.List)] = new PythonPickle.PicklerObject.PickleFunction(PythonPickle.PicklerObject.SaveList);
      PythonPickle.PicklerObject._dispatchTable[typeof (OldClass)] = new PythonPickle.PicklerObject.PickleFunction(PythonPickle.PicklerObject.SaveGlobal);
      PythonPickle.PicklerObject._dispatchTable[typeof (PythonFunction)] = new PythonPickle.PicklerObject.PickleFunction(PythonPickle.PicklerObject.SaveGlobal);
      PythonPickle.PicklerObject._dispatchTable[typeof (BuiltinFunction)] = new PythonPickle.PicklerObject.PickleFunction(PythonPickle.PicklerObject.SaveGlobal);
      PythonPickle.PicklerObject._dispatchTable[typeof (PythonType)] = new PythonPickle.PicklerObject.PickleFunction(PythonPickle.PicklerObject.SaveGlobal);
      PythonPickle.PicklerObject._dispatchTable[typeof (OldInstance)] = new PythonPickle.PicklerObject.PickleFunction(PythonPickle.PicklerObject.SaveInstance);
    }

    public PythonDictionary memo
    {
      get
      {
        if (this._memo == null)
        {
          PythonDictionary pythonDictionary = new PythonDictionary();
          foreach (KeyValuePair<object, int> keyValuePair in this._privMemo)
            pythonDictionary._storage.AddNoLock(ref pythonDictionary._storage, Builtin.id(keyValuePair.Key), (object) PythonTuple.MakeTuple((object) keyValuePair.Value, keyValuePair.Key));
          this._memo = pythonDictionary;
        }
        return this._memo;
      }
      set
      {
        this._memo = value;
        this._privMemo = (Dictionary<object, int>) null;
      }
    }

    public int proto
    {
      get => this._protocol;
      set => this._protocol = value;
    }

    public int _BATCHSIZE
    {
      get => this._batchSize;
      set => this._batchSize = value;
    }

    public object persistent_id
    {
      get => this._persist_id;
      set => this._persist_id = value;
    }

    public int binary
    {
      get => this._protocol != 0 ? 0 : 1;
      set => this._protocol = value;
    }

    public int fast
    {
      get => 0;
      set
      {
      }
    }

    public PicklerObject(CodeContext context, object file, object protocol, object bin)
    {
      if (file == null)
        this._file = (PythonPickle.FileOutput) new PythonPickle.PythonReadableFileOutput(context, (object) new PythonStringIO.StringO());
      else if (Converter.TryConvertToInt32(file, out int _))
      {
        this._file = (PythonPickle.FileOutput) new PythonPickle.PythonReadableFileOutput(context, (object) new PythonStringIO.StringO());
        protocol = file;
      }
      else
      {
        switch (file)
        {
          case PythonFile _:
            this._file = (PythonPickle.FileOutput) new PythonPickle.PythonFileOutput((PythonFile) file);
            break;
          case PythonPickle.FileOutput _:
            this._file = (PythonPickle.FileOutput) file;
            break;
          default:
            this._file = (PythonPickle.FileOutput) new PythonPickle.PythonFileLikeOutput(context, file);
            break;
        }
      }
      this._privMemo = new Dictionary<object, int>(256 /*0x0100*/, (IEqualityComparer<object>) PythonPickle.ReferenceEqualityComparer.Instance);
      if (protocol == null)
        protocol = (object) (PythonOps.IsTrue(bin) ? 1 : 0);
      int int32 = context.LanguageContext.ConvertToInt32(protocol);
      if (int32 > 2)
        throw PythonOps.ValueError("pickle protocol {0} asked for; the highest available protocol is {1}", (object) int32, (object) 2);
      if (int32 < 0)
        this._protocol = 2;
      else
        this._protocol = int32;
    }

    [Documentation("dump(obj) -> None\n\nPickle obj and write the result to the file object that was passed to the\nconstructor\n.\nNote that you may call dump() multiple times to pickle multiple objects. To\nunpickle the stream, you will need to call Unpickler's load() method a\ncorresponding number of times.\n\nThe first time a particular object is encountered, it will be pickled normally.\nIf the object is encountered again (in the same or a later dump() call), a\nreference to the previously generated value will be pickled. Unpickling will\nthen create multiple references to a single object.")]
    public void dump(CodeContext context, object obj)
    {
      if (this._protocol >= 2)
        this.WriteProto(context);
      this.Save(context, obj);
      this.Write(context, '.');
    }

    [Documentation("clear_memo() -> None\n\nClear the memo, which is used internally by the pickler to keep track of which\nobjects have already been pickled (so that shared or recursive objects are\npickled only once).")]
    public void clear_memo()
    {
      if (this._memo != null)
        this._memo.Clear();
      else
        this._privMemo.Clear();
    }

    private void Memoize(object obj)
    {
      if (this._memo != null)
      {
        if (this.MemoContains((object) PythonOps.Id(obj)))
          return;
        this._memo[(object) PythonOps.Id(obj)] = (object) PythonTuple.MakeTuple((object) this._memo.Count, obj);
      }
      else
      {
        if (this._privMemo.ContainsKey(obj))
          return;
        this._privMemo[obj] = this._privMemo.Count;
      }
    }

    private int MemoizeNew(object obj)
    {
      int count;
      if (this._memo != null)
        this._memo[(object) PythonOps.Id(obj)] = (object) PythonTuple.MakeTuple((object) (count = this._memo.Count), obj);
      else
        this._privMemo[obj] = count = this._privMemo.Count;
      return count;
    }

    private bool MemoContains(object obj)
    {
      return this._memo != null ? this._memo.Contains((object) PythonOps.Id(obj)) : this._privMemo.ContainsKey(obj);
    }

    private bool TryWriteFastGet(CodeContext context, object obj)
    {
      if (this._memo != null)
        return this.TryWriteSlowGet(context, obj);
      int index;
      if (!this._privMemo.TryGetValue(obj, out index))
        return false;
      this.WriteGetOrPut(context, true, index);
      return true;
    }

    private bool TryWriteSlowGet(CodeContext context, object obj)
    {
      object tup;
      if (!this._memo.TryGetValue(obj, out tup))
        return false;
      this.WriteGetOrPut(context, true, (PythonTuple) tup);
      return true;
    }

    [Documentation("getvalue() -> string\n\nReturn the value of the internal string. Raises PicklingError if a file object\nwas passed to this pickler's constructor.")]
    public object getvalue(CodeContext context)
    {
      return this._file is PythonPickle.PythonReadableFileOutput ? ((PythonPickle.PythonReadableFileOutput) this._file).GetValue(context) : throw PythonExceptions.CreateThrowable(PythonPickle.PicklingError(context), (object) "Attempt to getvalue() a non-list-based pickler");
    }

    private void Save(CodeContext context, object obj)
    {
      if (this._persist_id != null && this.TrySavePersistId(context, obj))
        return;
      switch (obj)
      {
        case null:
          PythonPickle.PicklerObject.SaveNone(this, context, obj);
          break;
        case int _:
          PythonPickle.PicklerObject.SaveInteger(this, context, obj);
          break;
        case BigInteger _:
          PythonPickle.PicklerObject.SaveLong(this, context, obj);
          break;
        case bool _:
          PythonPickle.PicklerObject.SaveBoolean(this, context, obj);
          break;
        case double _:
          PythonPickle.PicklerObject.SaveFloat(this, context, obj);
          break;
        default:
          if (this.TryWriteFastGet(context, obj))
            break;
          if (obj is string)
          {
            PythonPickle.PicklerObject.SaveUnicode(this, context, obj);
            break;
          }
          PythonPickle.PicklerObject.PickleFunction pickleFunction;
          if (!PythonPickle.PicklerObject._dispatchTable.TryGetValue(obj.GetType(), out pickleFunction))
            pickleFunction = !(obj is PythonType) ? new PythonPickle.PicklerObject.PickleFunction(this.SaveObject) : new PythonPickle.PicklerObject.PickleFunction(PythonPickle.PicklerObject.SaveGlobal);
          pickleFunction(this, context, obj);
          break;
      }
    }

    private bool TrySavePersistId(CodeContext context, object obj)
    {
      string res = Converter.ConvertToString(context.LanguageContext.CallSplat(this._persist_id, obj));
      if (res == null)
        return false;
      this.SavePersId(context, res);
      return true;
    }

    private void SavePersId(CodeContext context, string res)
    {
      if (this.binary != 0)
      {
        this.Save(context, (object) res);
        this.Write(context, 'Q');
      }
      else
      {
        this.Write(context, 'P');
        this.Write(context, res);
        this.Write(context, "\n");
      }
    }

    private static void SaveBoolean(
      PythonPickle.PicklerObject pickler,
      CodeContext context,
      object obj)
    {
      if (pickler._protocol < 2)
      {
        pickler.Write(context, 'I');
        pickler.Write(context, $"0{((bool) obj ? 1 : 0)}");
        pickler.Write(context, "\n");
      }
      else if ((bool) obj)
        pickler.Write(context, '\u0088');
      else
        pickler.Write(context, '\u0089');
    }

    private static void SaveDict(
      PythonPickle.PicklerObject pickler,
      CodeContext context,
      object obj)
    {
      int index = pickler.MemoizeNew(obj);
      if (pickler._protocol < 1)
      {
        pickler.Write(context, '(');
        pickler.Write(context, 'd');
      }
      else
        pickler.Write(context, '}');
      pickler.WritePut(context, index);
      pickler.BatchSetItems(context, (PythonDictionary) obj);
    }

    private static void SaveFloat(
      PythonPickle.PicklerObject pickler,
      CodeContext context,
      object obj)
    {
      if (pickler._protocol < 1)
      {
        pickler.Write(context, 'F');
        pickler.WriteFloatAsString(context, obj);
      }
      else
      {
        pickler.Write(context, 'G');
        pickler.WriteFloat64(context, obj);
      }
    }

    private static void SaveGlobal(
      PythonPickle.PicklerObject pickler,
      CodeContext context,
      object obj)
    {
      if (obj is PythonType pythonType)
      {
        pickler.SaveGlobalByName(context, obj, (object) pythonType.Name);
      }
      else
      {
        object ret;
        if (!PythonOps.TryGetBoundAttr(context, obj, "__name__", out ret))
          throw pickler.CannotPickle(context, obj, "could not determine its __name__");
        pickler.SaveGlobalByName(context, obj, ret);
      }
    }

    private void SaveGlobalByName(CodeContext context, object obj, object name)
    {
      object moduleForGlobal = this.FindModuleForGlobal(context, obj, name);
      if (this._protocol >= 2)
      {
        object obj1;
        if (PythonCopyReg.GetExtensionRegistry(context).TryGetValue((object) PythonTuple.MakeTuple(moduleForGlobal, name), out obj1))
        {
          if (PythonPickle.PicklerObject.IsUInt8(context, obj1))
          {
            this.Write(context, '\u0082');
            this.WriteUInt8(context, obj1);
            return;
          }
          if (PythonPickle.PicklerObject.IsUInt16(context, obj1))
          {
            this.Write(context, '\u0083');
            this.WriteUInt16(context, obj1);
            return;
          }
          if (!PythonPickle.PicklerObject.IsInt32(context, obj1))
            throw PythonOps.RuntimeError("unrecognized integer format");
          this.Write(context, '\u0084');
          this.WriteInt32(context, obj1);
          return;
        }
      }
      this.MemoizeNew(obj);
      this.Write(context, 'c');
      this.WriteStringPair(context, moduleForGlobal, name);
      this.WritePut(context, obj);
    }

    private static void SaveInstance(
      PythonPickle.PicklerObject pickler,
      CodeContext context,
      object obj)
    {
      pickler.Write(context, '(');
      object ret1;
      if (!PythonOps.TryGetBoundAttr(context, obj, "__class__", out ret1))
        throw pickler.CannotPickle(context, obj, "could not determine its __class__");
      if (pickler._protocol < 1)
      {
        object ret2;
        if (!PythonOps.TryGetBoundAttr(context, ret1, "__name__", out ret2))
          throw pickler.CannotPickle(context, obj, "its __class__ has no __name__");
        object moduleForGlobal = pickler.FindModuleForGlobal(context, ret1, ret2);
        pickler.MemoizeNew(obj);
        pickler.WriteInitArgs(context, obj);
        pickler.Write(context, 'i');
        pickler.WriteStringPair(context, moduleForGlobal, ret2);
      }
      else
      {
        pickler.Save(context, ret1);
        pickler.Memoize(obj);
        pickler.WriteInitArgs(context, obj);
        pickler.Write(context, 'o');
      }
      pickler.WritePut(context, obj);
      object ret3;
      if (PythonOps.TryGetBoundAttr(context, obj, "__getstate__", out ret3))
        pickler.Save(context, PythonCalls.Call(context, ret3));
      else
        pickler.Save(context, PythonOps.GetBoundAttr(context, obj, "__dict__"));
      pickler.Write(context, 'b');
    }

    private static void SaveInteger(
      PythonPickle.PicklerObject pickler,
      CodeContext context,
      object obj)
    {
      if (pickler._protocol < 1)
      {
        pickler.Write(context, 'I');
        pickler.WriteIntAsString(context, obj);
      }
      else if (PythonPickle.PicklerObject.IsUInt8(context, obj))
      {
        pickler.Write(context, 'K');
        pickler.WriteUInt8(context, obj);
      }
      else if (PythonPickle.PicklerObject.IsUInt16(context, obj))
      {
        pickler.Write(context, 'M');
        pickler.WriteUInt16(context, obj);
      }
      else
      {
        if (!PythonPickle.PicklerObject.IsInt32(context, obj))
          throw PythonOps.RuntimeError("unrecognized integer format");
        pickler.Write(context, 'J');
        pickler.WriteInt32(context, obj);
      }
    }

    private static void SaveList(
      PythonPickle.PicklerObject pickler,
      CodeContext context,
      object obj)
    {
      int index = pickler.MemoizeNew(obj);
      if (pickler._protocol < 1)
      {
        pickler.Write(context, '(');
        pickler.Write(context, 'l');
      }
      else
        pickler.Write(context, ']');
      pickler.WritePut(context, index);
      pickler.BatchAppends(context, ((IEnumerable) obj).GetEnumerator());
    }

    private static void SaveLong(
      PythonPickle.PicklerObject pickler,
      CodeContext context,
      object obj)
    {
      BigInteger self = (BigInteger) obj;
      if (pickler._protocol < 2)
      {
        pickler.Write(context, 'L');
        pickler.WriteLongAsString(context, obj);
      }
      else if (self.IsZero())
      {
        pickler.Write(context, '\u008A');
        pickler.WriteUInt8(context, 0);
      }
      else if (self <= PythonPickle.PicklerObject.MaxInt && self >= PythonPickle.PicklerObject.MinInt)
      {
        pickler.Write(context, '\u008A');
        int num = (int) self;
        if (PythonPickle.PicklerObject.IsInt8(num))
        {
          pickler.WriteUInt8(context, 1);
          pickler._file.Write(context, (char) (byte) num);
        }
        else if (PythonPickle.PicklerObject.IsInt16(num))
        {
          pickler.WriteUInt8(context, 2);
          pickler.WriteUInt8(context, num & (int) byte.MaxValue);
          pickler.WriteUInt8(context, num >> 8 & (int) byte.MaxValue);
        }
        else
        {
          pickler.WriteUInt8(context, 4);
          pickler.WriteInt32(context, num);
        }
      }
      else
      {
        byte[] byteArray = self.ToByteArray();
        if (byteArray.Length < 256 /*0x0100*/)
        {
          pickler.Write(context, '\u008A');
          pickler.WriteUInt8(context, byteArray.Length);
        }
        else
        {
          pickler.Write(context, '\u008B');
          pickler.WriteInt32(context, byteArray.Length);
        }
        foreach (byte num in byteArray)
          pickler.WriteUInt8(context, (int) num);
      }
    }

    private static void SaveNone(
      PythonPickle.PicklerObject pickler,
      CodeContext context,
      object obj)
    {
      pickler.Write(context, 'N');
    }

    private void SaveObject(PythonPickle.PicklerObject pickler, CodeContext context, object obj)
    {
      this.MemoizeNew(obj);
      PythonType pythonType = DynamicHelpers.GetPythonType(obj);
      object ret;
      object name;
      if (PythonCopyReg.GetDispatchTable(context).TryGetValue((object) pythonType, out ret))
        name = PythonCalls.Call(context, ret, obj);
      else if (PythonOps.TryGetBoundAttr(context, obj, "__reduce_ex__", out ret))
        name = !(obj is PythonType) ? context.LanguageContext.Call(context, ret, (object) this._protocol) : context.LanguageContext.Call(context, ret, obj, (object) this._protocol);
      else if (PythonOps.TryGetBoundAttr(context, obj, "__reduce__", out ret))
        name = !(obj is PythonType) ? context.LanguageContext.Call(context, ret) : context.LanguageContext.Call(context, ret, obj);
      else
        throw PythonOps.AttributeError("no reduce function found for {0}", obj);
      if (pythonType.Equals((object) TypeCache.String))
      {
        if (this.TryWriteFastGet(context, obj))
          return;
        this.SaveGlobalByName(context, obj, name);
      }
      else
      {
        PythonTuple pythonTuple = name is PythonTuple ? (PythonTuple) name : throw this.CannotPickle(context, obj, "{0} must return string or tuple", ret);
        switch (pythonTuple.__len__())
        {
          case 2:
            this.SaveReduce(context, obj, ret, pythonTuple[0], pythonTuple[1], (object) null, (object) null, (object) null);
            break;
          case 3:
            this.SaveReduce(context, obj, ret, pythonTuple[0], pythonTuple[1], pythonTuple[2], (object) null, (object) null);
            break;
          case 4:
            this.SaveReduce(context, obj, ret, pythonTuple[0], pythonTuple[1], pythonTuple[2], pythonTuple[3], (object) null);
            break;
          case 5:
            this.SaveReduce(context, obj, ret, pythonTuple[0], pythonTuple[1], pythonTuple[2], pythonTuple[3], pythonTuple[4]);
            break;
          default:
            throw this.CannotPickle(context, obj, "tuple returned by {0} must have to to five elements", ret);
        }
      }
    }

    private void SaveReduce(
      CodeContext context,
      object obj,
      object reduceCallable,
      object func,
      object args,
      object state,
      object listItems,
      object dictItems)
    {
      if (!PythonOps.IsCallable(context, func))
        throw this.CannotPickle(context, obj, "func from reduce() should be callable");
      switch (args)
      {
        case PythonTuple _:
        case null:
          switch (listItems)
          {
            case null:
            case IEnumerator _:
              switch (dictItems)
              {
                case null:
                case IEnumerator _:
                  string result;
                  if (func is PythonType)
                  {
                    result = ((PythonType) func).Name;
                  }
                  else
                  {
                    object ret;
                    if (!PythonOps.TryGetBoundAttr(context, func, "__name__", out ret))
                      throw this.CannotPickle(context, obj, "func from reduce() ({0}) should have a __name__ attribute");
                    if (!Converter.TryConvertToString(ret, out result) || result == null)
                      throw this.CannotPickle(context, obj, "__name__ of func from reduce() must be string");
                  }
                  if (this._protocol >= 2 && "__newobj__" == result)
                  {
                    PythonTuple pythonTuple = args != null ? (PythonTuple) args : throw this.CannotPickle(context, obj, "__newobj__ arglist is None");
                    if (pythonTuple.__len__() == 0)
                      throw this.CannotPickle(context, obj, "__newobj__ arglist is empty");
                    if (!DynamicHelpers.GetPythonType(obj).Equals(pythonTuple[0]))
                      throw this.CannotPickle(context, obj, "args[0] from __newobj__ args has the wrong class");
                    this.Save(context, pythonTuple[0]);
                    this.Save(context, pythonTuple[new IronPython.Runtime.Slice((object) 1, (object) null)]);
                    this.Write(context, '\u0081');
                  }
                  else
                  {
                    this.Save(context, func);
                    this.Save(context, args);
                    this.Write(context, 'R');
                  }
                  this.WritePut(context, obj);
                  if (state != null)
                  {
                    this.Save(context, state);
                    this.Write(context, 'b');
                  }
                  if (listItems != null)
                    this.BatchAppends(context, (IEnumerator) listItems);
                  if (dictItems == null)
                    return;
                  this.BatchSetItems(context, (IEnumerator) dictItems);
                  return;
                default:
                  throw this.CannotPickle(context, obj, "dictitems from reduce() should be a dict iterator");
              }
            default:
              throw this.CannotPickle(context, obj, "listitems from reduce() should be a list iterator");
          }
        default:
          throw this.CannotPickle(context, obj, "args from reduce() should be a tuple");
      }
    }

    private static void SaveTuple(
      PythonPickle.PicklerObject pickler,
      CodeContext context,
      object obj)
    {
      PythonTuple pythonTuple = (PythonTuple) obj;
      bool flag = false;
      int length = pythonTuple._data.Length;
      char data;
      if (pickler._protocol > 0 && length == 0)
        data = ')';
      else if (pickler._protocol >= 2 && length == 1)
        data = '\u0085';
      else if (pickler._protocol >= 2 && length == 2)
        data = '\u0086';
      else if (pickler._protocol >= 2 && length == 3)
      {
        data = '\u0087';
      }
      else
      {
        data = 't';
        flag = true;
      }
      if (flag)
        pickler.Write(context, '(');
      foreach (object obj1 in pythonTuple._data)
        pickler.Save(context, obj1);
      if (length > 0)
      {
        if (pickler.MemoContains(obj))
        {
          if (pickler._protocol == 1)
          {
            pickler.Write(context, '1');
          }
          else
          {
            if (pickler._protocol == 0)
              pickler.Write(context, '0');
            for (int index = 0; index < length; ++index)
              pickler.Write(context, '0');
          }
          pickler.WriteGet(context, obj);
        }
        else
        {
          pickler.Write(context, data);
          pickler.Memoize((object) pythonTuple);
          pickler.WritePut(context, (object) pythonTuple);
        }
      }
      else
        pickler.Write(context, data);
    }

    private static void SaveUnicode(
      PythonPickle.PicklerObject pickler,
      CodeContext context,
      object obj)
    {
      if (pickler._memo != null)
      {
        pickler.MemoizeNew(obj);
        if (pickler._protocol < 1)
        {
          pickler.Write(context, 'V');
          pickler.WriteUnicodeStringRaw(context, obj);
        }
        else
        {
          pickler.Write(context, 'X');
          pickler.WriteUnicodeStringUtf8(context, obj);
        }
        pickler.WritePut(context, obj);
      }
      else
      {
        int index = pickler._privMemo[obj] = pickler._privMemo.Count;
        if (pickler._protocol < 1)
        {
          pickler.Write(context, 'V');
          pickler.WriteUnicodeStringRaw(context, obj);
        }
        else
        {
          pickler.Write(context, 'X');
          pickler.WriteUnicodeStringUtf8(context, obj);
        }
        pickler.WriteGetOrPut(context, false, index);
      }
    }

    private void WriteFloatAsString(CodeContext context, object value)
    {
      this.Write(context, DoubleOps.__repr__(context, (double) value));
      this.Write(context, "\n");
    }

    private void WriteFloat64(CodeContext context, object value)
    {
      this.Write(context, PythonPickle._float64.pack(context, value));
    }

    private void WriteUInt8(CodeContext context, object value)
    {
      switch (value)
      {
        case int num1:
          this.Write(context, ScriptingRuntimeHelpers.CharToString((char) num1));
          break;
        case BigInteger bigInteger:
          this.Write(context, ScriptingRuntimeHelpers.CharToString((char) (int) bigInteger));
          break;
        case byte num2:
          this.Write(context, ScriptingRuntimeHelpers.CharToString((char) num2));
          break;
        default:
          throw Microsoft.Scripting.Utils.Assert.Unreachable;
      }
    }

    private void WriteUInt8(CodeContext context, int value)
    {
      this._file.Write(context, (char) value);
    }

    private void WriteUInt16(CodeContext context, object value)
    {
      int num = (int) value;
      this.WriteUInt8(context, num & (int) byte.MaxValue);
      this.WriteUInt8(context, num >> 8 & (int) byte.MaxValue);
    }

    private void WriteInt32(CodeContext context, object value)
    {
      int val = (int) value;
      this.WriteInt32(context, val);
    }

    private void WriteInt32(CodeContext context, int val) => this._file.Write(context, val);

    private void WriteIntAsString(CodeContext context, object value)
    {
      this.Write(context, PythonOps.Repr(context, value));
      this.Write(context, "\n");
    }

    private void WriteIntAsString(CodeContext context, int value)
    {
      this.Write(context, value.ToString());
      this.Write(context, "\n");
    }

    private void WriteLongAsString(CodeContext context, object value)
    {
      this.Write(context, PythonOps.Repr(context, value));
      this.Write(context, "\n");
    }

    private void WriteUnicodeStringRaw(CodeContext context, object value)
    {
      this.Write(context, StringOps.RawUnicodeEscapeEncode(((string) value).Replace("\\", "\\u005c").Replace("\n", "\\u000a")));
      this.Write(context, "\n");
    }

    private void WriteUnicodeStringUtf8(CodeContext context, object value)
    {
      string data1 = (string) value;
      for (int index = 0; index < data1.Length; ++index)
      {
        if (data1[index] >= '\u0080')
        {
          string data2 = ((IList<byte>) Encoding.UTF8.GetBytes((string) value)).MakeString();
          this.WriteInt32(context, data2.Length);
          this.Write(context, data2);
          return;
        }
      }
      this.WriteInt32(context, data1.Length);
      this.Write(context, data1);
    }

    private void WriteStringPair(CodeContext context, object value1, object value2)
    {
      this.Write(context, (string) value1);
      this.Write(context, "\n");
      this.Write(context, (string) value2);
      this.Write(context, "\n");
    }

    private static bool IsUInt8(CodeContext context, object value)
    {
      if (value is int num)
        return PythonPickle.PicklerObject.IsUInt8(num);
      PythonContext languageContext = context.LanguageContext;
      return languageContext.LessThanOrEqual((object) 0, value) && languageContext.LessThan(value, (object) 256 /*0x0100*/);
    }

    private static bool IsUInt8(int value) => value >= 0 && value < 256 /*0x0100*/;

    private static bool IsInt8(int value)
    {
      return value >= (int) sbyte.MinValue && value <= (int) sbyte.MaxValue;
    }

    private static bool IsUInt16(CodeContext context, object value)
    {
      if (value is int num)
        return PythonPickle.PicklerObject.IsUInt16(num);
      PythonContext languageContext = context.LanguageContext;
      return languageContext.LessThanOrEqual((object) 256 /*0x0100*/, value) && languageContext.LessThan(value, (object) 65536 /*0x010000*/);
    }

    private static bool IsUInt16(int value) => value >= 0 && value < 65536 /*0x010000*/;

    private static bool IsInt16(int value)
    {
      return value >= (int) short.MinValue && value <= (int) short.MaxValue;
    }

    private static bool IsInt32(CodeContext context, object value)
    {
      PythonContext languageContext = context.LanguageContext;
      return languageContext.LessThanOrEqual((object) int.MinValue, value) && languageContext.LessThanOrEqual(value, (object) int.MaxValue);
    }

    private void Write(CodeContext context, string data) => this._file.Write(context, data);

    private void Write(CodeContext context, char data) => this._file.Write(context, data);

    private void WriteGet(CodeContext context, object obj)
    {
      this.WriteGetOrPut(context, obj, true);
    }

    private void WriteGetOrPut(CodeContext context, object obj, bool isGet)
    {
      if (this._memo == null)
        this.WriteGetOrPut(context, isGet, this._privMemo[obj]);
      else
        this.WriteGetOrPut(context, isGet, (PythonTuple) this._memo[(object) PythonOps.Id(obj)]);
    }

    private void WriteGetOrPut(CodeContext context, bool isGet, PythonTuple tup)
    {
      object obj = tup[0];
      if (this._protocol < 1)
      {
        this.Write(context, isGet ? 'g' : 'p');
        this.WriteIntAsString(context, obj);
      }
      else if (PythonPickle.PicklerObject.IsUInt8(context, obj))
      {
        this.Write(context, isGet ? 'h' : 'q');
        this.WriteUInt8(context, obj);
      }
      else
      {
        this.Write(context, isGet ? 'j' : 'r');
        this.WriteInt32(context, obj);
      }
    }

    private void WriteGetOrPut(CodeContext context, bool isGet, int index)
    {
      if (this._protocol < 1)
      {
        this.Write(context, isGet ? 'g' : 'p');
        this.WriteIntAsString(context, index);
      }
      else if (index >= 0 && index <= 256 /*0x0100*/)
      {
        this.Write(context, isGet ? 'h' : 'q');
        this.WriteUInt8(context, index);
      }
      else
      {
        this.Write(context, isGet ? 'j' : 'r');
        this.WriteInt32(context, index);
      }
    }

    private void WriteInitArgs(CodeContext context, object obj)
    {
      object ret;
      if (!PythonOps.TryGetBoundAttr(context, obj, "__getinitargs__", out ret))
        return;
      object obj1 = PythonCalls.Call(context, ret);
      if (!(obj1 is PythonTuple))
        throw this.CannotPickle(context, obj, "__getinitargs__() must return tuple");
      foreach (object obj2 in (PythonTuple) obj1)
        this.Save(context, obj2);
    }

    private void WritePut(CodeContext context, object obj)
    {
      this.WriteGetOrPut(context, obj, false);
    }

    private void WritePut(CodeContext context, int index)
    {
      this.WriteGetOrPut(context, false, index);
    }

    private void WriteProto(CodeContext context)
    {
      this.Write(context, '\u0080');
      this.WriteUInt8(context, this._protocol);
    }

    private void BatchAppends(CodeContext context, IEnumerator enumerator)
    {
      if (this._protocol < 1)
      {
        while (enumerator.MoveNext())
        {
          this.Save(context, enumerator.Current);
          this.Write(context, 'a');
        }
      }
      else
      {
        if (!enumerator.MoveNext())
          return;
        object current = enumerator.Current;
        int num = 0;
        while (enumerator.MoveNext())
        {
          object obj = current;
          current = enumerator.Current;
          if (num == this._BATCHSIZE)
          {
            this.Write(context, 'e');
            num = 0;
          }
          if (num == 0)
            this.Write(context, '(');
          this.Save(context, obj);
          ++num;
        }
        if (num == this._BATCHSIZE)
        {
          this.Write(context, 'e');
          num = 0;
        }
        this.Save(context, current);
        if (num + 1 > 1)
          this.Write(context, 'e');
        else
          this.Write(context, 'a');
      }
    }

    private void BatchSetItems(CodeContext context, PythonDictionary dict)
    {
      using (IEnumerator<KeyValuePair<object, object>> enumerator = dict._storage.GetEnumerator())
      {
        if (this._protocol < 1)
        {
          while (enumerator.MoveNext())
          {
            KeyValuePair<object, object> current = enumerator.Current;
            this.Save(context, current.Key);
            this.Save(context, current.Value);
            this.Write(context, 's');
          }
        }
        else
        {
          if (!enumerator.MoveNext())
            return;
          KeyValuePair<object, object> current = enumerator.Current;
          object key = current.Key;
          object obj1 = current.Value;
          int num = 0;
          while (enumerator.MoveNext())
          {
            object obj2 = key;
            object obj3 = obj1;
            current = enumerator.Current;
            key = current.Key;
            obj1 = current.Value;
            if (num == this._BATCHSIZE)
            {
              this.Write(context, 'u');
              num = 0;
            }
            if (num == 0)
              this.Write(context, '(');
            this.Save(context, obj2);
            this.Save(context, obj3);
            ++num;
          }
          if (num == this._BATCHSIZE)
          {
            this.Write(context, 'u');
            num = 0;
          }
          this.Save(context, key);
          this.Save(context, obj1);
          if (num + 1 > 1)
            this.Write(context, 'u');
          else
            this.Write(context, 's');
        }
      }
    }

    private void BatchSetItems(CodeContext context, IEnumerator enumerator)
    {
      if (this._protocol < 1)
      {
        while (enumerator.MoveNext())
        {
          PythonTuple current = (PythonTuple) enumerator.Current;
          this.Save(context, current[0]);
          this.Save(context, current[1]);
          this.Write(context, 's');
        }
      }
      else
      {
        if (!enumerator.MoveNext())
          return;
        PythonTuple current1 = (PythonTuple) enumerator.Current;
        object obj1 = current1[0];
        object obj2 = current1[1];
        int num = 0;
        while (enumerator.MoveNext())
        {
          object obj3 = obj1;
          object obj4 = obj2;
          PythonTuple current2 = (PythonTuple) enumerator.Current;
          obj1 = current2[0];
          obj2 = current2[1];
          if (num == this._BATCHSIZE)
          {
            this.Write(context, 'u');
            num = 0;
          }
          if (num == 0)
            this.Write(context, '(');
          this.Save(context, obj3);
          this.Save(context, obj4);
          ++num;
        }
        if (num == this._BATCHSIZE)
        {
          this.Write(context, 'u');
          num = 0;
        }
        this.Save(context, obj1);
        this.Save(context, obj2);
        if (num + 1 > 1)
          this.Write(context, 'u');
        else
          this.Write(context, 's');
      }
    }

    private Exception CannotPickle(
      CodeContext context,
      object obj,
      string format,
      params object[] args)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Can't pickle ");
      stringBuilder.Append(PythonOps.ToString(context, obj));
      if (format != null)
      {
        stringBuilder.Append(": ");
        stringBuilder.Append(string.Format(format, args));
      }
      return PythonExceptions.CreateThrowable(PythonPickle.PickleError(context), (object) stringBuilder.ToString());
    }

    private object FindModuleForGlobal(CodeContext context, object obj, object name)
    {
      if (obj is PythonType self)
        return PythonType.Get__module__(context, self);
      object ret1;
      if (PythonOps.TryGetBoundAttr(context, obj, "__module__", out ret1))
      {
        LightExceptions.CheckAndThrow(Builtin.__import__(context, Converter.ConvertToString(ret1)));
        object ret2;
        object ret3;
        if (Importer.TryGetExistingModule(context, Converter.ConvertToString(ret1), out ret2) && PythonOps.TryGetBoundAttr(context, ret2, Converter.ConvertToString(name), out ret3))
        {
          if (PythonOps.IsRetBool(ret3, obj))
            return ret1;
          throw this.CannotPickle(context, obj, "it's not the same object as {0}.{1}", ret1, name);
        }
        throw this.CannotPickle(context, obj, "it's not found as {0}.{1}", ret1, name);
      }
      foreach (KeyValuePair<object, object> systemStateModule in (IEnumerable<KeyValuePair<object, object>>) context.LanguageContext.SystemStateModules)
      {
        object key = systemStateModule.Key;
        object o = systemStateModule.Value;
        object ret4;
        if (PythonOps.TryGetBoundAttr(context, o, Converter.ConvertToString(name), out ret4) && PythonOps.IsRetBool(ret4, obj))
          return key;
      }
      throw this.CannotPickle(context, obj, "could not determine its module");
    }

    private delegate void PickleFunction(
      PythonPickle.PicklerObject pickler,
      CodeContext context,
      object value);
  }

  [Documentation("Unpickler(file) -> Unpickler object\n\nAn Unpickler object reads a pickle bytecode stream and creates corresponding\nobjects.\nfile: an object (such as an open file or a StringIO) with read(num_chars) and\n    readline() methods that return strings")]
  [PythonType("Unpickler")]
  [PythonHidden(new PlatformID[] {})]
  public class UnpicklerObject
  {
    private static readonly object _mark = new object();
    private PythonPickle.FileInput _file;
    private List<object> _stack;
    private PythonDictionary _memo;
    private List<object> _privMemo;
    private object _pers_loader;

    public UnpicklerObject() => this._privMemo = new List<object>(200);

    public UnpicklerObject(CodeContext context, object file)
      : this()
    {
      this._file = (PythonPickle.FileInput) new PythonPickle.PythonFileInput(context, file);
    }

    internal UnpicklerObject(CodeContext context, PythonPickle.FileInput input)
      : this()
    {
      this._file = input;
    }

    [Documentation("load() -> unpickled object\n\nRead pickle data from the file object that was passed to the constructor and\nreturn the corresponding unpickled objects.")]
    public object load(CodeContext context)
    {
      this._stack = new List<object>(32 /*0x20*/);
      char o;
      while (true)
      {
        o = this._file.ReadChar(context);
        switch (o)
        {
          case '(':
            this.LoadMark(context);
            continue;
          case ')':
            this.LoadEmptyTuple(context);
            continue;
          case '.':
            goto label_54;
          case '0':
            this.LoadPop(context);
            continue;
          case '1':
            this.LoadPopMark(context);
            continue;
          case '2':
            this.LoadDup(context);
            continue;
          case 'F':
            this.LoadFloat(context);
            continue;
          case 'G':
            this.LoadBinFloat(context);
            continue;
          case 'I':
            this.LoadInt(context);
            continue;
          case 'J':
            this.LoadBinInt(context);
            continue;
          case 'K':
            this.LoadBinInt1(context);
            continue;
          case 'L':
            this.LoadLong(context);
            continue;
          case 'M':
            this.LoadBinInt2(context);
            continue;
          case 'N':
            this.LoadNoneValue(context);
            continue;
          case 'P':
            this.LoadPersId(context);
            continue;
          case 'Q':
            this.LoadBinPersid(context);
            continue;
          case 'R':
            this.LoadReduce(context);
            continue;
          case 'S':
            this.LoadString(context);
            continue;
          case 'T':
            this.LoadBinString(context);
            continue;
          case 'U':
            this.LoadShortBinstring(context);
            continue;
          case 'V':
            this.LoadUnicode(context);
            continue;
          case 'X':
            this.LoadBinUnicode(context);
            continue;
          case ']':
            this.LoadEmptyList(context);
            continue;
          case 'a':
            this.LoadAppend(context);
            continue;
          case 'b':
            this.LoadBuild(context);
            continue;
          case 'c':
            this.LoadGlobal(context);
            continue;
          case 'd':
            this.LoadDict(context);
            continue;
          case 'e':
            this.LoadAppends(context);
            continue;
          case 'g':
            this.LoadGet(context);
            continue;
          case 'h':
            this.LoadBinGet(context);
            continue;
          case 'i':
            this.LoadInst(context);
            continue;
          case 'j':
            this.LoadLongBinGet(context);
            continue;
          case 'l':
            this.LoadList(context);
            continue;
          case 'o':
            this.LoadObj(context);
            continue;
          case 'p':
            this.LoadPut(context);
            continue;
          case 'q':
            this.LoadBinPut(context);
            continue;
          case 'r':
            this.LoadLongBinPut(context);
            continue;
          case 's':
            this.LoadSetItem(context);
            continue;
          case 't':
            this.LoadTuple(context);
            continue;
          case 'u':
            this.LoadSetItems(context);
            continue;
          case '}':
            this.LoadEmptyDict(context);
            continue;
          case '\u0080':
            this.LoadProto(context);
            continue;
          case '\u0081':
            this.LoadNewObj(context);
            continue;
          case '\u0082':
            this.LoadExt1(context);
            continue;
          case '\u0083':
            this.LoadExt2(context);
            continue;
          case '\u0084':
            this.LoadExt4(context);
            continue;
          case '\u0085':
            this.LoadTuple1(context);
            continue;
          case '\u0086':
            this.LoadTuple2(context);
            continue;
          case '\u0087':
            this.LoadTuple3(context);
            continue;
          case '\u0088':
            this.LoadNewTrue(context);
            continue;
          case '\u0089':
            this.LoadNewFalse(context);
            continue;
          case '\u008A':
            this.LoadLong1(context);
            continue;
          case '\u008B':
            this.LoadLong4(context);
            continue;
          default:
            goto label_55;
        }
      }
label_54:
      return this.PopStack();
label_55:
      throw this.CannotUnpickle(context, "invalid opcode: {0}", (object) PythonOps.Repr(context, (object) o));
    }

    private object PopStack()
    {
      object obj = this._stack[this._stack.Count - 1];
      this._stack.RemoveAt(this._stack.Count - 1);
      return obj;
    }

    private object PeekStack() => this._stack[this._stack.Count - 1];

    public object[] StackGetSliceAsArray(int start)
    {
      object[] sliceAsArray = new object[this._stack.Count - start];
      for (int index = 0; index < sliceAsArray.Length; ++index)
        sliceAsArray[index] = this._stack[index + start];
      return sliceAsArray;
    }

    [Documentation("noload() -> unpickled object\n\nLike load(), but don't import any modules or create create any instances of\nuser-defined types. (Builtin objects such as ints, tuples, etc. are created as\nwith load().)\n\nThis is primarily useful for scanning a pickle for persistent ids without\nincurring the overhead of completely unpickling an object. See the pickle\nmodule documentation for more information about persistent ids.")]
    public void noload(CodeContext context)
    {
      throw PythonOps.NotImplementedError("noload() is not implemented");
    }

    private Exception CannotUnpickle(CodeContext context, string format, params object[] args)
    {
      return PythonExceptions.CreateThrowable(PythonPickle.UnpicklingError(context), (object) string.Format(format, args));
    }

    public PythonDictionary memo
    {
      get
      {
        if (this._memo == null)
        {
          PythonDictionary pythonDictionary = new PythonDictionary();
          for (int index = 0; index < this._privMemo.Count; ++index)
          {
            if (this._privMemo[index] != PythonPickle.UnpicklerObject._mark)
              pythonDictionary[(object) index] = this._privMemo[index];
          }
          this._memo = pythonDictionary;
        }
        return this._memo;
      }
      set
      {
        this._memo = value;
        this._privMemo = (List<object>) null;
      }
    }

    public object persistent_load
    {
      get => this._pers_loader;
      set => this._pers_loader = value;
    }

    private object MemoGet(CodeContext context, int key)
    {
      if (this._memo != null)
      {
        object obj;
        if (this._memo.TryGetValue((object) key, out obj))
          return obj;
      }
      else
      {
        object obj;
        if (key < this._privMemo.Count && (obj = this._privMemo[key]) != PythonPickle.UnpicklerObject._mark)
          return obj;
      }
      throw PythonExceptions.CreateThrowable(PythonPickle.BadPickleGet(context), (object) $"memo key {key} not found");
    }

    private void MemoPut(int key, object value)
    {
      if (this._memo != null)
      {
        this._memo[(object) key] = value;
      }
      else
      {
        while (key >= this._privMemo.Count)
          this._privMemo.Add(PythonPickle.UnpicklerObject._mark);
        this._privMemo[key] = value;
      }
    }

    private int GetMarkIndex(CodeContext context)
    {
      int index = this._stack.Count - 1;
      while (index > 0 && this._stack[index] != PythonPickle.UnpicklerObject._mark)
        --index;
      return index != -1 ? index : throw this.CannotUnpickle(context, "mark not found");
    }

    private string Read(CodeContext context, int size)
    {
      string str = this._file.Read(context, size);
      if (str.Length < size)
        throw PythonOps.EofError("unexpected EOF while unpickling");
      return str;
    }

    private string ReadLineNoNewline(CodeContext context)
    {
      string str = this._file.ReadLine(context);
      return str.Substring(0, str.Length - 1);
    }

    private object ReadFloatString(CodeContext context)
    {
      return DoubleOps.__new__(context, TypeCache.Double, (object) this.ReadLineNoNewline(context));
    }

    private double ReadFloat64(CodeContext context)
    {
      int index = 0;
      return PythonStruct.CreateDoubleValue(context, ref index, false, this.Read(context, 8));
    }

    private object ReadIntFromString(CodeContext context)
    {
      string x = this.ReadLineNoNewline(context);
      if ("00" == x)
        return ScriptingRuntimeHelpers.False;
      return "01" == x ? ScriptingRuntimeHelpers.True : Int32Ops.__new__(context, TypeCache.Int32, (object) x);
    }

    private int ReadInt32(CodeContext context) => this._file.ReadInt(context);

    private object ReadLongFromString(CodeContext context)
    {
      return BigIntegerOps.__new__(context, TypeCache.BigInteger, (object) this.ReadLineNoNewline(context));
    }

    private object ReadLong(CodeContext context, int size)
    {
      return (object) new BigInteger(this.Read(context, size).MakeByteArray());
    }

    private char ReadUInt8(CodeContext context) => this._file.ReadChar(context);

    private ushort ReadUInt16(CodeContext context)
    {
      int index = 0;
      return PythonStruct.CreateUShortValue(context, ref index, true, this.Read(context, 2));
    }

    public object find_global(CodeContext context, object module, object attr)
    {
      object ret;
      if (!Importer.TryGetExistingModule(context, Converter.ConvertToString(module), out ret))
      {
        LightExceptions.CheckAndThrow(Builtin.__import__(context, Converter.ConvertToString(module)));
        ret = context.LanguageContext.SystemStateModules[module];
      }
      return PythonOps.GetBoundAttr(context, ret, Converter.ConvertToString(attr));
    }

    private object MakeInstance(CodeContext context, object cls, object[] args)
    {
      if (!(cls is OldClass @class))
        return PythonOps.CallWithContext(context, cls, args);
      OldInstance o = new OldInstance(context, @class);
      if (args.Length != 0 || PythonOps.HasAttr(context, cls, "__getinitargs__"))
        PythonOps.CallWithContext(context, PythonOps.GetBoundAttr(context, (object) o, "__init__"), args);
      return (object) o;
    }

    private void PopMark(int markIndex)
    {
      for (int index = this._stack.Count - 1; index >= markIndex; --index)
        this._stack.RemoveAt(index);
    }

    private void SetItems(PythonDictionary dict, int markIndex)
    {
      DictionaryStorage storage = dict._storage;
      storage.EnsureCapacityNoLock((this._stack.Count - (markIndex + 1)) / 2);
      for (int index = markIndex + 1; index < this._stack.Count; index += 2)
        storage.AddNoLock(ref dict._storage, this._stack[index], this._stack[index + 1]);
      this.PopMark(markIndex);
    }

    private void LoadAppend(CodeContext context)
    {
      object obj = this.PopStack();
      object o = this.PeekStack();
      if (o is IronPython.Runtime.List)
        ((IronPython.Runtime.List) o).append(obj);
      else
        PythonCalls.Call(context, PythonOps.GetBoundAttr(context, o, "append"), obj);
    }

    private void LoadAppends(CodeContext context)
    {
      int markIndex = this.GetMarkIndex(context);
      IronPython.Runtime.List list = (IronPython.Runtime.List) this._stack[markIndex - 1];
      for (int index = markIndex + 1; index < this._stack.Count; ++index)
        list.AddNoLock(this._stack[index]);
      this.PopMark(markIndex);
    }

    private void LoadBinFloat(CodeContext context)
    {
      this._stack.Add((object) this.ReadFloat64(context));
    }

    private void LoadBinGet(CodeContext context)
    {
      this._stack.Add(this.MemoGet(context, (int) this.ReadUInt8(context)));
    }

    private void LoadBinInt(CodeContext context)
    {
      this._stack.Add((object) this.ReadInt32(context));
    }

    private void LoadBinInt1(CodeContext context)
    {
      this._stack.Add((object) (int) this.ReadUInt8(context));
    }

    private void LoadBinInt2(CodeContext context)
    {
      this._stack.Add((object) (int) this.ReadUInt16(context));
    }

    private void LoadBinPersid(CodeContext context)
    {
      if (this._pers_loader == null)
        throw this.CannotUnpickle(context, "cannot unpickle binary persistent ID w/o persistent_load");
      this._stack.Add(context.LanguageContext.CallSplat(this._pers_loader, this.PopStack()));
    }

    private void LoadBinPut(CodeContext context)
    {
      this.MemoPut((int) this.ReadUInt8(context), this.PeekStack());
    }

    private void LoadBinString(CodeContext context)
    {
      this._stack.Add((object) this.Read(context, this.ReadInt32(context)));
    }

    private void LoadBinUnicode(CodeContext context)
    {
      string s = this.Read(context, this.ReadInt32(context));
      for (int index = 0; index < s.Length; ++index)
      {
        if (s[index] >= '\u0080')
        {
          this._stack.Add((object) StringOps.decode(context, s, (object) "utf-8", "strict"));
          return;
        }
      }
      this._stack.Add((object) s);
    }

    private void LoadBuild(CodeContext context)
    {
      object obj1 = this.PopStack();
      object o = this.PeekStack();
      object ret1;
      if (PythonOps.TryGetBoundAttr(context, o, "__setstate__", out ret1))
      {
        PythonOps.CallWithContext(context, ret1, obj1);
      }
      else
      {
        PythonDictionary otherø;
        PythonDictionary pythonDictionary1;
        switch (obj1)
        {
          case null:
            otherø = (PythonDictionary) null;
            pythonDictionary1 = (PythonDictionary) null;
            break;
          case PythonDictionary _:
            otherø = (PythonDictionary) obj1;
            pythonDictionary1 = (PythonDictionary) null;
            break;
          case PythonTuple _:
            PythonTuple pythonTuple = (PythonTuple) obj1;
            otherø = pythonTuple.__len__() == 2 ? (PythonDictionary) pythonTuple[0] : throw PythonOps.ValueError("state for object without __setstate__ must be None, dict, or 2-tuple");
            pythonDictionary1 = (PythonDictionary) pythonTuple[1];
            break;
          default:
            throw PythonOps.ValueError("state for object without __setstate__ must be None, dict, or 2-tuple");
        }
        object ret2;
        if (otherø != null && PythonOps.TryGetBoundAttr(context, o, "__dict__", out ret2))
        {
          if (ret2 is PythonDictionary pythonDictionary2)
          {
            pythonDictionary2.update(context, (IDictionary<object, object>) otherø);
          }
          else
          {
            object ret3;
            if (PythonOps.TryGetBoundAttr(context, ret2, "update", out ret3))
              PythonOps.CallWithContext(context, ret3, (object) otherø);
            else
              throw this.CannotUnpickle(context, "could not update __dict__ {0} when building {1}", (object) otherø, o);
          }
        }
        if (pythonDictionary1 == null)
          return;
        foreach (object obj2 in (IEnumerable) pythonDictionary1)
          PythonOps.SetAttr(context, o, (string) obj2, pythonDictionary1[obj2]);
      }
    }

    private void LoadDict(CodeContext context)
    {
      int markIndex = this.GetMarkIndex(context);
      PythonDictionary dict = new PythonDictionary((this._stack.Count - 1 - markIndex) / 2);
      this.SetItems(dict, markIndex);
      this._stack.Add((object) dict);
    }

    private void LoadDup(CodeContext context) => this._stack.Add(this.PeekStack());

    private void LoadEmptyDict(CodeContext context)
    {
      this._stack.Add((object) new PythonDictionary((DictionaryStorage) new CommonDictionaryStorage()));
    }

    private void LoadEmptyList(CodeContext context)
    {
      this._stack.Add((object) PythonOps.MakeList());
    }

    private void LoadEmptyTuple(CodeContext context)
    {
      this._stack.Add((object) PythonTuple.MakeTuple());
    }

    private void LoadExt1(CodeContext context)
    {
      PythonTuple pythonTuple = (PythonTuple) PythonCopyReg.GetInvertedRegistry(context)[(object) (int) this.ReadUInt8(context)];
      this._stack.Add(this.find_global(context, pythonTuple[0], pythonTuple[1]));
    }

    private void LoadExt2(CodeContext context)
    {
      PythonTuple pythonTuple = (PythonTuple) PythonCopyReg.GetInvertedRegistry(context)[(object) (int) this.ReadUInt16(context)];
      this._stack.Add(this.find_global(context, pythonTuple[0], pythonTuple[1]));
    }

    private void LoadExt4(CodeContext context)
    {
      PythonTuple pythonTuple = (PythonTuple) PythonCopyReg.GetInvertedRegistry(context)[(object) this.ReadInt32(context)];
      this._stack.Add(this.find_global(context, pythonTuple[0], pythonTuple[1]));
    }

    private void LoadFloat(CodeContext context) => this._stack.Add(this.ReadFloatString(context));

    private void LoadGet(CodeContext context)
    {
      try
      {
        this._stack.Add(this.MemoGet(context, (int) this.ReadIntFromString(context)));
      }
      catch (ArgumentException ex)
      {
        throw PythonExceptions.CreateThrowable(PythonPickle.BadPickleGet(context), (object) "while executing GET: invalid integer value");
      }
    }

    private void LoadGlobal(CodeContext context)
    {
      string module = this.ReadLineNoNewline(context);
      string attr = this.ReadLineNoNewline(context);
      this._stack.Add(this.find_global(context, (object) module, (object) attr));
    }

    private void LoadInst(CodeContext context)
    {
      this.LoadGlobal(context);
      object obj = this.PopStack();
      switch (obj)
      {
        case OldClass _:
        case PythonType _:
          int markIndex = this.GetMarkIndex(context);
          object[] sliceAsArray = this.StackGetSliceAsArray(markIndex + 1);
          this.PopMark(markIndex);
          this._stack.Add(this.MakeInstance(context, obj, sliceAsArray));
          break;
        default:
          throw PythonOps.TypeError("expected class or type after INST, got {0}", (object) DynamicHelpers.GetPythonType(obj));
      }
    }

    private void LoadInt(CodeContext context) => this._stack.Add(this.ReadIntFromString(context));

    private void LoadList(CodeContext context)
    {
      int markIndex = this.GetMarkIndex(context);
      IronPython.Runtime.List list = IronPython.Runtime.List.FromArrayNoCopy(this.StackGetSliceAsArray(markIndex + 1));
      this.PopMark(markIndex);
      this._stack.Add((object) list);
    }

    private void LoadLong(CodeContext context) => this._stack.Add(this.ReadLongFromString(context));

    private void LoadLong1(CodeContext context)
    {
      int size = (int) this.ReadUInt8(context);
      if (size == 4)
        this._stack.Add((object) (BigInteger) this.ReadInt32(context));
      else
        this._stack.Add(this.ReadLong(context, size));
    }

    private void LoadLong4(CodeContext context)
    {
      this._stack.Add(this.ReadLong(context, this.ReadInt32(context)));
    }

    private void LoadLongBinGet(CodeContext context)
    {
      this._stack.Add(this.MemoGet(context, this.ReadInt32(context)));
    }

    private void LoadLongBinPut(CodeContext context)
    {
      this.MemoPut(this.ReadInt32(context), this.PeekStack());
    }

    private void LoadMark(CodeContext context)
    {
      this._stack.Add(PythonPickle.UnpicklerObject._mark);
    }

    private void LoadNewFalse(CodeContext context)
    {
      this._stack.Add(ScriptingRuntimeHelpers.False);
    }

    private void LoadNewObj(CodeContext context)
    {
      if (!(this.PopStack() is PythonTuple o))
        throw PythonOps.TypeError("expected tuple as second argument to NEWOBJ, got {0}", (object) DynamicHelpers.GetPythonType((object) o));
      if (!(this.PopStack() is PythonType owner))
        throw PythonOps.TypeError("expected new-style type as first argument to NEWOBJ, got {0}", (object) DynamicHelpers.GetPythonType((object) o));
      PythonTypeSlot slot;
      object func;
      if (!owner.TryResolveSlot(context, "__new__", out slot) || !slot.TryGetValue(context, (object) null, owner, out func))
        throw PythonOps.TypeError("didn't find __new__");
      object[] objArray = new object[o.__len__() + 1];
      o.CopyTo((Array) objArray, 1);
      objArray[0] = (object) owner;
      this._stack.Add(PythonOps.CallWithContext(context, func, objArray));
    }

    private void LoadNewTrue(CodeContext context) => this._stack.Add(ScriptingRuntimeHelpers.True);

    private void LoadNoneValue(CodeContext context) => this._stack.Add((object) null);

    private void LoadObj(CodeContext context)
    {
      int markIndex = this.GetMarkIndex(context);
      if (markIndex + 1 >= this._stack.Count)
        throw PythonExceptions.CreateThrowable(PythonPickle.UnpicklingError(context), (object) "could not find MARK");
      object obj = this._stack[markIndex + 1];
      switch (obj)
      {
        case OldClass _:
        case PythonType _:
          object[] sliceAsArray = this.StackGetSliceAsArray(markIndex + 2);
          this.PopMark(markIndex);
          this._stack.Add(this.MakeInstance(context, obj, sliceAsArray));
          break;
        default:
          throw PythonOps.TypeError("expected class or type as first argument to INST, got {0}", (object) DynamicHelpers.GetPythonType(obj));
      }
    }

    private void LoadPersId(CodeContext context)
    {
      if (this._pers_loader == null)
        throw this.CannotUnpickle(context, "A load persistent ID instruction is present but no persistent_load function is available");
      this._stack.Add(context.LanguageContext.CallSplat(this._pers_loader, (object) this.ReadLineNoNewline(context)));
    }

    private void LoadPop(CodeContext context) => this.PopStack();

    private void LoadPopMark(CodeContext context) => this.PopMark(this.GetMarkIndex(context));

    private void LoadProto(CodeContext context)
    {
      int num = (int) this.ReadUInt8(context);
      if (num > 2)
        throw PythonOps.ValueError("unsupported pickle protocol: {0}", (object) num);
    }

    private void LoadPut(CodeContext context)
    {
      this.MemoPut((int) this.ReadIntFromString(context), this.PeekStack());
    }

    private void LoadReduce(CodeContext context)
    {
      object o = this.PopStack();
      object obj = this.PopStack();
      if (o == null)
        this._stack.Add(PythonCalls.Call(context, PythonOps.GetBoundAttr(context, obj, "__basicnew__")));
      else if (o.GetType() != typeof (PythonTuple))
        throw PythonOps.TypeError("while executing REDUCE, expected tuple at the top of the stack, but got {0}", (object) DynamicHelpers.GetPythonType(o));
      this._stack.Add(PythonCalls.Call(context, obj, ((PythonTuple) o)._data));
    }

    private void LoadSetItem(CodeContext context)
    {
      object obj = this.PopStack();
      object key = this.PopStack();
      if (!(this.PeekStack() is PythonDictionary pythonDictionary))
        throw PythonOps.TypeError("while executing SETITEM, expected dict at stack[-3], but got {0}", (object) DynamicHelpers.GetPythonType(this.PeekStack()));
      pythonDictionary[key] = obj;
    }

    private void LoadSetItems(CodeContext context)
    {
      int markIndex = this.GetMarkIndex(context);
      if (!(this._stack[markIndex - 1] is PythonDictionary dict))
        throw PythonOps.TypeError("while executing SETITEMS, expected dict below last mark, but got {0}", (object) DynamicHelpers.GetPythonType(this._stack[markIndex - 1]));
      this.SetItems(dict, markIndex);
    }

    private void LoadShortBinstring(CodeContext context)
    {
      this._stack.Add((object) this.Read(context, (int) this.ReadUInt8(context)));
    }

    private void LoadString(CodeContext context)
    {
      string str = this.ReadLineNoNewline(context);
      if (str.Length < 2 || (str[0] != '"' || str[str.Length - 1] != '"') && (str[0] != '\'' || str[str.Length - 1] != '\''))
        throw PythonOps.ValueError("while executing STRING, expected string that starts and ends with quotes");
      this._stack.Add((object) StringOps.decode(context, str.Substring(1, str.Length - 2), (object) "string-escape", "strict"));
    }

    private void LoadTuple(CodeContext context)
    {
      int markIndex = this.GetMarkIndex(context);
      PythonTuple pythonTuple = PythonTuple.MakeTuple(this.StackGetSliceAsArray(markIndex + 1));
      this.PopMark(markIndex);
      this._stack.Add((object) pythonTuple);
    }

    private void LoadTuple1(CodeContext context)
    {
      this._stack.Add((object) PythonTuple.MakeTuple(this.PopStack()));
    }

    private void LoadTuple2(CodeContext context)
    {
      object obj = this.PopStack();
      this._stack.Add((object) PythonTuple.MakeTuple(this.PopStack(), obj));
    }

    private void LoadTuple3(CodeContext context)
    {
      object obj1 = this.PopStack();
      object obj2 = this.PopStack();
      this._stack.Add((object) PythonTuple.MakeTuple(this.PopStack(), obj2, obj1));
    }

    private void LoadUnicode(CodeContext context)
    {
      this._stack.Add((object) StringOps.decode(context, this.ReadLineNoNewline(context), (object) "raw-unicode-escape", "strict"));
    }
  }

  private class ReferenceEqualityComparer : IEqualityComparer<object>
  {
    public static PythonPickle.ReferenceEqualityComparer Instance = new PythonPickle.ReferenceEqualityComparer();

    public bool Equals(object x, object y) => x == y;

    public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
  }
}
