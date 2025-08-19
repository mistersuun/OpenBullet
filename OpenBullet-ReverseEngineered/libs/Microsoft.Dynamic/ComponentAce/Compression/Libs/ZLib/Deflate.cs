// Decompiled with JetBrains decompiler
// Type: ComponentAce.Compression.Libs.ZLib.Deflate
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace ComponentAce.Compression.Libs.ZLib;

public sealed class Deflate
{
  private const int MAX_MEM_LEVEL = 9;
  public const int Z_DEFAULT_COMPRESSION = -1;
  public const int DEF_MEM_LEVEL = 8;
  private const int STORED = 0;
  private const int FAST = 1;
  private const int SLOW = 2;
  private static Deflate.Config[] config_table = new Deflate.Config[10];
  private const int NeedMore = 0;
  private const int BlockDone = 1;
  private const int FinishStarted = 2;
  private const int FinishDone = 3;
  private const int PRESET_DICT = 32 /*0x20*/;
  private const int Z_DEFLATED = 8;
  private const int STORED_BLOCK = 0;
  private const int STATIC_TREES = 1;
  private const int DYN_TREES = 2;
  private const int Buf_size = 16 /*0x10*/;
  private const int REP_3_6 = 16 /*0x10*/;
  private const int REPZ_3_10 = 17;
  private const int REPZ_11_138 = 18;
  private const int MIN_MATCH = 3;
  private const int MAX_MATCH = 258;
  private const int MIN_LOOKAHEAD = 262;
  private const int MAX_BITS = 15;
  private const int D_CODES = 30;
  private const int BL_CODES = 19;
  private const int LENGTH_CODES = 29;
  private const int LITERALS = 256 /*0x0100*/;
  private const int L_CODES = 286;
  private const int HEAP_SIZE = 573;
  private const int END_BLOCK = 256 /*0x0100*/;
  private ZStream strm;
  private DeflateState status;
  private int pending_buf_size;
  private BlockType data_type;
  private byte method;
  private int last_flush;
  private int w_size;
  private int w_bits;
  private int w_mask;
  private byte[] window;
  private int window_size;
  private short[] prev;
  private short[] head;
  private int ins_h;
  private int hash_size;
  private int hash_bits;
  private int hash_mask;
  private int hash_shift;
  private int block_start;
  private int match_length;
  private int prev_match;
  private int match_available;
  private int strstart;
  private int match_start;
  private int lookahead;
  private int prev_length;
  private int max_chain_length;
  private int max_lazy_match;
  private CompressionStrategy strategy;
  private int good_match;
  private int nice_match;
  private short[] dyn_ltree;
  private short[] dyn_dtree;
  private short[] bl_tree;
  private Tree l_desc = new Tree();
  private Tree d_desc = new Tree();
  private Tree bl_desc = new Tree();
  internal short[] bl_count = new short[16 /*0x10*/];
  internal int[] heap = new int[573];
  internal int heap_len;
  internal int heap_max;
  internal byte[] depth = new byte[573];
  internal int l_buf;
  private int lit_bufsize;
  private int last_lit;
  private int d_buf;
  internal int opt_len;
  internal int static_len;
  internal int matches;
  internal int last_eob_len;
  private short bi_buf;
  private int bi_valid;

  public int level { get; set; }

  public int Pending { get; set; }

  public byte[] Pending_buf { get; set; }

  public int Pending_out { get; set; }

  public int NoHeader { get; set; }

  internal Deflate()
  {
    this.dyn_ltree = new short[1146];
    this.dyn_dtree = new short[122];
    this.bl_tree = new short[78];
  }

  private void lm_init()
  {
    this.window_size = 2 * this.w_size;
    Array.Clear((Array) this.head, 0, this.hash_size);
    this.max_lazy_match = Deflate.config_table[this.level].max_lazy;
    this.good_match = Deflate.config_table[this.level].good_length;
    this.nice_match = Deflate.config_table[this.level].nice_length;
    this.max_chain_length = Deflate.config_table[this.level].max_chain;
    this.strstart = 0;
    this.block_start = 0;
    this.lookahead = 0;
    this.match_length = this.prev_length = 2;
    this.match_available = 0;
    this.ins_h = 0;
  }

  private void tr_init()
  {
    this.l_desc.DynTree = this.dyn_ltree;
    this.l_desc.StatDesc = StaticTree.static_l_desc;
    this.d_desc.DynTree = this.dyn_dtree;
    this.d_desc.StatDesc = StaticTree.static_d_desc;
    this.bl_desc.DynTree = this.bl_tree;
    this.bl_desc.StatDesc = StaticTree.static_bl_desc;
    this.bi_buf = (short) 0;
    this.bi_valid = 0;
    this.last_eob_len = 8;
    this.init_block();
  }

