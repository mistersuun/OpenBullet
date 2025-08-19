// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.CTypesTest
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using System;
using System.IO;
using System.Reflection;

#nullable disable
namespace IronPython.Modules;

public static class CTypesTest
{
  public static string __file__ = Path.Combine(CTypesTest.FindRoot(), "Tests", $"_ctypes_test_{(Environment.OSVersion.Platform == PlatformID.Win32NT ? (object) "win" : (Environment.OSVersion.Platform == PlatformID.MacOSX ? (object) "macOS" : (object) "linux"))}{(Environment.Is64BitProcess ? 64 /*0x40*/ : 32 /*0x20*/)}.pyd");

  private static string FindRoot()
  {
    string root = Assembly.GetExecutingAssembly().Location;
    for (bool flag = false; !flag && !string.IsNullOrEmpty(root); root = Path.GetDirectoryName(root))
    {
      if (Directory.Exists(Path.Combine(root, "Src", "StdLib", "Lib")))
        return root;
    }
    return string.Empty;
  }
}
