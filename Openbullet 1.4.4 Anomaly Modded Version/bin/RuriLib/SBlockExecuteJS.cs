// Decompiled with JetBrains decompiler
// Type: RuriLib.SBlockExecuteJS
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using RuriLib.LS;
using System;
using System.Windows.Media;

#nullable disable
namespace RuriLib;

public class SBlockExecuteJS : BlockBase
{
  private string javascriptCode = "alert('henlo');";

  public string JavascriptCode
  {
    get => this.javascriptCode;
    set
    {
      this.javascriptCode = value;
      this.OnPropertyChanged(nameof (JavascriptCode));
    }
  }

  public SBlockExecuteJS() => this.Label = "EXECUTE JS";

  public override BlockBase FromLS(string line)
  {
    string input = line.Trim();
    if (input.StartsWith("#"))
      this.Label = LineParser.ParseLabel(ref input);
    this.JavascriptCode = LineParser.ParseLiteral(ref input, "SCRIPT");
    return (BlockBase) this;
  }

  public override string ToLS(bool indent = true)
  {
    BlockWriter blockWriter = new BlockWriter(this.GetType(), indent, this.Disabled);
    blockWriter.Label(this.Label).Token((object) "EXECUTEJS").Literal(this.JavascriptCode.Replace("\r\n", " ").Replace("\n", " "));
    return blockWriter.ToString();
  }

  public override void Process(BotData data)
  {
    base.Process(data);
    if (data.Driver == null)
    {
      data.Log(new LogEntry("Open a browser first!", Colors.White));
      throw new Exception("Browser not open");
    }
    data.Log(new LogEntry("Executing JS code!", Colors.White));
    data.Driver.ExecuteScript(BlockBase.ReplaceValues(this.javascriptCode, data), Array.Empty<object>());
    data.Log(new LogEntry("... executed!", Colors.White));
    BlockBase.UpdateSeleniumData(data);
  }
}