  private void init_block()
  {
    for (int index = 0; index < 286; ++index)
      this.dyn_ltree[index * 2] = (short) 0;
    for (int index = 0; index < 30; ++index)
      this.dyn_dtree[index * 2] = (short) 0;
    for (int index = 0; index < 19; ++index)
      this.bl_tree[index * 2] = (short) 0;
    this.dyn_ltree[512 /*0x0200*/] = (short) 1;
    this.opt_len = this.static_len = 0;
    this.last_lit = this.matches = 0;
  }

  internal void pqdownheap(short[] tree, int k)
  {
    int n = this.heap[k];
    for (int index = k << 1; index <= this.heap_len; index <<= 1)
    {
      if (index < this.heap_len && Deflate.smaller(tree, this.heap[index + 1], this.heap[index], this.depth))
        ++index;
      if (!Deflate.smaller(tree, n, this.heap[index], this.depth))
      {
        this.heap[k] = this.heap[index];
        k = index;
      }
      else
        break;
    }
    this.heap[k] = n;
  }

  internal static bool smaller(short[] tree, int n, int m, byte[] depth)
  {
    if ((int) tree[n * 2] < (int) tree[m * 2])
      return true;
    return (int) tree[n * 2] == (int) tree[m * 2] && (int) depth[n] <= (int) depth[m];
  }

  private void scan_tree(short[] tree, int max_code)
  {
    int num1 = -1;
    int num2 = (int) tree[1];
    int num3 = 0;
    int num4 = 7;
    int num5 = 4;
    if (num2 == 0)
    {
      num4 = 138;
      num5 = 3;
    }
    tree[(max_code + 1) * 2 + 1] = (short) ZLibUtil.Identity((long) ushort.MaxValue);
    for (int index = 0; index <= max_code; ++index)
    {
      int num6 = num2;
      num2 = (int) tree[(index + 1) * 2 + 1];
      if (++num3 >= num4 || num6 != num2)
      {
        if (num3 < num5)
          this.bl_tree[num6 * 2] = (short) ((int) this.bl_tree[num6 * 2] + num3);
        else if (num6 != 0)
        {
          if (num6 != num1)
            ++this.bl_tree[num6 * 2];
          ++this.bl_tree[32 /*0x20*/];
        }
        else if (num3 <= 10)
          ++this.bl_tree[34];
        else
          ++this.bl_tree[36];
        num3 = 0;
        num1 = num6;
        if (num2 == 0)
        {
          num4 = 138;
          num5 = 3;
        }
        else if (num6 == num2)
        {
          num4 = 6;
          num5 = 3;
        }
        else
        {
          num4 = 7;
          num5 = 4;
        }
      }
    }
  }

  private int build_bl_tree()
  {
    this.scan_tree(this.dyn_ltree, this.l_desc.MaxCode);
    this.scan_tree(this.dyn_dtree, this.d_desc.MaxCode);
    this.bl_desc.build_tree(this);
    int index = 18;
    while (index >= 3 && this.bl_tree[(int) ZLibUtil.bl_order[index] * 2 + 1] == (short) 0)
      --index;
    this.opt_len += 3 * (index + 1) + 5 + 5 + 4;
    return index;
  }

  private void send_all_trees(int lcodes, int dcodes, int blcodes)
  {
    this.send_bits(lcodes - 257, 5);
    this.send_bits(dcodes - 1, 5);
    this.send_bits(blcodes - 4, 4);
    for (int index = 0; index < blcodes; ++index)
      this.send_bits((int) this.bl_tree[(int) ZLibUtil.bl_order[index] * 2 + 1], 3);
    this.send_tree(this.dyn_ltree, lcodes - 1);
    this.send_tree(this.dyn_dtree, dcodes - 1);
  }

  private void send_tree(short[] tree, int max_code)
  {
    int num1 = -1;
    int num2 = (int) tree[1];
    int num3 = 0;
    int num4 = 7;
    int num5 = 4;
    if (num2 == 0)
    {
      num4 = 138;
      num5 = 3;
    }
    for (int index = 0; index <= max_code; ++index)
    {
      int c = num2;
      num2 = (int) tree[(index + 1) * 2 + 1];
      if (++num3 >= num4 || c != num2)
      {
        if (num3 < num5)
        {
          do
          {
            this.send_code(c, this.bl_tree);
          }
          while (--num3 != 0);
        }
        else if (c != 0)
        {
          if (c != num1)
          {
            this.send_code(c, this.bl_tree);
            --num3;
          }
          this.send_code(16 /*0x10*/, this.bl_tree);
          this.send_bits(num3 - 3, 2);
        }
        else if (num3 <= 10)
        {
          this.send_code(17, this.bl_tree);
          this.send_bits(num3 - 3, 3);
        }
        else
        {
          this.send_code(18, this.bl_tree);
          this.send_bits(num3 - 11, 7);
        }
        num3 = 0;
        num1 = c;
        if (num2 == 0)
        {
          num4 = 138;
          num5 = 3;
        }
        else if (c == num2)
        {
          num4 = 6;
          num5 = 3;
        }
        else
        {
          num4 = 7;
          num5 = 4;
        }
      }
    }
  }

