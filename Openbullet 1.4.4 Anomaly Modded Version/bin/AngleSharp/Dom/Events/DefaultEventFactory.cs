// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Events.DefaultEventFactory
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom.Events;
using System;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Dom.Events;

public class DefaultEventFactory : IEventFactory
{
  private readonly Dictionary<string, DefaultEventFactory.Creator> _creators = new Dictionary<string, DefaultEventFactory.Creator>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
  {
    {
      "event",
      (DefaultEventFactory.Creator) (() => new Event())
    },
    {
      "uievent",
      (DefaultEventFactory.Creator) (() => (Event) new UiEvent())
    },
    {
      "focusevent",
      (DefaultEventFactory.Creator) (() => (Event) new FocusEvent())
    },
    {
      "keyboardevent",
      (DefaultEventFactory.Creator) (() => (Event) new KeyboardEvent())
    },
    {
      "mouseevent",
      (DefaultEventFactory.Creator) (() => (Event) new MouseEvent())
    },
    {
      "wheelevent",
      (DefaultEventFactory.Creator) (() => (Event) new WheelEvent())
    },
    {
      "customevent",
      (DefaultEventFactory.Creator) (() => (Event) new CustomEvent())
    }
  };

  public DefaultEventFactory()
  {
    this.AddEventAlias("events", "event");
    this.AddEventAlias("htmlevents", "event");
    this.AddEventAlias("uievents", "uievent");
    this.AddEventAlias("keyevents", "keyboardevent");
    this.AddEventAlias("mouseevents", "mouseevent");
  }

  public void Register(string name, DefaultEventFactory.Creator creator)
  {
    this._creators.Add(name, creator);
  }

  public DefaultEventFactory.Creator Unregister(string name)
  {
    DefaultEventFactory.Creator creator;
    if (this._creators.TryGetValue(name, out creator))
      this._creators.Remove(name);
    return creator;
  }

  protected virtual Event CreateDefault(string name) => (Event) null;

  public Event Create(string name)
  {
    DefaultEventFactory.Creator creator;
    return name != null && this._creators.TryGetValue(name, out creator) ? creator() : this.CreateDefault(name);
  }

  private void AddEventAlias(string aliasName, string aliasFor)
  {
    this._creators.Add(aliasName, this._creators[aliasFor]);
  }

  public delegate Event Creator();
}
