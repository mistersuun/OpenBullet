// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.StringArgBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal class StringArgBuilder : SimpleArgBuilder
{
  private readonly bool _isWrapper;

  internal StringArgBuilder(Type parameterType)
    : base(parameterType)
  {
    this._isWrapper = parameterType == typeof (BStrWrapper);
  }

  internal override Expression Marshal(Expression parameter)
  {
    parameter = base.Marshal(parameter);
    if (this._isWrapper)
      parameter = (Expression) Expression.Property(Helpers.Convert(parameter, typeof (BStrWrapper)), typeof (BStrWrapper).GetProperty("WrappedObject"));
    return parameter;
  }

  internal override Expression MarshalToRef(Expression parameter)
  {
    parameter = this.Marshal(parameter);
    return (Expression) Expression.Call(typeof (System.Runtime.InteropServices.Marshal).GetMethod("StringToBSTR"), parameter);
  }

  internal override Expression UnmarshalFromRef(Expression value)
  {
    Expression newValue = (Expression) Expression.Condition((Expression) Expression.Equal(value, (Expression) Expression.Constant((object) IntPtr.Zero)), (Expression) Expression.Constant((object) null, typeof (string)), (Expression) Expression.Call(typeof (System.Runtime.InteropServices.Marshal).GetMethod("PtrToStringBSTR"), value));
    if (this._isWrapper)
      newValue = (Expression) Expression.New(typeof (BStrWrapper).GetConstructor(new Type[1]
      {
        typeof (string)
      }), newValue);
    return base.UnmarshalFromRef(newValue);
  }
}
