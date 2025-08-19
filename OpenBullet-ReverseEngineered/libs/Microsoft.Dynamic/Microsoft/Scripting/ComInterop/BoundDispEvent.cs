// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.BoundDispEvent
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal sealed class BoundDispEvent : DynamicObject
{
  private object _rcw;
  private Guid _sourceIid;
  private int _dispid;

  internal BoundDispEvent(object rcw, Guid sourceIid, int dispid)
  {
    this._rcw = rcw;
    this._sourceIid = sourceIid;
    this._dispid = dispid;
  }

  public override bool TryBinaryOperation(
    BinaryOperationBinder binder,
    object handler,
    out object result)
  {
    if (binder.Operation == ExpressionType.AddAssign)
    {
      result = this.InPlaceAdd(handler);
      return true;
    }
    if (binder.Operation == ExpressionType.SubtractAssign)
    {
      result = this.InPlaceSubtract(handler);
      return true;
    }
    result = (object) null;
    return false;
  }

  private static void VerifyHandler(object handler)
  {
    if ((object) (handler as Delegate) != null && handler.GetType() != typeof (Delegate))
      return;
    switch (handler)
    {
      case IDynamicMetaObjectProvider _:
        break;
      case DispCallable _:
        break;
      default:
        throw Microsoft.Scripting.Error.UnsupportedHandlerType();
    }
  }

  private object InPlaceAdd(object handler)
  {
    ContractUtils.RequiresNotNull(handler, nameof (handler));
    BoundDispEvent.VerifyHandler(handler);
    ComEventSink.FromRuntimeCallableWrapper(this._rcw, this._sourceIid, true).AddHandler(this._dispid, handler);
    return (object) this;
  }

  private object InPlaceSubtract(object handler)
  {
    ContractUtils.RequiresNotNull(handler, nameof (handler));
    BoundDispEvent.VerifyHandler(handler);
    ComEventSink.FromRuntimeCallableWrapper(this._rcw, this._sourceIid, false)?.RemoveHandler(this._dispid, handler);
    return (object) this;
  }
}
