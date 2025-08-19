// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.InputTypes.ButtonInputType
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom;
using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Html.InputTypes;

internal class ButtonInputType(IHtmlInputElement input, string name) : BaseInputType(input, name, false)
{
  public override bool IsAppendingData(IHtmlElement submitter)
  {
    return !this.Name.Is(InputTypeNames.Reset) || submitter == this.Input;
  }
}
