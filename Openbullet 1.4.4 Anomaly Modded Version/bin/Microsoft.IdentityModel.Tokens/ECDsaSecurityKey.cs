// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.ECDsaSecurityKey
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.Security.Cryptography;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class ECDsaSecurityKey : AsymmetricSecurityKey
{
  private bool? _hasPrivateKey;

  internal ECDsaSecurityKey(JsonWebKey webKey, bool usePrivateKey)
    : base((SecurityKey) webKey)
  {
    this.ECDsa = ECDsaAdapter.Instance.CreateECDsa(webKey, usePrivateKey);
    webKey.ConvertedSecurityKey = (SecurityKey) this;
  }

  public ECDsaSecurityKey(ECDsa ecdsa)
  {
    this.ECDsa = ecdsa ?? throw LogHelper.LogArgumentNullException(nameof (ecdsa));
  }

  public ECDsa ECDsa { get; private set; }

  [Obsolete("HasPrivateKey method is deprecated, please use PrivateKeyStatus.")]
  public override bool HasPrivateKey
  {
    get
    {
      if (!this._hasPrivateKey.HasValue)
      {
        try
        {
          this.ECDsa.SignHash(new byte[20]);
          this._hasPrivateKey = new bool?(true);
        }
        catch (CryptographicException ex)
        {
          this._hasPrivateKey = new bool?(false);
        }
      }
      return this._hasPrivateKey.Value;
    }
  }

  public override PrivateKeyStatus PrivateKeyStatus => PrivateKeyStatus.Unknown;

  public override int KeySize => this.ECDsa.KeySize;
}
