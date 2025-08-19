// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.NativeMethods
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace OpenQA.Selenium;

internal static class NativeMethods
{
  [DllImport("kernel32")]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static extern bool SetHandleInformation(
    IntPtr hObject,
    NativeMethods.HandleInformation dwMask,
    NativeMethods.HandleInformation dwFlags);

  [Flags]
  internal enum HandleInformation
  {
    None = 0,
    Inherit = 1,
    ProtectFromClose = 2,
  }
}
