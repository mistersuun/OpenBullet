// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.BytesContent
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.IO;

#nullable disable
namespace Leaf.xNet;

public class BytesContent : HttpContent
{
  protected byte[] Content;
  protected int Offset;
  protected int Count;

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
    this.Content = content;
    this.Offset = offset;
    this.Count = count;
    this.MimeContentType = "application/octet-stream";
  }

  protected BytesContent()
  {
  }

  public override long CalculateContentLength() => (long) this.Content.Length;

  public override void WriteTo(Stream stream)
  {
    stream.Write(this.Content, this.Offset, this.Count);
  }
}
