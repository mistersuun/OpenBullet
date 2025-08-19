// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.ProcessingInstruction
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

#nullable disable
namespace AngleSharp.Dom;

internal sealed class ProcessingInstruction : 
  CharacterData,
  IProcessingInstruction,
  ICharacterData,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IChildNode,
  INonDocumentTypeChildNode
{
  internal ProcessingInstruction(Document owner, string name)
    : base(owner, name, NodeType.ProcessingInstruction)
  {
  }

  public string Target => this.NodeName;

  public override Node Clone(Document owner, bool deep)
  {
    ProcessingInstruction target = new ProcessingInstruction(owner, this.Target);
    this.CloneNode((Node) target, owner, deep);
    return (Node) target;
  }

  internal static ProcessingInstruction Create(Document owner, string data)
  {
    int num = data.IndexOf(' ');
    ProcessingInstruction processingInstruction = new ProcessingInstruction(owner, num <= 0 ? data : data.Substring(0, num));
    if (num > 0)
      processingInstruction.Data = data.Substring(num);
    return processingInstruction;
  }
}
