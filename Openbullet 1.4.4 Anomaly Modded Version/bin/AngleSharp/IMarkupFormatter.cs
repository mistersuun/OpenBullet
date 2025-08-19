// Decompiled with JetBrains decompiler
// Type: AngleSharp.IMarkupFormatter
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;

#nullable disable
namespace AngleSharp;

public interface IMarkupFormatter
{
  string Text(ICharacterData text);

  string Comment(IComment comment);

  string Processing(IProcessingInstruction processing);

  string Doctype(IDocumentType doctype);

  string OpenTag(IElement element, bool selfClosing);

  string CloseTag(IElement element, bool selfClosing);

  string Attribute(IAttr attribute);
}
