// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.AuthenticatedEncryptionProvider
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.IO;
using System.Security.Cryptography;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class AuthenticatedEncryptionProvider
{
  private AuthenticatedEncryptionProvider.AuthenticatedKeys _authenticatedkeys;
  private string _hmacAlgorithm;
  private SymmetricSignatureProvider _symmetricSignatureProvider;

  public AuthenticatedEncryptionProvider(SecurityKey key, string algorithm)
  {
    if (key == null)
      throw LogHelper.LogArgumentNullException(nameof (key));
    if (string.IsNullOrWhiteSpace(algorithm))
      throw LogHelper.LogArgumentNullException(nameof (algorithm));
    if (!this.IsSupportedAlgorithm(key, algorithm))
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10668: Unable to create '{0}', algorithm '{1}'; key: '{2}' is not supported.", (object) this.GetType(), (object) algorithm, (object) key)));
    this.ValidateKeySize(key, algorithm);
    this._authenticatedkeys = this.GetAlgorithmParameters(key, algorithm);
    this._hmacAlgorithm = AuthenticatedEncryptionProvider.GetHmacAlgorithm(algorithm);
    this._symmetricSignatureProvider = key.CryptoProviderFactory.CreateForSigning((SecurityKey) this._authenticatedkeys.HmacKey, this._hmacAlgorithm) as SymmetricSignatureProvider;
    if (this._symmetricSignatureProvider == null)
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10649: Failed to create a SymmetricSignatureProvider for the algorithm '{0}'.", (object) this.Algorithm)));
    this.Key = key;
    this.Algorithm = algorithm;
  }

  public string Algorithm { get; private set; }

  public string Context { get; set; }

  public SecurityKey Key { get; private set; }

  public virtual AuthenticatedEncryptionResult Encrypt(byte[] plaintext, byte[] authenticatedData)
  {
    return this.Encrypt(plaintext, authenticatedData, (byte[]) null);
  }

  public virtual AuthenticatedEncryptionResult Encrypt(
    byte[] plaintext,
    byte[] authenticatedData,
    byte[] iv)
  {
    if (plaintext == null || plaintext.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (plaintext));
    if (authenticatedData == null || authenticatedData.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (authenticatedData));
    Aes aes = Aes.Create();
    aes.Mode = CipherMode.CBC;
    aes.Padding = PaddingMode.PKCS7;
    aes.Key = this._authenticatedkeys.AesKey.Key;
    if (iv != null)
      aes.IV = iv;
    byte[] numArray1;
    try
    {
      numArray1 = AuthenticatedEncryptionProvider.Transform(aes.CreateEncryptor(), plaintext, 0, plaintext.Length);
    }
    catch (Exception ex)
    {
      throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenEncryptionFailedException(LogHelper.FormatInvariant("IDX10654: Decryption failed. Cryptographic operation exception: '{0}'.", (object) ex)));
    }
    byte[] bigEndian = Utility.ConvertToBigEndian((long) (authenticatedData.Length * 8));
    byte[] numArray2 = new byte[authenticatedData.Length + aes.IV.Length + numArray1.Length + bigEndian.Length];
    Array.Copy((Array) authenticatedData, 0, (Array) numArray2, 0, authenticatedData.Length);
    Array.Copy((Array) aes.IV, 0, (Array) numArray2, authenticatedData.Length, aes.IV.Length);
    Array.Copy((Array) numArray1, 0, (Array) numArray2, authenticatedData.Length + aes.IV.Length, numArray1.Length);
    Array.Copy((Array) bigEndian, 0, (Array) numArray2, authenticatedData.Length + aes.IV.Length + numArray1.Length, bigEndian.Length);
    byte[] sourceArray = this._symmetricSignatureProvider.Sign(numArray2);
    byte[] authenticationTag = new byte[this._authenticatedkeys.HmacKey.Key.Length];
    byte[] destinationArray = authenticationTag;
    int length = authenticationTag.Length;
    Array.Copy((Array) sourceArray, (Array) destinationArray, length);
    return new AuthenticatedEncryptionResult(this.Key, numArray1, aes.IV, authenticationTag);
  }

  public virtual byte[] Decrypt(
    byte[] ciphertext,
    byte[] authenticatedData,
    byte[] iv,
    byte[] authenticationTag)
  {
    if (ciphertext == null || ciphertext.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (ciphertext));
    if (authenticatedData == null || authenticatedData.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (authenticatedData));
    if (iv == null || iv.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (iv));
    byte[] sourceArray = authenticationTag != null && authenticationTag.Length != 0 ? Utility.ConvertToBigEndian((long) (authenticatedData.Length * 8)) : throw LogHelper.LogArgumentNullException(nameof (authenticationTag));
    byte[] numArray = new byte[authenticatedData.Length + iv.Length + ciphertext.Length + sourceArray.Length];
    Array.Copy((Array) authenticatedData, 0, (Array) numArray, 0, authenticatedData.Length);
    Array.Copy((Array) iv, 0, (Array) numArray, authenticatedData.Length, iv.Length);
    Array.Copy((Array) ciphertext, 0, (Array) numArray, authenticatedData.Length + iv.Length, ciphertext.Length);
    Array.Copy((Array) sourceArray, 0, (Array) numArray, authenticatedData.Length + iv.Length + ciphertext.Length, sourceArray.Length);
    if (!this._symmetricSignatureProvider.Verify(numArray, authenticationTag, this._authenticatedkeys.HmacKey.Key.Length))
      throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenDecryptionFailedException(LogHelper.FormatInvariant("IDX10650: Failed to verify ciphertext with aad '{0}'; iv '{1}'; and authenticationTag '{2}'.", (object) Base64UrlEncoder.Encode(authenticatedData), (object) Base64UrlEncoder.Encode(iv), (object) Base64UrlEncoder.Encode(authenticationTag))));
    Aes aes = Aes.Create();
    aes.Mode = CipherMode.CBC;
    aes.Padding = PaddingMode.PKCS7;
    aes.Key = this._authenticatedkeys.AesKey.Key;
    aes.IV = iv;
    try
    {
      return AuthenticatedEncryptionProvider.Transform(aes.CreateDecryptor(), ciphertext, 0, ciphertext.Length);
    }
    catch (Exception ex)
    {
      throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenDecryptionFailedException(LogHelper.FormatInvariant("IDX10654: Decryption failed. Cryptographic operation exception: '{0}'.", (object) ex)));
    }
  }

  protected virtual bool IsSupportedAlgorithm(SecurityKey key, string algorithm)
  {
    return SupportedAlgorithms.IsSupportedAuthenticatedEncryptionAlgorithm(algorithm, key);
  }

  private AuthenticatedEncryptionProvider.AuthenticatedKeys GetAlgorithmParameters(
    SecurityKey key,
    string algorithm)
  {
    int length;
    if (algorithm.Equals("A256CBC-HS512", StringComparison.Ordinal))
      length = 32 /*0x20*/;
    else if (algorithm.Equals("A192CBC-HS384", StringComparison.Ordinal))
      length = 24;
    else if (algorithm.Equals("A128CBC-HS256", StringComparison.Ordinal))
      length = 16 /*0x10*/;
    else
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10668: Unable to create '{0}', algorithm '{1}'; key: '{2}' is not supported.", (object) this.GetType(), (object) algorithm, (object) key)));
    byte[] keyBytes = this.GetKeyBytes(key);
    byte[] numArray1 = new byte[length];
    byte[] numArray2 = new byte[length];
    Array.Copy((Array) keyBytes, length, (Array) numArray1, 0, length);
    Array.Copy((Array) keyBytes, (Array) numArray2, length);
    return new AuthenticatedEncryptionProvider.AuthenticatedKeys()
    {
      AesKey = new SymmetricSecurityKey(numArray1),
      HmacKey = new SymmetricSecurityKey(numArray2)
    };
  }

  private static string GetHmacAlgorithm(string algorithm)
  {
    if ("A128CBC-HS256".Equals(algorithm, StringComparison.Ordinal))
      return "HS256";
    if ("A192CBC-HS384".Equals(algorithm, StringComparison.Ordinal))
      return "HS384";
    if ("A256CBC-HS512".Equals(algorithm, StringComparison.Ordinal))
      return "HS512";
    throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10652: The algorithm '{0}' is not supported.", (object) algorithm), nameof (algorithm)));
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

  internal static byte[] Transform(
    ICryptoTransform transform,
    byte[] input,
    int inputOffset,
    int inputLength)
  {
    if (transform.CanTransformMultipleBlocks)
      return transform.TransformFinalBlock(input, inputOffset, inputLength);
    using (MemoryStream memoryStream = new MemoryStream())
    {
      using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, transform, CryptoStreamMode.Write))
      {
        cryptoStream.Write(input, inputOffset, inputLength);
        cryptoStream.FlushFinalBlock();
        return memoryStream.ToArray();
      }
    }
  }

  protected virtual void ValidateKeySize(SecurityKey key, string algorithm)
  {
    if (key == null)
      throw LogHelper.LogArgumentNullException(nameof (key));
    if (string.IsNullOrEmpty(algorithm))
      throw LogHelper.LogArgumentNullException(nameof (algorithm));
    if ("A128CBC-HS256".Equals(algorithm, StringComparison.Ordinal))
    {
      if (key.KeySize < 256 /*0x0100*/)
        throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException("key.KeySize", LogHelper.FormatInvariant("IDX10653: The encryption algorithm '{0}' requires a key size of at least '{1}' bits. Key '{2}', is of size: '{3}'.", (object) "A128CBC-HS256", (object) 256 /*0x0100*/, (object) key.KeyId, (object) key.KeySize)));
    }
    else if ("A192CBC-HS384".Equals(algorithm, StringComparison.Ordinal))
    {
      if (key.KeySize < 384)
        throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException("key.KeySize", LogHelper.FormatInvariant("IDX10653: The encryption algorithm '{0}' requires a key size of at least '{1}' bits. Key '{2}', is of size: '{3}'.", (object) "A192CBC-HS384", (object) 384, (object) key.KeyId, (object) key.KeySize)));
    }
    else if ("A256CBC-HS512".Equals(algorithm, StringComparison.Ordinal))
    {
      if (key.KeySize < 512 /*0x0200*/)
        throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException("key.KeySize", LogHelper.FormatInvariant("IDX10653: The encryption algorithm '{0}' requires a key size of at least '{1}' bits. Key '{2}', is of size: '{3}'.", (object) "A256CBC-HS512", (object) 512 /*0x0200*/, (object) key.KeyId, (object) key.KeySize)));
    }
    else
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10652: The algorithm '{0}' is not supported.", (object) algorithm)));
  }

  private struct AuthenticatedKeys
  {
    public SymmetricSecurityKey AesKey;
    public SymmetricSecurityKey HmacKey;
  }
}
