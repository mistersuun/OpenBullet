// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.LightLambda
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

public class LightLambda
{
  private readonly StrongBox<object>[] _closure;
  private readonly Microsoft.Scripting.Interpreter.Interpreter _interpreter;
  private static readonly CacheDict<Type, Func<LightLambda, Delegate>> _runCache = new CacheDict<Type, Func<LightLambda, Delegate>>(100);
  private readonly LightDelegateCreator _delegateCreator;
  private Delegate _compiled;
  private int _compilationThreshold;
  internal const int MaxParameters = 16 /*0x10*/;

  public event EventHandler<LightLambdaCompileEventArgs> Compile;

  internal LightLambda(
    LightDelegateCreator delegateCreator,
    StrongBox<object>[] closure,
    int compilationThreshold)
  {
    this._delegateCreator = delegateCreator;
    this._closure = closure;
    this._interpreter = delegateCreator.Interpreter;
    this._compilationThreshold = compilationThreshold;
  }

  private static Func<LightLambda, Delegate> GetRunDelegateCtor(Type delegateType)
  {
    lock (LightLambda._runCache)
    {
      Func<LightLambda, Delegate> func;
      return LightLambda._runCache.TryGetValue(delegateType, out func) ? func : LightLambda.MakeRunDelegateCtor(delegateType);
    }
  }

  private static Func<LightLambda, Delegate> MakeRunDelegateCtor(Type delegateType)
  {
    MethodInfo method1 = delegateType.GetMethod("Invoke");
    ParameterInfo[] parameters = method1.GetParameters();
    string str = "Run";
    if (parameters.Length >= 16 /*0x10*/)
      return (Func<LightLambda, Delegate>) null;
    Type[] types;
    if (method1.ReturnType == typeof (void))
    {
      str += "Void";
      types = new Type[parameters.Length];
    }
    else
    {
      types = new Type[parameters.Length + 1];
      types[types.Length - 1] = method1.ReturnType;
    }
    MethodInfo method2;
    if (method1.ReturnType == typeof (void) && types.Length == 2 && parameters[0].ParameterType.IsByRef && parameters[1].ParameterType.IsByRef)
    {
      method2 = typeof (LightLambda).GetMethod("RunVoidRef2", BindingFlags.Instance | BindingFlags.NonPublic);
      types[0] = parameters[0].ParameterType.GetElementType();
      types[1] = parameters[1].ParameterType.GetElementType();
    }
    else if (method1.ReturnType == typeof (void) && types.Length == 0)
    {
      method2 = typeof (LightLambda).GetMethod("RunVoid0", BindingFlags.Instance | BindingFlags.NonPublic);
    }
    else
    {
      for (int index = 0; index < parameters.Length; ++index)
      {
        types[index] = parameters[index].ParameterType;
        if (types[index].IsByRef)
          return (Func<LightLambda, Delegate>) null;
      }
      if (DelegateHelpers.MakeDelegate(types) == delegateType)
      {
        MethodInfo methodInfo = typeof (LightLambda).GetMethod($"Make{str}{(object) parameters.Length}", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(types);
        return LightLambda._runCache[delegateType] = (Func<LightLambda, Delegate>) methodInfo.CreateDelegate(typeof (Func<LightLambda, Delegate>));
      }
      method2 = typeof (LightLambda).GetMethod(str + (object) parameters.Length, BindingFlags.Instance | BindingFlags.NonPublic);
    }
    try
    {
      DynamicMethod dynamicMethod = new DynamicMethod("FastCtor", typeof (Delegate), new Type[1]
      {
        typeof (LightLambda)
      }, typeof (LightLambda), true);
      ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldftn, method2.IsGenericMethodDefinition ? method2.MakeGenericMethod(types) : method2);
      ilGenerator.Emit(OpCodes.Newobj, delegateType.GetConstructor(new Type[2]
      {
        typeof (object),
        typeof (IntPtr)
      }));
      ilGenerator.Emit(OpCodes.Ret);
      return LightLambda._runCache[delegateType] = (Func<LightLambda, Delegate>) ((MethodInfo) dynamicMethod).CreateDelegate(typeof (Func<LightLambda, Delegate>));
    }
    catch (SecurityException ex)
    {
    }
    MethodInfo targetMethod = method2.IsGenericMethodDefinition ? method2.MakeGenericMethod(types) : method2;
    return LightLambda._runCache[delegateType] = (Func<LightLambda, Delegate>) (lambda => targetMethod.CreateDelegate(delegateType, (object) lambda));
  }

