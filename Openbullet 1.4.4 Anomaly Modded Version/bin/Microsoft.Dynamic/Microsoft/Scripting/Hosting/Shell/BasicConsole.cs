// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Shell.BasicConsole
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.IO;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Hosting.Shell;

public class BasicConsole : IConsole, IDisposable
{
  private TextWriter _output;
  private TextWriter _errorOutput;
  private AutoResetEvent _ctrlCEvent;
  private Thread _creatingThread;
  private ConsoleColor _promptColor;
  private ConsoleColor _outColor;
  private ConsoleColor _errorColor;
  private ConsoleColor _warningColor;

  public TextWriter Output
  {
    get => this._output;
    set
    {
      ContractUtils.RequiresNotNull((object) value, nameof (value));
      this._output = value;
    }
  }

  public TextWriter ErrorOutput
  {
    get => this._errorOutput;
    set
    {
      ContractUtils.RequiresNotNull((object) value, nameof (value));
      this._errorOutput = value;
    }
  }

  protected AutoResetEvent CtrlCEvent
  {
    get => this._ctrlCEvent;
    set => this._ctrlCEvent = value;
  }

  protected Thread CreatingThread
  {
    get => this._creatingThread;
    set => this._creatingThread = value;
  }

  public ConsoleCancelEventHandler ConsoleCancelEventHandler { get; set; }

  public BasicConsole(bool colorful)
  {
    this._output = Console.Out;
    this._errorOutput = Console.Error;
    this.SetupColors(colorful);
    this._creatingThread = Thread.CurrentThread;
    this.ConsoleCancelEventHandler = (ConsoleCancelEventHandler) ((sender, e) =>
    {
      if (e.SpecialKey != ConsoleSpecialKey.ControlC)
        return;
      e.Cancel = true;
      this._ctrlCEvent.Set();
      this._creatingThread.Abort((object) new KeyboardInterruptException(""));
    });
    Console.CancelKeyPress += (ConsoleCancelEventHandler) ((sender, e) =>
    {
      if (this.ConsoleCancelEventHandler == null)
        return;
      this.ConsoleCancelEventHandler(sender, e);
    });
    this._ctrlCEvent = new AutoResetEvent(false);
  }

  private void SetupColors(bool colorful)
  {
    if (colorful)
    {
      this._promptColor = BasicConsole.PickColor(ConsoleColor.Gray, ConsoleColor.White);
      this._outColor = BasicConsole.PickColor(ConsoleColor.Cyan, ConsoleColor.White);
      this._errorColor = BasicConsole.PickColor(ConsoleColor.Red, ConsoleColor.White);
      this._warningColor = BasicConsole.PickColor(ConsoleColor.Yellow, ConsoleColor.White);
    }
    else
      this._promptColor = this._outColor = this._errorColor = this._warningColor = Console.ForegroundColor;
  }

  private static ConsoleColor PickColor(ConsoleColor best, ConsoleColor other)
  {
    best = BasicConsole.IsDark(Console.BackgroundColor) ? BasicConsole.MakeLight(best) : BasicConsole.MakeDark(best);
    other = BasicConsole.IsDark(Console.BackgroundColor) ? BasicConsole.MakeLight(other) : BasicConsole.MakeDark(other);
    return Console.BackgroundColor != best ? best : other;
  }

  private static bool IsDark(ConsoleColor color)
  {
    return color < ConsoleColor.Gray || color == ConsoleColor.DarkGray;
  }

  private static ConsoleColor MakeLight(ConsoleColor color)
  {
    return color == ConsoleColor.DarkGray ? ConsoleColor.White : color | ConsoleColor.White;
  }

  private static ConsoleColor MakeDark(ConsoleColor color)
  {
    return color == ConsoleColor.Gray ? ConsoleColor.Black : color & ~ConsoleColor.White;
  }

  protected void WriteColor(TextWriter output, string str, ConsoleColor c)
  {
    int foregroundColor = (int) Console.ForegroundColor;
    Console.ForegroundColor = c;
    output.Write(str);
    output.Flush();
    Console.ForegroundColor = (ConsoleColor) foregroundColor;
  }

  public virtual string ReadLine(int autoIndentSize)
  {
    this.Write("".PadLeft(autoIndentSize), Style.Prompt);
    string str = Console.In.ReadLine();
    if (str != null)
      return "".PadLeft(autoIndentSize) + str;
    return this._ctrlCEvent != null && this._ctrlCEvent.WaitOne(100, false) ? "" : (string) null;
  }

  public virtual void Write(string text, Style style)
  {
    switch (style)
    {
      case Style.Prompt:
        this.WriteColor(this._output, text, this._promptColor);
        break;
      case Style.Out:
        this.WriteColor(this._output, text, this._outColor);
        break;
      case Style.Error:
        this.WriteColor(this._errorOutput, text, this._errorColor);
        break;
      case Style.Warning:
        this.WriteColor(this._errorOutput, text, this._warningColor);
        break;
    }
  }

  public void WriteLine(string text, Style style) => this.Write(text + Environment.NewLine, style);

  public void WriteLine() => this.Write(Environment.NewLine, Style.Out);

  public void Dispose()
  {
    this._ctrlCEvent?.Close();
    GC.SuppressFinalize((object) this);
  }
}
