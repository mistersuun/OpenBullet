// Decompiled with JetBrains decompiler
// Type: Extreme.Net.FormUrlEncodedContent
// Assembly: Extreme.Net, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B85A5720-FE8B-4A1B-9FD2-F7651D37B15B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Extreme.Net.dll

using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace Extreme.Net;

public class FormUrlEncodedContent : BytesContent
{
  public FormUrlEncodedContent(
    IEnumerable<KeyValuePair<string, string>> content,
    bool dontEscape = false,
    Encoding encoding = null)
  {
    if (content == null)
      throw new ArgumentNullException(nameof (content));
    this._content = Encoding.ASCII.GetBytes(Http.ToPostQueryString(content, dontEscape, encoding));
    this._offset = 0;
    this._count = this._content.Length;
    this._contentType = "application/x-www-form-urlencoded";
  }
}
