// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.AnyElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

#nullable disable
namespace AngleSharp.Dom;

internal sealed class AnyElement(
  Document owner,
  string localName,
  string prefix,
  string namespaceUri,
  NodeFlags flags = NodeFlags.None) : Element(owner, localName, prefix, namespaceUri, flags)
{
  public override IElement ParseSubtree(string html) => this.ParseHtmlSubtree(html);
}
