// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Generation.DynamicILGenMethod
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Reflection;
using System.Reflection.Emit;

#nullable disable
namespace Microsoft.Scripting.Generation;

internal class DynamicILGenMethod : DynamicILGen
{
  private readonly DynamicMethod _dm;

  internal DynamicILGenMethod(DynamicMethod dm, ILGenerator il)
    : base(il)
  {
    this._dm = dm;
  }

  public override T CreateDelegate<T>(out MethodInfo mi)
  {
    ContractUtils.Requires(typeof (T).IsSubclassOf(typeof (Delegate)), nameof (T));
    mi = (MethodInfo) this._dm;
    return (T) ((MethodInfo) this._dm).CreateDelegate(typeof (T), (object) null);
  }

  public override MethodInfo Finish() => (MethodInfo) this._dm;
}
