// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.NetException
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Leaf.xNet;

[Serializable]
public class NetException : Exception
{
  public NetException()
    : this(Resources.NetException_Default)
  {
  }

  public NetException(string message, Exception innerException = null)
    : base(message, innerException)
  {
  }

  protected NetException(SerializationInfo serializationInfo, StreamingContext streamingContext)
    : base(serializationInfo, streamingContext)
  {
  }
}
