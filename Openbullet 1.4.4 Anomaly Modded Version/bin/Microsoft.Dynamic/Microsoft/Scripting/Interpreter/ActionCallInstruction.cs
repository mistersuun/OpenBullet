// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.ActionCallInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class ActionCallInstruction : CallInstruction
{
  private readonly Action _target;

  public override MethodInfo Info
  {
    get => RuntimeReflectionExtensions.GetMethodInfo((Delegate) this._target);
  }

  public override int ArgumentCount => 0;

  public ActionCallInstruction(Action target) => this._target = target;

  public ActionCallInstruction(MethodInfo target)
  {
    this._target = (Action) target.CreateDelegate(typeof (Action));
  }

  public override object Invoke()
  {
    this._target();
    return (object) null;
  }

  public override int Run(InterpretedFrame frame)
  {
    this._target();
    InterpretedFrame interpretedFrame = frame;
    interpretedFrame.StackIndex = interpretedFrame.StackIndex;
    return 1;
  }
}
