// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.RSACryptoServiceProviderProxy
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System.Security.Cryptography;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class RSACryptoServiceProviderProxy : RSA
{
  private const int PROV_RSA_AES = 24;
  private const int PROV_RSA_FULL = 1;
  private const int PROV_RSA_SCHANNEL = 12;
  private bool _disposed;
  private bool _disposeRsa;
  private object _signLock = new object();
  private object _verifyLock = new object();
  private RSACryptoServiceProvider _rsa;

  public override string SignatureAlgorithm => this._rsa.SignatureAlgorithm;

  public override string KeyExchangeAlgorithm => this._rsa.KeyExchangeAlgorithm;

  public RSACryptoServiceProviderProxy(RSACryptoServiceProvider rsa)
  {
    if (rsa == null)
      throw LogHelper.LogArgumentNullException(nameof (rsa));
    if ((rsa.CspKeyContainerInfo.ProviderType == 1 || rsa.CspKeyContainerInfo.ProviderType == 12) && !rsa.CspKeyContainerInfo.HardwareDevice)
    {
      CspParameters parameters = new CspParameters();
      parameters.ProviderType = 24;
      parameters.KeyContainerName = rsa.CspKeyContainerInfo.KeyContainerName;
      parameters.KeyNumber = (int) rsa.CspKeyContainerInfo.KeyNumber;
      if (rsa.CspKeyContainerInfo.MachineKeyStore)
        parameters.Flags = CspProviderFlags.UseMachineKeyStore;
      parameters.Flags |= CspProviderFlags.UseExistingKey;
      this._rsa = new RSACryptoServiceProvider(parameters);
      this._disposeRsa = true;
    }
    else
      this._rsa = rsa;
  }

  public byte[] Decrypt(byte[] input, bool fOAEP)
  {
    if (input == null || input.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (input));
    return this._rsa.Decrypt(input, fOAEP);
  }

  public override byte[] DecryptValue(byte[] input)
  {
    return input != null && input.Length != 0 ? this._rsa.DecryptValue(input) : throw LogHelper.LogArgumentNullException(nameof (input));
  }

  public byte[] Encrypt(byte[] input, bool fOAEP)
  {
    if (input == null || input.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (input));
    return this._rsa.Encrypt(input, fOAEP);
  }

  public override byte[] EncryptValue(byte[] input)
  {
    return input != null && input.Length != 0 ? this._rsa.EncryptValue(input) : throw LogHelper.LogArgumentNullException(nameof (input));
  }

  public byte[] SignData(byte[] input, object hash)
  {
    if (input == null || input.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (input));
    if (hash == null)
      throw LogHelper.LogArgumentNullException(nameof (hash));
    lock (this._signLock)
      return this._rsa.SignData(input, hash);
  }

  public bool VerifyData(byte[] input, object hash, byte[] signature)
  {
    if (input == null || input.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (input));
    if (hash == null)
      throw LogHelper.LogArgumentNullException(nameof (hash));
    if (signature == null || signature.Length == 0)
      throw LogHelper.LogArgumentNullException(nameof (signature));
    lock (this._verifyLock)
      return this._rsa.VerifyData(input, hash, signature);
  }

  public override RSAParameters ExportParameters(bool includePrivateParameters)
  {
    return this._rsa.ExportParameters(includePrivateParameters);
  }

  public override void ImportParameters(RSAParameters parameters)
  {
    this._rsa.ImportParameters(parameters);
  }

  protected override void Dispose(bool disposing)
  {
    if (this._disposed)
      return;
    this._disposed = true;
    if (!disposing || !this._disposeRsa)
      return;
    this._rsa.Dispose();
  }
}
