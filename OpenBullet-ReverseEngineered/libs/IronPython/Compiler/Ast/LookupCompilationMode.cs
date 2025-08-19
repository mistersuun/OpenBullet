// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.LookupCompilationMode
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting;
using Microsoft.Scripting.Ast;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler.Ast;

internal class LookupCompilationMode : CompilationMode
{
  public override ScriptCode MakeScriptCode(PythonAst ast)
  {
    return (ScriptCode) new PythonScriptCode(ast);
  }

  public override LightLambdaExpression ReduceAst(PythonAst instance, string name)
  {
    return (LightLambdaExpression) Utils.LightLambda<LookupCompilationDelegate>(typeof (object), Utils.Convert(instance.ReduceWorker(), typeof (object)), name, (IList<ParameterExpression>) PythonAst._arrayFuncParams);
  }

  public override System.Linq.Expressions.Expression GetGlobal(
    System.Linq.Expressions.Expression globalContext,
    int arrayIndex,
    PythonVariable variable,
    PythonGlobal global)
  {
    return (System.Linq.Expressions.Expression) new LookupGlobalVariable(globalContext, variable.Name, variable.Kind == VariableKind.Local);
  }
}
