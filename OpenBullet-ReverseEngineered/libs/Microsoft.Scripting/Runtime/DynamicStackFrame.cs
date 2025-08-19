// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.DynamicStackFrame
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Runtime;

[Serializable]
public class DynamicStackFrame
{
  private readonly string _funcName;
  private readonly string _filename;
  private readonly int _lineNo;
  private readonly MethodBase _method;

  public DynamicStackFrame(MethodBase method, string funcName, string filename, int line)
  {
    this._funcName = funcName;
    this._filename = filename;
    this._lineNo = line;
    this._method = method;
  }

  public MethodBase GetMethod() => this._method;

  public string GetMethodName() => this._funcName;

  public string GetFileName() => this._filename;

  public int GetFileLineNumber() => this._lineNo;

  public override string ToString()
  {
    return $"{this._funcName ?? "<function unknown>"} in {this._filename ?? "<filename unknown>"}:{this._lineNo}, {(this._method != (MethodBase) null ? (object) this._method.ToString() : (object) "<method unknown>")}";
  }
}
