// Decompiled with JetBrains decompiler
// Type: Standard.IApplicationDestinations
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("12337d35-94c6-48a0-bce7-6a9c69d4d600")]
[ComImport]
internal interface IApplicationDestinations
{
  void SetAppID([MarshalAs(UnmanagedType.LPWStr), In] string pszAppID);

  void RemoveDestination([MarshalAs(UnmanagedType.IUnknown)] object punk);

  void RemoveAllDestinations();
}
