// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.IWindowTimers
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using System;

#nullable disable
namespace AngleSharp.Dom;

[DomName("WindowTimers")]
[DomNoInterfaceObject]
public interface IWindowTimers
{
  [DomName("setTimeout")]
  int SetTimeout(Action<IWindow> handler, int timeout = 0);

  [DomName("clearTimeout")]
  void ClearTimeout(int handle = 0);

  [DomName("setInterval")]
  int SetInterval(Action<IWindow> handler, int timeout = 0);

  [DomName("clearInterval")]
  void ClearInterval(int handle = 0);
}
