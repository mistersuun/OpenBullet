// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.IncompleteStringErrorToken
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler;

public class IncompleteStringErrorToken : ErrorToken
{
  private readonly string _value;

  public IncompleteStringErrorToken(string message, string value)
    : base(message)
  {
    this._value = value;
  }

  public override string Image => this._value;

  public override object Value => (object) this._value;
}
