// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Exceptions.GeneratorExitException
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace IronPython.Runtime.Exceptions;

[Serializable]
public sealed class GeneratorExitException : Exception
{
  public GeneratorExitException()
  {
  }

  public GeneratorExitException(string message)
    : base(message)
  {
  }

  public GeneratorExitException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  private GeneratorExitException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}
