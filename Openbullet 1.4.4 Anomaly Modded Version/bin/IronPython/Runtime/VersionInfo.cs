// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.VersionInfo
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Runtime;

[PythonType("sys.version_info")]
public class VersionInfo : PythonTuple
{
  public readonly int major;
  public readonly int minor;
  public readonly int micro;
  public readonly string releaselevel;
  public readonly int serial;

  private VersionInfo(int major, int minor, int micro, string releaselevel, int serial)
    : base(new object[5]
    {
      (object) major,
      (object) minor,
      (object) micro,
      (object) releaselevel,
      (object) serial
    })
  {
    this.major = major;
    this.minor = minor;
    this.micro = micro;
    this.releaselevel = releaselevel;
    this.serial = serial;
  }

  internal VersionInfo()
    : this(2, 7, 9, "final", 0)
  {
  }

  public override string __repr__(CodeContext context)
  {
    return $"sys.version_info(major={this.major}, minor={this.minor}, micro={this.micro}, releaselevel='{this.releaselevel}', serial={this.serial})";
  }

  internal int GetHexVersion()
  {
    int num = 0;
    switch (this.releaselevel)
    {
      case "alpha":
        num = 10;
        break;
      case "beta":
        num = 11;
        break;
      case "candidate":
        num = 12;
        break;
      case "final":
        num = 15;
        break;
    }
    return this.major << 24 | this.minor << 16 /*0x10*/ | this.micro << 8 | num << 4 | this.serial;
  }

  internal string GetVersionString(string _initialVersionString)
  {
    return string.Format("{0}.{1}.{2}{4}{5} ({3})", (object) this.major, (object) this.minor, (object) this.micro, (object) _initialVersionString, this.releaselevel != "final" ? (object) "f" : (object) "", this.releaselevel != "final" ? (object) this.serial.ToString() : (object) "");
  }
}
