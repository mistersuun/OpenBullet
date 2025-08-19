// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlFormElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Io;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLFormElement")]
public interface IHtmlFormElement : 
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers
{
  [DomName("acceptCharset")]
  string AcceptCharset { get; set; }

  [DomName("action")]
  string Action { get; set; }

  [DomName("autocomplete")]
  string Autocomplete { get; set; }

  [DomName("enctype")]
  string Enctype { get; set; }

  [DomName("encoding")]
  string Encoding { get; set; }

  [DomName("method")]
  string Method { get; set; }

  [DomName("name")]
  string Name { get; set; }

  [DomName("noValidate")]
  bool NoValidate { get; set; }

  [DomName("target")]
  string Target { get; set; }

  [DomName("length")]
  int Length { get; }

  [DomName("elements")]
  IHtmlFormControlsCollection Elements { get; }

  [DomName("submit")]
  Task<IDocument> SubmitAsync();

  Task<IDocument> SubmitAsync(IHtmlElement sourceElement);

  DocumentRequest GetSubmission();

  DocumentRequest GetSubmission(IHtmlElement sourceElement);

  [DomName("reset")]
  void Reset();

  [DomName("checkValidity")]
  bool CheckValidity();

  [DomName("reportValidity")]
  bool ReportValidity();

  [DomAccessor(Accessors.Getter)]
  IElement this[int index] { get; }

  [DomAccessor(Accessors.Getter)]
  IElement this[string name] { get; }

  [DomName("requestAutocomplete")]
  void RequestAutocomplete();
}
