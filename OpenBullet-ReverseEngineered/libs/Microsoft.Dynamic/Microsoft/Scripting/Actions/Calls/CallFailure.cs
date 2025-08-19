// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.CallFailure
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

public sealed class CallFailure
{
  private readonly ConversionResult[] _results;
  private readonly string[] _keywordArgs;

  internal CallFailure(MethodCandidate candidate, ConversionResult[] results)
  {
    this.Candidate = candidate;
    this._results = results;
    this.Reason = CallFailureReason.ConversionFailure;
  }

  internal CallFailure(MethodCandidate candidate, string[] keywordArgs, bool unassignable)
  {
    this.Reason = unassignable ? CallFailureReason.UnassignableKeyword : CallFailureReason.DuplicateKeyword;
    this.Candidate = candidate;
    this._keywordArgs = keywordArgs;
  }

  internal CallFailure(MethodCandidate candidate, CallFailureReason reason)
  {
    this.Candidate = candidate;
    this.Reason = reason;
  }

  public MethodCandidate Candidate { get; }

  public CallFailureReason Reason { get; }

  public IList<ConversionResult> ConversionResults => (IList<ConversionResult>) this._results;

  public IList<string> KeywordArguments => (IList<string>) this._keywordArgs;
}
