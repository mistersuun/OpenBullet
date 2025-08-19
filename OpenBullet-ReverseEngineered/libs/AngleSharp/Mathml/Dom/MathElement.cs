// Decompiled with JetBrains decompiler
// Type: AngleSharp.Mathml.Dom.MathElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;

#nullable disable
namespace AngleSharp.Mathml.Dom;

public class MathElement(Document owner, string name, string prefix = null, NodeFlags flags = NodeFlags.None) : 
  Element(owner, name, prefix, NamespaceNames.MathMlUri, flags | NodeFlags.MathMember)
{
  public override IElement ParseSubtree(string html) => this.ParseHtmlSubtree(html);

  public override Node Clone(Document owner, bool deep)
  {
    MathElement mathElement = this.Context.GetFactory<IElementFactory<Document, MathElement>>().Create(owner, this.LocalName, this.Prefix);
    this.CloneElement((Element) mathElement, owner, deep);
    return (Node) mathElement;
  }
}
