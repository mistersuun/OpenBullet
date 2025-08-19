// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.BuiltinFunction
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Modules;
using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Runtime.Types;

[PythonType("builtin_function_or_method")]
[DontMapGetMemberNamesToDir]
public class BuiltinFunction : 
  PythonTypeSlot,
  ICodeFormattable,
  IDynamicMetaObjectProvider,
  IDelegateConvertible,
  IFastInvokable
{
  internal readonly BuiltinFunction.BuiltinFunctionData _data;
  internal readonly object _instance;
  private static readonly object _noInstance = new object();

  internal static BuiltinFunction MakeFunction(string name, MethodBase[] infos, Type declaringType)
  {
    return new BuiltinFunction(name, infos, declaringType, FunctionType.Function | FunctionType.AlwaysVisible);
  }

  internal static BuiltinFunction MakeMethod(
    string name,
    MethodBase[] infos,
    Type declaringType,
    FunctionType ft)
  {
    foreach (MethodBase info in infos)
    {
      if (info.ContainsGenericParameters)
        return (BuiltinFunction) new GenericBuiltinFunction(name, infos, declaringType, ft);
    }
    return new BuiltinFunction(name, infos, declaringType, ft);
  }

  internal virtual BuiltinFunction BindToInstance(object instance)
  {
    return new BuiltinFunction(instance, this._data);
  }

  internal BuiltinFunction(
    string name,
    MethodBase[] originalTargets,
    Type declaringType,
    FunctionType functionType)
  {
    this._data = new BuiltinFunction.BuiltinFunctionData(name, originalTargets, declaringType, functionType);
    this._instance = BuiltinFunction._noInstance;
  }

  internal BuiltinFunction(object instance, BuiltinFunction.BuiltinFunctionData data)
  {
    this._instance = instance;
    this._data = data;
  }

  internal void AddMethod(MethodInfo mi) => this._data.AddMethod((MethodBase) mi);

  internal bool TestData(object data) => this._data == data;

  internal bool IsUnbound => this._instance == BuiltinFunction._noInstance;

  internal string Name
  {
    get => this._data.Name;
    set => this._data.Name = value;
  }

  internal object Call(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], object>>> storage,
    object instance,
    object[] args)
  {
    storage = BuiltinFunction.GetInitializedStorage(context, storage);
    object obj;
    if (!this.GetDescriptor().TryGetValue(context, instance, DynamicHelpers.GetPythonTypeFromType(this.DeclaringType), out obj))
      obj = (object) this;
    return storage.Data.Target((CallSite) storage.Data, context, obj, args);
  }

  internal object Call0(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object>>> storage,
    object instance)
  {
    storage = BuiltinFunction.GetInitializedStorage(context, storage);
    object obj;
    if (!this.GetDescriptor().TryGetValue(context, instance, DynamicHelpers.GetPythonTypeFromType(this.DeclaringType), out obj))
      obj = (object) this;
    return storage.Data.Target((CallSite) storage.Data, context, obj);
  }

  private static SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], object>>> GetInitializedStorage(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], object>>> storage)
  {
    if (storage == null)
      storage = context.LanguageContext.GetGenericCallSiteStorage();
    if (storage.Data == null)
      storage.Data = context.LanguageContext.MakeSplatSite();
    return storage;
  }

  private static SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object>>> GetInitializedStorage(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object>>> storage)
  {
    if (storage.Data == null)
      storage.Data = CallSite<Func<CallSite, CodeContext, object, object>>.Create((CallSiteBinder) context.LanguageContext.InvokeNone);
    return storage;
  }

  internal object Call(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], IDictionary<object, object>, object>>> storage,
    object instance,
    object[] args,
    IDictionary<object, object> keywordArgs)
  {
    if (storage == null)
      storage = context.LanguageContext.GetGenericKeywordCallSiteStorage();
    if (storage.Data == null)
      storage.Data = context.LanguageContext.MakeKeywordSplatSite();
    return instance != null ? storage.Data.Target((CallSite) storage.Data, context, (object) this, ArrayUtils.Insert<object>(instance, args), keywordArgs) : storage.Data.Target((CallSite) storage.Data, context, (object) this, args, keywordArgs);
  }

  internal BuiltinFunction MakeGenericMethod(Type[] types)
  {
    BuiltinFunction.TypeList key = new BuiltinFunction.TypeList(types);
    if (this._data.BoundGenerics != null)
    {
      lock (this._data.BoundGenerics)
      {
        BuiltinFunction builtinFunction;
        if (this._data.BoundGenerics.TryGetValue(key, out builtinFunction))
          return builtinFunction;
      }
    }
    List<MethodBase> methodBaseList = new List<MethodBase>(this.Targets.Count);
    foreach (MethodBase target in (IEnumerable<MethodBase>) this.Targets)
    {
      MethodInfo methodInfo = target as MethodInfo;
      if (!(methodInfo == (MethodInfo) null) && methodInfo.ContainsGenericParameters && methodInfo.GetGenericArguments().Length == types.Length)
        methodBaseList.Add((MethodBase) methodInfo.MakeGenericMethod(types));
    }
    if (methodBaseList.Count == 0)
      return (BuiltinFunction) null;
    BuiltinFunction builtinFunction1 = new BuiltinFunction(this.Name, methodBaseList.ToArray(), this.DeclaringType, this.FunctionType);
    this.EnsureBoundGenericDict();
    lock (this._data.BoundGenerics)
      this._data.BoundGenerics[key] = builtinFunction1;
    return builtinFunction1;
  }

  internal PythonTypeSlot GetDescriptor()
  {
    return (this.FunctionType & FunctionType.Method) != FunctionType.None ? (PythonTypeSlot) new BuiltinMethodDescriptor(this) : (PythonTypeSlot) this;
  }

  public Type DeclaringType
  {
    [PythonHidden(new PlatformID[] {})] get => this._data.DeclaringType;
  }

  public IList<MethodBase> Targets
  {
    [PythonHidden(new PlatformID[] {})] get => (IList<MethodBase>) this._data.Targets;
  }

  internal override bool IsAlwaysVisible => (this._data.Type & FunctionType.AlwaysVisible) != 0;

  internal bool IsReversedOperator => (this.FunctionType & FunctionType.ReversedOperator) != 0;

  internal bool IsBinaryOperator => (this.FunctionType & FunctionType.BinaryOperator) != 0;

  internal FunctionType FunctionType
  {
    get => this._data.Type;
    set => this._data.Type = value;
  }

  internal System.Linq.Expressions.Expression MakeBoundFunctionTest(System.Linq.Expressions.Expression functionTarget)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (PythonOps).GetMethod("TestBoundBuiltinFunction"), functionTarget, (System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.Constant((object) this._data, typeof (object)));
  }

  internal override bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    value = (object) this;
    return true;
  }

  internal override bool GetAlwaysSucceeds => true;

  internal override void MakeGetExpression(
    PythonBinder binder,
    System.Linq.Expressions.Expression codeContext,
    DynamicMetaObject instance,
    DynamicMetaObject owner,
    IronPython.Runtime.Binding.ConditionalBuilder builder)
  {
    builder.FinishCondition((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) this));
  }

  public string __repr__(CodeContext context)
  {
    return this.IsUnbound || this.IsBuiltinModuleMethod ? $"<built-in function {this.Name}>" : $"<built-in method {this.__name__} of {PythonOps.GetPythonTypeName(this.__self__)} object at {PythonOps.HexId(this.__self__)}>";
  }

  DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(System.Linq.Expressions.Expression parameter)
  {
    return (DynamicMetaObject) new MetaBuiltinFunction(parameter, BindingRestrictions.Empty, this);
  }

  internal DynamicMetaObject MakeBuiltinFunctionCall(
    DynamicMetaObjectBinder call,
    System.Linq.Expressions.Expression codeContext,
    DynamicMetaObject function,
    DynamicMetaObject[] args,
    bool hasSelf,
    BindingRestrictions functionRestriction,
    Func<DynamicMetaObject[], BuiltinFunction.BindingResult> bind)
  {
    DynamicMetaObject dynamicMetaObject = BuiltinFunction.TranslateArguments(call, codeContext, new DynamicMetaObject(function.Expression, functionRestriction, function.Value), args, hasSelf, this.Name);
    if (dynamicMetaObject != null)
      return dynamicMetaObject;
    if (this.IsReversedOperator)
      ArrayUtils.SwapLastTwo<DynamicMetaObject>(args);
    BuiltinFunction.BindingResult bindingResult = bind(args);
    BindingTarget target = bindingResult.Target;
    DynamicMetaObject result = bindingResult.MetaObject;
    if (target.Overload != null && target.Overload.IsProtected)
      result = new DynamicMetaObject(BindingHelpers.TypeErrorForProtectedMember(target.Overload.DeclaringType, target.Overload.Name), result.Restrictions);
    else if (this.IsBinaryOperator && args.Length == 2 && BuiltinFunction.IsThrowException(result.Expression))
      result = new DynamicMetaObject((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Property((System.Linq.Expressions.Expression) null, typeof (PythonOps), "NotImplemented"), result.Restrictions);
    else if (target.Overload != null && call is IPythonSite pythonSite)
    {
      PythonContext context = pythonSite.Context;
      if (context.Options is PythonOptions options && options.EnableProfiler)
        result = new DynamicMetaObject(Profiler.GetProfiler(context).AddProfiling(result.Expression, target.Overload.ReflectionInfo), result.Restrictions);
    }
    WarningInfo info;
    if (target.Overload != null && BindingWarnings.ShouldWarn(PythonContext.GetPythonContext(call), target.Overload, out info))
      result = info.AddWarning(codeContext, result);
    DynamicMetaObject res = new DynamicMetaObject(result.Expression, functionRestriction.Merge(result.Restrictions));
    if (res.Expression.Type.IsValueType())
      res = BindingHelpers.AddPythonBoxing(res);
    else if (res.Expression.Type == typeof (void))
      res = new DynamicMetaObject((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block(res.Expression, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) null)), res.Restrictions);
    return res;
  }

  internal static DynamicMetaObject TranslateArguments(
    DynamicMetaObjectBinder call,
    System.Linq.Expressions.Expression codeContext,
    DynamicMetaObject function,
    DynamicMetaObject[] args,
    bool hasSelf,
    string name)
  {
    if (hasSelf)
      args = ArrayUtils.RemoveFirst<DynamicMetaObject>(args);
    CallSignature callSignature = BindingHelpers.GetCallSignature(call);
    if (callSignature.HasDictionaryArgument())
    {
      int index = callSignature.IndexOf(ArgumentType.Dictionary);
      DynamicMetaObject self = args[index];
      if (!(self.Value is IDictionary) && self.Value != null)
      {
        DynamicMetaObject[] dynamicMetaObjectArray = ArrayUtils.Insert<DynamicMetaObject>(function, args);
        dynamicMetaObjectArray[index + 1] = new DynamicMetaObject((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (PythonOps).GetMethod("UserMappingToPythonDictionary"), codeContext, args[index].Expression, Microsoft.Scripting.Ast.Utils.Constant((object) name)), BindingRestrictionsHelpers.GetRuntimeTypeRestriction(self.Expression, self.GetLimitType()), (object) PythonOps.UserMappingToPythonDictionary(PythonContext.GetPythonContext(call).SharedContext, self.Value, name));
        if (call is IPythonSite)
          dynamicMetaObjectArray = ArrayUtils.Insert<DynamicMetaObject>(new DynamicMetaObject(codeContext, BindingRestrictions.Empty), dynamicMetaObjectArray);
        return new DynamicMetaObject((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Dynamic((CallSiteBinder) call, typeof (object), DynamicUtils.GetExpressions(dynamicMetaObjectArray)), BindingRestrictions.Combine((IList<DynamicMetaObject>) dynamicMetaObjectArray).Merge(BindingRestrictionsHelpers.GetRuntimeTypeRestriction(self.Expression, self.GetLimitType())));
      }
    }
    if (callSignature.HasListArgument())
    {
      int index = callSignature.IndexOf(ArgumentType.List);
      DynamicMetaObject self = args[index];
      if (!(self.Value is IList<object>) && self.Value is IEnumerable)
      {
        DynamicMetaObject[] dynamicMetaObjectArray = ArrayUtils.Insert<DynamicMetaObject>(function, args);
        dynamicMetaObjectArray[index + 1] = new DynamicMetaObject((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (PythonOps).GetMethod("MakeTupleFromSequence"), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Convert(args[index].Expression, typeof (object))), BindingRestrictions.Empty);
        if (call is IPythonSite)
          dynamicMetaObjectArray = ArrayUtils.Insert<DynamicMetaObject>(new DynamicMetaObject(codeContext, BindingRestrictions.Empty), dynamicMetaObjectArray);
        return new DynamicMetaObject((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Dynamic((CallSiteBinder) call, typeof (object), DynamicUtils.GetExpressions(dynamicMetaObjectArray)), function.Restrictions.Merge(BindingRestrictions.Combine((IList<DynamicMetaObject>) args).Merge(BindingRestrictionsHelpers.GetRuntimeTypeRestriction(self.Expression, self.GetLimitType()))));
      }
    }
    return (DynamicMetaObject) null;
  }

  private static bool IsThrowException(System.Linq.Expressions.Expression expr)
  {
    if (expr.NodeType == ExpressionType.Throw)
      return true;
    if (expr.NodeType == ExpressionType.Convert)
      return BuiltinFunction.IsThrowException(((UnaryExpression) expr).Operand);
    if (expr.NodeType == ExpressionType.Block)
    {
      foreach (System.Linq.Expressions.Expression expression in ((BlockExpression) expr).Expressions)
      {
        if (BuiltinFunction.IsThrowException(expression))
          return true;
      }
    }
    return false;
  }

  public static object __new__(object cls, object newFunction, object inst)
  {
    return (object) new Method(newFunction, inst, (object) null);
  }

  public int __cmp__(CodeContext context, [NotNull] BuiltinFunction other)
  {
    if (other == this)
      return 0;
    if (!this.IsUnbound && !other.IsUnbound)
    {
      int num = PythonOps.Compare(this.__self__, other.__self__);
      if (num != 0)
        return num;
      if (this._data == other._data)
        return 0;
    }
    int num1 = string.CompareOrdinal(this.__name__, other.__name__);
    if (num1 != 0)
      return num1;
    int num2 = string.CompareOrdinal(this.Get__module__(context), other.Get__module__(context));
    if (num2 != 0)
      return num2;
    return IdDispenser.GetId((object) this) - IdDispenser.GetId((object) other) <= 0L ? -1 : 1;
  }

  [Python3Warning("builtin_function_or_method order comparisons not supported in 3.x")]
  public static NotImplementedType operator >(BuiltinFunction self, BuiltinFunction other)
  {
    return PythonOps.NotImplemented;
  }

  [Python3Warning("builtin_function_or_method order comparisons not supported in 3.x")]
  public static NotImplementedType operator <(BuiltinFunction self, BuiltinFunction other)
  {
    return PythonOps.NotImplemented;
  }

  [Python3Warning("builtin_function_or_method order comparisons not supported in 3.x")]
  public static NotImplementedType operator >=(BuiltinFunction self, BuiltinFunction other)
  {
    return PythonOps.NotImplemented;
  }

  [Python3Warning("builtin_function_or_method order comparisons not supported in 3.x")]
  public static NotImplementedType operator <=(BuiltinFunction self, BuiltinFunction other)
  {
    return PythonOps.NotImplemented;
  }

  public int __hash__(CodeContext context)
  {
    return PythonOps.Hash(context, this._instance) ^ PythonOps.Hash(context, (object) this._data);
  }

  [PropertyMethod]
  [SpecialName]
  public string Get__module__(CodeContext context)
  {
    if (this.Targets.Count > 0)
    {
      PythonType pythonTypeFromType = DynamicHelpers.GetPythonTypeFromType(this.DeclaringType);
      string moduleName = PythonTypeOps.GetModuleName(context, pythonTypeFromType.UnderlyingSystemType);
      if (moduleName != "__builtin__" || this.DeclaringType == typeof (Builtin))
        return moduleName;
    }
    return (string) null;
  }

  [PropertyMethod]
  [SpecialName]
  public void Set__module__(string value)
  {
  }

  public virtual BuiltinFunctionOverloadMapper Overloads
  {
    [PythonHidden(new PlatformID[] {})] get
    {
      return new BuiltinFunctionOverloadMapper(this, this.IsUnbound ? (object) null : this._instance);
    }
  }

  internal Dictionary<BuiltinFunction.TypeList, BuiltinFunction> OverloadDictionary
  {
    get
    {
      if (this._data.OverloadDictionary == null)
        Interlocked.CompareExchange<Dictionary<BuiltinFunction.TypeList, BuiltinFunction>>(ref this._data.OverloadDictionary, new Dictionary<BuiltinFunction.TypeList, BuiltinFunction>(), (Dictionary<BuiltinFunction.TypeList, BuiltinFunction>) null);
      return this._data.OverloadDictionary;
    }
  }

  public string __name__ => this.Name;

  public virtual string __doc__
  {
    get
    {
      StringBuilder stringBuilder = new StringBuilder();
      IList<MethodBase> targets = this.Targets;
      for (int index = 0; index < targets.Count; ++index)
      {
        if (targets[index] != (MethodBase) null)
        {
          if (this.IsBuiltinModuleMethod)
            stringBuilder.Append(DocBuilder.DocOneInfo(targets[index], this.Name, false));
          else
            stringBuilder.Append(DocBuilder.DocOneInfo(targets[index], this.Name));
        }
      }
      return stringBuilder.ToString();
    }
  }

  public object __self__
  {
    get => this.IsUnbound || this.IsBuiltinModuleMethod ? (object) null : this._instance;
  }

  internal object BindingSelf => this.IsUnbound ? (object) null : this._instance;

  private bool IsBuiltinModuleMethod => (this.FunctionType & FunctionType.ModuleMethod) != 0;

  public object __call__(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], IDictionary<object, object>, object>>> storage,
    [ParamDictionary] IDictionary<object, object> dictArgs,
    params object[] args)
  {
    return this.Call(context, storage, (object) null, args, dictArgs);
  }

  internal virtual bool IsOnlyGeneric => false;

  private BinderType BinderType
  {
    get => !this.IsBinaryOperator ? BinderType.Normal : BinderType.BinaryOperator;
  }

  private void EnsureBoundGenericDict()
  {
    if (this._data.BoundGenerics != null)
      return;
    Interlocked.CompareExchange<Dictionary<BuiltinFunction.TypeList, BuiltinFunction>>(ref this._data.BoundGenerics, new Dictionary<BuiltinFunction.TypeList, BuiltinFunction>(1), (Dictionary<BuiltinFunction.TypeList, BuiltinFunction>) null);
  }

  Delegate IDelegateConvertible.ConvertToDelegate(Type type)
  {
    ParameterInfo[] parameters1 = type.GetMethod("Invoke").GetParameters();
    if (this.Targets.Count == 1)
    {
      MethodInfo target = this.Targets[0] as MethodInfo;
      if (target != (MethodInfo) null)
      {
        ParameterInfo[] parameters2 = target.GetParameters();
        if (parameters2.Length == parameters1.Length)
        {
          bool flag = true;
          for (int index = 0; index < parameters2.Length; ++index)
          {
            if (parameters1[index].ParameterType != parameters2[index].ParameterType)
            {
              flag = false;
              break;
            }
          }
          if (flag)
            return this.IsUnbound ? target.CreateDelegate(type) : target.CreateDelegate(type, this._instance);
        }
      }
    }
    return (Delegate) null;
  }

  FastBindResult<T> IFastInvokable.MakeInvokeBinding<T>(
    CallSite<T> site,
    PythonInvokeBinder binder,
    CodeContext state,
    object[] args)
  {
    return new FastBindResult<T>(binder.LightBind<T>(ArrayUtils.Insert<object>((object) state, (object) this, args), 100), true);
  }

  internal class BindingResult
  {
    public readonly BindingTarget Target;
    public readonly DynamicMetaObject MetaObject;

    public BindingResult(BindingTarget target, DynamicMetaObject meta)
    {
      this.Target = target;
      this.MetaObject = meta;
    }
  }

  internal class TypeList
  {
    private Type[] _types;

    public TypeList(Type[] types) => this._types = types;

    public override bool Equals(object obj)
    {
      if (!(obj is BuiltinFunction.TypeList typeList) || this._types.Length != typeList._types.Length)
        return false;
      for (int index = 0; index < this._types.Length; ++index)
      {
        if (this._types[index] != typeList._types[index])
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      int hashCode = 6551;
      foreach (Type type in this._types)
        hashCode = hashCode << 5 ^ type.GetHashCode();
      return hashCode;
    }
  }

  internal sealed class BuiltinFunctionData
  {
    public string Name;
    public MethodBase[] Targets;
    public readonly Type DeclaringType;
    public FunctionType Type;
    public Dictionary<BuiltinFunction.TypeList, BuiltinFunction> BoundGenerics;
    public Dictionary<BuiltinFunction.TypeList, BuiltinFunction> OverloadDictionary;

    public BuiltinFunctionData(
      string name,
      MethodBase[] targets,
      Type declType,
      FunctionType functionType)
    {
      this.Name = name;
      this.Targets = targets;
      this.DeclaringType = declType;
      this.Type = functionType;
    }

    internal void AddMethod(MethodBase info)
    {
      MethodBase[] methodBaseArray = new MethodBase[this.Targets.Length + 1];
      this.Targets.CopyTo((Array) methodBaseArray, 0);
      methodBaseArray[this.Targets.Length] = info;
      this.Targets = methodBaseArray;
    }
  }
}
