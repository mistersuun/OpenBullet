// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ComInvokeAction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal sealed class ComInvokeAction : InvokeBinder
{
  internal ComInvokeAction(CallInfo callInfo)
    : base(callInfo)
  {
  }

  public override int GetHashCode() => base.GetHashCode();

  public override bool Equals(object obj) => base.Equals((object) (obj as ComInvokeAction));

  public override DynamicMetaObject FallbackInvoke(
    DynamicMetaObject target,
    DynamicMetaObject[] args,
    DynamicMetaObject errorSuggestion)
  {
    DynamicMetaObject result;
    if (ComBinder.TryBindInvoke((InvokeBinder) this, target, args, out result))
      return result;
    DynamicMetaObject dynamicMetaObject = errorSuggestion;
    if (dynamicMetaObject != null)
      return dynamicMetaObject;
    return new DynamicMetaObject((Expression) Expression.Throw((Expression) Expression.New(typeof (NotSupportedException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), (Expression) Expression.Constant((object) Microsoft.Scripting.Strings.CannotCall))), target.Restrictions.Merge(BindingRestrictions.Combine((IList<DynamicMetaObject>) args)));
  }
}
