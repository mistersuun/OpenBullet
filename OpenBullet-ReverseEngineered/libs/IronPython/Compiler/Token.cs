// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Token
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;

#nullable disable
namespace IronPython.Compiler;

public abstract class Token
{
  private readonly TokenKind _kind;

  protected Token(TokenKind kind) => this._kind = kind;

  public TokenKind Kind => this._kind;

  public virtual object Value => throw new NotSupportedException(Resources.TokenHasNoValue);

  public override string ToString() => $"{base.ToString()}({(object) this._kind})";

  public abstract string Image { get; }
}
