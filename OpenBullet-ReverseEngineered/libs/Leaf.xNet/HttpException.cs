// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.HttpException
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

#nullable disable
namespace Leaf.xNet;

[Serializable]
public sealed class HttpException : NetException
{
  public HttpExceptionStatus Status { get; internal set; }

  public HttpStatusCode HttpStatusCode { get; }

  internal bool EmptyMessageBody { get; set; }

  public HttpException()
    : this(Resources.HttpException_Default)
  {
  }

  public HttpException(string message, Exception innerException = null)
    : base(message, innerException)
  {
  }

  public HttpException(
    string message,
    HttpExceptionStatus status,
    HttpStatusCode httpStatusCode = HttpStatusCode.None,
    Exception innerException = null)
    : base(message, innerException)
  {
    this.Status = status;
    this.HttpStatusCode = httpStatusCode;
  }

  public HttpException(SerializationInfo serializationInfo, StreamingContext streamingContext)
    : base(serializationInfo, streamingContext)
  {
    if (serializationInfo == null)
      return;
    this.Status = (HttpExceptionStatus) serializationInfo.GetInt32(nameof (Status));
    this.HttpStatusCode = (HttpStatusCode) serializationInfo.GetInt32(nameof (HttpStatusCode));
  }

  [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
  public override void GetObjectData(
    SerializationInfo serializationInfo,
    StreamingContext streamingContext)
  {
    base.GetObjectData(serializationInfo, streamingContext);
    serializationInfo.AddValue("Status", (int) this.Status);
    serializationInfo.AddValue("HttpStatusCode", (int) this.HttpStatusCode);
    serializationInfo.AddValue("EmptyMessageBody", this.EmptyMessageBody);
  }
}
