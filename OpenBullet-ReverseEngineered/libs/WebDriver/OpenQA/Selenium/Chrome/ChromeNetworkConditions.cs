// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Chrome.ChromeNetworkConditions
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Chrome;

public class ChromeNetworkConditions
{
  private bool offline;
  private TimeSpan latency = TimeSpan.Zero;
  private long downloadThroughput = -1;
  private long uploadThroughput = -1;

  public bool IsOffline
  {
    get => this.offline;
    set => this.offline = value;
  }

  public TimeSpan Latency
  {
    get => this.latency;
    set => this.latency = value;
  }

  public long DownloadThroughput
  {
    get => this.downloadThroughput;
    set => this.downloadThroughput = value;
  }

  public long UploadThroughput
  {
    get => this.uploadThroughput;
    set => this.uploadThroughput = value;
  }

  internal static ChromeNetworkConditions FromDictionary(Dictionary<string, object> dictionary)
  {
    ChromeNetworkConditions networkConditions = new ChromeNetworkConditions();
    if (dictionary.ContainsKey("offline"))
      networkConditions.IsOffline = (bool) dictionary["offline"];
    if (dictionary.ContainsKey("latency"))
      networkConditions.Latency = TimeSpan.FromMilliseconds(Convert.ToDouble(dictionary["latency"]));
    if (dictionary.ContainsKey("upload_throughput"))
      networkConditions.UploadThroughput = (long) dictionary["upload_throughput"];
    if (dictionary.ContainsKey("download_throughput"))
      networkConditions.DownloadThroughput = (long) dictionary["download_throughput"];
    return networkConditions;
  }

  internal Dictionary<string, object> ToDictionary()
  {
    Dictionary<string, object> dictionary = new Dictionary<string, object>();
    dictionary["offline"] = (object) this.offline;
    if (this.latency != TimeSpan.Zero)
      dictionary["latency"] = (object) Convert.ToInt64(this.latency.TotalMilliseconds);
    if (this.downloadThroughput >= 0L)
      dictionary["download_throughput"] = (object) this.downloadThroughput;
    if (this.uploadThroughput >= 0L)
      dictionary["upload_throughput"] = (object) this.uploadThroughput;
    return dictionary;
  }
}
