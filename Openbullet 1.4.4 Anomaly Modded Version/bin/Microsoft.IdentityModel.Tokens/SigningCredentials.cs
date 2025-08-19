// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.SigningCredentials
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System.Security.Cryptography.X509Certificates;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class SigningCredentials
{
  private string _algorithm;
  private string _digest;
  private SecurityKey _key;

  protected SigningCredentials(X509Certificate2 certificate)
  {
    this.Key = certificate != null ? (SecurityKey) new X509SecurityKey(certificate) : throw LogHelper.LogArgumentNullException(nameof (certificate));
    this.Algorithm = "RS256";
  }

  protected SigningCredentials(X509Certificate2 certificate, string algorithm)
  {
    this.Key = certificate != null ? (SecurityKey) new X509SecurityKey(certificate) : throw LogHelper.LogArgumentNullException(nameof (certificate));
    this.Algorithm = algorithm;
  }

  public SigningCredentials(SecurityKey key, string algorithm)
  {
    this.Key = key;
    this.Algorithm = algorithm;
  }

  public SigningCredentials(SecurityKey key, string algorithm, string digest)
  {
    this.Key = key;
    this.Algorithm = algorithm;
    this.Digest = digest;
  }

  public string Algorithm
  {
    get => this._algorithm;
    private set
    {
      this._algorithm = !string.IsNullOrEmpty(value) ? value : throw LogHelper.LogArgumentNullException("algorithm");
    }
  }

  public string Digest
  {
    get => this._digest;
    private set
    {
      this._digest = !string.IsNullOrEmpty(value) ? value : throw LogHelper.LogArgumentNullException("digest");
    }
  }

  public CryptoProviderFactory CryptoProviderFactory { get; set; }

  public SecurityKey Key
  {
    get => this._key;
    private set => this._key = value ?? throw LogHelper.LogArgumentNullException("key");
  }

  public string Kid => this.Key.KeyId;
}
