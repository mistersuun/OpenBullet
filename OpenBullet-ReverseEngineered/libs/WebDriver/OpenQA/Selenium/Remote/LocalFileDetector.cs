// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.LocalFileDetector
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.IO;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class LocalFileDetector : IFileDetector
{
  public bool IsFile(string keySequence) => File.Exists(keySequence);
}
