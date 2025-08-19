// Decompiled with JetBrains decompiler
// Type: Standard.IObjectWithProgId
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("71e806fb-8dee-46fc-bf8c-7748a8a1ae13")]
[ComImport]
internal interface IObjectWithProgId
{
  void SetProgID([MarshalAs(UnmanagedType.LPWStr)] string pszProgID);

  [return: MarshalAs(UnmanagedType.LPWStr)]
  string GetProgID();
}
