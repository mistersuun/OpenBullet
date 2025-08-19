// Decompiled with JetBrains decompiler
// Type: AngleSharp.Browser.Dom.INavigatorContentUtilities
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Browser.Dom;

[DomName("NavigatorContentUtils")]
[DomNoInterfaceObject]
public interface INavigatorContentUtilities
{
  [DomName("registerProtocolHandler")]
  void RegisterProtocolHandler(string scheme, string url, string title);

  [DomName("registerContentHandler")]
  void RegisterContentHandler(string mimeType, string url, string title);

  [DomName("isProtocolHandlerRegistered")]
  bool IsProtocolHandlerRegistered(string scheme, string url);

  [DomName("isContentHandlerRegistered")]
  bool IsContentHandlerRegistered(string mimeType, string url);

  [DomName("unregisterProtocolHandler")]
  void UnregisterProtocolHandler(string scheme, string url);

  [DomName("unregisterContentHandler")]
  void UnregisterContentHandler(string mimeType, string url);
}
