// Decompiled with JetBrains decompiler
// Type: Tesseract.Internal.ErrorMessage
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

#nullable disable
namespace Tesseract.Internal;

internal static class ErrorMessage
{
  private const string ErrorMessageFormat = "{0}. See {1} for details.";
  private const string WikiUrlFormat = "https://github.com/charlesw/tesseract/wiki/Error-{0}";

  public static string Format(int errorNumber, string messageFormat, params object[] messageArgs)
  {
    return $"{string.Format(messageFormat, messageArgs)}. See {ErrorMessage.ErrorPageUrl(errorNumber)} for details.";
  }

  public static string ErrorPageUrl(int errorNumber)
  {
    return $"https://github.com/charlesw/tesseract/wiki/Error-{errorNumber}";
  }
}
