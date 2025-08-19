// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.Services.StormWall.StormWallSolver
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System.Collections.Generic;
using System.Text;

#nullable disable
namespace Leaf.xNet.Services.StormWall;

internal class StormWallSolver
{
  private string _cE;
  private string _rpct;
  private int _cK;
  private int _rpctLastIndex;
  private readonly Dictionary<char, int> _rRpct = new Dictionary<char, int>();

  public void Init(string cE, int cK, string rpct)
  {
    this._cE = cE;
    this._cK = cK;
    this._rpct = rpct;
    this._rpctLastIndex = this._rpct.Length - 1;
    this._rRpct.Clear();
    for (int index = 0; index < this._rpct.Length; ++index)
      this._rRpct[this._rpct[index]] = index;
  }

  public string Solve()
  {
    int num = this._cK;
    StringBuilder stringBuilder = new StringBuilder();
    foreach (char c in this._cE)
    {
      stringBuilder.Append(this.Csr(-1 * num, c));
      ++num;
      if (num > this._rpctLastIndex)
        num = 0;
    }
    return stringBuilder.ToString();
  }

  private char Csr(int t, char c)
  {
    if (!this._rRpct.ContainsKey(c))
      return c;
    int index = this._rRpct[c] + t;
    if (index > this._rpctLastIndex)
      index = index - this._rpctLastIndex - 1;
    else if (0 > index)
      index = this._rpctLastIndex + index + 1;
    return this._rpct[index];
  }
}
