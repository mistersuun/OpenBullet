// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.ISelectorVisitor
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Css.Dom;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Css;

public interface ISelectorVisitor
{
  void Attribute(string name, string op, string value);

  void Type(string name);

  void Id(string value);

  void Child(string name, int step, int offset, ISelector selector);

  void Class(string name);

  void PseudoClass(string name);

  void PseudoElement(string name);

  void List(IEnumerable<ISelector> selectors);

  void Combinator(IEnumerable<ISelector> selectors, IEnumerable<string> symbols);

  void Many(IEnumerable<ISelector> selectors);
}
