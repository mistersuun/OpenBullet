// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.InputTypes.FileInputType
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom;
using AngleSharp.Io.Dom;

#nullable disable
namespace AngleSharp.Html.InputTypes;

internal class FileInputType : BaseInputType
{
  private readonly FileList _files;

  public FileInputType(IHtmlInputElement input, string name)
    : base(input, name, true)
  {
    this._files = new FileList();
  }

  public FileList Files => this._files;

  public override void ConstructDataSet(FormDataSet dataSet)
  {
    if (this._files.Length == 0)
      dataSet.Append(this.Input.Name, (IFile) null, this.Input.Type);
    foreach (IFile file in this._files)
      dataSet.Append(this.Input.Name, file, this.Input.Type);
  }
}
