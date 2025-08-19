// Decompiled with JetBrains decompiler
// Type: LiteDB.FileHelper
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.IO;

#nullable disable
namespace LiteDB;

internal static class FileHelper
{
  public static string GetTempFile(string filename, string suffix = "-temp", bool checkIfExists = true)
  {
    int num = 0;
    string path;
    string directoryName;
    string path2;
    for (path = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + suffix + Path.GetExtension(filename)); checkIfExists && File.Exists(path); path = Path.Combine(directoryName, path2))
    {
      directoryName = Path.GetDirectoryName(filename);
      path2 = $"{Path.GetFileNameWithoutExtension(filename)}{suffix}-{(object) ++num}{Path.GetExtension(filename)}";
    }
    return path;
  }

  public static bool TryDelete(string filename)
  {
    try
    {
      File.Delete(filename);
      return true;
    }
    catch (IOException ex)
    {
      return false;
    }
  }

  public static void TryExec(Action action, TimeSpan timeout)
  {
    DateTime dateTime = DateTime.UtcNow.Add(timeout);
    do
    {
      try
      {
        action();
        return;
      }
      catch (IOException ex)
      {
        ex.WaitIfLocked(25);
      }
    }
    while (DateTime.UtcNow < dateTime);
    throw LiteException.LockTimeout(timeout);
  }
}
