// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.BranchLabel
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

public sealed class BranchLabel
{
  internal const int UnknownIndex = -2147483648 /*0x80000000*/;
  internal const int UnknownDepth = -2147483648 /*0x80000000*/;
  internal int _labelIndex = int.MinValue;
  internal int _targetIndex = int.MinValue;
  internal int _stackDepth = int.MinValue;
  internal int _continuationStackDepth = int.MinValue;
  private List<int> _forwardBranchFixups;

  internal int LabelIndex
  {
    get => this._labelIndex;
    set => this._labelIndex = value;
  }

  internal bool HasRuntimeLabel => this._labelIndex != int.MinValue;

  internal int TargetIndex => this._targetIndex;

  internal RuntimeLabel ToRuntimeLabel()
  {
    return new RuntimeLabel(this._targetIndex, this._continuationStackDepth, this._stackDepth);
  }

  internal void Mark(InstructionList instructions)
  {
    ContractUtils.Requires(this._targetIndex == int.MinValue && this._stackDepth == int.MinValue && this._continuationStackDepth == int.MinValue);
    this._stackDepth = instructions.CurrentStackDepth;
    this._continuationStackDepth = instructions.CurrentContinuationsDepth;
    this._targetIndex = instructions.Count;
    if (this._forwardBranchFixups == null)
      return;
    foreach (int forwardBranchFixup in this._forwardBranchFixups)
      this.FixupBranch(instructions, forwardBranchFixup);
    this._forwardBranchFixups = (List<int>) null;
  }

  internal void AddBranch(InstructionList instructions, int branchIndex)
  {
    if (this._targetIndex == int.MinValue)
    {
      if (this._forwardBranchFixups == null)
        this._forwardBranchFixups = new List<int>();
      this._forwardBranchFixups.Add(branchIndex);
    }
    else
      this.FixupBranch(instructions, branchIndex);
  }

  internal void FixupBranch(InstructionList instructions, int branchIndex)
  {
    instructions.FixupBranch(branchIndex, this._targetIndex - branchIndex);
  }
}
