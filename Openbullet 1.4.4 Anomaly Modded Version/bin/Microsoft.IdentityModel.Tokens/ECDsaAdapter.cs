// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.ECDsaAdapter
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

internal class ECDsaAdapter
{
  internal readonly CreateECDsaDelegate CreateECDsaFunction;
  internal static ECDsaAdapter Instance = new ECDsaAdapter();

  internal ECDsaAdapter()
  {
    this.CreateECDsaFunction = new CreateECDsaDelegate(this.CreateECDsaUsingCNGKey);
  }

  internal ECDsa CreateECDsa(JsonWebKey jsonWebKey, bool usePrivateKey)
  {
    if (this.CreateECDsaFunction != null)
      return this.CreateECDsaFunction(jsonWebKey, usePrivateKey);
    throw LogHelper.LogExceptionMessage((Exception) new PlatformNotSupportedException("IDX10690: ECDsa creation is not supported by NETSTANDARD1.4, when running on platforms other than Windows. For more details, see https://aka.ms/IdentityModel/create-ecdsa"));
  }

  private ECDsa CreateECDsaUsingCNGKey(JsonWebKey jsonWebKey, bool usePrivateKey)
  {
    if (jsonWebKey == null)
      throw LogHelper.LogArgumentNullException(nameof (jsonWebKey));
    if (jsonWebKey.Crv == null)
      throw LogHelper.LogArgumentNullException("Crv");
    if (jsonWebKey.X == null)
      throw LogHelper.LogArgumentNullException("X");
    if (jsonWebKey.Y == null)
      throw LogHelper.LogArgumentNullException("Y");
    GCHandle gcHandle = new GCHandle();
    try
    {
      uint magicValue = this.GetMagicValue(jsonWebKey.Crv, usePrivateKey);
      uint keyByteCount = this.GetKeyByteCount(jsonWebKey.Crv);
      byte[] numArray1 = !usePrivateKey ? new byte[(long) (2U * keyByteCount) + (long) (2 * Marshal.SizeOf<uint>())] : new byte[(long) (3U * keyByteCount) + (long) (2 * Marshal.SizeOf<uint>())];
      gcHandle = GCHandle.Alloc((object) numArray1, GCHandleType.Pinned);
      IntPtr num1 = gcHandle.AddrOfPinnedObject();
      byte[] numArray2 = Base64UrlEncoder.DecodeBytes(jsonWebKey.X);
      if ((long) numArray2.Length > (long) keyByteCount)
        throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException("x.Length", LogHelper.FormatInvariant("IDX10675: The byte count of '{0}' must be less than or equal to '{1}', but was {2}.", (object) "x", (object) keyByteCount, (object) numArray2.Length)));
      byte[] numArray3 = Base64UrlEncoder.DecodeBytes(jsonWebKey.Y);
      if ((long) numArray3.Length > (long) keyByteCount)
        throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException("y.Length", LogHelper.FormatInvariant("IDX10675: The byte count of '{0}' must be less than or equal to '{1}', but was {2}.", (object) "y", (object) keyByteCount, (object) numArray3.Length)));
      Marshal.WriteInt64(num1, 0, (long) magicValue);
      Marshal.WriteInt64(num1, 4, (long) keyByteCount);
      int num2 = 8;
      foreach (byte val in numArray2)
        Marshal.WriteByte(num1, num2++, val);
      foreach (byte val in numArray3)
        Marshal.WriteByte(num1, num2++, val);
      if (usePrivateKey)
      {
        byte[] numArray4 = jsonWebKey.D != null ? Base64UrlEncoder.DecodeBytes(jsonWebKey.D) : throw LogHelper.LogArgumentNullException("D");
        if ((long) numArray4.Length > (long) keyByteCount)
          throw LogHelper.LogExceptionMessage((Exception) new ArgumentOutOfRangeException("d.Length", LogHelper.FormatInvariant("IDX10675: The byte count of '{0}' must be less than or equal to '{1}', but was {2}.", (object) "d", (object) keyByteCount, (object) numArray4.Length)));
        foreach (byte val in numArray4)
          Marshal.WriteByte(num1, num2++, val);
        Marshal.Copy(num1, numArray1, 0, numArray1.Length);
        using (CngKey key = CngKey.Import(numArray1, CngKeyBlobFormat.EccPrivateBlob))
          return (ECDsa) new ECDsaCng(key);
      }
      Marshal.Copy(num1, numArray1, 0, numArray1.Length);
      using (CngKey key = CngKey.Import(numArray1, CngKeyBlobFormat.EccPublicBlob))
        return (ECDsa) new ECDsaCng(key);
    }
    catch (Exception ex)
    {
      throw LogHelper.LogExceptionMessage((Exception) new CryptographicException("IDX10689: Unable to create an ECDsa object. See inner exception for more details.", ex));
    }
    finally
    {
      if (gcHandle.IsAllocated)
        gcHandle.Free();
    }
  }

  private uint GetKeyByteCount(string curveId)
  {
    if (string.IsNullOrEmpty(curveId))
      throw LogHelper.LogArgumentNullException(nameof (curveId));
    switch (curveId)
    {
      case "P-256":
        return 32 /*0x20*/;
      case "P-384":
        return 48 /*0x30*/;
      case "P-512":
      case "P-521":
        return 66;
      default:
        throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10645: Elliptical Curve not supported for curveId: '{0}'", (object) curveId)));
    }
  }

  private int GetKeySize(string curveId)
  {
    if (string.IsNullOrEmpty(curveId))
      throw LogHelper.LogArgumentNullException(nameof (curveId));
    switch (curveId)
    {
      case "P-256":
        return 256 /*0x0100*/;
      case "P-384":
        return 384;
      case "P-512":
      case "P-521":
        return 521;
      default:
        throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10645: Elliptical Curve not supported for curveId: '{0}'", (object) curveId)));
    }
  }

  private uint GetMagicValue(string curveId, bool willCreateSignatures)
  {
    if (string.IsNullOrEmpty(curveId))
      throw LogHelper.LogArgumentNullException(nameof (curveId));
    switch (curveId)
    {
      case "P-256":
        return willCreateSignatures ? 844317509U : 827540293U;
      case "P-384":
        return willCreateSignatures ? 877871941U : 861094725U;
      case "P-512":
      case "P-521":
        return willCreateSignatures ? 911426373U : 894649157U;
      default:
        throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10645: Elliptical Curve not supported for curveId: '{0}'", (object) curveId)));
    }
  }

  [MethodImpl(MethodImplOptions.NoOptimization)]
  private bool SupportsCNGKey()
  {
    try
    {
      CngKeyBlobFormat eccPrivateBlob = CngKeyBlobFormat.EccPrivateBlob;
      return true;
    }
    catch
    {
      return false;
    }
  }

  private enum KeyBlobMagicNumber : uint
  {
    BCRYPT_ECDSA_PUBLIC_P256_MAGIC = 827540293, // 0x31534345
    BCRYPT_ECDSA_PRIVATE_P256_MAGIC = 844317509, // 0x32534345
    BCRYPT_ECDSA_PUBLIC_P384_MAGIC = 861094725, // 0x33534345
    BCRYPT_ECDSA_PRIVATE_P384_MAGIC = 877871941, // 0x34534345
    BCRYPT_ECDSA_PUBLIC_P521_MAGIC = 894649157, // 0x35534345
    BCRYPT_ECDSA_PRIVATE_P521_MAGIC = 911426373, // 0x36534345
  }
}
