// Decompiled with JetBrains decompiler
// Type: Extreme.Net.HttpException
// Assembly: Extreme.Net, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B85A5720-FE8B-4A1B-9FD2-F7651D37B15B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Extreme.Net.dll

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

#nullable disable
namespace Extreme.Net;

[Serializable]
public sealed class HttpException : NetException
{
  public HttpExceptionStatus Status { get; internal set; }

  public HttpStatusCode HttpStatusCode { get; private set; }

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

  protected HttpException(SerializationInfo serializationInfo, StreamingContext streamingContext)
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
    if (serializationInfo == null)
      return;
    serializationInfo.AddValue("Status", (int) this.Status);
    serializationInfo.AddValue("HttpStatusCode", (int) this.HttpStatusCode);
  }
}
