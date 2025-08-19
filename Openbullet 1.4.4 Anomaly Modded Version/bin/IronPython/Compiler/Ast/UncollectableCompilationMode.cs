// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.UncollectableCompilationMode
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Interpreter;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler.Ast;

internal class UncollectableCompilationMode : CompilationMode
{
  private static readonly Dictionary<object, CompilationMode.ConstantInfo> _allConstants = new Dictionary<object, CompilationMode.ConstantInfo>();
  private static readonly Dictionary<Type, UncollectableCompilationMode.DelegateCache> _delegateCache = new Dictionary<Type, UncollectableCompilationMode.DelegateCache>();

  public override LightLambdaExpression ReduceAst(PythonAst instance, string name)
  {
    return (LightLambdaExpression) Microsoft.Scripting.Ast.Utils.LightLambda<Func<FunctionCode, object>>(typeof (object), Microsoft.Scripting.Ast.Utils.Convert(instance.ReduceWorker(), typeof (object)), name, (IList<ParameterExpression>) new ParameterExpression[1]
    {
      PythonAst._functionCode
    });
  }

  public override System.Linq.Expressions.Expression GetConstant(object value)
  {
    if (CompilerHelpers.CanEmitConstant(value, CompilerHelpers.GetType(value)) && !CompilerHelpers.GetType(value).IsValueType())
      return Microsoft.Scripting.Ast.Utils.Constant(value);
    CompilationMode.ConstantInfo info;
    lock (UncollectableCompilationMode._allConstants)
    {
      if (!UncollectableCompilationMode._allConstants.TryGetValue(value, out info))
      {
        UncollectableCompilationMode._allConstants[value] = info = UncollectableCompilationMode.NextConstant(UncollectableCompilationMode._allConstants.Count, value);
        UncollectableCompilationMode.PublishConstant(value, info);
      }
    }
    return info.Expression;
  }

  public override Type GetConstantType(object value)
  {
    return value == null || value.GetType().IsValueType() ? typeof (object) : value.GetType();
  }

  public override System.Linq.Expressions.Expression GetGlobal(
    System.Linq.Expressions.Expression globalContext,
    int arrayIndex,
    PythonVariable variable,
    PythonGlobal global)
  {
    lock (StorageData.Globals)
    {
      CompilationMode.ConstantInfo info = UncollectableCompilationMode.NextGlobal(0);
      StorageData.GlobalStorageType(StorageData.GlobalCount + 1);
      UncollectableCompilationMode.PublishWorker<PythonGlobal>(StorageData.GlobalCount, 20, info, global, StorageData.Globals);
      ++StorageData.GlobalCount;
      return (System.Linq.Expressions.Expression) new PythonGlobalVariableExpression(info.Expression, variable, global);
    }
  }

  public override Type DelegateType => typeof (Expression<Func<FunctionCode, object>>);

