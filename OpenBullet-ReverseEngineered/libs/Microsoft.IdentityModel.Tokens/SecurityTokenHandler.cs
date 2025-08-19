// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.SecurityTokenHandler
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using System;
using System.Security.Claims;
using System.Xml;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public abstract class SecurityTokenHandler : TokenHandler, ISecurityTokenValidator
{
  public virtual SecurityKeyIdentifierClause CreateSecurityTokenReference(
    SecurityToken token,
    bool attached)
  {
    throw new NotImplementedException();
  }

  public virtual SecurityToken CreateToken(SecurityTokenDescriptor tokenDescriptor)
  {
    throw new NotImplementedException();
  }

  public virtual bool CanValidateToken => false;

  public virtual bool CanWriteToken => false;

  public abstract Type TokenType { get; }

  public virtual bool CanReadToken(XmlReader reader) => false;

  public virtual bool CanReadToken(string tokenString) => false;

  public virtual SecurityToken ReadToken(string tokenString) => (SecurityToken) null;

  public virtual SecurityToken ReadToken(XmlReader reader) => (SecurityToken) null;

  public virtual string WriteToken(SecurityToken token) => (string) null;

  public abstract void WriteToken(XmlWriter writer, SecurityToken token);

  public abstract SecurityToken ReadToken(
    XmlReader reader,
    TokenValidationParameters validationParameters);

  public virtual ClaimsPrincipal ValidateToken(
    string securityToken,
    TokenValidationParameters validationParameters,
    out SecurityToken validatedToken)
  {
    throw new NotImplementedException();
  }

  public virtual ClaimsPrincipal ValidateToken(
    XmlReader reader,
    TokenValidationParameters validationParameters,
    out SecurityToken validatedToken)
  {
    throw new NotImplementedException();
  }
}
