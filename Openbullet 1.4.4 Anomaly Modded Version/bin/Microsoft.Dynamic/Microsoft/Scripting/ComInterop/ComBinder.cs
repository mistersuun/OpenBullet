// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ComBinder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

public static class ComBinder
{
  public static bool IsComObject(object value) => ComObject.IsComObject(value);

  public static bool CanComBind(object value)
  {
    return ComBinder.IsComObject(value) || value is IPseudoComObject;
  }

  public static bool TryBindGetMember(
    GetMemberBinder binder,
    DynamicMetaObject instance,
    out DynamicMetaObject result,
    bool delayInvocation)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    ContractUtils.RequiresNotNull((object) instance, nameof (instance));
    if (ComBinder.TryGetMetaObject(ref instance))
    {
      ComBinder.ComGetMemberBinder binder1 = new ComBinder.ComGetMemberBinder(binder, delayInvocation);
      result = instance.BindGetMember((GetMemberBinder) binder1);
      if (result.Expression.Type.IsValueType)
        result = new DynamicMetaObject((Expression) Expression.Convert(result.Expression, typeof (object)), result.Restrictions);
      return true;
    }
    result = (DynamicMetaObject) null;
    return false;
  }

  public static bool TryBindGetMember(
    GetMemberBinder binder,
    DynamicMetaObject instance,
    out DynamicMetaObject result)
  {
    return ComBinder.TryBindGetMember(binder, instance, out result, false);
  }

  public static bool TryBindSetMember(
    SetMemberBinder binder,
    DynamicMetaObject instance,
    DynamicMetaObject value,
    out DynamicMetaObject result)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    ContractUtils.RequiresNotNull((object) instance, nameof (instance));
    ContractUtils.RequiresNotNull((object) value, nameof (value));
    if (ComBinder.TryGetMetaObject(ref instance))
    {
      result = instance.BindSetMember(binder, value);
      return true;
    }
    result = (DynamicMetaObject) null;
    return false;
  }

  public static bool TryBindInvoke(
    InvokeBinder binder,
    DynamicMetaObject instance,
    DynamicMetaObject[] args,
    out DynamicMetaObject result)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    ContractUtils.RequiresNotNull((object) instance, nameof (instance));
    ContractUtils.RequiresNotNull((object) args, nameof (args));
    if (ComBinder.TryGetMetaObjectInvoke(ref instance))
    {
      result = instance.BindInvoke(binder, args);
      return true;
    }
    result = (DynamicMetaObject) null;
    return false;
  }

  public static bool TryBindInvokeMember(
    InvokeMemberBinder binder,
    DynamicMetaObject instance,
    DynamicMetaObject[] args,
    out DynamicMetaObject result)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    ContractUtils.RequiresNotNull((object) instance, nameof (instance));
    ContractUtils.RequiresNotNull((object) args, nameof (args));
    if (ComBinder.TryGetMetaObject(ref instance))
    {
      result = instance.BindInvokeMember(binder, args);
      return true;
    }
    result = (DynamicMetaObject) null;
    return false;
  }

  public static bool TryBindGetIndex(
    GetIndexBinder binder,
    DynamicMetaObject instance,
    DynamicMetaObject[] args,
    out DynamicMetaObject result)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    ContractUtils.RequiresNotNull((object) instance, nameof (instance));
    ContractUtils.RequiresNotNull((object) args, nameof (args));
    if (ComBinder.TryGetMetaObjectInvoke(ref instance))
    {
      result = instance.BindGetIndex(binder, args);
      return true;
    }
    result = (DynamicMetaObject) null;
    return false;
  }

  public static bool TryBindSetIndex(
    SetIndexBinder binder,
    DynamicMetaObject instance,
    DynamicMetaObject[] args,
    DynamicMetaObject value,
    out DynamicMetaObject result)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    ContractUtils.RequiresNotNull((object) instance, nameof (instance));
    ContractUtils.RequiresNotNull((object) args, nameof (args));
    ContractUtils.RequiresNotNull((object) value, nameof (value));
    if (ComBinder.TryGetMetaObjectInvoke(ref instance))
    {
      result = instance.BindSetIndex(binder, args, value);
      return true;
    }
    result = (DynamicMetaObject) null;
    return false;
  }

  public static bool TryConvert(
    ConvertBinder binder,
    DynamicMetaObject instance,
    out DynamicMetaObject result)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    ContractUtils.RequiresNotNull((object) instance, nameof (instance));
    if (ComBinder.IsComObject(instance.Value) && binder.Type.IsInterface)
    {
      result = new DynamicMetaObject((Expression) Expression.Convert(instance.Expression, binder.Type), BindingRestrictions.GetExpressionRestriction((Expression) Expression.Call(typeof (ComObject).GetMethod("IsComObject", BindingFlags.Static | BindingFlags.NonPublic), Helpers.Convert(instance.Expression, typeof (object)))));
      return true;
    }
    result = (DynamicMetaObject) null;
    return false;
  }

  public static IEnumerable<string> GetDynamicMemberNames(object value)
  {
    ContractUtils.RequiresNotNull(value, nameof (value));
    ContractUtils.Requires(ComBinder.IsComObject(value), nameof (value), Microsoft.Scripting.Strings.ComObjectExpected);
    return (IEnumerable<string>) ComObject.ObjectToComObject(value).GetMemberNames(false);
  }

  internal static IList<string> GetDynamicDataMemberNames(object value)
  {
    ContractUtils.RequiresNotNull(value, nameof (value));
    ContractUtils.Requires(ComBinder.IsComObject(value), nameof (value), Microsoft.Scripting.Strings.ComObjectExpected);
    return ComObject.ObjectToComObject(value).GetMemberNames(true);
  }

  internal static IList<KeyValuePair<string, object>> GetDynamicDataMembers(
    object value,
    IEnumerable<string> names)
  {
    ContractUtils.RequiresNotNull(value, nameof (value));
    ContractUtils.Requires(ComBinder.IsComObject(value), nameof (value), Microsoft.Scripting.Strings.ComObjectExpected);
    return ComObject.ObjectToComObject(value).GetMembers(names);
  }

  private static bool TryGetMetaObject(ref DynamicMetaObject instance)
  {
    if (instance is ComUnwrappedMetaObject || !ComBinder.IsComObject(instance.Value))
      return false;
    instance = (DynamicMetaObject) new ComMetaObject(instance.Expression, instance.Restrictions, instance.Value);
    return true;
  }

  private static bool TryGetMetaObjectInvoke(ref DynamicMetaObject instance)
  {
    if (ComBinder.TryGetMetaObject(ref instance))
      return true;
    if (!(instance.Value is IPseudoComObject pseudoComObject))
      return false;
    instance = pseudoComObject.GetMetaObject(instance.Expression);
    return true;
  }

  internal class ComGetMemberBinder : GetMemberBinder
  {
    private readonly GetMemberBinder _originalBinder;
    internal bool _CanReturnCallables;

    internal ComGetMemberBinder(GetMemberBinder originalBinder, bool CanReturnCallables)
      : base(originalBinder.Name, originalBinder.IgnoreCase)
    {
      this._originalBinder = originalBinder;
      this._CanReturnCallables = CanReturnCallables;
    }

    public override DynamicMetaObject FallbackGetMember(
      DynamicMetaObject target,
      DynamicMetaObject errorSuggestion)
    {
      return this._originalBinder.FallbackGetMember(target, errorSuggestion);
    }

    public override int GetHashCode()
    {
      return this._originalBinder.GetHashCode() ^ (this._CanReturnCallables ? 1 : 0);
    }

    public override bool Equals(object obj)
    {
      return obj is ComBinder.ComGetMemberBinder comGetMemberBinder && this._CanReturnCallables == comGetMemberBinder._CanReturnCallables && this._originalBinder.Equals((object) comGetMemberBinder._originalBinder);
    }
  }
}
