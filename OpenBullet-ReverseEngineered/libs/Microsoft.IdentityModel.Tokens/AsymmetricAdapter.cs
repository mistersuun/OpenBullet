// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.AsymmetricAdapter
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.Security.Cryptography;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

internal class AsymmetricAdapter : IDisposable
{
  private bool _useRSAOeapPadding;
  private RSAEncryptionPadding _rsaEncryptionPadding;
  private bool _disposeCryptoOperators;
  private bool _disposed;
  private SignDelegate SignatureFunction;
  private VerifyDelegate VerifyFunction;
  private object _signRsaLock = new object();
  private object _signEcdsaLock = new object();
  private object _verifyRsaLock = new object();
  private object _verifyEcdsaLock = new object();

  internal AsymmetricAdapter(
    SecurityKey key,
    string algorithm,
    HashAlgorithm hashAlgorithm,
    HashAlgorithmName hashAlgorithmName,
    bool requirePrivateKey)
    : this(key, algorithm, hashAlgorithm, requirePrivateKey)
  {
    this.HashAlgorithmName = hashAlgorithmName;
  }

  internal AsymmetricAdapter(SecurityKey key, string algorithm, bool requirePrivateKey)
    : this(key, algorithm, (HashAlgorithm) null, requirePrivateKey)
  {
  }

  internal AsymmetricAdapter(
    SecurityKey key,
    string algorithm,
    HashAlgorithm hashAlgorithm,
    bool requirePrivateKey)
  {
    this.HashAlgorithm = hashAlgorithm;
    switch (key)
    {
      case RsaSecurityKey rsaSecurityKey2:
        this.InitializeUsingRsaSecurityKey(rsaSecurityKey2, algorithm);
        break;
      case X509SecurityKey x509SecurityKey2:
        this.InitializeUsingX509SecurityKey(x509SecurityKey2, algorithm, requirePrivateKey);
        break;
      case JsonWebKey webKey:
        SecurityKey key1;
        if (!JsonWebKeyConverter.TryConvertToSecurityKey(webKey, out key1))
          break;
        switch (key1)
        {
          case RsaSecurityKey rsaSecurityKey1:
            this.InitializeUsingRsaSecurityKey(rsaSecurityKey1, algorithm);
            return;
          case X509SecurityKey x509SecurityKey1:
            this.InitializeUsingX509SecurityKey(x509SecurityKey1, algorithm, requirePrivateKey);
            return;
          case ECDsaSecurityKey ecdsaSecurityKey:
            this.InitializeUsingEcdsaSecurityKey(ecdsaSecurityKey);
            return;
          default:
            throw LogHelper.LogExceptionMessage((Exception) new NotSupportedException(LogHelper.FormatInvariant("IDX10684: Unable to convert the JsonWebKey to an AsymmetricSecurityKey. Algorithm: '{0}', Key: '{1}'.", (object) algorithm, (object) key)));
        }
      case ECDsaSecurityKey ecDsaSecurityKey:
        this.ECDsaSecurityKey = ecDsaSecurityKey;
        this.SignatureFunction = new SignDelegate(this.SignWithECDsa);
        this.VerifyFunction = new VerifyDelegate(this.VerifyWithECDsa);
        break;
      default:
        throw LogHelper.LogExceptionMessage((Exception) new NotSupportedException(LogHelper.FormatInvariant("IDX10684: Unable to convert the JsonWebKey to an AsymmetricSecurityKey. Algorithm: '{0}', Key: '{1}'.", (object) algorithm, (object) key)));
    }
  }

  private void InitializeUsingRsaSecurityKey(RsaSecurityKey rsaSecurityKey, string algorithm)
  {
    if (rsaSecurityKey.Rsa != null)
    {
      this.InitializeUsingRsa(rsaSecurityKey.Rsa, algorithm);
    }
    else
    {
      RSA rsa = RSA.Create();
      rsa.ImportParameters(rsaSecurityKey.Parameters);
      this.InitializeUsingRsa(rsa, algorithm);
      this._disposeCryptoOperators = true;
    }
  }

  private void InitializeUsingX509SecurityKey(
    X509SecurityKey x509SecurityKey,
    string algorithm,
    bool requirePrivateKey)
  {
    if (requirePrivateKey)
      this.InitializeUsingRsa(x509SecurityKey.PrivateKey as RSA, algorithm);
    else
      this.InitializeUsingRsa(x509SecurityKey.PublicKey as RSA, algorithm);
  }

  private void InitializeUsingEcdsaSecurityKey(ECDsaSecurityKey ecdsaSecurityKey)
  {
    this.ECDsaSecurityKey = ecdsaSecurityKey;
    this.SignatureFunction = new SignDelegate(this.SignWithECDsa);
    this.VerifyFunction = new VerifyDelegate(this.VerifyWithECDsa);
  }

