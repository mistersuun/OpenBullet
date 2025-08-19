// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.PythonVariable
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler.Ast;

internal class PythonVariable
{
  private readonly string _name;
  private readonly ScopeStatement _scope;
  private VariableKind _kind;
  private bool _deleted;
  private bool _readBeforeInitialized;
  private bool _accessedInNestedScope;
  private int _index;

  public PythonVariable(string name, VariableKind kind, ScopeStatement scope)
  {
    this._name = name;
    this._kind = kind;
    this._scope = scope;
  }

  public string Name => this._name;

  public bool IsGlobal => this.Kind == VariableKind.Global || this.Scope.IsGlobal;

  public ScopeStatement Scope => this._scope;

  public VariableKind Kind
  {
    get => this._kind;
    set => this._kind = value;
  }

  internal bool Deleted
  {
    get => this._deleted;
    set => this._deleted = value;
  }

  internal int Index
  {
    get => this._index;
    set => this._index = value;
  }

  public bool ReadBeforeInitialized
  {
    get => this._readBeforeInitialized;
    set => this._readBeforeInitialized = value;
  }

  public bool AccessedInNestedScope
  {
    get => this._accessedInNestedScope;
    set => this._accessedInNestedScope = value;
  }
}
