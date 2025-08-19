// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.UploadProgressChangedEventArgs
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;

#nullable disable
namespace Leaf.xNet;

public sealed class UploadProgressChangedEventArgs : EventArgs
{
  public long BytesSent { get; }

  public long TotalBytesToSend { get; }

  public double ProgressPercentage
  {
    get => (double) this.BytesSent / (double) this.TotalBytesToSend * 100.0;
  }

  public UploadProgressChangedEventArgs(long bytesSent, long totalBytesToSend)
  {
    this.BytesSent = bytesSent;
    this.TotalBytesToSend = totalBytesToSend;
  }
}
