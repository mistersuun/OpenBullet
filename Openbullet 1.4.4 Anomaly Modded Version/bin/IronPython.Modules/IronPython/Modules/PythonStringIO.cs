// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonStringIO
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace IronPython.Modules;

public static class PythonStringIO
{
  public static PythonType InputType = DynamicHelpers.GetPythonTypeFromType(typeof (PythonStringIO.StringI));
  public static PythonType OutputType = DynamicHelpers.GetPythonTypeFromType(typeof (PythonStringIO.StringO));
  public const string __doc__ = "Provides file like objects for reading and writing to strings.";

  public static object StringIO() => (object) new PythonStringIO.StringO();

  public static object StringIO(string data) => (object) new PythonStringIO.StringI(data);

  [PythonType]
  [PythonHidden(new PlatformID[] {})]
  public class StringI : IEnumerator<string>, IDisposable, IEnumerator
  {
    private StringStream _sr;
    private string _enumValue;

    internal StringI(string data) => this._sr = new StringStream(data);

    public void close() => this._sr = (StringStream) null;

    public bool closed => this._sr == null;

    public void flush() => this.ThrowIfClosed();

    public string getvalue()
    {
      this.ThrowIfClosed();
      return this._sr.Data;
    }

    public string getvalue(bool usePos) => this._sr.Prefix;

    public bool isatty()
    {
      this.ThrowIfClosed();
      return false;
    }

    public object __iter__() => (object) this;

    public string next()
    {
      this.ThrowIfClosed();
      if (this._sr.EOF)
        throw PythonOps.StopIteration();
      return this.readline();
    }

    public string read()
    {
      this.ThrowIfClosed();
      return this._sr.ReadToEnd();
    }

    public string read(int s)
    {
      this.ThrowIfClosed();
      return s >= 0 ? this._sr.Read(s) : this._sr.ReadToEnd();
    }

    public string readline()
    {
      this.ThrowIfClosed();
      return this._sr.ReadLine(-1);
    }

    public string readline(int size)
    {
      this.ThrowIfClosed();
      return this._sr.ReadLine(size);
    }

    public IronPython.Runtime.List readlines()
    {
      this.ThrowIfClosed();
      IronPython.Runtime.List list = PythonOps.MakeList();
      while (!this._sr.EOF)
        list.AddNoLock((object) this.readline());
      return list;
    }

    public IronPython.Runtime.List readlines(int size)
    {
      this.ThrowIfClosed();
      IronPython.Runtime.List list = PythonOps.MakeList();
      while (!this._sr.EOF)
      {
        string str = this.readline();
        list.AddNoLock((object) str);
        if (str.Length < size)
          size -= str.Length;
        else
          break;
      }
      return list;
    }

    public void reset()
    {
      this.ThrowIfClosed();
      this._sr.Reset();
    }

    public void seek(int position) => this.seek(position, 0);

    public void seek(int position, int mode)
    {
      this.ThrowIfClosed();
      SeekOrigin origin;
      switch (mode)
      {
        case 1:
          origin = SeekOrigin.Current;
          break;
        case 2:
          origin = SeekOrigin.End;
          break;
        default:
          origin = SeekOrigin.Begin;
          break;
      }
      this._sr.Seek(position, origin);
    }

    public int tell()
    {
      this.ThrowIfClosed();
      return this._sr.Position;
    }

    public void truncate()
    {
      this.ThrowIfClosed();
      this._sr.Truncate();
    }

    public void truncate(int size)
    {
      this.ThrowIfClosed();
      this._sr.Truncate(size);
    }

    private void ThrowIfClosed()
    {
      if (this.closed)
        throw PythonOps.ValueError("I/O operation on closed file");
    }

    object IEnumerator.Current => (object) this._enumValue;

    bool IEnumerator.MoveNext()
    {
      if (!this._sr.EOF)
      {
        this._enumValue = this.readline();
        return true;
      }
      this._enumValue = (string) null;
      return false;
    }

    void IEnumerator.Reset() => throw new NotImplementedException();

    string IEnumerator<string>.Current => this._enumValue;

    void IDisposable.Dispose()
    {
    }
  }

