// Decompiled with JetBrains decompiler
// Type: Standard.IObjectWithAppUserModelId
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("36db0196-9665-46d1-9ba7-d3709eecf9ed")]
[ComImport]
internal interface IObjectWithAppUserModelId
{
  void SetAppID([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);

  [return: MarshalAs(UnmanagedType.LPWStr)]
  string GetAppID();
}
