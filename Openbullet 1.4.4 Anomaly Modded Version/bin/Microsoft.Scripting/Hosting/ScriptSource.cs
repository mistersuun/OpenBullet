// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.ScriptSource
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Hosting;

[DebuggerDisplay("{Path ?? \"<anonymous>\"}")]
public sealed class ScriptSource : MarshalByRefObject
{
  private readonly ScriptEngine _engine;
  private readonly SourceUnit _unit;

  internal SourceUnit SourceUnit => this._unit;

  public string Path => this._unit.Path;

  public SourceCodeKind Kind => this._unit.Kind;

  public ScriptEngine Engine => this._engine;

  internal ScriptSource(ScriptEngine engine, SourceUnit sourceUnit)
  {
    this._unit = sourceUnit;
    this._engine = engine;
  }

  public CompiledCode Compile()
  {
    return this.CompileInternal((CompilerOptions) null, (ErrorListener) null);
  }

  public CompiledCode Compile(ErrorListener errorListener)
  {
    ContractUtils.RequiresNotNull((object) errorListener, nameof (errorListener));
    return this.CompileInternal((CompilerOptions) null, errorListener);
  }

  public CompiledCode Compile(CompilerOptions compilerOptions)
  {
    ContractUtils.RequiresNotNull((object) compilerOptions, nameof (compilerOptions));
    return this.CompileInternal(compilerOptions, (ErrorListener) null);
  }

  public CompiledCode Compile(CompilerOptions compilerOptions, ErrorListener errorListener)
  {
    ContractUtils.RequiresNotNull((object) errorListener, nameof (errorListener));
    ContractUtils.RequiresNotNull((object) compilerOptions, nameof (compilerOptions));
    return this.CompileInternal(compilerOptions, errorListener);
  }

  private CompiledCode CompileInternal(CompilerOptions compilerOptions, ErrorListener errorListener)
  {
    ErrorSink errorSink = (ErrorSink) new ErrorListenerProxySink(this, errorListener);
    ScriptCode code = compilerOptions != null ? this._unit.Compile(compilerOptions, errorSink) : this._unit.Compile(errorSink);
    return code == null ? (CompiledCode) null : new CompiledCode(this._engine, code);
  }

  public object Execute(ScriptScope scope)
  {
    ContractUtils.RequiresNotNull((object) scope, nameof (scope));
    return this._unit.Execute(scope.Scope);
  }

  public object Execute() => this._unit.Execute();

  public T Execute<T>(ScriptScope scope)
  {
    return this._engine.Operations.ConvertTo<T>(this.Execute(scope));
  }

  public T Execute<T>() => this._engine.Operations.ConvertTo<T>(this.Execute());

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

  public int ExecuteProgram() => this._unit.LanguageContext.ExecuteProgram(this._unit);

  public ScriptCodeParseResult GetCodeProperties() => this._unit.GetCodeProperties();

  public ScriptCodeParseResult GetCodeProperties(CompilerOptions options)
  {
    return this._unit.GetCodeProperties(options);
  }

  public SourceCodeReader GetReader() => this._unit.GetReader();

  public Encoding DetectEncoding()
  {
    using (SourceCodeReader reader = this._unit.GetReader())
      return reader.Encoding;
  }

  public string[] GetCodeLines(int start, int count) => this._unit.GetCodeLines(start, count);

  public string GetCodeLine(int line) => this._unit.GetCodeLine(line);

  public string GetCode() => this._unit.GetCode();

  public int MapLine(int line) => this._unit.MapLine(line);

  public SourceSpan MapLine(SourceSpan span)
  {
    return new SourceSpan(this._unit.MakeLocation(span.Start), this._unit.MakeLocation(span.End));
  }

  public SourceLocation MapLine(SourceLocation location) => this._unit.MakeLocation(location);

  public string MapLinetoFile(int line) => this._unit.Path;

  public override object InitializeLifetimeService() => (object) null;
}
