// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlOptionsCollection
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLOptionsCollection")]
public interface IHtmlOptionsCollection : 
  IHtmlCollection<IHtmlOptionElement>,
  IEnumerable<IHtmlOptionElement>,
  IEnumerable
{
  [DomAccessor(Accessors.Getter)]
  IHtmlOptionElement GetOptionAt(int index);

  [DomAccessor(Accessors.Setter)]
  void SetOptionAt(int index, IHtmlOptionElement option);

  [DomName("add")]
  void Add(IHtmlOptionElement element, IHtmlElement before = null);

  [DomName("add")]
  void Add(IHtmlOptionsGroupElement element, IHtmlElement before = null);

  [DomName("remove")]
  void Remove(int index);

  [DomName("selectedIndex")]
  int SelectedIndex { get; set; }
}