  private Delegate CreateCustomDelegate(Type delegateType)
  {
    MethodInfo method = delegateType.GetMethod("Invoke");
    ParameterInfo[] parameters = method.GetParameters();
    ParameterExpression[] parameterExpressionArray = new ParameterExpression[parameters.Length];
    Expression[] expressionArray = new Expression[parameters.Length];
    for (int index = 0; index < parameters.Length; ++index)
    {
      ParameterExpression parameterExpression = Expression.Parameter(parameters[index].ParameterType, parameters[index].Name);
      parameterExpressionArray[index] = parameterExpression;
      expressionArray[index] = (Expression) Expression.Convert((Expression) parameterExpression, typeof (object));
    }
    NewArrayExpression newArrayExpression = Expression.NewArrayInit(typeof (object), expressionArray);
    UnaryExpression body = Expression.Convert((Expression) Expression.Call(Microsoft.Scripting.Ast.Utils.Constant((object) this), typeof (LightLambda).GetMethod("Run"), (Expression) newArrayExpression), method.ReturnType);
    return Expression.Lambda(delegateType, (Expression) body, parameterExpressionArray).Compile();
  }

  internal Delegate MakeDelegate(Type delegateType)
  {
    Func<LightLambda, Delegate> runDelegateCtor = LightLambda.GetRunDelegateCtor(delegateType);
    return runDelegateCtor != null ? runDelegateCtor(this) : this.CreateCustomDelegate(delegateType);
  }

  private bool TryGetCompiled()
  {
    if (this._delegateCreator.HasCompiled)
    {
      this._compiled = this._delegateCreator.CreateCompiledDelegate(this._closure);
      EventHandler<LightLambdaCompileEventArgs> compile = this.Compile;
      if (compile != null && this._delegateCreator.SameDelegateType)
        compile((object) this, new LightLambdaCompileEventArgs(this._compiled));
      return true;
    }
    if (this._compilationThreshold-- == 0)
    {
      if (this._interpreter.CompileSynchronously)
      {
        this._delegateCreator.Compile((object) null);
        return this.TryGetCompiled();
      }
      new Task(new Action<object>(this._delegateCreator.Compile), (object) null).Start();
    }
    return false;
  }

  private InterpretedFrame MakeFrame() => new InterpretedFrame(this._interpreter, this._closure);

