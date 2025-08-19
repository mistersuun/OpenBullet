// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.LastChildSelector
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using System;

#nullable disable
namespace AngleSharp.Css.Dom;

internal sealed class LastChildSelector(int step, int offset, ISelector kind) : 
  ChildSelector(PseudoClassNames.NthLastChild, step, offset, kind),
  ISelector
{
  public bool Match(IElement element, IElement scope)
  {
    IElement parentElement = element.ParentElement;
    if (parentElement != null)
    {
      int num1 = Math.Sign(this.Step);
      int num2 = 0;
      for (int index = parentElement.ChildNodes.Length - 1; index >= 0; --index)
      {
        if (parentElement.ChildNodes[index] is IElement childNode && this.Kind.Match(childNode, scope))
        {
          ++num2;
          if (childNode == element)
          {
            int num3 = num2 - this.Offset;
            if (num3 == 0)
              return true;
            return Math.Sign(num3) == num1 && num3 % this.Step == 0;
          }
        }
      }
    }
    return false;
  }
}
