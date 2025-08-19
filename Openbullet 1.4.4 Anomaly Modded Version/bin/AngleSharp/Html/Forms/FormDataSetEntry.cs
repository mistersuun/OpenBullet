// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Forms.FormDataSetEntry
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System.Text;

#nullable disable
namespace AngleSharp.Html.Forms;

public abstract class FormDataSetEntry
{
  private readonly string _name;
  private readonly string _type;

  public FormDataSetEntry(string name, string type)
  {
    this._name = name;
    this._type = type;
  }

  public bool HasName => this._name != null;

  public string Name => this._name ?? string.Empty;

  public string Type => this._type ?? InputTypeNames.Text;

  public abstract void Accept(IFormDataSetVisitor visitor);

  public abstract bool Contains(string boundary, Encoding encoding);
}
