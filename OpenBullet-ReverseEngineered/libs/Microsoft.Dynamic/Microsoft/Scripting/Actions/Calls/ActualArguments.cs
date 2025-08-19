// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.ActualArguments
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

public sealed class ActualArguments
{
  public ActualArguments(
    IList<DynamicMetaObject> args,
    IList<DynamicMetaObject> namedArgs,
    IList<string> argNames,
    int hiddenCount,
    int collapsedCount,
    int firstSplattedArg,
    int splatIndex)
  {
    ContractUtils.RequiresNotNullItems<DynamicMetaObject>(args, nameof (args));
    ContractUtils.RequiresNotNullItems<DynamicMetaObject>(namedArgs, nameof (namedArgs));
    ContractUtils.RequiresNotNullItems<string>(argNames, nameof (argNames));
    ContractUtils.Requires(namedArgs.Count == argNames.Count);
    ContractUtils.Requires(splatIndex == -1 || firstSplattedArg == -1 || firstSplattedArg >= 0 && firstSplattedArg <= splatIndex);
    ContractUtils.Requires(splatIndex == -1 || splatIndex >= 0);
    ContractUtils.Requires(collapsedCount >= 0);
    ContractUtils.Requires(hiddenCount >= 0);
    this.Arguments = args;
    this.NamedArguments = namedArgs;
    this.ArgNames = argNames;
    this.CollapsedCount = collapsedCount;
    this.SplatIndex = collapsedCount > 0 ? splatIndex : -1;
    this.FirstSplattedArg = firstSplattedArg;
    this.HiddenCount = hiddenCount;
  }

  public int CollapsedCount { get; }

  public int SplatIndex { get; }

  public int FirstSplattedArg { get; }

  public IList<string> ArgNames { get; }

  public IList<DynamicMetaObject> NamedArguments { get; }

  public IList<DynamicMetaObject> Arguments { get; }

  internal int ToSplattedItemIndex(int collapsedArgIndex)
  {
    return this.SplatIndex - this.FirstSplattedArg + collapsedArgIndex;
  }

  public int Count => this.Arguments.Count + this.NamedArguments.Count;

  public int HiddenCount { get; }

  public int VisibleCount => this.Count + this.CollapsedCount - this.HiddenCount;

  public DynamicMetaObject this[int index]
  {
    get
    {
      return index >= this.Arguments.Count ? this.NamedArguments[index - this.Arguments.Count] : this.Arguments[index];
    }
  }

  internal bool TryBindNamedArguments(
    MethodCandidate method,
    out ArgumentBinding binding,
    out CallFailure failure)
  {
    if (this.NamedArguments.Count == 0)
    {
      binding = new ArgumentBinding(this.Arguments.Count);
      failure = (CallFailure) null;
      return true;
    }
    int[] binding1 = new int[this.NamedArguments.Count];
    BitArray bitArray = new BitArray(this.NamedArguments.Count);
    for (int index = 0; index < binding1.Length; ++index)
      binding1[index] = -1;
    List<string> stringList1 = (List<string>) null;
    List<string> stringList2 = (List<string>) null;
    int count = this.Arguments.Count;
    for (int index1 = 0; index1 < this.ArgNames.Count; ++index1)
    {
      int num = method.IndexOfParameter(this.ArgNames[index1]);
      if (num >= 0)
      {
        int index2 = num - count;
        if (num < count || bitArray[index2])
        {
          if (stringList2 == null)
            stringList2 = new List<string>();
          stringList2.Add(this.ArgNames[index1]);
        }
        else
        {
          binding1[index1] = index2;
          bitArray[index2] = true;
        }
      }
      else
      {
        if (stringList1 == null)
          stringList1 = new List<string>();
        stringList1.Add(this.ArgNames[index1]);
      }
    }
    binding = new ArgumentBinding(count, binding1);
    if (stringList1 != null)
    {
      failure = new CallFailure(method, stringList1.ToArray(), true);
      return false;
    }
    if (stringList2 != null)
    {
      failure = new CallFailure(method, stringList2.ToArray(), false);
      return false;
    }
    failure = (CallFailure) null;
    return true;
  }
}
