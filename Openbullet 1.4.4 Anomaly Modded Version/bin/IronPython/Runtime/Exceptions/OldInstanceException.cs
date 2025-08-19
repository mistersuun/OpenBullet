// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Exceptions.OldInstanceException
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using System;
using System.Runtime.Serialization;

#nullable disable
namespace IronPython.Runtime.Exceptions;

[Serializable]
internal class OldInstanceException : Exception, IPythonException
{
  private OldInstance _instance;

  public OldInstanceException(OldInstance instance) => this._instance = instance;

  public OldInstanceException(string msg)
    : base(msg)
  {
  }

  public OldInstanceException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  protected OldInstanceException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }

  public OldInstance Instance => this._instance;

  public object ToPythonException() => (object) this._instance;
}
