// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Forms.FormDataSetExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Io;
using AngleSharp.Text;
using System.IO;
using System.Text;

#nullable disable
namespace AngleSharp.Html.Forms;

internal static class FormDataSetExtensions
{
  public static Stream CreateBody(this FormDataSet formDataSet, string enctype, string charset)
  {
    Encoding encoding = TextEncoding.Resolve(charset);
    return formDataSet.CreateBody(enctype, encoding);
  }

  public static Stream CreateBody(this FormDataSet formDataSet, string enctype, Encoding encoding)
  {
    if (enctype.Isi(MimeTypeNames.UrlencodedForm))
      return formDataSet.AsUrlEncoded(encoding);
    if (enctype.Isi(MimeTypeNames.MultipartForm))
      return formDataSet.AsMultipart(encoding);
    if (enctype.Isi(MimeTypeNames.Plain))
      return formDataSet.AsPlaintext(encoding);
    return enctype.Isi(MimeTypeNames.ApplicationJson) ? formDataSet.AsJson() : Stream.Null;
  }
}
