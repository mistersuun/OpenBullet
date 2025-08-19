// Decompiled with JetBrains decompiler
// Type: Standard.ICustomDestinationList
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("6332debf-87b5-4670-90c0-5e57b408a49e")]
[ComImport]
internal interface ICustomDestinationList
{
  void SetAppID([MarshalAs(UnmanagedType.LPWStr), In] string pszAppID);

  [return: MarshalAs(UnmanagedType.Interface)]
  object BeginList(out uint pcMaxSlots, [In] ref Guid riid);

  [MethodImpl(MethodImplOptions.PreserveSig)]
  HRESULT AppendCategory([MarshalAs(UnmanagedType.LPWStr)] string pszCategory, IObjectArray poa);

  void AppendKnownCategory(KDC category);

  [MethodImpl(MethodImplOptions.PreserveSig)]
  HRESULT AddUserTasks(IObjectArray poa);

  void CommitList();

  [return: MarshalAs(UnmanagedType.Interface)]
  object GetRemovedDestinations([In] ref Guid riid);

  void DeleteList([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);

  void AbortList();
}
