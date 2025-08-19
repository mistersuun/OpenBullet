// Decompiled with JetBrains decompiler
// Type: RuriLib.Models.WebhookFormat
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;

#nullable disable
namespace RuriLib.Models;

public class WebhookFormat
{
  public WebhookFormat(
    string data,
    string type,
    string capturedData,
    DateTime timestamp,
    string configName,
    string configAuthor,
    string user)
  {
    this.Data = data;
    this.Type = type;
    this.CapturedData = capturedData;
    this.Timestamp = Math.Round(timestamp.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
    this.ConfigName = configName;
    this.ConfigAuthor = configAuthor;
    this.User = user;
  }

  public string Data { get; set; }

  public string Type { get; set; }

  public string CapturedData { get; set; }

  public double Timestamp { get; set; }

  public string ConfigName { get; set; }

  public string ConfigAuthor { get; set; }

  public string User { get; set; }
}
