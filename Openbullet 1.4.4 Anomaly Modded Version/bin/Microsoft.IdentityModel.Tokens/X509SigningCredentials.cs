// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.X509SigningCredentials
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using System.Security.Cryptography.X509Certificates;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class X509SigningCredentials : SigningCredentials
{
  public X509SigningCredentials(X509Certificate2 certificate)
    : base(certificate)
  {
    this.Certificate = certificate;
  }

  public X509SigningCredentials(X509Certificate2 certificate, string algorithm)
    : base(certificate, algorithm)
  {
    this.Certificate = certificate;
  }

  public X509Certificate2 Certificate { get; private set; }
}
