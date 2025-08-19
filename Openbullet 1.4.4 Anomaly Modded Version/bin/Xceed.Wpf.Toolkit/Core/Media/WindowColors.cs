// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Media.WindowColors
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using Microsoft.Win32;
using System;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Media;

public static class WindowColors
{
  private static System.Windows.Media.Color? _colorizationMode;
  private static bool? _colorizationOpaqueBlend;

  public static System.Windows.Media.Color ColorizationColor
  {
    get
    {
      if (WindowColors._colorizationMode.HasValue)
        return WindowColors._colorizationMode.Value;
      try
      {
        WindowColors._colorizationMode = new System.Windows.Media.Color?(WindowColors.GetDWMColorValue(nameof (ColorizationColor)));
      }
      catch
      {
        WindowColors._colorizationMode = new System.Windows.Media.Color?(System.Windows.Media.Color.FromArgb(byte.MaxValue, (byte) 175, (byte) 175, (byte) 175));
      }
      return WindowColors._colorizationMode.Value;
    }
  }

  public static bool ColorizationOpaqueBlend
  {
    get
    {
      if (WindowColors._colorizationOpaqueBlend.HasValue)
        return WindowColors._colorizationOpaqueBlend.Value;
      try
      {
        WindowColors._colorizationOpaqueBlend = new bool?(WindowColors.GetDWMBoolValue(nameof (ColorizationOpaqueBlend)));
      }
      catch
      {
        WindowColors._colorizationOpaqueBlend = new bool?(false);
      }
      return WindowColors._colorizationOpaqueBlend.Value;
    }
  }

  private static int GetDWMIntValue(string keyName)
  {
    return (int) Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\DWM", RegistryKeyPermissionCheck.ReadSubTree, RegistryOptions.None).GetValue(keyName);
  }

  private static System.Windows.Media.Color GetDWMColorValue(string keyName)
  {
    byte[] bytes = BitConverter.GetBytes(WindowColors.GetDWMIntValue(keyName));
    return new System.Windows.Media.Color()
    {
      B = bytes[0],
      G = bytes[1],
      R = bytes[2],
      A = byte.MaxValue
    };
  }

  private static bool GetDWMBoolValue(string keyName) => WindowColors.GetDWMIntValue(keyName) != 0;
}
