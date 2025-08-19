// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.DottedName
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Collections.Generic;
using System.Text;

#nullable disable
namespace IronPython.Compiler.Ast;

public class DottedName : Node
{
  private readonly string[] _names;

  public DottedName(string[] names) => this._names = names;

  public IList<string> Names => (IList<string>) this._names;

  public string MakeString()
  {
    if (this._names.Length == 0)
      return string.Empty;
    StringBuilder stringBuilder = new StringBuilder(this._names[0]);
    for (int index = 1; index < this._names.Length; ++index)
    {
      stringBuilder.Append('.');
      stringBuilder.Append(this._names[index]);
    }
    return stringBuilder.ToString();
  }

  public override void Walk(PythonWalker walker)
  {
    walker.Walk(this);
    walker.PostWalk(this);
  }
}