  internal byte[] Decrypt(byte[] data)
  {
    return this.RsaCryptoServiceProviderProxy != null ? this.RsaCryptoServiceProviderProxy.Decrypt(data, this._useRSAOeapPadding) : this.RSA.Decrypt(data, this._rsaEncryptionPadding);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (this._disposed)
      return;
    this._disposed = true;
    if (!disposing || !this._disposeCryptoOperators)
      return;
    if (this.ECDsaSecurityKey != null)
      this.ECDsaSecurityKey.ECDsa.Dispose();
    if (this.RsaCryptoServiceProviderProxy != null)
      this.RsaCryptoServiceProviderProxy.Dispose();
    if (this.RSA == null)
      return;
    this.RSA.Dispose();
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  private ECDsaSecurityKey ECDsaSecurityKey { get; set; }

  internal byte[] Encrypt(byte[] data)
  {
    return this.RsaCryptoServiceProviderProxy != null ? this.RsaCryptoServiceProviderProxy.Encrypt(data, this._useRSAOeapPadding) : this.RSA.Encrypt(data, this._rsaEncryptionPadding);
  }

  private HashAlgorithm HashAlgorithm { get; set; }

  private HashAlgorithmName HashAlgorithmName { get; set; }

  private RSASignaturePadding RSASignaturePadding { get; set; }

  private void InitializeUsingRsa(RSA rsa, string algorithm)
  {
    this.RSASignaturePadding = algorithm.Equals("PS256", StringComparison.Ordinal) || algorithm.Equals("http://www.w3.org/2007/05/xmldsig-more#sha256-rsa-MGF1", StringComparison.Ordinal) || algorithm.Equals("PS384", StringComparison.Ordinal) || algorithm.Equals("http://www.w3.org/2007/05/xmldsig-more#sha384-rsa-MGF1", StringComparison.Ordinal) || algorithm.Equals("PS512", StringComparison.Ordinal) || algorithm.Equals("http://www.w3.org/2007/05/xmldsig-more#sha512-rsa-MGF1", StringComparison.Ordinal) ? RSASignaturePadding.Pss : RSASignaturePadding.Pkcs1;
    this._useRSAOeapPadding = algorithm.Equals("RSA-OAEP", StringComparison.Ordinal) || algorithm.Equals("http://www.w3.org/2001/04/xmlenc#rsa-oaep", StringComparison.Ordinal);
    if (rsa is RSACryptoServiceProvider rsa1)
    {
      this.RsaCryptoServiceProviderProxy = new RSACryptoServiceProviderProxy(rsa1);
      this.SignatureFunction = new SignDelegate(this.SignWithRsaCryptoServiceProviderProxy);
      this.VerifyFunction = new VerifyDelegate(this.VerifyWithRsaCryptoServiceProviderProxy);
      this._disposeCryptoOperators = true;
    }
    else
    {
      this._rsaEncryptionPadding = algorithm.Equals("RSA-OAEP", StringComparison.Ordinal) || algorithm.Equals("http://www.w3.org/2001/04/xmlenc#rsa-oaep", StringComparison.Ordinal) ? RSAEncryptionPadding.OaepSHA1 : RSAEncryptionPadding.Pkcs1;
      this.RSA = rsa;
      this.SignatureFunction = new SignDelegate(this.SignWithRsa);
      this.VerifyFunction = new VerifyDelegate(this.VerifyWithRsa);
    }
  }

  private RSA RSA { get; set; }

  private RSACryptoServiceProviderProxy RsaCryptoServiceProviderProxy { get; set; }

  internal byte[] Sign(byte[] bytes)
  {
    if (this.SignatureFunction != null)
      return this.SignatureFunction(bytes);
    throw LogHelper.LogExceptionMessage((Exception) new CryptographicException("IDX10685: Unable to Sign, Internal SignFunction is not available."));
  }

  private byte[] SignWithECDsa(byte[] bytes)
  {
    lock (this._signEcdsaLock)
      return this.ECDsaSecurityKey.ECDsa.SignHash(this.HashAlgorithm.ComputeHash(bytes));
  }

  private byte[] SignWithRsa(byte[] bytes)
  {
    lock (this._signRsaLock)
      return this.RSA.SignHash(this.HashAlgorithm.ComputeHash(bytes), this.HashAlgorithmName, this.RSASignaturePadding);
  }

  internal byte[] SignWithRsaCryptoServiceProviderProxy(byte[] bytes)
  {
    return this.RsaCryptoServiceProviderProxy.SignData(bytes, (object) this.HashAlgorithm);
  }

  internal bool Verify(byte[] bytes, byte[] signature)
  {
    if (this.VerifyFunction != null)
      return this.VerifyFunction(bytes, signature);
    throw LogHelper.LogExceptionMessage((Exception) new NotSupportedException("IDX10686: Unable to Verify, Internal VerifyFunction is not available."));
  }

  private bool VerifyWithECDsa(byte[] bytes, byte[] signature)
  {
    lock (this._verifyEcdsaLock)
      return this.ECDsaSecurityKey.ECDsa.VerifyHash(this.HashAlgorithm.ComputeHash(bytes), signature);
  }

  private bool VerifyWithRsa(byte[] bytes, byte[] signature)
  {
    lock (this._verifyRsaLock)
      return this.RSA.VerifyHash(this.HashAlgorithm.ComputeHash(bytes), signature, this.HashAlgorithmName, this.RSASignaturePadding);
  }

  private bool VerifyWithRsaCryptoServiceProviderProxy(byte[] bytes, byte[] signature)
  {
    lock (this._verifyRsaLock)
      return this.RsaCryptoServiceProviderProxy.VerifyData(bytes, (object) this.HashAlgorithm, signature);
  }
}
