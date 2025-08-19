// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonMergeSettings
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;

#nullable disable
namespace Newtonsoft.Json.Linq;

internal class JsonMergeSettings
{
  private MergeArrayHandling _mergeArrayHandling;
  private MergeNullValueHandling _mergeNullValueHandling;

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
}
