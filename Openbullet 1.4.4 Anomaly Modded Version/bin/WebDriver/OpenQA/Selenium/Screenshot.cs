// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Screenshot
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

#nullable disable
namespace OpenQA.Selenium;

[Serializable]
public class Screenshot
{
  private string base64Encoded = string.Empty;
  private byte[] byteArray;

  public Screenshot(string base64EncodedScreenshot)
  {
    this.base64Encoded = base64EncodedScreenshot;
    this.byteArray = Convert.FromBase64String(this.base64Encoded);
  }

  public string AsBase64EncodedString => this.base64Encoded;

  public byte[] AsByteArray => this.byteArray;

  public void SaveAsFile(string fileName) => this.SaveAsFile(fileName, ScreenshotImageFormat.Png);

  public void SaveAsFile(string fileName, ScreenshotImageFormat format)
  {
    using (MemoryStream memoryStream = new MemoryStream(this.byteArray))
    {
      using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
      {
        using (Image image = Image.FromStream((Stream) memoryStream))
          image.Save((Stream) fileStream, Screenshot.ConvertScreenshotImageFormat(format));
      }
    }
  }

  public override string ToString() => this.base64Encoded;

  private static ImageFormat ConvertScreenshotImageFormat(ScreenshotImageFormat format)
  {
    ImageFormat imageFormat = ImageFormat.Png;
    switch (format)
    {
      case ScreenshotImageFormat.Jpeg:
        imageFormat = ImageFormat.Jpeg;
        break;
      case ScreenshotImageFormat.Gif:
        imageFormat = ImageFormat.Gif;
        break;
      case ScreenshotImageFormat.Tiff:
        imageFormat = ImageFormat.Tiff;
        break;
      case ScreenshotImageFormat.Bmp:
        imageFormat = ImageFormat.Bmp;
        break;
    }
    return imageFormat;
  }
}
