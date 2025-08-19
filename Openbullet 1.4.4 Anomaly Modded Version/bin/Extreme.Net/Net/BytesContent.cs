// Decompiled with JetBrains decompiler
// Type: Extreme.Net.BytesContent
// Assembly: Extreme.Net, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B85A5720-FE8B-4A1B-9FD2-F7651D37B15B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Extreme.Net.dll

using System;
using System.IO;

#nullable disable
namespace Extreme.Net;

public class BytesContent : HttpContent
{
  protected byte[] _content;
  protected int _offset;
  protected int _count;

  public BytesContent(byte[] content)
    : this(content, 0, content.Length)
  {
  }

  public BytesContent(byte[] content, int offset, int count)
  {
    if (content == null)
      throw new ArgumentNullException(nameof (content));
    if (offset < 0)
      throw ExceptionHelper.CanNotBeLess<int>(nameof (offset), 0);
    if (offset > content.Length)
      throw ExceptionHelper.CanNotBeGreater<int>(nameof (offset), content.Length);
    if (count < 0)
      throw ExceptionHelper.CanNotBeLess<int>(nameof (count), 0);
    if (count > content.Length - offset)
      throw ExceptionHelper.CanNotBeGreater<int>(nameof (count), content.Length - offset);
    this._content = content;
    this._offset = offset;
    this._count = count;
    this._contentType = "application/octet-stream";
  }

  protected BytesContent()
  {
  }

  public override long CalculateContentLength() => (long) this._content.Length;

  public override void WriteTo(Stream stream)
  {
    stream.Write(this._content, this._offset, this._count);
  }
}
