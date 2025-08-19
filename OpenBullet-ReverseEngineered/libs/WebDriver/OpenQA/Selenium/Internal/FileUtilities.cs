// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Internal.FileUtilities
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

#nullable disable
namespace OpenQA.Selenium.Internal;

internal static class FileUtilities
{
  public static bool CopyDirectory(string sourceDirectory, string destinationDirectory)
  {
    DirectoryInfo directoryInfo1 = new DirectoryInfo(sourceDirectory);
    DirectoryInfo directoryInfo2 = new DirectoryInfo(destinationDirectory);
    if (directoryInfo1.Exists)
    {
      if (!directoryInfo2.Exists)
        directoryInfo2.Create();
      foreach (FileInfo file in directoryInfo1.GetFiles())
        file.CopyTo(Path.Combine(directoryInfo2.FullName, file.Name));
      foreach (DirectoryInfo directory in directoryInfo1.GetDirectories())
        FileUtilities.CopyDirectory(directory.FullName, Path.Combine(directoryInfo2.FullName, directory.Name));
    }
    return true;
  }

  public static void DeleteDirectory(string directoryToDelete)
  {
    int num = 0;
    while (Directory.Exists(directoryToDelete))
    {
      if (num < 10)
      {
        try
        {
          Directory.Delete(directoryToDelete, true);
        }
        catch (IOException ex)
        {
          Thread.Sleep(500);
        }
        catch (UnauthorizedAccessException ex)
        {
          Thread.Sleep(500);
        }
        finally
        {
          ++num;
        }
      }
      else
        break;
    }
    if (!Directory.Exists(directoryToDelete))
      return;
    Console.WriteLine("Unable to delete directory '{0}'", (object) directoryToDelete);
  }

  public static string FindFile(string fileName)
  {
    string currentDirectory = FileUtilities.GetCurrentDirectory();
    if (File.Exists(Path.Combine(currentDirectory, fileName)))
      return currentDirectory;
    string environmentVariable = Environment.GetEnvironmentVariable("PATH");
    if (!string.IsNullOrEmpty(environmentVariable))
    {
      string str = Environment.ExpandEnvironmentVariables(environmentVariable);
      char[] chArray = new char[1]{ Path.PathSeparator };
      foreach (string path1 in str.Split(chArray))
      {
        if (path1.IndexOfAny(Path.GetInvalidPathChars()) < 0 && File.Exists(Path.Combine(path1, fileName)))
          return path1;
      }
    }
    return string.Empty;
  }

  public static string GetCurrentDirectory()
  {
    Assembly assembly = typeof (FileUtilities).Assembly;
    string str = Path.GetDirectoryName(assembly.Location);
    if (string.IsNullOrEmpty(str))
      str = Directory.GetCurrentDirectory();
    string currentDirectory = str;
    if (AppDomain.CurrentDomain.ShadowCopyFiles)
      currentDirectory = Path.GetDirectoryName(new Uri(assembly.CodeBase).LocalPath);
    return currentDirectory;
  }

  public static string GenerateRandomTempDirectoryName(string directoryPattern)
  {
    string path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, directoryPattern, (object) Guid.NewGuid().ToString("N"));
    return Path.Combine(Path.GetTempPath(), path2);
  }
}
