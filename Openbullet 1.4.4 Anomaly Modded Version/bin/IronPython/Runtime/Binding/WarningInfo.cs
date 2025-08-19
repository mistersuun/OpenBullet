// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.WarningInfo
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Utils;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class WarningInfo
{
  private readonly string _message;
  private readonly PythonType _type;
  private readonly Expression _condition;

  public WarningInfo(PythonType type, string message)
  {
    this._message = message;
    this._type = type;
  }

  public WarningInfo(PythonType type, string message, Expression condition)
  {
    this._message = message;
    this._type = type;
    this._condition = condition;
  }

  public DynamicMetaObject AddWarning(Expression codeContext, DynamicMetaObject result)
  {
    Expression ifTrue = (Expression) Expression.Call(typeof (PythonOps).GetMethod("Warn"), codeContext, Microsoft.Scripting.Ast.Utils.Constant((object) this._type), Microsoft.Scripting.Ast.Utils.Constant((object) this._message), Microsoft.Scripting.Ast.Utils.Constant((object) ArrayUtils.EmptyObjects));
    if (this._condition != null)
      ifTrue = (Expression) Expression.Condition(this._condition, ifTrue, (Expression) Microsoft.Scripting.Ast.Utils.Empty());
    return new DynamicMetaObject((Expression) Expression.Block(ifTrue, result.Expression), result.Restrictions);
  }
}
