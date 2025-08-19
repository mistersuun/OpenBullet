// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Events.ErrorEvent
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using System;

#nullable disable
namespace AngleSharp.Dom.Events;

[DomName("ErrorEvent")]
public class ErrorEvent : Event
{
  [DomName("message")]
  public string Message => this.Error.Message;

  [DomName("filename")]
  public string FileName { get; private set; }

  [DomName("lineno")]
  public int Line { get; private set; }

  [DomName("colno")]
  public int Column { get; private set; }

  [DomName("error")]
  public Exception Error { get; private set; }

  public void Init(string filename, int line, int column, Exception error)
  {
    this.FileName = filename;
    this.Line = line;
    this.Column = column;
    this.Error = error;
  }
}
