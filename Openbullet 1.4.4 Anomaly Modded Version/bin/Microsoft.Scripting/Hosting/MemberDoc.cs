// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.MemberDoc
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace Microsoft.Scripting.Hosting;

[Serializable]
public class MemberDoc
{
  private readonly string _name;
  private readonly MemberKind _kind;

  public MemberDoc(string name, MemberKind kind)
  {
    ContractUtils.RequiresNotNull((object) name, nameof (name));
    ContractUtils.Requires(kind >= MemberKind.None && kind <= MemberKind.Namespace, nameof (kind));
    this._name = name;
    this._kind = kind;
  }

  public string Name => this._name;

  public MemberKind Kind => this._kind;
}