  private void put_byte(byte[] p, int start, int len)
  {
    Array.Copy((Array) p, start, (Array) this.Pending_buf, this.Pending, len);
    this.Pending += len;
  }

  private void put_byte(byte c) => this.Pending_buf[this.Pending++] = c;

  private void put_short(int w)
  {
    this.put_byte((byte) w);
    this.put_byte((byte) ZLibUtil.URShift(w, 8));
  }

  private void putShortMSB(int b)
  {
    this.put_byte((byte) (b >> 8));
    this.put_byte((byte) b);
  }

  private void send_code(int c, short[] tree)
  {
    this.send_bits((int) tree[c * 2] & (int) ushort.MaxValue, (int) tree[c * 2 + 1] & (int) ushort.MaxValue);
  }

  private void send_bits(int value_Renamed, int length)
  {
    int num = length;
    if (this.bi_valid > 16 /*0x10*/ - num)
    {
      int number = value_Renamed;
      this.bi_buf = (short) ((int) (ushort) this.bi_buf | (int) (ushort) (number << this.bi_valid & (int) ushort.MaxValue));
      this.put_short((int) this.bi_buf);
      this.bi_buf = (short) ZLibUtil.URShift(number, 16 /*0x10*/ - this.bi_valid);
      this.bi_valid += num - 16 /*0x10*/;
    }
    else
    {
      this.bi_buf = (short) ((int) (ushort) this.bi_buf | (int) (ushort) (value_Renamed << this.bi_valid & (int) ushort.MaxValue));
      this.bi_valid += num;
    }
  }

  private void _tr_align()
  {
    this.send_bits(2, 3);
    this.send_code(256 /*0x0100*/, StaticTree.static_ltree);
    this.bi_flush();
    if (1 + this.last_eob_len + 10 - this.bi_valid < 9)
    {
      this.send_bits(2, 3);
      this.send_code(256 /*0x0100*/, StaticTree.static_ltree);
      this.bi_flush();
    }
    this.last_eob_len = 7;
  }

  private bool _tr_tally(int dist, int lc)
  {
    this.Pending_buf[this.d_buf + this.last_lit * 2] = (byte) ZLibUtil.URShift(dist, 8);
    this.Pending_buf[this.d_buf + this.last_lit * 2 + 1] = (byte) dist;
    this.Pending_buf[this.l_buf + this.last_lit] = (byte) lc;
    ++this.last_lit;
    if (dist == 0)
    {
      ++this.dyn_ltree[lc * 2];
    }
    else
    {
      ++this.matches;
      --dist;
      ++this.dyn_ltree[((int) ZLibUtil._length_code[lc] + 256 /*0x0100*/ + 1) * 2];
      ++this.dyn_dtree[Tree.d_code(dist) * 2];
    }
    if ((this.last_lit & 8191 /*0x1FFF*/) == 0 && this.level > 2)
    {
      int number = this.last_lit * 8;
      int num1 = this.strstart - this.block_start;
      for (int index = 0; index < 30; ++index)
        number = (int) ((long) number + (long) this.dyn_dtree[index * 2] * (5L + (long) ZLibUtil.extra_dbits[index]));
      int num2 = ZLibUtil.URShift(number, 3);
      if (this.matches < this.last_lit / 2 && num2 < num1 / 2)
        return true;
    }
    return this.last_lit == this.lit_bufsize - 1;
  }

  private void compress_block(short[] ltree, short[] dtree)
  {
    int num1 = 0;
    if (this.last_lit != 0)
    {
      do
      {
        int num2 = (int) this.Pending_buf[this.d_buf + num1 * 2] << 8 & 65280 | (int) this.Pending_buf[this.d_buf + num1 * 2 + 1] & (int) byte.MaxValue;
        int c1 = (int) this.Pending_buf[this.l_buf + num1] & (int) byte.MaxValue;
        ++num1;
        if (num2 == 0)
        {
          this.send_code(c1, ltree);
        }
        else
        {
          int index = (int) ZLibUtil._length_code[c1];
          this.send_code(index + 256 /*0x0100*/ + 1, ltree);
          int extraLbit = ZLibUtil.extra_lbits[index];
          if (extraLbit != 0)
            this.send_bits(c1 - ZLibUtil.base_length[index], extraLbit);
          int dist = num2 - 1;
          int c2 = Tree.d_code(dist);
          this.send_code(c2, dtree);
          int extraDbit = ZLibUtil.extra_dbits[c2];
          if (extraDbit != 0)
            this.send_bits(dist - ZLibUtil.base_dist[c2], extraDbit);
        }
      }
      while (num1 < this.last_lit);
    }
    this.send_code(256 /*0x0100*/, ltree);
    this.last_eob_len = (int) ltree[513];
  }

