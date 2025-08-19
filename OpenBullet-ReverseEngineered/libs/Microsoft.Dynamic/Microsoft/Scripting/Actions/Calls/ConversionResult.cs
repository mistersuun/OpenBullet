// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.ConversionResult
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

public sealed class ConversionResult
{
  private readonly object _arg;

  internal ConversionResult(object arg, Type argType, Type toType, bool failed)
  {
    this._arg = arg;
    this.ArgType = argType;
    this.To = toType;
    this.Failed = failed;
  }

  public object Arg => this._arg;

  public Type ArgType { get; }

  public Type To { get; }

  public bool Failed { get; }

  internal static void ReplaceLastFailure(IList<ConversionResult> failures, bool isFailure)
  {
    ConversionResult failure = failures[failures.Count - 1];
    failures.RemoveAt(failures.Count - 1);
    failures.Add(new ConversionResult(failure.Arg, failure.ArgType, failure.To, isFailure));
  }

  public string GetArgumentTypeName(ActionBinder binder)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    return this._arg == null ? binder.GetTypeName(this.ArgType) : binder.GetObjectTypeName(this._arg);
  }
}
