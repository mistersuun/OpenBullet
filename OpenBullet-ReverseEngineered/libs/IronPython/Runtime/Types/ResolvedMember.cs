// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.ResolvedMember
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Actions;

#nullable disable
namespace IronPython.Runtime.Types;

internal class ResolvedMember
{
  public readonly string Name;
  public readonly MemberGroup Member;
  public static readonly ResolvedMember[] Empty = new ResolvedMember[0];

  public ResolvedMember(string name, MemberGroup member)
  {
    this.Name = name;
    this.Member = member;
  }
}
