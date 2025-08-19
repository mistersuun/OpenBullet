// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.CandidateSet
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

internal sealed class CandidateSet
{
  private readonly int _arity;
  private readonly List<MethodCandidate> _candidates;

  internal CandidateSet(int count)
  {
    this._arity = count;
    this._candidates = new List<MethodCandidate>();
  }

  internal List<MethodCandidate> Candidates => this._candidates;

  internal int Arity => this._arity;

  internal bool IsParamsDictionaryOnly()
  {
    foreach (MethodCandidate candidate in this._candidates)
    {
      if (!candidate.HasParamsDictionary)
        return false;
    }
    return true;
  }

  internal void Add(MethodCandidate target) => this._candidates.Add(target);

  public override string ToString()
  {
    return $"{this._arity}: ({this._candidates[0].Overload.Name} on {this._candidates[0].Overload.DeclaringType.FullName})";
  }
}
