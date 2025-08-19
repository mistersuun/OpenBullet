// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.SymmetricSignatureProvider
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.Security.Cryptography;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class SymmetricSignatureProvider : SignatureProvider
{
  private bool _disposed;
  private KeyedHashAlgorithm _keyedHash;
  private object _signLock = new object();
  private object _verifyLock = new object();
  public static readonly int DefaultMinimumSymmetricKeySizeInBits = 128 /*0x80*/;
  private int _minimumSymmetricKeySizeInBits = SymmetricSignatureProvider.DefaultMinimumSymmetricKeySizeInBits;

  public SymmetricSignatureProvider(SecurityKey key, string algorithm)
    : this(key, algorithm, true)
  {
  }

  public SymmetricSignatureProvider(SecurityKey key, string algorithm, bool willCreateSignatures)
    : base(key, algorithm)
  {
    if (!key.CryptoProviderFactory.IsSupportedAlgorithm(algorithm, key))
      throw LogHelper.LogExceptionMessage((Exception) new NotSupportedException(LogHelper.FormatInvariant("IDX10634: Unable to create the SignatureProvider.\nAlgorithm: '{0}', SecurityKey: '{1}'\n is not supported. The list of supported algorithms is available here: https://aka.ms/IdentityModel/supported-algorithms", (object) (algorithm ?? "null"), (object) key)));
    if (key.KeySize < this.MinimumSymmetricKeySizeInBits)
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException("KeySize", LogHelper.FormatInvariant("IDX10603: Decryption failed. Keys tried: '{0}'.\nExceptions caught:\n '{1}'.\ntoken: '{2}'", (object) (algorithm ?? "null"), (object) this.MinimumSymmetricKeySizeInBits, (object) key.KeySize)));
    this.WillCreateSignatures = willCreateSignatures;
  }

  public int MinimumSymmetricKeySizeInBits
  {
    get => this._minimumSymmetricKeySizeInBits;
    set
    {
      this._minimumSymmetricKeySizeInBits = value >= SymmetricSignatureProvider.DefaultMinimumSymmetricKeySizeInBits ? value : throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException(nameof (value), LogHelper.FormatInvariant("IDX10628: Cannot set the MinimumSymmetricKeySizeInBits to less than '{0}'.", (object) SymmetricSignatureProvider.DefaultMinimumSymmetricKeySizeInBits)));
    }
  }

  protected virtual byte[] GetKeyBytes(SecurityKey key)
  {
    switch (key)
    {
      case null:
        throw LogHelper.LogArgumentNullException(nameof (key));
      case SymmetricSecurityKey symmetricSecurityKey:
        return symmetricSecurityKey.Key;
      case JsonWebKey jsonWebKey:
        if (jsonWebKey.K != null && jsonWebKey.Kty == "oct")
          return Base64UrlEncoder.DecodeBytes(jsonWebKey.K);
        break;
    }
    throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10667: Unable to obtain required byte array for KeyHashAlgorithm from SecurityKey: '{0}'.", (object) key)));
  }

  protected virtual KeyedHashAlgorithm GetKeyedHashAlgorithm(byte[] keyBytes, string algorithm)
  {
    if (this._keyedHash == null)
    {
      try
      {
        this._keyedHash = this.Key.CryptoProviderFactory.CreateKeyedHashAlgorithm(keyBytes, algorithm);
      }
      catch (Exception ex)
      {
        throw LogHelper.LogExceptionMessage((Exception) new InvalidOperationException(LogHelper.FormatInvariant("IDX10677: GetKeyedHashAlgorithm threw, key: {0}, algorithm {1}.", (object) this.Key, (object) (algorithm ?? "null")), ex));
      }
    }
    return this._keyedHash;
  }

  private KeyedHashAlgorithm KeyedHashAlgorithm
  {
    get
    {
      if (this._keyedHash == null)
      {
        try
        {
          this._keyedHash = this.GetKeyedHashAlgorithm(this.GetKeyBytes(this.Key), this.Algorithm);
        }
        catch (Exception ex)
        {
          throw LogHelper.LogExceptionMessage((Exception) new InvalidOperationException(LogHelper.FormatInvariant("IDX10677: GetKeyedHashAlgorithm threw, key: {0}, algorithm {1}.", (object) this.Key, (object) (this.Algorithm ?? "null")), ex));
        }
      }
      return this._keyedHash;
    }
  }

  public override byte[] Sign(byte[] input)
  {
    if (input == null || input.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (input));
    if (this._disposed)
    {
      this.CryptoProviderCache?.TryRemove((SignatureProvider) this);
      throw LogHelper.LogExceptionMessage((Exception) new ObjectDisposedException(typeof (SymmetricSignatureProvider).ToString()));
    }
    LogHelper.LogInformation("IDX10642: Creating signature using the input: '{0}'.", (object) input);
    try
    {
      lock (this._signLock)
        return this.KeyedHashAlgorithm.ComputeHash(input);
    }
    catch
    {
      this.CryptoProviderCache?.TryRemove((SignatureProvider) this);
      throw;
    }
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
      throw LogHelper.LogExceptionMessage((Exception) new ObjectDisposedException(typeof (SymmetricSignatureProvider).ToString()));
    }
    LogHelper.LogInformation("IDX10643: Comparing the signature created over the input with the token signature: '{0}'.", (object) input);
    try
    {
      lock (this._verifyLock)
        return Utility.AreEqual(signature, this.KeyedHashAlgorithm.ComputeHash(input));
    }
    catch
    {
      this.CryptoProviderCache?.TryRemove((SignatureProvider) this);
      throw;
    }
  }

  public bool Verify(byte[] input, byte[] signature, int length)
  {
    if (input == null || input.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (input));
    if (signature == null || signature.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (signature));
    if (length < 1)
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10655: 'length' must be greater than 1: '{0}'", (object) length)));
    if (this._disposed)
    {
      this.CryptoProviderCache?.TryRemove((SignatureProvider) this);
      throw LogHelper.LogExceptionMessage((Exception) new ObjectDisposedException(typeof (SymmetricSignatureProvider).ToString()));
    }
    LogHelper.LogInformation("IDX10643: Comparing the signature created over the input with the token signature: '{0}'.", (object) input);
    try
    {
      lock (this._verifyLock)
        return Utility.AreEqual(signature, this.KeyedHashAlgorithm.ComputeHash(input), length);
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
    this.CryptoProviderCache?.TryRemove((SignatureProvider) this);
    if (!disposing || this._keyedHash == null)
      return;
    this._keyedHash.Dispose();
    this._keyedHash = (KeyedHashAlgorithm) null;
  }
}
