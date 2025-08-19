// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.TokenValidationParameters
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Claims;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class TokenValidationParameters
{
  private string _authenticationType;
  private TimeSpan _clockSkew = TokenValidationParameters.DefaultClockSkew;
  private string _nameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
  private string _roleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
  public static readonly string DefaultAuthenticationType = "AuthenticationTypes.Federation";
  public static readonly TimeSpan DefaultClockSkew = TimeSpan.FromSeconds(300.0);
  public const int DefaultMaximumTokenSizeInBytes = 256000;

  protected TokenValidationParameters(TokenValidationParameters other)
  {
    if (other == null)
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentNullException(nameof (other)));
    this.ActorValidationParameters = other.ActorValidationParameters?.Clone();
    this.AudienceValidator = other.AudienceValidator;
    this._authenticationType = other._authenticationType;
    this.ClockSkew = other.ClockSkew;
    this.CryptoProviderFactory = other.CryptoProviderFactory;
    this.IssuerSigningKey = other.IssuerSigningKey;
    this.IssuerSigningKeyResolver = other.IssuerSigningKeyResolver;
    this.IssuerSigningKeys = other.IssuerSigningKeys;
    this.IssuerSigningKeyValidator = other.IssuerSigningKeyValidator;
    this.IssuerValidator = other.IssuerValidator;
    this.LifetimeValidator = other.LifetimeValidator;
    this.TokenReplayValidator = other.TokenReplayValidator;
    this.NameClaimType = other.NameClaimType;
    this.NameClaimTypeRetriever = other.NameClaimTypeRetriever;
    this.PropertyBag = other.PropertyBag;
    this.RequireAudience = other.RequireAudience;
    this.RequireExpirationTime = other.RequireExpirationTime;
    this.RequireSignedTokens = other.RequireSignedTokens;
    this.RoleClaimType = other.RoleClaimType;
    this.RoleClaimTypeRetriever = other.RoleClaimTypeRetriever;
    this.SaveSigninToken = other.SaveSigninToken;
    this.SignatureValidator = other.SignatureValidator;
    this.TokenDecryptionKey = other.TokenDecryptionKey;
    this.TokenDecryptionKeyResolver = other.TokenDecryptionKeyResolver;
    this.TokenDecryptionKeys = other.TokenDecryptionKeys;
    this.TokenReader = other.TokenReader;
    this.TokenReplayCache = other.TokenReplayCache;
    this.ValidateActor = other.ValidateActor;
    this.ValidateAudience = other.ValidateAudience;
    this.ValidateIssuer = other.ValidateIssuer;
    this.ValidateIssuerSigningKey = other.ValidateIssuerSigningKey;
    this.ValidateLifetime = other.ValidateLifetime;
    this.ValidateTokenReplay = other.ValidateTokenReplay;
    this.ValidAudience = other.ValidAudience;
    this.ValidAudiences = other.ValidAudiences;
    this.ValidIssuer = other.ValidIssuer;
    this.ValidIssuers = other.ValidIssuers;
  }

  public TokenValidationParameters()
  {
    this.RequireExpirationTime = true;
    this.RequireSignedTokens = true;
    this.RequireAudience = true;
    this.SaveSigninToken = false;
    this.ValidateActor = false;
    this.ValidateAudience = true;
    this.ValidateIssuer = true;
    this.ValidateIssuerSigningKey = false;
    this.ValidateLifetime = true;
    this.ValidateTokenReplay = false;
  }

  public TokenValidationParameters ActorValidationParameters { get; set; }

  public AudienceValidator AudienceValidator { get; set; }

  public string AuthenticationType
  {
    get => this._authenticationType;
    set
    {
      this._authenticationType = !string.IsNullOrWhiteSpace(value) ? value : throw LogHelper.LogExceptionMessage((Exception) new ArgumentNullException(nameof (AuthenticationType)));
    }
  }

  [DefaultValue(300)]
  public TimeSpan ClockSkew
  {
    get => this._clockSkew;
    set
    {
      this._clockSkew = !(value < TimeSpan.Zero) ? value : throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException(nameof (value), LogHelper.FormatInvariant("IDX10100: ClockSkew must be greater than TimeSpan.Zero. value: '{0}'", (object) value)));
    }
  }

  public virtual TokenValidationParameters Clone() => new TokenValidationParameters(this);

  public virtual ClaimsIdentity CreateClaimsIdentity(SecurityToken securityToken, string issuer)
  {
    string str1 = this.NameClaimTypeRetriever == null ? this.NameClaimType : this.NameClaimTypeRetriever(securityToken, issuer);
    string str2 = this.RoleClaimTypeRetriever == null ? this.RoleClaimType : this.RoleClaimTypeRetriever(securityToken, issuer);
    LogHelper.LogInformation("IDX10245: Creating claims identity from the validated token: '{0}'.", (object) securityToken);
    return new ClaimsIdentity(this.AuthenticationType ?? TokenValidationParameters.DefaultAuthenticationType, str1 ?? "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", str2 ?? "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
  }

  public CryptoProviderFactory CryptoProviderFactory { get; set; }

  public IssuerSigningKeyValidator IssuerSigningKeyValidator { get; set; }

  public SecurityKey IssuerSigningKey { get; set; }

  public IssuerSigningKeyResolver IssuerSigningKeyResolver { get; set; }

  public IEnumerable<SecurityKey> IssuerSigningKeys { get; set; }

  public IssuerValidator IssuerValidator { get; set; }

  public LifetimeValidator LifetimeValidator { get; set; }

  public string NameClaimType
  {
    get => this._nameClaimType;
    set
    {
      this._nameClaimType = !string.IsNullOrWhiteSpace(value) ? value : throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException(nameof (value), "IDX10102: NameClaimType cannot be null or whitespace."));
    }
  }

  public string RoleClaimType
  {
    get => this._roleClaimType;
    set
    {
      this._roleClaimType = !string.IsNullOrWhiteSpace(value) ? value : throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException(nameof (value), "IDX10103: RoleClaimType cannot be null or whitespace."));
    }
  }

  public Func<SecurityToken, string, string> NameClaimTypeRetriever { get; set; }

  public IDictionary<string, object> PropertyBag { get; set; }

  [DefaultValue(true)]
  public bool RequireAudience { get; set; }

  [DefaultValue(true)]
  public bool RequireExpirationTime { get; set; }

  [DefaultValue(true)]
  public bool RequireSignedTokens { get; set; }

  public Func<SecurityToken, string, string> RoleClaimTypeRetriever { get; set; }

  [DefaultValue(false)]
  public bool SaveSigninToken { get; set; }

  public SignatureValidator SignatureValidator { get; set; }

  public SecurityKey TokenDecryptionKey { get; set; }

  public TokenDecryptionKeyResolver TokenDecryptionKeyResolver { get; set; }

  public IEnumerable<SecurityKey> TokenDecryptionKeys { get; set; }

  public TokenReader TokenReader { get; set; }

  public ITokenReplayCache TokenReplayCache { get; set; }

  public TokenReplayValidator TokenReplayValidator { get; set; }

  [DefaultValue(false)]
  public bool ValidateActor { get; set; }

  [DefaultValue(true)]
  public bool ValidateAudience { get; set; }

  [DefaultValue(true)]
  public bool ValidateIssuer { get; set; }

  [DefaultValue(true)]
  public bool ValidateLifetime { get; set; }

  [DefaultValue(false)]
  public bool ValidateIssuerSigningKey { get; set; }

  [DefaultValue(false)]
  public bool ValidateTokenReplay { get; set; }

  public string ValidAudience { get; set; }

  public IEnumerable<string> ValidAudiences { get; set; }

  public string ValidIssuer { get; set; }

  public IEnumerable<string> ValidIssuers { get; set; }

  public IEnumerable<string> ValidTypes { get; set; }
}
