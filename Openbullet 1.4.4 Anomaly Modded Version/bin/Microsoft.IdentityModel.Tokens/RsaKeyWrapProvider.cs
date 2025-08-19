// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.RsaKeyWrapProvider
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class RsaKeyWrapProvider : KeyWrapProvider
{
  private AsymmetricAdapter _asymmetricAdapter;
  private bool _disposed;

  public RsaKeyWrapProvider(SecurityKey key, string algorithm, bool willUnwrap)
  {
    if (key == null)
      throw LogHelper.LogArgumentNullException(nameof (key));
    if (string.IsNullOrEmpty(algorithm))
      throw LogHelper.LogArgumentNullException(nameof (algorithm));
    this.Algorithm = this.IsSupportedAlgorithm(key, algorithm) ? algorithm : throw LogHelper.LogExceptionMessage((Exception) new NotSupportedException(LogHelper.FormatInvariant("IDX10661: Unable to create the KeyWrapProvider.\nKeyWrapAlgorithm: '{0}', SecurityKey: '{1}'\n is not supported.", (object) algorithm, (object) key)));
    this.Key = key;
    this._asymmetricAdapter = new AsymmetricAdapter(key, algorithm, willUnwrap);
  }

  public override string Algorithm { get; }

  public override string Context { get; set; }

  public override SecurityKey Key { get; }

  protected override void Dispose(bool disposing)
  {
    if (this._disposed || !disposing)
      return;
    this._disposed = true;
    this._asymmetricAdapter.Dispose();
  }

  protected virtual bool IsSupportedAlgorithm(SecurityKey key, string algorithm)
  {
    return key != null && !string.IsNullOrEmpty(algorithm) && key.KeySize >= 2048 /*0x0800*/ && SupportedAlgorithms.IsSupportedKeyWrapAlgorithm(algorithm, key);
  }

  public override byte[] UnwrapKey(byte[] keyBytes)
  {
    if (keyBytes == null || keyBytes.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (keyBytes));
    if (this._disposed)
      throw LogHelper.LogExceptionMessage((Exception) new ObjectDisposedException(this.GetType().ToString()));
    try
    {
      return this._asymmetricAdapter.Decrypt(keyBytes);
    }
    catch (Exception ex)
    {
      throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenKeyWrapException(LogHelper.FormatInvariant("IDX10659: UnwrapKey failed, exception from cryptographic operation: '{0}'", (object) ex)));
    }
  }

  public override byte[] WrapKey(byte[] keyBytes)
  {
    if (keyBytes == null || keyBytes.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (keyBytes));
    if (this._disposed)
      throw LogHelper.LogExceptionMessage((Exception) new ObjectDisposedException(this.GetType().ToString()));
    try
    {
      return this._asymmetricAdapter.Encrypt(keyBytes);
    }
    catch (Exception ex)
    {
      throw LogHelper.LogExceptionMessage((Exception) new SecurityTokenKeyWrapException(LogHelper.FormatInvariant("IDX10658: WrapKey failed, exception from cryptographic operation: '{0}'", (object) ex)));
    }
  }
}
