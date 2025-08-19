// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.CollectableCompilationMode
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using Microsoft.Scripting.Ast;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler.Ast;

internal class CollectableCompilationMode : CompilationMode
{
  public override LightLambdaExpression ReduceAst(PythonAst instance, string name)
  {
    return (LightLambdaExpression) Utils.LightLambda<Func<FunctionCode, object>>(typeof (object), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[2]
    {
      PythonAst._globalArray,
      PythonAst._globalContext
    }, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) PythonAst._globalArray, instance.GlobalArrayInstance), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) PythonAst._globalContext, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) instance.ModuleContext.GlobalContext)), Utils.Convert(instance.ReduceWorker(), typeof (object))), name, (IList<ParameterExpression>) new ParameterExpression[1]
    {
      PythonAst._functionCode
    });
  }

  public override void PrepareScope(
    PythonAst ast,
    ReadOnlyCollectionBuilder<ParameterExpression> locals,
    List<System.Linq.Expressions.Expression> init)
  {
    locals.Add(PythonAst._globalArray);
    init.Add((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) PythonAst._globalArray, ast._arrayExpression));
  }

  public override System.Linq.Expressions.Expression GetGlobal(
    System.Linq.Expressions.Expression globalContext,
    int arrayIndex,
    PythonVariable variable,
    PythonGlobal global)
  {
    return (System.Linq.Expressions.Expression) new PythonGlobalVariableExpression((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.ArrayIndex((System.Linq.Expressions.Expression) PythonAst._globalArray, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) arrayIndex)), variable, global);
  }

  public override Type DelegateType => typeof (Expression<Func<FunctionCode, object>>);
}
