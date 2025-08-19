// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Forms.TextDataSetEntry
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System.Text;

#nullable disable
namespace AngleSharp.Html.Forms;

internal sealed class TextDataSetEntry : FormDataSetEntry
{
  private readonly string _value;

  public TextDataSetEntry(string name, string value, string type)
    : base(name, type)
  {
    this._value = value;
  }

  public override bool Contains(string boundary, Encoding encoding)
  {
    return this._value != null && this._value.Contains(boundary);
  }

  public override void Accept(IFormDataSetVisitor visitor)
  {
    visitor.Text((FormDataSetEntry) this, this._value);
  }
}
