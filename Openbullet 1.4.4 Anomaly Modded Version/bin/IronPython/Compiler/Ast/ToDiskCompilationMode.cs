// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.ToDiskCompilationMode
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Ast;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler.Ast;

internal class ToDiskCompilationMode : CollectableCompilationMode
{
  public override System.Linq.Expressions.Expression GetConstant(object value)
  {
    return Utils.Constant(value);
  }

  public override void PrepareScope(
    PythonAst ast,
    ReadOnlyCollectionBuilder<ParameterExpression> locals,
    List<System.Linq.Expressions.Expression> init)
  {
    locals.Add(PythonAst._globalArray);
    init.Add((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) PythonAst._globalArray, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (PythonOps).GetMethod("GetGlobalArrayFromContext"), (System.Linq.Expressions.Expression) PythonAst._globalContext)));
  }

  public override LightLambdaExpression ReduceAst(PythonAst instance, string name)
  {
    return (LightLambdaExpression) Utils.LightLambda<LookupCompilationDelegate>(typeof (object), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      PythonAst._globalArray
    }, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) PythonAst._globalArray, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call((System.Linq.Expressions.Expression) null, typeof (PythonOps).GetMethod("GetGlobalArrayFromContext"), (System.Linq.Expressions.Expression) PythonAst._globalContext)), Utils.Convert(instance.ReduceWorker(), typeof (object))), name, (IList<ParameterExpression>) PythonAst._arrayFuncParams);
  }

  public override ScriptCode MakeScriptCode(PythonAst ast)
  {
    PythonCompilerOptions options = ast.CompilerContext.Options as PythonCompilerOptions;
    return (ScriptCode) new PythonSavableScriptCode((Expression<LookupCompilationDelegate>) ast.Reduce().Reduce(), ast.SourceUnit, ast.GetNames(), options.ModuleName);
  }
}
