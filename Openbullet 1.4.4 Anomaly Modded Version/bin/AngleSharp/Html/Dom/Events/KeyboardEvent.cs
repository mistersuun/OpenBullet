// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.Events.KeyboardEvent
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom.Events;

[DomName("KeyboardEvent")]
public class KeyboardEvent : UiEvent
{
  private string _modifiers;

  public KeyboardEvent()
  {
  }

  [DomConstructor]
  [DomInitDict(1, true)]
  public KeyboardEvent(
    string type,
    bool bubbles = false,
    bool cancelable = false,
    IWindow view = null,
    int detail = 0,
    string key = null,
    KeyboardLocation location = KeyboardLocation.Standard,
    string modifiersList = null,
    bool repeat = false)
  {
    this.Init(type, bubbles, cancelable, view, detail, key ?? string.Empty, location, modifiersList ?? string.Empty, repeat);
  }

  [DomName("key")]
  public string Key { get; private set; }

  [DomName("location")]
  public KeyboardLocation Location { get; private set; }

  [DomName("ctrlKey")]
  public bool IsCtrlPressed => this._modifiers.IsCtrlPressed();

  [DomName("shiftKey")]
  public bool IsShiftPressed => this._modifiers.IsShiftPressed();

  [DomName("altKey")]
  public bool IsAltPressed => this._modifiers.IsAltPressed();

  [DomName("metaKey")]
  public bool IsMetaPressed => this._modifiers.IsMetaPressed();

  [DomName("repeat")]
  public bool IsRepeated { get; private set; }

  [DomName("getModifierState")]
  public bool GetModifierState(string key) => this._modifiers.ContainsKey(key);

  [DomName("locale")]
  public string Locale => !this.IsTrusted ? (string) null : string.Empty;

  [DomName("initKeyboardEvent")]
  public void Init(
    string type,
    bool bubbles,
    bool cancelable,
    IWindow view,
    int detail,
    string key,
    KeyboardLocation location,
    string modifiersList,
    bool repeat)
  {
    this.Init(type, bubbles, cancelable, view, detail);
    this.Key = key;
    this.Location = location;
    this.IsRepeated = repeat;
    this._modifiers = modifiersList;
  }
}
