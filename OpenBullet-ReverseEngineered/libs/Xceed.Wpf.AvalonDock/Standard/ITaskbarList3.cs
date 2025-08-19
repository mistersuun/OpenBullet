// Decompiled with JetBrains decompiler
// Type: Standard.ITaskbarList3
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
[ComImport]
internal interface ITaskbarList3 : ITaskbarList2, ITaskbarList
{
  new void HrInit();

  new void AddTab(IntPtr hwnd);

  new void DeleteTab(IntPtr hwnd);

  new void ActivateTab(IntPtr hwnd);

  new void SetActiveAlt(IntPtr hwnd);

  new void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

  [MethodImpl(MethodImplOptions.PreserveSig)]
  HRESULT SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal);

  [MethodImpl(MethodImplOptions.PreserveSig)]
  HRESULT SetProgressState(IntPtr hwnd, TBPF tbpFlags);

  [MethodImpl(MethodImplOptions.PreserveSig)]
  HRESULT RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);

  [MethodImpl(MethodImplOptions.PreserveSig)]
  HRESULT UnregisterTab(IntPtr hwndTab);

  [MethodImpl(MethodImplOptions.PreserveSig)]
  HRESULT SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);

  [MethodImpl(MethodImplOptions.PreserveSig)]
  HRESULT SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, uint dwReserved);

  [MethodImpl(MethodImplOptions.PreserveSig)]
  HRESULT ThumbBarAddButtons(IntPtr hwnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] THUMBBUTTON[] pButtons);

  [MethodImpl(MethodImplOptions.PreserveSig)]
  HRESULT ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] THUMBBUTTON[] pButtons);

  [MethodImpl(MethodImplOptions.PreserveSig)]
  HRESULT ThumbBarSetImageList(IntPtr hwnd, [MarshalAs(UnmanagedType.IUnknown)] object himl);

  [MethodImpl(MethodImplOptions.PreserveSig)]
  HRESULT SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);

  [MethodImpl(MethodImplOptions.PreserveSig)]
  HRESULT SetThumbnailTooltip(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszTip);

  [MethodImpl(MethodImplOptions.PreserveSig)]
  HRESULT SetThumbnailClip(IntPtr hwnd, RefRECT prcClip);
}
