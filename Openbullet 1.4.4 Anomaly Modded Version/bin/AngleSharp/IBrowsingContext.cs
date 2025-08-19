// Decompiled with JetBrains decompiler
// Type: AngleSharp.IBrowsingContext
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Browser;
using AngleSharp.Browser.Dom;
using AngleSharp.Dom;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp;

public interface IBrowsingContext : IEventTarget
{
  IWindow Current { get; }

  IDocument Active { get; set; }

  IHistory SessionHistory { get; }

  Sandboxes Security { get; }

  IBrowsingContext Parent { get; }

  IDocument Creator { get; }

  IEnumerable<object> OriginalServices { get; }

  T GetService<T>() where T : class;

  IEnumerable<T> GetServices<T>() where T : class;

  IBrowsingContext CreateChild(string name, Sandboxes security);

  IBrowsingContext FindChild(string name);
}