  private void set_data_type()
  {
    int num1 = 0;
    int number = 0;
    int num2 = 0;
    for (; num1 < 7; ++num1)
      num2 += (int) this.dyn_ltree[num1 * 2];
    for (; num1 < 128 /*0x80*/; ++num1)
      number += (int) this.dyn_ltree[num1 * 2];
    for (; num1 < 256 /*0x0100*/; ++num1)
      num2 += (int) this.dyn_ltree[num1 * 2];
    this.data_type = num2 > ZLibUtil.URShift(number, 2) ? BlockType.Z_BINARY : BlockType.Z_ASCII;
  }

  private void bi_flush()
  {
    if (this.bi_valid == 16 /*0x10*/)
    {
      this.put_short((int) this.bi_buf);
      this.bi_buf = (short) 0;
      this.bi_valid = 0;
    }
    else
    {
      if (this.bi_valid < 8)
        return;
      this.put_byte((byte) this.bi_buf);
      this.bi_buf = (short) ZLibUtil.URShift((int) this.bi_buf, 8);
      this.bi_valid -= 8;
    }
  }

  private void bi_windup()
  {
    if (this.bi_valid > 8)
      this.put_short((int) this.bi_buf);
    else if (this.bi_valid > 0)
      this.put_byte((byte) this.bi_buf);
    this.bi_buf = (short) 0;
    this.bi_valid = 0;
  }

  private void copy_block(int buf, int len, bool header)
  {
    this.bi_windup();
    this.last_eob_len = 8;
    if (header)
    {
      this.put_short((int) (short) len);
      this.put_short((int) (short) ~len);
    }
    this.put_byte(this.window, buf, len);
  }

  private void flush_block_only(bool eof)
  {
    this._tr_flush_block(this.block_start >= 0 ? this.block_start : -1, this.strstart - this.block_start, eof);
    this.block_start = this.strstart;
    this.strm.FlushPending();
  }

  private int deflate_stored(int flush)
  {
    int num1 = (int) ushort.MaxValue;
    if (num1 > this.pending_buf_size - 5)
      num1 = this.pending_buf_size - 5;
    do
    {
      do
      {
        if (this.lookahead <= 1)
        {
          this.fill_window();
          if (this.lookahead == 0 && flush == 0)
            return 0;
          if (this.lookahead == 0)
            goto label_12;
        }
        this.strstart += this.lookahead;
        this.lookahead = 0;
        int num2 = this.block_start + num1;
        if (this.strstart == 0 || this.strstart >= num2)
        {
          this.lookahead = this.strstart - num2;
          this.strstart = num2;
          this.flush_block_only(false);
          if (this.strm.avail_out == 0)
            return 0;
        }
      }
      while (this.strstart - this.block_start < this.w_size - 262);
      this.flush_block_only(false);
    }
    while (this.strm.avail_out != 0);
    return 0;
label_12:
    this.flush_block_only(flush == 4);
    return this.strm.avail_out == 0 ? (flush != 4 ? 0 : 2) : (flush != 4 ? 1 : 3);
  }

  private void _tr_stored_block(int buf, int stored_len, bool eof)
  {
    this.send_bits(eof ? 1 : 0, 3);
    this.copy_block(buf, stored_len, true);
  }

  private void _tr_flush_block(int buf, int stored_len, bool eof)
  {
    int num1 = 0;
    int num2;
    int num3;
    if (this.level > 0)
    {
      if (this.data_type == BlockType.Z_UNKNOWN)
        this.set_data_type();
      this.l_desc.build_tree(this);
      this.d_desc.build_tree(this);
      num1 = this.build_bl_tree();
      num2 = ZLibUtil.URShift(this.opt_len + 3 + 7, 3);
      num3 = ZLibUtil.URShift(this.static_len + 3 + 7, 3);
      if (num3 <= num2)
        num2 = num3;
    }
    else
      num2 = num3 = stored_len + 5;
    if (stored_len + 4 <= num2 && buf != -1)
      this._tr_stored_block(buf, stored_len, eof);
    else if (num3 == num2)
    {
      this.send_bits(2 + (eof ? 1 : 0), 3);
      this.compress_block(StaticTree.static_ltree, StaticTree.static_dtree);
    }
    else
    {
      this.send_bits(4 + (eof ? 1 : 0), 3);
      this.send_all_trees(this.l_desc.MaxCode + 1, this.d_desc.MaxCode + 1, num1 + 1);
      this.compress_block(this.dyn_ltree, this.dyn_dtree);
    }
    this.init_block();
    if (!eof)
      return;
    this.bi_windup();
  }

