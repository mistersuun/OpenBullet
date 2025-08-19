// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.IProvideClassInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("B196B283-BAB4-101A-B69C-00AA00341D07")]
[ComImport]
internal interface IProvideClassInfo
{
  void GetClassInfo(out IntPtr info);
}
