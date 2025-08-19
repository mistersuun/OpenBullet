// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.RsaSecurityKey
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.Security.Cryptography;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class RsaSecurityKey : AsymmetricSecurityKey
{
  private bool? _hasPrivateKey;
  private bool _foundPrivateKeyDetermined;
  private PrivateKeyStatus _foundPrivateKey;

  internal RsaSecurityKey(JsonWebKey webKey)
    : base((SecurityKey) webKey)
  {
    this.IntializeWithRsaParameters(webKey.CreateRsaParameters());
    webKey.ConvertedSecurityKey = (SecurityKey) this;
  }

  public RsaSecurityKey(RSAParameters rsaParameters)
  {
    this.IntializeWithRsaParameters(rsaParameters);
  }

  internal void IntializeWithRsaParameters(RSAParameters rsaParameters)
  {
    if (rsaParameters.Modulus == null)
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10700: {0} is unable to use 'rsaParameters'. {1} is null.", (object) this, (object) "Modulus")));
    if (rsaParameters.Exponent == null)
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10700: {0} is unable to use 'rsaParameters'. {1} is null.", (object) this, (object) "Exponent")));
    this._hasPrivateKey = new bool?(rsaParameters.D != null && rsaParameters.DP != null && rsaParameters.DQ != null && rsaParameters.P != null && rsaParameters.Q != null && rsaParameters.InverseQ != null);
    this._foundPrivateKey = this._hasPrivateKey.Value ? PrivateKeyStatus.Exists : PrivateKeyStatus.DoesNotExist;
    this._foundPrivateKeyDetermined = true;
    this.Parameters = rsaParameters;
  }

  public RsaSecurityKey(RSA rsa)
  {
    this.Rsa = rsa ?? throw LogHelper.LogArgumentNullException(nameof (rsa));
  }

  [Obsolete("HasPrivateKey method is deprecated, please use PrivateKeyStatus.")]
  public override bool HasPrivateKey
  {
    get
    {
      if (!this._hasPrivateKey.HasValue)
      {
        try
        {
          this.Rsa.SignData(new byte[20], HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
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

  public override PrivateKeyStatus PrivateKeyStatus
  {
    get
    {
      if (this._foundPrivateKeyDetermined)
        return this._foundPrivateKey;
      this._foundPrivateKeyDetermined = true;
      if (this.Rsa != null)
      {
        try
        {
          RSAParameters rsaParameters = this.Rsa.ExportParameters(true);
          this._foundPrivateKey = rsaParameters.D == null || rsaParameters.DP == null || rsaParameters.DQ == null || rsaParameters.P == null || rsaParameters.Q == null || rsaParameters.InverseQ == null ? PrivateKeyStatus.DoesNotExist : PrivateKeyStatus.Exists;
        }
        catch (Exception ex)
        {
          this._foundPrivateKey = PrivateKeyStatus.Unknown;
          return this._foundPrivateKey;
        }
      }
      else
        this._foundPrivateKey = this.Parameters.D == null || this.Parameters.DP == null || this.Parameters.DQ == null || this.Parameters.P == null || this.Parameters.Q == null || this.Parameters.InverseQ == null ? PrivateKeyStatus.DoesNotExist : PrivateKeyStatus.Exists;
      return this._foundPrivateKey;
    }
  }

  public override int KeySize
  {
    get
    {
      if (this.Rsa != null)
        return this.Rsa.KeySize;
      return this.Parameters.Modulus != null ? this.Parameters.Modulus.Length * 8 : 0;
    }
  }

  public RSAParameters Parameters { get; private set; }

  public RSA Rsa { get; private set; }
}
