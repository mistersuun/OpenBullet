// Decompiled with JetBrains decompiler
// Type: AngleSharp.Text.TextSource
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Text;

public sealed class TextSource : IDisposable
{
  private const int BufferSize = 4096 /*0x1000*/;
  private readonly Stream _baseStream;
  private readonly MemoryStream _raw;
  private readonly byte[] _buffer;
  private readonly char[] _chars;
  private StringBuilder _content;
  private TextSource.EncodingConfidence _confidence;
  private bool _finished;
  private Encoding _encoding;
  private Decoder _decoder;
  private int _index;

  private TextSource(Encoding encoding)
  {
    this._buffer = new byte[4096 /*0x1000*/];
    this._chars = new char[4097];
    this._raw = new MemoryStream();
    this._index = 0;
    this._encoding = encoding ?? TextEncoding.Utf8;
    this._decoder = this._encoding.GetDecoder();
  }

  public TextSource(string source)
    : this((Stream) null, TextEncoding.Utf8)
  {
    this._finished = true;
    this._content.Append(source);
    this._confidence = TextSource.EncodingConfidence.Irrelevant;
  }

  public TextSource(Stream baseStream, Encoding encoding = null)
    : this(encoding)
  {
    this._baseStream = baseStream;
    this._content = StringBuilderPool.Obtain();
    this._confidence = TextSource.EncodingConfidence.Tentative;
  }

  public string Text => this._content.ToString();

  public char this[int index] => TextSource.Replace(this._content[index]);

  public int Length => this._content.Length;

  public Encoding CurrentEncoding
  {
    get => this._encoding;
    set
    {
      if (this._confidence != TextSource.EncodingConfidence.Tentative)
        return;
      if (this._encoding.IsUnicode())
      {
        this._confidence = TextSource.EncodingConfidence.Certain;
      }
      else
      {
        if (value.IsUnicode())
          value = TextEncoding.Utf8;
        if (value == this._encoding)
        {
          this._confidence = TextSource.EncodingConfidence.Certain;
        }
        else
        {
          this._encoding = value;
          this._decoder = value.GetDecoder();
          byte[] array = this._raw.ToArray();
          char[] chars1 = new char[this._encoding.GetMaxCharCount(array.Length)];
          int chars2 = this._decoder.GetChars(array, 0, array.Length, chars1, 0);
          string str = new string(chars1, 0, chars2);
          int num = Math.Min(this._index, str.Length);
          if (str.Substring(0, num).Is(this._content.ToString(0, num)))
          {
            this._confidence = TextSource.EncodingConfidence.Certain;
            this._content.Remove(num, this._content.Length - num);
            this._content.Append(str.Substring(num));
          }
          else
          {
            this._index = 0;
            this._content.Clear().Append(str);
            throw new NotSupportedException();
          }
        }
      }
    }
  }

  public int Index
  {
    get => this._index;
    set => this._index = value;
  }

  public void Dispose()
  {
    if (this._content == null)
      return;
    this._raw.Dispose();
    this._content.Clear().ToPool();
    this._content = (StringBuilder) null;
  }

  public char ReadCharacter()
  {
    if (this._index < this._content.Length)
      return TextSource.Replace(this._content[this._index++]);
    this.ExpandBuffer(4096L /*0x1000*/);
    int index = this._index++;
    return index >= this._content.Length ? char.MaxValue : TextSource.Replace(this._content[index]);
  }

  public string ReadCharacters(int characters)
  {
    int index = this._index;
    if (index + characters <= this._content.Length)
    {
      this._index += characters;
      return this._content.ToString(index, characters);
    }
    this.ExpandBuffer((long) Math.Max(4096 /*0x1000*/, characters));
    this._index += characters;
    characters = Math.Min(characters, this._content.Length - index);
    return this._content.ToString(index, characters);
  }

  public Task PrefetchAsync(int length, CancellationToken cancellationToken)
  {
    return this.ExpandBufferAsync((long) length, cancellationToken);
  }

  public async Task PrefetchAllAsync(CancellationToken cancellationToken)
  {
    ConfiguredTaskAwaitable configuredTaskAwaitable;
    if (this._content.Length == 0)
    {
      configuredTaskAwaitable = this.DetectByteOrderMarkAsync(cancellationToken).ConfigureAwait(false);
      await configuredTaskAwaitable;
    }
    while (!this._finished)
    {
      configuredTaskAwaitable = this.ReadIntoBufferAsync(cancellationToken).ConfigureAwait(false);
      await configuredTaskAwaitable;
    }
  }

