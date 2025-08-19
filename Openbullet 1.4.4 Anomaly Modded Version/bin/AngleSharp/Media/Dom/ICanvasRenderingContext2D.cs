// Decompiled with JetBrains decompiler
// Type: AngleSharp.Media.Dom.ICanvasRenderingContext2D
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Html.Dom;

#nullable disable
namespace AngleSharp.Media.Dom;

[DomName("CanvasRenderingContext2D")]
public interface ICanvasRenderingContext2D : IRenderingContext
{
  [DomName("canvas")]
  IHtmlCanvasElement Canvas { get; }

  [DomName("width")]
  int Width { get; set; }

  [DomName("height")]
  int Height { get; set; }

  [DomName("save")]
  void SaveState();

  [DomName("restore")]
  void RestoreState();
}
