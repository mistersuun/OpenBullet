// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.SymmetricKeyWrapProvider
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.Security.Cryptography;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class SymmetricKeyWrapProvider : KeyWrapProvider
{
  private static readonly byte[] _defaultIV = new byte[8]
  {
    (byte) 166,
    (byte) 166,
    (byte) 166,
    (byte) 166,
    (byte) 166,
    (byte) 166,
    (byte) 166,
    (byte) 166
  };
  private static readonly int _blockSizeInBits = 64 /*0x40*/;
  private static readonly int _blockSizeInBytes = SymmetricKeyWrapProvider._blockSizeInBits >> 3;
  private static object _encryptorLock = new object();
  private static object _decryptorLock = new object();
  private SymmetricAlgorithm _symmetricAlgorithm;
  private ICryptoTransform _symmetricAlgorithmEncryptor;
  private ICryptoTransform _symmetricAlgorithmDecryptor;
  private bool _disposed;

  public SymmetricKeyWrapProvider(SecurityKey key, string algorithm)
  {
    if (key == null)
      throw LogHelper.LogArgumentNullException(nameof (key));
    if (string.IsNullOrEmpty(algorithm))
      throw LogHelper.LogArgumentNullException(nameof (algorithm));
    this.Algorithm = this.IsSupportedAlgorithm(key, algorithm) ? algorithm : throw LogHelper.LogExceptionMessage((Exception) new NotSupportedException(LogHelper.FormatInvariant("IDX10661: Unable to create the KeyWrapProvider.\nKeyWrapAlgorithm: '{0}', SecurityKey: '{1}'\n is not supported.", (object) algorithm, (object) key)));
    this.Key = key;
    this._symmetricAlgorithm = this.GetSymmetricAlgorithm(key, algorithm);
    if (this._symmetricAlgorithm == null)
      throw LogHelper.LogExceptionMessage((Exception) new InvalidOperationException(LogHelper.FormatInvariant("IDX10669: Failed to create symmetric algorithm.")));
  }

  public override string Algorithm { get; }

  public override string Context { get; set; }

  public override SecurityKey Key { get; }

  protected override void Dispose(bool disposing)
  {
    if (this._disposed || !disposing)
      return;
    if (this._symmetricAlgorithm != null)
    {
      this._symmetricAlgorithm.Dispose();
      this._symmetricAlgorithm = (SymmetricAlgorithm) null;
    }
    this._disposed = true;
  }

  private static byte[] GetBytes(ulong i)
  {
    byte[] bytes = BitConverter.GetBytes(i);
    if (BitConverter.IsLittleEndian)
      Array.Reverse((Array) bytes);
    return bytes;
  }

  protected virtual SymmetricAlgorithm GetSymmetricAlgorithm(SecurityKey key, string algorithm)
  {
    byte[] key1 = (byte[]) null;
    if (key is SymmetricSecurityKey symmetricSecurityKey)
      key1 = symmetricSecurityKey.Key;
    else if (key is JsonWebKey jsonWebKey && jsonWebKey.K != null && jsonWebKey.Kty == "oct")
      key1 = Base64UrlEncoder.DecodeBytes(jsonWebKey.K);
    if (key1 == null)
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10657: The SecurityKey provided for the symmetric key wrap algorithm cannot be converted to byte array. Type is: '{0}'.", (object) key.GetType())));
    this.ValidateKeySize(key1, algorithm);
    try
    {
      Aes symmetricAlgorithm = Aes.Create();
      symmetricAlgorithm.Mode = CipherMode.ECB;
      symmetricAlgorithm.Padding = PaddingMode.None;
      symmetricAlgorithm.KeySize = key1.Length * 8;
      symmetricAlgorithm.Key = key1;
      byte[] byteArray = new byte[symmetricAlgorithm.BlockSize >> 3];
      Utility.Zero(byteArray);
      symmetricAlgorithm.IV = byteArray;
      return (SymmetricAlgorithm) symmetricAlgorithm;
    }
    catch (Exception ex)
    {
      throw LogHelper.LogExceptionMessage((Exception) new InvalidOperationException(LogHelper.FormatInvariant("IDX10663: Failed to create symmetric algorithm with SecurityKey: '{0}', KeyWrapAlgorithm: '{1}'.", (object) key, (object) algorithm), ex));
    }
  }

  protected virtual bool IsSupportedAlgorithm(SecurityKey key, string algorithm)
  {
    if (key == null || string.IsNullOrEmpty(algorithm) || !algorithm.Equals("A128KW", StringComparison.Ordinal) && !algorithm.Equals("A256KW", StringComparison.Ordinal))
      return false;
    switch (key)
    {
      case SymmetricSecurityKey _:
        return true;
      case JsonWebKey jsonWebKey:
        if (jsonWebKey.K != null && jsonWebKey.Kty == "oct")
          return true;
        break;
    }
    return false;
  }

  public override byte[] UnwrapKey(byte[] keyBytes)
  {
    if (keyBytes == null || keyBytes.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (keyBytes));
    if (keyBytes.Length % 8 != 0)
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10664: The length of input must be a multiple of 64 bits. The input size is: '{0}' bits.", (object) (keyBytes.Length << 3)), nameof (keyBytes)));
    if (this._disposed)
      throw LogHelper.LogExceptionMessage((Exception) new ObjectDisposedException(this.GetType().ToString()));
    try
    {
      return this.UnwrapKeyPrivate(keyBytes, 0, keyBytes.Length);
    }
    catch (Exception ex)
    {
      throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenKeyWrapException(LogHelper.FormatInvariant("IDX10659: UnwrapKey failed, exception from cryptographic operation: '{0}'", (object) ex)));
    }
  }

  private byte[] UnwrapKeyPrivate(byte[] inputBuffer, int inputOffset, int inputCount)
  {
    byte[] numArray1 = new byte[SymmetricKeyWrapProvider._blockSizeInBytes];
    Array.Copy((Array) inputBuffer, inputOffset, (Array) numArray1, 0, SymmetricKeyWrapProvider._blockSizeInBytes);
    int num = inputCount - SymmetricKeyWrapProvider._blockSizeInBytes >> 3;
    byte[] numArray2 = new byte[num << 3];
    Array.Copy((Array) inputBuffer, inputOffset + SymmetricKeyWrapProvider._blockSizeInBytes, (Array) numArray2, 0, inputCount - SymmetricKeyWrapProvider._blockSizeInBytes);
    if (this._symmetricAlgorithmDecryptor == null)
    {
      lock (SymmetricKeyWrapProvider._decryptorLock)
      {
        if (this._symmetricAlgorithmDecryptor == null)
          this._symmetricAlgorithmDecryptor = this._symmetricAlgorithm.CreateDecryptor();
      }
    }
    byte[] numArray3 = new byte[16 /*0x10*/];
    for (int index1 = 5; index1 >= 0; --index1)
    {
      for (int index2 = num; index2 > 0; --index2)
      {
        ulong i = (ulong) (num * index1 + index2);
        Utility.Xor(numArray1, SymmetricKeyWrapProvider.GetBytes(i), 0, true);
        Array.Copy((Array) numArray1, (Array) numArray3, SymmetricKeyWrapProvider._blockSizeInBytes);
        Array.Copy((Array) numArray2, index2 - 1 << 3, (Array) numArray3, SymmetricKeyWrapProvider._blockSizeInBytes, SymmetricKeyWrapProvider._blockSizeInBytes);
        byte[] sourceArray = this._symmetricAlgorithmDecryptor.TransformFinalBlock(numArray3, 0, 16 /*0x10*/);
        Array.Copy((Array) sourceArray, (Array) numArray1, SymmetricKeyWrapProvider._blockSizeInBytes);
        Array.Copy((Array) sourceArray, SymmetricKeyWrapProvider._blockSizeInBytes, (Array) numArray2, index2 - 1 << 3, SymmetricKeyWrapProvider._blockSizeInBytes);
      }
    }
    if (!Utility.AreEqual(numArray1, SymmetricKeyWrapProvider._defaultIV))
      throw LogHelper.LogExceptionMessage((Exception) new InvalidOperationException("IDX10665: Data is not authentic"));
    byte[] destinationArray = new byte[num << 3];
    for (int index = 0; index < num; ++index)
      Array.Copy((Array) numArray2, index << 3, (Array) destinationArray, index << 3, 8);
    return destinationArray;
  }

  private void ValidateKeySize(byte[] key, string algorithm)
  {
    if ("A128KW".Equals(algorithm, StringComparison.Ordinal))
    {
      if (key.Length != 16 /*0x10*/)
        throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException("Length", LogHelper.FormatInvariant("IDX10662: The KeyWrap algorithm '{0}' requires a key size of '{1}' bits. Key '{2}', is of size:'{3}'.", (object) "A128KW", (object) 128 /*0x80*/, (object) this.Key.KeyId, (object) (key.Length << 3))));
    }
    else if ("A256KW".Equals(algorithm, StringComparison.Ordinal))
    {
      if (key.Length != 32 /*0x20*/)
        throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException("Length", LogHelper.FormatInvariant("IDX10662: The KeyWrap algorithm '{0}' requires a key size of '{1}' bits. Key '{2}', is of size:'{3}'.", (object) "A256KW", (object) 256 /*0x0100*/, (object) this.Key.KeyId, (object) (key.Length << 3))));
    }
    else
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException(nameof (algorithm), LogHelper.FormatInvariant("IDX10652: The algorithm '{0}' is not supported.", (object) algorithm)));
  }

  public override byte[] WrapKey(byte[] keyBytes)
  {
    if (keyBytes == null || keyBytes.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (keyBytes));
    if (keyBytes.Length % 8 != 0)
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10664: The length of input must be a multiple of 64 bits. The input size is: '{0}' bits.", (object) (keyBytes.Length << 3)), nameof (keyBytes)));
    if (this._disposed)
      throw LogHelper.LogExceptionMessage((Exception) new ObjectDisposedException(this.GetType().ToString()));
    try
    {
      return this.WrapKeyPrivate(keyBytes, 0, keyBytes.Length);
    }
    catch (Exception ex)
    {
      throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenKeyWrapException(LogHelper.FormatInvariant("IDX10658: WrapKey failed, exception from cryptographic operation: '{0}'", (object) ex)));
    }
  }

  private byte[] WrapKeyPrivate(byte[] inputBuffer, int inputOffset, int inputCount)
  {
    byte[] numArray1 = SymmetricKeyWrapProvider._defaultIV.Clone() as byte[];
    int num = inputCount >> 3;
    byte[] numArray2 = new byte[num << 3];
    Array.Copy((Array) inputBuffer, inputOffset, (Array) numArray2, 0, inputCount);
    if (this._symmetricAlgorithmEncryptor == null)
    {
      lock (SymmetricKeyWrapProvider._encryptorLock)
      {
        if (this._symmetricAlgorithmEncryptor == null)
          this._symmetricAlgorithmEncryptor = this._symmetricAlgorithm.CreateEncryptor();
      }
    }
    byte[] numArray3 = new byte[16 /*0x10*/];
    for (int index1 = 0; index1 < 6; ++index1)
    {
      for (int index2 = 0; index2 < num; ++index2)
      {
        ulong i = (ulong) (num * index1 + index2 + 1);
        Array.Copy((Array) numArray1, (Array) numArray3, numArray1.Length);
        Array.Copy((Array) numArray2, index2 << 3, (Array) numArray3, 8, 8);
        byte[] sourceArray = this._symmetricAlgorithmEncryptor.TransformFinalBlock(numArray3, 0, 16 /*0x10*/);
        Array.Copy((Array) sourceArray, (Array) numArray1, 8);
        Utility.Xor(numArray1, SymmetricKeyWrapProvider.GetBytes(i), 0, true);
        Array.Copy((Array) sourceArray, 8, (Array) numArray2, index2 << 3, 8);
      }
    }
    byte[] destinationArray = new byte[num + 1 << 3];
    Array.Copy((Array) numArray1, (Array) destinationArray, numArray1.Length);
    for (int index = 0; index < num; ++index)
      Array.Copy((Array) numArray2, index << 3, (Array) destinationArray, index + 1 << 3, 8);
    return destinationArray;
  }
}
