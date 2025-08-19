// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.Http
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

#nullable disable
namespace Leaf.xNet;

public static class Http
{
  public const string NewLine = "\r\n";
  public static readonly RemoteCertificateValidationCallback AcceptAllCertificationsCallback;
  internal static readonly Dictionary<HttpHeader, string> Headers = new Dictionary<HttpHeader, string>()
  {
    {
      HttpHeader.Accept,
      "Accept"
    },
    {
      HttpHeader.AcceptCharset,
      "Accept-Charset"
    },
    {
      HttpHeader.AcceptLanguage,
      "Accept-Language"
    },
    {
      HttpHeader.AcceptDatetime,
      "Accept-Datetime"
    },
    {
      HttpHeader.CacheControl,
      "Cache-Control"
    },
    {
      HttpHeader.ContentType,
      "Content-Type"
    },
    {
      HttpHeader.Date,
      "Date"
    },
    {
      HttpHeader.Expect,
      "Expect"
    },
    {
      HttpHeader.From,
      "From"
    },
    {
      HttpHeader.IfMatch,
      "If-Match"
    },
    {
      HttpHeader.IfModifiedSince,
      "If-Modified-Since"
    },
    {
      HttpHeader.IfNoneMatch,
      "If-None-Match"
    },
    {
      HttpHeader.IfRange,
      "If-Range"
    },
    {
      HttpHeader.IfUnmodifiedSince,
      "If-Unmodified-Since"
    },
    {
      HttpHeader.MaxForwards,
      "Max-Forwards"
    },
    {
      HttpHeader.Pragma,
      "Pragma"
    },
    {
      HttpHeader.Range,
      "Range"
    },
    {
      HttpHeader.Referer,
      "Referer"
    },
    {
      HttpHeader.Origin,
      "Origin"
    },
    {
      HttpHeader.Upgrade,
      "Upgrade"
    },
    {
      HttpHeader.UpgradeInsecureRequests,
      "Upgrade-Insecure-Requests"
    },
    {
      HttpHeader.UserAgent,
      "User-Agent"
    },
    {
      HttpHeader.Via,
      "Via"
    },
    {
      HttpHeader.Warning,
      "Warning"
    },
    {
      HttpHeader.DNT,
      "DNT"
    },
    {
      HttpHeader.AccessControlAllowOrigin,
      "Access-Control-Allow-Origin"
    },
    {
      HttpHeader.AcceptRanges,
      "Accept-Ranges"
    },
    {
      HttpHeader.Age,
      "Age"
    },
    {
      HttpHeader.Allow,
      "Allow"
    },
    {
      HttpHeader.ContentEncoding,
      "Content-Encoding"
    },
    {
      HttpHeader.ContentLanguage,
      "Content-Language"
    },
    {
      HttpHeader.ContentLength,
      "Content-Length"
    },
    {
      HttpHeader.ContentLocation,
      "Content-Location"
    },
    {
      HttpHeader.ContentMD5,
      "Content-MD5"
    },
    {
      HttpHeader.ContentDisposition,
      "Content-Disposition"
    },
    {
      HttpHeader.ContentRange,
      "Content-Range"
    },
    {
      HttpHeader.ETag,
      "ETag"
    },
    {
      HttpHeader.Expires,
      "Expires"
    },
    {
      HttpHeader.LastModified,
      "Last-Modified"
    },
    {
      HttpHeader.Link,
      "Link"
    },
    {
      HttpHeader.Location,
      "Location"
    },
    {
      HttpHeader.P3P,
      "P3P"
    },
    {
      HttpHeader.Refresh,
      "Refresh"
    },
    {
      HttpHeader.RetryAfter,
      "Retry-After"
    },
    {
      HttpHeader.Server,
      "Server"
    },
    {
      HttpHeader.TransferEncoding,
      "Transfer-Encoding"
    }
  };
  private static readonly byte[] FirefoxVersions = new byte[9]
  {
    (byte) 64 /*0x40*/,
    (byte) 63 /*0x3F*/,
    (byte) 62,
    (byte) 60,
    (byte) 58,
    (byte) 52,
    (byte) 51,
    (byte) 46,
    (byte) 45
  };

  static Http()
  {
    Http.AcceptAllCertificationsCallback = new RemoteCertificateValidationCallback(Http.AcceptAllCertifications);
  }

  public static string ToQueryString(
    IEnumerable<KeyValuePair<string, string>> parameters,
    bool valuesUnescaped = false,
    bool keysUnescaped = false)
  {
    if (parameters == null)
      throw new ArgumentNullException(nameof (parameters));
    StringBuilder stringBuilder = new StringBuilder();
    foreach (KeyValuePair<string, string> parameter in parameters)
    {
      if (!string.IsNullOrEmpty(parameter.Key))
      {
        stringBuilder.Append(keysUnescaped ? parameter.Key : Uri.EscapeDataString(parameter.Key));
        stringBuilder.Append('=');
        stringBuilder.Append(valuesUnescaped ? parameter.Value : Uri.EscapeDataString(parameter.Value ?? string.Empty));
        stringBuilder.Append('&');
      }
    }
    if (stringBuilder.Length != 0)
      stringBuilder.Remove(stringBuilder.Length - 1, 1);
    return stringBuilder.ToString();
  }

