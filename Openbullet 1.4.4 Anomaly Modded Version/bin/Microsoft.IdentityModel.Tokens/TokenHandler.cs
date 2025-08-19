// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.TokenHandler
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.ComponentModel;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public abstract class TokenHandler
{
  private int _defaultTokenLifetimeInMinutes = TokenHandler.DefaultTokenLifetimeInMinutes;
  private int _maximumTokenSizeInBytes = 256000;
  public static readonly int DefaultTokenLifetimeInMinutes = 60;

  public virtual int MaximumTokenSizeInBytes
  {
    get => this._maximumTokenSizeInBytes;
    set
    {
      this._maximumTokenSizeInBytes = value >= 1 ? value : throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException(nameof (value), LogHelper.FormatInvariant("IDX10101: MaximumTokenSizeInBytes must be greater than zero. value: '{0}'", (object) value)));
    }
  }

  [DefaultValue(true)]
  public bool SetDefaultTimesOnTokenCreation { get; set; } = true;

  public int TokenLifetimeInMinutes
  {
    get => this._defaultTokenLifetimeInMinutes;
    set
    {
      this._defaultTokenLifetimeInMinutes = value >= 1 ? value : throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException(nameof (value), LogHelper.FormatInvariant("IDX10104: TokenLifetimeInMinutes must be greater than zero. value: '{0}'", (object) value)));
    }
  }
}
