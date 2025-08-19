// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.OperationBinder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;

#nullable disable
namespace Microsoft.Scripting.Actions;

[Obsolete("Use ExtensionBinaryOperationBinder or ExtensionUnaryOperationBinder")]
public abstract class OperationBinder : DynamicMetaObjectBinder
{
  protected OperationBinder(string operation) => this.Operation = operation;

  public string Operation { get; }

  public DynamicMetaObject FallbackOperation(DynamicMetaObject target, DynamicMetaObject[] args)
  {
    return this.FallbackOperation(target, args, (DynamicMetaObject) null);
  }

  public abstract DynamicMetaObject FallbackOperation(
    DynamicMetaObject target,
    DynamicMetaObject[] args,
    DynamicMetaObject errorSuggestion);

  public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
  {
    ContractUtils.RequiresNotNull((object) target, nameof (target));
    ContractUtils.RequiresNotNullItems<DynamicMetaObject>((IList<DynamicMetaObject>) args, nameof (args));
    return target is OperationMetaObject operationMetaObject ? operationMetaObject.BindOperation(this, args) : this.FallbackOperation(target, args);
  }

  public override bool Equals(object obj)
  {
    return obj is OperationBinder operationBinder && operationBinder.Operation == this.Operation;
  }

  public override int GetHashCode() => 268435456 /*0x10000000*/ ^ this.Operation.GetHashCode();
}
