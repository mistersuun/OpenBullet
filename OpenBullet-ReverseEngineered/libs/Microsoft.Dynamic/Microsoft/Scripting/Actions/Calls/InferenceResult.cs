// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.InferenceResult
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Dynamic;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

public class InferenceResult
{
  public InferenceResult(Type type, BindingRestrictions restrictions)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    ContractUtils.RequiresNotNull((object) restrictions, nameof (restrictions));
    this.Type = type;
    this.Restrictions = restrictions;
  }

  public Type Type { get; }

  public BindingRestrictions Restrictions { get; }
}
