// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.NativeMethods
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Scripting.Utils;

internal static class NativeMethods
{
  [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static extern bool SetEnvironmentVariable(string name, string value);
}
