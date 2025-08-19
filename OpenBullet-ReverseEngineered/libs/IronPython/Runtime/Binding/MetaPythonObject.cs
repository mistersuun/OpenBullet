// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.MetaPythonObject
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Runtime;
using System;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class MetaPythonObject : DynamicMetaObject
{
  public MetaPythonObject(Expression expression, BindingRestrictions restrictions)
    : base(expression, restrictions)
  {
  }

  public MetaPythonObject(Expression expression, BindingRestrictions restrictions, object value)
    : base(expression, restrictions, value)
  {
  }

  public DynamicMetaObject FallbackConvert(DynamicMetaObjectBinder binder)
  {
    return binder is PythonConversionBinder conversionBinder ? conversionBinder.FallbackConvert(binder.ReturnType, (DynamicMetaObject) this, (DynamicMetaObject) null) : ((ConvertBinder) binder).FallbackConvert((DynamicMetaObject) this);
  }

  internal static MethodCallExpression MakeTryGetTypeMember(
    PythonContext PythonContext,
    PythonTypeSlot dts,
    Expression self,
    ParameterExpression tmp)
  {
    return MetaPythonObject.MakeTryGetTypeMember(PythonContext, dts, tmp, self, (Expression) Expression.Property((Expression) Expression.Convert(self, typeof (IPythonObject)), PythonTypeInfo._IPythonObject.PythonType));
  }

  internal static MethodCallExpression MakeTryGetTypeMember(
    PythonContext PythonContext,
    PythonTypeSlot dts,
    ParameterExpression tmp,
    Expression instance,
    Expression pythonType)
  {
    return Expression.Call(PythonTypeInfo._PythonOps.SlotTryGetBoundValue, Utils.Constant((object) PythonContext.SharedContext), Utils.Convert((Expression) Utils.WeakConstant((object) dts), typeof (PythonTypeSlot)), Utils.Convert(instance, typeof (object)), Utils.Convert(pythonType, typeof (PythonType)), (Expression) tmp);
  }

  public DynamicMetaObject Restrict(Type type) => MetaObjectExtensions.Restrict(this, type);

  public PythonType PythonType => DynamicHelpers.GetPythonType(this.Value);

  public static PythonType GetPythonType(DynamicMetaObject value)
  {
    return value.HasValue ? DynamicHelpers.GetPythonType(value.Value) : DynamicHelpers.GetPythonTypeFromType(value.GetLimitType());
  }

  protected static DynamicMetaObject MakeDelegateTarget(
    DynamicMetaObjectBinder action,
    Type toType,
    DynamicMetaObject arg)
  {
    PythonContext pythonContext = PythonContext.GetPythonContext(action);
    CodeContext codeContext = pythonContext == null ? DefaultContext.Default : pythonContext.SharedContext;
    return new DynamicMetaObject((Expression) Expression.Convert((Expression) Expression.Call(typeof (PythonOps).GetMethod("GetDelegate"), Utils.Constant((object) codeContext), arg.Expression, Utils.Constant((object) toType)), toType), arg.Restrictions);
  }

  protected static DynamicMetaObject GetMemberFallback(
    DynamicMetaObject self,
    DynamicMetaObjectBinder member,
    DynamicMetaObject codeContext)
  {
    return member is PythonGetMemberBinder pythonGetMemberBinder ? pythonGetMemberBinder.Fallback(self, codeContext) : ((GetMemberBinder) member).FallbackGetMember(self.Restrict(self.GetLimitType()));
  }

  protected static string GetGetMemberName(DynamicMetaObjectBinder member)
  {
    switch (member)
    {
      case PythonGetMemberBinder pythonGetMemberBinder:
        return pythonGetMemberBinder.Name;
      case InvokeMemberBinder invokeMemberBinder:
        return invokeMemberBinder.Name;
      default:
        return ((GetMemberBinder) member).Name;
    }
  }
}
