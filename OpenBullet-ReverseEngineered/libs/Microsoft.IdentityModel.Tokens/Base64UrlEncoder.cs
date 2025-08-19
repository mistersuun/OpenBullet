// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.Base64UrlEncoder
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.Text;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public static class Base64UrlEncoder
{
  private static char base64PadCharacter = '=';
  private static string doubleBase64PadCharacter = "==";
  private static char base64Character62 = '+';
  private static char base64Character63 = '/';
  private static char base64UrlCharacter62 = '-';
  private static char _base64UrlCharacter63 = '_';

  public static string Encode(string arg)
  {
    return arg != null ? Base64UrlEncoder.Encode(Encoding.UTF8.GetBytes(arg)) : throw LogHelper.LogArgumentNullException(nameof (arg));
  }

  public static string Encode(byte[] inArray, int offset, int length)
  {
    if (inArray == null)
      throw LogHelper.LogArgumentNullException(nameof (inArray));
    return Convert.ToBase64String(inArray, offset, length).Split(Base64UrlEncoder.base64PadCharacter)[0].Replace(Base64UrlEncoder.base64Character62, Base64UrlEncoder.base64UrlCharacter62).Replace(Base64UrlEncoder.base64Character63, Base64UrlEncoder._base64UrlCharacter63);
  }

  public static string Encode(byte[] inArray)
  {
    if (inArray == null)
      throw LogHelper.LogArgumentNullException(nameof (inArray));
    return Convert.ToBase64String(inArray, 0, inArray.Length).Split(Base64UrlEncoder.base64PadCharacter)[0].Replace(Base64UrlEncoder.base64Character62, Base64UrlEncoder.base64UrlCharacter62).Replace(Base64UrlEncoder.base64Character63, Base64UrlEncoder._base64UrlCharacter63);
  }

  public static byte[] DecodeBytes(string str)
  {
    if (str == null)
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentNullException(nameof (str)));
    str = str.Replace(Base64UrlEncoder.base64UrlCharacter62, Base64UrlEncoder.base64Character62);
    str = str.Replace(Base64UrlEncoder._base64UrlCharacter63, Base64UrlEncoder.base64Character63);
    switch (str.Length % 4)
    {
      case 0:
        return Convert.FromBase64String(str);
      case 2:
        str += Base64UrlEncoder.doubleBase64PadCharacter;
        goto case 0;
      case 3:
        str += Base64UrlEncoder.base64PadCharacter.ToString();
        goto case 0;
      default:
        throw LogHelper.LogExceptionMessage((Exception) new FormatException(LogHelper.FormatInvariant("IDX10400: Unable to decode: '{0}' as Base64url encoded string.", (object) str)));
    }
  }

  public static string Decode(string arg)
  {
    return Encoding.UTF8.GetString(Base64UrlEncoder.DecodeBytes(arg));
  }
}
