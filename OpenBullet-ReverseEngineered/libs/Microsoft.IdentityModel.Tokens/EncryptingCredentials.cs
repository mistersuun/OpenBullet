// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.EncryptingCredentials
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System.Security.Cryptography.X509Certificates;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class EncryptingCredentials
{
  private string _alg;
  private string _enc;
  private SecurityKey _key;

  protected EncryptingCredentials(X509Certificate2 certificate, string alg, string enc)
  {
    this.Key = certificate != null ? (SecurityKey) new X509SecurityKey(certificate) : throw LogHelper.LogArgumentNullException(nameof (certificate));
    this.Alg = alg;
    this.Enc = enc;
  }

  public EncryptingCredentials(SecurityKey key, string alg, string enc)
  {
    this.Key = key;
    this.Alg = alg;
    this.Enc = enc;
  }

  public EncryptingCredentials(SymmetricSecurityKey key, string enc)
    : this((SecurityKey) key, "none", enc)
  {
  }

  public string Alg
  {
    get => this._alg;
    private set
    {
      this._alg = !string.IsNullOrEmpty(value) ? value : throw LogHelper.LogArgumentNullException("alg");
    }
  }

  public string Enc
  {
    get => this._enc;
    private set
    {
      this._enc = !string.IsNullOrEmpty(value) ? value : throw LogHelper.LogArgumentNullException("enc");
    }
  }

  public CryptoProviderFactory CryptoProviderFactory { get; set; }

  public SecurityKey Key
  {
    get => this._key;
    private set => this._key = value ?? throw LogHelper.LogArgumentNullException("key");
  }
}
