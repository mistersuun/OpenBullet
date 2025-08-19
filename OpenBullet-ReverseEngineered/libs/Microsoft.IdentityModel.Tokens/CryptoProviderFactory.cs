// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.CryptoProviderFactory
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Security.Cryptography;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class CryptoProviderFactory
{
  private static CryptoProviderFactory _default;
  private static ConcurrentDictionary<string, string> _typeToAlgorithmMap = new ConcurrentDictionary<string, string>();
  private static object _cacheLock = new object();

  public static CryptoProviderFactory Default
  {
    get => CryptoProviderFactory._default;
    set
    {
      CryptoProviderFactory._default = value ?? throw LogHelper.LogArgumentNullException(nameof (value));
    }
  }

  [DefaultValue(true)]
  public static bool DefaultCacheSignatureProviders { get; set; } = true;

  static CryptoProviderFactory() => CryptoProviderFactory.Default = new CryptoProviderFactory();

  public CryptoProviderFactory()
  {
  }

  public CryptoProviderFactory(CryptoProviderFactory other)
  {
    this.CustomCryptoProvider = other != null ? other.CustomCryptoProvider : throw LogHelper.LogArgumentNullException(nameof (other));
  }

  public CryptoProviderCache CryptoProviderCache { get; } = (CryptoProviderCache) new InMemoryCryptoProviderCache();

  public ICryptoProvider CustomCryptoProvider { get; set; }

  [DefaultValue(true)]
  public bool CacheSignatureProviders { get; set; } = CryptoProviderFactory.DefaultCacheSignatureProviders;

  public virtual AuthenticatedEncryptionProvider CreateAuthenticatedEncryptionProvider(
    SecurityKey key,
    string algorithm)
  {
    if (key == null)
      throw LogHelper.LogArgumentNullException(nameof (key));
    if (string.IsNullOrEmpty(algorithm))
      throw LogHelper.LogArgumentNullException(nameof (algorithm));
    if (this.CustomCryptoProvider != null)
    {
      if (this.CustomCryptoProvider.IsSupportedAlgorithm(algorithm, (object) key))
      {
        if (!(this.CustomCryptoProvider.Create(algorithm, (object) key) is AuthenticatedEncryptionProvider encryptionProvider))
          throw LogHelper.LogExceptionMessage((Exception) new InvalidOperationException(LogHelper.FormatInvariant("IDX10646: A CustomCryptoProvider was set and returned 'true' for IsSupportedAlgorithm(Algorithm: '{0}', Key: '{1}'), but Create.(algorithm, args) as '{2}' == NULL.", (object) algorithm, (object) key, (object) typeof (AuthenticatedEncryptionProvider))));
        return encryptionProvider;
      }
    }
    return SupportedAlgorithms.IsSupportedAuthenticatedEncryptionAlgorithm(algorithm, key) ? new AuthenticatedEncryptionProvider(key, algorithm) : throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10652: The algorithm '{0}' is not supported.", (object) algorithm), nameof (algorithm)));
  }

  public virtual KeyWrapProvider CreateKeyWrapProvider(SecurityKey key, string algorithm)
  {
    return this.CreateKeyWrapProvider(key, algorithm, false);
  }

  private KeyWrapProvider CreateKeyWrapProvider(SecurityKey key, string algorithm, bool willUnwrap)
  {
    if (key == null)
      throw LogHelper.LogArgumentNullException(nameof (key));
    if (string.IsNullOrEmpty(algorithm))
      throw LogHelper.LogArgumentNullException(nameof (algorithm));
    if (this.CustomCryptoProvider != null)
    {
      if (this.CustomCryptoProvider.IsSupportedAlgorithm(algorithm, (object) key, (object) willUnwrap))
      {
        if (!(this.CustomCryptoProvider.Create(algorithm, (object) key, (object) willUnwrap) is KeyWrapProvider keyWrapProvider))
          throw LogHelper.LogExceptionMessage((Exception) new InvalidOperationException(LogHelper.FormatInvariant("IDX10646: A CustomCryptoProvider was set and returned 'true' for IsSupportedAlgorithm(Algorithm: '{0}', Key: '{1}'), but Create.(algorithm, args) as '{2}' == NULL.", (object) algorithm, (object) key, (object) typeof (SignatureProvider))));
        return keyWrapProvider;
      }
    }
    if (key is RsaSecurityKey key1 && SupportedAlgorithms.IsSupportedRsaAlgorithm(algorithm, (SecurityKey) key1))
      return (KeyWrapProvider) new RsaKeyWrapProvider(key, algorithm, willUnwrap);
    if (key is X509SecurityKey key2 && SupportedAlgorithms.IsSupportedRsaAlgorithm(algorithm, (SecurityKey) key2))
      return (KeyWrapProvider) new RsaKeyWrapProvider((SecurityKey) key2, algorithm, willUnwrap);
    if (key is JsonWebKey key3)
    {
      if (key3.Kty == "RSA" && SupportedAlgorithms.IsSupportedRsaAlgorithm(algorithm, key))
        return (KeyWrapProvider) new RsaKeyWrapProvider((SecurityKey) key3, algorithm, willUnwrap);
      if (key3.Kty == "oct" && SupportedAlgorithms.IsSupportedSymmetricAlgorithm(algorithm))
        return (KeyWrapProvider) new SymmetricKeyWrapProvider((SecurityKey) key3, algorithm);
    }
    if (key is SymmetricSecurityKey key4 && SupportedAlgorithms.IsSupportedSymmetricAlgorithm(algorithm))
      return (KeyWrapProvider) new SymmetricKeyWrapProvider((SecurityKey) key4, algorithm);
    throw LogHelper.LogExceptionMessage((Exception) new NotSupportedException(LogHelper.FormatInvariant("IDX10661: Unable to create the KeyWrapProvider.\nKeyWrapAlgorithm: '{0}', SecurityKey: '{1}'\n is not supported.", (object) algorithm, (object) key)));
  }

  public virtual KeyWrapProvider CreateKeyWrapProviderForUnwrap(SecurityKey key, string algorithm)
  {
    return this.CreateKeyWrapProvider(key, algorithm, true);
  }

  public virtual SignatureProvider CreateForSigning(SecurityKey key, string algorithm)
  {
    return this.CreateSignatureProvider(key, algorithm, true);
  }

  public virtual SignatureProvider CreateForVerifying(SecurityKey key, string algorithm)
  {
    return this.CreateSignatureProvider(key, algorithm, false);
  }

  public virtual HashAlgorithm CreateHashAlgorithm(HashAlgorithmName algorithm)
  {
    if (this.CustomCryptoProvider != null && this.CustomCryptoProvider.IsSupportedAlgorithm(algorithm.Name))
    {
      if (!(this.CustomCryptoProvider.Create(algorithm.Name) is HashAlgorithm hashAlgorithm))
        throw LogHelper.LogExceptionMessage((Exception) new InvalidOperationException(LogHelper.FormatInvariant("IDX10647: A CustomCryptoProvider was set and returned 'true' for IsSupportedAlgorithm(Algorithm: '{0}'), but Create.(algorithm, args) as '{1}' == NULL.", (object) algorithm, (object) typeof (HashAlgorithm))));
      CryptoProviderFactory._typeToAlgorithmMap[hashAlgorithm.GetType().ToString()] = algorithm.Name;
      return hashAlgorithm;
    }
    if (HashAlgorithmName.op_Equality(algorithm, HashAlgorithmName.SHA256))
      return (HashAlgorithm) SHA256.Create();
    if (HashAlgorithmName.op_Equality(algorithm, HashAlgorithmName.SHA384))
      return (HashAlgorithm) SHA384.Create();
    if (HashAlgorithmName.op_Equality(algorithm, HashAlgorithmName.SHA512))
      return (HashAlgorithm) SHA512.Create();
    throw LogHelper.LogExceptionMessage((Exception) new NotSupportedException(LogHelper.FormatInvariant("IDX10640: Algorithm is not supported: '{0}'.", (object) algorithm)));
  }

  public virtual HashAlgorithm CreateHashAlgorithm(string algorithm)
  {
    if (string.IsNullOrEmpty(algorithm))
      throw LogHelper.LogArgumentNullException(nameof (algorithm));
    if (this.CustomCryptoProvider != null && this.CustomCryptoProvider.IsSupportedAlgorithm(algorithm))
    {
      if (!(this.CustomCryptoProvider.Create(algorithm) is HashAlgorithm hashAlgorithm))
        throw LogHelper.LogExceptionMessage((Exception) new InvalidOperationException(LogHelper.FormatInvariant("IDX10647: A CustomCryptoProvider was set and returned 'true' for IsSupportedAlgorithm(Algorithm: '{0}'), but Create.(algorithm, args) as '{1}' == NULL.", (object) algorithm, (object) typeof (HashAlgorithm))));
      CryptoProviderFactory._typeToAlgorithmMap[hashAlgorithm.GetType().ToString()] = algorithm;
      return hashAlgorithm;
    }
    switch (algorithm)
    {
      case "SHA256":
      case "http://www.w3.org/2001/04/xmlenc#sha256":
        return (HashAlgorithm) SHA256.Create();
      case "SHA384":
      case "http://www.w3.org/2001/04/xmldsig-more#sha384":
        return (HashAlgorithm) SHA384.Create();
      case "SHA512":
      case "http://www.w3.org/2001/04/xmlenc#sha512":
        return (HashAlgorithm) SHA512.Create();
      default:
        throw LogHelper.LogExceptionMessage((Exception) new NotSupportedException(LogHelper.FormatInvariant("IDX10640: Algorithm is not supported: '{0}'.", (object) algorithm)));
    }
  }

  public virtual KeyedHashAlgorithm CreateKeyedHashAlgorithm(byte[] keyBytes, string algorithm)
  {
    if (keyBytes == null)
      throw LogHelper.LogArgumentNullException(nameof (keyBytes));
    if (string.IsNullOrEmpty(algorithm))
      throw LogHelper.LogArgumentNullException(nameof (algorithm));
    if (this.CustomCryptoProvider != null)
    {
      if (this.CustomCryptoProvider.IsSupportedAlgorithm(algorithm, (object) keyBytes))
      {
        if (!(this.CustomCryptoProvider.Create(algorithm, (object) keyBytes) is KeyedHashAlgorithm keyedHashAlgorithm))
          throw LogHelper.LogExceptionMessage((Exception) new InvalidOperationException(LogHelper.FormatInvariant("IDX10647: A CustomCryptoProvider was set and returned 'true' for IsSupportedAlgorithm(Algorithm: '{0}'), but Create.(algorithm, args) as '{1}' == NULL.", (object) algorithm, (object) typeof (KeyedHashAlgorithm))));
        return keyedHashAlgorithm;
      }
    }
    switch (algorithm)
    {
      case "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256":
      case "HS256":
        return (KeyedHashAlgorithm) new HMACSHA256(keyBytes);
      case "http://www.w3.org/2001/04/xmldsig-more#hmac-sha384":
      case "HS384":
        return (KeyedHashAlgorithm) new HMACSHA384(keyBytes);
      case "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512":
      case "HS512":
        return (KeyedHashAlgorithm) new HMACSHA512(keyBytes);
      default:
        throw LogHelper.LogExceptionMessage((Exception) new NotSupportedException(LogHelper.FormatInvariant("IDX10666: Unable to create KeyedHashAlgorithm for algorithm '{0}'.", (object) algorithm)));
    }
  }

  private SignatureProvider CreateSignatureProvider(
    SecurityKey key,
    string algorithm,
    bool willCreateSignatures)
  {
    if (key == null)
      throw LogHelper.LogArgumentNullException(nameof (key));
    if (string.IsNullOrEmpty(algorithm))
      throw LogHelper.LogArgumentNullException(nameof (algorithm));
    SignatureProvider signatureProvider1 = (SignatureProvider) null;
    if (this.CustomCryptoProvider != null)
    {
      if (this.CustomCryptoProvider.IsSupportedAlgorithm(algorithm, (object) key, (object) willCreateSignatures))
      {
        if (!(this.CustomCryptoProvider.Create(algorithm, (object) key, (object) willCreateSignatures) is SignatureProvider signatureProvider2))
          throw LogHelper.LogExceptionMessage((Exception) new InvalidOperationException(LogHelper.FormatInvariant("IDX10646: A CustomCryptoProvider was set and returned 'true' for IsSupportedAlgorithm(Algorithm: '{0}', Key: '{1}'), but Create.(algorithm, args) as '{2}' == NULL.", (object) algorithm, (object) key, (object) typeof (SignatureProvider))));
        return signatureProvider2;
      }
    }
    string typeofProvider = (string) null;
    bool flag = true;
    switch (key)
    {
      case AsymmetricSecurityKey _:
        typeofProvider = typeof (AsymmetricSignatureProvider).ToString();
        break;
      case JsonWebKey webKey:
        try
        {
          SecurityKey key1;
          if (JsonWebKeyConverter.TryConvertToSecurityKey(webKey, out key1))
          {
            if (key1 is AsymmetricSecurityKey)
            {
              typeofProvider = typeof (AsymmetricSignatureProvider).ToString();
              break;
            }
            if (key1 is SymmetricSecurityKey)
            {
              typeofProvider = typeof (SymmetricSignatureProvider).ToString();
              flag = false;
              break;
            }
            break;
          }
          if (webKey.Kty == "RSA" || webKey.Kty == "EC")
          {
            typeofProvider = typeof (AsymmetricSignatureProvider).ToString();
            break;
          }
          if (webKey.Kty == "oct")
          {
            typeofProvider = typeof (SymmetricSignatureProvider).ToString();
            flag = false;
            break;
          }
          break;
        }
        catch (Exception ex)
        {
          throw LogHelper.LogExceptionMessage((Exception) new InvalidOperationException(LogHelper.FormatInvariant("IDX10694: JsonWebKeyConverter threw attempting to convert JsonWebKey: '{0}'. Exception: '{1}'.", (object) key, (object) ex), ex));
        }
      case SymmetricSecurityKey _:
        typeofProvider = typeof (SymmetricSignatureProvider).ToString();
        flag = false;
        break;
    }
    if (typeofProvider == null)
      throw LogHelper.LogExceptionMessage((Exception) new NotSupportedException(LogHelper.FormatInvariant("IDX10621: '{0}' supports: '{1}' of types: '{2}' or '{3}'. SecurityKey received was of type '{4}'.", (object) typeof (SymmetricSignatureProvider), (object) typeof (SecurityKey), (object) typeof (AsymmetricSecurityKey), (object) typeof (SymmetricSecurityKey), (object) key.GetType())));
    if (!this.IsSupportedAlgorithm(algorithm, key))
      throw LogHelper.LogExceptionMessage((Exception) new NotSupportedException(LogHelper.FormatInvariant("IDX10634: Unable to create the SignatureProvider.\nAlgorithm: '{0}', SecurityKey: '{1}'\n is not supported. The list of supported algorithms is available here: https://aka.ms/IdentityModel/supported-algorithms", (object) algorithm, (object) key)));
    if (this.CacheSignatureProviders)
    {
      if (this.CryptoProviderCache.TryGetSignatureProvider(key, algorithm, typeofProvider, willCreateSignatures, out signatureProvider1))
        return signatureProvider1;
      lock (CryptoProviderFactory._cacheLock)
      {
        if (this.CryptoProviderCache.TryGetSignatureProvider(key, algorithm, typeofProvider, willCreateSignatures, out signatureProvider1))
          return signatureProvider1;
        signatureProvider1 = !flag ? (SignatureProvider) new SymmetricSignatureProvider(key, algorithm, willCreateSignatures) : (SignatureProvider) new AsymmetricSignatureProvider(key, algorithm, willCreateSignatures, this);
        this.CryptoProviderCache.TryAdd(signatureProvider1);
      }
    }
    else
      signatureProvider1 = !flag ? (SignatureProvider) new SymmetricSignatureProvider(key, algorithm, willCreateSignatures) : (SignatureProvider) new AsymmetricSignatureProvider(key, algorithm, willCreateSignatures);
    return signatureProvider1;
  }

  public virtual bool IsSupportedAlgorithm(string algorithm)
  {
    return this.CustomCryptoProvider != null && this.CustomCryptoProvider.IsSupportedAlgorithm(algorithm) || SupportedAlgorithms.IsSupportedHashAlgorithm(algorithm);
  }

  public virtual bool IsSupportedAlgorithm(string algorithm, SecurityKey key)
  {
    if (this.CustomCryptoProvider != null)
    {
      if (this.CustomCryptoProvider.IsSupportedAlgorithm(algorithm, (object) key))
        return true;
    }
    return SupportedAlgorithms.IsSupportedAlgorithm(algorithm, !(key is JsonWebKey jsonWebKey) || jsonWebKey.ConvertedSecurityKey == null ? key : jsonWebKey.ConvertedSecurityKey);
  }

  public virtual void ReleaseHashAlgorithm(HashAlgorithm hashAlgorithm)
  {
    if (hashAlgorithm == null)
      throw LogHelper.LogArgumentNullException(nameof (hashAlgorithm));
    string algorithm;
    if (this.CustomCryptoProvider != null && CryptoProviderFactory._typeToAlgorithmMap.TryGetValue(hashAlgorithm.GetType().ToString(), out algorithm) && this.CustomCryptoProvider.IsSupportedAlgorithm(algorithm))
      this.CustomCryptoProvider.Release((object) hashAlgorithm);
    else
      hashAlgorithm.Dispose();
  }

  public virtual void ReleaseKeyWrapProvider(KeyWrapProvider provider)
  {
    if (provider == null)
      throw LogHelper.LogArgumentNullException(nameof (provider));
    if (this.CustomCryptoProvider != null && this.CustomCryptoProvider.IsSupportedAlgorithm(provider.Algorithm))
      this.CustomCryptoProvider.Release((object) provider);
    else
      provider.Dispose();
  }

  public virtual void ReleaseRsaKeyWrapProvider(RsaKeyWrapProvider provider)
  {
    if (provider == null)
      throw LogHelper.LogArgumentNullException(nameof (provider));
    if (this.CustomCryptoProvider != null && this.CustomCryptoProvider.IsSupportedAlgorithm(provider.Algorithm))
      this.CustomCryptoProvider.Release((object) provider);
    else
      provider.Dispose();
  }

  public virtual void ReleaseSignatureProvider(SignatureProvider signatureProvider)
  {
    if (signatureProvider == null)
      throw LogHelper.LogArgumentNullException(nameof (signatureProvider));
    if (this.CustomCryptoProvider != null && this.CustomCryptoProvider.IsSupportedAlgorithm(signatureProvider.Algorithm))
    {
      this.CustomCryptoProvider.Release((object) signatureProvider);
    }
    else
    {
      if (signatureProvider.CryptoProviderCache != null)
        return;
      signatureProvider.Dispose();
    }
  }
}
