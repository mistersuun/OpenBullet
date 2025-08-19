// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.DefaultAttributeObserver
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom;
using AngleSharp.Text;
using System;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Dom;

public class DefaultAttributeObserver : IAttributeObserver
{
  private readonly List<Action<IElement, string, string>> _actions;

  public DefaultAttributeObserver()
  {
    this._actions = new List<Action<IElement, string, string>>();
    this.RegisterStandardObservers();
  }

  protected virtual void RegisterStandardObservers()
  {
    this.RegisterObserver<Element>(AttributeNames.Class, (Action<Element, string>) ((element, value) => element.UpdateClassList(value)));
    this.RegisterObserver<HtmlElement>(AttributeNames.DropZone, (Action<HtmlElement, string>) ((element, value) => element.UpdateDropZone(value)));
    this.RegisterObserver<HtmlBaseElement>(AttributeNames.Href, (Action<HtmlBaseElement, string>) ((element, value) => element.UpdateUrl(value)));
    this.RegisterObserver<HtmlEmbedElement>(AttributeNames.Src, (Action<HtmlEmbedElement, string>) ((element, value) => element.UpdateSource(value)));
    this.RegisterObserver<HtmlLinkElement>(AttributeNames.Sizes, (Action<HtmlLinkElement, string>) ((element, value) => element.UpdateSizes(value)));
    this.RegisterObserver<HtmlLinkElement>(AttributeNames.Media, (Action<HtmlLinkElement, string>) ((element, value) => element.UpdateMedia(value)));
    this.RegisterObserver<HtmlLinkElement>(AttributeNames.Disabled, (Action<HtmlLinkElement, string>) ((element, value) => element.UpdateDisabled(value)));
    this.RegisterObserver<HtmlLinkElement>(AttributeNames.Href, (Action<HtmlLinkElement, string>) ((element, value) => element.UpdateSource(value)));
    this.RegisterObserver<HtmlUrlBaseElement>(AttributeNames.Rel, (Action<HtmlUrlBaseElement, string>) ((element, value) => element.UpdateRel(value)));
    this.RegisterObserver<HtmlUrlBaseElement>(AttributeNames.Ping, (Action<HtmlUrlBaseElement, string>) ((element, value) => element.UpdatePing(value)));
    this.RegisterObserver<HtmlTableCellElement>(AttributeNames.Headers, (Action<HtmlTableCellElement, string>) ((element, value) => element.UpdateHeaders(value)));
    this.RegisterObserver<HtmlStyleElement>(AttributeNames.Media, (Action<HtmlStyleElement, string>) ((element, value) => element.UpdateMedia(value)));
    this.RegisterObserver<HtmlSelectElement>(AttributeNames.Value, (Action<HtmlSelectElement, string>) ((element, value) => element.UpdateValue(value)));
    this.RegisterObserver<HtmlOutputElement>(AttributeNames.For, (Action<HtmlOutputElement, string>) ((element, value) => element.UpdateFor(value)));
    this.RegisterObserver<HtmlObjectElement>(AttributeNames.Data, (Action<HtmlObjectElement, string>) ((element, value) => element.UpdateSource(value)));
    this.RegisterObserver<HtmlAudioElement>(AttributeNames.Src, (Action<HtmlAudioElement, string>) ((element, value) => element.UpdateSource(value)));
    this.RegisterObserver<HtmlVideoElement>(AttributeNames.Src, (Action<HtmlVideoElement, string>) ((element, value) => element.UpdateSource(value)));
    this.RegisterObserver<HtmlImageElement>(AttributeNames.Src, (Action<HtmlImageElement, string>) ((element, value) => element.UpdateSource()));
    this.RegisterObserver<HtmlImageElement>(AttributeNames.SrcSet, (Action<HtmlImageElement, string>) ((element, value) => element.UpdateSource()));
    this.RegisterObserver<HtmlImageElement>(AttributeNames.Sizes, (Action<HtmlImageElement, string>) ((element, value) => element.UpdateSource()));
    this.RegisterObserver<HtmlImageElement>(AttributeNames.CrossOrigin, (Action<HtmlImageElement, string>) ((element, value) => element.UpdateSource()));
    this.RegisterObserver<HtmlIFrameElement>(AttributeNames.Sandbox, (Action<HtmlIFrameElement, string>) ((element, value) => element.UpdateSandbox(value)));
    this.RegisterObserver<HtmlIFrameElement>(AttributeNames.SrcDoc, (Action<HtmlIFrameElement, string>) ((element, value) => element.UpdateSource()));
    this.RegisterObserver<HtmlFrameElementBase>(AttributeNames.Src, (Action<HtmlFrameElementBase, string>) ((element, value) => element.UpdateSource()));
    this.RegisterObserver<HtmlInputElement>(AttributeNames.Type, (Action<HtmlInputElement, string>) ((element, value) => element.UpdateType(value)));
  }

  public void RegisterObserver<TElement>(string expectedName, Action<TElement, string> callback) where TElement : IElement
  {
    this._actions.Add((Action<IElement, string, string>) ((element, actualName, value) =>
    {
      if (!(element is TElement) || !actualName.Is(expectedName))
        return;
      callback((TElement) element, value);
    }));
  }

  void IAttributeObserver.NotifyChange(IElement host, string name, string value)
  {
    foreach (Action<IElement, string, string> action in this._actions)
      action(host, name, value);
  }
}
