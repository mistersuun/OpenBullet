// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.CompilationMode
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler.Ast;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Ast;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler;

[Serializable]
internal abstract class CompilationMode
{
  public static readonly CompilationMode ToDisk = (CompilationMode) new ToDiskCompilationMode();
  public static readonly CompilationMode Uncollectable = (CompilationMode) new UncollectableCompilationMode();
  public static readonly CompilationMode Collectable = (CompilationMode) new CollectableCompilationMode();
  public static readonly CompilationMode Lookup = (CompilationMode) new LookupCompilationMode();

  public virtual ScriptCode MakeScriptCode(PythonAst ast)
  {
    return (ScriptCode) new RuntimeScriptCode(ast, ast.ModuleContext.GlobalContext);
  }

  public virtual System.Linq.Expressions.Expression GetConstant(object value)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant(value);
  }

  public virtual Type GetConstantType(object value)
  {
    return value == null ? typeof (object) : value.GetType();
  }

  public virtual void PrepareScope(
    PythonAst ast,
    ReadOnlyCollectionBuilder<ParameterExpression> locals,
    List<System.Linq.Expressions.Expression> init)
  {
  }

  public virtual Type DelegateType => typeof (Expression<LookupCompilationDelegate>);

  public virtual CompilationMode.ConstantInfo GetContext() => (CompilationMode.ConstantInfo) null;

  public virtual void PublishContext(
    CodeContext codeContext,
    CompilationMode.ConstantInfo _contextInfo)
  {
  }

  public System.Linq.Expressions.Expression Dynamic(
    DynamicMetaObjectBinder binder,
    Type retType,
    System.Linq.Expressions.Expression arg0)
  {
    if (retType == typeof (object))
      return (System.Linq.Expressions.Expression) new PythonDynamicExpression1((CallSiteBinder) binder, this, arg0);
    return retType == typeof (bool) ? (System.Linq.Expressions.Expression) new PythonDynamicExpression1<bool>((CallSiteBinder) binder, this, arg0) : this.ReduceDynamic(binder, retType, arg0);
  }

  public System.Linq.Expressions.Expression Dynamic(
    DynamicMetaObjectBinder binder,
    Type retType,
    System.Linq.Expressions.Expression arg0,
    System.Linq.Expressions.Expression arg1)
  {
    if (retType == typeof (object))
      return (System.Linq.Expressions.Expression) new PythonDynamicExpression2((CallSiteBinder) binder, this, arg0, arg1);
    return retType == typeof (bool) ? (System.Linq.Expressions.Expression) new PythonDynamicExpression2<bool>((CallSiteBinder) binder, this, arg0, arg1) : this.ReduceDynamic(binder, retType, arg0, arg1);
  }

  public System.Linq.Expressions.Expression Dynamic(
    DynamicMetaObjectBinder binder,
    Type retType,
    System.Linq.Expressions.Expression arg0,
    System.Linq.Expressions.Expression arg1,
    System.Linq.Expressions.Expression arg2)
  {
    return retType == typeof (object) ? (System.Linq.Expressions.Expression) new PythonDynamicExpression3((CallSiteBinder) binder, this, arg0, arg1, arg2) : this.ReduceDynamic(binder, retType, arg0, arg1, arg2);
  }

  public System.Linq.Expressions.Expression Dynamic(
    DynamicMetaObjectBinder binder,
    Type retType,
    System.Linq.Expressions.Expression arg0,
    System.Linq.Expressions.Expression arg1,
    System.Linq.Expressions.Expression arg2,
    System.Linq.Expressions.Expression arg3)
  {
    return retType == typeof (object) ? (System.Linq.Expressions.Expression) new PythonDynamicExpression4((CallSiteBinder) binder, this, arg0, arg1, arg2, arg3) : this.ReduceDynamic(binder, retType, arg0, arg1, arg2, arg3);
  }

  public System.Linq.Expressions.Expression Dynamic(
    DynamicMetaObjectBinder binder,
    Type retType,
    System.Linq.Expressions.Expression[] args)
  {
    switch (args.Length)
    {
      case 1:
        return this.Dynamic(binder, retType, args[0]);
      case 2:
        return this.Dynamic(binder, retType, args[0], args[1]);
      case 3:
        return this.Dynamic(binder, retType, args[0], args[1], args[2]);
      case 4:
        return this.Dynamic(binder, retType, args[0], args[1], args[2], args[3]);
      default:
        return retType == typeof (object) ? (System.Linq.Expressions.Expression) new PythonDynamicExpressionN((CallSiteBinder) binder, this, (IList<System.Linq.Expressions.Expression>) args) : this.ReduceDynamic(binder, retType, args);
    }
  }

  public virtual System.Linq.Expressions.Expression ReduceDynamic(
    DynamicMetaObjectBinder binder,
    Type retType,
    System.Linq.Expressions.Expression arg0)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Dynamic((CallSiteBinder) binder, retType, arg0);
  }

  public virtual System.Linq.Expressions.Expression ReduceDynamic(
    DynamicMetaObjectBinder binder,
    Type retType,
    System.Linq.Expressions.Expression arg0,
    System.Linq.Expressions.Expression arg1)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Dynamic((CallSiteBinder) binder, retType, arg0, arg1);
  }

  public virtual System.Linq.Expressions.Expression ReduceDynamic(
    DynamicMetaObjectBinder binder,
    Type retType,
    System.Linq.Expressions.Expression arg0,
    System.Linq.Expressions.Expression arg1,
    System.Linq.Expressions.Expression arg2)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Dynamic((CallSiteBinder) binder, retType, arg0, arg1, arg2);
  }

  public virtual System.Linq.Expressions.Expression ReduceDynamic(
    DynamicMetaObjectBinder binder,
    Type retType,
    System.Linq.Expressions.Expression arg0,
    System.Linq.Expressions.Expression arg1,
    System.Linq.Expressions.Expression arg2,
    System.Linq.Expressions.Expression arg3)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Dynamic((CallSiteBinder) binder, retType, arg0, arg1, arg2, arg3);
  }

  public virtual System.Linq.Expressions.Expression ReduceDynamic(
    DynamicMetaObjectBinder binder,
    Type retType,
    System.Linq.Expressions.Expression[] args)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Dynamic((CallSiteBinder) binder, retType, args);
  }

  public abstract System.Linq.Expressions.Expression GetGlobal(
    System.Linq.Expressions.Expression globalContext,
    int arrayIndex,
    PythonVariable variable,
    PythonGlobal global);

  public abstract LightLambdaExpression ReduceAst(PythonAst instance, string name);

  public class ConstantInfo
  {
    public readonly System.Linq.Expressions.Expression Expression;
    public readonly FieldInfo Field;
    public readonly int Offset;

    public ConstantInfo(System.Linq.Expressions.Expression expr, FieldInfo field, int offset)
    {
      this.Expression = expr;
      this.Field = field;
      this.Offset = offset;
    }
  }

  public abstract class SiteInfo : CompilationMode.ConstantInfo
  {
    public readonly DynamicMetaObjectBinder Binder;
    public readonly Type DelegateType;
    protected Type _siteType;

    public Type SiteType
    {
      get
      {
        if (this._siteType != (Type) null)
          this._siteType = typeof (CallSite<>).MakeGenericType(this.DelegateType);
        return this._siteType;
      }
    }

    public SiteInfo(
      DynamicMetaObjectBinder binder,
      System.Linq.Expressions.Expression expr,
      FieldInfo field,
      int index,
      Type delegateType)
      : base(expr, field, index)
    {
      this.Binder = binder;
      this.DelegateType = delegateType;
    }

    public SiteInfo(
      DynamicMetaObjectBinder binder,
      System.Linq.Expressions.Expression expr,
      FieldInfo field,
      int index,
      Type delegateType,
      Type siteType)
      : this(binder, expr, field, index, delegateType)
    {
      this._siteType = siteType;
    }

    public abstract CallSite MakeSite();
  }

  public class SiteInfoLarge(
    DynamicMetaObjectBinder binder,
    System.Linq.Expressions.Expression expr,
    FieldInfo field,
    int index,
    Type delegateType) : CompilationMode.SiteInfo(binder, expr, field, index, delegateType)
  {
    public override CallSite MakeSite()
    {
      return CallSite.Create(this.DelegateType, (CallSiteBinder) this.Binder);
    }
  }

  public class SiteInfo<T>(
    DynamicMetaObjectBinder binder,
    System.Linq.Expressions.Expression expr,
    FieldInfo field,
    int index) : CompilationMode.SiteInfo(binder, expr, field, index, typeof (T), typeof (CallSite<T>))
    where T : class
  {
    public override CallSite MakeSite()
    {
      return (CallSite) CallSite<T>.Create((CallSiteBinder) this.Binder);
    }
  }
}
