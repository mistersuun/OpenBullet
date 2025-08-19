// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Events.IWindowEventHandlers
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom.Events;

[DomName("WindowEventHandlers")]
[DomNoInterfaceObject]
public interface IWindowEventHandlers
{
  [DomName("onafterprint")]
  event DomEventHandler Printed;

  [DomName("onbeforeprint")]
  event DomEventHandler Printing;

  [DomName("onbeforeunload")]
  event DomEventHandler Unloading;

  [DomName("onhashchange")]
  event DomEventHandler HashChanged;

  [DomName("onmessage")]
  event DomEventHandler MessageReceived;

  [DomName("onoffline")]
  event DomEventHandler WentOffline;

  [DomName("ononline")]
  event DomEventHandler WentOnline;

  [DomName("onpagehide")]
  event DomEventHandler PageHidden;

  [DomName("onpageshow")]
  event DomEventHandler PageShown;

  [DomName("onpopstate")]
  event DomEventHandler PopState;

  [DomName("onstorage")]
  event DomEventHandler Storage;

  [DomName("onunload")]
  event DomEventHandler Unloaded;
}
