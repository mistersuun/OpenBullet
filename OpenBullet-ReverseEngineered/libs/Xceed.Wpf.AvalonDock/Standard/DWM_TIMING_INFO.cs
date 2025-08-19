// Decompiled with JetBrains decompiler
// Type: Standard.DWM_TIMING_INFO
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct DWM_TIMING_INFO
{
  public int cbSize;
  public UNSIGNED_RATIO rateRefresh;
  public ulong qpcRefreshPeriod;
  public UNSIGNED_RATIO rateCompose;
  public ulong qpcVBlank;
  public ulong cRefresh;
  public uint cDXRefresh;
  public ulong qpcCompose;
  public ulong cFrame;
  public uint cDXPresent;
  public ulong cRefreshFrame;
  public ulong cFrameSubmitted;
  public uint cDXPresentSubmitted;
  public ulong cFrameConfirmed;
  public uint cDXPresentConfirmed;
  public ulong cRefreshConfirmed;
  public uint cDXRefreshConfirmed;
  public ulong cFramesLate;
  public uint cFramesOutstanding;
  public ulong cFrameDisplayed;
  public ulong qpcFrameDisplayed;
  public ulong cRefreshFrameDisplayed;
  public ulong cFrameComplete;
  public ulong qpcFrameComplete;
  public ulong cFramePending;
  public ulong qpcFramePending;
  public ulong cFramesDisplayed;
  public ulong cFramesComplete;
  public ulong cFramesPending;
  public ulong cFramesAvailable;
  public ulong cFramesDropped;
  public ulong cFramesMissed;
  public ulong cRefreshNextDisplayed;
  public ulong cRefreshNextPresented;
  public ulong cRefreshesDisplayed;
  public ulong cRefreshesPresented;
  public ulong cRefreshStarted;
  public ulong cPixelsReceived;
  public ulong cPixelsDrawn;
  public ulong cBuffersEmpty;
}
