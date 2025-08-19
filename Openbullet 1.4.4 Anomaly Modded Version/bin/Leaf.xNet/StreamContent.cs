// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.StreamContent
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.IO;

#nullable disable
namespace Leaf.xNet;

public class StreamContent : HttpContent
{
  protected Stream ContentStream;
  protected int BufferSize;
  protected long InitialStreamPosition;

  public StreamContent(Stream contentStream, int bufferSize = 32768 /*0x8000*/)
  {
    if (contentStream == null)
      throw new ArgumentNullException(nameof (contentStream));
    if (!contentStream.CanRead || !contentStream.CanSeek)
      throw new ArgumentException(Resources.ArgumentException_CanNotReadOrSeek, nameof (contentStream));
    if (bufferSize < 1)
      throw ExceptionHelper.CanNotBeLess<int>(nameof (bufferSize), 1);
    this.ContentStream = contentStream;
    this.BufferSize = bufferSize;
    this.InitialStreamPosition = this.ContentStream.Position;
    this.MimeContentType = "application/octet-stream";
  }

  protected StreamContent()
  {
  }

  public override long CalculateContentLength()
  {
    this.ThrowIfDisposed();
    return this.ContentStream.Length;
  }

  public override void WriteTo(Stream stream)
  {
    this.ThrowIfDisposed();
    if (stream == null)
      throw new ArgumentNullException(nameof (stream));
    this.ContentStream.Position = this.InitialStreamPosition;
    byte[] buffer = new byte[this.BufferSize];
    while (true)
    {
      int count = this.ContentStream.Read(buffer, 0, buffer.Length);
      if (count != 0)
        stream.Write(buffer, 0, count);
      else
        break;
    }
  }

  protected override void Dispose(bool disposing)
  {
    if (!disposing || this.ContentStream == null)
      return;
    this.ContentStream.Dispose();
    this.ContentStream = (Stream) null;
  }

  private void ThrowIfDisposed()
  {
    if (this.ContentStream == null)
      throw new ObjectDisposedException(nameof (StreamContent));
  }
}
