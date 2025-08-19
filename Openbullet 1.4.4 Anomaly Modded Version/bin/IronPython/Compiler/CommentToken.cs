// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.CommentToken
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler;

public sealed class CommentToken : Token
{
  private readonly string _comment;

  public CommentToken(string comment)
    : base(TokenKind.Comment)
  {
    this._comment = comment;
  }

  public string Comment => this._comment;

  public override string Image => this._comment;

  public override object Value => (object) this._comment;
}
