// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.StringCleanup
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using System.Reflection.Emit;
using System.Runtime.InteropServices;

#nullable disable
namespace IronPython.Modules;

internal class StringCleanup : MarshalCleanup
{
  private readonly LocalBuilder _local;

  public StringCleanup(LocalBuilder local) => this._local = local;

  public override void Cleanup(ILGenerator generator)
  {
    generator.Emit(OpCodes.Ldloc, this._local);
    generator.Emit(OpCodes.Call, typeof (Marshal).GetMethod("FreeHGlobal"));
  }
}
