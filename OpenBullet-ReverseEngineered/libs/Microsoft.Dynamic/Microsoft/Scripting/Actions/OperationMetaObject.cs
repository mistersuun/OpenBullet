// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.OperationMetaObject
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class OperationMetaObject : DynamicMetaObject
{
  public OperationMetaObject(Expression expression, BindingRestrictions restrictions)
    : base(expression, restrictions)
  {
  }

  public OperationMetaObject(Expression expression, BindingRestrictions restrictions, object value)
    : base(expression, restrictions, value)
  {
  }

  [Obsolete("Use ExtensionBinaryOperationBinder or ExtensionUnaryOperationBinder")]
  public virtual DynamicMetaObject BindOperation(
    OperationBinder binder,
    params DynamicMetaObject[] args)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    return binder.FallbackOperation((DynamicMetaObject) this, args);
  }
}
