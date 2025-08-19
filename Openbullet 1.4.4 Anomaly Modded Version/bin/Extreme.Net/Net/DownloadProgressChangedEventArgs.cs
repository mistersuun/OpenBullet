// Decompiled with JetBrains decompiler
// Type: Extreme.Net.DownloadProgressChangedEventArgs
// Assembly: Extreme.Net, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B85A5720-FE8B-4A1B-9FD2-F7651D37B15B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Extreme.Net.dll

using System;

#nullable disable
namespace Extreme.Net;

public sealed class DownloadProgressChangedEventArgs : EventArgs
{
  public long BytesReceived { get; private set; }

  public long TotalBytesToReceive { get; private set; }

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
