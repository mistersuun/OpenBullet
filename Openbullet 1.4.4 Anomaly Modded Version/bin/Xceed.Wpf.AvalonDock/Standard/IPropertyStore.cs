// Decompiled with JetBrains decompiler
// Type: Standard.IPropertyStore
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99")]
[ComImport]
internal interface IPropertyStore
{
  uint GetCount();

  PKEY GetAt(uint iProp);

  void GetValue([In] ref PKEY pkey, [In, Out] PROPVARIANT pv);

  void SetValue([In] ref PKEY pkey, PROPVARIANT pv);

  void Commit();
}
