// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.LocalOrArg
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using System;
using System.Reflection.Emit;

#nullable disable
namespace IronPython.Modules;

internal abstract class LocalOrArg
{
  public abstract void Emit(ILGenerator ilgen);

  public abstract Type Type { get; }
}
