// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.XamlFormatter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class XamlFormatter : ITextFormatter
{
  public string GetText(FlowDocument document)
  {
    using (MemoryStream memoryStream = new MemoryStream())
    {
      new TextRange(document.ContentStart, document.ContentEnd).Save((Stream) memoryStream, DataFormats.Xaml);
      return Encoding.Default.GetString(memoryStream.ToArray());
    }
  }

  public void SetText(FlowDocument document, string text)
  {
    try
    {
      if (string.IsNullOrEmpty(text))
      {
        document.Blocks.Clear();
      }
      else
      {
        using (MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(text)))
          new TextRange(document.ContentStart, document.ContentEnd).Load((Stream) memoryStream, DataFormats.Xaml);
      }
    }
    catch
    {
      throw new InvalidDataException("Data provided is not in the correct Xaml format.");
    }
  }
}