  private void fill_window()
  {
    do
    {
      int size = this.window_size - this.lookahead - this.strstart;
      if (size == 0 && this.strstart == 0 && this.lookahead == 0)
        size = this.w_size;
      else if (size == -1)
        --size;
      else if (this.strstart >= this.w_size + this.w_size - 262)
      {
        Array.Copy((Array) this.window, this.w_size, (Array) this.window, 0, this.w_size);
        this.match_start -= this.w_size;
        this.strstart -= this.w_size;
        this.block_start -= this.w_size;
        int hashSize = this.hash_size;
        int index1 = hashSize;
        do
        {
          int num = (int) this.head[--index1] & (int) ushort.MaxValue;
          this.head[index1] = num >= this.w_size ? (short) (num - this.w_size) : (short) 0;
        }
        while (--hashSize != 0);
        int wSize = this.w_size;
        int index2 = wSize;
        do
        {
          int num = (int) this.prev[--index2] & (int) ushort.MaxValue;
          this.prev[index2] = num >= this.w_size ? (short) (num - this.w_size) : (short) 0;
        }
        while (--wSize != 0);
        size += this.w_size;
      }
      if (this.strm.avail_in == 0)
        break;
      this.lookahead += this.strm.ReadBuf(this.window, this.strstart + this.lookahead, size);
      if (this.lookahead >= 3)
      {
        this.ins_h = (int) this.window[this.strstart] & (int) byte.MaxValue;
        this.ins_h = (this.ins_h << this.hash_shift ^ (int) this.window[this.strstart + 1] & (int) byte.MaxValue) & this.hash_mask;
      }
    }
    while (this.lookahead < 262 && this.strm.avail_in != 0);
  }

  private int deflate_fast(int flush)
  {
    int cur_match = 0;
    do
    {
      bool flag;
      do
      {
        if (this.lookahead < 262)
        {
          this.fill_window();
          if (this.lookahead < 262 && flush == 0)
            return 0;
          if (this.lookahead == 0)
            goto label_19;
        }
        if (this.lookahead >= 3)
        {
          this.ins_h = (this.ins_h << this.hash_shift ^ (int) this.window[this.strstart + 2] & (int) byte.MaxValue) & this.hash_mask;
          cur_match = (int) this.head[this.ins_h] & (int) ushort.MaxValue;
          this.prev[this.strstart & this.w_mask] = this.head[this.ins_h];
          this.head[this.ins_h] = (short) this.strstart;
        }
        if (cur_match != 0 && (this.strstart - cur_match & (int) ushort.MaxValue) <= this.w_size - 262 && this.strategy != CompressionStrategy.Z_HUFFMAN_ONLY)
          this.match_length = this.longest_match(cur_match);
        if (this.match_length >= 3)
        {
          flag = this._tr_tally(this.strstart - this.match_start, this.match_length - 3);
          this.lookahead -= this.match_length;
          if (this.match_length <= this.max_lazy_match && this.lookahead >= 3)
          {
            --this.match_length;
            do
            {
              ++this.strstart;
              this.ins_h = (this.ins_h << this.hash_shift ^ (int) this.window[this.strstart + 2] & (int) byte.MaxValue) & this.hash_mask;
              cur_match = (int) this.head[this.ins_h] & (int) ushort.MaxValue;
              this.prev[this.strstart & this.w_mask] = this.head[this.ins_h];
              this.head[this.ins_h] = (short) this.strstart;
            }
            while (--this.match_length != 0);
            ++this.strstart;
          }
          else
          {
            this.strstart += this.match_length;
            this.match_length = 0;
            this.ins_h = (int) this.window[this.strstart] & (int) byte.MaxValue;
            this.ins_h = (this.ins_h << this.hash_shift ^ (int) this.window[this.strstart + 1] & (int) byte.MaxValue) & this.hash_mask;
          }
        }
        else
        {
          flag = this._tr_tally(0, (int) this.window[this.strstart] & (int) byte.MaxValue);
          --this.lookahead;
          ++this.strstart;
        }
      }
      while (!flag);
      this.flush_block_only(false);
    }
    while (this.strm.avail_out != 0);
    return 0;
label_19:
    this.flush_block_only(flush == 4);
    return this.strm.avail_out == 0 ? (flush == 4 ? 2 : 0) : (flush != 4 ? 1 : 3);
  }

