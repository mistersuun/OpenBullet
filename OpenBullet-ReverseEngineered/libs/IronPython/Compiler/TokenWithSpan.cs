// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.TokenWithSpan
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting;

#nullable disable
namespace IronPython.Compiler;

internal struct TokenWithSpan(Token token, IndexSpan span)
{
  private readonly Token _token = token;
  private readonly IndexSpan _span = span;

  public IndexSpan Span => this._span;

  public Token Token => this._token;
}
