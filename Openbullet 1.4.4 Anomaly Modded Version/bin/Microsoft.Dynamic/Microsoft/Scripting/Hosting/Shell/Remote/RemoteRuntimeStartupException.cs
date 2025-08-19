// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Shell.Remote.RemoteRuntimeStartupException
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Scripting.Hosting.Shell.Remote;

[Serializable]
public class RemoteRuntimeStartupException : Exception
{
  public RemoteRuntimeStartupException()
  {
  }

  public RemoteRuntimeStartupException(string message)
    : base(message)
  {
  }

  public RemoteRuntimeStartupException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  protected RemoteRuntimeStartupException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}
