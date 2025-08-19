// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.JsonWebKeyConverter
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.Security.Cryptography;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class JsonWebKeyConverter
{
  public static JsonWebKey ConvertFromSecurityKey(SecurityKey key)
  {
    switch (key)
    {
      case null:
        throw LogHelper.LogArgumentNullException(nameof (key));
      case RsaSecurityKey key1:
        return JsonWebKeyConverter.ConvertFromRSASecurityKey(key1);
      case SymmetricSecurityKey key2:
        return JsonWebKeyConverter.ConvertFromSymmetricSecurityKey(key2);
      case X509SecurityKey key3:
        return JsonWebKeyConverter.ConvertFromX509SecurityKey(key3);
      default:
        throw LogHelper.LogExceptionMessage((Exception) new NotSupportedException(LogHelper.FormatInvariant("IDX10674: JsonWebKeyConverter does not support SecurityKey of type: {0}", (object) key.GetType().FullName)));
    }
  }

  public static JsonWebKey ConvertFromRSASecurityKey(RsaSecurityKey key)
  {
    if (key == null)
      throw LogHelper.LogArgumentNullException(nameof (key));
    RSAParameters rsaParameters;
    if (key.Rsa != null)
    {
      try
      {
        rsaParameters = key.Rsa.ExportParameters(true);
      }
      catch
      {
        rsaParameters = key.Rsa.ExportParameters(false);
      }
    }
    else
      rsaParameters = key.Parameters;
    return new JsonWebKey()
    {
      N = rsaParameters.Modulus != null ? Base64UrlEncoder.Encode(rsaParameters.Modulus) : (string) null,
      E = rsaParameters.Exponent != null ? Base64UrlEncoder.Encode(rsaParameters.Exponent) : (string) null,
      D = rsaParameters.D != null ? Base64UrlEncoder.Encode(rsaParameters.D) : (string) null,
      P = rsaParameters.P != null ? Base64UrlEncoder.Encode(rsaParameters.P) : (string) null,
      Q = rsaParameters.Q != null ? Base64UrlEncoder.Encode(rsaParameters.Q) : (string) null,
      DP = rsaParameters.DP != null ? Base64UrlEncoder.Encode(rsaParameters.DP) : (string) null,
      DQ = rsaParameters.DQ != null ? Base64UrlEncoder.Encode(rsaParameters.DQ) : (string) null,
      QI = rsaParameters.InverseQ != null ? Base64UrlEncoder.Encode(rsaParameters.InverseQ) : (string) null,
      Kty = "RSA",
      Kid = key.KeyId,
      ConvertedSecurityKey = (SecurityKey) key
    };
  }

  public static JsonWebKey ConvertFromX509SecurityKey(X509SecurityKey key)
  {
    if (key == null)
      throw LogHelper.LogArgumentNullException(nameof (key));
    JsonWebKey jsonWebKey = new JsonWebKey()
    {
      Kty = "RSA",
      Kid = key.KeyId,
      X5t = key.X5t,
      ConvertedSecurityKey = (SecurityKey) key
    };
    if (key.Certificate.RawData != null)
      jsonWebKey.X5c.Add(Convert.ToBase64String(key.Certificate.RawData));
    return jsonWebKey;
  }

  public static JsonWebKey ConvertFromSymmetricSecurityKey(SymmetricSecurityKey key)
  {
    if (key == null)
      throw LogHelper.LogArgumentNullException(nameof (key));
    return new JsonWebKey()
    {
      K = Base64UrlEncoder.Encode(key.Key),
      Kid = key.KeyId,
      Kty = "oct",
      ConvertedSecurityKey = (SecurityKey) key
    };
  }

  internal static bool TryConvertToSecurityKey(JsonWebKey webKey, out SecurityKey key)
  {
    if (webKey.ConvertedSecurityKey != null)
    {
      key = webKey.ConvertedSecurityKey;
      return true;
    }
    key = (SecurityKey) null;
    try
    {
      if ("RSA".Equals(webKey.Kty, StringComparison.Ordinal))
      {
        if (JsonWebKeyConverter.TryConvertToX509SecurityKey(webKey, out key))
          return true;
        if (JsonWebKeyConverter.TryCreateToRsaSecurityKey(webKey, out key))
          return true;
      }
      else
      {
        if ("EC".Equals(webKey.Kty, StringComparison.Ordinal))
          return JsonWebKeyConverter.TryConvertToECDsaSecurityKey(webKey, out key);
        if ("oct".Equals(webKey.Kty, StringComparison.Ordinal))
          return JsonWebKeyConverter.TryConvertToSymmetricSecurityKey(webKey, out key);
      }
    }
    catch (Exception ex)
    {
      LogHelper.LogWarning(LogHelper.FormatInvariant("IDX10813: Unable to create a {0} from the properties found in the JsonWebKey: '{1}', Exception '{2}'.", (object) typeof (SecurityKey), (object) webKey, (object) ex));
    }
    LogHelper.LogWarning(LogHelper.FormatInvariant("IDX10812: Unable to create a {0} from the properties found in the JsonWebKey: '{1}'.", (object) typeof (SecurityKey), (object) webKey));
    return false;
  }

  internal static bool TryConvertToSymmetricSecurityKey(JsonWebKey webKey, out SecurityKey key)
  {
    if (webKey.ConvertedSecurityKey is SymmetricSecurityKey)
    {
      key = webKey.ConvertedSecurityKey;
      return true;
    }
    key = (SecurityKey) null;
    if (string.IsNullOrEmpty(webKey.K))
      return false;
    try
    {
      key = (SecurityKey) new SymmetricSecurityKey(webKey);
      return true;
    }
    catch (Exception ex)
    {
      LogHelper.LogExceptionMessage((Exception) new InvalidOperationException(LogHelper.FormatInvariant("IDX10813: Unable to create a {0} from the properties found in the JsonWebKey: '{1}', Exception '{2}'.", (object) typeof (SymmetricSecurityKey), (object) webKey, (object) ex), ex));
    }
    return false;
  }

  internal static bool TryConvertToX509SecurityKey(JsonWebKey webKey, out SecurityKey key)
  {
    if (webKey.ConvertedSecurityKey is X509SecurityKey)
    {
      key = webKey.ConvertedSecurityKey;
      return true;
    }
    key = (SecurityKey) null;
    if (webKey.X5c != null)
    {
      if (webKey.X5c.Count != 0)
      {
        try
        {
          key = (SecurityKey) new X509SecurityKey(webKey);
          return true;
        }
        catch (Exception ex)
        {
          LogHelper.LogExceptionMessage((Exception) new InvalidOperationException(LogHelper.FormatInvariant("IDX10813: Unable to create a {0} from the properties found in the JsonWebKey: '{1}', Exception '{2}'.", (object) typeof (X509SecurityKey), (object) webKey, (object) ex), ex));
        }
        return false;
      }
    }
    return false;
  }

  internal static bool TryCreateToRsaSecurityKey(JsonWebKey webKey, out SecurityKey key)
  {
    if (webKey.ConvertedSecurityKey is RsaSecurityKey)
    {
      key = webKey.ConvertedSecurityKey;
      return true;
    }
    key = (SecurityKey) null;
    if (!string.IsNullOrWhiteSpace(webKey.E))
    {
      if (!string.IsNullOrWhiteSpace(webKey.N))
      {
        try
        {
          key = (SecurityKey) new RsaSecurityKey(webKey);
          return true;
        }
        catch (Exception ex)
        {
          LogHelper.LogExceptionMessage((Exception) new InvalidOperationException(LogHelper.FormatInvariant("IDX10813: Unable to create a {0} from the properties found in the JsonWebKey: '{1}', Exception '{2}'.", (object) typeof (RsaSecurityKey), (object) webKey, (object) ex), ex));
        }
        return false;
      }
    }
    return false;
  }

  internal static bool TryConvertToECDsaSecurityKey(JsonWebKey webKey, out SecurityKey key)
  {
    if (webKey.ConvertedSecurityKey is ECDsaSecurityKey)
    {
      key = webKey.ConvertedSecurityKey;
      return true;
    }
    key = (SecurityKey) null;
    if (!string.IsNullOrEmpty(webKey.Crv) && !string.IsNullOrEmpty(webKey.X))
    {
      if (!string.IsNullOrEmpty(webKey.Y))
      {
        try
        {
          key = (SecurityKey) new ECDsaSecurityKey(webKey, !string.IsNullOrEmpty(webKey.D));
          return true;
        }
        catch (Exception ex)
        {
          LogHelper.LogExceptionMessage((Exception) new InvalidOperationException(LogHelper.FormatInvariant("IDX10813: Unable to create a {0} from the properties found in the JsonWebKey: '{1}', Exception '{2}'.", (object) typeof (ECDsaSecurityKey), (object) webKey, (object) ex), ex));
        }
        return false;
      }
    }
    return false;
  }
}
