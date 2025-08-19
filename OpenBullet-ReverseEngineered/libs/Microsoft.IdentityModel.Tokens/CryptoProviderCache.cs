// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.CryptoProviderCache
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public abstract class CryptoProviderCache
{
  protected abstract string GetCacheKey(SignatureProvider signatureProvider);

  protected abstract string GetCacheKey(
    SecurityKey securityKey,
    string algorithm,
    string typeofProvider);

  public abstract bool TryAdd(SignatureProvider signatureProvider);

  public abstract bool TryGetSignatureProvider(
    SecurityKey securityKey,
    string algorithm,
    string typeofProvider,
    bool willCreateSignatures,
    out SignatureProvider signatureProvider);

  public abstract bool TryRemove(SignatureProvider signatureProvider);
}
