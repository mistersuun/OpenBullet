// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.PythonSavableScriptCode
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

#nullable disable
namespace IronPython.Compiler;

internal class PythonSavableScriptCode : SavableScriptCode, ICustomScriptCodeData
{
  private readonly Expression<LookupCompilationDelegate> _code;
  private readonly string[] _names;
  private readonly string _moduleName;

  public PythonSavableScriptCode(
    Expression<LookupCompilationDelegate> code,
    SourceUnit sourceUnit,
    string[] names,
    string moduleName)
    : base(sourceUnit)
  {
    this._code = code;
    this._names = names;
    this._moduleName = moduleName;
  }

  protected override KeyValuePair<MethodBuilder, Type> CompileForSave(TypeGen typeGen)
  {
    LambdaExpression lambdaExpression = this.RewriteForSave(typeGen, (LambdaExpression) this._code);
    MethodBuilder methodBuilder = typeGen.TypeBuilder.DefineMethod(lambdaExpression.Name ?? "lambda_method", CompilerHelpers.PublicStatic | MethodAttributes.SpecialName);
    lambdaExpression.CompileToMethod(methodBuilder);
    methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof (CachedOptimizedCodeAttribute).GetConstructor(new Type[1]
    {
      typeof (string[])
    }), new object[1]{ (object) this._names }));
    return new KeyValuePair<MethodBuilder, Type>(methodBuilder, typeof (LookupCompilationDelegate));
  }

  public override object Run() => throw new NotSupportedException();

  public override object Run(Scope scope) => throw new NotSupportedException();

  public override Scope CreateScope() => throw new NotSupportedException();

  string ICustomScriptCodeData.GetCustomScriptCodeData() => this._moduleName;
}
