// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.CompiledCode
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;
using System.Runtime.Remoting;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Hosting;

public sealed class CompiledCode : MarshalByRefObject
{
  private readonly ScriptEngine _engine;
  private readonly ScriptCode _code;
  private ScriptScope _defaultScope;

  internal ScriptCode ScriptCode => this._code;

  internal CompiledCode(ScriptEngine engine, ScriptCode code)
  {
    this._engine = engine;
    this._code = code;
  }

  public ScriptEngine Engine => this._engine;

  public ScriptScope DefaultScope
  {
    get
    {
      if (this._defaultScope == null)
        Interlocked.CompareExchange<ScriptScope>(ref this._defaultScope, new ScriptScope(this._engine, this._code.CreateScope()), (ScriptScope) null);
      return this._defaultScope;
    }
  }

  public object Execute() => this._code.Run(this.DefaultScope.Scope);

  public object Execute(ScriptScope scope)
  {
    ContractUtils.RequiresNotNull((object) scope, nameof (scope));
    return this._code.Run(scope.Scope);
  }

  public T Execute<T>() => this._engine.Operations.ConvertTo<T>(this.Execute());

  public T Execute<T>(ScriptScope scope)
  {
    return this._engine.Operations.ConvertTo<T>(this.Execute(scope));
  }

  public ObjectHandle ExecuteAndWrap() => new ObjectHandle(this.Execute());

  public ObjectHandle ExecuteAndWrap(ScriptScope scope) => new ObjectHandle(this.Execute(scope));

  public ObjectHandle ExecuteAndWrap(out ObjectHandle exception)
  {
    exception = (ObjectHandle) null;
    try
    {
      return new ObjectHandle(this.Execute());
    }
    catch (Exception ex)
    {
      exception = new ObjectHandle((object) ex);
      return (ObjectHandle) null;
    }
  }

  public ObjectHandle ExecuteAndWrap(ScriptScope scope, out ObjectHandle exception)
  {
    exception = (ObjectHandle) null;
    try
    {
      return new ObjectHandle(this.Execute(scope));
    }
    catch (Exception ex)
    {
      exception = new ObjectHandle((object) ex);
      return (ObjectHandle) null;
    }
  }

  public override object InitializeLifetimeService() => (object) null;
}
