// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.AuthenticatedEncryptionResult
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class AuthenticatedEncryptionResult
{
  public AuthenticatedEncryptionResult(
    SecurityKey key,
    byte[] ciphertext,
    byte[] iv,
    byte[] authenticationTag)
  {
    this.Key = key;
    this.Ciphertext = ciphertext;
    this.IV = iv;
    this.AuthenticationTag = authenticationTag;
  }

  public SecurityKey Key { get; private set; }

  public byte[] Ciphertext { get; private set; }

  public byte[] IV { get; private set; }

  public byte[] AuthenticationTag { get; private set; }
}
