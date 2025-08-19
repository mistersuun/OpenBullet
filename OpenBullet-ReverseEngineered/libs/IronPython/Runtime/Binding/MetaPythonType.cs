// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.MetaPythonType
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class MetaPythonType(
  Expression expression,
  BindingRestrictions restrictions,
  PythonType value) : MetaPythonObject(expression, BindingRestrictions.Empty, (object) value), IPythonInvokable, IPythonConvertible, IPythonGetable
{
  public DynamicMetaObject Invoke(
    PythonInvokeBinder pythonInvoke,
    Expression codeContext,
    DynamicMetaObject target,
    DynamicMetaObject[] args)
  {
    return BuiltinFunction.TranslateArguments((DynamicMetaObjectBinder) pythonInvoke, codeContext, target, args, false, this.Value.Name) ?? this.InvokeWorker((DynamicMetaObjectBinder) pythonInvoke, args, codeContext);
  }

  public override DynamicMetaObject BindInvokeMember(
    InvokeMemberBinder action,
    DynamicMetaObject[] args)
  {
    foreach (PythonType pythonType in (IEnumerable<PythonType>) this.Value.ResolutionOrder)
    {
      if (pythonType.IsSystemType)
        return action.FallbackInvokeMember((DynamicMetaObject) this, args);
      if (!pythonType.TryResolveSlot(DefaultContext.DefaultCLS, action.Name, out PythonTypeSlot _))
      {
        if (pythonType.IsOldClass)
          break;
      }
      else
        break;
    }
    return BindingHelpers.GenericInvokeMember(action, (ValidationInfo) null, (DynamicMetaObject) this, args);
  }

  public override DynamicMetaObject BindInvoke(InvokeBinder call, DynamicMetaObject[] args)
  {
    return this.InvokeWorker((DynamicMetaObjectBinder) call, args, PythonContext.GetCodeContext((DynamicMetaObjectBinder) call));
  }

  private DynamicMetaObject InvokeWorker(
    DynamicMetaObjectBinder call,
    DynamicMetaObject[] args,
    Expression codeContext)
  {
    if (this.NeedsDeferral())
      return call.Defer(ArrayUtils.Insert<DynamicMetaObject>((DynamicMetaObject) this, args));
    for (int index = 0; index < args.Length; ++index)
    {
      if (args[index].NeedsDeferral())
        return call.Defer(ArrayUtils.Insert<DynamicMetaObject>((DynamicMetaObject) this, args));
    }
    return BindingHelpers.AddPythonBoxing(!this.IsStandardDotNetType(call) ? this.MakePythonTypeCall(call, codeContext, args) : this.MakeStandardDotNetTypeCall(call, codeContext, args));
  }

  private DynamicMetaObject MakeStandardDotNetTypeCall(
    DynamicMetaObjectBinder call,
    Expression codeContext,
    DynamicMetaObject[] args)
  {
    CallSignature callSignature = BindingHelpers.GetCallSignature(call);
    PythonContext pythonContext = PythonContext.GetPythonContext(call);
    MethodBase[] constructors = PythonTypeOps.GetConstructors(this.Value.UnderlyingSystemType, pythonContext.Binder.PrivateBinding);
    if (constructors.Length != 0)
      return pythonContext.Binder.CallMethod((DefaultOverloadResolver) new PythonOverloadResolver(pythonContext.Binder, (IList<DynamicMetaObject>) args, callSignature, codeContext), (IList<MethodBase>) constructors, this.Restrictions.Merge(BindingRestrictions.GetInstanceRestriction(this.Expression, (object) this.Value)));
    string str = !this.Value.UnderlyingSystemType.IsAbstract() ? $"Cannot create instances of {this.Value.Name} because it has no public constructors" : $"Cannot create instances of {this.Value.Name} because it is abstract";
    return new DynamicMetaObject(call.Throw((Expression) Expression.New(typeof (TypeErrorException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) str))), this.Restrictions.Merge(BindingRestrictions.GetInstanceRestriction(this.Expression, (object) this.Value)));
  }

  private DynamicMetaObject MakePythonTypeCall(
    DynamicMetaObjectBinder call,
    Expression codeContext,
    DynamicMetaObject[] args)
  {
    ValidationInfo validationInfo = this.MakeVersionCheck();
    DynamicMetaObject dynamicMetaObject1 = (DynamicMetaObject) new RestrictedMetaObject(Microsoft.Scripting.Ast.Utils.Convert(this.Expression, this.LimitType), BindingRestrictionsHelpers.GetRuntimeTypeRestriction(this.Expression, this.LimitType), (object) this.Value);
    CallSignature callSignature = BindingHelpers.GetCallSignature(call);
    MetaPythonType.ArgumentValues ai = new MetaPythonType.ArgumentValues(callSignature, dynamicMetaObject1, args);
    if (this.TooManyArgsForDefaultNew(call, args))
      return this.MakeIncorrectArgumentsForCallError(call, ai, validationInfo);
    if (this.Value.UnderlyingSystemType.IsGenericTypeDefinition())
      return this.MakeGenericTypeDefinitionError(call, ai, validationInfo);
    if (this.Value.HasAbstractMethods(PythonContext.GetPythonContext(call).SharedContext))
      return this.MakeAbstractInstantiationError(call, ai, validationInfo);
    DynamicMetaObject dynamicMetaObject2 = BuiltinFunction.TranslateArguments(call, codeContext, dynamicMetaObject1, args, false, this.Value.Name);
    if (dynamicMetaObject2 != null)
      return dynamicMetaObject2;
    MetaPythonType.NewAdapter newAdapter;
    MetaPythonType.InitAdapter initAdapter;
    this.GetAdapters(ai, call, codeContext, out newAdapter, out initAdapter);
    PythonContext pythonContext = PythonContext.GetPythonContext(call);
    DynamicMetaObject expression1 = newAdapter.GetExpression(pythonContext.Binder);
    if (expression1.Expression.Type == typeof (void))
      return BindingHelpers.AddDynamicTestAndDefer(call, expression1, args, validationInfo);
    BindingRestrictions empty = BindingRestrictions.Empty;
    Expression expression2;
    BindingRestrictions restrictions;
    if (!this.Value.IsSystemType && (!(newAdapter is MetaPythonType.DefaultNewAdapter) || this.HasFinalizer(call)))
    {
      expression2 = (Expression) Expression.Dynamic((CallSiteBinder) this.Value.GetLateBoundInitBinder(callSignature), typeof (object), ArrayUtils.Insert<Expression>(codeContext, (Expression) Expression.Convert(expression1.Expression, typeof (object)), DynamicUtils.GetExpressions(args)));
      restrictions = expression1.Restrictions;
    }
    else
    {
      ParameterExpression left = Expression.Variable(expression1.GetLimitType(), "newInst");
      Expression expression3 = (Expression) left;
      DynamicMetaObject dynamicMetaObject3 = initAdapter.MakeInitCall(pythonContext.Binder, (DynamicMetaObject) new RestrictedMetaObject(Microsoft.Scripting.Ast.Utils.Convert((Expression) left, this.Value.UnderlyingSystemType), expression1.Restrictions));
      List<Expression> expressionList = new List<Expression>();
      if (dynamicMetaObject3.Expression != expression3)
      {
        DynamicMetaObject dynamicMetaObject4 = dynamicMetaObject3;
        if (expressionList.Count == 0)
          expressionList.Add((Expression) Expression.Assign((Expression) left, expression1.Expression));
        if (!this.Value.UnderlyingSystemType.IsAssignableFrom(expression1.Expression.Type))
          expressionList.Add(Microsoft.Scripting.Ast.Utils.IfThen((Expression) Expression.TypeIs((Expression) left, this.Value.UnderlyingSystemType), dynamicMetaObject4.Expression));
        else
          expressionList.Add(dynamicMetaObject4.Expression);
      }
      Expression expression4;
      if (expressionList.Count == 0)
      {
        expression4 = expression1.Expression;
      }
      else
      {
        expressionList.Add((Expression) left);
        expression4 = (Expression) Expression.Block((IEnumerable<Expression>) expressionList);
      }
      expression2 = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        left
      }, expression4);
      restrictions = dynamicMetaObject3.Restrictions;
    }
    return BindingHelpers.AddDynamicTestAndDefer(call, new DynamicMetaObject(expression2, dynamicMetaObject1.Restrictions.Merge(restrictions)), ArrayUtils.Insert<DynamicMetaObject>((DynamicMetaObject) this, args), validationInfo);
  }

  private void GetAdapters(
    MetaPythonType.ArgumentValues ai,
    DynamicMetaObjectBinder call,
    Expression codeContext,
    out MetaPythonType.NewAdapter newAdapter,
    out MetaPythonType.InitAdapter initAdapter)
  {
    PythonTypeSlot slot1;
    this.Value.TryResolveSlot(PythonContext.GetPythonContext(call).SharedContext, "__new__", out slot1);
    PythonTypeSlot slot2;
    this.Value.TryResolveSlot(PythonContext.GetPythonContext(call).SharedContext, "__init__", out slot2);
    newAdapter = this.GetNewAdapter(ai, slot1, call, codeContext);
    initAdapter = this.GetInitAdapter(ai, slot2, call, codeContext);
  }

  private MetaPythonType.InitAdapter GetInitAdapter(
    MetaPythonType.ArgumentValues ai,
    PythonTypeSlot init,
    DynamicMetaObjectBinder call,
    Expression codeContext)
  {
    PythonContext pythonContext = PythonContext.GetPythonContext(call);
    if (this.Value.IsMixedNewStyleOldStyle())
      return (MetaPythonType.InitAdapter) new MetaPythonType.MixedInitAdapter(ai, pythonContext, codeContext);
    if (init == InstanceOps.Init && !this.HasFinalizer(call) || this.Value == TypeCache.PythonType && ai.Arguments.Length == 2)
      return (MetaPythonType.InitAdapter) new MetaPythonType.DefaultInitAdapter(ai, pythonContext, codeContext);
    switch (init)
    {
      case BuiltinMethodDescriptor _:
        return (MetaPythonType.InitAdapter) new MetaPythonType.BuiltinInitAdapter(ai, ((BuiltinMethodDescriptor) init).Template, pythonContext, codeContext);
      case BuiltinFunction _:
        return (MetaPythonType.InitAdapter) new MetaPythonType.BuiltinInitAdapter(ai, (BuiltinFunction) init, pythonContext, codeContext);
      default:
        return (MetaPythonType.InitAdapter) new MetaPythonType.SlotInitAdapter(init, ai, pythonContext, codeContext);
    }
  }

  private MetaPythonType.NewAdapter GetNewAdapter(
    MetaPythonType.ArgumentValues ai,
    PythonTypeSlot newInst,
    DynamicMetaObjectBinder call,
    Expression codeContext)
  {
    PythonContext pythonContext = PythonContext.GetPythonContext(call);
    if (this.Value.IsMixedNewStyleOldStyle())
      return (MetaPythonType.NewAdapter) new MetaPythonType.MixedNewAdapter(ai, pythonContext, codeContext);
    if (newInst == InstanceOps.New)
      return (MetaPythonType.NewAdapter) new MetaPythonType.DefaultNewAdapter(ai, this.Value, pythonContext, codeContext);
    switch (newInst)
    {
      case ConstructorFunction _:
        return (MetaPythonType.NewAdapter) new MetaPythonType.ConstructorNewAdapter(ai, this.Value, pythonContext, codeContext);
      case BuiltinFunction _:
        return (MetaPythonType.NewAdapter) new MetaPythonType.BuiltinNewAdapter(ai, this.Value, (BuiltinFunction) newInst, pythonContext, codeContext);
      default:
        return new MetaPythonType.NewAdapter(ai, pythonContext, codeContext);
    }
  }

  private DynamicMetaObject MakeIncorrectArgumentsForCallError(
    DynamicMetaObjectBinder call,
    MetaPythonType.ArgumentValues ai,
    ValidationInfo valInfo)
  {
    string str = !this.Value.IsSystemType ? "object.__new__() takes no parameters" : (this.Value.UnderlyingSystemType.GetConstructors().Length != 0 ? "object.__new__() takes no parameters" : "cannot create instances of " + this.Value.Name);
    return BindingHelpers.AddDynamicTestAndDefer(call, new DynamicMetaObject(call.Throw((Expression) Expression.New(typeof (TypeErrorException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) str))), this.GetErrorRestrictions(ai)), ai.Arguments, valInfo);
  }

  private DynamicMetaObject MakeGenericTypeDefinitionError(
    DynamicMetaObjectBinder call,
    MetaPythonType.ArgumentValues ai,
    ValidationInfo valInfo)
  {
    string str = $"cannot create instances of {this.Value.Name} because it is a generic type definition";
    return BindingHelpers.AddDynamicTestAndDefer(call, new DynamicMetaObject(call.Throw((Expression) Expression.New(typeof (TypeErrorException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) str)), typeof (object)), this.GetErrorRestrictions(ai)), ai.Arguments, valInfo);
  }

  private DynamicMetaObject MakeAbstractInstantiationError(
    DynamicMetaObjectBinder call,
    MetaPythonType.ArgumentValues ai,
    ValidationInfo valInfo)
  {
    string abstractErrorMessage = this.Value.GetAbstractErrorMessage(PythonContext.GetPythonContext(call).SharedContext);
    return BindingHelpers.AddDynamicTestAndDefer(call, new DynamicMetaObject((Expression) Expression.Throw((Expression) Expression.New(typeof (ArgumentTypeException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) abstractErrorMessage)), typeof (object)), this.GetErrorRestrictions(ai)), ai.Arguments, valInfo);
  }

  private BindingRestrictions GetErrorRestrictions(MetaPythonType.ArgumentValues ai)
  {
    BindingRestrictions errorRestrictions = this.Restrict(this.GetRuntimeType()).Restrictions.Merge(MetaPythonType.GetInstanceRestriction(ai));
    foreach (DynamicMetaObject self in ai.Arguments)
    {
      if (self.HasValue)
        errorRestrictions = errorRestrictions.Merge(self.Restrict(self.GetRuntimeType()).Restrictions);
    }
    return errorRestrictions;
  }

  private static BindingRestrictions GetInstanceRestriction(MetaPythonType.ArgumentValues ai)
  {
    return BindingRestrictions.GetInstanceRestriction(ai.Self.Expression, ai.Self.Value);
  }

  private bool HasFinalizer(DynamicMetaObjectBinder action)
  {
    return !this.Value.IsSystemType && this.Value.TryResolveSlot(PythonContext.GetPythonContext(action).SharedContext, "__del__", out PythonTypeSlot _);
  }

  private bool HasDefaultNew(DynamicMetaObjectBinder action)
  {
    PythonTypeSlot slot;
    this.Value.TryResolveSlot(PythonContext.GetPythonContext(action).SharedContext, "__new__", out slot);
    return slot == InstanceOps.New;
  }

  private bool HasDefaultInit(DynamicMetaObjectBinder action)
  {
    PythonTypeSlot slot;
    this.Value.TryResolveSlot(PythonContext.GetPythonContext(action).SharedContext, "__init__", out slot);
    return slot == InstanceOps.Init;
  }

  private bool HasDefaultNewAndInit(DynamicMetaObjectBinder action)
  {
    return this.HasDefaultNew(action) && this.HasDefaultInit(action);
  }

  private bool TooManyArgsForDefaultNew(DynamicMetaObjectBinder action, DynamicMetaObject[] args)
  {
    if (args.Length != 0 && this.HasDefaultNewAndInit(action))
    {
      Argument[] argumentInfos = BindingHelpers.GetCallSignature(action).GetArgumentInfos();
      for (int index = 0; index < argumentInfos.Length; ++index)
      {
        switch (argumentInfos[index].Kind)
        {
          case ArgumentType.List:
            if (((ICollection<object>) args[index].Value).Count > 0)
              return true;
            break;
          case ArgumentType.Dictionary:
            if (PythonOps.Length(args[index].Value) > 0)
              return true;
            break;
          default:
            return true;
        }
      }
    }
    return false;
  }

  private ValidationInfo MakeVersionCheck()
  {
    int version = this.Value.Version;
    return new ValidationInfo((Expression) Expression.Equal((Expression) Expression.Call(typeof (PythonOps).GetMethod("GetTypeVersion"), (Expression) Expression.Convert(this.Expression, typeof (PythonType))), Microsoft.Scripting.Ast.Utils.Constant((object) version)));
  }

  private bool IsStandardDotNetType(DynamicMetaObjectBinder action)
  {
    PythonContext pythonContext = PythonContext.GetPythonContext(action);
    return this.Value.IsSystemType && !this.Value.IsPythonType && !pythonContext.Binder.HasExtensionTypes(this.Value.UnderlyingSystemType) && !typeof (Delegate).IsAssignableFrom(this.Value.UnderlyingSystemType) && !this.Value.UnderlyingSystemType.IsArray;
  }

  public override DynamicMetaObject BindCreateInstance(
    CreateInstanceBinder create,
    DynamicMetaObject[] args)
  {
    return this.InvokeWorker((DynamicMetaObjectBinder) create, args, Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext((DynamicMetaObjectBinder) create).SharedContext));
  }

  public override DynamicMetaObject BindConvert(ConvertBinder conversion)
  {
    return this.ConvertWorker((DynamicMetaObjectBinder) conversion, conversion.Type, conversion.Explicit ? ConversionResultKind.ExplicitCast : ConversionResultKind.ImplicitCast);
  }

  public DynamicMetaObject BindConvert(PythonConversionBinder binder)
  {
    return this.ConvertWorker((DynamicMetaObjectBinder) binder, binder.Type, binder.ResultKind);
  }

  public DynamicMetaObject ConvertWorker(
    DynamicMetaObjectBinder binder,
    Type type,
    ConversionResultKind kind)
  {
    return type.IsSubclassOf(typeof (Delegate)) ? MetaPythonObject.MakeDelegateTarget(binder, type, this.Restrict(this.Value.GetType())) : this.FallbackConvert(binder);
  }

  public override IEnumerable<string> GetDynamicMemberNames()
  {
    foreach (object memberName in this.Value.GetMemberNames((this.Value.PythonContext ?? DefaultContext.DefaultPythonContext).SharedContext))
    {
      if (memberName is string)
        yield return (string) memberName;
    }
  }

  public PythonType Value => (PythonType) base.Value;

  public override DynamicMetaObject BindGetMember(GetMemberBinder member)
  {
    return this.GetMemberWorker((DynamicMetaObjectBinder) member, PythonContext.GetCodeContext((DynamicMetaObjectBinder) member));
  }

  private ValidationInfo GetTypeTest()
  {
    int version = this.Value.Version;
    return new ValidationInfo((Expression) Expression.Call(typeof (PythonOps).GetMethod("CheckSpecificTypeVersion"), Microsoft.Scripting.Ast.Utils.Convert(this.Expression, typeof (PythonType)), Microsoft.Scripting.Ast.Utils.Constant((object) version)));
  }

  public override DynamicMetaObject BindSetMember(SetMemberBinder member, DynamicMetaObject value)
  {
    PythonContext pythonContext = PythonContext.GetPythonContext((DynamicMetaObjectBinder) member);
    if (!this.Value.IsSystemType)
      return this.MakeSetMember(member, value);
    MemberTracker memberTracker = MemberTracker.FromMemberInfo((MemberInfo) this.Value.UnderlyingSystemType);
    foreach (MemberTracker mt in pythonContext.Binder.GetMember(MemberRequestKind.Set, this.Value.UnderlyingSystemType, member.Name))
    {
      if (MetaPythonType.IsProtectedSetter(mt))
        return new DynamicMetaObject(BindingHelpers.TypeErrorForProtectedMember(this.Value.UnderlyingSystemType, member.Name), this.Restrictions.Merge(value.Restrictions).Merge(BindingRestrictions.GetInstanceRestriction(this.Expression, (object) this.Value)));
    }
    return new DynamicMetaObject(pythonContext.Binder.SetMember(member.Name, new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) memberTracker), BindingRestrictions.Empty, (object) memberTracker), value, (OverloadResolverFactory) new PythonOverloadResolverFactory(pythonContext.Binder, Microsoft.Scripting.Ast.Utils.Constant((object) pythonContext.SharedContext))).Expression, this.Restrictions.Merge(value.Restrictions).Merge(BindingRestrictions.GetInstanceRestriction(this.Expression, (object) this.Value)));
  }

  public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder member)
  {
    if (!this.Value.IsSystemType)
      return this.MakeDeleteMember(member);
    PythonContext pythonContext = PythonContext.GetPythonContext((DynamicMetaObjectBinder) member);
    MemberTracker memberTracker = MemberTracker.FromMemberInfo((MemberInfo) this.Value.UnderlyingSystemType);
    return new DynamicMetaObject(pythonContext.Binder.DeleteMember(member.Name, new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) memberTracker), BindingRestrictions.Empty, (object) memberTracker), (OverloadResolverFactory) pythonContext.SharedOverloadResolverFactory).Expression, BindingRestrictions.GetInstanceRestriction(this.Expression, (object) this.Value).Merge(this.Restrictions));
  }

  public DynamicMetaObject GetMember(PythonGetMemberBinder member, DynamicMetaObject codeContext)
  {
    return this.GetMemberWorker((DynamicMetaObjectBinder) member, codeContext.Expression);
  }

  private DynamicMetaObject GetMemberWorker(DynamicMetaObjectBinder member, Expression codeContext)
  {
    return new MetaPythonType.MetaGetBinderHelper(this, member, codeContext, this.GetTypeTest(), this.MakeMetaTypeTest(this.Restrict(this.GetRuntimeType()).Expression)).MakeTypeGetMember();
  }

  private ValidationInfo MakeMetaTypeTest(Expression self)
  {
    PythonType pythonType = DynamicHelpers.GetPythonType((object) this.Value);
    if (pythonType.IsSystemType)
      return ValidationInfo.Empty;
    int version = pythonType.Version;
    return new ValidationInfo((Expression) Expression.Call(typeof (PythonOps).GetMethod("CheckTypeVersion"), self, Microsoft.Scripting.Ast.Utils.Constant((object) version)));
  }

  private DynamicMetaObject MakeSetMember(SetMemberBinder member, DynamicMetaObject value)
  {
    PythonContext pythonContext = PythonContext.GetPythonContext((DynamicMetaObjectBinder) member);
    DynamicMetaObject dynamicMetaObject = this.Restrict(this.Value.GetType());
    PythonTypeSlot pts;
    if (this.Value.GetType() != typeof (PythonType) && DynamicHelpers.GetPythonType((object) this.Value).IsSystemType && this.Value.TryGetCustomSetAttr(pythonContext.SharedContext, out pts))
    {
      ParameterExpression parameterExpression = Expression.Variable(typeof (object), "boundVal");
      return BindingHelpers.AddDynamicTestAndDefer((DynamicMetaObjectBinder) member, new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        parameterExpression
      }, (Expression) Expression.Dynamic((CallSiteBinder) pythonContext.Invoke(new CallSignature(2)), typeof (object), Microsoft.Scripting.Ast.Utils.Constant((object) pythonContext.SharedContext), (Expression) Expression.Block((Expression) Expression.Call(typeof (PythonOps).GetMethod("SlotTryGetValue"), Microsoft.Scripting.Ast.Utils.Constant((object) pythonContext.SharedContext), Microsoft.Scripting.Ast.Utils.Convert((Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) pts), typeof (PythonTypeSlot)), Microsoft.Scripting.Ast.Utils.Convert(this.Expression, typeof (object)), Microsoft.Scripting.Ast.Utils.Convert((Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) DynamicHelpers.GetPythonType((object) this.Value)), typeof (PythonType)), (Expression) parameterExpression), (Expression) parameterExpression), (Expression) Expression.Constant((object) member.Name), value.Expression)), dynamicMetaObject.Restrictions), new DynamicMetaObject[2]
      {
        (DynamicMetaObject) this,
        value
      }, this.TestUserType());
    }
    return BindingHelpers.AddDynamicTestAndDefer((DynamicMetaObjectBinder) member, new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("PythonTypeSetCustomMember"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext((DynamicMetaObjectBinder) member).SharedContext), dynamicMetaObject.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) member.Name), Microsoft.Scripting.Ast.Utils.Convert(value.Expression, typeof (object))), dynamicMetaObject.Restrictions.Merge(value.Restrictions)), new DynamicMetaObject[2]
    {
      (DynamicMetaObject) this,
      value
    }, this.TestUserType());
  }

  private static bool IsProtectedSetter(MemberTracker mt)
  {
    if (mt is PropertyTracker propertyTracker)
    {
      MethodInfo setMethod = propertyTracker.GetSetMethod(true);
      if (setMethod != (MethodInfo) null && setMethod.IsProtected())
        return true;
    }
    return mt is FieldTracker fieldTracker && fieldTracker.Field.IsProtected();
  }

  private DynamicMetaObject MakeDeleteMember(DeleteMemberBinder member)
  {
    DynamicMetaObject dynamicMetaObject = this.Restrict(this.Value.GetType());
    return BindingHelpers.AddDynamicTestAndDefer((DynamicMetaObjectBinder) member, new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("PythonTypeDeleteCustomMember"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext((DynamicMetaObjectBinder) member).SharedContext), dynamicMetaObject.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) member.Name)), dynamicMetaObject.Restrictions), new DynamicMetaObject[1]
    {
      (DynamicMetaObject) this
    }, this.TestUserType());
  }

  private ValidationInfo TestUserType()
  {
    return new ValidationInfo((Expression) Expression.Not((Expression) Expression.Call(typeof (PythonOps).GetMethod("IsPythonType"), Microsoft.Scripting.Ast.Utils.Convert(this.Expression, typeof (PythonType)))));
  }

  private class CallAdapter
  {
    private readonly MetaPythonType.ArgumentValues _argInfo;
    private readonly PythonContext _state;
    private readonly Expression _context;

    public CallAdapter(
      MetaPythonType.ArgumentValues ai,
      PythonContext state,
      Expression codeContext)
    {
      this._argInfo = ai;
      this._state = state;
      this._context = codeContext;
    }

    protected PythonContext PythonContext => this._state;

    protected Expression CodeContext => this._context;

    protected MetaPythonType.ArgumentValues Arguments => this._argInfo;
  }

  private class ArgumentValues
  {
    public readonly DynamicMetaObject Self;
    public readonly DynamicMetaObject[] Arguments;
    public readonly CallSignature Signature;

    public ArgumentValues(
      CallSignature signature,
      DynamicMetaObject self,
      DynamicMetaObject[] args)
    {
      this.Self = self;
      this.Signature = signature;
      this.Arguments = args;
    }
  }

  private class NewAdapter(
    MetaPythonType.ArgumentValues ai,
    PythonContext state,
    Expression codeContext) : MetaPythonType.CallAdapter(ai, state, codeContext)
  {
    public virtual DynamicMetaObject GetExpression(PythonBinder binder)
    {
      return this.MakeDefaultNew((Microsoft.Scripting.Actions.DefaultBinder) binder, (Expression) Expression.Call(typeof (PythonOps).GetMethod("PythonTypeGetMember"), this.CodeContext, Microsoft.Scripting.Ast.Utils.Convert(this.Arguments.Self.Expression, typeof (PythonType)), Microsoft.Scripting.Ast.Utils.Constant((object) null), Microsoft.Scripting.Ast.Utils.Constant((object) "__new__")));
    }

    protected DynamicMetaObject MakeDefaultNew(Microsoft.Scripting.Actions.DefaultBinder binder, Expression function)
    {
      List<Expression> args = new List<Expression>();
      args.Add(this.CodeContext);
      args.Add(function);
      this.AppendNewArgs(args);
      return new DynamicMetaObject((Expression) Expression.Dynamic((CallSiteBinder) this.PythonContext.Invoke(this.GetDynamicNewSignature()), typeof (object), args.ToArray()), this.Arguments.Self.Restrictions);
    }

    private void AppendNewArgs(List<Expression> args)
    {
      args.Add(this.Arguments.Self.Expression);
      foreach (DynamicMetaObject dynamicMetaObject in this.Arguments.Arguments)
        args.Add(dynamicMetaObject.Expression);
    }

    protected CallSignature GetDynamicNewSignature()
    {
      return this.Arguments.Signature.InsertArgument(Argument.Simple);
    }
  }

  private class DefaultNewAdapter : MetaPythonType.NewAdapter
  {
    private readonly PythonType _creating;

    public DefaultNewAdapter(
      MetaPythonType.ArgumentValues ai,
      PythonType creating,
      PythonContext state,
      Expression codeContext)
      : base(ai, state, codeContext)
    {
      this._creating = creating;
    }

    public override DynamicMetaObject GetExpression(PythonBinder binder)
    {
      PythonOverloadResolver resolver;
      if (this._creating.IsSystemType || this._creating.HasSystemCtor)
        resolver = new PythonOverloadResolver(binder, (IList<DynamicMetaObject>) DynamicMetaObject.EmptyMetaObjects, new CallSignature(0), this.CodeContext);
      else
        resolver = new PythonOverloadResolver(binder, (IList<DynamicMetaObject>) new DynamicMetaObject[1]
        {
          this.Arguments.Self
        }, new CallSignature(1), this.CodeContext);
      return binder.CallMethod((DefaultOverloadResolver) resolver, (IList<MethodBase>) this._creating.UnderlyingSystemType.GetConstructors(), BindingRestrictions.Empty, this._creating.Name);
    }
  }

  private class ConstructorNewAdapter : MetaPythonType.NewAdapter
  {
    private readonly PythonType _creating;

    public ConstructorNewAdapter(
      MetaPythonType.ArgumentValues ai,
      PythonType creating,
      PythonContext state,
      Expression codeContext)
      : base(ai, state, codeContext)
    {
      this._creating = creating;
    }

    public override DynamicMetaObject GetExpression(PythonBinder binder)
    {
      PythonOverloadResolver resolver = this._creating.IsSystemType || this._creating.HasSystemCtor ? new PythonOverloadResolver(binder, (IList<DynamicMetaObject>) this.Arguments.Arguments, this.Arguments.Signature, this.CodeContext) : new PythonOverloadResolver(binder, (IList<DynamicMetaObject>) ArrayUtils.Insert<DynamicMetaObject>(this.Arguments.Self, this.Arguments.Arguments), this.GetDynamicNewSignature(), this.CodeContext);
      return binder.CallMethod((DefaultOverloadResolver) resolver, (IList<MethodBase>) this._creating.UnderlyingSystemType.GetConstructors(), this.Arguments.Self.Restrictions, this._creating.Name);
    }
  }

  private class BuiltinNewAdapter : MetaPythonType.NewAdapter
  {
    private readonly PythonType _creating;
    private readonly BuiltinFunction _ctor;

    public BuiltinNewAdapter(
      MetaPythonType.ArgumentValues ai,
      PythonType creating,
      BuiltinFunction ctor,
      PythonContext state,
      Expression codeContext)
      : base(ai, state, codeContext)
    {
      this._creating = creating;
      this._ctor = ctor;
    }

    public override DynamicMetaObject GetExpression(PythonBinder binder)
    {
      return binder.CallMethod((DefaultOverloadResolver) new PythonOverloadResolver(binder, (IList<DynamicMetaObject>) ArrayUtils.Insert<DynamicMetaObject>(this.Arguments.Self, this.Arguments.Arguments), this.Arguments.Signature.InsertArgument(new Argument(ArgumentType.Simple)), this.CodeContext), this._ctor.Targets, this._creating.Name);
    }
  }

  private class MixedNewAdapter(
    MetaPythonType.ArgumentValues ai,
    PythonContext state,
    Expression codeContext) : MetaPythonType.NewAdapter(ai, state, codeContext)
  {
    public override DynamicMetaObject GetExpression(PythonBinder binder)
    {
      return this.MakeDefaultNew((Microsoft.Scripting.Actions.DefaultBinder) binder, (Expression) Expression.Call(typeof (PythonOps).GetMethod("GetMixedMember"), this.CodeContext, this.Arguments.Self.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) null), Microsoft.Scripting.Ast.Utils.Constant((object) "__new__")));
    }
  }

  private abstract class InitAdapter(
    MetaPythonType.ArgumentValues ai,
    PythonContext state,
    Expression codeContext) : MetaPythonType.CallAdapter(ai, state, codeContext)
  {
    public abstract DynamicMetaObject MakeInitCall(
      PythonBinder binder,
      DynamicMetaObject createExpr);

    protected DynamicMetaObject MakeDefaultInit(
      PythonBinder binder,
      DynamicMetaObject createExpr,
      Expression init)
    {
      List<Expression> expressionList = new List<Expression>();
      expressionList.Add(this.CodeContext);
      expressionList.Add((Expression) Expression.Convert(createExpr.Expression, typeof (object)));
      foreach (DynamicMetaObject dynamicMetaObject in this.Arguments.Arguments)
        expressionList.Add(dynamicMetaObject.Expression);
      return new DynamicMetaObject((Expression) Expression.Dynamic((CallSiteBinder) ((PythonType) this.Arguments.Self.Value).GetLateBoundInitBinder(this.Arguments.Signature), typeof (object), expressionList.ToArray()), this.Arguments.Self.Restrictions.Merge(createExpr.Restrictions));
    }
  }

  private class SlotInitAdapter : MetaPythonType.InitAdapter
  {
    private readonly PythonTypeSlot _slot;

    public SlotInitAdapter(
      PythonTypeSlot slot,
      MetaPythonType.ArgumentValues ai,
      PythonContext state,
      Expression codeContext)
      : base(ai, state, codeContext)
    {
      this._slot = slot;
    }

    public override DynamicMetaObject MakeInitCall(
      PythonBinder binder,
      DynamicMetaObject createExpr)
    {
      Expression init = (Expression) Expression.Call(typeof (PythonOps).GetMethod("GetInitSlotMember"), this.CodeContext, (Expression) Expression.Convert(this.Arguments.Self.Expression, typeof (PythonType)), (Expression) Expression.Convert((Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) this._slot), typeof (PythonTypeSlot)), Microsoft.Scripting.Ast.Utils.Convert(createExpr.Expression, typeof (object)));
      return this.MakeDefaultInit(binder, createExpr, init);
    }
  }

  private class DefaultInitAdapter(
    MetaPythonType.ArgumentValues ai,
    PythonContext state,
    Expression codeContext) : MetaPythonType.InitAdapter(ai, state, codeContext)
  {
    public override DynamicMetaObject MakeInitCall(
      PythonBinder binder,
      DynamicMetaObject createExpr)
    {
      return createExpr;
    }
  }

  private class BuiltinInitAdapter : MetaPythonType.InitAdapter
  {
    private readonly BuiltinFunction _method;

    public BuiltinInitAdapter(
      MetaPythonType.ArgumentValues ai,
      BuiltinFunction method,
      PythonContext state,
      Expression codeContext)
      : base(ai, state, codeContext)
    {
      this._method = method;
    }

    public override DynamicMetaObject MakeInitCall(
      PythonBinder binder,
      DynamicMetaObject createExpr)
    {
      return this._method == InstanceOps.Init.Template ? createExpr : binder.CallMethod((DefaultOverloadResolver) new PythonOverloadResolver(binder, createExpr, (IList<DynamicMetaObject>) this.Arguments.Arguments, this.Arguments.Signature, this.CodeContext), this._method.Targets, this.Arguments.Self.Restrictions);
    }
  }

  private class MixedInitAdapter(
    MetaPythonType.ArgumentValues ai,
    PythonContext state,
    Expression codeContext) : MetaPythonType.InitAdapter(ai, state, codeContext)
  {
    public override DynamicMetaObject MakeInitCall(
      PythonBinder binder,
      DynamicMetaObject createExpr)
    {
      Expression init = (Expression) Expression.Call(typeof (PythonOps).GetMethod("GetMixedMember"), this.CodeContext, (Expression) Expression.Convert(this.Arguments.Self.Expression, typeof (PythonType)), Microsoft.Scripting.Ast.Utils.Convert(createExpr.Expression, typeof (object)), Microsoft.Scripting.Ast.Utils.Constant((object) "__init__"));
      return this.MakeDefaultInit(binder, createExpr, init);
    }
  }

  public abstract class GetBinderHelper<TResult>
  {
    private readonly PythonType _value;
    private readonly string _name;
    internal readonly CodeContext _context;

    public GetBinderHelper(PythonType value, CodeContext context, string name)
    {
      this._value = value;
      this._name = name;
      this._context = context;
    }

    protected abstract TResult Finish(bool metaOnly);

    protected abstract void AddError();

    protected abstract void AddMetaGetAttribute(PythonType metaType, PythonTypeSlot pts);

    protected abstract bool AddMetaSlotAccess(PythonType pt, PythonTypeSlot pts);

    protected abstract void AddMetaOldClassAccess();

    protected abstract bool AddSlotAccess(PythonType pt, PythonTypeSlot pts);

    protected abstract void AddOldClassAccess(PythonType pt);

    public TResult MakeTypeGetMember()
    {
      bool flag = false;
      bool metaOnly = false;
      CodeContext sharedClsContext = this._context.LanguageContext.SharedClsContext;
      PythonType pythonType1 = DynamicHelpers.GetPythonType((object) this.Value);
      PythonTypeSlot slot;
      foreach (PythonType pythonType2 in (IEnumerable<PythonType>) pythonType1.ResolutionOrder)
      {
        if (pythonType2.TryLookupSlot(sharedClsContext, this._name, out slot) && slot.IsSetDescriptor(sharedClsContext, pythonType1) && this.AddMetaSlotAccess(pythonType1, slot))
        {
          metaOnly = flag = true;
          break;
        }
      }
      if (!flag)
      {
        foreach (PythonType pt in (IEnumerable<PythonType>) this.Value.ResolutionOrder)
        {
          if (pt.IsOldClass)
            this.AddOldClassAccess(pt);
          else if (pt.TryLookupSlot(sharedClsContext, this._name, out slot) && this.AddSlotAccess(pt, slot))
          {
            flag = true;
            break;
          }
        }
      }
      if (!flag)
      {
        foreach (PythonType pythonType3 in (IEnumerable<PythonType>) pythonType1.ResolutionOrder)
        {
          if (pythonType3.OldClass != null)
          {
            this.AddMetaOldClassAccess();
            flag = true;
            break;
          }
          if (pythonType3.TryLookupSlot(sharedClsContext, this._name, out slot) && this.AddMetaSlotAccess(pythonType1, slot))
          {
            flag = true;
            break;
          }
        }
      }
      if (!flag && pythonType1.TryResolveSlot(this._context, "__getattr__", out slot) && !slot.IsSetDescriptor(sharedClsContext, pythonType1))
      {
        this.AddMetaGetAttribute(pythonType1, slot);
        flag = slot.GetAlwaysSucceeds;
      }
      if (!flag)
        this.AddError();
      return this.Finish(metaOnly);
    }

    protected PythonType Value => this._value;
  }

  private class MetaGetBinderHelper : MetaPythonType.GetBinderHelper<DynamicMetaObject>
  {
    private readonly DynamicMetaObjectBinder _member;
    private readonly MetaPythonType _type;
    private readonly Expression _codeContext;
    private readonly DynamicMetaObject _restrictedSelf;
    private readonly ConditionalBuilder _cb;
    private readonly string _symName;
    private readonly PythonContext _state;
    private readonly ValidationInfo _valInfo;
    private readonly ValidationInfo _metaValInfo;
    private ParameterExpression _tmp;

    public MetaGetBinderHelper(
      MetaPythonType type,
      DynamicMetaObjectBinder member,
      Expression codeContext,
      ValidationInfo validationInfo,
      ValidationInfo metaValidation)
      : base(type.Value, PythonContext.GetPythonContext(member).SharedContext, MetaPythonObject.GetGetMemberName(member))
    {
      this._member = member;
      this._codeContext = codeContext;
      this._type = type;
      this._cb = new ConditionalBuilder(member);
      this._symName = MetaPythonObject.GetGetMemberName(member);
      this._restrictedSelf = new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Convert(this.Expression, this.Value.GetType()), this.Restrictions.Merge(BindingRestrictions.GetInstanceRestriction(this.Expression, (object) this.Value)), (object) this.Value);
      this._state = PythonContext.GetPythonContext(member);
      this._valInfo = validationInfo;
      this._metaValInfo = metaValidation;
    }

    protected override void AddOldClassAccess(PythonType pt)
    {
      this.EnsureTmp();
      this._cb.AddCondition((Expression) Expression.Call(typeof (PythonOps).GetMethod("OldClassTryLookupOneSlot"), Microsoft.Scripting.Ast.Utils.Constant((object) pt), Microsoft.Scripting.Ast.Utils.Constant((object) pt.OldClass), Microsoft.Scripting.Ast.Utils.Constant((object) this._symName), (Expression) this._tmp), (Expression) this._tmp);
    }

    private void EnsureTmp()
    {
      if (this._tmp != null)
        return;
      this._tmp = Expression.Variable(typeof (object), "tmp");
      this._cb.AddVariable(this._tmp);
    }

    protected override bool AddSlotAccess(PythonType pt, PythonTypeSlot pts)
    {
      pts.MakeGetExpression(this._state.Binder, this._codeContext, (DynamicMetaObject) null, new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Convert((Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) this.Value), typeof (PythonType)), BindingRestrictions.Empty, (object) this.Value), this._cb);
      if (pts.IsAlwaysVisible)
        return pts.GetAlwaysSucceeds;
      this._cb.ExtendLastCondition((Expression) Expression.Call(typeof (PythonOps).GetMethod("IsClsVisible"), this._codeContext));
      return false;
    }

    protected override void AddMetaOldClassAccess()
    {
      this._cb.FinishCondition((Expression) Expression.Call(Microsoft.Scripting.Ast.Utils.Convert(this.Expression, typeof (PythonType)), typeof (PythonType).GetMethod("__getattribute__"), this._codeContext, Microsoft.Scripting.Ast.Utils.Constant((object) MetaPythonObject.GetGetMemberName(this._member))));
    }

    protected override void AddError()
    {
      this._cb.FinishCondition(this.GetFallbackError(this._member).Expression);
    }

    protected override void AddMetaGetAttribute(PythonType metaType, PythonTypeSlot pts)
    {
      this.EnsureTmp();
      Expression condition = (Expression) Expression.Call(typeof (PythonOps).GetMethod("SlotTryGetBoundValue"), this._codeContext, (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) pts, typeof (PythonTypeSlot)), this.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) metaType), (Expression) this._tmp);
      DynamicExpression body = Expression.Dynamic((CallSiteBinder) this._state.InvokeOne, typeof (object), this._codeContext, (Expression) this._tmp, Microsoft.Scripting.Ast.Utils.Constant((object) MetaPythonObject.GetGetMemberName(this._member)));
      if (!pts.GetAlwaysSucceeds)
        this._cb.AddCondition(condition, (Expression) body);
      else
        this._cb.FinishCondition((Expression) Expression.Block(condition, (Expression) body));
    }

    protected override bool AddMetaSlotAccess(PythonType metaType, PythonTypeSlot pts)
    {
      pts.MakeGetExpression(this._state.Binder, this._codeContext, (DynamicMetaObject) this._type, new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) metaType), BindingRestrictions.Empty, (object) metaType), this._cb);
      if (pts.IsAlwaysVisible)
        return pts.GetAlwaysSucceeds;
      this._cb.ExtendLastCondition((Expression) Expression.Call(typeof (PythonOps).GetMethod("IsClsVisible"), this._codeContext));
      return false;
    }

    protected override DynamicMetaObject Finish(bool metaOnly)
    {
      DynamicMetaObject res = this._cb.GetMetaObject(this._restrictedSelf);
      if (metaOnly)
        res = BindingHelpers.AddDynamicTestAndDefer(this._member, res, new DynamicMetaObject[1]
        {
          (DynamicMetaObject) this._type
        }, this._metaValInfo);
      else if (!this.Value.IsSystemType)
        res = BindingHelpers.AddDynamicTestAndDefer(this._member, res, new DynamicMetaObject[1]
        {
          (DynamicMetaObject) this._type
        }, this._valInfo);
      return res;
    }

    private DynamicMetaObject GetFallbackError(DynamicMetaObjectBinder member)
    {
      if (!(member is PythonGetMemberBinder))
        return ((GetMemberBinder) member).FallbackGetMember((DynamicMetaObject) this._type);
      PythonGetMemberBinder pythonGetMemberBinder = member as PythonGetMemberBinder;
      if (pythonGetMemberBinder.IsNoThrow)
        return new DynamicMetaObject((Expression) Expression.Constant((object) OperationFailed.Value), BindingRestrictions.GetInstanceRestriction(this.Expression, (object) this.Value).Merge(this.Restrictions));
      return new DynamicMetaObject(member.Throw((Expression) Expression.Call(typeof (PythonOps).GetMethod("AttributeErrorForMissingAttribute", new Type[2]
      {
        typeof (string),
        typeof (string)
      }), Microsoft.Scripting.Ast.Utils.Constant((object) DynamicHelpers.GetPythonType((object) this.Value).Name), Microsoft.Scripting.Ast.Utils.Constant((object) pythonGetMemberBinder.Name)), typeof (object)), BindingRestrictions.GetInstanceRestriction(this.Expression, (object) this.Value).Merge(this.Restrictions));
    }

    private Expression Expression => this._type.Expression;

    private BindingRestrictions Restrictions => this._type.Restrictions;
  }

  internal class FastGetBinderHelper : MetaPythonType.GetBinderHelper<TypeGetBase>
  {
    private readonly PythonGetMemberBinder _binder;
    private readonly int _version;
    private readonly int _metaVersion;
    private bool _canOptimize;
    private List<FastGetDelegate> _gets = new List<FastGetDelegate>();

    public FastGetBinderHelper(PythonType type, CodeContext context, PythonGetMemberBinder binder)
      : base(type, context, binder.Name)
    {
      this._version = type.Version;
      this._metaVersion = DynamicHelpers.GetPythonType((object) type).Version;
      this._binder = binder;
    }

    public Func<CallSite, object, CodeContext, object> GetBinding()
    {
      Dictionary<string, TypeGetBase> cachedGets = this.GetCachedGets();
      TypeGetBase typeGetBase;
      lock (cachedGets)
      {
        if (cachedGets.TryGetValue(this._binder.Name, out typeGetBase))
        {
          if (typeGetBase.IsValid(this.Value))
            goto label_8;
        }
        TypeGetBase member = this.MakeTypeGetMember();
        if (member != null)
          typeGetBase = cachedGets[this._binder.Name] = member;
      }
label_8:
      return typeGetBase != null && typeGetBase.ShouldUseNonOptimizedSite ? typeGetBase._func : (Func<CallSite, object, CodeContext, object>) null;
    }

    private Dictionary<string, TypeGetBase> GetCachedGets()
    {
      if (this._binder.IsNoThrow)
      {
        Dictionary<string, TypeGetBase> cachedTypeTryGets = this.Value._cachedTypeTryGets;
        if (cachedTypeTryGets == null)
        {
          Interlocked.CompareExchange<Dictionary<string, TypeGetBase>>(ref this.Value._cachedTypeTryGets, new Dictionary<string, TypeGetBase>(), (Dictionary<string, TypeGetBase>) null);
          cachedTypeTryGets = this.Value._cachedTypeTryGets;
        }
        return cachedTypeTryGets;
      }
      Dictionary<string, TypeGetBase> cachedTypeGets = this.Value._cachedTypeGets;
      if (cachedTypeGets == null)
      {
        Interlocked.CompareExchange<Dictionary<string, TypeGetBase>>(ref this.Value._cachedTypeGets, new Dictionary<string, TypeGetBase>(), (Dictionary<string, TypeGetBase>) null);
        cachedTypeGets = this.Value._cachedTypeGets;
      }
      return cachedTypeGets;
    }

    protected override void AddOldClassAccess(PythonType pt)
    {
      this._gets.Add(new FastGetDelegate(new MetaPythonType.FastGetBinderHelper.OldClassDelegate(this.Value, pt, this._binder.Name).Target));
    }

    protected override bool AddSlotAccess(PythonType pt, PythonTypeSlot pts)
    {
      if (pts.CanOptimizeGets)
        this._canOptimize = true;
      if (pts.IsAlwaysVisible)
      {
        this._gets.Add(new FastGetDelegate(new MetaPythonType.FastGetBinderHelper.SlotAccessDelegate(pts, this.Value).Target));
        return pts.GetAlwaysSucceeds;
      }
      this._gets.Add(new FastGetDelegate(new MetaPythonType.FastGetBinderHelper.SlotAccessDelegate(pts, this.Value).TargetCheckCls));
      return false;
    }

    protected override void AddMetaOldClassAccess()
    {
      this._gets.Add(new FastGetDelegate(new MetaPythonType.FastGetBinderHelper.MetaOldClassDelegate(this._binder.Name).Target));
    }

    protected override void AddError()
    {
      if (this._binder.IsNoThrow)
        this._gets.Add(new FastGetDelegate(new MetaPythonType.FastGetBinderHelper.ErrorBinder(this._binder.Name).TargetNoThrow));
      else
        this._gets.Add(new FastGetDelegate(new MetaPythonType.FastGetBinderHelper.ErrorBinder(this._binder.Name).Target));
    }

    protected override void AddMetaGetAttribute(PythonType metaType, PythonTypeSlot pts)
    {
      this._gets.Add(new FastGetDelegate(new MetaPythonType.FastGetBinderHelper.MetaGetAttributeDelegate(this._context, pts, metaType, this._binder.Name).Target));
    }

    protected override bool AddMetaSlotAccess(PythonType metaType, PythonTypeSlot pts)
    {
      if (pts.CanOptimizeGets)
        this._canOptimize = true;
      if (pts.IsAlwaysVisible)
      {
        this._gets.Add(new FastGetDelegate(new MetaPythonType.FastGetBinderHelper.SlotAccessDelegate(pts, metaType).MetaTarget));
        return pts.GetAlwaysSucceeds;
      }
      this._gets.Add(new FastGetDelegate(new MetaPythonType.FastGetBinderHelper.SlotAccessDelegate(pts, metaType).MetaTargetCheckCls));
      return false;
    }

    protected override TypeGetBase Finish(bool metaOnly)
    {
      return metaOnly ? (DynamicHelpers.GetPythonType((object) this.Value).IsSystemType ? (TypeGetBase) new SystemTypeGet(this._binder, this._gets.ToArray(), this.Value, metaOnly, this._canOptimize) : (TypeGetBase) new TypeGet(this._binder, this._gets.ToArray(), metaOnly ? this._metaVersion : this._version, metaOnly, this._canOptimize)) : (this.Value.IsSystemType ? (TypeGetBase) new SystemTypeGet(this._binder, this._gets.ToArray(), this.Value, metaOnly, this._canOptimize) : (TypeGetBase) new TypeGet(this._binder, this._gets.ToArray(), metaOnly ? this._metaVersion : this._version, metaOnly, this._canOptimize));
    }

    private class OldClassDelegate
    {
      private readonly WeakReference _type;
      private readonly WeakReference _declType;
      private readonly string _name;

      public OldClassDelegate(PythonType declType, PythonType oldClass, string name)
      {
        this._type = oldClass.GetSharedWeakReference();
        this._declType = declType.GetSharedWeakReference();
        this._name = name;
      }

      public bool Target(CodeContext context, object self, out object result)
      {
        return PythonOps.OldClassTryLookupOneSlot((PythonType) this._declType.Target, ((PythonType) this._type.Target).OldClass, this._name, out result);
      }
    }

    private class SlotAccessDelegate
    {
      private readonly PythonTypeSlot _slot;
      private readonly PythonType _owner;
      private readonly WeakReference _weakOwner;
      private readonly WeakReference _weakSlot;

      public SlotAccessDelegate(PythonTypeSlot slot, PythonType owner)
      {
        if (owner.IsSystemType)
        {
          this._owner = owner;
          this._slot = slot;
        }
        else
        {
          this._weakOwner = owner.GetSharedWeakReference();
          this._weakSlot = new WeakReference((object) slot);
        }
      }

      public bool TargetCheckCls(CodeContext context, object self, out object result)
      {
        if (PythonOps.IsClsVisible(context))
          return this.Slot.TryGetValue(context, (object) null, this.Type, out result);
        result = (object) null;
        return false;
      }

      public bool Target(CodeContext context, object self, out object result)
      {
        return this.Slot.TryGetValue(context, (object) null, this.Type, out result);
      }

      public bool MetaTargetCheckCls(CodeContext context, object self, out object result)
      {
        if (PythonOps.IsClsVisible(context))
          return this.Slot.TryGetValue(context, self, this.Type, out result);
        result = (object) null;
        return false;
      }

      public bool MetaTarget(CodeContext context, object self, out object result)
      {
        return this.Slot.TryGetValue(context, self, this.Type, out result);
      }

      private PythonType Type => this._owner ?? (PythonType) this._weakOwner.Target;

      private PythonTypeSlot Slot => this._slot ?? (PythonTypeSlot) this._weakSlot.Target;
    }

    private class MetaOldClassDelegate
    {
      private readonly string _name;

      public MetaOldClassDelegate(string name) => this._name = name;

      public bool Target(CodeContext context, object self, out object result)
      {
        result = ((PythonType) self).__getattribute__(context, this._name);
        return true;
      }
    }

    private class MetaGetAttributeDelegate
    {
      private readonly string _name;
      private readonly PythonType _metaType;
      private readonly WeakReference _weakMetaType;
      private readonly PythonTypeSlot _slot;
      private readonly WeakReference _weakSlot;
      private readonly CallSite<Func<CallSite, CodeContext, object, string, object>> _invokeSite;

      public MetaGetAttributeDelegate(
        CodeContext context,
        PythonTypeSlot slot,
        PythonType metaType,
        string name)
      {
        this._name = name;
        if (metaType.IsSystemType)
        {
          this._metaType = metaType;
          this._slot = slot;
        }
        else
        {
          this._weakMetaType = metaType.GetSharedWeakReference();
          this._weakSlot = new WeakReference((object) slot);
        }
        this._invokeSite = CallSite<Func<CallSite, CodeContext, object, string, object>>.Create((CallSiteBinder) context.LanguageContext.InvokeOne);
      }

      public bool Target(CodeContext context, object self, out object result)
      {
        object obj;
        if (this.Slot.TryGetValue(context, self, this.MetaType, out obj))
        {
          result = this._invokeSite.Target((CallSite) this._invokeSite, context, obj, this._name);
          return true;
        }
        result = (object) null;
        return false;
      }

      private PythonType MetaType => this._metaType ?? (PythonType) this._weakMetaType.Target;

      private PythonTypeSlot Slot => this._slot ?? (PythonTypeSlot) this._weakSlot.Target;
    }

    private class ErrorBinder
    {
      private readonly string _name;

      public ErrorBinder(string name) => this._name = name;

      public bool TargetNoThrow(CodeContext context, object self, out object result)
      {
        result = (object) OperationFailed.Value;
        return true;
      }

      public bool Target(CodeContext context, object self, out object result)
      {
        throw PythonOps.AttributeErrorForObjectMissingAttribute(self, this._name);
      }
    }
  }
}
