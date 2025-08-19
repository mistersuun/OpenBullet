// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.ICharacterData
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom;

[DomName("CharacterData")]
public interface ICharacterData : 
  INode,
  IEventTarget,
  IMarkupFormattable,
  IChildNode,
  INonDocumentTypeChildNode
{
  [DomName("data")]
  string Data { get; set; }

  [DomName("length")]
  int Length { get; }

  [DomName("substringData")]
  string Substring(int offset, int count);

  [DomName("appendData")]
  void Append(string value);

  [DomName("insertData")]
  void Insert(int offset, string value);

  [DomName("deleteData")]
  void Delete(int offset, int count);

  [DomName("replaceData")]
  void Replace(int offset, int count, string value);
}
