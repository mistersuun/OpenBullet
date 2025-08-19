// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.CompilerContext
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public sealed class CompilerContext
{
  public SourceUnit SourceUnit { get; }

  public ParserSink ParserSink { get; }

  public ErrorSink Errors { get; }

  public CompilerOptions Options { get; }

  public CompilerContext(SourceUnit sourceUnit, CompilerOptions options, ErrorSink errorSink)
    : this(sourceUnit, options, errorSink, ParserSink.Null)
  {
  }

  public CompilerContext(
    SourceUnit sourceUnit,
    CompilerOptions options,
    ErrorSink errorSink,
    ParserSink parserSink)
  {
    ContractUtils.RequiresNotNull((object) sourceUnit, nameof (sourceUnit));
    ContractUtils.RequiresNotNull((object) errorSink, nameof (errorSink));
    ContractUtils.RequiresNotNull((object) parserSink, nameof (parserSink));
    ContractUtils.RequiresNotNull((object) options, nameof (options));
    this.SourceUnit = sourceUnit;
    this.Options = options;
    this.Errors = errorSink;
    this.ParserSink = parserSink;
  }
}
