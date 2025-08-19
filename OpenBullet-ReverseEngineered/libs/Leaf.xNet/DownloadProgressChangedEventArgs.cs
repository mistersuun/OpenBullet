// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.DownloadProgressChangedEventArgs
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;

#nullable disable
namespace Leaf.xNet;

public sealed class DownloadProgressChangedEventArgs : EventArgs
{
  public long BytesReceived { get; }

  public long TotalBytesToReceive { get; }

  public double ProgressPercentage
  {
    get => (double) this.BytesReceived / (double) this.TotalBytesToReceive * 100.0;
  }

  public DownloadProgressChangedEventArgs(long bytesReceived, long totalBytesToReceive)
  {
    this.BytesReceived = bytesReceived;
    this.TotalBytesToReceive = totalBytesToReceive;
  }
}
