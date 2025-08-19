// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonSha256
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using Microsoft.Scripting.Runtime;
using Mono.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

#nullable disable
namespace IronPython.Modules;

[Documentation("SHA256 hash algorithm")]
public static class PythonSha256
{
  private const int BLOCK_SIZE = 64 /*0x40*/;
  public const string __doc__ = "SHA256 hash algorithm";

  public static PythonSha256.Sha256Object sha256(object data)
  {
    return new PythonSha256.Sha256Object(data);
  }

  public static PythonSha256.Sha256Object sha256(ArrayModule.array data)
  {
    return new PythonSha256.Sha256Object((object) data);
  }

  public static PythonSha256.Sha256Object sha256(Bytes data)
  {
    return new PythonSha256.Sha256Object((IList<byte>) data);
  }

  public static PythonSha256.Sha256Object sha256(PythonBuffer data)
  {
    return new PythonSha256.Sha256Object((IList<byte>) data);
  }

  public static PythonSha256.Sha256Object sha256(ByteArray data)
  {
    return new PythonSha256.Sha256Object((IList<byte>) data);
  }

  public static PythonSha256.Sha256Object sha256() => new PythonSha256.Sha256Object();

  public static PythonSha256.Sha224Object sha224(object data)
  {
    return new PythonSha256.Sha224Object(data);
  }

  public static PythonSha256.Sha224Object sha224(ArrayModule.array data)
  {
    return new PythonSha256.Sha224Object((object) data);
  }

  public static PythonSha256.Sha224Object sha224(Bytes data)
  {
    return new PythonSha256.Sha224Object((IList<byte>) data);
  }

  public static PythonSha256.Sha224Object sha224(PythonBuffer data)
  {
    return new PythonSha256.Sha224Object((IList<byte>) data);
  }

  public static PythonSha256.Sha224Object sha224(ByteArray data)
  {
    return new PythonSha256.Sha224Object((IList<byte>) data);
  }

  public static PythonSha256.Sha224Object sha224() => new PythonSha256.Sha224Object();

  [PythonHidden(new PlatformID[] {})]
  public sealed class Sha256Object : HashBase<SHA256>
  {
    internal Sha256Object()
      : base("SHA256", 64 /*0x40*/, 32 /*0x20*/)
    {
    }

    internal Sha256Object(object initialData)
      : this()
    {
      this.update(initialData);
    }

    internal Sha256Object(IList<byte> initialBytes)
      : this()
    {
      this.update(initialBytes);
    }

    [Documentation("copy() -> object (copy of this object)")]
    public override HashBase<SHA256> copy()
    {
      PythonSha256.Sha256Object sha256Object = new PythonSha256.Sha256Object();
      sha256Object._hasher = this.CloneHasher();
      return (HashBase<SHA256>) sha256Object;
    }

    protected override void CreateHasher() => this._hasher = SHA256.Create();
  }

  [PythonHidden(new PlatformID[] {})]
  public sealed class Sha224Object : HashBase<SHA224>
  {
    internal Sha224Object()
      : base("SHA224", 64 /*0x40*/, 28)
    {
    }

    internal Sha224Object(object initialData)
      : this()
    {
      this.update(initialData);
    }

    internal Sha224Object(IList<byte> initialBytes)
      : this()
    {
      this.update(initialBytes);
    }

    protected override void CreateHasher() => this._hasher = SHA224.Create();

    [Documentation("copy() -> object (copy of this object)")]
    public override HashBase<SHA224> copy()
    {
      PythonSha256.Sha224Object sha224Object = new PythonSha256.Sha224Object();
      sha224Object._hasher = this.CloneHasher();
      return (HashBase<SHA224>) sha224Object;
    }
  }
}
