// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.KeyWrapProvider
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using System;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public abstract class KeyWrapProvider : IDisposable
{
  public abstract string Algorithm { get; }

  public abstract string Context { get; set; }

  public abstract SecurityKey Key { get; }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  protected abstract void Dispose(bool disposing);

  public abstract byte[] UnwrapKey(byte[] keyBytes);

  public abstract byte[] WrapKey(byte[] keyBytes);
}
