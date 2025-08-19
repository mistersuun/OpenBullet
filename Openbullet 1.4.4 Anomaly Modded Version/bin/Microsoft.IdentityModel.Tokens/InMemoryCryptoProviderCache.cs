// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.InMemoryCryptoProviderCache
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Concurrent;
using System.Globalization;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class InMemoryCryptoProviderCache : CryptoProviderCache
{
  private ConcurrentDictionary<string, SignatureProvider> _signingSignatureProviders = new ConcurrentDictionary<string, SignatureProvider>();
  private ConcurrentDictionary<string, SignatureProvider> _verifyingSignatureProviders = new ConcurrentDictionary<string, SignatureProvider>();

  protected override string GetCacheKey(SignatureProvider signatureProvider)
  {
    if (signatureProvider == null)
      throw LogHelper.LogArgumentNullException(nameof (signatureProvider));
    return this.GetCacheKeyPrivate(signatureProvider.Key, signatureProvider.Algorithm, signatureProvider.GetType().ToString());
  }

  protected override string GetCacheKey(
    SecurityKey securityKey,
    string algorithm,
    string typeofProvider)
  {
    if (securityKey == null)
      throw LogHelper.LogArgumentNullException(nameof (securityKey));
    if (string.IsNullOrEmpty(algorithm))
      throw LogHelper.LogArgumentNullException(nameof (algorithm));
    if (string.IsNullOrEmpty(typeofProvider))
      throw LogHelper.LogArgumentNullException(nameof (typeofProvider));
    return this.GetCacheKeyPrivate(securityKey, algorithm, typeofProvider);
  }

  private string GetCacheKeyPrivate(
    SecurityKey securityKey,
    string algorithm,
    string typeofProvider)
  {
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}-{2}-{3}", (object) securityKey.GetType(), string.IsNullOrEmpty(securityKey.KeyId) ? (object) securityKey.InternalId : (object) securityKey.KeyId, (object) algorithm, (object) typeofProvider);
  }

  public override bool TryAdd(SignatureProvider signatureProvider)
  {
    string key = signatureProvider != null ? this.GetCacheKey(signatureProvider) : throw LogHelper.LogArgumentNullException(nameof (signatureProvider));
    if (signatureProvider.WillCreateSignatures)
    {
      if (this._signingSignatureProviders.TryAdd(key, signatureProvider))
      {
        signatureProvider.CryptoProviderCache = (CryptoProviderCache) this;
        return true;
      }
    }
    else if (this._verifyingSignatureProviders.TryAdd(key, signatureProvider))
    {
      signatureProvider.CryptoProviderCache = (CryptoProviderCache) this;
      return true;
    }
    return false;
  }

  public override bool TryGetSignatureProvider(
    SecurityKey securityKey,
    string algorithm,
    string typeofProvider,
    bool willCreateSignatures,
    out SignatureProvider signatureProvider)
  {
    if (securityKey == null)
      throw LogHelper.LogArgumentNullException(nameof (securityKey));
    if (string.IsNullOrEmpty(algorithm))
      throw LogHelper.LogArgumentNullException(nameof (algorithm));
    if (string.IsNullOrEmpty(typeofProvider))
      throw LogHelper.LogArgumentNullException(nameof (typeofProvider));
    string cacheKeyPrivate = this.GetCacheKeyPrivate(securityKey, algorithm, typeofProvider);
    return willCreateSignatures ? this._signingSignatureProviders.TryGetValue(cacheKeyPrivate, out signatureProvider) : this._verifyingSignatureProviders.TryGetValue(cacheKeyPrivate, out signatureProvider);
  }

  public override bool TryRemove(SignatureProvider signatureProvider)
  {
    if (signatureProvider == null)
      throw LogHelper.LogArgumentNullException(nameof (signatureProvider));
    if (signatureProvider.CryptoProviderCache != this)
      return false;
    string cacheKey = this.GetCacheKey(signatureProvider);
    if (signatureProvider.WillCreateSignatures)
    {
      SignatureProvider signatureProvider1;
      if (this._signingSignatureProviders.TryRemove(cacheKey, out signatureProvider1))
      {
        signatureProvider1.CryptoProviderCache = (CryptoProviderCache) null;
        return true;
      }
    }
    else
    {
      SignatureProvider signatureProvider2;
      if (this._verifyingSignatureProviders.TryRemove(cacheKey, out signatureProvider2))
      {
        signatureProvider2.CryptoProviderCache = (CryptoProviderCache) null;
        return true;
      }
    }
    return false;
  }
}
