// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.FormUrlEncodedContent
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace Leaf.xNet;

public class FormUrlEncodedContent : BytesContent
{
  public FormUrlEncodedContent(
    IEnumerable<KeyValuePair<string, string>> content,
    bool valuesUnescaped = false,
    bool keysUnescaped = false)
  {
    if (content == null)
      throw new ArgumentNullException(nameof (content));
    this.Init(Http.ToQueryString(content, valuesUnescaped, keysUnescaped));
  }

  public FormUrlEncodedContent(RequestParams rp)
  {
    if (rp == null)
      throw new ArgumentNullException(nameof (rp));
    this.Init(rp.Query);
  }

  private void Init(string content)
  {
    this.Content = Encoding.ASCII.GetBytes(content);
    this.Offset = 0;
    this.Count = this.Content.Length;
    this.MimeContentType = "application/x-www-form-urlencoded";
  }
}
