// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.SignatureProvider
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public abstract class SignatureProvider : IDisposable
{
  protected SignatureProvider(SecurityKey key, string algorithm)
  {
    this.Key = key ?? throw LogHelper.LogArgumentNullException(nameof (key));
    this.Algorithm = !string.IsNullOrEmpty(algorithm) ? algorithm : throw LogHelper.LogArgumentNullException(nameof (algorithm));
  }

  public string Algorithm { get; private set; }

  public string Context { get; set; }

  public CryptoProviderCache CryptoProviderCache { get; set; }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  protected abstract void Dispose(bool disposing);

  public SecurityKey Key { get; private set; }

  public abstract byte[] Sign(byte[] input);

  public abstract bool Verify(byte[] input, byte[] signature);

  public bool WillCreateSignatures { get; protected set; }
}
