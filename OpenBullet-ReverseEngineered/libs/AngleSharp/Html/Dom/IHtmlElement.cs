// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLElement")]
public interface IHtmlElement : 
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers
{
  [DomName("lang")]
  string Language { get; set; }

  [DomName("title")]
  string Title { get; set; }

  [DomName("dir")]
  string Direction { get; set; }

  [DomName("dataset")]
  IStringMap Dataset { get; }

  [DomName("translate")]
  bool IsTranslated { get; set; }

  [DomName("tabIndex")]
  int TabIndex { get; set; }

  [DomName("spellcheck")]
  bool IsSpellChecked { get; set; }

  [DomName("contentEditable")]
  string ContentEditable { get; set; }

  [DomName("isContentEditable")]
  bool IsContentEditable { get; }

  [DomName("hidden")]
  bool IsHidden { get; set; }

  [DomName("draggable")]
  bool IsDraggable { get; set; }

  [DomName("accessKey")]
  string AccessKey { get; set; }

  [DomName("accessKeyLabel")]
  string AccessKeyLabel { get; }

  [DomName("contextMenu")]
  IHtmlMenuElement ContextMenu { get; set; }

  [DomName("dropzone")]
  [DomPutForwards("value")]
  ISettableTokenList DropZone { get; }

  [DomName("click")]
  void DoClick();

  [DomName("focus")]
  void DoFocus();

  [DomName("blur")]
  void DoBlur();

  [DomName("forceSpellCheck")]
  void DoSpellCheck();
}
