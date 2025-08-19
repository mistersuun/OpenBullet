// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonWinsoundModule
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

#nullable disable
namespace IronPython.Modules;

public static class PythonWinsoundModule
{
  public static readonly string __doc__ = "PlaySound(sound, flags) - play a sound\r\nSND_FILENAME - sound is a wav file name\r\nSND_ALIAS - sound is a registry sound association name\r\nSND_LOOP - Play the sound repeatedly; must also specify SND_ASYNC\r\nSND_MEMORY - sound is a memory image of a wav file\r\nSND_PURGE - stop all instances of the specified sound\r\nSND_ASYNC - PlaySound returns immediately\r\nSND_NODEFAULT - Do not play a default beep if the sound can not be found\r\nSND_NOSTOP - Do not interrupt any sounds currently playing\r\nSND_NOWAIT - Return immediately if the sound driver is busy\r\n\r\nBeep(frequency, duration) - Make a beep through the PC speaker.";
  public const int SND_SYNC = 0;
  public const int SND_ASYNC = 1;
  public const int SND_NODEFAULT = 2;
  public const int SND_MEMORY = 4;
  public const int SND_LOOP = 8;
  public const int SND_NOSTOP = 16 /*0x10*/;
  public const int SND_NOWAIT = 8192 /*0x2000*/;
  public const int SND_ALIAS = 65536 /*0x010000*/;
  public const int SND_ALIAS_ID = 1114112 /*0x110000*/;
  public const int SND_FILENAME = 131072 /*0x020000*/;
  public const int SND_RESOURCE = 262148 /*0x040004*/;
  public const int SND_PURGE = 64 /*0x40*/;
  public const int SND_APPLICATION = 128 /*0x80*/;
  public const int MB_OK = 0;
  public const int MB_ICONASTERISK = 64 /*0x40*/;
  public const int MB_ICONEXCLAMATION = 48 /*0x30*/;
  public const int MB_ICONHAND = 16 /*0x10*/;
  public const int MB_ICONQUESTION = 32 /*0x20*/;

  [DllImport("winmm.dll")]
  private static extern bool PlaySound(string fileName, IntPtr hMod, int flags);

  [DllImport("winmm.dll")]
  private static extern bool PlaySound(byte[] bytes, IntPtr hMod, int flags);

  [DllImport("winmm.dll")]
  private static extern bool PlaySound(IntPtr input, IntPtr hMod, int flags);

  [DllImport("kernel32.dll")]
  private static extern bool Beep(int dwFreq, int dwDuration);

  [DllImport("user32.dll")]
  private static extern bool MessageBeep(int uType);

  [Documentation("PlaySound(sound, flags) - a wrapper around the Windows PlaySound API\r\n\r\nThe sound argument can be a filename, data, or None.\r\nFor flag values, ored together, see module documentation.")]
  public static void PlaySound(CodeContext context, [NotNull] string sound, int flags)
  {
    if ((flags & 1) == 1 && (flags & 4) == 4)
      throw PythonOps.RuntimeError("Cannot play asynchronously from memory");
    if (!PythonWinsoundModule.PlaySound(sound, IntPtr.Zero, flags))
      throw PythonOps.RuntimeError("Failed to play sound");
  }

  [Documentation("PlaySound(sound, flags) - a wrapper around the Windows PlaySound API\r\n\r\nThe sound argument can be a filename, data, or None.\r\nFor flag values, ored together, see module documentation.")]
  public static void PlaySound(CodeContext context, [NotNull] IList<byte> sound, int flags)
  {
    if ((flags & 1) == 1 && (flags & 4) == 4)
      throw PythonOps.RuntimeError("Cannot play asynchronously from memory");
    if (!PythonWinsoundModule.PlaySound(sound.ToArray<byte>(), IntPtr.Zero, flags))
      throw PythonOps.RuntimeError("Failed to play sound");
  }

  [Documentation("PlaySound(sound, flags) - a wrapper around the Windows PlaySound API\r\n\r\nThe sound argument can be a filename, data, or None.\r\nFor flag values, ored together, see module documentation.")]
  public static void PlaySound(CodeContext context, object sound, int flags)
  {
    if ((flags & 1) == 1 && (flags & 4) == 4)
      throw PythonOps.RuntimeError("Cannot play asynchronously from memory");
    bool flag;
    switch (sound)
    {
      case null:
        flag = PythonWinsoundModule.PlaySound(IntPtr.Zero, IntPtr.Zero, flags);
        break;
      case string _:
        flag = PythonWinsoundModule.PlaySound((string) sound, IntPtr.Zero, flags);
        break;
      case IList<byte> _:
        flag = PythonWinsoundModule.PlaySound(((IEnumerable<byte>) sound).ToArray<byte>(), IntPtr.Zero, flags);
        break;
      default:
        throw PythonOps.RuntimeError("Failed to play sound");
    }
    if (!flag)
      throw PythonOps.RuntimeError("Failed to play sound");
  }

  [Documentation("Beep(frequency, duration) - a wrapper around the Windows Beep API\r\n\r\nThe frequency argument specifies frequency, in hertz, of the sound.\r\nThis parameter must be in the range 37 through 32,767.\r\nThe duration argument specifies the number of milliseconds.\r\n")]
  public static void Beep(CodeContext context, int freq, int dur)
  {
    if (freq < 37 || freq > (int) short.MaxValue)
      throw PythonOps.ValueError("frequency must be in 37 thru 32767");
    if (!PythonWinsoundModule.Beep(freq, dur))
      throw PythonOps.RuntimeError("Failed to beep");
  }

  [Documentation("MessageBeep(x) - call Windows MessageBeep(x). x defaults to MB_OK.")]
  public static void MessageBeep(CodeContext context, int x = 0)
  {
    PythonWinsoundModule.MessageBeep(x);
  }
}
