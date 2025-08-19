// Decompiled with JetBrains decompiler
// Type: Standard.SafeGdiplusStartupToken
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;

#nullable disable
namespace Standard;

internal sealed class SafeGdiplusStartupToken : SafeHandleZeroOrMinusOneIsInvalid
{
  private SafeGdiplusStartupToken()
    : base(true)
  {
  }

  [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
  protected override bool ReleaseHandle()
  {
    return NativeMethods.GdiplusShutdown(this.handle) == Status.Ok;
  }

  public static SafeGdiplusStartupToken Startup()
  {
    SafeGdiplusStartupToken gdiplusStartupToken = new SafeGdiplusStartupToken();
    IntPtr token;
    if (NativeMethods.GdiplusStartup(out token, new StartupInput(), out StartupOutput _) == Status.Ok)
    {
      gdiplusStartupToken.handle = token;
      return gdiplusStartupToken;
    }
    gdiplusStartupToken.Dispose();
    throw new Exception("Unable to initialize GDI+");
  }
}
