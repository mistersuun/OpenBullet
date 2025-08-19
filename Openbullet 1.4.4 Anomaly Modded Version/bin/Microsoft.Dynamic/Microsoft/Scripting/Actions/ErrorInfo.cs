// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.ErrorInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Actions;

public sealed class ErrorInfo
{
  private ErrorInfo(Expression value, ErrorInfoKind kind)
  {
    this.Expression = value;
    this.Kind = kind;
  }

  public static ErrorInfo FromException(Expression exceptionValue)
  {
    ContractUtils.RequiresNotNull((object) exceptionValue, nameof (exceptionValue));
    ContractUtils.Requires(typeof (Exception).IsAssignableFrom(exceptionValue.Type), nameof (exceptionValue), Microsoft.Scripting.Strings.MustBeExceptionInstance);
    return new ErrorInfo(exceptionValue, ErrorInfoKind.Exception);
  }

  public static ErrorInfo FromValue(Expression resultValue)
  {
    ContractUtils.RequiresNotNull((object) resultValue, nameof (resultValue));
    return new ErrorInfo(resultValue, ErrorInfoKind.Error);
  }

  public static ErrorInfo FromValueNoError(Expression resultValue)
  {
    ContractUtils.RequiresNotNull((object) resultValue, nameof (resultValue));
    return new ErrorInfo(resultValue, ErrorInfoKind.Success);
  }

  public ErrorInfoKind Kind { get; }

  public Expression Expression { get; }
}
