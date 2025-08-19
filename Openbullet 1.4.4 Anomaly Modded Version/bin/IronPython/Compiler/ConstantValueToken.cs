// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.ConstantValueToken
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler;

public class ConstantValueToken : Token
{
  private readonly object _value;

  public ConstantValueToken(object value)
    : base(TokenKind.Constant)
  {
    this._value = value;
  }

  public object Constant => this._value;

  public override object Value => this._value;

  public override string Image => this._value != null ? this._value.ToString() : "None";
}
