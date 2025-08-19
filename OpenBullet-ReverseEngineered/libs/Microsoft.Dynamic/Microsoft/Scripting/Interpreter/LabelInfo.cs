// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.LabelInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class LabelInfo
{
  private readonly LabelTarget _node;
  private BranchLabel _label;
  private object _definitions;
  private readonly List<LabelScopeInfo> _references = new List<LabelScopeInfo>();
  private bool _acrossBlockJump;

  internal LabelInfo(LabelTarget node) => this._node = node;

  internal BranchLabel GetLabel(LightCompiler compiler)
  {
    this.EnsureLabel(compiler);
    return this._label;
  }

  internal void Reference(LabelScopeInfo block)
  {
    this._references.Add(block);
    if (!this.HasDefinitions)
      return;
    this.ValidateJump(block);
  }

  internal void Define(LabelScopeInfo block)
  {
    for (LabelScopeInfo labelScopeInfo = block; labelScopeInfo != null; labelScopeInfo = labelScopeInfo.Parent)
    {
      if (labelScopeInfo.ContainsTarget(this._node))
        throw new InvalidOperationException("Label target already defined: " + this._node.Name);
    }
    this.AddDefinition(block);
    block.AddLabelInfo(this._node, this);
    if (this.HasDefinitions && !this.HasMultipleDefinitions)
    {
      foreach (LabelScopeInfo reference in this._references)
        this.ValidateJump(reference);
    }
    else
    {
      if (this._acrossBlockJump)
        throw new InvalidOperationException("Ambiguous jump");
      this._label = (BranchLabel) null;
    }
  }

  private void ValidateJump(LabelScopeInfo reference)
  {
    for (LabelScopeInfo scope = reference; scope != null; scope = scope.Parent)
    {
      if (this.DefinedIn(scope))
        return;
      if (scope.Kind == LabelScopeKind.Filter)
        break;
    }
    this._acrossBlockJump = true;
    if (this.HasMultipleDefinitions)
      throw new InvalidOperationException("Ambiguous jump " + this._node.Name);
    LabelScopeInfo first = this.FirstDefinition();
    LabelScopeInfo labelScopeInfo1 = LabelInfo.CommonNode<LabelScopeInfo>(first, reference, (Func<LabelScopeInfo, LabelScopeInfo>) (b => b.Parent));
    for (LabelScopeInfo labelScopeInfo2 = reference; labelScopeInfo2 != labelScopeInfo1; labelScopeInfo2 = labelScopeInfo2.Parent)
    {
      if (labelScopeInfo2.Kind == LabelScopeKind.Filter)
        throw new InvalidOperationException("Control cannot leave filter test");
    }
    for (LabelScopeInfo labelScopeInfo3 = first; labelScopeInfo3 != labelScopeInfo1; labelScopeInfo3 = labelScopeInfo3.Parent)
    {
      if (!labelScopeInfo3.CanJumpInto)
      {
        if (labelScopeInfo3.Kind == LabelScopeKind.Expression)
          throw new InvalidOperationException("Control cannot enter an expression");
        throw new InvalidOperationException("Control cannot enter try");
      }
    }
  }

  internal void ValidateFinish()
  {
    if (this._references.Count > 0 && !this.HasDefinitions)
      throw new InvalidOperationException("label target undefined");
  }

  private void EnsureLabel(LightCompiler compiler)
  {
    if (this._label != null)
      return;
    this._label = compiler.Instructions.MakeLabel();
  }

  private bool DefinedIn(LabelScopeInfo scope)
  {
    if (this._definitions == scope)
      return true;
    return this._definitions is HashSet<LabelScopeInfo> definitions && definitions.Contains(scope);
  }

  private bool HasDefinitions => this._definitions != null;

  private LabelScopeInfo FirstDefinition()
  {
    return this._definitions is LabelScopeInfo definitions ? definitions : ((IEnumerable<LabelScopeInfo>) this._definitions).First<LabelScopeInfo>();
  }

  private void AddDefinition(LabelScopeInfo scope)
  {
    if (this._definitions == null)
    {
      this._definitions = (object) scope;
    }
    else
    {
      if (!(this._definitions is HashSet<LabelScopeInfo> labelScopeInfoSet1))
      {
        HashSet<LabelScopeInfo> labelScopeInfoSet = new HashSet<LabelScopeInfo>();
        labelScopeInfoSet.Add((LabelScopeInfo) this._definitions);
        labelScopeInfoSet1 = labelScopeInfoSet;
        this._definitions = (object) labelScopeInfoSet;
      }
      labelScopeInfoSet1.Add(scope);
    }
  }

  private bool HasMultipleDefinitions => this._definitions is HashSet<LabelScopeInfo>;

  internal static T CommonNode<T>(T first, T second, Func<T, T> parent) where T : class
  {
    EqualityComparer<T> comparer = EqualityComparer<T>.Default;
    if (comparer.Equals(first, second))
      return first;
    HashSet<T> objSet = new HashSet<T>((IEqualityComparer<T>) comparer);
    for (T obj = first; (object) obj != null; obj = parent(obj))
      objSet.Add(obj);
    for (T obj = second; (object) obj != null; obj = parent(obj))
    {
      if (objSet.Contains(obj))
        return obj;
    }
    return default (T);
  }
}
