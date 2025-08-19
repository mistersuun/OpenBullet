// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonMergeSettings
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using System;

#nullable disable
namespace Newtonsoft.Json.Linq;

public class JsonMergeSettings
{
  private MergeArrayHandling _mergeArrayHandling;
  private MergeNullValueHandling _mergeNullValueHandling;
  private StringComparison _propertyNameComparison;

  public JsonMergeSettings() => this._propertyNameComparison = StringComparison.Ordinal;

  public MergeArrayHandling MergeArrayHandling
  {
    get => this._mergeArrayHandling;
    set
    {
      this._mergeArrayHandling = value >= MergeArrayHandling.Concat && value <= MergeArrayHandling.Merge ? value : throw new ArgumentOutOfRangeException(nameof (value));
    }
  }

  public MergeNullValueHandling MergeNullValueHandling
  {
    get => this._mergeNullValueHandling;
    set
    {
      this._mergeNullValueHandling = value >= MergeNullValueHandling.Ignore && value <= MergeNullValueHandling.Merge ? value : throw new ArgumentOutOfRangeException(nameof (value));
    }
  }

  public StringComparison PropertyNameComparison
  {
    get => this._propertyNameComparison;
    set
    {
      this._propertyNameComparison = value >= StringComparison.CurrentCulture && value <= StringComparison.OrdinalIgnoreCase ? value : throw new ArgumentOutOfRangeException(nameof (value));
    }
  }
}
