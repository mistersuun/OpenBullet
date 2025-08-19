// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.IChildNode
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom;

[DomName("ChildNode")]
[DomNoInterfaceObject]
public interface IChildNode
{
  [DomName("before")]
  void Before(params INode[] nodes);

  [DomName("after")]
  void After(params INode[] nodes);

  [DomName("replace")]
  void Replace(params INode[] nodes);

  [DomName("remove")]
  void Remove();
}
