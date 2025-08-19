// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.X509SecurityKey
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class X509SecurityKey : AsymmetricSecurityKey
{
  private AsymmetricAlgorithm _privateKey;
  private bool _privateKeyAvailabilityDetermined;
  private AsymmetricAlgorithm _publicKey;
  private object _thisLock = new object();

  internal X509SecurityKey(JsonWebKey webKey)
    : base((SecurityKey) webKey)
  {
    this.Certificate = new X509Certificate2(Convert.FromBase64String(webKey.X5c[0]));
    this.X5t = Base64UrlEncoder.Encode(this.Certificate.GetCertHash());
    webKey.ConvertedSecurityKey = (SecurityKey) this;
  }

  public X509SecurityKey(X509Certificate2 certificate)
  {
    this.Certificate = certificate ?? throw LogHelper.LogArgumentNullException(nameof (certificate));
    this.KeyId = certificate.Thumbprint;
    this.X5t = Base64UrlEncoder.Encode(certificate.GetCertHash());
  }

  public X509SecurityKey(X509Certificate2 certificate, string keyId)
  {
    this.Certificate = certificate ?? throw LogHelper.LogArgumentNullException(nameof (certificate));
    this.KeyId = !string.IsNullOrEmpty(keyId) ? keyId : throw LogHelper.LogArgumentNullException(nameof (keyId));
    this.X5t = Base64UrlEncoder.Encode(certificate.GetCertHash());
  }

  public override int KeySize => this.PublicKey.KeySize;

  public string X5t { get; }

  public AsymmetricAlgorithm PrivateKey
  {
    get
    {
      if (!this._privateKeyAvailabilityDetermined)
      {
        lock (this.ThisLock)
        {
          if (!this._privateKeyAvailabilityDetermined)
          {
            this._privateKey = (AsymmetricAlgorithm) RSACertificateExtensions.GetRSAPrivateKey(this.Certificate);
            this._privateKeyAvailabilityDetermined = true;
          }
        }
      }
      return this._privateKey;
    }
  }

  public AsymmetricAlgorithm PublicKey
  {
    get
    {
      if (this._publicKey == null)
      {
        lock (this.ThisLock)
        {
          if (this._publicKey == null)
            this._publicKey = (AsymmetricAlgorithm) RSACertificateExtensions.GetRSAPublicKey(this.Certificate);
        }
      }
      return this._publicKey;
    }
  }

  private object ThisLock => this._thisLock;

  [Obsolete("HasPrivateKey method is deprecated, please use PrivateKeyStatus.")]
  public override bool HasPrivateKey => this.PrivateKey != null;

  public override PrivateKeyStatus PrivateKeyStatus
  {
    get => this.PrivateKey != null ? PrivateKeyStatus.Exists : PrivateKeyStatus.DoesNotExist;
  }

  public X509Certificate2 Certificate { get; private set; }

  public override bool Equals(object obj)
  {
    return obj is X509SecurityKey x509SecurityKey && x509SecurityKey.Certificate.Thumbprint.ToString() == this.Certificate.Thumbprint.ToString();
  }

  public override int GetHashCode() => this.Certificate.GetHashCode();
}
