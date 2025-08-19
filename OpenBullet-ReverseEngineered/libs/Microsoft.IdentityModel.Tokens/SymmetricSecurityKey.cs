// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.SymmetricSecurityKey
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class SymmetricSecurityKey : SecurityKey
{
  private int _keySize;
  private byte[] _key;

  internal SymmetricSecurityKey(JsonWebKey webKey)
    : base((SecurityKey) webKey)
  {
    this._key = !string.IsNullOrEmpty(webKey.K) ? Base64UrlEncoder.DecodeBytes(webKey.K) : throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10703: Cannot create a '{0}', key length is zero.", (object) typeof (SymmetricSecurityKey))));
    this._keySize = this._key.Length * 8;
    webKey.ConvertedSecurityKey = (SecurityKey) this;
  }

  public SymmetricSecurityKey(byte[] key)
  {
    if (key == null)
      throw LogHelper.LogArgumentNullException(nameof (key));
    this._key = key.Length != 0 ? key.CloneByteArray() : throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10703: Cannot create a '{0}', key length is zero.", (object) typeof (SymmetricSecurityKey))));
    this._keySize = this._key.Length * 8;
  }

  public override int KeySize => this._keySize;

  public virtual byte[] Key => this._key.CloneByteArray();
}
