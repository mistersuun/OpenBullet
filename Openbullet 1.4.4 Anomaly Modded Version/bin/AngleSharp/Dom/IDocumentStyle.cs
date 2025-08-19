// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.IDocumentStyle
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom;

[DomName("DocumentStyle")]
[DomNoInterfaceObject]
public interface IDocumentStyle
{
  [DomName("styleSheets")]
  IStyleSheetList StyleSheets { get; }

  [DomName("selectedStyleSheetSet")]
  string SelectedStyleSheetSet { get; set; }

  [DomName("lastStyleSheetSet")]
  string LastStyleSheetSet { get; }

  [DomName("preferredStyleSheetSet")]
  string PreferredStyleSheetSet { get; }

  [DomName("styleSheetSets")]
  IStringList StyleSheetSets { get; }

  [DomName("enableStyleSheetsForSet")]
  void EnableStyleSheetsForSet(string name);
}
