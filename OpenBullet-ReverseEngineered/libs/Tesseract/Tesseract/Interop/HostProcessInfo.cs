// Decompiled with JetBrains decompiler
// Type: Tesseract.Interop.HostProcessInfo
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;

#nullable disable
namespace Tesseract.Interop;

internal static class HostProcessInfo
{
  public static readonly bool Is64Bit = IntPtr.Size == 8;
}
