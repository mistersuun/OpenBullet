// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.SecurityTokenInvalidSigningKeyException
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using System;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class SecurityTokenInvalidSigningKeyException : SecurityTokenValidationException
{
  public SecurityKey SigningKey { get; set; }

  public SecurityTokenInvalidSigningKeyException()
    : base("SecurityToken has invalid issuer signing key.")
  {
  }

  public SecurityTokenInvalidSigningKeyException(string message)
    : base(message)
  {
  }

  public SecurityTokenInvalidSigningKeyException(string message, Exception inner)
    : base(message, inner)
  {
  }
}
