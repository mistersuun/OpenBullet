// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Chrome.ChromePerformanceLoggingPreferences
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Chrome;

public class ChromePerformanceLoggingPreferences
{
  private bool isCollectingNetworkEvents = true;
  private bool isCollectingPageEvents = true;
  private TimeSpan bufferUsageReportingInterval = TimeSpan.FromMilliseconds(1000.0);
  private List<string> tracingCategories = new List<string>();

  public bool IsCollectingNetworkEvents
  {
    get => this.isCollectingNetworkEvents;
    set => this.isCollectingNetworkEvents = value;
  }

  public bool IsCollectingPageEvents
  {
    get => this.isCollectingPageEvents;
    set => this.isCollectingPageEvents = value;
  }

  public TimeSpan BufferUsageReportingInterval
  {
    get => this.bufferUsageReportingInterval;
    set
    {
      this.bufferUsageReportingInterval = value.TotalMilliseconds > 0.0 ? value : throw new ArgumentException("Interval must be greater than zero.");
    }
  }

  public string TracingCategories
  {
    get
    {
      return this.tracingCategories.Count == 0 ? string.Empty : string.Join(",", this.tracingCategories.ToArray());
    }
  }

  public void AddTracingCategory(string category)
  {
    if (string.IsNullOrEmpty(category))
      throw new ArgumentException("category must not be null or empty", nameof (category));
    this.AddTracingCategories(category);
  }

  public void AddTracingCategories(params string[] categoriesToAdd)
  {
    this.AddTracingCategories((IEnumerable<string>) new List<string>((IEnumerable<string>) categoriesToAdd));
  }

  public void AddTracingCategories(IEnumerable<string> categoriesToAdd)
  {
    if (categoriesToAdd == null)
      throw new ArgumentNullException(nameof (categoriesToAdd), "categoriesToAdd must not be null");
    this.tracingCategories.AddRange(categoriesToAdd);
  }
}
