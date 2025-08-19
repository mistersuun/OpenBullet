// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.AsymmetricSecurityKey
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using System;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public abstract class AsymmetricSecurityKey : SecurityKey
{
  public AsymmetricSecurityKey()
  {
  }

  internal AsymmetricSecurityKey(SecurityKey key)
    : base(key)
  {
  }

  [Obsolete("HasPrivateKey method is deprecated, please use PrivateKeyStatus.")]
  public abstract bool HasPrivateKey { get; }

  public abstract PrivateKeyStatus PrivateKeyStatus { get; }
}
