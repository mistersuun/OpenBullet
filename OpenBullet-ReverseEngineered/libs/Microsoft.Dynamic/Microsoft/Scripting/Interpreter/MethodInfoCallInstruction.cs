// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.MethodInfoCallInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class MethodInfoCallInstruction : CallInstruction
{
  private readonly MethodInfo _target;
  private readonly int _argumentCount;

  public override MethodInfo Info => this._target;

  public override int ArgumentCount => this._argumentCount;

  internal MethodInfoCallInstruction(MethodInfo target, int argumentCount)
  {
    this._target = target;
    this._argumentCount = argumentCount;
  }

  public override object Invoke(params object[] args) => this.InvokeWorker(args);

  public override object InvokeInstance(object instance, params object[] args)
  {
    if (this._target.IsStatic)
    {
      try
      {
        return this._target.Invoke((object) null, args);
      }
      catch (TargetInvocationException ex)
      {
        throw ExceptionHelpers.UpdateForRethrow(ex.InnerException);
      }
    }
    else
    {
      try
      {
        return this._target.Invoke(instance, args);
      }
      catch (TargetInvocationException ex)
      {
        throw ExceptionHelpers.UpdateForRethrow(ex.InnerException);
      }
    }
  }

  private object InvokeWorker(params object[] args)
  {
    if (this._target.IsStatic)
    {
      try
      {
        return this._target.Invoke((object) null, args);
      }
      catch (TargetInvocationException ex)
      {
        throw ExceptionHelpers.UpdateForRethrow(ex.InnerException);
      }
    }
    else
    {
      try
      {
        return this._target.Invoke(args[0], MethodInfoCallInstruction.GetNonStaticArgs(args));
      }
      catch (TargetInvocationException ex)
      {
        throw ExceptionHelpers.UpdateForRethrow(ex.InnerException);
      }
    }
  }

  private static object[] GetNonStaticArgs(object[] args)
  {
    object[] nonStaticArgs = new object[args.Length - 1];
    for (int index = 0; index < nonStaticArgs.Length; ++index)
      nonStaticArgs[index] = args[index + 1];
    return nonStaticArgs;
  }

  public sealed override int Run(InterpretedFrame frame)
  {
    int index1 = frame.StackIndex - this._argumentCount;
    object[] objArray = new object[this._argumentCount];
    for (int index2 = 0; index2 < objArray.Length; ++index2)
      objArray[index2] = frame.Data[index1 + index2];
    object obj = this.Invoke(objArray);
    if (this._target.ReturnType != typeof (void))
    {
      frame.Data[index1] = obj;
      frame.StackIndex = index1 + 1;
    }
    else
      frame.StackIndex = index1;
    return 1;
  }

  public override object Invoke() => this.InvokeWorker();

  public override object Invoke(object arg0) => this.InvokeWorker(arg0);

  public override object Invoke(object arg0, object arg1) => this.InvokeWorker(arg0, arg1);
}
