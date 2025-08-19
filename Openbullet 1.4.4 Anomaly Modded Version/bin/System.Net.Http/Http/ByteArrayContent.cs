// Decompiled with JetBrains decompiler
// Type: System.Net.Http.ByteArrayContent
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.IO;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http;

public class ByteArrayContent : HttpContent
{
  private readonly byte[] _content;
  private readonly int _offset;
  private readonly int _count;

  public ByteArrayContent(byte[] content)
  {
    this._content = content != null ? content : throw new ArgumentNullException(nameof (content));
    this._offset = 0;
    this._count = content.Length;
    this.SetBuffer(this._content, this._offset, this._count);
  }

  public ByteArrayContent(byte[] content, int offset, int count)
  {
    if (content == null)
      throw new ArgumentNullException(nameof (content));
    if (offset < 0 || offset > content.Length)
      throw new ArgumentOutOfRangeException(nameof (offset));
    if (count < 0 || count > content.Length - offset)
      throw new ArgumentOutOfRangeException(nameof (count));
    this._content = content;
    this._offset = offset;
    this._count = count;
    this.SetBuffer(this._content, this._offset, this._count);
  }

  protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
  {
    return stream.WriteAsync(this._content, this._offset, this._count);
  }

  protected internal override bool TryComputeLength(out long length)
  {
    length = (long) this._count;
    return true;
  }

  protected override Task<Stream> CreateContentReadStreamAsync()
  {
    return Task.FromResult<Stream>((Stream) new MemoryStream(this._content, this._offset, this._count, false));
  }
}
