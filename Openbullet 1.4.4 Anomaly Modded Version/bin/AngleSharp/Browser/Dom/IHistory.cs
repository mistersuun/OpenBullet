// Decompiled with JetBrains decompiler
// Type: AngleSharp.Browser.Dom.IHistory
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;

#nullable disable
namespace AngleSharp.Browser.Dom;

[DomName("History")]
public interface IHistory
{
  [DomName("length")]
  int Length { get; }

  int Index { get; }

  IDocument this[int index] { get; }

  [DomName("state")]
  object State { get; }

  [DomName("go")]
  void Go(int delta = 0);

  [DomName("back")]
  void Back();

  [DomName("forward")]
  void Forward();

  [DomName("pushState")]
  void PushState(object data, string title, string url = null);

  [DomName("replaceState")]
  void ReplaceState(object data, string title, string url = null);
}
