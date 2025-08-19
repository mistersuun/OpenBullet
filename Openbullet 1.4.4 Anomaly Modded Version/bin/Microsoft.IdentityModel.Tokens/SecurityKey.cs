// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.SecurityKey
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using System;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public abstract class SecurityKey
{
  private CryptoProviderFactory _cryptoProviderFactory;

  internal SecurityKey(SecurityKey key)
  {
    this._cryptoProviderFactory = key._cryptoProviderFactory;
    this.KeyId = key.KeyId;
  }

  public SecurityKey() => this._cryptoProviderFactory = CryptoProviderFactory.Default;

  [JsonIgnore]
  internal string InternalId { get; } = Guid.NewGuid().ToString();

  public abstract int KeySize { get; }

  [JsonIgnore]
  public virtual string KeyId { get; set; }

  [JsonIgnore]
  public CryptoProviderFactory CryptoProviderFactory
  {
    get => this._cryptoProviderFactory;
    set
    {
      this._cryptoProviderFactory = value ?? throw LogHelper.LogArgumentNullException(nameof (value));
    }
  }

  public override string ToString()
  {
    return $"{this.GetType()}, KeyId: '{this.KeyId}', InternalId: '{this.InternalId}'.";
  }
}
