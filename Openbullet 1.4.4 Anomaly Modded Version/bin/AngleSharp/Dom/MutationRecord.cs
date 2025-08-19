// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.MutationRecord
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class MutationRecord : IMutationRecord
{
  private static readonly string CharacterDataType = "characterData";
  private static readonly string AttributesType = "attributes";
  private static readonly string ChildListType = "childList";

  private MutationRecord()
  {
  }

  public static MutationRecord CharacterData(INode target, string previousValue = null)
  {
    return new MutationRecord()
    {
      Type = MutationRecord.CharacterDataType,
      Target = target,
      PreviousValue = previousValue
    };
  }

  public static MutationRecord ChildList(
    INode target,
    INodeList addedNodes = null,
    INodeList removedNodes = null,
    INode previousSibling = null,
    INode nextSibling = null)
  {
    return new MutationRecord()
    {
      Type = MutationRecord.ChildListType,
      Target = target,
      Added = addedNodes,
      Removed = removedNodes,
      PreviousSibling = previousSibling,
      NextSibling = nextSibling
    };
  }

  public static MutationRecord Attributes(
    INode target,
    string attributeName = null,
    string attributeNamespace = null,
    string previousValue = null)
  {
    return new MutationRecord()
    {
      Type = MutationRecord.AttributesType,
      Target = target,
      AttributeName = attributeName,
      AttributeNamespace = attributeNamespace,
      PreviousValue = previousValue
    };
  }

  public MutationRecord Copy(bool clearPreviousValue)
  {
    return new MutationRecord()
    {
      Type = this.Type,
      Target = this.Target,
      PreviousSibling = this.PreviousSibling,
      NextSibling = this.NextSibling,
      AttributeName = this.AttributeName,
      AttributeNamespace = this.AttributeNamespace,
      PreviousValue = clearPreviousValue ? (string) null : this.PreviousValue,
      Added = this.Added,
      Removed = this.Removed
    };
  }

  public bool IsAttribute => this.Type.Is(MutationRecord.AttributesType);

  public bool IsCharacterData => this.Type.Is(MutationRecord.CharacterDataType);

  public bool IsChildList => this.Type.Is(MutationRecord.ChildListType);

  public string Type { get; private set; }

  public INode Target { get; private set; }

  public INodeList Added { get; private set; }

  public INodeList Removed { get; private set; }

  public INode PreviousSibling { get; private set; }

  public INode NextSibling { get; private set; }

  public string AttributeName { get; private set; }

  public string AttributeNamespace { get; private set; }

  public string PreviousValue { get; private set; }
}
