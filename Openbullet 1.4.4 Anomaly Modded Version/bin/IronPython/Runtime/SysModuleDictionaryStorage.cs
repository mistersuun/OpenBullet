// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.SysModuleDictionaryStorage
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Modules;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

internal class SysModuleDictionaryStorage : ModuleDictionaryStorage
{
  private SysModuleDictionaryStorage.ExceptionState _exceptionState;
  private object _excType;
  private object _excValue;
  private object _excTraceback;
  private SysModuleDictionaryStorage.ExceptionStateFlags _setValues = SysModuleDictionaryStorage.ExceptionStateFlags.Type | SysModuleDictionaryStorage.ExceptionStateFlags.Value | SysModuleDictionaryStorage.ExceptionStateFlags.Traceback;
  private SysModuleDictionaryStorage.ExceptionStateFlags _removedValues;

  public SysModuleDictionaryStorage()
    : base(typeof (SysModule))
  {
  }

  public override void Add(ref DictionaryStorage storage, object key, object value)
  {
    lock (this)
      this.AddNoLock(ref storage, key, value);
  }

  public override void AddNoLock(ref DictionaryStorage storage, object key, object value)
  {
    if (key is string str)
    {
      switch (str)
      {
        case "exc_type":
          this._setValues |= SysModuleDictionaryStorage.ExceptionStateFlags.Type;
          this._removedValues &= ~SysModuleDictionaryStorage.ExceptionStateFlags.Type;
          this._excType = value;
          break;
        case "exc_value":
          this._setValues |= SysModuleDictionaryStorage.ExceptionStateFlags.Value;
          this._removedValues &= ~SysModuleDictionaryStorage.ExceptionStateFlags.Value;
          this._excValue = value;
          break;
        case "exc_traceback":
          this._setValues |= SysModuleDictionaryStorage.ExceptionStateFlags.Traceback;
          this._removedValues &= ~SysModuleDictionaryStorage.ExceptionStateFlags.Traceback;
          this._excTraceback = value;
          break;
      }
    }
    base.AddNoLock(ref storage, key, value);
  }

  public override bool Remove(ref DictionaryStorage storage, object key)
  {
    if ((this._exceptionState != null || this._setValues != (SysModuleDictionaryStorage.ExceptionStateFlags) 0) && key is string str)
    {
      switch (str)
      {
        case "exc_type":
          lock (this)
          {
            this._excType = (object) null;
            this._setValues &= ~SysModuleDictionaryStorage.ExceptionStateFlags.Type;
            this._removedValues |= SysModuleDictionaryStorage.ExceptionStateFlags.Type;
            break;
          }
        case "exc_value":
          lock (this)
          {
            this._excValue = (object) null;
            this._setValues &= ~SysModuleDictionaryStorage.ExceptionStateFlags.Value;
            this._removedValues |= SysModuleDictionaryStorage.ExceptionStateFlags.Value;
            break;
          }
        case "exc_traceback":
          lock (this)
          {
            this._excTraceback = (object) null;
            this._setValues &= ~SysModuleDictionaryStorage.ExceptionStateFlags.Traceback;
            this._removedValues |= SysModuleDictionaryStorage.ExceptionStateFlags.Traceback;
            break;
          }
      }
    }
    return base.Remove(ref storage, key);
  }

  public override bool TryGetValue(object key, out object value)
  {
    SysModuleDictionaryStorage.ExceptionState exceptionState = this._exceptionState;
    return (exceptionState != null || this._setValues != (SysModuleDictionaryStorage.ExceptionStateFlags) 0) && key is string strKey && this.TryGetExcValue(exceptionState, strKey, out value) || base.TryGetValue(key, out value);
  }

