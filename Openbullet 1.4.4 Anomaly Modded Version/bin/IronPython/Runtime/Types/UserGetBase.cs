// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.UserGetBase
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;

#nullable disable
namespace IronPython.Runtime.Types;

internal class UserGetBase : FastGetBase
{
  internal readonly int _version;

  public UserGetBase(PythonGetMemberBinder binder, int version) => this._version = version;

  public override bool IsValid(PythonType type) => this._version == type.Version;
}
