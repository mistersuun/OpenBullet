// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.DispCallable
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal sealed class DispCallable : IPseudoComObject
{
  private readonly IDispatchComObject _dispatch;
  private readonly string _memberName;
  private readonly int _dispId;

  internal DispCallable(IDispatchComObject dispatch, string memberName, int dispId)
  {
    this._dispatch = dispatch;
    this._memberName = memberName;
    this._dispId = dispId;
  }

  public override string ToString()
  {
    return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "<bound dispmethod {0}>", (object) this._memberName);
  }

  public IDispatchComObject DispatchComObject => this._dispatch;

  public IDispatch DispatchObject => this._dispatch.DispatchObject;

  public string MemberName => this._memberName;

  public int DispId => this._dispId;

  public DynamicMetaObject GetMetaObject(Expression parameter)
  {
    return (DynamicMetaObject) new DispCallableMetaObject(parameter, this);
  }

  public override bool Equals(object obj)
  {
    return obj is DispCallable dispCallable && dispCallable._dispatch == this._dispatch && dispCallable._dispId == this._dispId;
  }

  public override int GetHashCode() => this._dispatch.GetHashCode() ^ this._dispId;
}
