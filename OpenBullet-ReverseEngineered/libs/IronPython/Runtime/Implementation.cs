// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Implementation
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace IronPython.Runtime;

[PythonHidden(new PlatformID[] {})]
[PythonType("sys.implementation")]
public class Implementation
{
  internal static readonly string _Name = "IronPython";
  internal static readonly string _name = Implementation._Name.ToLowerInvariant();
  internal static readonly VersionInfo _version = new VersionInfo();
  internal static readonly int _hexversion = Implementation._version.GetHexVersion();
  public readonly string cache_tag;
  public readonly string name = Implementation._name;
  public readonly VersionInfo version = Implementation._version;
  public readonly int hexversion = Implementation._hexversion;

  public string __repr__(CodeContext context)
  {
    IEnumerable<string> source = PythonOps.GetAttrNames(context, (object) this).Where<object>((Func<object, bool>) (attr => !attr.ToString().StartsWith("_"))).Select<object, string>((Func<object, string>) (attr => $"{attr}={PythonOps.Repr(context, PythonOps.GetBoundAttr(context, (object) this, attr.ToString()))}"));
    return $"{PythonOps.GetPythonTypeName((object) this)}({string.Join(",", source.ToArray<string>())})";
  }
}
