// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonMD5
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

public static class PythonMD5
{
  public const string __doc__ = "MD5 hash algorithm";
  private const int DIGEST_SIZE = 16 /*0x10*/;
  private const int BLOCK_SIZE = 64 /*0x40*/;

  [Documentation("Size of the resulting digest in bytes (constant)")]
  public static int digest_size => 16 /*0x10*/;

  [Documentation("new([data]) -> object (new md5 object)")]
  public static PythonMD5.MD5Object @new(object data) => new PythonMD5.MD5Object(data);

  public static PythonMD5.MD5Object @new(ArrayModule.array data)
  {
    return new PythonMD5.MD5Object((object) data);
  }

  [Documentation("new([data]) -> object (new md5 object)")]
  public static PythonMD5.MD5Object @new(Bytes data) => new PythonMD5.MD5Object((IList<byte>) data);

  [Documentation("new([data]) -> object (new md5 object)")]
  public static PythonMD5.MD5Object @new(PythonBuffer data)
  {
    return new PythonMD5.MD5Object((IList<byte>) data);
  }

  [Documentation("new([data]) -> object (new md5 object)")]
  public static PythonMD5.MD5Object @new(ByteArray data)
  {
    return new PythonMD5.MD5Object((IList<byte>) data);
  }

  [Documentation("new([data]) -> object (new md5 object)")]
  public static PythonMD5.MD5Object @new() => new PythonMD5.MD5Object();

  [Documentation("new([data]) -> object (object used to calculate MD5 hash)")]
  [PythonHidden(new PlatformID[] {})]
  public class MD5Object : HashBase<MD5>
  {
    public MD5Object()
      : base("MD5", 64 /*0x40*/, 16 /*0x10*/)
    {
    }

    public MD5Object(object initialData)
      : this()
    {
      this.update(initialData);
    }

    internal MD5Object(IList<byte> initialBytes)
      : this()
    {
      this.update(initialBytes);
    }

    protected override void CreateHasher() => this._hasher = (MD5) new Mono.Security.Cryptography.MD5CryptoServiceProvider();

    [Documentation("copy() -> object (copy of this md5 object)")]
    public override HashBase<MD5> copy()
    {
      PythonMD5.MD5Object md5Object = new PythonMD5.MD5Object();
      md5Object._hasher = this.CloneHasher();
      return (HashBase<MD5>) md5Object;
    }
  }
}
