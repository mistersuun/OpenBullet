// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.ListSelector
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Css.Dom;

internal sealed class ListSelector : Selectors, ISelector
{
  public void Accept(ISelectorVisitor visitor)
  {
    visitor.List((IEnumerable<ISelector>) this._selectors);
  }

  public bool Match(IElement element, IElement scope)
  {
    for (int index = 0; index < this._selectors.Count; ++index)
    {
      if (this._selectors[index].Match(element, scope))
        return true;
    }
    return false;
  }

  protected override string Stringify()
  {
    string[] strArray = new string[this._selectors.Count];
    for (int index = 0; index < this._selectors.Count; ++index)
      strArray[index] = this._selectors[index].Text;
    return string.Join(", ", strArray);
  }
}
