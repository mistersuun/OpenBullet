// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.RelativeModuleName
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler.Ast;

public class RelativeModuleName : ModuleName
{
  private readonly int _dotCount;

  public RelativeModuleName(string[] names, int dotCount)
    : base(names)
  {
    this._dotCount = dotCount;
  }

  public int DotCount => this._dotCount;
}
