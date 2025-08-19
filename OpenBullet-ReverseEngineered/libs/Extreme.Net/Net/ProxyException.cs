// Decompiled with JetBrains decompiler
// Type: Extreme.Net.ProxyException
// Assembly: Extreme.Net, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B85A5720-FE8B-4A1B-9FD2-F7651D37B15B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Extreme.Net.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Extreme.Net;

[Serializable]
public sealed class ProxyException : NetException
{
  public ProxyClient ProxyClient { get; private set; }

  public ProxyException()
    : this(Resources.ProxyException_Default)
  {
  }

  public ProxyException(string message, Exception innerException = null)
    : base(message, innerException)
  {
  }

  public ProxyException(string message, ProxyClient proxyClient, Exception innerException = null)
    : base(message, innerException)
  {
    this.ProxyClient = proxyClient;
  }

  protected ProxyException(SerializationInfo serializationInfo, StreamingContext streamingContext)
    : base(serializationInfo, streamingContext)
  {
  }
}
