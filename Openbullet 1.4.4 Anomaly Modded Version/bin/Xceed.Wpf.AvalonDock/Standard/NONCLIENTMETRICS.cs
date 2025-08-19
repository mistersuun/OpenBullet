// Decompiled with JetBrains decompiler
// Type: Standard.NONCLIENTMETRICS
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

internal struct NONCLIENTMETRICS
{
  public int cbSize;
  public int iBorderWidth;
  public int iScrollWidth;
  public int iScrollHeight;
  public int iCaptionWidth;
  public int iCaptionHeight;
  public LOGFONT lfCaptionFont;
  public int iSmCaptionWidth;
  public int iSmCaptionHeight;
  public LOGFONT lfSmCaptionFont;
  public int iMenuWidth;
  public int iMenuHeight;
  public LOGFONT lfMenuFont;
  public LOGFONT lfStatusFont;
  public LOGFONT lfMessageFont;
  public int iPaddedBorderWidth;

  public static NONCLIENTMETRICS VistaMetricsStruct
  {
    get
    {
      return new NONCLIENTMETRICS()
      {
        cbSize = Marshal.SizeOf(typeof (NONCLIENTMETRICS))
      };
    }
  }

  public static NONCLIENTMETRICS XPMetricsStruct
  {
    get
    {
      return new NONCLIENTMETRICS()
      {
        cbSize = Marshal.SizeOf(typeof (NONCLIENTMETRICS)) - 4
      };
    }
  }
}
