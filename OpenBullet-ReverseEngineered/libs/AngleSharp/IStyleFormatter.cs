// Decompiled with JetBrains decompiler
// Type: AngleSharp.IStyleFormatter
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System.Collections.Generic;

#nullable disable
namespace AngleSharp;

public interface IStyleFormatter
{
  string Sheet(IEnumerable<IStyleFormattable> rules);

  string Declaration(string name, string value, bool important);

  string BlockDeclarations(IEnumerable<IStyleFormattable> declarations);

  string Rule(string name, string value);

  string Rule(string name, string prelude, string rules);

  string BlockRules(IEnumerable<IStyleFormattable> rules);

  string Comment(string data);
}
