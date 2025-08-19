// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.ScopeExtension
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public class ScopeExtension
{
  public static readonly ScopeExtension[] EmptyArray = new ScopeExtension[0];

  public Scope Scope { get; }

  public ScopeExtension(Scope scope)
  {
    ContractUtils.RequiresNotNull((object) scope, nameof (scope));
    this.Scope = scope;
  }
}
