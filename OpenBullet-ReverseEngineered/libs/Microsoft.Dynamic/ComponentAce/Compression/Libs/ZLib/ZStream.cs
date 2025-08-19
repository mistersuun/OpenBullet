// Decompiled with JetBrains decompiler
// Type: ComponentAce.Compression.Libs.ZLib.ZStream
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace ComponentAce.Compression.Libs.ZLib;

public sealed class ZStream
{
  private const int DEF_WBITS = 15;
  private const int MAX_MEM_LEVEL = 9;
  private byte[] _next_in;
  private int _next_in_index;
  private int _avail_in;
  private long _total_in;
  private byte[] _next_out;
  private int _next_out_index;
  private int _avail_out;
  private long _total_out;
  private string _msg;
  private Deflate _dstate;
  private Inflate _istate;

  public long adler { get; set; }

  public BlockType Data_type { get; set; }

  public byte[] next_in
  {
    get => this._next_in;
    set => this._next_in = value;
  }

  public int next_in_index
  {
    get => this._next_in_index;
    set => this._next_in_index = value;
  }

  public int avail_in
  {
    get => this._avail_in;
    set => this._avail_in = value;
  }

  public long total_in
  {
    get => this._total_in;
    set => this._total_in = value;
  }

  public byte[] next_out
  {
    get => this._next_out;
    set => this._next_out = value;
  }

  public int next_out_index
  {
    get => this._next_out_index;
    set => this._next_out_index = value;
  }

  public int avail_out
  {
    get => this._avail_out;
    set => this._avail_out = value;
  }

  public long total_out
  {
    get => this._total_out;
    set => this._total_out = value;
  }

  public string msg
  {
    get => this._msg;
    set => this._msg = value;
  }

  internal Deflate dstate
  {
    get => this._dstate;
    set => this._dstate = value;
  }

  internal Inflate istate
  {
    get => this._istate;
    set => this._istate = value;
  }

  public bool IsInitialized => this._dstate != null || this._istate != null;

  public int inflateInit() => this.inflateInit(15);

  public int inflateInit(int windowBits)
  {
    this._istate = new Inflate();
    return this._istate.inflateInit(this, windowBits);
  }

  public int inflate(FlushStrategy flush)
  {
    return this._istate == null ? -2 : this._istate.inflate(this, flush);
  }

  public int inflateEnd()
  {
    this.next_in_index = 0;
    this.next_out_index = 0;
    if (this._istate == null)
      return -2;
    int num = this._istate.inflateEnd(this);
    this._istate = (Inflate) null;
    return num;
  }

  public int inflateSync() => this._istate == null ? -2 : this._istate.inflateSync(this);

  public int inflateSetDictionary(byte[] dictionary, int dictLength)
  {
    return this._istate == null ? -2 : this._istate.inflateSetDictionary(this, dictionary, dictLength);
  }

  public int DeflateInit(int level) => this.DeflateInit(level, 15);

  public int DeflateInit(int level, int bits)
  {
    this._dstate = new Deflate();
    return this._dstate.DeflateInit(this, level, bits);
  }

  public int DeflateInit(int level, int windowBits, int memLevel, CompressionStrategy strategy)
  {
    this._dstate = new Deflate();
    return this._dstate.DeflateInit2(this, level, windowBits, memLevel, strategy);
  }

  public int reset()
  {
    if (this._dstate != null)
      return this._dstate.deflateReset(this);
    return this._istate != null ? this._istate.inflateReset(this) : -2;
  }

  public int deflate(FlushStrategy flush)
  {
    return this._dstate == null ? -2 : this._dstate.deflate(this, flush);
  }

  public int deflateEnd()
  {
    this.next_in_index = 0;
    this.next_out_index = 0;
    if (this._dstate == null)
      return -2;
    int num = this._dstate.deflateEnd();
    this._dstate = (Deflate) null;
    return num;
  }

  public int deflateParams(int level, CompressionStrategy strategy)
  {
    return this._dstate == null ? -2 : this._dstate.deflateParams(this, level, strategy);
  }

  public int deflateSetDictionary(byte[] dictionary, int dictLength)
  {
    return this._dstate == null ? -2 : this._dstate.deflateSetDictionary(this, dictionary, dictLength);
  }

  public void FlushPending()
  {
    int length = this._dstate.Pending;
    if (length > this._avail_out)
      length = this._avail_out;
    if (length == 0)
      return;
    Array.Copy((Array) this._dstate.Pending_buf, this._dstate.Pending_out, (Array) this._next_out, this._next_out_index, length);
    this._next_out_index += length;
    this._dstate.Pending_out += length;
    this._total_out += (long) length;
    this._avail_out -= length;
    this._dstate.Pending -= length;
    if (this._dstate.Pending != 0)
      return;
    this._dstate.Pending_out = 0;
  }

  public int ReadBuf(byte[] buf, int start, int size)
  {
    int num = this._avail_in;
    if (num > size)
      num = size;
    if (num == 0)
      return 0;
    this._avail_in -= num;
    if (this._dstate.NoHeader == 0)
      this.adler = Adler32.GetAdler32Checksum(this.adler, this._next_in, this._next_in_index, num);
    Array.Copy((Array) this._next_in, this._next_in_index, (Array) buf, start, num);
    this._next_in_index += num;
    this._total_in += (long) num;
    return num;
  }

  public void free()
  {
    this._next_in = (byte[]) null;
    this._next_out = (byte[]) null;
    this._msg = (string) null;
  }
}
