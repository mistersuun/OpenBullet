// Decompiled with JetBrains decompiler
// Type: RuriLib.TypeSwitch
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace RuriLib;

public class TypeSwitch
{
  private Dictionary<Type, Action<object>> matches = new Dictionary<Type, Action<object>>();

  public TypeSwitch Case<T>(Action<T> action)
  {
    this.matches.Add(typeof (T), (Action<object>) (x => action((T) x)));
    return this;
  }

  public void Switch(object x) => this.matches[x.GetType()](x);
}