  private int deflate_slow(int flush)
  {
    int cur_match = 0;
    while (true)
    {
      do
      {
        if (this.lookahead < 262)
        {
          this.fill_window();
          if (this.lookahead < 262 && flush == 0)
            return 0;
          if (this.lookahead == 0)
            goto label_26;
        }
        if (this.lookahead >= 3)
        {
          this.ins_h = (this.ins_h << this.hash_shift ^ (int) this.window[this.strstart + 2] & (int) byte.MaxValue) & this.hash_mask;
          cur_match = (int) this.head[this.ins_h] & (int) ushort.MaxValue;
          this.prev[this.strstart & this.w_mask] = this.head[this.ins_h];
          this.head[this.ins_h] = (short) this.strstart;
        }
        this.prev_length = this.match_length;
        this.prev_match = this.match_start;
        this.match_length = 2;
        if (cur_match != 0 && this.prev_length < this.max_lazy_match && (this.strstart - cur_match & (int) ushort.MaxValue) <= this.w_size - 262)
        {
          if (this.strategy != CompressionStrategy.Z_HUFFMAN_ONLY)
            this.match_length = this.longest_match(cur_match);
          if (this.match_length <= 5 && (this.strategy == CompressionStrategy.Z_FILTERED || this.match_length == 3 && this.strstart - this.match_start > 4096 /*0x1000*/))
            this.match_length = 2;
        }
        if (this.prev_length >= 3 && this.match_length <= this.prev_length)
        {
          int num = this.strstart + this.lookahead - 3;
          bool flag = this._tr_tally(this.strstart - 1 - this.prev_match, this.prev_length - 3);
          this.lookahead -= this.prev_length - 1;
          this.prev_length -= 2;
          do
          {
            if (++this.strstart <= num)
            {
              this.ins_h = (this.ins_h << this.hash_shift ^ (int) this.window[this.strstart + 2] & (int) byte.MaxValue) & this.hash_mask;
              cur_match = (int) this.head[this.ins_h] & (int) ushort.MaxValue;
              this.prev[this.strstart & this.w_mask] = this.head[this.ins_h];
              this.head[this.ins_h] = (short) this.strstart;
            }
          }
          while (--this.prev_length != 0);
          this.match_available = 0;
          this.match_length = 2;
          ++this.strstart;
          if (flag)
          {
            this.flush_block_only(false);
            if (this.strm.avail_out == 0)
              return 0;
          }
        }
        else if (this.match_available != 0)
        {
          if (this._tr_tally(0, (int) this.window[this.strstart - 1] & (int) byte.MaxValue))
            this.flush_block_only(false);
          ++this.strstart;
          --this.lookahead;
        }
        else
          goto label_25;
      }
      while (this.strm.avail_out != 0);
      break;
label_25:
      this.match_available = 1;
      ++this.strstart;
      --this.lookahead;
    }
    return 0;
label_26:
    if (this.match_available != 0)
    {
      this._tr_tally(0, (int) this.window[this.strstart - 1] & (int) byte.MaxValue);
      this.match_available = 0;
    }
    this.flush_block_only(flush == 4);
    return this.strm.avail_out == 0 ? (flush == 4 ? 2 : 0) : (flush != 4 ? 1 : 3);
  }

  private int longest_match(int cur_match)
  {
    int maxChainLength = this.max_chain_length;
    int index1 = this.strstart;
    int num1 = this.prev_length;
    int num2 = this.strstart > this.w_size - 262 ? this.strstart - (this.w_size - 262) : 0;
    int num3 = this.nice_match;
    int wMask = this.w_mask;
    int num4 = this.strstart + 258;
    byte num5 = this.window[index1 + num1 - 1];
    byte num6 = this.window[index1 + num1];
    if (this.prev_length >= this.good_match)
      maxChainLength >>= 2;
    if (num3 > this.lookahead)
      num3 = this.lookahead;
    do
    {
      int index2 = cur_match;
      int num7;
      if ((int) this.window[index2 + num1] == (int) num6 && (int) this.window[index2 + num1 - 1] == (int) num5 && (int) this.window[index2] == (int) this.window[index1] && (int) this.window[num7 = index2 + 1] == (int) this.window[index1 + 1])
      {
        int num8 = index1 + 2;
        int num9 = num7 + 1;
        int num10;
        int num11;
        int num12;
        int num13;
        int num14;
        int num15;
        int num16;
        do
          ;
        while ((int) this.window[++num8] == (int) this.window[num10 = num9 + 1] && (int) this.window[++num8] == (int) this.window[num11 = num10 + 1] && (int) this.window[++num8] == (int) this.window[num12 = num11 + 1] && (int) this.window[++num8] == (int) this.window[num13 = num12 + 1] && (int) this.window[++num8] == (int) this.window[num14 = num13 + 1] && (int) this.window[++num8] == (int) this.window[num15 = num14 + 1] && (int) this.window[++num8] == (int) this.window[num16 = num15 + 1] && (int) this.window[++num8] == (int) this.window[num9 = num16 + 1] && num8 < num4);
        int num17 = 258 - (num4 - num8);
        index1 = num4 - 258;
        if (num17 > num1)
        {
          this.match_start = cur_match;
          num1 = num17;
          if (num17 < num3)
          {
            num5 = this.window[index1 + num1 - 1];
            num6 = this.window[index1 + num1];
          }
          else
            break;
        }
      }
    }
    while ((cur_match = (int) this.prev[cur_match & wMask] & (int) ushort.MaxValue) > num2 && --maxChainLength != 0);
    return num1 <= this.lookahead ? num1 : this.lookahead;
  }

  internal int DeflateInit(ZStream strm, int level, int bits)
  {
    return this.DeflateInit2(strm, level, bits, 8, CompressionStrategy.Z_DEFAULT_STRATEGY);
  }

  internal int DeflateInit(ZStream strm, int level) => this.DeflateInit(strm, level, 15);

