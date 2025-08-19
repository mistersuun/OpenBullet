// Decompiled with JetBrains decompiler
// Type: RuriLib.BlockLSCode
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace RuriLib;

public class BlockLSCode : BlockBase
{
  private string script = "";

  public string Script
  {
    get => this.script;
    set
    {
      this.script = value;
      this.OnPropertyChanged(nameof (Script));
    }
  }

  public BlockLSCode() => this.Label = "LS";

  public override BlockBase FromLS(List<string> lines)
  {
    this.Script = string.Join(Environment.NewLine, (IEnumerable<string>) lines);
    return (BlockBase) this;
  }

  public override string ToLS(bool indent = true) => this.script;
}
