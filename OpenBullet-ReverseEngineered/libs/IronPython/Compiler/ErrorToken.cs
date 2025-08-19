// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.ErrorToken
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler;

public class ErrorToken : Token
{
  private readonly string _message;

  public ErrorToken(string message)
    : base(TokenKind.Error)
  {
    this._message = message;
  }

  public string Message => this._message;

  public override string Image => this._message;

  public override object Value => (object) this._message;
}
