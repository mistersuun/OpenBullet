// Decompiled with JetBrains decompiler
// Type: Standard.SafeHBITMAP
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;

#nullable disable
namespace Standard;

internal sealed class SafeHBITMAP : SafeHandleZeroOrMinusOneIsInvalid
{
  private SafeHBITMAP()
    : base(true)
  {
  }

  [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
  protected override bool ReleaseHandle() => NativeMethods.DeleteObject(this.handle);
}
