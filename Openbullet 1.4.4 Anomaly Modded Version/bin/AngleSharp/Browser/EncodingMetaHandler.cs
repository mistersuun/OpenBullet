// Decompiled with JetBrains decompiler
// Type: AngleSharp.Browser.EncodingMetaHandler
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom;
using AngleSharp.Io;
using AngleSharp.Text;
using System.Text;

#nullable disable
namespace AngleSharp.Browser;

public class EncodingMetaHandler : IMetaHandler
{
  public EncodingMetaHandler() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

  void IMetaHandler.HandleContent(IHtmlMetaElement element)
  {
    Encoding encoding = this.GetEncoding(element);
    if (encoding == null)
      return;
    element.Owner.Source.CurrentEncoding = encoding;
  }

  protected virtual Encoding GetEncoding(IHtmlMetaElement element)
  {
    string charset1 = element.Charset;
    if (charset1 != null)
    {
      string charset2 = charset1.Trim();
      if (TextEncoding.IsSupported(charset2))
        return TextEncoding.Resolve(charset2);
    }
    return !element.HttpEquivalent.Isi(HeaderNames.ContentType) ? (Encoding) null : TextEncoding.Parse(element.Content ?? string.Empty);
  }
}