  internal int DeflateInit2(
    ZStream strm,
    int level,
    int windowBits,
    int memLevel,
    CompressionStrategy strategy)
  {
    int num = 0;
    strm.msg = (string) null;
    if (level == -1)
      level = 6;
    if (windowBits < 0)
    {
      num = 1;
      windowBits = -windowBits;
    }
    if (memLevel < 1 || memLevel > 9 || windowBits < 9 || windowBits > 15 || level < 0 || level > 9 || strategy < CompressionStrategy.Z_DEFAULT_STRATEGY || strategy > CompressionStrategy.Z_HUFFMAN_ONLY)
      return -2;
    strm.dstate = this;
    this.NoHeader = num;
    this.w_bits = windowBits;
    this.w_size = 1 << this.w_bits;
    this.w_mask = this.w_size - 1;
    this.hash_bits = memLevel + 7;
    this.hash_size = 1 << this.hash_bits;
    this.hash_mask = this.hash_size - 1;
    this.hash_shift = (this.hash_bits + 3 - 1) / 3;
    this.window = new byte[this.w_size * 2];
    this.prev = new short[this.w_size];
    this.head = new short[this.hash_size];
    this.lit_bufsize = 1 << memLevel + 6;
    this.Pending_buf = new byte[this.lit_bufsize * 4];
    this.pending_buf_size = this.lit_bufsize * 4;
    this.d_buf = this.lit_bufsize;
    this.l_buf = 3 * this.lit_bufsize;
    this.level = level;
    this.strategy = strategy;
    this.method = (byte) 8;
    return this.deflateReset(strm);
  }

  internal int deflateReset(ZStream strm)
  {
    strm.total_in = strm.total_out = 0L;
    strm.msg = (string) null;
    strm.Data_type = BlockType.Z_UNKNOWN;
    this.Pending = 0;
    this.Pending_out = 0;
    if (this.NoHeader < 0)
      this.NoHeader = 0;
    this.status = this.NoHeader != 0 ? DeflateState.BUSY_STATE : DeflateState.INIT_STATE;
    strm.adler = Adler32.GetAdler32Checksum(0L, (byte[]) null, 0, 0);
    this.last_flush = 0;
    this.tr_init();
    this.lm_init();
    return 0;
  }

  internal int deflateEnd()
  {
    if (this.status != DeflateState.INIT_STATE && this.status != DeflateState.BUSY_STATE && this.status != DeflateState.FINISH_STATE)
      return -2;
    this.Pending_buf = (byte[]) null;
    this.head = (short[]) null;
    this.prev = (short[]) null;
    this.window = (byte[]) null;
    return this.status != DeflateState.BUSY_STATE ? 0 : -3;
  }

  internal int deflateParams(ZStream strm, int level, CompressionStrategy strategy)
  {
    int num = 0;
    if (level == -1)
      level = 6;
    if (level < 0 || level > 9 || strategy < CompressionStrategy.Z_DEFAULT_STRATEGY || strategy > CompressionStrategy.Z_HUFFMAN_ONLY)
      return -2;
    if (Deflate.config_table[this.level].func != Deflate.config_table[level].func && strm.total_in != 0L)
      num = strm.deflate(FlushStrategy.Z_PARTIAL_FLUSH);
    if (this.level != level)
    {
      this.level = level;
      this.max_lazy_match = Deflate.config_table[this.level].max_lazy;
      this.good_match = Deflate.config_table[this.level].good_length;
      this.nice_match = Deflate.config_table[this.level].nice_length;
      this.max_chain_length = Deflate.config_table[this.level].max_chain;
    }
    this.strategy = strategy;
    return num;
  }

  internal int deflateSetDictionary(ZStream strm, byte[] dictionary, int dictLength)
  {
    int length = dictLength;
    int sourceIndex = 0;
    if (dictionary == null || this.status != DeflateState.INIT_STATE)
      return -2;
    strm.adler = Adler32.GetAdler32Checksum(strm.adler, dictionary, 0, dictLength);
    if (length < 3)
      return 0;
    if (length > this.w_size - 262)
    {
      length = this.w_size - 262;
      sourceIndex = dictLength - length;
    }
    Array.Copy((Array) dictionary, sourceIndex, (Array) this.window, 0, length);
    this.strstart = length;
    this.block_start = length;
    this.ins_h = (int) this.window[0] & (int) byte.MaxValue;
    this.ins_h = (this.ins_h << this.hash_shift ^ (int) this.window[1] & (int) byte.MaxValue) & this.hash_mask;
    for (int index = 0; index <= length - 3; ++index)
    {
      this.ins_h = (this.ins_h << this.hash_shift ^ (int) this.window[index + 2] & (int) byte.MaxValue) & this.hash_mask;
      this.prev[index & this.w_mask] = this.head[this.ins_h];
      this.head[this.ins_h] = (short) index;
    }
    return 0;
  }

