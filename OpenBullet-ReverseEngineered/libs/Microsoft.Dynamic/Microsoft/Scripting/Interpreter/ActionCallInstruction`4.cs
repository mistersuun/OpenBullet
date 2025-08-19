// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.ActionCallInstruction`4
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class ActionCallInstruction<T0, T1, T2, T3> : CallInstruction
{
  private readonly Action<T0, T1, T2, T3> _target;

  public override MethodInfo Info
  {
    get => RuntimeReflectionExtensions.GetMethodInfo((Delegate) this._target);
  }

  public override int ArgumentCount => 4;

  public ActionCallInstruction(Action<T0, T1, T2, T3> target) => this._target = target;

  public ActionCallInstruction(MethodInfo target)
  {
    this._target = (Action<T0, T1, T2, T3>) target.CreateDelegate(typeof (Action<T0, T1, T2, T3>));
  }

  public override object Invoke(object arg0, object arg1, object arg2, object arg3)
  {
    this._target(arg0 != null ? (T0) arg0 : default (T0), arg1 != null ? (T1) arg1 : default (T1), arg2 != null ? (T2) arg2 : default (T2), arg3 != null ? (T3) arg3 : default (T3));
    return (object) null;
  }

  public override int Run(InterpretedFrame frame)
  {
    this._target((T0) frame.Data[frame.StackIndex - 4], (T1) frame.Data[frame.StackIndex - 3], (T2) frame.Data[frame.StackIndex - 2], (T3) frame.Data[frame.StackIndex - 1]);
    frame.StackIndex -= 4;
    return 1;
  }
}
