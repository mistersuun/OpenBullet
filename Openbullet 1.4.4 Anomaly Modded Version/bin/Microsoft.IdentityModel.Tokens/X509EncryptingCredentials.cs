// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.X509EncryptingCredentials
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using System.Security.Cryptography.X509Certificates;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class X509EncryptingCredentials : EncryptingCredentials
{
  public X509EncryptingCredentials(X509Certificate2 certificate)
    : this(certificate, "http://www.w3.org/2001/04/xmlenc#rsa-oaep", "A128CBC-HS256")
  {
  }

  public X509EncryptingCredentials(
    X509Certificate2 certificate,
    string keyWrapAlgorithm,
    string dataEncryptionAlgorithm)
    : base(certificate, keyWrapAlgorithm, dataEncryptionAlgorithm)
  {
    this.Certificate = certificate;
  }

  public X509Certificate2 Certificate { get; private set; }
}
