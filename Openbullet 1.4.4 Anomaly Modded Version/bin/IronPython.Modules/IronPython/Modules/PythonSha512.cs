// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonSha512
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

#nullable disable
namespace IronPython.Modules;

[Documentation("SHA512 hash algorithm")]
public static class PythonSha512
{
  private const int BLOCK_SIZE = 128 /*0x80*/;
  public const string __doc__ = "SHA512 hash algorithm";

  public static PythonSha512.Sha512Object sha512(object data)
  {
    return new PythonSha512.Sha512Object(data);
  }

  public static PythonSha512.Sha512Object sha512(ArrayModule.array data)
  {
    return new PythonSha512.Sha512Object((object) data);
  }

  public static PythonSha512.Sha512Object sha512(Bytes data)
  {
    return new PythonSha512.Sha512Object((IList<byte>) data);
  }

  public static PythonSha512.Sha512Object sha512(PythonBuffer data)
  {
    return new PythonSha512.Sha512Object((IList<byte>) data);
  }

  public static PythonSha512.Sha512Object sha512(ByteArray data)
  {
    return new PythonSha512.Sha512Object((IList<byte>) data);
  }

  public static PythonSha512.Sha512Object sha512() => new PythonSha512.Sha512Object();

  public static PythonSha512.Sha384Object sha384(object data)
  {
    return new PythonSha512.Sha384Object(data);
  }

  public static PythonSha512.Sha384Object sha384(ArrayModule.array data)
  {
    return new PythonSha512.Sha384Object((object) data);
  }

  public static PythonSha512.Sha384Object sha384(Bytes data)
  {
    return new PythonSha512.Sha384Object((IList<byte>) data);
  }

  public static PythonSha512.Sha384Object sha384(PythonBuffer data)
  {
    return new PythonSha512.Sha384Object((IList<byte>) data);
  }

  public static PythonSha512.Sha384Object sha384(ByteArray data)
  {
    return new PythonSha512.Sha384Object((IList<byte>) data);
  }

  public static PythonSha512.Sha384Object sha384() => new PythonSha512.Sha384Object();

  [PythonHidden(new PlatformID[] {})]
  public sealed class Sha384Object : HashBase<SHA384>
  {
    internal Sha384Object()
      : base("SHA384", 128 /*0x80*/, 48 /*0x30*/)
    {
    }

    internal Sha384Object(object initialData)
      : this()
    {
      this.update(initialData);
    }

    internal Sha384Object(IList<byte> initialBytes)
      : this()
    {
      this.update(initialBytes);
    }

    protected override void CreateHasher() => this._hasher = SHA384.Create();

    [Documentation("copy() -> object (copy of this md5 object)")]
    public override HashBase<SHA384> copy()
    {
      PythonSha512.Sha384Object sha384Object = new PythonSha512.Sha384Object();
      sha384Object._hasher = this.CloneHasher();
      return (HashBase<SHA384>) sha384Object;
    }
  }

  [PythonHidden(new PlatformID[] {})]
  public sealed class Sha512Object : HashBase<SHA512>
  {
    internal Sha512Object()
      : base("SHA512", 128 /*0x80*/, 64 /*0x40*/)
    {
    }

    internal Sha512Object(object initialData)
      : this()
    {
      this.update(initialData);
    }

    internal Sha512Object(IList<byte> initialBytes)
      : this()
    {
      this.update(initialBytes);
    }

    protected override void CreateHasher() => this._hasher = SHA512.Create();

    [Documentation("copy() -> object (copy of this md5 object)")]
    public override HashBase<SHA512> copy()
    {
      PythonSha512.Sha512Object sha512Object = new PythonSha512.Sha512Object();
      sha512Object._hasher = this.CloneHasher();
      return (HashBase<SHA512>) sha512Object;
    }
  }
}
