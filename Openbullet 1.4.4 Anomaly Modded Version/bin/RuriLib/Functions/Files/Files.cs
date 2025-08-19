// Decompiled with JetBrains decompiler
// Type: RuriLib.Functions.Files.Files
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using OpenQA.Selenium;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

#nullable disable
namespace RuriLib.Functions.Files;

public static class Files
{
  public static void SaveScreenshot(Screenshot screenshot, BotData data)
  {
    string fileName = RuriLib.Functions.Files.Files.MakeScreenshotPath(data);
    data.Screenshots.Add(fileName);
    screenshot.SaveAsFile(fileName);
  }

  public static void SaveScreenshot(Bitmap screenshot, BotData data)
  {
    string filename = RuriLib.Functions.Files.Files.MakeScreenshotPath(data);
    data.Screenshots.Add(filename);
    screenshot.Save(filename);
  }

  public static void SaveScreenshot(MemoryStream stream, BotData data)
  {
    string path = RuriLib.Functions.Files.Files.MakeScreenshotPath(data);
    using (FileStream destination = File.Create(path))
      stream.CopyTo((Stream) destination);
    data.Screenshots.Add(path);
  }

  private static string MakeScreenshotPath(BotData data)
  {
    string str = RuriLib.Functions.Files.Files.MakeValidFileName(data.ConfigSettings.Name);
    string fileName = RuriLib.Functions.Files.Files.MakeValidFileName(data.Data.Data);
    if (!Directory.Exists("Screenshots\\" + str))
      Directory.CreateDirectory("Screenshots\\" + str);
    string availableFileName = RuriLib.Functions.Files.Files.GetFirstAvailableFileName($"Screenshots\\{str}\\", fileName, "bmp");
    return $"Screenshots\\{str}\\{availableFileName}";
  }

  public static string GetFirstAvailableFileName(
    string basePath,
    string fileName,
    string extension)
  {
    int num = 1;
    while (true)
    {
      if (File.Exists($"{basePath}{fileName}{(object) num}.{extension}"))
        ++num;
      else
        break;
    }
    return $"{fileName}{(object) num}.{extension}";
  }

  public static string MakeValidFileName(string name, bool underscore = true)
  {
    string pattern = string.Format("([{0}]*\\.+$)|([{0}]+)", (object) Regex.Escape(new string(Path.GetInvalidFileNameChars())));
    return Regex.Replace(name, pattern, underscore ? "_" : "");
  }
}
