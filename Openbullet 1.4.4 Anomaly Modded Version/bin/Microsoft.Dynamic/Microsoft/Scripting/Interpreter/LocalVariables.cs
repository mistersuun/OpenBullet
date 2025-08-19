// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.LocalVariables
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

public sealed class LocalVariables
{
  private readonly HybridReferenceDictionary<ParameterExpression, LocalVariables.VariableScope> _variables = new HybridReferenceDictionary<ParameterExpression, LocalVariables.VariableScope>();
  private int _localCount;
  private int _maxLocalCount;

  internal LocalVariables()
  {
  }

  public LocalDefinition DefineLocal(ParameterExpression variable, int start)
  {
    ContractUtils.RequiresNotNull((object) variable, nameof (variable));
    ContractUtils.Requires(start >= 0, nameof (start), "start must be positive");
    LocalVariable variable1 = new LocalVariable(this._localCount++, false, false);
    this._maxLocalCount = Math.Max(this._localCount, this._maxLocalCount);
    LocalVariables.VariableScope parent;
    LocalVariables.VariableScope variableScope;
    if (this._variables.TryGetValue(variable, out parent))
    {
      variableScope = new LocalVariables.VariableScope(variable1, start, parent);
      if (parent.ChildScopes == null)
        parent.ChildScopes = new List<LocalVariables.VariableScope>();
      parent.ChildScopes.Add(variableScope);
    }
    else
      variableScope = new LocalVariables.VariableScope(variable1, start, (LocalVariables.VariableScope) null);
    this._variables[variable] = variableScope;
    return new LocalDefinition(variable1.Index, variable);
  }

  public void UndefineLocal(LocalDefinition definition, int end)
  {
    LocalVariables.VariableScope variable = this._variables[definition.Parameter];
    variable.Stop = end;
    if (variable.Parent != null)
      this._variables[definition.Parameter] = variable.Parent;
    else
      this._variables.Remove(definition.Parameter);
    --this._localCount;
  }

  internal void Box(ParameterExpression variable, InstructionList instructions)
  {
    LocalVariables.VariableScope variable1 = this._variables[variable];
    LocalVariable variable2 = variable1.Variable;
    this._variables[variable].Variable.IsBoxed = true;
    int index = 0;
    for (int instructionIndex = variable1.Start; instructionIndex < variable1.Stop && instructionIndex < instructions.Count; ++instructionIndex)
    {
      if (variable1.ChildScopes != null && variable1.ChildScopes[index].Start == instructionIndex)
      {
        instructionIndex = variable1.ChildScopes[index].Stop;
        ++index;
      }
      else
        instructions.SwitchToBoxed(variable2.Index, instructionIndex);
    }
  }

  public int LocalCount => this._maxLocalCount;

  public int GetOrDefineLocal(ParameterExpression var)
  {
    int localIndex = this.GetLocalIndex(var);
    return localIndex == -1 ? this.DefineLocal(var, 0).Index : localIndex;
  }

  public int GetLocalIndex(ParameterExpression var)
  {
    LocalVariables.VariableScope variableScope;
    return !this._variables.TryGetValue(var, out variableScope) ? -1 : variableScope.Variable.Index;
  }

  public bool TryGetLocalOrClosure(ParameterExpression var, out LocalVariable local)
  {
    LocalVariables.VariableScope variableScope;
    if (this._variables.TryGetValue(var, out variableScope))
    {
      local = variableScope.Variable;
      return true;
    }
    if (this.ClosureVariables != null && this.ClosureVariables.TryGetValue(var, out local))
      return true;
    local = (LocalVariable) null;
    return false;
  }

  internal Dictionary<ParameterExpression, LocalVariable> CopyLocals()
  {
    Dictionary<ParameterExpression, LocalVariable> dictionary = new Dictionary<ParameterExpression, LocalVariable>(this._variables.Count);
    foreach (KeyValuePair<ParameterExpression, LocalVariables.VariableScope> variable in this._variables)
      dictionary[variable.Key] = variable.Value.Variable;
    return dictionary;
  }

  internal bool ContainsVariable(ParameterExpression variable)
  {
    return this._variables.ContainsKey(variable);
  }

  internal Dictionary<ParameterExpression, LocalVariable> ClosureVariables { get; private set; }

  internal LocalVariable AddClosureVariable(ParameterExpression variable)
  {
    if (this.ClosureVariables == null)
      this.ClosureVariables = new Dictionary<ParameterExpression, LocalVariable>();
    LocalVariable localVariable = new LocalVariable(this.ClosureVariables.Count, true, false);
    this.ClosureVariables.Add(variable, localVariable);
    return localVariable;
  }

  private sealed class VariableScope
  {
    public readonly int Start;
    public int Stop = int.MaxValue;
    public readonly LocalVariable Variable;
    public readonly LocalVariables.VariableScope Parent;
    public List<LocalVariables.VariableScope> ChildScopes;

    public VariableScope(LocalVariable variable, int start, LocalVariables.VariableScope parent)
    {
      this.Variable = variable;
      this.Start = start;
      this.Parent = parent;
    }
  }
}
