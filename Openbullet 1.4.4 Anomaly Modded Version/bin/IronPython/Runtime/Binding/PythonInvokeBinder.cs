// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.PythonInvokeBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.ComInterop;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class PythonInvokeBinder : 
  DynamicMetaObjectBinder,
  IPythonSite,
  IExpressionSerializable,
  ILightExceptionBinder
{
  private readonly PythonContext _context;
  private readonly CallSignature _signature;
  private PythonInvokeBinder.LightThrowBinder _lightThrowBinder;

  public PythonInvokeBinder(PythonContext context, CallSignature signature)
  {
    this._context = context;
    this._signature = signature;
  }

  public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
  {
    DynamicMetaObject context = target;
    DynamicMetaObject target1 = args[0];
    args = ArrayUtils.RemoveFirst<DynamicMetaObject>(args);
    return this.BindWorker(context, target1, args);
  }

  private DynamicMetaObject BindWorker(
    DynamicMetaObject context,
    DynamicMetaObject target,
    DynamicMetaObject[] args)
  {
    if (target is IPythonInvokable pythonInvokable)
      return pythonInvokable.Invoke(this, context.Expression, target, args);
    return target.Value is IDynamicMetaObjectProvider || ComBinder.CanComBind(target.Value) ? this.InvokeForeignObject(target, args) : this.Fallback(context.Expression, target, args);
  }

  public override T BindDelegate<T>(CallSite<T> site, object[] args)
  {
    if (args[1] is IFastInvokable fastInvokable)
    {
      FastBindResult<T> fastBindResult = fastInvokable.MakeInvokeBinding<T>(site, this, (CodeContext) args[0], ArrayUtils.ShiftLeft<object>(args, 2));
      if ((object) fastBindResult.Target != null)
      {
        if (fastBindResult.ShouldCache)
          this.CacheTarget<T>(fastBindResult.Target);
        return fastBindResult.Target;
      }
    }
    PythonType pythonType = args[1] as PythonType;
    T target = this.LightBind<T>(args, this.Context.Options.CompilationThreshold);
    this.CacheTarget<T>(target);
    return target;
  }

  internal DynamicMetaObject Fallback(
    Expression codeContext,
    DynamicMetaObject target,
    DynamicMetaObject[] args)
  {
    return target.NeedsDeferral() ? this.Defer(args) : PythonProtocol.Call((DynamicMetaObjectBinder) this, target, args) ?? this.Context.Binder.Create(this.Signature, target, args, codeContext) ?? this.Context.Binder.Call(this.Signature, (OverloadResolverFactory) new PythonOverloadResolverFactory(this.Context.Binder, codeContext), target, args);
  }

  public override int GetHashCode()
  {
    return this._signature.GetHashCode() ^ this._context.Binder.GetHashCode();
  }

  public override bool Equals(object obj)
  {
    return obj is PythonInvokeBinder pythonInvokeBinder && pythonInvokeBinder._context.Binder == this._context.Binder && this._signature == pythonInvokeBinder._signature;
  }

  public override string ToString() => "Python Invoke " + this.Signature.ToString();

  public CallSignature Signature => this._signature;

  internal DynamicMetaObject InvokeForeignObject(DynamicMetaObject target, DynamicMetaObject[] args)
  {
    CallInfo callInfo;
    List<Expression> metaArgs;
    Expression test;
    BindingRestrictions restrictions;
    this.TranslateArguments(target, args, out callInfo, out metaArgs, out test, out restrictions);
    return BindingHelpers.AddDynamicTestAndDefer((DynamicMetaObjectBinder) this, new DynamicMetaObject((Expression) Expression.Dynamic((CallSiteBinder) this._context.CompatInvoke(callInfo), typeof (object), metaArgs.ToArray()), restrictions.Merge(BindingRestrictionsHelpers.GetRuntimeTypeRestriction(target.Expression, target.GetLimitType()))), args, new ValidationInfo(test));
  }

  private void TranslateArguments(
    DynamicMetaObject target,
    DynamicMetaObject[] args,
    out CallInfo callInfo,
    out List<Expression> metaArgs,
    out Expression test,
    out BindingRestrictions restrictions)
  {
    Argument[] argumentInfos = this._signature.GetArgumentInfos();
    List<string> stringList1 = new List<string>();
    metaArgs = new List<Expression>();
    metaArgs.Add(target.Expression);
    Expression expression = (Expression) null;
    Expression right = (Expression) null;
    restrictions = BindingRestrictions.Empty;
    for (int index1 = 0; index1 < argumentInfos.Length; ++index1)
    {
      Argument obj = argumentInfos[index1];
      switch (obj.Kind)
      {
        case ArgumentType.Simple:
          metaArgs.Add(args[index1].Expression);
          break;
        case ArgumentType.Named:
          stringList1.Add(obj.Name);
          metaArgs.Add(args[index1].Expression);
          break;
        case ArgumentType.List:
          IList<object> objectList = (IList<object>) args[index1].Value;
          expression = (Expression) Expression.Equal((Expression) Expression.Property(Microsoft.Scripting.Ast.Utils.Convert(args[index1].Expression, args[index1].GetLimitType()), typeof (ICollection<object>).GetProperty("Count")), Microsoft.Scripting.Ast.Utils.Constant((object) objectList.Count));
          for (int index2 = 0; index2 < objectList.Count; ++index2)
            metaArgs.Add((Expression) Expression.Call(Microsoft.Scripting.Ast.Utils.Convert(args[index1].Expression, typeof (IList<object>)), typeof (IList<object>).GetMethod("get_Item"), Microsoft.Scripting.Ast.Utils.Constant((object) index2)));
          restrictions = restrictions.Merge(BindingRestrictionsHelpers.GetRuntimeTypeRestriction(args[index1].Expression, args[index1].GetLimitType()));
          break;
        case ArgumentType.Dictionary:
          PythonDictionary pythonDictionary = (PythonDictionary) args[index1].Value;
          List<string> stringList2 = new List<string>();
          foreach (KeyValuePair<object, object> keyValuePair in pythonDictionary)
          {
            string key = (string) keyValuePair.Key;
            stringList1.Add(key);
            stringList2.Add(key);
            metaArgs.Add((Expression) Expression.Call(Microsoft.Scripting.Ast.Utils.Convert(args[index1].Expression, typeof (PythonDictionary)), typeof (PythonDictionary).GetMethod("get_Item", new Type[1]
            {
              typeof (object)
            }), Microsoft.Scripting.Ast.Utils.Constant((object) key)));
          }
          restrictions = restrictions.Merge(BindingRestrictionsHelpers.GetRuntimeTypeRestriction(args[index1].Expression, args[index1].GetLimitType()));
          right = (Expression) Expression.Call(typeof (PythonOps).GetMethod("CheckDictionaryMembers"), Microsoft.Scripting.Ast.Utils.Convert(args[index1].Expression, typeof (PythonDictionary)), Microsoft.Scripting.Ast.Utils.Constant((object) stringList2.ToArray()));
          break;
        default:
          throw new InvalidOperationException();
      }
    }
    callInfo = new CallInfo(metaArgs.Count - 1, stringList1.ToArray());
    test = expression;
    if (right == null)
      return;
    if (test != null)
      test = (Expression) Expression.AndAlso(test, right);
    else
      test = right;
  }

  public PythonContext Context => this._context;

  public virtual Expression CreateExpression()
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeInvokeAction"), BindingHelpers.CreateBinderStateExpression(), this.Signature.CreateExpression());
  }

  public virtual bool SupportsLightThrow => false;

  public virtual CallSiteBinder GetLightExceptionBinder()
  {
    if (this._lightThrowBinder == null)
      this._lightThrowBinder = new PythonInvokeBinder.LightThrowBinder(this._context, this._signature);
    return (CallSiteBinder) this._lightThrowBinder;
  }

  public CallSiteBinder GetLightExceptionBinder(bool really)
  {
    return really ? this.GetLightExceptionBinder() : (CallSiteBinder) this;
  }

  private class LightThrowBinder(PythonContext context, CallSignature signature) : PythonInvokeBinder(context, signature)
  {
    public override bool SupportsLightThrow => true;

    public override CallSiteBinder GetLightExceptionBinder() => (CallSiteBinder) this;
  }
}
