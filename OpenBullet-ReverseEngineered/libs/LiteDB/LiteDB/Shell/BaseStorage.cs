// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.BaseStorage
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

#nullable disable
namespace LiteDB.Shell;

internal class BaseStorage
{
  public bool IsFileCommand(StringScanner s, string command)
  {
    return s.Scan($"fs\\.{command}\\s*").Length > 0;
  }

  public string ReadId(StringScanner s)
  {
    return s.Scan("^[\\w-$@!+%;\\.]+(\\/[\\w-$@!+%;\\.]+)*$".Substring(1, "^[\\w-$@!+%;\\.]+(\\/[\\w-$@!+%;\\.]+)*$".Length - 2));
  }
}
