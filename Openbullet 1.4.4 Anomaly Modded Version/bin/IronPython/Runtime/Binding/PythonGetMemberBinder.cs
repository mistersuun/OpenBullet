// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.PythonGetMemberBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.ComInterop;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Interpreter;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class PythonGetMemberBinder : 
  DynamicMetaObjectBinder,
  IPythonSite,
  IExpressionSerializable,
  ILightExceptionBinder
{
  private readonly PythonContext _context;
  private readonly GetMemberOptions _options;
  private readonly string _name;
  private PythonGetMemberBinder.LightThrowBinder _lightThrowBinder;

  public PythonGetMemberBinder(PythonContext context, string name)
  {
    this._context = context;
    this._name = name;
  }

  public PythonGetMemberBinder(PythonContext context, string name, bool isNoThrow)
    : this(context, name)
  {
    this._options = isNoThrow ? GetMemberOptions.IsNoThrow : GetMemberOptions.None;
  }

  public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
  {
    DynamicMetaObject codeContext = args[0];
    if (target is IPythonGetable pythonGetable)
      return pythonGetable.GetMember(this, codeContext);
    return target.Value is IDynamicMetaObjectProvider || ComBinder.IsComObject(target.Value) ? this.GetForeignObject(target) : this.Fallback(target, codeContext);
  }

  public override T BindDelegate<T>(CallSite<T> site, object[] args)
  {
    if (args[0] is IFastGettable fastGettable)
      return fastGettable.MakeGetBinding<T>(site, this, (CodeContext) args[1], this.Name) ?? base.BindDelegate<T>(site, args);
    if (args[0] is IPythonObject self && !(args[0] is IProxyObject))
    {
      FastBindResult<T> binding = UserTypeOps.MakeGetBinding<T>((CodeContext) args[1], site, self, this);
      if ((object) binding.Target == null)
        return base.BindDelegate<T>(site, args);
      if (binding.ShouldCache)
        this.CacheTarget<T>(binding.Target);
      return binding.Target;
    }
    if (args[0] != null)
    {
      if (args[0].GetType() == typeof (PythonModule))
      {
        if (this.SupportsLightThrow)
          return (T) new Func<CallSite, object, CodeContext, object>(new PythonGetMemberBinder.PythonModuleDelegate(this._name).LightThrowTarget);
        return !this.IsNoThrow ? (T) new Func<CallSite, object, CodeContext, object>(new PythonGetMemberBinder.PythonModuleDelegate(this._name).Target) : (T) new Func<CallSite, object, CodeContext, object>(new PythonGetMemberBinder.PythonModuleDelegate(this._name).NoThrowTarget);
      }
      if (args[0].GetType() == typeof (NamespaceTracker))
      {
        switch (this.Name)
        {
          case "__str__":
          case "__repr__":
          case "__doc__":
            break;
          case "__file__":
            return (T) new Func<CallSite, object, CodeContext, object>(new PythonGetMemberBinder.NamespaceTrackerDelegate(this._name).GetFile);
          case "__dict__":
            return (T) new Func<CallSite, object, CodeContext, object>(new PythonGetMemberBinder.NamespaceTrackerDelegate(this._name).GetDict);
          case "__name__":
            return (T) new Func<CallSite, object, CodeContext, object>(new PythonGetMemberBinder.NamespaceTrackerDelegate(this._name).GetName);
          default:
            return this.IsNoThrow ? (T) new Func<CallSite, object, CodeContext, object>(new PythonGetMemberBinder.NamespaceTrackerDelegate(this._name).NoThrowTarget) : (T) new Func<CallSite, object, CodeContext, object>(new PythonGetMemberBinder.NamespaceTrackerDelegate(this._name).Target);
        }
      }
    }
    if (args[0] == null || ComBinder.IsComObject(args[0]) || args[0] is IDynamicMetaObjectProvider)
      return this.LightBind<T>(args, this.Context.Options.CompilationThreshold);
    Type parameterType = typeof (T).GetMethod("Invoke").GetParameters()[1].ParameterType;
    CodeContext context = (CodeContext) args[1];
    T obj = default (T);
    if (parameterType == typeof (object))
      obj = (T) this.MakeGetMemberTarget<object>(this.Name, args[0], context);
    else if (parameterType == typeof (List))
      obj = (T) this.MakeGetMemberTarget<List>(this.Name, args[0], context);
    else if (parameterType == typeof (string))
      obj = (T) this.MakeGetMemberTarget<string>(this.Name, args[0], context);
    return (object) obj != null ? obj : base.BindDelegate<T>(site, args);
  }

  private Func<CallSite, TSelfType, CodeContext, object> MakeGetMemberTarget<TSelfType>(
    string name,
    object target,
    CodeContext context)
  {
    Type type = CompilerHelpers.GetType(target);
    if (typeof (TypeTracker).IsAssignableFrom(type))
      return (Func<CallSite, TSelfType, CodeContext, object>) null;
    MemberGroup member = this.Context.Binder.GetMember(MemberRequestKind.Get, type, name);
    if (member.Count == 0 && type.IsInterface())
    {
      type = typeof (object);
      member = this.Context.Binder.GetMember(MemberRequestKind.Get, type, name);
    }
    if (member.Count == 0 && typeof (IStrongBox).IsAssignableFrom(type))
      return (Func<CallSite, TSelfType, CodeContext, object>) null;
    MethodInfo method1 = this.Context.Binder.GetMethod(type, "GetCustomMember");
    if (method1 != (MethodInfo) null && method1.IsSpecialName)
      return (Func<CallSite, TSelfType, CodeContext, object>) null;
    Expression error;
    TrackerTypes memberType = this.Context.Binder.GetMemberType(member, out error);
    if (error == null)
    {
      if (DynamicHelpers.GetPythonTypeFromType(type).IsHiddenMember(name))
        return (Func<CallSite, TSelfType, CodeContext, object>) null;
      switch (memberType)
      {
        case TrackerTypes.Event:
          if (member.Count != 1 || ((EventTracker) member[0]).IsStatic)
            return (Func<CallSite, TSelfType, CodeContext, object>) null;
          PythonTypeSlot slot1 = PythonTypeOps.GetSlot(member, name, this._context.DomainManager.Configuration.PrivateBinding);
          return new Func<CallSite, TSelfType, CodeContext, object>(new PythonGetMemberBinder.FastSlotGet<TSelfType>(type, slot1, DynamicHelpers.GetPythonTypeFromType(member[0].DeclaringType)).GetBindSlot);
        case TrackerTypes.Method:
          PythonTypeSlot slot2 = PythonTypeOps.GetSlot(member, name, this._context.DomainManager.Configuration.PrivateBinding);
          switch (slot2)
          {
            case BuiltinMethodDescriptor _:
              return new Func<CallSite, TSelfType, CodeContext, object>(new PythonGetMemberBinder.FastMethodGet<TSelfType>(type, (BuiltinMethodDescriptor) slot2).GetMethod);
            case BuiltinFunction _:
              return new Func<CallSite, TSelfType, CodeContext, object>(new PythonGetMemberBinder.FastSlotGet<TSelfType>(type, slot2, DynamicHelpers.GetPythonTypeFromType(type)).GetRetSlot);
            default:
              return new Func<CallSite, TSelfType, CodeContext, object>(new PythonGetMemberBinder.FastSlotGet<TSelfType>(type, slot2, DynamicHelpers.GetPythonTypeFromType(type)).GetBindSlot);
          }
        case TrackerTypes.Property:
          if (member.Count == 1)
          {
            PropertyTracker propertyTracker = (PropertyTracker) member[0];
            if (!propertyTracker.IsStatic && propertyTracker.GetIndexParameters().Length == 0)
            {
              MethodInfo getMethod = propertyTracker.GetGetMethod();
              ParameterInfo[] parameters;
              if (getMethod != (MethodInfo) null && (parameters = getMethod.GetParameters()).Length == 0)
              {
                if (getMethod.ReturnType == typeof (bool))
                  return new Func<CallSite, TSelfType, CodeContext, object>(new PythonGetMemberBinder.FastPropertyGet<TSelfType>(type, new Func<object, object>(CallInstruction.Create(getMethod, parameters).Invoke)).GetPropertyBool);
                return getMethod.ReturnType == typeof (int) ? new Func<CallSite, TSelfType, CodeContext, object>(new PythonGetMemberBinder.FastPropertyGet<TSelfType>(type, new Func<object, object>(CallInstruction.Create(getMethod, parameters).Invoke)).GetPropertyInt) : new Func<CallSite, TSelfType, CodeContext, object>(new PythonGetMemberBinder.FastPropertyGet<TSelfType>(type, new Func<object, object>(CallInstruction.Create(getMethod, parameters).Invoke)).GetProperty);
              }
            }
          }
          return (Func<CallSite, TSelfType, CodeContext, object>) null;
        case TrackerTypes.Type:
        case TrackerTypes.TypeGroup:
          object pythonType;
          if (member.Count == 1)
          {
            pythonType = (object) DynamicHelpers.GetPythonTypeFromType(((TypeTracker) member[0]).Type);
          }
          else
          {
            TypeTracker existingTypeEntity = (TypeTracker) member[0];
            for (int index = 1; index < member.Count; ++index)
              existingTypeEntity = TypeGroup.UpdateTypeEntity(existingTypeEntity, (TypeTracker) member[index]);
            pythonType = (object) existingTypeEntity;
          }
          return new Func<CallSite, TSelfType, CodeContext, object>(new PythonGetMemberBinder.FastTypeGet<TSelfType>(type, pythonType).GetTypeObject);
        case TrackerTypes.All:
          MethodInfo method2 = this.Context.Binder.GetMethod(type, "GetBoundMember");
          if (method2 != (MethodInfo) null && method2.IsSpecialName)
            return (Func<CallSite, TSelfType, CodeContext, object>) null;
          if (member.Count != 0 || context.ModuleContext.ExtensionMethods.GetBinder(this._context).GetMember(MemberRequestKind.Get, type, name).Count != 0)
            return (Func<CallSite, TSelfType, CodeContext, object>) null;
          if (this.IsNoThrow)
            return new Func<CallSite, TSelfType, CodeContext, object>(new PythonGetMemberBinder.FastErrorGet<TSelfType>(type, name, context.ModuleContext.ExtensionMethods).GetErrorNoThrow);
          return this.SupportsLightThrow ? new Func<CallSite, TSelfType, CodeContext, object>(new PythonGetMemberBinder.FastErrorGet<TSelfType>(type, name, context.ModuleContext.ExtensionMethods).GetErrorLightThrow) : new Func<CallSite, TSelfType, CodeContext, object>(new PythonGetMemberBinder.FastErrorGet<TSelfType>(type, name, context.ModuleContext.ExtensionMethods).GetError);
        default:
          return (Func<CallSite, TSelfType, CodeContext, object>) null;
      }
    }
    else
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (MemberTracker memberTracker in member)
      {
        if (stringBuilder.Length != 0)
          stringBuilder.Append(", ");
        stringBuilder.Append((object) memberTracker.MemberType);
        stringBuilder.Append(" : ");
        stringBuilder.Append(memberTracker.ToString());
      }
      return new Func<CallSite, TSelfType, CodeContext, object>(new PythonGetMemberBinder.FastErrorGet<TSelfType>(type, stringBuilder.ToString(), context.ModuleContext.ExtensionMethods).GetAmbiguous);
    }
  }

  private DynamicMetaObject GetForeignObject(DynamicMetaObject self)
  {
    return new DynamicMetaObject((Expression) Expression.Dynamic((CallSiteBinder) this._context.CompatGetMember(this.Name, this.IsNoThrow), typeof (object), self.Expression), self.Restrictions.Merge(BindingRestrictionsHelpers.GetRuntimeTypeRestriction(self.Expression, self.GetLimitType())));
  }

  public DynamicMetaObject Fallback(DynamicMetaObject self, DynamicMetaObject codeContext)
  {
    return PythonGetMemberBinder.FallbackWorker(this._context, self, codeContext, this.Name, this._options, (DynamicMetaObjectBinder) this, (DynamicMetaObject) null);
  }

  public DynamicMetaObject Fallback(
    DynamicMetaObject self,
    DynamicMetaObject codeContext,
    DynamicMetaObject errorSuggestion)
  {
    return PythonGetMemberBinder.FallbackWorker(this._context, self, codeContext, this.Name, this._options, (DynamicMetaObjectBinder) this, errorSuggestion);
  }

  internal static DynamicMetaObject FallbackWorker(
    PythonContext context,
    DynamicMetaObject self,
    DynamicMetaObject codeContext,
    string name,
    GetMemberOptions options,
    DynamicMetaObjectBinder action,
    DynamicMetaObject errorSuggestion)
  {
    if (self.NeedsDeferral())
      return action.Defer(self);
    PythonOverloadResolverFactory resolverFactory = new PythonOverloadResolverFactory(context.Binder, codeContext.Expression);
    bool isNoThrow = (options & GetMemberOptions.IsNoThrow) != GetMemberOptions.None;
    Type limitType = self.GetLimitType();
    if ((limitType == typeof (DynamicNull) || PythonBinder.IsPythonType(limitType)) && DynamicHelpers.GetPythonTypeFromType(limitType).IsHiddenMember(name))
    {
      DynamicMetaObject member = PythonContext.GetPythonContext(action).Binder.GetMember(name, self, (OverloadResolverFactory) resolverFactory, isNoThrow, errorSuggestion);
      Expression failureExpression = PythonGetMemberBinder.GetFailureExpression(limitType, self, name, isNoThrow, action);
      return BindingHelpers.FilterShowCls(codeContext, action, member, failureExpression);
    }
    DynamicMetaObject dynamicMetaObject = context.Binder.GetMember(name, self, (OverloadResolverFactory) resolverFactory, isNoThrow, errorSuggestion);
    if (dynamicMetaObject is ErrorMetaObject)
    {
      ExtensionMethodSet extensionMethods = ((CodeContext) codeContext.Value).ModuleContext.ExtensionMethods;
      if (extensionMethods != (ExtensionMethodSet) null)
        dynamicMetaObject = extensionMethods.GetBinder(context).GetMember(name, self, (OverloadResolverFactory) resolverFactory, isNoThrow, errorSuggestion);
      dynamicMetaObject = new DynamicMetaObject(dynamicMetaObject.Expression, dynamicMetaObject.Restrictions.Merge(extensionMethods.GetRestriction(codeContext.Expression)));
    }
    if (dynamicMetaObject.Expression.Type.IsValueType())
      dynamicMetaObject = new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Convert(dynamicMetaObject.Expression, typeof (object)), dynamicMetaObject.Restrictions);
    return dynamicMetaObject;
  }

  private static Expression GetFailureExpression(
    Type limitType,
    DynamicMetaObject self,
    string name,
    bool isNoThrow,
    DynamicMetaObjectBinder action)
  {
    return !isNoThrow ? Microsoft.Scripting.Actions.DefaultBinder.MakeError(PythonContext.GetPythonContext(action).Binder.MakeMissingMemberError(limitType, self, name), typeof (object)).Expression : (Expression) Expression.Field((Expression) null, typeof (OperationFailed).GetField("Value"));
  }

  public string Name => this._name;

  public PythonContext Context => this._context;

  public bool IsNoThrow => (this._options & GetMemberOptions.IsNoThrow) != 0;

  public override int GetHashCode()
  {
    return (int) ((GetMemberOptions) (this._name.GetHashCode() ^ this._context.Binder.GetHashCode()) ^ this._options);
  }

  public override bool Equals(object obj)
  {
    return obj is PythonGetMemberBinder pythonGetMemberBinder && pythonGetMemberBinder._context.Binder == this._context.Binder && pythonGetMemberBinder._options == this._options && pythonGetMemberBinder._name == this._name;
  }

  public override string ToString()
  {
    return $"Python GetMember {this.Name} IsNoThrow: {this._options} LightThrow: {this.SupportsLightThrow}";
  }

  public Expression CreateExpression()
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeGetAction"), BindingHelpers.CreateBinderStateExpression(), Microsoft.Scripting.Ast.Utils.Constant((object) this.Name), Microsoft.Scripting.Ast.Utils.Constant((object) this.IsNoThrow));
  }

  public virtual bool SupportsLightThrow => false;

  public virtual CallSiteBinder GetLightExceptionBinder()
  {
    if (this._lightThrowBinder == null)
      this._lightThrowBinder = new PythonGetMemberBinder.LightThrowBinder(this._context, this.Name, this.IsNoThrow);
    return (CallSiteBinder) this._lightThrowBinder;
  }

  private class FastErrorGet<TSelfType> : FastGetBase
  {
    private readonly Type _type;
    private readonly string _name;
    private readonly ExtensionMethodSet _extMethods;

    public FastErrorGet(Type type, string name, ExtensionMethodSet extMethodSet)
    {
      this._type = type;
      this._name = name;
      this._extMethods = extMethodSet;
    }

    public override bool IsValid(PythonType type) => true;

    public object GetError(CallSite site, TSelfType target, CodeContext context)
    {
      if ((object) target != null && target.GetType() == this._type && (object) this._extMethods == (object) context.ModuleContext.ExtensionMethods)
        throw PythonOps.AttributeErrorForObjectMissingAttribute((object) target, this._name);
      return ((CallSite<Func<CallSite, TSelfType, CodeContext, object>>) site).Update(site, target, context);
    }

    public object GetErrorLightThrow(CallSite site, TSelfType target, CodeContext context)
    {
      return (object) target != null && target.GetType() == this._type && (object) this._extMethods == (object) context.ModuleContext.ExtensionMethods ? LightExceptions.Throw(PythonOps.AttributeErrorForObjectMissingAttribute((object) target, this._name)) : ((CallSite<Func<CallSite, TSelfType, CodeContext, object>>) site).Update(site, target, context);
    }

    public object GetErrorNoThrow(CallSite site, TSelfType target, CodeContext context)
    {
      return (object) target != null && target.GetType() == this._type && (object) this._extMethods == (object) context.ModuleContext.ExtensionMethods ? (object) OperationFailed.Value : ((CallSite<Func<CallSite, TSelfType, CodeContext, object>>) site).Update(site, target, context);
    }

    public object GetAmbiguous(CallSite site, TSelfType target, CodeContext context)
    {
      if ((object) target != null && target.GetType() == this._type && (object) this._extMethods == (object) context.ModuleContext.ExtensionMethods)
        throw new AmbiguousMatchException(this._name);
      return ((CallSite<Func<CallSite, TSelfType, CodeContext, object>>) site).Update(site, target, context);
    }
  }

  private class BuiltinBase<TSelfType> : FastGetBase
  {
    public override bool IsValid(PythonType type) => true;
  }

  private class FastMethodGet<TSelfType> : PythonGetMemberBinder.BuiltinBase<TSelfType>
  {
    private readonly Type _type;
    private readonly BuiltinMethodDescriptor _method;

    public FastMethodGet(Type type, BuiltinMethodDescriptor method)
    {
      this._type = type;
      this._method = method;
    }

    public object GetMethod(CallSite site, TSelfType target, CodeContext context)
    {
      return (object) target != null && target.GetType() == this._type ? this._method.UncheckedGetAttribute((object) target) : ((CallSite<Func<CallSite, TSelfType, CodeContext, object>>) site).Update(site, target, context);
    }
  }

  private class FastSlotGet<TSelfType> : PythonGetMemberBinder.BuiltinBase<TSelfType>
  {
    private readonly Type _type;
    private readonly PythonTypeSlot _slot;
    private readonly PythonType _owner;

    public FastSlotGet(Type type, PythonTypeSlot slot, PythonType owner)
    {
      this._type = type;
      this._slot = slot;
      this._owner = owner;
    }

    public object GetRetSlot(CallSite site, TSelfType target, CodeContext context)
    {
      return (object) target != null && target.GetType() == this._type ? (object) this._slot : ((CallSite<Func<CallSite, TSelfType, CodeContext, object>>) site).Update(site, target, context);
    }

    public object GetBindSlot(CallSite site, TSelfType target, CodeContext context)
    {
      if ((object) target == null || !(target.GetType() == this._type))
        return ((CallSite<Func<CallSite, TSelfType, CodeContext, object>>) site).Update(site, target, context);
      object bindSlot;
      this._slot.TryGetValue(context, (object) target, this._owner, out bindSlot);
      return bindSlot;
    }
  }

  private class FastTypeGet<TSelfType> : PythonGetMemberBinder.BuiltinBase<TSelfType>
  {
    private readonly Type _type;
    private readonly object _pyType;

    public FastTypeGet(Type type, object pythonType)
    {
      this._type = type;
      this._pyType = pythonType;
    }

    public object GetTypeObject(CallSite site, TSelfType target, CodeContext context)
    {
      return (object) target != null && target.GetType() == this._type ? this._pyType : ((CallSite<Func<CallSite, TSelfType, CodeContext, object>>) site).Update(site, target, context);
    }
  }

  private class FastPropertyGet<TSelfType> : PythonGetMemberBinder.BuiltinBase<TSelfType>
  {
    private readonly Type _type;
    private readonly Func<object, object> _propGetter;

    public FastPropertyGet(Type type, Func<object, object> propGetter)
    {
      this._type = type;
      this._propGetter = propGetter;
    }

    public object GetProperty(CallSite site, TSelfType target, CodeContext context)
    {
      return (object) target != null && target.GetType() == this._type ? this._propGetter((object) target) : ((CallSite<Func<CallSite, TSelfType, CodeContext, object>>) site).Update(site, target, context);
    }

    public object GetPropertyBool(CallSite site, TSelfType target, CodeContext context)
    {
      return (object) target != null && target.GetType() == this._type ? ScriptingRuntimeHelpers.BooleanToObject((bool) this._propGetter((object) target)) : ((CallSite<Func<CallSite, TSelfType, CodeContext, object>>) site).Update(site, target, context);
    }

    public object GetPropertyInt(CallSite site, TSelfType target, CodeContext context)
    {
      return (object) target != null && target.GetType() == this._type ? ScriptingRuntimeHelpers.Int32ToObject((int) this._propGetter((object) target)) : ((CallSite<Func<CallSite, TSelfType, CodeContext, object>>) site).Update(site, target, context);
    }
  }

  private class PythonModuleDelegate : FastGetBase
  {
    private readonly string _name;

    public PythonModuleDelegate(string name) => this._name = name;

    public object Target(CallSite site, object self, CodeContext context)
    {
      return self != null && self.GetType() == typeof (PythonModule) ? ((PythonModule) self).__getattribute__(context, this._name) : FastGetBase.Update(site, self, context);
    }

    public object NoThrowTarget(CallSite site, object self, CodeContext context)
    {
      return self != null && self.GetType() == typeof (PythonModule) ? ((PythonModule) self).GetAttributeNoThrow(context, this._name) : FastGetBase.Update(site, self, context);
    }

    public object LightThrowTarget(CallSite site, object self, CodeContext context)
    {
      if (self == null || !(self.GetType() == typeof (PythonModule)))
        return FastGetBase.Update(site, self, context);
      object attributeNoThrow = ((PythonModule) self).GetAttributeNoThrow(context, this._name);
      return attributeNoThrow == OperationFailed.Value ? LightExceptions.Throw(PythonOps.AttributeErrorForObjectMissingAttribute(self, this._name)) : attributeNoThrow;
    }

    public override bool IsValid(PythonType type) => true;
  }

  private class NamespaceTrackerDelegate : FastGetBase
  {
    private readonly string _name;

    public NamespaceTrackerDelegate(string name) => this._name = name;

    public object Target(CallSite site, object self, CodeContext context)
    {
      if (self == null || !(self.GetType() == typeof (NamespaceTracker)))
        return FastGetBase.Update(site, self, context);
      object obj = NamespaceTrackerOps.GetCustomMember(context, (NamespaceTracker) self, this._name);
      return obj != OperationFailed.Value ? obj : throw PythonOps.AttributeErrorForMissingAttribute(self, this._name);
    }

    public object NoThrowTarget(CallSite site, object self, CodeContext context)
    {
      return self != null && self.GetType() == typeof (NamespaceTracker) ? NamespaceTrackerOps.GetCustomMember(context, (NamespaceTracker) self, this._name) : FastGetBase.Update(site, self, context);
    }

    public object GetName(CallSite site, object self, CodeContext context)
    {
      return self != null && self.GetType() == typeof (NamespaceTracker) ? (object) NamespaceTrackerOps.Get__name__(context, (NamespaceTracker) self) : FastGetBase.Update(site, self, context);
    }

    public object GetFile(CallSite site, object self, CodeContext context)
    {
      return self != null && self.GetType() == typeof (NamespaceTracker) ? NamespaceTrackerOps.Get__file__((NamespaceTracker) self) : FastGetBase.Update(site, self, context);
    }

    public object GetDict(CallSite site, object self, CodeContext context)
    {
      return self != null && self.GetType() == typeof (NamespaceTracker) ? (object) NamespaceTrackerOps.Get__dict__(context, (NamespaceTracker) self) : FastGetBase.Update(site, self, context);
    }

    public override bool IsValid(PythonType type) => true;
  }

  private class LightThrowBinder(PythonContext context, string name, bool isNoThrow) : 
    PythonGetMemberBinder(context, name, isNoThrow)
  {
    public override bool SupportsLightThrow => true;

    public override CallSiteBinder GetLightExceptionBinder() => (CallSiteBinder) this;
  }
}
