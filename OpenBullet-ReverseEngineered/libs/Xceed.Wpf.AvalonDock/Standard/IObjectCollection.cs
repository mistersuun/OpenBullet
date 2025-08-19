// Decompiled with JetBrains decompiler
// Type: Standard.IObjectCollection
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("92CA9DCD-5622-4bba-A805-5E9F541BD8C9")]
[ComImport]
internal interface IObjectCollection : IObjectArray
{
  new uint GetCount();

  [return: MarshalAs(UnmanagedType.IUnknown)]
  new object GetAt([In] uint uiIndex, [In] ref Guid riid);

  void AddObject([MarshalAs(UnmanagedType.IUnknown)] object punk);

  void AddFromArray(IObjectArray poaSource);

  void RemoveObjectAt(uint uiIndex);

  void Clear();
}
