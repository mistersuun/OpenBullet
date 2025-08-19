// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.PythonCompilerOptions
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using Microsoft.Scripting;
using System;

#nullable disable
namespace IronPython.Compiler;

[Serializable]
public sealed class PythonCompilerOptions : CompilerOptions
{
  private ModuleOptions _module;
  private bool _skipFirstLine;
  private bool _dontImplyIndent;
  private string _moduleName;
  private int[] _initialIndentation;
  private CompilationMode _compilationMode;

  public PythonCompilerOptions()
    : this(ModuleOptions.None)
  {
  }

  public PythonCompilerOptions(ModuleOptions features) => this._module = features;

  [Obsolete("Use the overload that takes ModuleOptions instead")]
  public PythonCompilerOptions(bool trueDivision) => this.TrueDivision = trueDivision;

  public bool DontImplyDedent
  {
    get => this._dontImplyIndent;
    set => this._dontImplyIndent = value;
  }

  public int[] InitialIndent
  {
    get => this._initialIndentation;
    set => this._initialIndentation = value;
  }

  public bool TrueDivision
  {
    get => (this._module & ModuleOptions.TrueDivision) != 0;
    set
    {
      if (value)
        this._module |= ModuleOptions.TrueDivision;
      else
        this._module &= ~ModuleOptions.TrueDivision;
    }
  }

  public bool AllowWithStatement
  {
    get => (this._module & ModuleOptions.WithStatement) != 0;
    set
    {
      if (value)
        this._module |= ModuleOptions.WithStatement;
      else
        this._module &= ~ModuleOptions.WithStatement;
    }
  }

  public bool AbsoluteImports
  {
    get => (this._module & ModuleOptions.AbsoluteImports) != 0;
    set
    {
      if (value)
        this._module |= ModuleOptions.AbsoluteImports;
      else
        this._module &= ~ModuleOptions.AbsoluteImports;
    }
  }

  public bool Verbatim
  {
    get => (this._module & ModuleOptions.Verbatim) != 0;
    set
    {
      if (value)
        this._module |= ModuleOptions.Verbatim;
      else
        this._module &= ~ModuleOptions.Verbatim;
    }
  }

  public bool PrintFunction
  {
    get => (this._module & ModuleOptions.PrintFunction) != 0;
    set
    {
      if (value)
        this._module |= ModuleOptions.PrintFunction;
      else
        this._module &= ~ModuleOptions.PrintFunction;
    }
  }

  public bool UnicodeLiterals
  {
    get => (this._module & ModuleOptions.UnicodeLiterals) != 0;
    set
    {
      if (value)
        this._module |= ModuleOptions.UnicodeLiterals;
      else
        this._module &= ~ModuleOptions.UnicodeLiterals;
    }
  }

  public bool Interpreted
  {
    get => (this._module & ModuleOptions.Interpret) != 0;
    set
    {
      if (value)
        this._module |= ModuleOptions.Interpret;
      else
        this._module &= ~ModuleOptions.Interpret;
    }
  }

  public bool Optimized
  {
    get => (this._module & ModuleOptions.Optimized) != 0;
    set
    {
      if (value)
        this._module |= ModuleOptions.Optimized;
      else
        this._module &= ~ModuleOptions.Optimized;
    }
  }

  public ModuleOptions Module
  {
    get => this._module;
    set => this._module = value;
  }

  public string ModuleName
  {
    get => this._moduleName;
    set => this._moduleName = value;
  }

  public bool SkipFirstLine
  {
    get => this._skipFirstLine;
    set => this._skipFirstLine = value;
  }

  internal CompilationMode CompilationMode
  {
    get => this._compilationMode;
    set => this._compilationMode = value;
  }
}
