// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.Arg
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using System;
using System.Reflection.Emit;

#nullable disable
namespace IronPython.Modules;

internal class Arg : LocalOrArg
{
  private readonly int _index;
  private readonly Type _type;

  public Arg(int index, Type type)
  {
    this._index = index;
    this._type = type;
  }

  public override void Emit(ILGenerator ilgen) => ilgen.Emit(OpCodes.Ldarg, this._index);

  public override Type Type => this._type;
}
