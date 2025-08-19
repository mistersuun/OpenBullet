// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Exceptions.PythonException
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#nullable disable
namespace IronPython.Runtime.Exceptions;

[Serializable]
internal class PythonException : Exception, IPythonAwareException
{
  private object _pyExceptionObject;
  private List<DynamicStackFrame> _frames;
  private TraceBack _traceback;

  public PythonException()
  {
  }

  public PythonException(string msg)
    : base(msg)
  {
  }

  public PythonException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  protected PythonException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }

  object IPythonAwareException.PythonException
  {
    get => this._pyExceptionObject;
    set => this._pyExceptionObject = value;
  }

  List<DynamicStackFrame> IPythonAwareException.Frames
  {
    get => this._frames;
    set => this._frames = value;
  }

  TraceBack IPythonAwareException.TraceBack
  {
    get => this._traceback;
    set => this._traceback = value;
  }
}
