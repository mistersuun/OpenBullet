// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.BuiltinMethodDescriptor
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Generation;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Types;

[PythonType("method_descriptor")]
[DontMapGetMemberNamesToDir]
public sealed class BuiltinMethodDescriptor : 
  PythonTypeSlot,
  IDynamicMetaObjectProvider,
  ICodeFormattable
{
  internal readonly BuiltinFunction _template;

  internal BuiltinMethodDescriptor(BuiltinFunction function) => this._template = function;

  internal object UncheckedGetAttribute(object instance)
  {
    return (object) this._template.BindToInstance(instance);
  }

  internal override bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    if (instance != null || owner == TypeCache.Null)
    {
      this.CheckSelf(context, instance);
      value = this.UncheckedGetAttribute(instance);
      return true;
    }
    value = (object) this;
    return true;
  }

  internal override void MakeGetExpression(
    PythonBinder binder,
    Expression codeContext,
    DynamicMetaObject instance,
    DynamicMetaObject owner,
    ConditionalBuilder builder)
  {
    if (instance != null)
      builder.FinishCondition((Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeBoundBuiltinFunction"), Utils.Constant((object) this._template), instance.Expression));
    else
      builder.FinishCondition(Utils.Constant((object) this));
  }

  internal override bool GetAlwaysSucceeds => true;

  internal BuiltinFunction Template => this._template;

  public Type DeclaringType
  {
    [PythonHidden(new PlatformID[] {})] get => this._template.DeclaringType;
  }

  internal static void CheckSelfWorker(CodeContext context, object self, BuiltinFunction template)
  {
    Type type = CompilerHelpers.GetType(self);
    if (!(type != template.DeclaringType) || template.DeclaringType.IsAssignableFrom(type))
      return;
    context.LanguageContext.Binder.Convert(self, template.DeclaringType);
  }

  internal override bool IsAlwaysVisible => this._template.IsAlwaysVisible;

  private void CheckSelf(CodeContext context, object self)
  {
    if ((this._template.FunctionType & FunctionType.FunctionMethodMask) != FunctionType.Method)
      return;
    BuiltinMethodDescriptor.CheckSelfWorker(context, self, this._template);
  }

  public string __name__ => this.Template.__name__;

  public string __doc__ => this.Template.__doc__;

  public object __call__(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], IDictionary<object, object>, object>>> storage,
    [ParamDictionary] IDictionary<object, object> dictArgs,
    params object[] args)
  {
    return this._template.__call__(context, storage, dictArgs, args);
  }

  public PythonType __objclass__
  {
    get => DynamicHelpers.GetPythonTypeFromType(this._template.DeclaringType);
  }

  public bool __eq__(object other)
  {
    return other is BuiltinMethodDescriptor methodDescriptor && PythonOps.Id((object) this.__objclass__) == PythonOps.Id((object) methodDescriptor.__objclass__) && this.__name__ == methodDescriptor.__name__;
  }

  public int __cmp__(object other)
  {
    if (!(other is BuiltinMethodDescriptor methodDescriptor))
      throw PythonOps.TypeError("instancemethod.__cmp__(x,y) requires y to be a 'instancemethod', not a {0}", (object) PythonTypeOps.GetName(other));
    long num = PythonOps.Id((object) this.__objclass__) - PythonOps.Id((object) methodDescriptor.__objclass__);
    if (num == 0L)
      return StringOps.Compare(this.__name__, methodDescriptor.__name__);
    return num <= 0L ? -1 : 1;
  }

  public string __repr__(CodeContext context)
  {
    return $"<method '{this.Template.Name}' of '{DynamicHelpers.GetPythonTypeFromType(this.DeclaringType).Name}' objects>";
  }

  DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
  {
    return (DynamicMetaObject) new MetaBuiltinMethodDescriptor(parameter, BindingRestrictions.Empty, this);
  }
}