  internal void RunVoidRef2<T0, T1>(ref T0 arg0, ref T1 arg1)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
    {
      ((ActionRef<T0, T1>) this._compiled)(ref arg0, ref arg1);
    }
    else
    {
      InterpretedFrame frame = this.MakeFrame();
      frame.Data[0] = (object) arg0;
      frame.Data[1] = (object) arg1;
      ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
      try
      {
        this._interpreter.Run(frame);
      }
      finally
      {
        frame.Leave(currentFrame);
        arg0 = (T0) frame.Data[0];
        arg1 = (T1) frame.Data[1];
      }
    }
  }

  public object Run(params object[] arguments)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
    {
      try
      {
        return this._compiled.DynamicInvoke(arguments);
      }
      catch (TargetInvocationException ex)
      {
        throw ExceptionHelpers.UpdateForRethrow(ex.InnerException);
      }
    }
    else
    {
      InterpretedFrame frame = this.MakeFrame();
      for (int index = 0; index < arguments.Length; ++index)
        frame.Data[index] = arguments[index];
      ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
      try
      {
        this._interpreter.Run(frame);
      }
      finally
      {
        frame.Leave(currentFrame);
      }
      return frame.Pop();
    }
  }

  internal TRet Run0<TRet>()
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
      return ((Func<TRet>) this._compiled)();
    InterpretedFrame frame = this.MakeFrame();
    ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
    try
    {
      this._interpreter.Run(frame);
    }
    finally
    {
      frame.Leave(currentFrame);
    }
    return (TRet) frame.Pop();
  }

  internal void RunVoid0()
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
    {
      ((Action) this._compiled)();
    }
    else
    {
      InterpretedFrame frame = this.MakeFrame();
      ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
      try
      {
        this._interpreter.Run(frame);
      }
      finally
      {
        frame.Leave(currentFrame);
      }
    }
  }

  internal static Delegate MakeRun0<TRet>(LightLambda lambda)
  {
    return (Delegate) new Func<TRet>(lambda.Run0<TRet>);
  }

  internal static Delegate MakeRunVoid0(LightLambda lambda)
  {
    return (Delegate) new Action(lambda.RunVoid0);
  }

  internal TRet Run1<T0, TRet>(T0 arg0)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
      return ((Func<T0, TRet>) this._compiled)(arg0);
    InterpretedFrame frame = this.MakeFrame();
    frame.Data[0] = (object) arg0;
    ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
    try
    {
      this._interpreter.Run(frame);
    }
    finally
    {
      frame.Leave(currentFrame);
    }
    return (TRet) frame.Pop();
  }

  internal void RunVoid1<T0>(T0 arg0)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
    {
      ((Action<T0>) this._compiled)(arg0);
    }
    else
    {
      InterpretedFrame frame = this.MakeFrame();
      frame.Data[0] = (object) arg0;
      ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
      try
      {
        this._interpreter.Run(frame);
      }
      finally
      {
        frame.Leave(currentFrame);
      }
    }
  }

  internal static Delegate MakeRun1<T0, TRet>(LightLambda lambda)
  {
    return (Delegate) new Func<T0, TRet>(lambda.Run1<T0, TRet>);
  }

  internal static Delegate MakeRunVoid1<T0>(LightLambda lambda)
  {
    return (Delegate) new Action<T0>(lambda.RunVoid1<T0>);
  }

  internal TRet Run2<T0, T1, TRet>(T0 arg0, T1 arg1)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
      return ((Func<T0, T1, TRet>) this._compiled)(arg0, arg1);
    InterpretedFrame frame = this.MakeFrame();
    frame.Data[0] = (object) arg0;
    frame.Data[1] = (object) arg1;
    ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
    try
    {
      this._interpreter.Run(frame);
    }
    finally
    {
      frame.Leave(currentFrame);
    }
    return (TRet) frame.Pop();
  }

  internal void RunVoid2<T0, T1>(T0 arg0, T1 arg1)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
    {
      ((Action<T0, T1>) this._compiled)(arg0, arg1);
    }
    else
    {
      InterpretedFrame frame = this.MakeFrame();
      frame.Data[0] = (object) arg0;
      frame.Data[1] = (object) arg1;
      ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
      try
      {
        this._interpreter.Run(frame);
      }
      finally
      {
        frame.Leave(currentFrame);
      }
    }
  }

  internal static Delegate MakeRun2<T0, T1, TRet>(LightLambda lambda)
  {
    return (Delegate) new Func<T0, T1, TRet>(lambda.Run2<T0, T1, TRet>);
  }

  internal static Delegate MakeRunVoid2<T0, T1>(LightLambda lambda)
  {
    return (Delegate) new Action<T0, T1>(lambda.RunVoid2<T0, T1>);
  }

  internal TRet Run3<T0, T1, T2, TRet>(T0 arg0, T1 arg1, T2 arg2)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
      return ((Func<T0, T1, T2, TRet>) this._compiled)(arg0, arg1, arg2);
    InterpretedFrame frame = this.MakeFrame();
    frame.Data[0] = (object) arg0;
    frame.Data[1] = (object) arg1;
    frame.Data[2] = (object) arg2;
    ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
    try
    {
      this._interpreter.Run(frame);
    }
    finally
    {
      frame.Leave(currentFrame);
    }
    return (TRet) frame.Pop();
  }

  internal void RunVoid3<T0, T1, T2>(T0 arg0, T1 arg1, T2 arg2)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
    {
      ((Action<T0, T1, T2>) this._compiled)(arg0, arg1, arg2);
    }
    else
    {
      InterpretedFrame frame = this.MakeFrame();
      frame.Data[0] = (object) arg0;
      frame.Data[1] = (object) arg1;
      frame.Data[2] = (object) arg2;
      ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
      try
      {
        this._interpreter.Run(frame);
      }
      finally
      {
        frame.Leave(currentFrame);
      }
    }
  }

  internal static Delegate MakeRun3<T0, T1, T2, TRet>(LightLambda lambda)
  {
    return (Delegate) new Func<T0, T1, T2, TRet>(lambda.Run3<T0, T1, T2, TRet>);
  }

  internal static Delegate MakeRunVoid3<T0, T1, T2>(LightLambda lambda)
  {
    return (Delegate) new Action<T0, T1, T2>(lambda.RunVoid3<T0, T1, T2>);
  }

  internal TRet Run4<T0, T1, T2, T3, TRet>(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
      return ((Func<T0, T1, T2, T3, TRet>) this._compiled)(arg0, arg1, arg2, arg3);
    InterpretedFrame frame = this.MakeFrame();
    frame.Data[0] = (object) arg0;
    frame.Data[1] = (object) arg1;
    frame.Data[2] = (object) arg2;
    frame.Data[3] = (object) arg3;
    ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
    try
    {
      this._interpreter.Run(frame);
    }
    finally
    {
      frame.Leave(currentFrame);
    }
    return (TRet) frame.Pop();
  }

  internal void RunVoid4<T0, T1, T2, T3>(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
    {
      ((Action<T0, T1, T2, T3>) this._compiled)(arg0, arg1, arg2, arg3);
    }
    else
    {
      InterpretedFrame frame = this.MakeFrame();
      frame.Data[0] = (object) arg0;
      frame.Data[1] = (object) arg1;
      frame.Data[2] = (object) arg2;
      frame.Data[3] = (object) arg3;
      ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
      try
      {
        this._interpreter.Run(frame);
      }
      finally
      {
        frame.Leave(currentFrame);
      }
    }
  }

  internal static Delegate MakeRun4<T0, T1, T2, T3, TRet>(LightLambda lambda)
  {
    return (Delegate) new Func<T0, T1, T2, T3, TRet>(lambda.Run4<T0, T1, T2, T3, TRet>);
  }

  internal static Delegate MakeRunVoid4<T0, T1, T2, T3>(LightLambda lambda)
  {
    return (Delegate) new Action<T0, T1, T2, T3>(lambda.RunVoid4<T0, T1, T2, T3>);
  }

  internal TRet Run5<T0, T1, T2, T3, T4, TRet>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
      return ((Func<T0, T1, T2, T3, T4, TRet>) this._compiled)(arg0, arg1, arg2, arg3, arg4);
    InterpretedFrame frame = this.MakeFrame();
    frame.Data[0] = (object) arg0;
    frame.Data[1] = (object) arg1;
    frame.Data[2] = (object) arg2;
    frame.Data[3] = (object) arg3;
    frame.Data[4] = (object) arg4;
    ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
    try
    {
      this._interpreter.Run(frame);
    }
    finally
    {
      frame.Leave(currentFrame);
    }
    return (TRet) frame.Pop();
  }

  internal void RunVoid5<T0, T1, T2, T3, T4>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
    {
      ((Action<T0, T1, T2, T3, T4>) this._compiled)(arg0, arg1, arg2, arg3, arg4);
    }
    else
    {
      InterpretedFrame frame = this.MakeFrame();
      frame.Data[0] = (object) arg0;
      frame.Data[1] = (object) arg1;
      frame.Data[2] = (object) arg2;
      frame.Data[3] = (object) arg3;
      frame.Data[4] = (object) arg4;
      ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
      try
      {
        this._interpreter.Run(frame);
      }
      finally
      {
        frame.Leave(currentFrame);
      }
    }
  }

  internal static Delegate MakeRun5<T0, T1, T2, T3, T4, TRet>(LightLambda lambda)
  {
    return (Delegate) new Func<T0, T1, T2, T3, T4, TRet>(lambda.Run5<T0, T1, T2, T3, T4, TRet>);
  }

  internal static Delegate MakeRunVoid5<T0, T1, T2, T3, T4>(LightLambda lambda)
  {
    return (Delegate) new Action<T0, T1, T2, T3, T4>(lambda.RunVoid5<T0, T1, T2, T3, T4>);
  }

  internal TRet Run6<T0, T1, T2, T3, T4, T5, TRet>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
      return ((Func<T0, T1, T2, T3, T4, T5, TRet>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5);
    InterpretedFrame frame = this.MakeFrame();
    frame.Data[0] = (object) arg0;
    frame.Data[1] = (object) arg1;
    frame.Data[2] = (object) arg2;
    frame.Data[3] = (object) arg3;
    frame.Data[4] = (object) arg4;
    frame.Data[5] = (object) arg5;
    ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
    try
    {
      this._interpreter.Run(frame);
    }
    finally
    {
      frame.Leave(currentFrame);
    }
    return (TRet) frame.Pop();
  }

  internal void RunVoid6<T0, T1, T2, T3, T4, T5>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
    {
      ((Action<T0, T1, T2, T3, T4, T5>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5);
    }
    else
    {
      InterpretedFrame frame = this.MakeFrame();
      frame.Data[0] = (object) arg0;
      frame.Data[1] = (object) arg1;
      frame.Data[2] = (object) arg2;
      frame.Data[3] = (object) arg3;
      frame.Data[4] = (object) arg4;
      frame.Data[5] = (object) arg5;
      ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
      try
      {
        this._interpreter.Run(frame);
      }
      finally
      {
        frame.Leave(currentFrame);
      }
    }
  }

  internal static Delegate MakeRun6<T0, T1, T2, T3, T4, T5, TRet>(LightLambda lambda)
  {
    return (Delegate) new Func<T0, T1, T2, T3, T4, T5, TRet>(lambda.Run6<T0, T1, T2, T3, T4, T5, TRet>);
  }

  internal static Delegate MakeRunVoid6<T0, T1, T2, T3, T4, T5>(LightLambda lambda)
  {
    return (Delegate) new Action<T0, T1, T2, T3, T4, T5>(lambda.RunVoid6<T0, T1, T2, T3, T4, T5>);
  }

  internal TRet Run7<T0, T1, T2, T3, T4, T5, T6, TRet>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
      return ((Func<T0, T1, T2, T3, T4, T5, T6, TRet>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5, arg6);
    InterpretedFrame frame = this.MakeFrame();
    frame.Data[0] = (object) arg0;
    frame.Data[1] = (object) arg1;
    frame.Data[2] = (object) arg2;
    frame.Data[3] = (object) arg3;
    frame.Data[4] = (object) arg4;
    frame.Data[5] = (object) arg5;
    frame.Data[6] = (object) arg6;
    ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
    try
    {
      this._interpreter.Run(frame);
    }
    finally
    {
      frame.Leave(currentFrame);
    }
    return (TRet) frame.Pop();
  }

  internal void RunVoid7<T0, T1, T2, T3, T4, T5, T6>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
    {
      ((Action<T0, T1, T2, T3, T4, T5, T6>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5, arg6);
    }
    else
    {
      InterpretedFrame frame = this.MakeFrame();
      frame.Data[0] = (object) arg0;
      frame.Data[1] = (object) arg1;
      frame.Data[2] = (object) arg2;
      frame.Data[3] = (object) arg3;
      frame.Data[4] = (object) arg4;
      frame.Data[5] = (object) arg5;
      frame.Data[6] = (object) arg6;
      ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
      try
      {
        this._interpreter.Run(frame);
      }
      finally
      {
        frame.Leave(currentFrame);
      }
    }
  }

  internal static Delegate MakeRun7<T0, T1, T2, T3, T4, T5, T6, TRet>(LightLambda lambda)
  {
    return (Delegate) new Func<T0, T1, T2, T3, T4, T5, T6, TRet>(lambda.Run7<T0, T1, T2, T3, T4, T5, T6, TRet>);
  }

  internal static Delegate MakeRunVoid7<T0, T1, T2, T3, T4, T5, T6>(LightLambda lambda)
  {
    return (Delegate) new Action<T0, T1, T2, T3, T4, T5, T6>(lambda.RunVoid7<T0, T1, T2, T3, T4, T5, T6>);
  }

  internal TRet Run8<T0, T1, T2, T3, T4, T5, T6, T7, TRet>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
      return ((Func<T0, T1, T2, T3, T4, T5, T6, T7, TRet>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    InterpretedFrame frame = this.MakeFrame();
    frame.Data[0] = (object) arg0;
    frame.Data[1] = (object) arg1;
    frame.Data[2] = (object) arg2;
    frame.Data[3] = (object) arg3;
    frame.Data[4] = (object) arg4;
    frame.Data[5] = (object) arg5;
    frame.Data[6] = (object) arg6;
    frame.Data[7] = (object) arg7;
    ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
    try
    {
      this._interpreter.Run(frame);
    }
    finally
    {
      frame.Leave(currentFrame);
    }
    return (TRet) frame.Pop();
  }

  internal void RunVoid8<T0, T1, T2, T3, T4, T5, T6, T7>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
    {
      ((Action<T0, T1, T2, T3, T4, T5, T6, T7>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }
    else
    {
      InterpretedFrame frame = this.MakeFrame();
      frame.Data[0] = (object) arg0;
      frame.Data[1] = (object) arg1;
      frame.Data[2] = (object) arg2;
      frame.Data[3] = (object) arg3;
      frame.Data[4] = (object) arg4;
      frame.Data[5] = (object) arg5;
      frame.Data[6] = (object) arg6;
      frame.Data[7] = (object) arg7;
      ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
      try
      {
        this._interpreter.Run(frame);
      }
      finally
      {
        frame.Leave(currentFrame);
      }
    }
  }

  internal static Delegate MakeRun8<T0, T1, T2, T3, T4, T5, T6, T7, TRet>(LightLambda lambda)
  {
    return (Delegate) new Func<T0, T1, T2, T3, T4, T5, T6, T7, TRet>(lambda.Run8<T0, T1, T2, T3, T4, T5, T6, T7, TRet>);
  }

  internal static Delegate MakeRunVoid8<T0, T1, T2, T3, T4, T5, T6, T7>(LightLambda lambda)
  {
    return (Delegate) new Action<T0, T1, T2, T3, T4, T5, T6, T7>(lambda.RunVoid8<T0, T1, T2, T3, T4, T5, T6, T7>);
  }

  internal TRet Run9<T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
      return ((Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    InterpretedFrame frame = this.MakeFrame();
    frame.Data[0] = (object) arg0;
    frame.Data[1] = (object) arg1;
    frame.Data[2] = (object) arg2;
    frame.Data[3] = (object) arg3;
    frame.Data[4] = (object) arg4;
    frame.Data[5] = (object) arg5;
    frame.Data[6] = (object) arg6;
    frame.Data[7] = (object) arg7;
    frame.Data[8] = (object) arg8;
    ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
    try
    {
      this._interpreter.Run(frame);
    }
    finally
    {
      frame.Leave(currentFrame);
    }
    return (TRet) frame.Pop();
  }

  internal void RunVoid9<T0, T1, T2, T3, T4, T5, T6, T7, T8>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
    {
      ((Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    }
    else
    {
      InterpretedFrame frame = this.MakeFrame();
      frame.Data[0] = (object) arg0;
      frame.Data[1] = (object) arg1;
      frame.Data[2] = (object) arg2;
      frame.Data[3] = (object) arg3;
      frame.Data[4] = (object) arg4;
      frame.Data[5] = (object) arg5;
      frame.Data[6] = (object) arg6;
      frame.Data[7] = (object) arg7;
      frame.Data[8] = (object) arg8;
      ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
      try
      {
        this._interpreter.Run(frame);
      }
      finally
      {
        frame.Leave(currentFrame);
      }
    }
  }

  internal static Delegate MakeRun9<T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>(LightLambda lambda)
  {
    return (Delegate) new Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>(lambda.Run9<T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>);
  }

  internal static Delegate MakeRunVoid9<T0, T1, T2, T3, T4, T5, T6, T7, T8>(LightLambda lambda)
  {
    return (Delegate) new Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>(lambda.RunVoid9<T0, T1, T2, T3, T4, T5, T6, T7, T8>);
  }

  internal TRet Run10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8,
    T9 arg9)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
      return ((Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
    InterpretedFrame frame = this.MakeFrame();
    frame.Data[0] = (object) arg0;
    frame.Data[1] = (object) arg1;
    frame.Data[2] = (object) arg2;
    frame.Data[3] = (object) arg3;
    frame.Data[4] = (object) arg4;
    frame.Data[5] = (object) arg5;
    frame.Data[6] = (object) arg6;
    frame.Data[7] = (object) arg7;
    frame.Data[8] = (object) arg8;
    frame.Data[9] = (object) arg9;
    ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
    try
    {
      this._interpreter.Run(frame);
    }
    finally
    {
      frame.Leave(currentFrame);
    }
    return (TRet) frame.Pop();
  }

  internal void RunVoid10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8,
    T9 arg9)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
    {
      ((Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
    }
    else
    {
      InterpretedFrame frame = this.MakeFrame();
      frame.Data[0] = (object) arg0;
      frame.Data[1] = (object) arg1;
      frame.Data[2] = (object) arg2;
      frame.Data[3] = (object) arg3;
      frame.Data[4] = (object) arg4;
      frame.Data[5] = (object) arg5;
      frame.Data[6] = (object) arg6;
      frame.Data[7] = (object) arg7;
      frame.Data[8] = (object) arg8;
      frame.Data[9] = (object) arg9;
      ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
      try
      {
        this._interpreter.Run(frame);
      }
      finally
      {
        frame.Leave(currentFrame);
      }
    }
  }

  internal static Delegate MakeRun10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>(
    LightLambda lambda)
  {
    return (Delegate) new Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>(lambda.Run10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>);
  }

  internal static Delegate MakeRunVoid10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(LightLambda lambda)
  {
    return (Delegate) new Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(lambda.RunVoid10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>);
  }

  internal TRet Run11<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8,
    T9 arg9,
    T10 arg10)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
      return ((Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
    InterpretedFrame frame = this.MakeFrame();
    frame.Data[0] = (object) arg0;
    frame.Data[1] = (object) arg1;
    frame.Data[2] = (object) arg2;
    frame.Data[3] = (object) arg3;
    frame.Data[4] = (object) arg4;
    frame.Data[5] = (object) arg5;
    frame.Data[6] = (object) arg6;
    frame.Data[7] = (object) arg7;
    frame.Data[8] = (object) arg8;
    frame.Data[9] = (object) arg9;
    frame.Data[10] = (object) arg10;
    ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
    try
    {
      this._interpreter.Run(frame);
    }
    finally
    {
      frame.Leave(currentFrame);
    }
    return (TRet) frame.Pop();
  }

  internal void RunVoid11<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8,
    T9 arg9,
    T10 arg10)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
    {
      ((Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
    }
    else
    {
      InterpretedFrame frame = this.MakeFrame();
      frame.Data[0] = (object) arg0;
      frame.Data[1] = (object) arg1;
      frame.Data[2] = (object) arg2;
      frame.Data[3] = (object) arg3;
      frame.Data[4] = (object) arg4;
      frame.Data[5] = (object) arg5;
      frame.Data[6] = (object) arg6;
      frame.Data[7] = (object) arg7;
      frame.Data[8] = (object) arg8;
      frame.Data[9] = (object) arg9;
      frame.Data[10] = (object) arg10;
      ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
      try
      {
        this._interpreter.Run(frame);
      }
      finally
      {
        frame.Leave(currentFrame);
      }
    }
  }

  internal static Delegate MakeRun11<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet>(
    LightLambda lambda)
  {
    return (Delegate) new Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet>(lambda.Run11<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet>);
  }

  internal static Delegate MakeRunVoid11<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
    LightLambda lambda)
  {
    return (Delegate) new Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(lambda.RunVoid11<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>);
  }

  internal TRet Run12<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRet>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8,
    T9 arg9,
    T10 arg10,
    T11 arg11)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
      return ((Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRet>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
    InterpretedFrame frame = this.MakeFrame();
    frame.Data[0] = (object) arg0;
    frame.Data[1] = (object) arg1;
    frame.Data[2] = (object) arg2;
    frame.Data[3] = (object) arg3;
    frame.Data[4] = (object) arg4;
    frame.Data[5] = (object) arg5;
    frame.Data[6] = (object) arg6;
    frame.Data[7] = (object) arg7;
    frame.Data[8] = (object) arg8;
    frame.Data[9] = (object) arg9;
    frame.Data[10] = (object) arg10;
    frame.Data[11] = (object) arg11;
    ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
    try
    {
      this._interpreter.Run(frame);
    }
    finally
    {
      frame.Leave(currentFrame);
    }
    return (TRet) frame.Pop();
  }

  internal void RunVoid12<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8,
    T9 arg9,
    T10 arg10,
    T11 arg11)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
    {
      ((Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
    }
    else
    {
      InterpretedFrame frame = this.MakeFrame();
      frame.Data[0] = (object) arg0;
      frame.Data[1] = (object) arg1;
      frame.Data[2] = (object) arg2;
      frame.Data[3] = (object) arg3;
      frame.Data[4] = (object) arg4;
      frame.Data[5] = (object) arg5;
      frame.Data[6] = (object) arg6;
      frame.Data[7] = (object) arg7;
      frame.Data[8] = (object) arg8;
      frame.Data[9] = (object) arg9;
      frame.Data[10] = (object) arg10;
      frame.Data[11] = (object) arg11;
      ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
      try
      {
        this._interpreter.Run(frame);
      }
      finally
      {
        frame.Leave(currentFrame);
      }
    }
  }

  internal static Delegate MakeRun12<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRet>(
    LightLambda lambda)
  {
    return (Delegate) new Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRet>(lambda.Run12<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRet>);
  }

  internal static Delegate MakeRunVoid12<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
    LightLambda lambda)
  {
    return (Delegate) new Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(lambda.RunVoid12<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>);
  }

  internal TRet Run13<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8,
    T9 arg9,
    T10 arg10,
    T11 arg11,
    T12 arg12)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
      return ((Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
    InterpretedFrame frame = this.MakeFrame();
    frame.Data[0] = (object) arg0;
    frame.Data[1] = (object) arg1;
    frame.Data[2] = (object) arg2;
    frame.Data[3] = (object) arg3;
    frame.Data[4] = (object) arg4;
    frame.Data[5] = (object) arg5;
    frame.Data[6] = (object) arg6;
    frame.Data[7] = (object) arg7;
    frame.Data[8] = (object) arg8;
    frame.Data[9] = (object) arg9;
    frame.Data[10] = (object) arg10;
    frame.Data[11] = (object) arg11;
    frame.Data[12] = (object) arg12;
    ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
    try
    {
      this._interpreter.Run(frame);
    }
    finally
    {
      frame.Leave(currentFrame);
    }
    return (TRet) frame.Pop();
  }

  internal void RunVoid13<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8,
    T9 arg9,
    T10 arg10,
    T11 arg11,
    T12 arg12)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
    {
      ((Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
    }
    else
    {
      InterpretedFrame frame = this.MakeFrame();
      frame.Data[0] = (object) arg0;
      frame.Data[1] = (object) arg1;
      frame.Data[2] = (object) arg2;
      frame.Data[3] = (object) arg3;
      frame.Data[4] = (object) arg4;
      frame.Data[5] = (object) arg5;
      frame.Data[6] = (object) arg6;
      frame.Data[7] = (object) arg7;
      frame.Data[8] = (object) arg8;
      frame.Data[9] = (object) arg9;
      frame.Data[10] = (object) arg10;
      frame.Data[11] = (object) arg11;
      frame.Data[12] = (object) arg12;
      ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
      try
      {
        this._interpreter.Run(frame);
      }
      finally
      {
        frame.Leave(currentFrame);
      }
    }
  }

  internal static Delegate MakeRun13<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet>(
    LightLambda lambda)
  {
    return (Delegate) new Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet>(lambda.Run13<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet>);
  }

  internal static Delegate MakeRunVoid13<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
    LightLambda lambda)
  {
    return (Delegate) new Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(lambda.RunVoid13<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>);
  }

  internal TRet Run14<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRet>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8,
    T9 arg9,
    T10 arg10,
    T11 arg11,
    T12 arg12,
    T13 arg13)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
      return ((Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRet>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
    InterpretedFrame frame = this.MakeFrame();
    frame.Data[0] = (object) arg0;
    frame.Data[1] = (object) arg1;
    frame.Data[2] = (object) arg2;
    frame.Data[3] = (object) arg3;
    frame.Data[4] = (object) arg4;
    frame.Data[5] = (object) arg5;
    frame.Data[6] = (object) arg6;
    frame.Data[7] = (object) arg7;
    frame.Data[8] = (object) arg8;
    frame.Data[9] = (object) arg9;
    frame.Data[10] = (object) arg10;
    frame.Data[11] = (object) arg11;
    frame.Data[12] = (object) arg12;
    frame.Data[13] = (object) arg13;
    ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
    try
    {
      this._interpreter.Run(frame);
    }
    finally
    {
      frame.Leave(currentFrame);
    }
    return (TRet) frame.Pop();
  }

  internal void RunVoid14<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8,
    T9 arg9,
    T10 arg10,
    T11 arg11,
    T12 arg12,
    T13 arg13)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
    {
      ((Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
    }
    else
    {
      InterpretedFrame frame = this.MakeFrame();
      frame.Data[0] = (object) arg0;
      frame.Data[1] = (object) arg1;
      frame.Data[2] = (object) arg2;
      frame.Data[3] = (object) arg3;
      frame.Data[4] = (object) arg4;
      frame.Data[5] = (object) arg5;
      frame.Data[6] = (object) arg6;
      frame.Data[7] = (object) arg7;
      frame.Data[8] = (object) arg8;
      frame.Data[9] = (object) arg9;
      frame.Data[10] = (object) arg10;
      frame.Data[11] = (object) arg11;
      frame.Data[12] = (object) arg12;
      frame.Data[13] = (object) arg13;
      ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
      try
      {
        this._interpreter.Run(frame);
      }
      finally
      {
        frame.Leave(currentFrame);
      }
    }
  }

  internal static Delegate MakeRun14<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRet>(
    LightLambda lambda)
  {
    return (Delegate) new Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRet>(lambda.Run14<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRet>);
  }

  internal static Delegate MakeRunVoid14<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
    LightLambda lambda)
  {
    return (Delegate) new Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(lambda.RunVoid14<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>);
  }

  internal TRet Run15<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRet>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8,
    T9 arg9,
    T10 arg10,
    T11 arg11,
    T12 arg12,
    T13 arg13,
    T14 arg14)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
      return ((Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRet>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
    InterpretedFrame frame = this.MakeFrame();
    frame.Data[0] = (object) arg0;
    frame.Data[1] = (object) arg1;
    frame.Data[2] = (object) arg2;
    frame.Data[3] = (object) arg3;
    frame.Data[4] = (object) arg4;
    frame.Data[5] = (object) arg5;
    frame.Data[6] = (object) arg6;
    frame.Data[7] = (object) arg7;
    frame.Data[8] = (object) arg8;
    frame.Data[9] = (object) arg9;
    frame.Data[10] = (object) arg10;
    frame.Data[11] = (object) arg11;
    frame.Data[12] = (object) arg12;
    frame.Data[13] = (object) arg13;
    frame.Data[14] = (object) arg14;
    ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
    try
    {
      this._interpreter.Run(frame);
    }
    finally
    {
      frame.Leave(currentFrame);
    }
    return (TRet) frame.Pop();
  }

  internal void RunVoid15<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8,
    T9 arg9,
    T10 arg10,
    T11 arg11,
    T12 arg12,
    T13 arg13,
    T14 arg14)
  {
    if ((object) this._compiled != null || this.TryGetCompiled())
    {
      ((Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>) this._compiled)(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
    }
    else
    {
      InterpretedFrame frame = this.MakeFrame();
      frame.Data[0] = (object) arg0;
      frame.Data[1] = (object) arg1;
      frame.Data[2] = (object) arg2;
      frame.Data[3] = (object) arg3;
      frame.Data[4] = (object) arg4;
      frame.Data[5] = (object) arg5;
      frame.Data[6] = (object) arg6;
      frame.Data[7] = (object) arg7;
      frame.Data[8] = (object) arg8;
      frame.Data[9] = (object) arg9;
      frame.Data[10] = (object) arg10;
      frame.Data[11] = (object) arg11;
      frame.Data[12] = (object) arg12;
      frame.Data[13] = (object) arg13;
      frame.Data[14] = (object) arg14;
      ThreadLocal<InterpretedFrame>.StorageInfo currentFrame = frame.Enter();
      try
      {
        this._interpreter.Run(frame);
      }
      finally
      {
        frame.Leave(currentFrame);
      }
    }
  }

  internal static Delegate MakeRun15<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRet>(
    LightLambda lambda)
  {
    return (Delegate) new Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRet>(lambda.Run15<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRet>);
  }

  internal static Delegate MakeRunVoid15<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
    LightLambda lambda)
  {
    return (Delegate) new Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(lambda.RunVoid15<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>);
  }
}
