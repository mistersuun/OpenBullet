// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ScriptCode
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;

#nullable disable
namespace Microsoft.Scripting;

public abstract class ScriptCode
{
  protected ScriptCode(SourceUnit sourceUnit)
  {
    ContractUtils.RequiresNotNull((object) sourceUnit, nameof (sourceUnit));
    this.SourceUnit = sourceUnit;
  }

  public LanguageContext LanguageContext => this.SourceUnit.LanguageContext;

  public SourceUnit SourceUnit { get; }

  public virtual Scope CreateScope() => this.SourceUnit.LanguageContext.CreateScope();

  public virtual object Run() => this.Run(this.CreateScope());

  public abstract object Run(Scope scope);

  public override string ToString()
  {
    return $"ScriptCode '{this.SourceUnit.Path}' from {this.LanguageContext.GetType().Name}";
  }
}
