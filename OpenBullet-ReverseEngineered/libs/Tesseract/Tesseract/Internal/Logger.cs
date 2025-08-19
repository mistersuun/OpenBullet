// Decompiled with JetBrains decompiler
// Type: Tesseract.Internal.Logger
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System.Diagnostics;

#nullable disable
namespace Tesseract.Internal;

internal static class Logger
{
  private static readonly TraceSource trace = new TraceSource("Tesseract");

  public static void TraceInformation(string format, params object[] args)
  {
  }

  public static void TraceError(string format, params object[] args)
  {
  }

  public static void TraceWarning(string format, params object[] args)
  {
  }
}
