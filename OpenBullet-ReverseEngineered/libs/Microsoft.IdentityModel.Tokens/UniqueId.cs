// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.UniqueId
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.Security.Cryptography;
using System.Text;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public static class UniqueId
{
  private const int RandomSaltSize = 16 /*0x10*/;
  private const string NcNamePrefix = "_";
  private const string UuidUriPrefix = "urn:uuid:";
  private static readonly string reusableUuid = UniqueId.GetRandomUuid();
  private static readonly string optimizedNcNamePrefix = $"_{UniqueId.reusableUuid}-";

  public static string CreateUniqueId() => UniqueId.optimizedNcNamePrefix + UniqueId.GetNextId();

  public static string CreateUniqueId(string prefix)
  {
    if (string.IsNullOrEmpty(prefix))
      throw LogHelper.LogArgumentNullException(nameof (prefix));
    return $"{prefix}{UniqueId.reusableUuid}-{UniqueId.GetNextId()}";
  }

  public static string CreateRandomId() => "_" + UniqueId.GetRandomUuid();

  public static string CreateRandomId(string prefix)
  {
    if (string.IsNullOrEmpty(prefix))
      throw LogHelper.LogArgumentNullException(nameof (prefix));
    return prefix + UniqueId.GetRandomUuid();
  }

  public static Uri CreateRandomUri() => new Uri("urn:uuid:" + UniqueId.GetRandomUuid());

  private static string GetNextId()
  {
    RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
    byte[] numArray = new byte[16 /*0x10*/];
    byte[] data = numArray;
    randomNumberGenerator.GetBytes(data);
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; index < numArray.Length; ++index)
      stringBuilder.AppendFormat("{0:X2}", (object) numArray[index]);
    return stringBuilder.ToString();
  }

  private static string GetRandomUuid() => Guid.NewGuid().ToString("D");
}
