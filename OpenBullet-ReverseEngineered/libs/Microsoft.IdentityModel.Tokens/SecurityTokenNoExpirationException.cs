// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.SecurityTokenNoExpirationException
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using System;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class SecurityTokenNoExpirationException : SecurityTokenValidationException
{
  public SecurityTokenNoExpirationException()
  {
  }

  public SecurityTokenNoExpirationException(string message)
    : base(message)
  {
  }

  public SecurityTokenNoExpirationException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}
