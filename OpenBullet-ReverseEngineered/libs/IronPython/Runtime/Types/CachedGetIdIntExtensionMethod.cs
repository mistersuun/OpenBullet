// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.CachedGetIdIntExtensionMethod
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Runtime.Types;

internal sealed class CachedGetIdIntExtensionMethod : CachedGetKey
{
  private readonly int _id;

  public CachedGetIdIntExtensionMethod(string name, int id)
    : base(name)
  {
    this._id = id;
  }

  public override int GetHashCode() => this.Name.GetHashCode() ^ this._id;

  public override bool Equals(CachedGetKey other)
  {
    return other is CachedGetIdIntExtensionMethod intExtensionMethod && intExtensionMethod._id == this._id && intExtensionMethod.Name == this.Name;
  }
}
