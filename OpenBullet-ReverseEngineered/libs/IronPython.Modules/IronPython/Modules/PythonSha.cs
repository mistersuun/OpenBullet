// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonSha
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

#nullable disable
namespace IronPython.Modules;

public static class PythonSha
{
  public const string __doc__ = "implements the SHA1 hash algorithm";
  private const int DIGEST_SIZE = 20;
  private const int BLOCK_SIZE = 64 /*0x40*/;
  private static readonly Encoding _raw = Encoding.GetEncoding("iso-8859-1");
  private static readonly byte[] _empty = PythonSha._raw.GetBytes(string.Empty);

  public static int digest_size
  {
    [Documentation("Size of the resulting digest in bytes (constant)")] get => 20;
  }

  public static int digestsize
  {
    [Documentation("Size of the resulting digest in bytes (constant)")] get
    {
      return PythonSha.digest_size;
    }
  }

  public static int blocksize
  {
    [Documentation("Block size")] get => 64 /*0x40*/;
  }

  [Documentation("new([data]) -> object (object used to calculate hash)")]
  public static PythonSha.sha @new(object data) => new PythonSha.sha(data);

  [Documentation("new([data]) -> object (object used to calculate hash)")]
  public static PythonSha.sha @new(ArrayModule.array data) => new PythonSha.sha((object) data);

  [Documentation("new([data]) -> object (object used to calculate hash)")]
  public static PythonSha.sha @new(Bytes data) => new PythonSha.sha((IList<byte>) data);

  [Documentation("new([data]) -> object (object used to calculate hash)")]
  public static PythonSha.sha @new(PythonBuffer data) => new PythonSha.sha((IList<byte>) data);

  [Documentation("new([data]) -> object (object used to calculate hash)")]
  public static PythonSha.sha @new(ByteArray data) => new PythonSha.sha((IList<byte>) data);

  [Documentation("new([data]) -> object (object used to calculate hash)")]
  public static PythonSha.sha @new() => new PythonSha.sha();

  [Documentation("new([data]) -> object (object used to calculate hash)")]
  [PythonType]
  [PythonHidden(new PlatformID[] {})]
  public class sha : HashBase<SHA1>
  {
    public sha()
      : base("SHA1", 64 /*0x40*/, 20)
    {
    }

    public sha(object initialData)
      : this()
    {
      this.update(initialData);
    }

    internal sha(IList<byte> initialBytes)
      : this()
    {
      this.update(initialBytes);
    }

    protected override void CreateHasher() => this._hasher = (SHA1) new SHA1Managed();

    [Documentation("copy() -> object (copy of this object)")]
    public override HashBase<SHA1> copy()
    {
      PythonSha.sha sha = new PythonSha.sha();
      sha._hasher = this.CloneHasher();
      return (HashBase<SHA1>) sha;
    }
  }
}
