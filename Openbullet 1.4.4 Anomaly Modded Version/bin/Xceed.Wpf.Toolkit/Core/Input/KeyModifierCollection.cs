// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Input.KeyModifierCollection
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Input;

[TypeConverter(typeof (KeyModifierCollectionConverter))]
public class KeyModifierCollection : Collection<KeyModifier>
{
  public bool AreActive
  {
    get
    {
      if (this.Count == 0)
        return true;
      if (this.Contains(KeyModifier.Blocked))
        return false;
      return this.Contains(KeyModifier.Exact) ? this.IsExactMatch() : this.MatchAny();
    }
  }

  private static bool IsKeyPressed(KeyModifier modifier, ICollection<Key> keys)
  {
    switch (modifier)
    {
      case KeyModifier.None:
        return true;
      case KeyModifier.Ctrl:
        return keys.Contains(Key.LeftCtrl) || keys.Contains(Key.RightCtrl);
      case KeyModifier.LeftCtrl:
        return keys.Contains(Key.LeftCtrl);
      case KeyModifier.RightCtrl:
        return keys.Contains(Key.RightCtrl);
      case KeyModifier.Shift:
        return keys.Contains(Key.LeftShift) || keys.Contains(Key.RightShift);
      case KeyModifier.LeftShift:
        return keys.Contains(Key.LeftShift);
      case KeyModifier.RightShift:
        return keys.Contains(Key.RightShift);
      case KeyModifier.Alt:
        return keys.Contains(Key.LeftAlt) || keys.Contains(Key.RightAlt);
      case KeyModifier.LeftAlt:
        return keys.Contains(Key.LeftAlt);
      case KeyModifier.RightAlt:
        return keys.Contains(Key.RightAlt);
      default:
        throw new NotSupportedException("Unknown modifier");
    }
  }

  private static bool HasModifier(Key key, ICollection<KeyModifier> modifiers)
  {
    switch (key)
    {
      case Key.LeftShift:
        return modifiers.Contains(KeyModifier.Shift) || modifiers.Contains(KeyModifier.LeftShift);
      case Key.RightShift:
        return modifiers.Contains(KeyModifier.Shift) || modifiers.Contains(KeyModifier.RightShift);
      case Key.LeftCtrl:
        return modifiers.Contains(KeyModifier.Ctrl) || modifiers.Contains(KeyModifier.LeftCtrl);
      case Key.RightCtrl:
        return modifiers.Contains(KeyModifier.Ctrl) || modifiers.Contains(KeyModifier.RightCtrl);
      case Key.LeftAlt:
        return modifiers.Contains(KeyModifier.Alt) || modifiers.Contains(KeyModifier.LeftAlt);
      case Key.RightAlt:
        return modifiers.Contains(KeyModifier.Alt) || modifiers.Contains(KeyModifier.RightAlt);
      default:
        throw new NotSupportedException("Unknown key");
    }
  }

  private bool IsExactMatch()
  {
    HashSet<KeyModifier> keyModifiers = this.GetKeyModifiers();
    HashSet<Key> keysPressed = this.GetKeysPressed();
    if (this.Contains(KeyModifier.None))
      return keyModifiers.Count == 0 && keysPressed.Count == 0;
    foreach (KeyModifier modifier in keyModifiers)
    {
      if (!KeyModifierCollection.IsKeyPressed(modifier, (ICollection<Key>) keysPressed))
        return false;
    }
    foreach (Key key in keysPressed)
    {
      if (!KeyModifierCollection.HasModifier(key, (ICollection<KeyModifier>) keyModifiers))
        return false;
    }
    return true;
  }

  private bool MatchAny()
  {
    if (this.Contains(KeyModifier.None))
      return true;
    HashSet<KeyModifier> keyModifiers = this.GetKeyModifiers();
    HashSet<Key> keysPressed = this.GetKeysPressed();
    foreach (KeyModifier modifier in keyModifiers)
    {
      if (KeyModifierCollection.IsKeyPressed(modifier, (ICollection<Key>) keysPressed))
        return true;
    }
    return false;
  }

  private HashSet<KeyModifier> GetKeyModifiers()
  {
    HashSet<KeyModifier> keyModifiers = new HashSet<KeyModifier>();
    foreach (KeyModifier keyModifier in (Collection<KeyModifier>) this)
    {
      switch (keyModifier)
      {
        case KeyModifier.Ctrl:
        case KeyModifier.LeftCtrl:
        case KeyModifier.RightCtrl:
        case KeyModifier.Shift:
        case KeyModifier.LeftShift:
        case KeyModifier.RightShift:
        case KeyModifier.Alt:
        case KeyModifier.LeftAlt:
        case KeyModifier.RightAlt:
          if (!keyModifiers.Contains(keyModifier))
          {
            keyModifiers.Add(keyModifier);
            continue;
          }
          continue;
        default:
          continue;
      }
    }
    return keyModifiers;
  }

  private HashSet<Key> GetKeysPressed()
  {
    HashSet<Key> keysPressed = new HashSet<Key>();
    if (Keyboard.IsKeyDown(Key.LeftAlt))
      keysPressed.Add(Key.LeftAlt);
    if (Keyboard.IsKeyDown(Key.RightAlt))
      keysPressed.Add(Key.RightAlt);
    if (Keyboard.IsKeyDown(Key.LeftCtrl))
      keysPressed.Add(Key.LeftCtrl);
    if (Keyboard.IsKeyDown(Key.RightCtrl))
      keysPressed.Add(Key.RightCtrl);
    if (Keyboard.IsKeyDown(Key.LeftShift))
      keysPressed.Add(Key.LeftShift);
    if (Keyboard.IsKeyDown(Key.RightShift))
      keysPressed.Add(Key.RightShift);
    return keysPressed;
  }
}
