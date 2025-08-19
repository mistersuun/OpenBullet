// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonSubprocessHandle
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable disable
namespace IronPython.Modules;

[PythonType("_subprocess_handle")]
public class PythonSubprocessHandle
{
  private readonly IntPtr _internalHandle;
  internal bool _closed;
  internal bool _duplicated;
  internal bool _isProcess;
  internal int _exitCode;
  private static List<PythonSubprocessHandle> _active = new List<PythonSubprocessHandle>();

  internal PythonSubprocessHandle(IntPtr handle) => this._internalHandle = handle;

  internal PythonSubprocessHandle(IntPtr handle, bool isProcess)
  {
    this._internalHandle = handle;
    this._isProcess = isProcess;
  }

  ~PythonSubprocessHandle()
  {
    if (this._isProcess)
    {
      lock (PythonSubprocessHandle._active)
      {
        int index1 = -1;
        for (int index2 = 0; index2 < PythonSubprocessHandle._active.Count; ++index2)
        {
          if (PythonSubprocessHandle._active[index2] == null)
            index1 = index2;
          else if (PythonSubprocessHandle._active[index2].PollForExit())
          {
            PythonSubprocessHandle._active[index2] = (PythonSubprocessHandle) null;
            index1 = index2;
            if (PythonSubprocessHandle._active[index2] == this)
            {
              this.Close();
              return;
            }
          }
          else if (PythonSubprocessHandle._active[index2] == this)
            return;
        }
        if (!this.PollForExit())
        {
          if (index1 != -1)
          {
            PythonSubprocessHandle._active[index1] = this;
            return;
          }
          PythonSubprocessHandle._active.Add(this);
          return;
        }
        this.Close();
      }
    }
    this.Close();
  }

  private bool PollForExit()
  {
    if (PythonSubprocess.WaitForSingleObjectPI(this._internalHandle, 0) != 0)
      return false;
    PythonSubprocess.GetExitCodeProcessPI(this._internalHandle, out this._exitCode);
    return true;
  }

  public void Close()
  {
    lock (this)
    {
      if (this._closed)
        return;
      PythonSubprocess.CloseHandle(this._internalHandle);
      this._closed = true;
      GC.SuppressFinalize((object) this);
    }
  }

  public object Detach(CodeContext context)
  {
    lock (this)
    {
      if (!this._closed)
      {
        this._closed = true;
        GC.SuppressFinalize((object) this);
        return this._internalHandle.ToPython();
      }
    }
    return (object) -1;
  }

  public static implicit operator int(PythonSubprocessHandle type)
  {
    return type._internalHandle.ToInt32();
  }

  public static implicit operator BigInteger(PythonSubprocessHandle type)
  {
    return (BigInteger) type._internalHandle.ToInt32();
  }

  public static implicit operator IntPtr(PythonSubprocessHandle type) => type._internalHandle;
}
