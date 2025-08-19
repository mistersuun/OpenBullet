// Decompiled with JetBrains decompiler
// Type: Mono.Security.Cryptography.SHA224
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using System;
using System.Security.Cryptography;

#nullable disable
namespace Mono.Security.Cryptography;

[PythonHidden(new PlatformID[] {})]
public abstract class SHA224 : HashAlgorithm
{
  public SHA224() => this.HashSizeValue = 224 /*0xE0*/;

  public static SHA224 Create() => SHA224.Create(nameof (SHA224));

  public static SHA224 Create(string hashName)
  {
    return (SHA224) (CryptoConfig.CreateFromName(hashName) ?? (object) new SHA224Managed());
  }
}