  internal int deflate(ZStream strm, FlushStrategy f)
  {
    int flush = (int) f;
    if (flush > 4 || flush < 0)
      return -2;
    if (strm.next_out == null || strm.next_in == null && strm.avail_in != 0 || this.status == DeflateState.FINISH_STATE && flush != 4)
    {
      strm.msg = ZLibUtil.z_errmsg[4];
      return -2;
    }
    if (strm.avail_out == 0)
    {
      strm.msg = ZLibUtil.z_errmsg[7];
      return -5;
    }
    this.strm = strm;
    int lastFlush = this.last_flush;
    this.last_flush = flush;
    if (this.status == DeflateState.INIT_STATE)
    {
      int num1 = 8 + (this.w_bits - 8 << 4) << 8;
      int num2 = this.level > 0 ? (this.level - 1 & (int) byte.MaxValue) >> 1 : 0;
      if (num2 > 3)
        num2 = 3;
      int num3 = num1 | num2 << 6;
      if (this.strstart != 0)
        num3 |= 32 /*0x20*/;
      int b = num3 + (31 /*0x1F*/ - num3 % 31 /*0x1F*/);
      this.status = DeflateState.BUSY_STATE;
      this.putShortMSB(b);
      if (this.strstart != 0)
      {
        this.putShortMSB((int) ZLibUtil.URShift(strm.adler, 16 /*0x10*/));
        this.putShortMSB((int) (strm.adler & (long) ushort.MaxValue));
      }
      strm.adler = Adler32.GetAdler32Checksum(0L, (byte[]) null, 0, 0);
    }
    if (this.Pending != 0)
    {
      strm.FlushPending();
      if (strm.avail_out == 0)
      {
        this.last_flush = -1;
        return 0;
      }
    }
    else if (strm.avail_in == 0 && flush <= lastFlush && flush != 4)
    {
      strm.msg = ZLibUtil.z_errmsg[7];
      return -5;
    }
    if (this.status == DeflateState.FINISH_STATE && strm.avail_in != 0)
    {
      strm.msg = ZLibUtil.z_errmsg[7];
      return -5;
    }
    if (strm.avail_in != 0 || this.lookahead != 0 || flush != 0 && this.status != DeflateState.FINISH_STATE)
    {
      int num = -1;
      switch (Deflate.config_table[this.level].func)
      {
        case 0:
          num = this.deflate_stored(flush);
          break;
        case 1:
          num = this.deflate_fast(flush);
          break;
        case 2:
          num = this.deflate_slow(flush);
          break;
      }
      if (num == 2 || num == 3)
        this.status = DeflateState.FINISH_STATE;
      if (num == 0 || num == 2)
      {
        if (strm.avail_out == 0)
          this.last_flush = -1;
        return 0;
      }
      if (num == 1)
      {
        if (flush == 1)
        {
          this._tr_align();
        }
        else
        {
          this._tr_stored_block(0, 0, false);
          if (flush == 3)
          {
            for (int index = 0; index < this.hash_size; ++index)
              this.head[index] = (short) 0;
          }
        }
        strm.FlushPending();
        if (strm.avail_out == 0)
        {
          this.last_flush = -1;
          return 0;
        }
      }
    }
    if (flush != 4)
      return 0;
    if (this.NoHeader != 0)
      return 1;
    this.putShortMSB((int) ZLibUtil.URShift(strm.adler, 16 /*0x10*/));
    this.putShortMSB((int) (strm.adler & (long) ushort.MaxValue));
    strm.FlushPending();
    this.NoHeader = -1;
    return this.Pending == 0 ? 1 : 0;
  }

  static Deflate()
  {
    Deflate.config_table[0] = new Deflate.Config(0, 0, 0, 0, 0);
    Deflate.config_table[1] = new Deflate.Config(4, 4, 8, 4, 1);
    Deflate.config_table[2] = new Deflate.Config(4, 5, 16 /*0x10*/, 8, 1);
    Deflate.config_table[3] = new Deflate.Config(4, 6, 32 /*0x20*/, 32 /*0x20*/, 1);
    Deflate.config_table[4] = new Deflate.Config(4, 4, 16 /*0x10*/, 16 /*0x10*/, 2);
    Deflate.config_table[5] = new Deflate.Config(8, 16 /*0x10*/, 32 /*0x20*/, 32 /*0x20*/, 2);
    Deflate.config_table[6] = new Deflate.Config(8, 16 /*0x10*/, 128 /*0x80*/, 128 /*0x80*/, 2);
    Deflate.config_table[7] = new Deflate.Config(8, 32 /*0x20*/, 128 /*0x80*/, 256 /*0x0100*/, 2);
    Deflate.config_table[8] = new Deflate.Config(32 /*0x20*/, 128 /*0x80*/, 258, 1024 /*0x0400*/, 2);
    Deflate.config_table[9] = new Deflate.Config(32 /*0x20*/, 258, 258, 4096 /*0x1000*/, 2);
  }

  internal class Config
  {
    internal int good_length;
    internal int max_lazy;
    internal int nice_length;
    internal int max_chain;
    internal int func;

    internal Config(int good_length, int max_lazy, int nice_length, int max_chain, int func)
    {
      this.good_length = good_length;
      this.max_lazy = max_lazy;
      this.nice_length = nice_length;
      this.max_chain = max_chain;
      this.func = func;
    }
  }
}
