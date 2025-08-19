// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.ClassSelector
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;

#nullable disable
namespace AngleSharp.Css.Dom;

internal sealed class ClassSelector : ISelector
{
  private readonly string _cls;

  public ClassSelector(string cls) => this._cls = cls;

  public Priority Specificity => Priority.OneClass;

  public string Text => "." + this._cls;

  public void Accept(ISelectorVisitor visitor) => visitor.Class(this._cls);

  public bool Match(IElement element, IElement scope) => element.ClassList.Contains(this._cls);
}
