// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.AttrAvailableSelector
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;

#nullable disable
namespace AngleSharp.Css.Dom;

internal sealed class AttrAvailableSelector(string name, string prefix) : BaseAttrSelector(name, prefix), ISelector
{
  public string Text => $"[{this.Attribute}]";

  public void Accept(ISelectorVisitor visitor)
  {
    visitor.Attribute(this.Attribute, string.Empty, (string) null);
  }

  public bool Match(IElement element, IElement scope) => element.HasAttribute(this.Name);
}
