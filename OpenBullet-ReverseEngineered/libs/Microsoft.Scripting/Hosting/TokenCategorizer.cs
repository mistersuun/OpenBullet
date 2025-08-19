// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.TokenCategorizer
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Microsoft.Scripting.Hosting;

public sealed class TokenCategorizer : MarshalByRefObject
{
  private readonly TokenizerService _tokenizer;

  internal TokenCategorizer(TokenizerService tokenizer) => this._tokenizer = tokenizer;

  public void Initialize(object state, ScriptSource scriptSource, SourceLocation initialLocation)
  {
    this._tokenizer.Initialize(state, (TextReader) scriptSource.SourceUnit.GetReader(), scriptSource.SourceUnit, initialLocation);
  }

  public object CurrentState => this._tokenizer.CurrentState;

  public SourceLocation CurrentPosition => this._tokenizer.CurrentPosition;

  public TokenInfo ReadToken() => this._tokenizer.ReadToken();

  public bool IsRestartable => this._tokenizer.IsRestartable;

  public ErrorSink ErrorSink
  {
    get => this._tokenizer.ErrorSink;
    set => this._tokenizer.ErrorSink = value;
  }

  public bool SkipToken() => this._tokenizer.SkipToken();

  public IEnumerable<TokenInfo> ReadTokens(int characterCount)
  {
    return this._tokenizer.ReadTokens(characterCount);
  }

  public bool SkipTokens(int characterCount) => this._tokenizer.SkipTokens(characterCount);

  public override object InitializeLifetimeService() => (object) null;
}
