// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.FuncCallInstruction`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class FuncCallInstruction<TRet> : CallInstruction
{
  private readonly Func<TRet> _target;

  public override MethodInfo Info
  {
    get => RuntimeReflectionExtensions.GetMethodInfo((Delegate) this._target);
  }

  public override int ArgumentCount => 0;

  public FuncCallInstruction(Func<TRet> target) => this._target = target;

  public FuncCallInstruction(MethodInfo target)
  {
    this._target = (Func<TRet>) target.CreateDelegate(typeof (Func<TRet>));
  }

  public override object Invoke() => (object) this._target();

  public override int Run(InterpretedFrame frame)
  {
    frame.Data[frame.StackIndex] = (object) this._target();
    frame.StackIndex -= -1;
    return 1;
  }
}
