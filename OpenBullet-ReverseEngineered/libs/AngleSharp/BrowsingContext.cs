// Decompiled with JetBrains decompiler
// Type: AngleSharp.BrowsingContext
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Browser;
using AngleSharp.Browser.Dom;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp;

public sealed class BrowsingContext : EventTarget, IBrowsingContext, IEventTarget, IDisposable
{
  private readonly IEnumerable<object> _originalServices;
  private readonly List<object> _services;
  private readonly Sandboxes _security;
  private readonly IBrowsingContext _parent;
  private readonly IDocument _creator;
  private readonly IHistory _history;
  private readonly Dictionary<string, WeakReference<IBrowsingContext>> _children;

  private BrowsingContext(Sandboxes security)
  {
    this._services = new List<object>();
    this._originalServices = (IEnumerable<object>) this._services;
    this._security = security;
    this._children = new Dictionary<string, WeakReference<IBrowsingContext>>();
  }

  internal BrowsingContext(IEnumerable<object> services, Sandboxes security)
    : this(security)
  {
    this._services.AddRange(services);
    this._originalServices = services;
    this._history = this.GetService<IHistory>();
  }

  internal BrowsingContext(IBrowsingContext parent, Sandboxes security)
    : this(parent.OriginalServices, security)
  {
    this._parent = parent;
    this._creator = this._parent.Active;
  }

  public IDocument Active { get; set; }

  public IDocument Creator => this._creator;

  public IEnumerable<object> OriginalServices => this._originalServices;

  public IWindow Current => this.Active?.DefaultView;

  public IBrowsingContext Parent => this._parent;

  public IHistory SessionHistory => this._history;

  public Sandboxes Security => this._security;

  public T GetService<T>() where T : class
  {
    int count = this._services.Count;
    for (int index = 0; index < count; ++index)
    {
      switch (this._services[index])
      {
        case T service:
label_3:
          return service;
        case Func<IBrowsingContext, T> func:
          service = func((IBrowsingContext) this);
          this._services[index] = (object) service;
          goto label_3;
        default:
          continue;
      }
    }
    return default (T);
  }

  public IEnumerable<T> GetServices<T>() where T : class
  {
    BrowsingContext browsingContext = this;
    int count = browsingContext._services.Count;
    for (int i = 0; i < count; ++i)
    {
      switch (browsingContext._services[i])
      {
        case T service:
label_4:
          yield return service;
          break;
        case Func<IBrowsingContext, T> func:
          service = func((IBrowsingContext) browsingContext);
          browsingContext._services[i] = (object) service;
          goto label_4;
      }
    }
  }

  public IBrowsingContext CreateChild(string name, Sandboxes security)
  {
    BrowsingContext child = new BrowsingContext((IBrowsingContext) this, security);
    if (!string.IsNullOrEmpty(name))
      this._children[name] = new WeakReference<IBrowsingContext>((IBrowsingContext) child);
    return (IBrowsingContext) child;
  }

  public IBrowsingContext FindChild(string name)
  {
    IBrowsingContext child = (IBrowsingContext) null;
    WeakReference<IBrowsingContext> weakReference;
    if (!string.IsNullOrEmpty(name) && this._children.TryGetValue(name, out weakReference))
      weakReference.TryGetTarget(ref child);
    return child;
  }

  public static IBrowsingContext New(IConfiguration configuration = null)
  {
    if (configuration == null)
      configuration = Configuration.Default;
    return (IBrowsingContext) new BrowsingContext(configuration.Services, Sandboxes.None);
  }

  public static IBrowsingContext NewFrom<TService>(TService instance)
  {
    return (IBrowsingContext) new BrowsingContext(Configuration.Default.WithOnly<TService>(instance).Services, Sandboxes.None);
  }

  void IDisposable.Dispose()
  {
    this.Active?.Dispose();
    this.Active = (IDocument) null;
  }
}
