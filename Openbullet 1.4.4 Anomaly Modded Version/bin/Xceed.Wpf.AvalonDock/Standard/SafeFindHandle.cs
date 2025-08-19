// Decompiled with JetBrains decompiler
// Type: Standard.SafeFindHandle
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using Microsoft.Win32.SafeHandles;
using System.Security.Permissions;

#nullable disable
namespace Standard;

internal sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
{
  [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
  private SafeFindHandle()
    : base(true)
  {
  }

  protected override bool ReleaseHandle() => NativeMethods.FindClose(this.handle);
}