  public void InsertText(string content)
  {
    if (this._index >= 0 && this._index < this._content.Length)
      this._content.Insert(this._index, content);
    else
      this._content.Append(content);
    this._index += content.Length;
  }

  private static char Replace(char c) => c != char.MaxValue ? c : '�';

  private async Task DetectByteOrderMarkAsync(CancellationToken cancellationToken)
  {
    int num = await this._baseStream.ReadAsync(this._buffer, 0, 4096 /*0x1000*/).ConfigureAwait(false);
    int sourceIndex = 0;
    if (num > 2 && this._buffer[0] == (byte) 239 && this._buffer[1] == (byte) 187 && this._buffer[2] == (byte) 191)
    {
      this._encoding = TextEncoding.Utf8;
      sourceIndex = 3;
    }
    else if (num > 3 && this._buffer[0] == byte.MaxValue && this._buffer[1] == (byte) 254 && this._buffer[2] == (byte) 0 && this._buffer[3] == (byte) 0)
    {
      this._encoding = TextEncoding.Utf32Le;
      sourceIndex = 4;
    }
    else if (num > 3 && this._buffer[0] == (byte) 0 && this._buffer[1] == (byte) 0 && this._buffer[2] == (byte) 254 && this._buffer[3] == byte.MaxValue)
    {
      this._encoding = TextEncoding.Utf32Be;
      sourceIndex = 4;
    }
    else if (num > 1 && this._buffer[0] == (byte) 254 && this._buffer[1] == byte.MaxValue)
    {
      this._encoding = TextEncoding.Utf16Be;
      sourceIndex = 2;
    }
    else if (num > 1 && this._buffer[0] == byte.MaxValue && this._buffer[1] == (byte) 254)
    {
      this._encoding = TextEncoding.Utf16Le;
      sourceIndex = 2;
    }
    else if (num > 3 && this._buffer[0] == (byte) 132 && this._buffer[1] == (byte) 49 && this._buffer[2] == (byte) 149 && this._buffer[3] == (byte) 51)
    {
      this._encoding = TextEncoding.Gb18030;
      sourceIndex = 4;
    }
    if (sourceIndex > 0)
    {
      num -= sourceIndex;
      Array.Copy((Array) this._buffer, sourceIndex, (Array) this._buffer, 0, num);
      this._decoder = this._encoding.GetDecoder();
      this._confidence = TextSource.EncodingConfidence.Certain;
    }
    this.AppendContentFromBuffer(num);
  }

  private async Task ExpandBufferAsync(long size, CancellationToken cancellationToken)
  {
    ConfiguredTaskAwaitable configuredTaskAwaitable;
    if (!this._finished && this._content.Length == 0)
    {
      configuredTaskAwaitable = this.DetectByteOrderMarkAsync(cancellationToken).ConfigureAwait(false);
      await configuredTaskAwaitable;
    }
    while (size + (long) this._index > (long) this._content.Length && !this._finished)
    {
      configuredTaskAwaitable = this.ReadIntoBufferAsync(cancellationToken).ConfigureAwait(false);
      await configuredTaskAwaitable;
    }
  }

  private async Task ReadIntoBufferAsync(CancellationToken cancellationToken)
  {
    this.AppendContentFromBuffer(await this._baseStream.ReadAsync(this._buffer, 0, 4096 /*0x1000*/, cancellationToken).ConfigureAwait(false));
  }

  private void ExpandBuffer(long size)
  {
    if (!this._finished && this._content.Length == 0)
      this.DetectByteOrderMarkAsync(CancellationToken.None).Wait();
    while (size + (long) this._index > (long) this._content.Length && !this._finished)
      this.ReadIntoBuffer();
  }

  private void ReadIntoBuffer()
  {
    this.AppendContentFromBuffer(this._baseStream.Read(this._buffer, 0, 4096 /*0x1000*/));
  }

  private void AppendContentFromBuffer(int size)
  {
    this._finished = size == 0;
    int chars = this._decoder.GetChars(this._buffer, 0, size, this._chars, 0);
    if (this._confidence != TextSource.EncodingConfidence.Certain)
      this._raw.Write(this._buffer, 0, size);
    this._content.Append(this._chars, 0, chars);
  }

  private enum EncodingConfidence : byte
  {
    Tentative,
    Certain,
    Irrelevant,
  }
}
