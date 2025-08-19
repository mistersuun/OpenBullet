// Decompiled with JetBrains decompiler
// Type: System.Security.Cryptography.IncrementalHash
// Assembly: System.Security.Cryptography.Algorithms, Version=4.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ECE29162-DF05-4A45-A6F3-DE36C417868A
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Security.Cryptography.Algorithms.dll

#nullable disable
namespace System.Security.Cryptography;

public sealed class IncrementalHash : IDisposable
{
  private const int NTE_BAD_ALGID = -2146893816 /*0x80090008*/;
  private static readonly byte[] s_empty = new byte[0];
  private readonly HashAlgorithmName _algorithmName;
  private HashAlgorithm _hash;
  private bool _disposed;
  private bool _resetPending;

  private IncrementalHash(HashAlgorithmName name, HashAlgorithm hash)
  {
    this._algorithmName = name;
    this._hash = hash;
  }

  public HashAlgorithmName AlgorithmName => this._algorithmName;

  public void AppendData(byte[] data)
  {
    if (data == null)
      throw new ArgumentNullException(nameof (data));
    this.AppendData(data, 0, data.Length);
  }

  public void AppendData(byte[] data, int offset, int count)
  {
    if (data == null)
      throw new ArgumentNullException(nameof (data));
    if (offset < 0)
      throw new ArgumentOutOfRangeException(nameof (offset), SR.ArgumentOutOfRange_NeedNonNegNum);
    if (count < 0 || count > data.Length)
      throw new ArgumentOutOfRangeException(nameof (count));
    if (data.Length - count < offset)
      throw new ArgumentException(SR.Argument_InvalidOffLen);
    if (this._disposed)
      throw new ObjectDisposedException(typeof (IncrementalHash).Name);
    if (this._resetPending)
    {
      this._hash.Initialize();
      this._resetPending = false;
    }
    this._hash.TransformBlock(data, offset, count, (byte[]) null, 0);
  }

  public byte[] GetHashAndReset()
  {
    if (this._disposed)
      throw new ObjectDisposedException(typeof (IncrementalHash).Name);
    if (this._resetPending)
      this._hash.Initialize();
    this._hash.TransformFinalBlock(IncrementalHash.s_empty, 0, 0);
    byte[] hash = this._hash.Hash;
    this._resetPending = true;
    return hash;
  }

  public void Dispose()
  {
    this._disposed = true;
    if (this._hash == null)
      return;
    this._hash.Dispose();
    this._hash = (HashAlgorithm) null;
  }

  public static IncrementalHash CreateHash(HashAlgorithmName hashAlgorithm)
  {
    return !string.IsNullOrEmpty(hashAlgorithm.Name) ? new IncrementalHash(hashAlgorithm, IncrementalHash.GetHashAlgorithm(hashAlgorithm)) : throw new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, nameof (hashAlgorithm));
  }

  public static IncrementalHash CreateHMAC(HashAlgorithmName hashAlgorithm, byte[] key)
  {
    if (key == null)
      throw new ArgumentNullException(nameof (key));
    return !string.IsNullOrEmpty(hashAlgorithm.Name) ? new IncrementalHash(hashAlgorithm, IncrementalHash.GetHMAC(hashAlgorithm, key)) : throw new ArgumentException(SR.Cryptography_HashAlgorithmNameNullOrEmpty, nameof (hashAlgorithm));
  }

  private static HashAlgorithm GetHashAlgorithm(HashAlgorithmName hashAlgorithm)
  {
    if (HashAlgorithmName.op_Equality(hashAlgorithm, HashAlgorithmName.MD5))
      return (HashAlgorithm) new MD5CryptoServiceProvider();
    if (HashAlgorithmName.op_Equality(hashAlgorithm, HashAlgorithmName.SHA1))
      return (HashAlgorithm) new SHA1CryptoServiceProvider();
    if (HashAlgorithmName.op_Equality(hashAlgorithm, HashAlgorithmName.SHA256))
      return (HashAlgorithm) new SHA256CryptoServiceProvider();
    if (HashAlgorithmName.op_Equality(hashAlgorithm, HashAlgorithmName.SHA384))
      return (HashAlgorithm) new SHA384CryptoServiceProvider();
    if (HashAlgorithmName.op_Equality(hashAlgorithm, HashAlgorithmName.SHA512))
      return (HashAlgorithm) new SHA512CryptoServiceProvider();
    throw new CryptographicException(-2146893816 /*0x80090008*/);
  }

  private static HashAlgorithm GetHMAC(HashAlgorithmName hashAlgorithm, byte[] key)
  {
    if (HashAlgorithmName.op_Equality(hashAlgorithm, HashAlgorithmName.MD5))
      return (HashAlgorithm) new HMACMD5(key);
    if (HashAlgorithmName.op_Equality(hashAlgorithm, HashAlgorithmName.SHA1))
      return (HashAlgorithm) new HMACSHA1(key);
    if (HashAlgorithmName.op_Equality(hashAlgorithm, HashAlgorithmName.SHA256))
      return (HashAlgorithm) new HMACSHA256(key);
    if (HashAlgorithmName.op_Equality(hashAlgorithm, HashAlgorithmName.SHA384))
      return (HashAlgorithm) new HMACSHA384(key);
    if (HashAlgorithmName.op_Equality(hashAlgorithm, HashAlgorithmName.SHA512))
      return (HashAlgorithm) new HMACSHA512(key);
    throw new CryptographicException(-2146893816 /*0x80090008*/);
  }
}
