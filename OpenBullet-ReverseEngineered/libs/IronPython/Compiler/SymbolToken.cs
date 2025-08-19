// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.SymbolToken
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler;

public class SymbolToken : Token
{
  private readonly string _image;

  public SymbolToken(TokenKind kind, string image)
    : base(kind)
  {
    this._image = image;
  }

  public string Symbol => this._image;

  public override object Value => (object) this._image;

  public override string Image => this._image;
}
