// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.StringContent
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.Text;

#nullable disable
namespace Leaf.xNet;

public class StringContent : BytesContent
{
  public StringContent(string content)
    : this(content, Encoding.UTF8)
  {
  }

  public StringContent(string content, Encoding encoding)
  {
    if (content == null)
      throw new ArgumentNullException(nameof (content));
    this.Content = encoding?.GetBytes(content) ?? throw new ArgumentNullException(nameof (encoding));
    this.Offset = 0;
    this.Count = this.Content.Length;
    this.MimeContentType = "text/plain";
  }
}