  public override CompilationMode.ConstantInfo GetContext()
  {
    lock (StorageData.Contexts)
    {
      int num1 = StorageData.ContextCount++;
      int num2 = num1 - 50;
      Type type = StorageData.ContextStorageType(num1);
      FieldInfo field;
      System.Linq.Expressions.Expression expression;
      if (num2 < 0)
      {
        field = type.GetField($"Context{num1 % 50:000}");
        expression = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field((System.Linq.Expressions.Expression) null, field);
      }
      else
      {
        field = typeof (StorageData).GetField("Contexts");
        expression = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.ArrayIndex((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field((System.Linq.Expressions.Expression) null, field), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) num2, typeof (int)));
      }
      return new CompilationMode.ConstantInfo((System.Linq.Expressions.Expression) new UncollectableCompilationMode.CodeContextExpression(expression), field, num1);
    }
  }

  private static CompilationMode.ConstantInfo NextConstant(int offset, object value)
  {
    return new CompilationMode.ConstantInfo((System.Linq.Expressions.Expression) new UncollectableCompilationMode.ConstantExpression(offset, value), (FieldInfo) null, offset);
  }

  private static CompilationMode.ConstantInfo NextGlobal(int offset)
  {
    return new CompilationMode.ConstantInfo((System.Linq.Expressions.Expression) new UncollectableCompilationMode.GlobalExpression(offset), (FieldInfo) null, offset);
  }

  public static CompilationMode.SiteInfo NextSite<T>(DynamicMetaObjectBinder binder) where T : class
  {
    lock (StorageData.SiteLockObj)
    {
      int index = SiteStorage<T>.SiteCount++;
      int num = index - 1500;
      Type type = SiteStorage<T>.SiteStorageType(index);
      FieldInfo field;
      System.Linq.Expressions.Expression expr;
      if (num < 0)
      {
        field = type.GetField($"Site{index % 50:000}");
        expr = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field((System.Linq.Expressions.Expression) null, field);
      }
      else
      {
        field = typeof (SiteStorage<T>).GetField("Sites");
        expr = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.ArrayIndex((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field((System.Linq.Expressions.Expression) null, field), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) num, typeof (int)));
      }
      return UncollectableCompilationMode.PublishSite((CompilationMode.SiteInfo) new CompilationMode.SiteInfo<T>(binder, expr, field, index));
    }
  }

  private static CompilationMode.SiteInfo NextSite(
    DynamicMetaObjectBinder binder,
    Type delegateType)
  {
    Type type1 = typeof (SiteStorage<>).MakeGenericType(delegateType);
    lock (StorageData.SiteLockObj)
    {
      int index = (int) type1.GetField("SiteCount").GetValue((object) null);
      type1.GetField("SiteCount").SetValue((object) null, (object) (index + 1));
      int num = index - 1500;
      Type type2 = (Type) type1.GetMethod("SiteStorageType").Invoke((object) null, new object[1]
      {
        (object) index
      });
      FieldInfo field;
      System.Linq.Expressions.Expression expr;
      if (num < 0)
      {
        field = type2.GetField($"Site{index % 50:000}");
        expr = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field((System.Linq.Expressions.Expression) null, field);
      }
      else
      {
        field = type1.GetField("Sites");
        expr = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.ArrayIndex((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field((System.Linq.Expressions.Expression) null, field), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) num, typeof (int)));
      }
      return UncollectableCompilationMode.PublishSite((CompilationMode.SiteInfo) new CompilationMode.SiteInfoLarge(binder, expr, field, index, delegateType));
    }
  }

  public override void PublishContext(
    CodeContext context,
    CompilationMode.ConstantInfo codeContextInfo)
  {
    int index = codeContextInfo.Offset - 50;
    if (index < 0)
    {
      codeContextInfo.Field.SetValue((object) null, (object) context);
    }
    else
    {
      lock (StorageData.Contexts)
        StorageData.Contexts[index] = context;
    }
    ((UncollectableCompilationMode.CodeContextExpression) codeContextInfo.Expression).Context = context;
  }

  private static void PublishConstant(object constant, CompilationMode.ConstantInfo info)
  {
    StorageData.ConstantStorageType(info.Offset);
    UncollectableCompilationMode.PublishWorker<object>(0, 5, info, constant, StorageData.Constants);
  }

  private static CompilationMode.SiteInfo PublishSite(CompilationMode.SiteInfo si)
  {
    int index = si.Offset - 1500;
    CallSite callSite = si.MakeSite();
    if (index < 0)
    {
      si.Field.SetValue((object) null, (object) callSite);
    }
    else
    {
      lock (StorageData.SiteLockObj)
        ((CallSite[]) si.Field.GetValue((object) null))[index] = callSite;
    }
    return si;
  }

  private static void PublishWorker<T>(
    int start,
    int nTypes,
    CompilationMode.ConstantInfo info,
    T value,
    T[] fallbackArray)
  {
    int index = start + info.Offset - nTypes * 50;
    ((UncollectableCompilationMode.ReducibleExpression) info.Expression).Start = start;
    if (index < 0)
      ((UncollectableCompilationMode.ReducibleExpression) info.Expression).FieldInfo.SetValue((object) null, (object) value);
    else
      fallbackArray[index] = value;
  }

  public override System.Linq.Expressions.Expression ReduceDynamic(
    DynamicMetaObjectBinder binder,
    Type retType,
    System.Linq.Expressions.Expression arg0)
  {
    UncollectableCompilationMode.DelegateCache delegateCache;
    lock (UncollectableCompilationMode._delegateCache)
    {
      delegateCache = UncollectableCompilationMode.DelegateCache.FirstCacheNode(arg0.Type).NextCacheNode(retType);
      if (delegateCache.DelegateType == (Type) null)
        delegateCache.MakeDelegateType(retType, arg0);
    }
    CompilationMode.SiteInfo siteInfo = delegateCache.NextSite(binder);
    return UncollectableCompilationMode.MakeDynamicExpression(binder, siteInfo.Expression, delegateCache.TargetField, delegateCache.InvokeMethod, arg0);
  }

  private static System.Linq.Expressions.Expression MakeDynamicExpression(
    DynamicMetaObjectBinder binder,
    System.Linq.Expressions.Expression expr,
    FieldInfo targetField,
    MethodInfo invokeMethod,
    System.Linq.Expressions.Expression arg0)
  {
    return (System.Linq.Expressions.Expression) new ReducableDynamicExpression((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field(expr, targetField), invokeMethod, expr, arg0), binder, (IList<System.Linq.Expressions.Expression>) new System.Linq.Expressions.Expression[1]
    {
      arg0
    });
  }

  public override System.Linq.Expressions.Expression ReduceDynamic(
    DynamicMetaObjectBinder binder,
    Type retType,
    System.Linq.Expressions.Expression arg0,
    System.Linq.Expressions.Expression arg1)
  {
    UncollectableCompilationMode.DelegateCache delegateCache;
    lock (UncollectableCompilationMode._delegateCache)
    {
      delegateCache = UncollectableCompilationMode.DelegateCache.FirstCacheNode(arg0.Type).NextCacheNode(arg1.Type).NextCacheNode(retType);
      if (delegateCache.DelegateType == (Type) null)
        delegateCache.MakeDelegateType(retType, arg0, arg1);
    }
    CompilationMode.SiteInfo siteInfo = delegateCache.NextSite(binder);
    return UncollectableCompilationMode.MakeDynamicExpression(binder, siteInfo.Expression, delegateCache.TargetField, delegateCache.InvokeMethod, arg0, arg1);
  }

  private static System.Linq.Expressions.Expression MakeDynamicExpression(
    DynamicMetaObjectBinder binder,
    System.Linq.Expressions.Expression expr,
    FieldInfo targetField,
    MethodInfo invokeMethod,
    System.Linq.Expressions.Expression arg0,
    System.Linq.Expressions.Expression arg1)
  {
    return (System.Linq.Expressions.Expression) new ReducableDynamicExpression((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field(expr, targetField), invokeMethod, expr, arg0, arg1), binder, (IList<System.Linq.Expressions.Expression>) new System.Linq.Expressions.Expression[2]
    {
      arg0,
      arg1
    });
  }

  public override System.Linq.Expressions.Expression ReduceDynamic(
    DynamicMetaObjectBinder binder,
    Type retType,
    System.Linq.Expressions.Expression arg0,
    System.Linq.Expressions.Expression arg1,
    System.Linq.Expressions.Expression arg2)
  {
    UncollectableCompilationMode.DelegateCache delegateCache;
    lock (UncollectableCompilationMode._delegateCache)
    {
      delegateCache = UncollectableCompilationMode.DelegateCache.FirstCacheNode(arg0.Type).NextCacheNode(arg1.Type).NextCacheNode(arg2.Type).NextCacheNode(retType);
      if (delegateCache.DelegateType == (Type) null)
        delegateCache.MakeDelegateType(retType, arg0, arg1, arg2);
    }
    CompilationMode.SiteInfo siteInfo = delegateCache.NextSite(binder);
    return UncollectableCompilationMode.MakeDynamicExpression(binder, siteInfo.Expression, delegateCache.TargetField, delegateCache.InvokeMethod, arg0, arg1, arg2);
  }

  private static System.Linq.Expressions.Expression MakeDynamicExpression(
    DynamicMetaObjectBinder binder,
    System.Linq.Expressions.Expression expr,
    FieldInfo targetField,
    MethodInfo invokeMethod,
    System.Linq.Expressions.Expression arg0,
    System.Linq.Expressions.Expression arg1,
    System.Linq.Expressions.Expression arg2)
  {
    return (System.Linq.Expressions.Expression) new ReducableDynamicExpression((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field(expr, targetField), invokeMethod, expr, arg0, arg1, arg2), binder, (IList<System.Linq.Expressions.Expression>) new System.Linq.Expressions.Expression[3]
    {
      arg0,
      arg1,
      arg2
    });
  }

  public override System.Linq.Expressions.Expression ReduceDynamic(
    DynamicMetaObjectBinder binder,
    Type retType,
    System.Linq.Expressions.Expression arg0,
    System.Linq.Expressions.Expression arg1,
    System.Linq.Expressions.Expression arg2,
    System.Linq.Expressions.Expression arg3)
  {
    UncollectableCompilationMode.DelegateCache delegateCache;
    lock (UncollectableCompilationMode._delegateCache)
    {
      delegateCache = UncollectableCompilationMode.DelegateCache.FirstCacheNode(arg0.Type).NextCacheNode(arg1.Type).NextCacheNode(arg2.Type).NextCacheNode(arg3.Type).NextCacheNode(retType);
      if (delegateCache.DelegateType == (Type) null)
        delegateCache.MakeDelegateType(retType, arg0, arg1, arg2, arg3);
    }
    CompilationMode.SiteInfo siteInfo = delegateCache.NextSite(binder);
    return UncollectableCompilationMode.MakeDynamicExpression(binder, siteInfo.Expression, delegateCache.TargetField, delegateCache.InvokeMethod, arg0, arg1, arg2, arg3);
  }

  private static System.Linq.Expressions.Expression MakeDynamicExpression(
    DynamicMetaObjectBinder binder,
    System.Linq.Expressions.Expression expr,
    FieldInfo targetField,
    MethodInfo invokeMethod,
    System.Linq.Expressions.Expression arg0,
    System.Linq.Expressions.Expression arg1,
    System.Linq.Expressions.Expression arg2,
    System.Linq.Expressions.Expression arg3)
  {
    return (System.Linq.Expressions.Expression) new ReducableDynamicExpression((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field(expr, targetField), invokeMethod, (IEnumerable<System.Linq.Expressions.Expression>) new ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression>(5)
    {
      expr,
      arg0,
      arg1,
      arg2,
      arg3
    }.ToReadOnlyCollection()), binder, (IList<System.Linq.Expressions.Expression>) new System.Linq.Expressions.Expression[4]
    {
      arg0,
      arg1,
      arg2,
      arg3
    });
  }

  public override System.Linq.Expressions.Expression ReduceDynamic(
    DynamicMetaObjectBinder binder,
    Type retType,
    System.Linq.Expressions.Expression[] args)
  {
    Type delegateType = UncollectableCompilationMode.GetDelegateType(retType, (IList<System.Linq.Expressions.Expression>) args);
    Type type = typeof (CallSite<>).MakeGenericType(delegateType);
    CompilationMode.SiteInfo siteInfo = UncollectableCompilationMode.NextSite(binder, delegateType);
    return UncollectableCompilationMode.MakeDynamicExpression(binder, siteInfo.Expression, type.GetField("Target"), delegateType.GetMethod("Invoke"), (IList<System.Linq.Expressions.Expression>) args);
  }

  private static System.Linq.Expressions.Expression MakeDynamicExpression(
    DynamicMetaObjectBinder binder,
    System.Linq.Expressions.Expression expr,
    FieldInfo targetField,
    MethodInfo invokeMethod,
    IList<System.Linq.Expressions.Expression> args)
  {
    ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression> collectionBuilder = new ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression>(args.Count + 1);
    collectionBuilder.Add(expr);
    foreach (System.Linq.Expressions.Expression expression in (IEnumerable<System.Linq.Expressions.Expression>) args)
      collectionBuilder.Add(expression);
    return (System.Linq.Expressions.Expression) new ReducableDynamicExpression((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field(expr, targetField), invokeMethod, (IEnumerable<System.Linq.Expressions.Expression>) collectionBuilder.ToReadOnlyCollection()), binder, args);
  }

  private static Type GetDelegateType(Type retType, IList<System.Linq.Expressions.Expression> args)
  {
    Type delegateType;
    if (retType != typeof (void))
    {
      Type[] types = new Type[args.Count + 2];
      types[0] = typeof (CallSite);
      for (int index = 0; index < args.Count; ++index)
        types[index + 1] = args[index].Type;
      types[types.Length - 1] = retType;
      Type type = UncollectableCompilationMode.GetFuncType(types);
      if ((object) type == null)
        type = PythonOps.MakeNewCustomDelegate(types);
      delegateType = type;
    }
    else
    {
      Type[] typeArray = new Type[args.Count + 1];
      typeArray[0] = typeof (CallSite);
      for (int index = 0; index < args.Count; ++index)
        typeArray[index + 1] = args[index].Type;
      Type type = UncollectableCompilationMode.GetActionType(typeArray);
      if ((object) type == null)
        type = PythonOps.MakeNewCustomDelegate(ArrayUtils.Append<Type>(typeArray, typeof (void)));
      delegateType = type;
    }
    return delegateType;
  }

  private static Type GetFuncType(Type[] types)
  {
    switch (types.Length)
    {
      case 1:
        return typeof (Func<>).MakeGenericType(types);
      case 2:
        return typeof (Func<,>).MakeGenericType(types);
      case 3:
        return typeof (Func<,,>).MakeGenericType(types);
      case 4:
        return typeof (Func<,,,>).MakeGenericType(types);
      case 5:
        return typeof (Func<,,,,>).MakeGenericType(types);
      case 6:
        return typeof (Func<,,,,,>).MakeGenericType(types);
      case 7:
        return typeof (Func<,,,,,,>).MakeGenericType(types);
      case 8:
        return typeof (Func<,,,,,,,>).MakeGenericType(types);
      case 9:
        return typeof (Func<,,,,,,,,>).MakeGenericType(types);
      case 10:
        return typeof (Func<,,,,,,,,,>).MakeGenericType(types);
      case 11:
        return typeof (Func<,,,,,,,,,,>).MakeGenericType(types);
      case 12:
        return typeof (Func<,,,,,,,,,,,>).MakeGenericType(types);
      case 13:
        return typeof (Func<,,,,,,,,,,,,>).MakeGenericType(types);
      case 14:
        return typeof (Func<,,,,,,,,,,,,,>).MakeGenericType(types);
      case 15:
        return typeof (Func<,,,,,,,,,,,,,,>).MakeGenericType(types);
      case 16 /*0x10*/:
        return typeof (Func<,,,,,,,,,,,,,,,>).MakeGenericType(types);
      case 17:
        return typeof (Func<,,,,,,,,,,,,,,,,>).MakeGenericType(types);
      default:
        return (Type) null;
    }
  }

  private static Type GetActionType(Type[] types)
  {
    switch (types.Length)
    {
      case 0:
        return typeof (Action);
      case 1:
        return typeof (Action<>).MakeGenericType(types);
      case 2:
        return typeof (Action<,>).MakeGenericType(types);
      case 3:
        return typeof (Action<,,>).MakeGenericType(types);
      case 4:
        return typeof (Action<,,,>).MakeGenericType(types);
      case 5:
        return typeof (Action<,,,,>).MakeGenericType(types);
      case 6:
        return typeof (Action<,,,,,>).MakeGenericType(types);
      case 7:
        return typeof (Action<,,,,,,>).MakeGenericType(types);
      case 8:
        return typeof (Action<,,,,,,,>).MakeGenericType(types);
      case 9:
        return typeof (Action<,,,,,,,,>).MakeGenericType(types);
      case 10:
        return typeof (Action<,,,,,,,,,>).MakeGenericType(types);
      case 11:
        return typeof (Action<,,,,,,,,,,>).MakeGenericType(types);
      case 12:
        return typeof (Action<,,,,,,,,,,,>).MakeGenericType(types);
      case 13:
        return typeof (Action<,,,,,,,,,,,,>).MakeGenericType(types);
      case 14:
        return typeof (Action<,,,,,,,,,,,,,>).MakeGenericType(types);
      case 15:
        return typeof (Action<,,,,,,,,,,,,,,>).MakeGenericType(types);
      case 16 /*0x10*/:
        return typeof (Action<,,,,,,,,,,,,,,,>).MakeGenericType(types);
      default:
        return (Type) null;
    }
  }

  private class CodeContextExpression : System.Linq.Expressions.Expression, IInstructionProvider
  {
    private readonly System.Linq.Expressions.Expression _expression;
    public CodeContext Context;

    public CodeContextExpression(System.Linq.Expressions.Expression expression)
    {
      this._expression = expression;
    }

    public override System.Linq.Expressions.Expression Reduce() => this._expression;

    public override bool CanReduce => true;

    public override Type Type => typeof (CodeContext);

    public override ExpressionType NodeType => ExpressionType.Extension;

    public void AddInstructions(LightCompiler compiler)
    {
      compiler.Instructions.EmitLoad((object) this.Context);
    }
  }

  private sealed class DelegateCache
  {
    public Type DelegateType;
    public Type SiteType;
    public Func<DynamicMetaObjectBinder, CompilationMode.SiteInfo> NextSite;
    public FieldInfo TargetField;
    public MethodInfo InvokeMethod;
    public Dictionary<Type, UncollectableCompilationMode.DelegateCache> TypeChain;

    public void MakeDelegateType(Type retType, params System.Linq.Expressions.Expression[] args)
    {
      this.DelegateType = UncollectableCompilationMode.GetDelegateType(retType, (IList<System.Linq.Expressions.Expression>) args);
      this.SiteType = typeof (CallSite<>).MakeGenericType(this.DelegateType);
      this.NextSite = (Func<DynamicMetaObjectBinder, CompilationMode.SiteInfo>) typeof (UncollectableCompilationMode).GetMethod("NextSite").MakeGenericMethod(this.DelegateType).CreateDelegate(typeof (Func<DynamicMetaObjectBinder, CompilationMode.SiteInfo>));
      this.TargetField = this.SiteType.GetField("Target");
      this.InvokeMethod = this.DelegateType.GetMethod("Invoke");
    }

    public static UncollectableCompilationMode.DelegateCache FirstCacheNode(Type argType)
    {
      UncollectableCompilationMode.DelegateCache delegateCache;
      if (!UncollectableCompilationMode._delegateCache.TryGetValue(argType, out delegateCache))
      {
        delegateCache = new UncollectableCompilationMode.DelegateCache();
        UncollectableCompilationMode._delegateCache[argType] = delegateCache;
      }
      return delegateCache;
    }

    public UncollectableCompilationMode.DelegateCache NextCacheNode(Type argType)
    {
      if (this.TypeChain == null)
        this.TypeChain = new Dictionary<Type, UncollectableCompilationMode.DelegateCache>();
      UncollectableCompilationMode.DelegateCache delegateCache;
      if (!this.TypeChain.TryGetValue(argType, out delegateCache))
      {
        delegateCache = new UncollectableCompilationMode.DelegateCache();
        this.TypeChain[argType] = delegateCache;
      }
      return delegateCache;
    }
  }

  internal abstract class ReducibleExpression : System.Linq.Expressions.Expression
  {
    private readonly int _offset;
    private int _start = -1;
    private FieldInfo _fieldInfo;

    public ReducibleExpression(int offset) => this._offset = offset;

    public abstract string Name { get; }

    public abstract int FieldCount { get; }

    public abstract override Type Type { get; }

    protected abstract Type GetStorageType(int index);

    public FieldInfo FieldInfo => this._fieldInfo;

    public int Start
    {
      get => this._start;
      set
      {
        this._start = value;
        int index = this._offset + this._start;
        Type storageType = this.GetStorageType(index);
        if (storageType != typeof (StorageData))
          this._fieldInfo = storageType.GetField(this.Name + $"{index % 50:000}");
        else
          this._fieldInfo = typeof (StorageData).GetField(this.Name + "s");
      }
    }

    public override System.Linq.Expressions.Expression Reduce()
    {
      int num = this._offset + this._start - this.FieldCount;
      return num < 0 ? (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field((System.Linq.Expressions.Expression) null, this._fieldInfo) : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.ArrayIndex((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field((System.Linq.Expressions.Expression) null, this._fieldInfo), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) num, typeof (int)));
    }

    public override ExpressionType NodeType => ExpressionType.Extension;

    protected override System.Linq.Expressions.Expression Accept(ExpressionVisitor visitor)
    {
      return (System.Linq.Expressions.Expression) this;
    }

    protected override System.Linq.Expressions.Expression VisitChildren(ExpressionVisitor visitor)
    {
      return (System.Linq.Expressions.Expression) this;
    }

    public override bool CanReduce => true;
  }

  internal sealed class ConstantExpression : UncollectableCompilationMode.ReducibleExpression
  {
    private object _value;

    public ConstantExpression(int offset, object value)
      : base(offset)
    {
      this._value = value;
    }

    public override string Name => "Constant";

    public override int FieldCount => 250;

    protected override Type GetStorageType(int index) => StorageData.ConstantStorageType(index);

    public override Type Type
    {
      get
      {
        Type type = this._value.GetType();
        return !type.IsValueType() ? type : typeof (object);
      }
    }

    public object Value => this._value;

    public override System.Linq.Expressions.Expression Reduce()
    {
      return this._value.GetType().IsValueType() ? base.Reduce() : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Convert(base.Reduce(), this._value.GetType());
    }
  }

  internal sealed class GlobalExpression(int offset) : 
    UncollectableCompilationMode.ReducibleExpression(offset)
  {
    public override string Name => "Global";

    public override int FieldCount => 1000;

    protected override Type GetStorageType(int index) => StorageData.GlobalStorageType(index);

    public override Type Type => typeof (PythonGlobal);
  }
}
