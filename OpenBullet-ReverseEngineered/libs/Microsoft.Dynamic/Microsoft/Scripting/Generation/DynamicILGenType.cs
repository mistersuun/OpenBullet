// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Generation.DynamicILGenType
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Reflection;
using System.Reflection.Emit;

#nullable disable
namespace Microsoft.Scripting.Generation;

internal class DynamicILGenType : DynamicILGen
{
  private readonly TypeBuilder _tb;
  private readonly MethodBuilder _mb;

  internal DynamicILGenType(TypeBuilder tb, MethodBuilder mb, ILGenerator il)
    : base(il)
  {
    this._tb = tb;
    this._mb = mb;
  }

  public override T CreateDelegate<T>(out MethodInfo mi)
  {
    ContractUtils.Requires(typeof (T).IsSubclassOf(typeof (Delegate)), nameof (T));
    mi = this.CreateMethod();
    return (T) mi.CreateDelegate(typeof (T));
  }

  private MethodInfo CreateMethod() => this._tb.CreateType().GetMethod(this._mb.Name);

  public override MethodInfo Finish() => this.CreateMethod();
}
