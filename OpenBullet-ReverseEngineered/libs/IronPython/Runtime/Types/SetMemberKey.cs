// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.SetMemberKey
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;

#nullable disable
namespace IronPython.Runtime.Types;

internal class SetMemberKey : IEquatable<SetMemberKey>
{
  public readonly Type Type;
  public readonly string Name;

  public SetMemberKey(Type type, string name)
  {
    this.Type = type;
    this.Name = name;
  }

  public bool Equals(SetMemberKey other) => this.Type == other.Type && this.Name == other.Name;

  public override bool Equals(object obj) => obj is SetMemberKey other && this.Equals(other);

  public override int GetHashCode() => this.Type.GetHashCode() ^ this.Name.GetHashCode();
}
