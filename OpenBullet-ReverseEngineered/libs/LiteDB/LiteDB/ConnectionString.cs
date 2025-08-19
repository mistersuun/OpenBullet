// Decompiled with JetBrains decompiler
// Type: LiteDB.ConnectionString
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace LiteDB;

public class ConnectionString
{
  public string Filename { get; set; } = "";

  public bool Journal { get; set; } = true;

  public string Password { get; set; }

  public int CacheSize { get; set; } = 5000;

  public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(1.0);

  public FileMode Mode { get; set; }

  public long InitialSize { get; set; }

  public long LimitSize { get; set; } = long.MaxValue;

  public byte Log { get; set; }

  public bool UtcDate { get; set; }

  public bool Upgrade { get; set; }

  public bool Flush { get; set; }

  public ConnectionString()
  {
  }

  public ConnectionString(string connectionString)
  {
    if (string.IsNullOrEmpty(connectionString))
      throw new ArgumentNullException(nameof (connectionString));
    Dictionary<string, string> dict = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    if (connectionString.Contains("="))
      dict.ParseKeyValue(connectionString);
    else
      dict["filename"] = connectionString;
    this.Filename = dict.GetValue<string>("filename", this.Filename);
    this.Journal = dict.GetValue<bool>("journal", this.Journal);
    this.Password = dict.GetValue<string>("password", this.Password);
    this.CacheSize = dict.GetValue<int>("cache size", this.CacheSize);
    this.Timeout = dict.GetValue<TimeSpan>("timeout", this.Timeout);
    this.Mode = dict.GetValue<FileMode>("mode", this.Mode);
    this.InitialSize = dict.GetFileSize("initial size", this.InitialSize);
    this.LimitSize = dict.GetFileSize("limit size", this.LimitSize);
    this.Log = dict.GetValue<byte>("log", this.Log);
    this.UtcDate = dict.GetValue<bool>("utc", this.UtcDate);
    this.Upgrade = dict.GetValue<bool>("upgrade", this.Upgrade);
    this.Flush = dict.GetValue<bool>("flush", this.Flush);
  }
}
