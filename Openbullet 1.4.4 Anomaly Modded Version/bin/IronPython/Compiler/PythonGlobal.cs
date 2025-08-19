// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.PythonGlobal
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System;
using System.Diagnostics;
using System.Reflection;

#nullable disable
namespace IronPython.Compiler;

[DebuggerDisplay("{Display}")]
public sealed class PythonGlobal
{
  private object _value;
  private ModuleGlobalCache _global;
  private string _name;
  private CodeContext _context;
  internal static PropertyInfo CurrentValueProperty = typeof (PythonGlobal).GetProperty(nameof (CurrentValue));
  internal static PropertyInfo RawValueProperty = typeof (PythonGlobal).GetProperty(nameof (RawValue));

  public PythonGlobal(CodeContext context, string name)
  {
    this._value = (object) Uninitialized.Instance;
    this._context = context;
    this._name = name;
  }

  public object CurrentValue
  {
    get => this._value != Uninitialized.Instance ? this._value : this.GetCachedValue(false);
    set
    {
      if (value == Uninitialized.Instance && this._value == Uninitialized.Instance)
        throw PythonOps.GlobalNameError(this._name);
      this._value = value;
    }
  }

  public object CurrentValueLightThrow
  {
    get => this._value != Uninitialized.Instance ? this._value : this.GetCachedValue(true);
  }

  public string Name => this._name;

  private object GetCachedValue(bool lightThrow)
  {
    if (this._global == null)
      this._global = this._context.LanguageContext.GetModuleGlobalCache(this._name);
    if (this._global.IsCaching)
    {
      if (this._global.HasValue)
        return this._global.Value;
    }
    else
    {
      object cachedValue;
      if (this._context.TryLookupBuiltin(this._name, out cachedValue))
        return cachedValue;
    }
    if (lightThrow)
      return LightExceptions.Throw(PythonOps.GlobalNameError(this._name));
    throw PythonOps.GlobalNameError(this._name);
  }

  public object RawValue
  {
    get => this._value;
    internal set => this._value = value;
  }

  public string Display
  {
    get
    {
      try
      {
        return PythonGlobal.GetStringDisplay(this.CurrentValue);
      }
      catch (MissingMemberException ex)
      {
        return "<uninitialized>";
      }
    }
  }

  private static string GetStringDisplay(object val) => val != null ? val.ToString() : "(null)";

  public override string ToString()
  {
    return $"ModuleGlobal: {this._name} Value: {this._value} ({(this.RawValue == Uninitialized.Instance ? (object) "Module Local" : (object) "Global")})";
  }
}