  [PythonType]
  [PythonHidden(new PlatformID[] {})]
  [DontMapIEnumerableToContains]
  public class StringO : IEnumerator<string>, IDisposable, IEnumerator
  {
    private StringStream _sr = new StringStream("");
    private int _softspace;
    private string _enumValue;

    internal StringO()
    {
    }

    public object __iter__() => (object) this;

    public void close()
    {
      if (this._sr == null)
        return;
      this._sr = (StringStream) null;
    }

    public bool closed => this._sr == null;

    public void flush()
    {
    }

    public string getvalue()
    {
      this.ThrowIfClosed();
      return this._sr.Data;
    }

    public string getvalue(bool usePos)
    {
      this.ThrowIfClosed();
      return this._sr.Prefix;
    }

    public bool isatty()
    {
      this.ThrowIfClosed();
      return false;
    }

    public string next()
    {
      this.ThrowIfClosed();
      if (this._sr.EOF)
        throw PythonOps.StopIteration();
      return this.readline();
    }

    public string read()
    {
      this.ThrowIfClosed();
      return this._sr.ReadToEnd();
    }

    public string read(int i)
    {
      this.ThrowIfClosed();
      return i >= 0 ? this._sr.Read(i) : this._sr.ReadToEnd();
    }

    public string readline()
    {
      this.ThrowIfClosed();
      return this._sr.ReadLine(-1);
    }

    public string readline(int size)
    {
      this.ThrowIfClosed();
      return this._sr.ReadLine(size);
    }

    public IronPython.Runtime.List readlines()
    {
      this.ThrowIfClosed();
      IronPython.Runtime.List list = PythonOps.MakeList();
      while (!this._sr.EOF)
        list.AddNoLock((object) this.readline());
      return list;
    }

    public IronPython.Runtime.List readlines(int size)
    {
      this.ThrowIfClosed();
      IronPython.Runtime.List list = PythonOps.MakeList();
      while (!this._sr.EOF)
      {
        string str = this.readline();
        list.AddNoLock((object) str);
        if (str.Length < size)
          size -= str.Length;
        else
          break;
      }
      return list;
    }

    public void reset()
    {
      this.ThrowIfClosed();
      this._sr.Reset();
    }

    public void seek(int position) => this.seek(position, 0);

    public void seek(int offset, int origin)
    {
      this.ThrowIfClosed();
      SeekOrigin origin1;
      switch (origin)
      {
        case 1:
          origin1 = SeekOrigin.Current;
          break;
        case 2:
          origin1 = SeekOrigin.End;
          break;
        default:
          origin1 = SeekOrigin.Begin;
          break;
      }
      this._sr.Seek(offset, origin1);
    }

    public int softspace
    {
      get => this._softspace;
      set => this._softspace = value;
    }

    public int tell()
    {
      this.ThrowIfClosed();
      return this._sr.Position;
    }

    public void truncate()
    {
      this.ThrowIfClosed();
      this._sr.Truncate();
    }

    public void truncate(int size)
    {
      this.ThrowIfClosed();
      this._sr.Truncate(size);
    }

    public void write(string s)
    {
      if (s == null)
        throw PythonOps.TypeError("write argument must be a string or read-only character buffer, not None");
      this.ThrowIfClosed();
      this._sr.Write(s);
    }

    public void write([NotNull] PythonBuffer buffer) => this._sr.Write(buffer.ToString());

    public void writelines(object o)
    {
      IEnumerator enumerator = PythonOps.GetEnumerator(o);
      while (enumerator.MoveNext())
      {
        if (!(enumerator.Current is string current))
          throw PythonOps.TypeError("string expected");
        this.write(current);
      }
    }

    private void ThrowIfClosed()
    {
      if (this.closed)
        throw PythonOps.ValueError("I/O operation on closed file");
    }

    object IEnumerator.Current => (object) this._enumValue;

    bool IEnumerator.MoveNext()
    {
      if (!this._sr.EOF)
      {
        this._enumValue = this.readline();
        return true;
      }
      this._enumValue = (string) null;
      return false;
    }

    void IEnumerator.Reset() => throw new NotImplementedException();

    string IEnumerator<string>.Current => this._enumValue;

    void IDisposable.Dispose()
    {
    }
  }
}
