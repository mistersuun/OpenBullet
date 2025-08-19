// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.ChildSelector
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

#nullable disable
namespace AngleSharp.Css.Dom;

internal abstract class ChildSelector
{
  private readonly string _name;
  private readonly int _step;
  private readonly int _offset;
  private readonly ISelector _kind;

  public ChildSelector(string name, int step, int offset, ISelector kind)
  {
    this._name = name;
    this._step = step;
    this._offset = offset;
    this._kind = kind;
  }

  public Priority Specificity => Priority.OneClass;

  public string Text
  {
    get
    {
      string str1 = this._step.ToString();
      string str2 = string.Empty;
      string empty = string.Empty;
      if (this._offset > 0)
      {
        str2 = "+";
        empty = this._offset.ToString();
      }
      else if (this._offset < 0)
      {
        str2 = "-";
        empty = (-this._offset).ToString();
      }
      return $":{this._name}({str1}n{str2}{empty})";
    }
  }

  public string Name => this._name;

  public int Step => this._step;

  public int Offset => this._offset;

  public ISelector Kind => this._kind;

  public void Accept(ISelectorVisitor visitor)
  {
    visitor.Child(this._name, this._step, this._offset, this._kind);
  }
}
