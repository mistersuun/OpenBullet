// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.InvariantContext
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Runtime;

internal sealed class InvariantContext : LanguageContext
{
  internal InvariantContext(ScriptDomainManager manager)
    : base(manager)
  {
  }

  public override bool CanCreateSourceCode => false;

  public override ScriptCode CompileSourceCode(
    SourceUnit sourceUnit,
    CompilerOptions options,
    ErrorSink errorSink)
  {
    throw new NotSupportedException();
  }

  public override T ScopeGetVariable<T>(Scope scope, string name)
  {
    object obj;
    if (scope.Storage is ScopeStorage storage1 && storage1.TryGetValue(name, false, out obj))
      return this.Operations.ConvertTo<T>(obj);
    return scope.Storage is StringDictionaryExpando storage2 && storage2.Dictionary.TryGetValue(name, out obj) ? this.Operations.ConvertTo<T>(obj) : base.ScopeGetVariable<T>(scope, name);
  }

  public override object ScopeGetVariable(Scope scope, string name)
  {
    object obj;
    return scope.Storage is ScopeStorage storage1 && storage1.TryGetValue(name, false, out obj) || scope.Storage is StringDictionaryExpando storage2 && storage2.Dictionary.TryGetValue(name, out obj) ? obj : base.ScopeGetVariable(scope, name);
  }

  public override void ScopeSetVariable(Scope scope, string name, object value)
  {
    if (scope.Storage is ScopeStorage storage2)
      storage2.SetValue(name, false, value);
    else if (scope.Storage is StringDictionaryExpando storage1)
      storage1.Dictionary[name] = value;
    else
      base.ScopeSetVariable(scope, name, value);
  }

  public override bool ScopeTryGetVariable(Scope scope, string name, out object value)
  {
    return scope.Storage is ScopeStorage storage1 && storage1.TryGetValue(name, false, out value) || scope.Storage is StringDictionaryExpando storage2 && storage2.Dictionary.TryGetValue(name, out value) || base.ScopeTryGetVariable(scope, name, out value);
  }
}
