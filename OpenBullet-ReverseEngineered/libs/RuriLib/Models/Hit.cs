// Decompiled with JetBrains decompiler
// Type: RuriLib.Models.Hit
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using LiteDB;
using Newtonsoft.Json;
using System;
using System.Text;

#nullable disable
namespace RuriLib.Models;

public class Hit
{
  public Guid Id { get; set; }

  public string Data { get; set; }

  public VariableList CapturedData { get; set; }

  [JsonIgnore]
  [BsonIgnore]
  public string CapturedString => this.CapturedData.ToCaptureString();

  public string Proxy { get; set; }

  public DateTime Date { get; set; }

  public string Type { get; set; }

  public string ConfigName { get; set; }

  public string WordlistName { get; set; }

  public Hit()
  {
  }

  public Hit(
    string data,
    VariableList capturedData,
    string proxy,
    string type,
    string configName,
    string wordlistName)
  {
    this.Data = data;
    this.CapturedData = capturedData;
    this.Proxy = proxy;
    this.Date = DateTime.Now;
    this.Type = type;
    this.ConfigName = configName;
    this.WordlistName = wordlistName;
  }

  public string ToFormattedString(string format)
  {
    StringBuilder stringBuilder1 = new StringBuilder(format).Replace("<DATA>", this.Data).Replace("<PROXY>", this.Proxy);
    DateTime date = this.Date;
    string longDateString = date.ToLongDateString();
    date = this.Date;
    string longTimeString = date.ToLongTimeString();
    string newValue = $"{longDateString} {longTimeString}";
    StringBuilder stringBuilder2 = stringBuilder1.Replace("<DATE>", newValue).Replace("<CONFIG>", this.ConfigName).Replace("<WORDLIST>", this.WordlistName).Replace("<TYPE>", this.Type).Replace("<CAPTURE>", this.CapturedData.ToCaptureString());
    foreach (CVar cvar in this.CapturedData.All)
      stringBuilder2.Replace($"<{cvar.Name}>", cvar.ToString());
    return stringBuilder2.ToString();
  }
}
