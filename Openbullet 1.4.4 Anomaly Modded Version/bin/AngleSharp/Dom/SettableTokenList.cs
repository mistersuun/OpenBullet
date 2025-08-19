// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.SettableTokenList
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class SettableTokenList : 
  TokenList,
  ISettableTokenList,
  ITokenList,
  IEnumerable<string>,
  IEnumerable
{
  internal SettableTokenList(string value)
    : base(value)
  {
  }

  public string Value
  {
    get => this.ToString();
    set => this.Update(value);
  }
}
