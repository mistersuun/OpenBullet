// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.BindingTarget
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

public sealed class BindingTarget
{
  private readonly CallFailure[] _callFailures;
  private readonly MethodCandidate[] _ambiguousMatches;
  private readonly int[] _expectedArgs;

  internal BindingTarget(
    string name,
    int actualArgumentCount,
    MethodCandidate candidate,
    NarrowingLevel level,
    RestrictedArguments restrictedArgs)
  {
    this.Name = name;
    this.MethodCandidate = candidate;
    this.RestrictedArguments = restrictedArgs;
    this.NarrowingLevel = level;
    this.ActualArgumentCount = actualArgumentCount;
  }

  internal BindingTarget(string name, int actualArgumentCount, int[] expectedArgCount)
  {
    this.Name = name;
    this.Result = BindingResult.IncorrectArgumentCount;
    this._expectedArgs = expectedArgCount;
    this.ActualArgumentCount = actualArgumentCount;
  }

  internal BindingTarget(string name, int actualArgumentCount, CallFailure[] failures)
  {
    this.Name = name;
    this.Result = BindingResult.CallFailure;
    this._callFailures = failures;
    this.ActualArgumentCount = actualArgumentCount;
  }

  internal BindingTarget(string name, int actualArgumentCount, MethodCandidate[] ambiguousMatches)
  {
    this.Name = name;
    this.Result = BindingResult.AmbiguousMatch;
    this._ambiguousMatches = ambiguousMatches;
    this.ActualArgumentCount = actualArgumentCount;
  }

  internal BindingTarget(string name, BindingResult result)
  {
    this.Name = name;
    this.Result = result;
  }

  public BindingResult Result { get; }

  public Expression MakeExpression()
  {
    if (this.MethodCandidate == null)
      throw new InvalidOperationException("An expression cannot be produced because the method binding was unsuccessful.");
    return this.RestrictedArguments != null ? this.MethodCandidate.MakeExpression(this.RestrictedArguments) : throw new InvalidOperationException("An expression cannot be produced because the method binding was done with Expressions, not MetaObject's");
  }

  [Obsolete("Use Overload instead")]
  public MethodBase Method => this.MethodCandidate?.Overload.ReflectionInfo;

  public OverloadInfo Overload => this.MethodCandidate?.Overload;

  public string Name { get; }

  public MethodCandidate MethodCandidate { get; }

  public IEnumerable<MethodCandidate> AmbiguousMatches
  {
    get => (IEnumerable<MethodCandidate>) this._ambiguousMatches;
  }

  public ICollection<CallFailure> CallFailures => (ICollection<CallFailure>) this._callFailures;

  public IList<int> ExpectedArgumentCount => (IList<int>) this._expectedArgs;

  public int ActualArgumentCount { get; }

  public RestrictedArguments RestrictedArguments { get; }

  public Type ReturnType => this.MethodCandidate?.ReturnType;

  public NarrowingLevel NarrowingLevel { get; }

  public bool Success => this.Result == BindingResult.Success;
}
