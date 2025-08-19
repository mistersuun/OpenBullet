// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.KwCallInfo
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Runtime;

public sealed class KwCallInfo
{
  private readonly object[] _args;
  private readonly string[] _names;

  public KwCallInfo(object[] args, string[] names)
  {
    this._args = args;
    this._names = names;
  }

  public object[] Arguments => this._args;

  public string[] Names => this._names;
}
