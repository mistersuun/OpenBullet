// Decompiled with JetBrains decompiler
// Type: Ionic.BZip2.WorkItem
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using System.IO;

#nullable disable
namespace Ionic.BZip2;

internal class WorkItem
{
  public int index;
  public MemoryStream ms;
  public int ordinal;
  public BitWriter bw;

  public BZip2Compressor Compressor { get; private set; }

  public WorkItem(int ix, int blockSize)
  {
    this.ms = new MemoryStream();
    this.bw = new BitWriter((Stream) this.ms);
    this.Compressor = new BZip2Compressor(this.bw, blockSize);
    this.index = ix;
  }
}
