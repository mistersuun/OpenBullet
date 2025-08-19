// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.IWindow
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Browser.Dom;
using AngleSharp.Dom.Events;
using System;

#nullable disable
namespace AngleSharp.Dom;

[DomName("Window")]
public interface IWindow : 
  IEventTarget,
  IGlobalEventHandlers,
  IWindowEventHandlers,
  IWindowTimers,
  IDisposable
{
  [DomName("document")]
  IDocument Document { get; }

  [DomName("location")]
  [DomPutForwards("href")]
  ILocation Location { get; }

  [DomName("closed")]
  bool IsClosed { get; }

  [DomName("status")]
  string Status { get; set; }

  [DomName("name")]
  string Name { get; set; }

  [DomName("outerHeight")]
  int OuterHeight { get; }

  [DomName("outerWidth")]
  int OuterWidth { get; }

  [DomName("screenX")]
  int ScreenX { get; }

  [DomName("screenY")]
  int ScreenY { get; }

  [DomName("window")]
  [DomName("frames")]
  [DomName("self")]
  IWindow Proxy { get; }

  [DomName("navigator")]
  INavigator Navigator { get; }

  [DomName("close")]
  void Close();

  IWindow Open(string url = "about:blank", string name = null, string features = null, string replace = null);

  [DomName("stop")]
  void Stop();

  [DomName("focus")]
  void Focus();

  [DomName("blur")]
  void Blur();

  [DomName("alert")]
  void Alert(string message);

  [DomName("confirm")]
  bool Confirm(string message);

  [DomName("print")]
  void Print();

  [DomName("history")]
  IHistory History { get; }
}
