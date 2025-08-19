// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.SourceUnit
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting;

[DebuggerDisplay("{Path ?? \"<anonymous>\"}")]
public sealed class SourceUnit
{
  private readonly LanguageContext _language;
  private readonly TextContentProvider _contentProvider;
  private ScriptCodeParseResult? _parseResult;
  private KeyValuePair<int, int>[] _lineMap;

  public string Path { get; }

  public bool HasPath => this.Path != null;

  public SourceCodeKind Kind { get; }

  public SymbolDocumentInfo Document
  {
    get
    {
      return this.Path != null ? Expression.SymbolDocument(this.Path, this._language.LanguageGuid, this._language.VendorGuid) : (SymbolDocumentInfo) null;
    }
  }

  public LanguageContext LanguageContext => this._language;

  public ScriptCodeParseResult GetCodeProperties()
  {
    return this.GetCodeProperties(this._language.GetCompilerOptions());
  }

  public ScriptCodeParseResult GetCodeProperties(CompilerOptions options)
  {
    ContractUtils.RequiresNotNull((object) options, nameof (options));
    this._language.CompileSourceCode(this, options, ErrorSink.Null);
    return this._parseResult ?? ScriptCodeParseResult.Complete;
  }

  public ScriptCodeParseResult? CodeProperties
  {
    get => this._parseResult;
    set => this._parseResult = value;
  }

  public SourceUnit(
    LanguageContext context,
    TextContentProvider contentProvider,
    string path,
    SourceCodeKind kind)
  {
    this._language = context;
    this._contentProvider = contentProvider;
    this.Kind = kind;
    this.Path = path;
  }

  public SourceCodeReader GetReader() => this._contentProvider.GetReader();

  public string[] GetCodeLines(int start, int count)
  {
    ContractUtils.Requires(start > 0, nameof (start));
    ContractUtils.Requires(count > 0, nameof (count));
    List<string> stringList = new List<string>(count);
    using (SourceCodeReader reader = this.GetReader())
    {
      reader.SeekLine(start);
      for (; count > 0; --count)
      {
        string str = reader.ReadLine();
        if (str != null)
          stringList.Add(str);
        else
          break;
      }
    }
    return stringList.ToArray();
  }

  public string GetCodeLine(int line)
  {
    string[] codeLines = this.GetCodeLines(line, 1);
    return codeLines.Length == 0 ? (string) null : codeLines[0];
  }

  public string GetCode()
  {
    using (SourceCodeReader reader = this.GetReader())
      return reader.ReadToEnd();
  }

  public SourceLocation MakeLocation(int index, int line, int column)
  {
    return new SourceLocation(index, this.MapLine(line), column);
  }

  public SourceLocation MakeLocation(SourceLocation loc)
  {
    return new SourceLocation(loc.Index, this.MapLine(loc.Line), loc.Column);
  }

  public int MapLine(int line)
  {
    if (this._lineMap != null)
    {
      int index = SourceUnit.BinarySearch<int>(this._lineMap, line);
      int num = line - this._lineMap[index].Key;
      line = this._lineMap[index].Value + num;
      if (line < 1)
        line = 1;
    }
    return line;
  }

  public bool HasLineMapping => this._lineMap != null;

  private static int BinarySearch<T>(KeyValuePair<int, T>[] array, int line)
  {
    int num = Array.BinarySearch<KeyValuePair<int, T>>(array, new KeyValuePair<int, T>(line, default (T)), (IComparer<KeyValuePair<int, T>>) new SourceUnit.KeyComparer<T>());
    if (num < 0)
    {
      num = ~num - 1;
      if (num == -1)
        num = 0;
    }
    return num;
  }

  public bool EmitDebugSymbols
  {
    get => this.HasPath && this.LanguageContext.DomainManager.Configuration.DebugMode;
  }

  public ScriptCode Compile() => this.Compile(ErrorSink.Default);

  public ScriptCode Compile(ErrorSink errorSink)
  {
    return this.Compile(this._language.GetCompilerOptions(), errorSink);
  }

  public ScriptCode Compile(CompilerOptions options, ErrorSink errorSink)
  {
    ContractUtils.RequiresNotNull((object) errorSink, nameof (errorSink));
    ContractUtils.RequiresNotNull((object) options, nameof (options));
    return this._language.CompileSourceCode(this, options, errorSink);
  }

  public object Execute(Scope scope) => this.Execute(scope, ErrorSink.Default);

  public object Execute(Scope scope, ErrorSink errorSink)
  {
    ContractUtils.RequiresNotNull((object) scope, nameof (scope));
    return (this.Compile(this._language.GetCompilerOptions(scope), errorSink) ?? throw new SyntaxErrorException()).Run(scope);
  }

  public object Execute() => this.Compile().Run();

  public object Execute(ErrorSink errorSink) => this.Compile(errorSink).Run();

  public object Execute(CompilerOptions options, ErrorSink errorSink)
  {
    return this.Compile(options, errorSink).Run();
  }

  public int ExecuteProgram() => this._language.ExecuteProgram(this);

  public void SetLineMapping(KeyValuePair<int, int>[] lineMap)
  {
    this._lineMap = lineMap == null || lineMap.Length == 0 ? (KeyValuePair<int, int>[]) null : lineMap;
  }

  private class KeyComparer<T1> : IComparer<KeyValuePair<int, T1>>
  {
    public int Compare(KeyValuePair<int, T1> x, KeyValuePair<int, T1> y) => x.Key - y.Key;
  }
}
