// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using System;
using System.Collections.Generic;
using System.Security.Claims;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class SecurityTokenDescriptor
{
  public string Audience { get; set; }

  public string CompressionAlgorithm { get; set; }

  public EncryptingCredentials EncryptingCredentials { get; set; }

  public DateTime? Expires { get; set; }

  public string Issuer { get; set; }

  public DateTime? IssuedAt { get; set; }

  public DateTime? NotBefore { get; set; }

  public IDictionary<string, object> Claims { get; set; }

  public IDictionary<string, object> AdditionalHeaderClaims { get; set; }

  public SigningCredentials SigningCredentials { get; set; }

  public ClaimsIdentity Subject { get; set; }
}
