// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IValidation
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomNoInterfaceObject]
public interface IValidation
{
  [DomName("willValidate")]
  bool WillValidate { get; }

  [DomName("validity")]
  IValidityState Validity { get; }

  [DomName("validationMessage")]
  string ValidationMessage { get; }

  [DomName("checkValidity")]
  bool CheckValidity();

  [DomName("setCustomValidity")]
  void SetCustomValidity(string error);
}
