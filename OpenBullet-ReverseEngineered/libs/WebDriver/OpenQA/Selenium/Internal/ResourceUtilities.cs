// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Internal.ResourceUtilities
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

#nullable disable
namespace OpenQA.Selenium.Internal;

public static class ResourceUtilities
{
  private static string assemblyVersion;
  private static string platformFamily;

  public static string AssemblyVersion
  {
    get
    {
      if (string.IsNullOrEmpty(ResourceUtilities.assemblyVersion))
      {
        Version version = Assembly.GetCallingAssembly().GetName().Version;
        ResourceUtilities.assemblyVersion = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2}", (object) version.Major, (object) version.Minor, (object) version.Revision);
      }
      return ResourceUtilities.assemblyVersion;
    }
  }

  public static string PlatformFamily
  {
    get
    {
      if (string.IsNullOrEmpty(ResourceUtilities.platformFamily))
        ResourceUtilities.platformFamily = ResourceUtilities.GetPlatformString();
      return ResourceUtilities.platformFamily;
    }
  }

  public static Stream GetResourceStream(string fileName, string resourceId)
  {
    string path = Path.Combine(FileUtilities.GetCurrentDirectory(), Path.GetFileName(fileName));
    Stream stream;
    if (File.Exists(path))
      stream = (Stream) new FileStream(path, FileMode.Open, FileAccess.Read);
    else if (File.Exists(fileName))
      stream = (Stream) new FileStream(fileName, FileMode.Open, FileAccess.Read);
    else
      stream = !string.IsNullOrEmpty(resourceId) ? Assembly.GetCallingAssembly().GetManifestResourceStream(resourceId) : throw new WebDriverException("The file specified does not exist, and you have specified no internal resource ID");
    return stream != null ? stream : throw new WebDriverException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Cannot find a file named '{0}' or an embedded resource with the id '{1}'.", (object) path, (object) resourceId));
  }

  public static bool IsValidResourceName(string resourceId)
  {
    return new List<string>((IEnumerable<string>) Assembly.GetCallingAssembly().GetManifestResourceNames()).Contains(resourceId);
  }

  private static string GetPlatformString()
  {
    string platformString = "unknown";
    switch (Environment.OSVersion.Platform)
    {
      case PlatformID.Win32S:
      case PlatformID.Win32Windows:
      case PlatformID.Win32NT:
      case PlatformID.WinCE:
        platformString = "windows";
        break;
      case PlatformID.Unix:
      case PlatformID.MacOSX:
      case (PlatformID) 128 /*0x80*/:
        using (Process process = new Process())
        {
          process.StartInfo.FileName = "uname";
          process.StartInfo.UseShellExecute = false;
          process.StartInfo.RedirectStandardOutput = true;
          process.Start();
          process.WaitForExit(1000);
          platformString = !process.StandardOutput.ReadToEnd().ToLowerInvariant().StartsWith("darwin") ? "linux" : "mac";
          break;
        }
    }
    return platformString;
  }
}
