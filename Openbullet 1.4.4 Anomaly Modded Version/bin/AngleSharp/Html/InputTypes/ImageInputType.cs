// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.InputTypes.ImageInputType
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io.Processors;

#nullable disable
namespace AngleSharp.Html.InputTypes;

internal class ImageInputType : BaseInputType
{
  private readonly ImageRequestProcessor _request;

  public ImageInputType(IHtmlInputElement input, string name)
    : base(input, name, true)
  {
    HtmlInputElement htmlInputElement = input as HtmlInputElement;
    string source = input.Source;
    if (source == null || htmlInputElement == null)
      return;
    Url url = htmlInputElement.HyperReference(source);
    this._request = new ImageRequestProcessor(htmlInputElement.Context);
    htmlInputElement.Process((IRequestProcessor) this._request, url);
  }

  public int Width
  {
    get
    {
      ImageRequestProcessor request = this._request;
      return request == null ? 0 : request.Width;
    }
  }

  public int Height
  {
    get
    {
      ImageRequestProcessor request = this._request;
      return request == null ? 0 : request.Height;
    }
  }

  public override bool IsAppendingData(IHtmlElement submitter)
  {
    return submitter == this.Input && !string.IsNullOrEmpty(this.Input.Name);
  }

  public override void ConstructDataSet(FormDataSet dataSet)
  {
    string name = this.Input.Name;
    string str = this.Input.Value;
    dataSet.Append(name + ".x", "0", this.Input.Type);
    dataSet.Append(name + ".y", "0", this.Input.Type);
    if (string.IsNullOrEmpty(str))
      return;
    dataSet.Append(name, str, this.Input.Type);
  }
}
