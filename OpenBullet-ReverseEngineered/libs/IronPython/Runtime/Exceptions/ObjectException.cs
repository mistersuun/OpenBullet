// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Exceptions.ObjectException
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using System;
using System.Runtime.Serialization;

#nullable disable
namespace IronPython.Runtime.Exceptions;

[Serializable]
internal sealed class ObjectException : Exception, IPythonException
{
  private object _instance;
  private PythonType _type;

  public ObjectException(PythonType type, object instance)
  {
    this._instance = instance;
    this._type = type;
  }

  public ObjectException(string msg)
    : base(msg)
  {
  }

  public ObjectException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  private ObjectException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }

  public object Instance => this._instance;

  public PythonType Type => this._type;

  public object ToPythonException() => (object) this;
}
