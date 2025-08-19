// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.Validators
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public static class Validators
{
  public static void ValidateAudience(
    IEnumerable<string> audiences,
    SecurityToken securityToken,
    TokenValidationParameters validationParameters)
  {
    if (validationParameters == null)
      throw LogHelper.LogArgumentNullException(nameof (validationParameters));
    if (!validationParameters.ValidateAudience)
      LogHelper.LogWarning("IDX10233: ValidateAudience property on ValidationParameters is set to false. Exiting without validating the audience.");
    else if (validationParameters.AudienceValidator != null)
    {
      if (!validationParameters.AudienceValidator(audiences, securityToken, validationParameters))
        throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenInvalidAudienceException(LogHelper.FormatInvariant("IDX10231: Audience validation failed. Delegate returned false, securitytoken: '{0}'.", (object) securityToken))
        {
          InvalidAudience = Utility.SerializeAsSingleCommaDelimitedString(audiences)
        });
    }
    else
    {
      if (audiences == null)
        throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenInvalidAudienceException("IDX10207: Unable to validate audience. The 'audiences' parameter is null.")
        {
          InvalidAudience = (string) null
        });
      if (string.IsNullOrWhiteSpace(validationParameters.ValidAudience) && validationParameters.ValidAudiences == null)
        throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenInvalidAudienceException("IDX10208: Unable to validate audience. validationParameters.ValidAudience is null or whitespace and validationParameters.ValidAudiences is null.")
        {
          InvalidAudience = Utility.SerializeAsSingleCommaDelimitedString(audiences)
        });
      foreach (string audience in audiences)
      {
        if (!string.IsNullOrWhiteSpace(audience))
        {
          if (validationParameters.ValidAudiences != null)
          {
            foreach (string validAudience in validationParameters.ValidAudiences)
            {
              if (string.Equals(audience, validAudience, StringComparison.Ordinal))
              {
                LogHelper.LogInformation("IDX10234: Audience Validated.Audience: '{0}'", (object) audience);
                return;
              }
            }
          }
          if (!string.IsNullOrWhiteSpace(validationParameters.ValidAudience) && string.Equals(audience, validationParameters.ValidAudience, StringComparison.Ordinal))
          {
            LogHelper.LogInformation("IDX10234: Audience Validated.Audience: '{0}'", (object) audience);
            return;
          }
        }
      }
      throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenInvalidAudienceException(LogHelper.FormatInvariant("IDX10214: Audience validation failed. Audiences: '{0}'. Did not match: validationParameters.ValidAudience: '{1}' or validationParameters.ValidAudiences: '{2}'.", (object) Utility.SerializeAsSingleCommaDelimitedString(audiences), (object) (validationParameters.ValidAudience ?? "null"), (object) Utility.SerializeAsSingleCommaDelimitedString(validationParameters.ValidAudiences)))
      {
        InvalidAudience = Utility.SerializeAsSingleCommaDelimitedString(audiences)
      });
    }
  }

  public static string ValidateIssuer(
    string issuer,
    SecurityToken securityToken,
    TokenValidationParameters validationParameters)
  {
    if (validationParameters == null)
      throw LogHelper.LogArgumentNullException(nameof (validationParameters));
    if (!validationParameters.ValidateIssuer)
    {
      LogHelper.LogInformation("IDX10235: ValidateIssuer property on ValidationParameters is set to false. Exiting without validating the issuer.");
      return issuer;
    }
    if (validationParameters.IssuerValidator != null)
      return validationParameters.IssuerValidator(issuer, securityToken, validationParameters);
    if (string.IsNullOrWhiteSpace(issuer))
      throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenInvalidIssuerException("IDX10211: Unable to validate issuer. The 'issuer' parameter is null or whitespace")
      {
        InvalidIssuer = issuer
      });
    if (string.IsNullOrWhiteSpace(validationParameters.ValidIssuer) && validationParameters.ValidIssuers == null)
      throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenInvalidIssuerException("IDX10204: Unable to validate issuer. validationParameters.ValidIssuer is null or whitespace AND validationParameters.ValidIssuers is null.")
      {
        InvalidIssuer = issuer
      });
    if (string.Equals(validationParameters.ValidIssuer, issuer, StringComparison.Ordinal))
    {
      LogHelper.LogInformation("IDX10236: Issuer Validated.Issuer: '{0}'", (object) issuer);
      return issuer;
    }
    if (validationParameters.ValidIssuers != null)
    {
      foreach (string validIssuer in validationParameters.ValidIssuers)
      {
        if (string.IsNullOrEmpty(validIssuer))
          LogHelper.LogInformation("IDX10235: ValidateIssuer property on ValidationParameters is set to false. Exiting without validating the issuer.");
        else if (string.Equals(validIssuer, issuer, StringComparison.Ordinal))
        {
          LogHelper.LogInformation("IDX10236: Issuer Validated.Issuer: '{0}'", (object) issuer);
          return issuer;
        }
      }
    }
    throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenInvalidIssuerException(LogHelper.FormatInvariant("IDX10205: Issuer validation failed. Issuer: '{0}'. Did not match: validationParameters.ValidIssuer: '{1}' or validationParameters.ValidIssuers: '{2}'.", (object) issuer, (object) (validationParameters.ValidIssuer ?? "null"), (object) Utility.SerializeAsSingleCommaDelimitedString(validationParameters.ValidIssuers)))
    {
      InvalidIssuer = issuer
    });
  }

  public static void ValidateIssuerSecurityKey(
    SecurityKey securityKey,
    SecurityToken securityToken,
    TokenValidationParameters validationParameters)
  {
    if (validationParameters == null)
      throw LogHelper.LogArgumentNullException(nameof (validationParameters));
    if (!validationParameters.ValidateIssuerSigningKey)
    {
      LogHelper.LogInformation("IDX10237: ValidateIssuerSigningKey property on ValidationParameters is set to false. Exiting without validating the issuer signing key.");
    }
    else
    {
      if (validationParameters.IssuerSigningKeyValidator != null && !validationParameters.IssuerSigningKeyValidator(securityKey, securityToken, validationParameters))
        throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenInvalidSigningKeyException(LogHelper.FormatInvariant("IDX10232: IssuerSigningKey validation failed. Delegate returned false, securityKey: '{0}'.", (object) securityKey))
        {
          SigningKey = securityKey
        });
      if (!validationParameters.RequireSignedTokens && securityKey == null)
      {
        LogHelper.LogInformation("IDX10252: RequireSignedTokens property on ValidationParameters is set to false and the issuer signing key is null. Exiting without validating the issuer signing key.");
      }
      else
      {
        if (securityKey == null)
          throw LogHelper.LogExceptionMessage((Exception) new ArgumentNullException(nameof (securityKey), "IDX10253: RequireSignedTokens property on ValidationParameters is set to true, but the issuer signing key is null."));
        if (securityToken == null)
          throw LogHelper.LogArgumentNullException(nameof (securityToken));
        X509Certificate2 certificate;
        if ((certificate = securityKey is X509SecurityKey x509SecurityKey ? x509SecurityKey.Certificate : (X509Certificate2) null) == null)
          return;
        DateTime utcNow = DateTime.UtcNow;
        DateTime universalTime1 = certificate.NotBefore.ToUniversalTime();
        DateTime universalTime2 = certificate.NotAfter.ToUniversalTime();
        if (universalTime1 > DateTimeUtil.Add(utcNow, validationParameters.ClockSkew))
          throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenInvalidSigningKeyException(LogHelper.FormatInvariant("IDX10248: X509SecurityKey validation failed. The associated certificate is not yet valid. ValidFrom (UTC): '{0}', Current time (UTC): '{1}'.", (object) universalTime1, (object) utcNow)));
        LogHelper.LogInformation("IDX10250: The associated certificate is valid. ValidFrom (UTC): '{0}', Current time (UTC): '{1}'.", (object) universalTime1, (object) utcNow);
        if (universalTime2 < DateTimeUtil.Add(utcNow, validationParameters.ClockSkew.Negate()))
          throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenInvalidSigningKeyException(LogHelper.FormatInvariant("IDX10249: X509SecurityKey validation failed. The associated certificate has expired. ValidTo (UTC): '{0}', Current time (UTC): '{1}'.", (object) universalTime2, (object) utcNow)));
        LogHelper.LogInformation("IDX10251: The associated certificate is valid. ValidTo (UTC): '{0}', Current time (UTC): '{1}'.", (object) universalTime2, (object) utcNow);
      }
    }
  }

  public static void ValidateLifetime(
    DateTime? notBefore,
    DateTime? expires,
    SecurityToken securityToken,
    TokenValidationParameters validationParameters)
  {
    if (validationParameters == null)
      throw LogHelper.LogArgumentNullException(nameof (validationParameters));
    if (!validationParameters.ValidateLifetime)
      LogHelper.LogInformation("IDX10238: ValidateLifetime property on ValidationParameters is set to false. Exiting without validating the lifetime.");
    else if (validationParameters.LifetimeValidator != null)
    {
      if (!validationParameters.LifetimeValidator(notBefore, expires, securityToken, validationParameters))
        throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenInvalidLifetimeException(LogHelper.FormatInvariant("IDX10230: Lifetime validation failed. Delegate returned false, securitytoken: '{0}'.", (object) securityToken))
        {
          NotBefore = notBefore,
          Expires = expires
        });
    }
    else
    {
      if (!expires.HasValue && validationParameters.RequireExpirationTime)
        throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenNoExpirationException(LogHelper.FormatInvariant("IDX10225: Lifetime validation failed. The token is missing an Expiration Time. Tokentype: '{0}'.", securityToken == null ? (object) "null" : (object) securityToken.GetType().ToString())));
      if (notBefore.HasValue && expires.HasValue && notBefore.Value > expires.Value)
        throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenInvalidLifetimeException(LogHelper.FormatInvariant("IDX10224: Lifetime validation failed. The NotBefore: '{0}' is after Expires: '{1}'.", (object) notBefore.Value, (object) expires.Value))
        {
          NotBefore = notBefore,
          Expires = expires
        });
      DateTime utcNow = DateTime.UtcNow;
      if (notBefore.HasValue && notBefore.Value > DateTimeUtil.Add(utcNow, validationParameters.ClockSkew))
        throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenNotYetValidException(LogHelper.FormatInvariant("IDX10222: Lifetime validation failed. The token is not yet valid. ValidFrom: '{0}', Current time: '{1}'.", (object) notBefore.Value, (object) utcNow))
        {
          NotBefore = notBefore.Value
        });
      if (expires.HasValue && expires.Value < DateTimeUtil.Add(utcNow, validationParameters.ClockSkew.Negate()))
        throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenExpiredException(LogHelper.FormatInvariant("IDX10223: Lifetime validation failed. The token is expired. ValidTo: '{0}', Current time: '{1}'.", (object) expires.Value, (object) utcNow))
        {
          Expires = expires.Value
        });
      LogHelper.LogInformation("IDX10239: Lifetime of the token is valid.");
    }
  }

  public static void ValidateTokenReplay(
    DateTime? expirationTime,
    string securityToken,
    TokenValidationParameters validationParameters)
  {
    if (string.IsNullOrWhiteSpace(securityToken))
      throw LogHelper.LogArgumentNullException(nameof (securityToken));
    if (validationParameters == null)
      throw LogHelper.LogArgumentNullException(nameof (validationParameters));
    if (!validationParameters.ValidateTokenReplay)
      LogHelper.LogInformation("IDX10246: ValidateTokenReplay property on ValidationParameters is set to false. Exiting without validating the token replay.");
    else if (validationParameters.TokenReplayValidator != null)
    {
      if (!validationParameters.TokenReplayValidator(expirationTime, securityToken, validationParameters))
        throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenReplayDetectedException(LogHelper.FormatInvariant("IDX10228: The securityToken has previously been validated, securityToken: '{0}'.", (object) securityToken)));
    }
    else
    {
      if (validationParameters.TokenReplayCache != null)
      {
        if (!expirationTime.HasValue)
          throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenNoExpirationException(LogHelper.FormatInvariant("IDX10227: TokenValidationParameters.TokenReplayCache is not null, indicating to check for token replay but the security token has no expiration time: token '{0}'.", (object) securityToken)));
        if (validationParameters.TokenReplayCache.TryFind(securityToken))
          throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenReplayDetectedException(LogHelper.FormatInvariant("IDX10228: The securityToken has previously been validated, securityToken: '{0}'.", (object) securityToken)));
        if (!validationParameters.TokenReplayCache.TryAdd(securityToken, expirationTime.Value))
          throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenReplayAddFailedException(LogHelper.FormatInvariant("IDX10229: TokenValidationParameters.TokenReplayCache was unable to add the securityToken: '{0}'.", (object) securityToken)));
      }
      LogHelper.LogInformation("IDX10240: No token replay is detected.");
    }
  }

  public static void ValidateTokenReplay(
    string securityToken,
    DateTime? expirationTime,
    TokenValidationParameters validationParameters)
  {
    Validators.ValidateTokenReplay(expirationTime, securityToken, validationParameters);
  }
}
