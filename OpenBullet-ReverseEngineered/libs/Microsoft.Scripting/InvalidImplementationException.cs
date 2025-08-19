// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.InvalidImplementationException
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Scripting;

[Serializable]
public class InvalidImplementationException : Exception
{
  public InvalidImplementationException()
  {
  }

  public InvalidImplementationException(string message)
    : base(message)
  {
  }

  public InvalidImplementationException(string message, Exception e)
    : base(message, e)
  {
  }

  protected InvalidImplementationException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}
