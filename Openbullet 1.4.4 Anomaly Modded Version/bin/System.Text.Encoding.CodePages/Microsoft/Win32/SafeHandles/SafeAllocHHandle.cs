// Decompiled with JetBrains decompiler
// Type: Microsoft.Win32.SafeHandles.SafeAllocHHandle
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Win32.SafeHandles;

internal sealed class SafeAllocHHandle : SafeBuffer
{
  private SafeAllocHHandle()
    : base(true)
  {
  }

  internal SafeAllocHHandle(IntPtr handle)
    : base(true)
  {
    this.SetHandle(handle);
  }

  internal static SafeAllocHHandle InvalidHandle => new SafeAllocHHandle(IntPtr.Zero);

  protected override bool ReleaseHandle()
  {
    if (this.handle != IntPtr.Zero)
      Marshal.FreeHGlobal(this.handle);
    return true;
  }
}
