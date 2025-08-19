// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.ProxyException
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Leaf.xNet;

[Serializable]
public sealed class ProxyException : NetException
{
  public ProxyClient ProxyClient { get; }

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

  public ProxyException(SerializationInfo serializationInfo, StreamingContext streamingContext)
    : base(serializationInfo, streamingContext)
  {
  }
}
