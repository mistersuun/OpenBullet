// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.OperatorToken
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler;

public class OperatorToken : Token
{
  private readonly int _precedence;
  private readonly string _image;

  public OperatorToken(TokenKind kind, string image, int precedence)
    : base(kind)
  {
    this._image = image;
    this._precedence = precedence;
  }

  public int Precedence => this._precedence;

  public override object Value => (object) this._image;

  public override string Image => this._image;
}
