// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.CachedOptimizedCodeAttribute
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace Microsoft.Scripting.Runtime;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class CachedOptimizedCodeAttribute : Attribute
{
  public CachedOptimizedCodeAttribute() => this.Names = ArrayUtils.EmptyStrings;

  public CachedOptimizedCodeAttribute(string[] names)
  {
    ContractUtils.RequiresNotNull((object) names, nameof (names));
    this.Names = names;
  }

  public string[] Names { get; }
}
