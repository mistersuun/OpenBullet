// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.EntityReference
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

#nullable disable
namespace AngleSharp.Dom;

internal sealed class EntityReference : Node
{
  internal EntityReference(Document owner)
    : this(owner, string.Empty)
  {
  }

  internal EntityReference(Document owner, string name)
    : base(owner, name, NodeType.EntityReference)
  {
  }

  public override Node Clone(Document owner, bool deep)
  {
    EntityReference target = new EntityReference(owner, this.NodeName);
    this.CloneNode((Node) target, owner, deep);
    return (Node) target;
  }
}