  private bool TryGetExcValue(
    SysModuleDictionaryStorage.ExceptionState exState,
    string strKey,
    out object value)
  {
    switch (strKey)
    {
      case "exc_type":
        lock (this)
        {
          if ((this._removedValues & SysModuleDictionaryStorage.ExceptionStateFlags.Type) == (SysModuleDictionaryStorage.ExceptionStateFlags) 0)
          {
            value = (this._setValues & SysModuleDictionaryStorage.ExceptionStateFlags.Type) == (SysModuleDictionaryStorage.ExceptionStateFlags) 0 ? exState.Type : this._excType;
            return true;
          }
          break;
        }
      case "exc_value":
        lock (this)
        {
          if ((this._removedValues & SysModuleDictionaryStorage.ExceptionStateFlags.Value) == (SysModuleDictionaryStorage.ExceptionStateFlags) 0)
          {
            value = (this._setValues & SysModuleDictionaryStorage.ExceptionStateFlags.Value) == (SysModuleDictionaryStorage.ExceptionStateFlags) 0 ? exState.Value : this._excValue;
            return true;
          }
          break;
        }
      case "exc_traceback":
        lock (this)
        {
          if ((this._removedValues & SysModuleDictionaryStorage.ExceptionStateFlags.Traceback) == (SysModuleDictionaryStorage.ExceptionStateFlags) 0)
          {
            if ((this._setValues & SysModuleDictionaryStorage.ExceptionStateFlags.Traceback) != (SysModuleDictionaryStorage.ExceptionStateFlags) 0)
            {
              value = this._excTraceback;
            }
            else
            {
              this._excTraceback = SysModuleDictionaryStorage.CreateTraceBack(exState);
              this._setValues |= SysModuleDictionaryStorage.ExceptionStateFlags.Traceback;
              value = this._excTraceback;
            }
            return true;
          }
          break;
        }
    }
    value = (object) null;
    return false;
  }

  public override List<KeyValuePair<object, object>> GetItems()
  {
    List<KeyValuePair<object, object>> items = base.GetItems();
    object obj;
    if (this.TryGetValue((object) "exc_traceback", out obj))
      items.Add(new KeyValuePair<object, object>((object) "exc_traceback", obj));
    if (this.TryGetValue((object) "exc_type", out obj))
      items.Add(new KeyValuePair<object, object>((object) "exc_type", obj));
    if (this.TryGetValue((object) "exc_value", out obj))
      items.Add(new KeyValuePair<object, object>((object) "exc_value", obj));
    return items;
  }

  private static object CreateTraceBack(SysModuleDictionaryStorage.ExceptionState list)
  {
    return (object) PythonOps.CreateTraceBack(list.ClrException, (IList<DynamicStackFrame>) list.Traceback, (IList<FunctionStack>) null, list.FrameCount);
  }

  public override void Clear(ref DictionaryStorage storage)
  {
    lock (this)
    {
      this._exceptionState = (SysModuleDictionaryStorage.ExceptionState) null;
      this._setValues = (SysModuleDictionaryStorage.ExceptionStateFlags) 0;
      this._removedValues = (SysModuleDictionaryStorage.ExceptionStateFlags) 0;
      this._excTraceback = this._excType = this._excValue = (object) null;
      base.Clear(ref storage);
    }
  }

  public void UpdateExceptionInfo(
    Exception clrException,
    object type,
    object value,
    List<DynamicStackFrame> traceback)
  {
    lock (this)
    {
      this._exceptionState = new SysModuleDictionaryStorage.ExceptionState(clrException, type, value, traceback);
      this._setValues = this._removedValues = (SysModuleDictionaryStorage.ExceptionStateFlags) 0;
    }
  }

  public void UpdateExceptionInfo(object type, object value, object traceback)
  {
    lock (this)
    {
      this._exceptionState = new SysModuleDictionaryStorage.ExceptionState((Exception) null, type, value, (List<DynamicStackFrame>) null);
      this._excTraceback = traceback;
      this._setValues = SysModuleDictionaryStorage.ExceptionStateFlags.Traceback;
      this._removedValues = (SysModuleDictionaryStorage.ExceptionStateFlags) 0;
    }
  }

  public void ExceptionHandled()
  {
    lock (this)
    {
      this._setValues = SysModuleDictionaryStorage.ExceptionStateFlags.Type | SysModuleDictionaryStorage.ExceptionStateFlags.Value | SysModuleDictionaryStorage.ExceptionStateFlags.Traceback;
      this._removedValues = (SysModuleDictionaryStorage.ExceptionStateFlags) 0;
      this._exceptionState = (SysModuleDictionaryStorage.ExceptionState) null;
      this._excTraceback = this._excType = this._excValue = (object) null;
    }
  }

  public override void Reload() => base.Reload();

  [Flags]
  private enum ExceptionStateFlags
  {
    Type = 1,
    Value = 2,
    Traceback = 4,
  }

  private class ExceptionState
  {
    public readonly object Type;
    public readonly object Value;
    public readonly List<DynamicStackFrame> Traceback;
    public readonly int FrameCount;
    public readonly Exception ClrException;

    public ExceptionState(
      Exception clrException,
      object type,
      object value,
      List<DynamicStackFrame> traceback)
    {
      this.Type = type;
      this.Value = value;
      this.Traceback = traceback;
      this.ClrException = clrException;
      if (traceback == null)
        return;
      this.FrameCount = traceback.Count;
    }
  }
}
