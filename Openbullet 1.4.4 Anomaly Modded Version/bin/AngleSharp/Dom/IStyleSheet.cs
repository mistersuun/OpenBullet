// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.IStyleSheet
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Css.Dom;
using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Dom;

[DomName("StyleSheet")]
public interface IStyleSheet : IStyleFormattable
{
  [DomName("type")]
  string Type { get; }

  [DomName("href")]
  string Href { get; }

  [DomName("ownerNode")]
  IElement OwnerNode { get; }

  [DomName("title")]
  string Title { get; }

  [DomName("media")]
  [DomPutForwards("mediaText")]
  IMediaList Media { get; }

  [DomName("disabled")]
  bool IsDisabled { get; set; }

  IBrowsingContext Context { get; }

  TextSource Source { get; }

  void SetOwner(IElement element);

  string LocateNamespace(string prefix);
}
