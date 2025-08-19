// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonLoadSettings
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;

#nullable disable
namespace Newtonsoft.Json.Linq;

internal class JsonLoadSettings
{
  private CommentHandling _commentHandling;
  private LineInfoHandling _lineInfoHandling;

  public JsonLoadSettings()
  {
    this._lineInfoHandling = LineInfoHandling.Load;
    this._commentHandling = CommentHandling.Ignore;
  }

  public CommentHandling CommentHandling
  {
    get => this._commentHandling;
    set
    {
      this._commentHandling = value >= CommentHandling.Ignore && value <= CommentHandling.Load ? value : throw new ArgumentOutOfRangeException(nameof (value));
    }
  }

  public LineInfoHandling LineInfoHandling
  {
    get => this._lineInfoHandling;
    set
    {
      this._lineInfoHandling = value >= LineInfoHandling.Ignore && value <= LineInfoHandling.Load ? value : throw new ArgumentOutOfRangeException(nameof (value));
    }
  }
}
