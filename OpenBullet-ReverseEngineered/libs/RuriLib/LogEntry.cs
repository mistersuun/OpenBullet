// Decompiled with JetBrains decompiler
// Type: RuriLib.LogEntry
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using RuriLib.ViewModels;
using System;
using System.Windows.Media;

#nullable disable
namespace RuriLib;

public class LogEntry : ViewModelBase
{
  private string logString = "";
  private Color logColor = Colors.White;
  private DateTime logTime = DateTime.Now;
  private LogLevel logLevel;
  private string logComponent = "";

  public string LogString
  {
    get => this.logString;
    set
    {
      this.logString = value;
      this.OnPropertyChanged(nameof (LogString));
    }
  }

  public Color LogColor
  {
    get => this.logColor;
    set
    {
      this.logColor = value;
      this.OnPropertyChanged(nameof (LogColor));
    }
  }

  public DateTime LogTime
  {
    get => this.logTime;
    set
    {
      this.logTime = value;
      this.OnPropertyChanged(nameof (LogTime));
    }
  }

  public LogLevel LogLevel
  {
    get => this.logLevel;
    set
    {
      this.logLevel = value;
      this.OnPropertyChanged(nameof (LogLevel));
    }
  }

  public string LogComponent
  {
    get => this.logComponent;
    set
    {
      this.logComponent = value;
      this.OnPropertyChanged(nameof (LogComponent));
    }
  }

  public LogEntry(string logString, Color logColor)
  {
    this.LogString = logString;
    this.LogColor = logColor;
    this.LogTime = DateTime.Now;
  }

  public LogEntry(string logComponent, string logString, LogLevel logLevel)
  {
    this.LogString = logString;
    this.LogTime = DateTime.Now;
    this.LogLevel = logLevel;
    this.LogComponent = logComponent;
    switch (this.LogLevel)
    {
      case LogLevel.Info:
        this.LogColor = Colors.White;
        break;
      case LogLevel.Warning:
        this.LogColor = Colors.Gold;
        break;
      case LogLevel.Error:
        this.LogColor = Colors.Tomato;
        break;
    }
  }
}