  public static string DetermineMediaType(string extension)
  {
    string mediaType = "application/octet-stream";
    try
    {
      using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(extension))
      {
        object obj = registryKey?.GetValue("Content Type");
        if (obj != null)
          mediaType = obj.ToString();
      }
    }
    catch (IOException ex)
    {
    }
    catch (ObjectDisposedException ex)
    {
    }
    catch (UnauthorizedAccessException ex)
    {
    }
    catch (SecurityException ex)
    {
    }
    return mediaType;
  }

  public static string IEUserAgent()
  {
    string str1 = Http.RandomWindowsVersion();
    string str2;
    string str3;
    string str4;
    string str5;
    if (str1.Contains("NT 5.1"))
    {
      str2 = "9.0";
      str3 = "5.0";
      str4 = "5.0";
      str5 = ".NET CLR 2.0.50727; .NET CLR 3.5.30729";
    }
    else if (str1.Contains("NT 6.0"))
    {
      str2 = "9.0";
      str3 = "5.0";
      str4 = "5.0";
      str5 = ".NET CLR 2.0.50727; Media Center PC 5.0; .NET CLR 3.5.30729";
    }
    else
    {
      switch (Randomizer.Instance.Next(3))
      {
        case 0:
          str2 = "10.0";
          str4 = "6.0";
          str3 = "5.0";
          break;
        case 1:
          str2 = "10.6";
          str4 = "6.0";
          str3 = "5.0";
          break;
        default:
          str2 = "11.0";
          str4 = "7.0";
          str3 = "5.0";
          break;
      }
      str5 = ".NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E";
    }
    return $"Mozilla/{str3} (compatible; MSIE {str2}; {str1}; Trident/{str4}; {str5})";
  }

  public static string OperaUserAgent()
  {
    string str1;
    string str2;
    switch (Randomizer.Instance.Next(4))
    {
      case 0:
        str1 = "12.16";
        str2 = "2.12.388";
        break;
      case 1:
        str1 = "12.14";
        str2 = "2.12.388";
        break;
      case 2:
        str1 = "12.02";
        str2 = "2.10.289";
        break;
      default:
        str1 = "12.00";
        str2 = "2.10.181";
        break;
    }
    return $"Opera/9.80 ({Http.RandomWindowsVersion()}); U) Presto/{str2} Version/{str1}";
  }

  public static string ChromeUserAgent()
  {
    int num1 = Randomizer.Instance.Next(62, 70);
    int num2 = Randomizer.Instance.Next(2100, 3538);
    int num3 = Randomizer.Instance.Next(170);
    return $"Mozilla/5.0 ({Http.RandomWindowsVersion()}) AppleWebKit/537.36 (KHTML, like Gecko) {$"Chrome/{num1}.0.{num2}.{num3} Safari/537.36"}";
  }

  public static string FirefoxUserAgent()
  {
    byte firefoxVersion = Http.FirefoxVersions[Randomizer.Instance.Next(Http.FirefoxVersions.Length - 1)];
    return $"Mozilla/5.0 ({Http.RandomWindowsVersion()}; rv:{firefoxVersion}.0) Gecko/20100101 Firefox/{firefoxVersion}.0";
  }

  public static string OperaMiniUserAgent()
  {
    string str1;
    string str2;
    string str3;
    string str4;
    switch (Randomizer.Instance.Next(3))
    {
      case 0:
        str1 = "iOS";
        str2 = "7.0.73345";
        str3 = "11.62";
        str4 = "2.10.229";
        break;
      case 1:
        str1 = "J2ME/MIDP";
        str2 = "7.1.23511";
        str3 = "12.00";
        str4 = "2.10.181";
        break;
      default:
        str1 = "Android";
        str2 = "7.5.54678";
        str3 = "12.02";
        str4 = "2.10.289";
        break;
    }
    return $"Opera/9.80 ({str1}; Opera Mini/{str2}/28.2555; U; ru) Presto/{str4} Version/{str3}";
  }

  public static string RandomUserAgent()
  {
    int num = Randomizer.Instance.Next(99) + 1;
    if (num >= 1 && num <= 70)
      return Http.ChromeUserAgent();
    if (num > 70 && num <= 85)
      return Http.FirefoxUserAgent();
    if (num > 85 && num <= 91)
      return Http.IEUserAgent();
    return num > 91 && num <= 96 /*0x60*/ ? Http.OperaUserAgent() : Http.OperaMiniUserAgent();
  }

  private static bool AcceptAllCertifications(
    object sender,
    X509Certificate certification,
    X509Chain chain,
    SslPolicyErrors sslPolicyErrors)
  {
    return true;
  }

  private static string RandomWindowsVersion()
  {
    string str1 = "Windows NT ";
    int num = Randomizer.Instance.Next(99) + 1;
    string str2 = num < 1 || num > 45 ? (num <= 45 || num > 80 /*0x50*/ ? (num <= 80 /*0x50*/ || num > 95 ? str1 + "6.2" : str1 + "6.3") : str1 + "6.1") : str1 + "10.0";
    if (Randomizer.Instance.NextDouble() <= 0.65)
      str2 += Randomizer.Instance.NextDouble() <= 0.5 ? "; WOW64" : "; Win64; x64";
    return str2;
  }
}
