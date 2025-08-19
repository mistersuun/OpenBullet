// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.Asn1Object
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Modules;

internal class Asn1Object
{
  public Asn1Object(string shortName, string longName, int nid, byte[] oid)
  {
    this.ShortName = shortName;
    this.LongName = longName;
    this.NID = nid;
    this.OID = oid;
    this.OIDString = string.Join<byte>(".", (IEnumerable<byte>) this.OID);
  }

  public string ShortName { get; set; }

  public string LongName { get; set; }

  public int NID { get; set; }

  public byte[] OID { get; set; }

  public string OIDString { get; }

  public PythonTuple ToTuple()
  {
    return PythonTuple.MakeTuple((object) this.NID, (object) this.ShortName, (object) this.LongName, (object) this.OIDString);
  }
}
