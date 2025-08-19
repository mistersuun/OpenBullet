// Decompiled with JetBrains decompiler
// Type: LiteDB.Logger
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;

#nullable disable
namespace LiteDB;

public class Logger
{
  public const byte NONE = 0;
  public const byte ERROR = 1;
  public const byte RECOVERY = 2;
  public const byte COMMAND = 4;
  public const byte LOCK = 8;
  public const byte QUERY = 16 /*0x10*/;
  public const byte JOURNAL = 32 /*0x20*/;
  public const byte CACHE = 64 /*0x40*/;
  public const byte DISK = 128 /*0x80*/;
  public const byte FULL = 255 /*0xFF*/;

  public Logger(byte level = 0, Action<string> logging = null)
  {
    this.Level = level;
    if (logging == null)
      return;
    this.Logging += logging;
  }

  public event Action<string> Logging;

  public byte Level { get; set; }

  public Logger() => this.Level = (byte) 0;

  public void Write(byte level, Func<string> fn)
  {
    if (((int) level & (int) this.Level) == 0)
      return;
    this.Write(level, fn());
  }

  public void Write(byte level, string message, params object[] args)
  {
    if (((int) level & (int) this.Level) == 0 || string.IsNullOrEmpty(message) || this.Logging == null)
      return;
    string str1 = string.Format(message, args);
    string str2;
    switch (level)
    {
      case 1:
        str2 = "ERROR";
        break;
      case 2:
        str2 = "RECOVERY";
        break;
      case 4:
        str2 = "COMMAND";
        break;
      case 8:
        str2 = "LOCK";
        break;
      case 16 /*0x10*/:
        str2 = "QUERY";
        break;
      case 32 /*0x20*/:
        str2 = "JOURNAL";
        break;
      case 64 /*0x40*/:
        str2 = "CACHE";
        break;
      case 128 /*0x80*/:
        str2 = "DISK";
        break;
      default:
        str2 = "";
        break;
    }
    string str3 = str2;
    string str4 = $"{DateTime.Now.ToString("HH:mm:ss.ffff")} [{str3}] {str1}";
    try
    {
      this.Logging(str4);
    }
    catch
    {
    }
  }
}
