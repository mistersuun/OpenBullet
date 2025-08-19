// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.SupportedAlgorithms
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.Security.Cryptography;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

internal static class SupportedAlgorithms
{
  public static bool IsSupportedAlgorithm(string algorithm, SecurityKey key)
  {
    switch (key)
    {
      case RsaSecurityKey _:
        return SupportedAlgorithms.IsSupportedRsaAlgorithm(algorithm, key);
      case X509SecurityKey x509SecurityKey:
        return x509SecurityKey.PublicKey is RSA && SupportedAlgorithms.IsSupportedRsaAlgorithm(algorithm, key);
      case JsonWebKey jsonWebKey:
        if ("RSA".Equals(jsonWebKey.Kty, StringComparison.Ordinal))
          return SupportedAlgorithms.IsSupportedRsaAlgorithm(algorithm, key);
        if ("EC".Equals(jsonWebKey.Kty, StringComparison.Ordinal))
          return SupportedAlgorithms.IsSupportedEcdsaAlgorithm(algorithm);
        return "oct".Equals(jsonWebKey.Kty, StringComparison.Ordinal) && SupportedAlgorithms.IsSupportedSymmetricAlgorithm(algorithm);
      case ECDsaSecurityKey _:
        return SupportedAlgorithms.IsSupportedEcdsaAlgorithm(algorithm);
      case SymmetricSecurityKey _:
        return SupportedAlgorithms.IsSupportedSymmetricAlgorithm(algorithm);
      default:
        return false;
    }
  }

  internal static bool IsSupportedAuthenticatedEncryptionAlgorithm(
    string algorithm,
    SecurityKey key)
  {
    if (key == null || string.IsNullOrEmpty(algorithm) || !algorithm.Equals("A128CBC-HS256", StringComparison.Ordinal) && !algorithm.Equals("A192CBC-HS384", StringComparison.Ordinal) && !algorithm.Equals("A256CBC-HS512", StringComparison.Ordinal))
      return false;
    switch (key)
    {
      case SymmetricSecurityKey _:
        return true;
      case JsonWebKey jsonWebKey:
        return jsonWebKey.K != null && jsonWebKey.Kty == "oct";
      default:
        return false;
    }
  }

  private static bool IsSupportedEcdsaAlgorithm(string algorithm)
  {
    return algorithm == "ES256" || algorithm == "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha256" || algorithm == "ES384" || algorithm == "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha384" || algorithm == "ES512" || algorithm == "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha512";
  }

  internal static bool IsSupportedHashAlgorithm(string algorithm)
  {
    return algorithm == "SHA256" || algorithm == "http://www.w3.org/2001/04/xmlenc#sha256" || algorithm == "SHA384" || algorithm == "http://www.w3.org/2001/04/xmldsig-more#sha384" || algorithm == "SHA512" || algorithm == "http://www.w3.org/2001/04/xmlenc#sha512";
  }

  internal static bool IsSupportedKeyWrapAlgorithm(string algorithm, SecurityKey key)
  {
    if (key == null || string.IsNullOrEmpty(algorithm) || !algorithm.Equals("RSA1_5", StringComparison.Ordinal) && !algorithm.Equals("RSA-OAEP", StringComparison.Ordinal) && !algorithm.Equals("http://www.w3.org/2001/04/xmlenc#rsa-oaep", StringComparison.Ordinal))
      return false;
    switch (key)
    {
      case RsaSecurityKey _:
        return true;
      case X509SecurityKey _:
        return true;
      case JsonWebKey jsonWebKey:
        if (jsonWebKey.Kty == "RSA")
          return true;
        break;
    }
    return false;
  }

  internal static bool IsSupportedRsaAlgorithm(string algorithm, SecurityKey key)
  {
    switch (algorithm)
    {
      case "PS256":
      case "PS384":
      case "PS512":
      case "http://www.w3.org/2007/05/xmldsig-more#sha256-rsa-MGF1":
      case "http://www.w3.org/2007/05/xmldsig-more#sha384-rsa-MGF1":
      case "http://www.w3.org/2007/05/xmldsig-more#sha512-rsa-MGF1":
        return SupportedAlgorithms.IsSupportedRsaPss(key);
      case "RS256":
      case "RS384":
      case "RS512":
      case "RSA-OAEP":
      case "RSA1_5":
      case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256":
      case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384":
      case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512":
      case "http://www.w3.org/2001/04/xmlenc#rsa-oaep":
        return true;
      default:
        return false;
    }
  }

  private static bool IsSupportedRsaPss(SecurityKey key)
  {
    switch (key)
    {
      case RsaSecurityKey rsaSecurityKey when rsaSecurityKey.Rsa is RSACryptoServiceProvider:
        LogHelper.LogInformation("IDX10693: RSACryptoServiceProvider doesn't support the RSASSA-PSS signature algorithm. The list of supported algorithms is available here: https://aka.ms/IdentityModel/supported-algorithms");
        return false;
      case X509SecurityKey x509SecurityKey when x509SecurityKey.PublicKey is RSACryptoServiceProvider:
        LogHelper.LogInformation("IDX10693: RSACryptoServiceProvider doesn't support the RSASSA-PSS signature algorithm. The list of supported algorithms is available here: https://aka.ms/IdentityModel/supported-algorithms");
        return false;
      default:
        return true;
    }
  }

  internal static bool IsSupportedSymmetricAlgorithm(string algorithm)
  {
    switch (algorithm)
    {
      case "A128CBC-HS256":
      case "A128KW":
      case "A192CBC-HS384":
      case "A256CBC-HS512":
      case "A256KW":
      case "HS256":
      case "HS384":
      case "HS512":
      case "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256":
      case "http://www.w3.org/2001/04/xmldsig-more#hmac-sha384":
      case "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512":
        return true;
      default:
        return false;
    }
  }
}
