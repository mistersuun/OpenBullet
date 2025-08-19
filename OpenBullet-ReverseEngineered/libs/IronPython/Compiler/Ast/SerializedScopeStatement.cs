// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.SerializedScopeStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using Microsoft.Scripting.Ast;
using System;

#nullable disable
namespace IronPython.Compiler.Ast;

internal class SerializedScopeStatement : ScopeStatement
{
  private readonly string _name;
  private readonly string _filename;
  private readonly FunctionAttributes _flags;
  private readonly string[] _parameterNames;

  internal SerializedScopeStatement(
    string name,
    string[] argNames,
    FunctionAttributes flags,
    int startIndex,
    int endIndex,
    string path,
    string[] freeVars,
    string[] names,
    string[] cellVars,
    string[] varNames)
  {
    this._name = name;
    this._filename = path;
    this._flags = flags;
    this.SetLoc((PythonAst) null, startIndex, endIndex);
    this._parameterNames = argNames;
    if (freeVars != null)
    {
      foreach (string freeVar in freeVars)
        this.AddFreeVariable(new PythonVariable(freeVar, VariableKind.Local, (ScopeStatement) this), false);
    }
    if (names != null)
    {
      foreach (string name1 in names)
        this.AddGlobalVariable(new PythonVariable(name1, VariableKind.Global, (ScopeStatement) this));
    }
    if (varNames != null)
    {
      foreach (string varName in varNames)
        this.EnsureVariable(varName);
    }
    if (cellVars == null)
      return;
    foreach (string cellVar in cellVars)
      this.AddCellVariable(new PythonVariable(cellVar, VariableKind.Local, (ScopeStatement) this));
  }

  internal override LightLambdaExpression GetLambda() => throw new InvalidOperationException();

  internal override bool ExposesLocalVariable(PythonVariable variable)
  {
    throw new InvalidOperationException();
  }

  internal override PythonVariable BindReference(PythonNameBinder binder, PythonReference reference)
  {
    throw new InvalidOperationException();
  }

  public override void Walk(PythonWalker walker) => throw new InvalidOperationException();

  public override string Name => this._name;

  internal override string Filename => this._filename;

  internal override FunctionAttributes Flags => this._flags;

  internal override string[] ParameterNames => this._parameterNames;

  internal override int ArgCount => this._parameterNames.Length;
}
