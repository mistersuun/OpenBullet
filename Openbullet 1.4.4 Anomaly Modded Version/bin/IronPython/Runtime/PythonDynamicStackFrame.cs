// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonDynamicStackFrame
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using System;
using System.Reflection;
using System.Runtime.Serialization;

#nullable disable
namespace IronPython.Runtime;

[Serializable]
internal sealed class PythonDynamicStackFrame : DynamicStackFrame, ISerializable
{
  private readonly CodeContext _context;
  private readonly FunctionCode _code;

  public PythonDynamicStackFrame(CodeContext context, FunctionCode funcCode, int line)
    : base(PythonDynamicStackFrame.GetMethod(context, funcCode), funcCode.co_name, funcCode.co_filename, line)
  {
    this._context = context;
    this._code = funcCode;
  }

  private static MethodBase GetMethod(CodeContext context, FunctionCode funcCode)
  {
    return !context.LanguageContext.EnableTracing || (object) funcCode._tracingDelegate == null ? (MethodBase) RuntimeReflectionExtensions.GetMethodInfo(funcCode._normalDelegate) : (MethodBase) RuntimeReflectionExtensions.GetMethodInfo(funcCode._tracingDelegate);
  }

  private PythonDynamicStackFrame(SerializationInfo info, StreamingContext context)
    : base((MethodBase) info.GetValue("method", typeof (MethodBase)), (string) info.GetValue("funcName", typeof (string)), (string) info.GetValue("filename", typeof (string)), (int) info.GetValue("line", typeof (int)))
  {
  }

  public CodeContext CodeContext => this._context;

  public FunctionCode Code => this._code;

  void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
  {
    info.AddValue("method", (object) this.GetMethod());
    info.AddValue("funcName", (object) this.GetMethodName());
    info.AddValue("filename", (object) this.GetFileName());
    info.AddValue("line", this.GetFileLineNumber());
  }
}
