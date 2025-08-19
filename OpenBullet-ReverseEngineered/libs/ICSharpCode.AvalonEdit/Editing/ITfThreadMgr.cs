// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.ITfThreadMgr
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

[Guid("aa80e801-2021-11d2-93e0-0060b067b86e")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[ComImport]
internal interface ITfThreadMgr
{
  void Activate(out int clientId);

  void Deactivate();

  void CreateDocumentMgr(out IntPtr docMgr);

  void EnumDocumentMgrs(out IntPtr enumDocMgrs);

  void GetFocus(out IntPtr docMgr);

  void SetFocus(IntPtr docMgr);

  void AssociateFocus(IntPtr hwnd, IntPtr newDocMgr, out IntPtr prevDocMgr);

  void IsThreadFocus([MarshalAs(UnmanagedType.Bool)] out bool isFocus);

  void GetFunctionProvider(ref Guid classId, out IntPtr funcProvider);

  void EnumFunctionProviders(out IntPtr enumProviders);

  void GetGlobalCompartment(out IntPtr compartmentMgr);
}
