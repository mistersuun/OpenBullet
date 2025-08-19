// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.AsymmetricSignatureProvider
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class AsymmetricSignatureProvider : SignatureProvider
{
  private bool _disposed;
  private AsymmetricAdapter _asymmetricAdapter;
  private CryptoProviderFactory _cryptoProviderFactory;
  private IReadOnlyDictionary<string, int> _minimumAsymmetricKeySizeInBitsForSigningMap;
  private IReadOnlyDictionary<string, int> _minimumAsymmetricKeySizeInBitsForVerifyingMap;
  public static readonly Dictionary<string, int> DefaultMinimumAsymmetricKeySizeInBitsForSigningMap = new Dictionary<string, int>()
  {
    {
      "ES256",
      256 /*0x0100*/
    },
    {
      "ES384",
      256 /*0x0100*/
    },
    {
      "ES512",
      256 /*0x0100*/
    },
    {
      "RS256",
      2048 /*0x0800*/
    },
    {
      "RS384",
      2048 /*0x0800*/
    },
    {
      "RS512",
      2048 /*0x0800*/
    },
    {
      "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256",
      2048 /*0x0800*/
    },
    {
      "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384",
      2048 /*0x0800*/
    },
    {
      "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512",
      2048 /*0x0800*/
    },
    {
      "PS256",
      528
    },
    {
      "PS384",
      784
    },
    {
      "PS512",
      1040
    },
    {
      "http://www.w3.org/2007/05/xmldsig-more#sha256-rsa-MGF1",
      528
    },
    {
      "http://www.w3.org/2007/05/xmldsig-more#sha384-rsa-MGF1",
      784
    },
    {
      "http://www.w3.org/2007/05/xmldsig-more#sha512-rsa-MGF1",
      1040
    }
  };
  public static readonly Dictionary<string, int> DefaultMinimumAsymmetricKeySizeInBitsForVerifyingMap = new Dictionary<string, int>()
  {
    {
      "ES256",
      256 /*0x0100*/
    },
    {
      "ES384",
      256 /*0x0100*/
    },
    {
      "ES512",
      256 /*0x0100*/
    },
    {
      "RS256",
      1024 /*0x0400*/
    },
    {
      "RS384",
      1024 /*0x0400*/
    },
    {
      "RS512",
      1024 /*0x0400*/
    },
    {
      "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256",
      1024 /*0x0400*/
    },
    {
      "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384",
      1024 /*0x0400*/
    },
    {
      "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512",
      1024 /*0x0400*/
    },
    {
      "PS256",
      528
    },
    {
      "PS384",
      784
    },
    {
      "PS512",
      1040
    },
    {
      "http://www.w3.org/2007/05/xmldsig-more#sha256-rsa-MGF1",
      528
    },
    {
      "http://www.w3.org/2007/05/xmldsig-more#sha384-rsa-MGF1",
      784
    },
    {
      "http://www.w3.org/2007/05/xmldsig-more#sha512-rsa-MGF1",
      1040
    }
  };

  internal AsymmetricSignatureProvider(
    SecurityKey key,
    string algorithm,
    CryptoProviderFactory cryptoProviderFactory)
    : this(key, algorithm)
  {
    this._cryptoProviderFactory = cryptoProviderFactory;
  }

  internal AsymmetricSignatureProvider(
    SecurityKey key,
    string algorithm,
    bool willCreateSignatures,
    CryptoProviderFactory cryptoProviderFactory)
    : this(key, algorithm, willCreateSignatures)
  {
    this._cryptoProviderFactory = cryptoProviderFactory;
  }

  public AsymmetricSignatureProvider(SecurityKey key, string algorithm)
    : this(key, algorithm, false)
  {
  }

  public AsymmetricSignatureProvider(SecurityKey key, string algorithm, bool willCreateSignatures)
    : base(key, algorithm)
  {
    this._cryptoProviderFactory = key.CryptoProviderFactory;
    this._minimumAsymmetricKeySizeInBitsForSigningMap = (IReadOnlyDictionary<string, int>) new Dictionary<string, int>((IDictionary<string, int>) AsymmetricSignatureProvider.DefaultMinimumAsymmetricKeySizeInBitsForSigningMap);
    this._minimumAsymmetricKeySizeInBitsForVerifyingMap = (IReadOnlyDictionary<string, int>) new Dictionary<string, int>((IDictionary<string, int>) AsymmetricSignatureProvider.DefaultMinimumAsymmetricKeySizeInBitsForVerifyingMap);
    if (key is JsonWebKey webKey)
      JsonWebKeyConverter.TryConvertToSecurityKey(webKey, out SecurityKey _);
    if (willCreateSignatures && this.FoundPrivateKey(key) == PrivateKeyStatus.DoesNotExist)
      throw LogHelper.LogExceptionMessage((Exception) new InvalidOperationException(LogHelper.FormatInvariant("IDX10638: Cannot create the SignatureProvider, 'key.HasPrivateKey' is false, cannot create signatures. Key: {0}.", (object) key)));
    if (!this._cryptoProviderFactory.IsSupportedAlgorithm(algorithm, key))
      throw LogHelper.LogExceptionMessage((Exception) new NotSupportedException(LogHelper.FormatInvariant("IDX10634: Unable to create the SignatureProvider.\nAlgorithm: '{0}', SecurityKey: '{1}'\n is not supported. The list of supported algorithms is available here: https://aka.ms/IdentityModel/supported-algorithms", (object) (algorithm ?? "null"), (object) key)));
    this.ValidateAsymmetricSecurityKeySize(key, algorithm, willCreateSignatures);
    this._asymmetricAdapter = this.ResolveAsymmetricAdapter(webKey?.ConvertedSecurityKey ?? key, algorithm, willCreateSignatures);
    this.WillCreateSignatures = willCreateSignatures;
  }

  public IReadOnlyDictionary<string, int> MinimumAsymmetricKeySizeInBitsForSigningMap
  {
    get => this._minimumAsymmetricKeySizeInBitsForSigningMap;
  }

  public IReadOnlyDictionary<string, int> MinimumAsymmetricKeySizeInBitsForVerifyingMap
  {
    get => this._minimumAsymmetricKeySizeInBitsForVerifyingMap;
  }

  private PrivateKeyStatus FoundPrivateKey(SecurityKey key)
  {
    switch (key)
    {
      case AsymmetricSecurityKey asymmetricSecurityKey:
        return asymmetricSecurityKey.PrivateKeyStatus;
      case JsonWebKey jsonWebKey:
        return !jsonWebKey.HasPrivateKey ? PrivateKeyStatus.DoesNotExist : PrivateKeyStatus.Exists;
      default:
        return PrivateKeyStatus.Unknown;
    }
  }

  protected virtual HashAlgorithmName GetHashAlgorithmName(string algorithm)
  {
    if (string.IsNullOrWhiteSpace(algorithm))
      throw LogHelper.LogArgumentNullException(nameof (algorithm));
    switch (algorithm)
    {
      case "ES256":
      case "PS256":
      case "RS256":
      case "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha256":
      case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256":
      case "http://www.w3.org/2007/05/xmldsig-more#sha256-rsa-MGF1":
        return HashAlgorithmName.SHA256;
      case "ES384":
      case "PS384":
      case "RS384":
      case "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha384":
      case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384":
      case "http://www.w3.org/2007/05/xmldsig-more#sha384-rsa-MGF1":
        return HashAlgorithmName.SHA384;
      case "ES512":
      case "PS512":
      case "RS512":
      case "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha512":
      case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512":
      case "http://www.w3.org/2007/05/xmldsig-more#sha512-rsa-MGF1":
        return HashAlgorithmName.SHA512;
      default:
        throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException(nameof (algorithm), LogHelper.FormatInvariant("IDX10652: The algorithm '{0}' is not supported.", (object) algorithm)));
    }
  }

  private AsymmetricAdapter ResolveAsymmetricAdapter(
    SecurityKey key,
    string algorithm,
    bool requirePrivateKey)
  {
    HashAlgorithmName hashAlgorithmName = this.GetHashAlgorithmName(algorithm);
    return new AsymmetricAdapter(key, algorithm, this._cryptoProviderFactory.CreateHashAlgorithm(hashAlgorithmName), hashAlgorithmName, requirePrivateKey);
  }

  public override byte[] Sign(byte[] input)
  {
    if (input == null || input.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (input));
    if (this._disposed)
    {
      this.CryptoProviderCache?.TryRemove((SignatureProvider) this);
      throw LogHelper.LogExceptionMessage((Exception) new ObjectDisposedException(this.GetType().ToString()));
    }
    try
    {
      return this._asymmetricAdapter.Sign(input);
    }
    catch
    {
      this.CryptoProviderCache?.TryRemove((SignatureProvider) this);
      throw;
    }
  }

  public virtual void ValidateAsymmetricSecurityKeySize(
    SecurityKey key,
    string algorithm,
    bool willCreateSignatures)
  {
    if (key == null)
      throw LogHelper.LogArgumentNullException(nameof (key));
    if (string.IsNullOrEmpty(algorithm))
      throw LogHelper.LogArgumentNullException(nameof (algorithm));
    int keySize = key.KeySize;
    switch (key)
    {
      case AsymmetricSecurityKey asymmetricSecurityKey2:
        keySize = asymmetricSecurityKey2.KeySize;
        break;
      case JsonWebKey webKey:
        SecurityKey key1;
        JsonWebKeyConverter.TryConvertToSecurityKey(webKey, out key1);
        if (key1 is AsymmetricSecurityKey asymmetricSecurityKey1)
        {
          keySize = asymmetricSecurityKey1.KeySize;
          break;
        }
        if (key1 is SymmetricSecurityKey)
          throw LogHelper.LogExceptionMessage((Exception) new NotSupportedException(LogHelper.FormatInvariant("IDX10704: Cannot verify the key size. The SecurityKey is not or cannot be converted to an AsymmetricSecuritKey. SecurityKey: '{0}'.", (object) key)));
        break;
      default:
        throw LogHelper.LogExceptionMessage((Exception) new NotSupportedException(LogHelper.FormatInvariant("IDX10704: Cannot verify the key size. The SecurityKey is not or cannot be converted to an AsymmetricSecuritKey. SecurityKey: '{0}'.", (object) key)));
    }
    if (willCreateSignatures)
    {
      if (this.MinimumAsymmetricKeySizeInBitsForSigningMap.ContainsKey(algorithm) && keySize < this.MinimumAsymmetricKeySizeInBitsForSigningMap[algorithm])
        throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException("key.KeySize", LogHelper.FormatInvariant("IDX10630: The '{0}' for signing cannot be smaller than '{1}' bits. KeySize: '{2}'.", (object) key, (object) this.MinimumAsymmetricKeySizeInBitsForSigningMap[algorithm], (object) keySize)));
    }
    else if (this.MinimumAsymmetricKeySizeInBitsForVerifyingMap.ContainsKey(algorithm) && keySize < this.MinimumAsymmetricKeySizeInBitsForVerifyingMap[algorithm])
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException("key.KeySize", LogHelper.FormatInvariant("IDX10631: The '{0}' for verifying cannot be smaller than '{1}' bits. KeySize: '{2}'.", (object) key, (object) this.MinimumAsymmetricKeySizeInBitsForVerifyingMap[algorithm], (object) keySize)));
  }

  public override bool Verify(byte[] input, byte[] signature)
  {
    if (input == null || input.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (input));
    if (signature == null || signature.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (signature));
    if (this._disposed)
    {
      this.CryptoProviderCache?.TryRemove((SignatureProvider) this);
      throw LogHelper.LogExceptionMessage((Exception) new ObjectDisposedException(this.GetType().ToString()));
    }
    try
    {
      return this._asymmetricAdapter.Verify(input, signature);
    }
    catch
    {
      this.CryptoProviderCache?.TryRemove((SignatureProvider) this);
      throw;
    }
  }

  protected override void Dispose(bool disposing)
  {
    if (this._disposed)
      return;
    this._disposed = true;
    if (!disposing)
      return;
    this.CryptoProviderCache?.TryRemove((SignatureProvider) this);
    this._asymmetricAdapter.Dispose();
  }
}
