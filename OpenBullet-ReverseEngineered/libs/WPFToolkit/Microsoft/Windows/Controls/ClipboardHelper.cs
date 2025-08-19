// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.ClipboardHelper
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;

#nullable disable
namespace Microsoft.Windows.Controls;

internal static class ClipboardHelper
{
  private const string DATAGRIDVIEW_htmlPrefix = "Version:1.0\r\nStartHTML:00000097\r\nEndHTML:{0}\r\nStartFragment:00000133\r\nEndFragment:{1}\r\n";
  private const string DATAGRIDVIEW_htmlStartFragment = "<HTML>\r\n<BODY>\r\n<!--StartFragment-->";
  private const string DATAGRIDVIEW_htmlEndFragment = "\r\n<!--EndFragment-->\r\n</BODY>\r\n</HTML>";

  internal static void FormatCell(
    object cellValue,
    bool firstCell,
    bool lastCell,
    StringBuilder sb,
    string format)
  {
    bool csv = string.Equals(format, DataFormats.CommaSeparatedValue, StringComparison.OrdinalIgnoreCase);
    if (csv || string.Equals(format, DataFormats.Text, StringComparison.OrdinalIgnoreCase) || string.Equals(format, DataFormats.UnicodeText, StringComparison.OrdinalIgnoreCase))
    {
      if (cellValue != null)
      {
        bool escapeApplied = false;
        int length = sb.Length;
        ClipboardHelper.FormatPlainText(cellValue.ToString(), csv, (TextWriter) new StringWriter(sb, (IFormatProvider) CultureInfo.CurrentCulture), ref escapeApplied);
        if (escapeApplied)
          sb.Insert(length, '"');
      }
      if (lastCell)
      {
        sb.Append('\r');
        sb.Append('\n');
      }
      else
        sb.Append(csv ? ',' : '\t');
    }
    else
    {
      if (!string.Equals(format, DataFormats.Html, StringComparison.OrdinalIgnoreCase))
        return;
      if (firstCell)
        sb.Append("<TR>");
      sb.Append("<TD>");
      if (cellValue != null)
        ClipboardHelper.FormatPlainTextAsHtml(cellValue.ToString(), (TextWriter) new StringWriter(sb, (IFormatProvider) CultureInfo.CurrentCulture));
      else
        sb.Append("&nbsp;");
      sb.Append("</TD>");
      if (!lastCell)
        return;
      sb.Append("</TR>");
    }
  }

  internal static void GetClipboardContentForHtml(StringBuilder content)
  {
    content.Insert(0, "<TABLE>");
    content.Append("</TABLE>");
    int num = 135 + content.Length;
    string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Version:1.0\r\nStartHTML:00000097\r\nEndHTML:{0}\r\nStartFragment:00000133\r\nEndFragment:{1}\r\n", (object) (num + 36).ToString("00000000", (IFormatProvider) CultureInfo.InvariantCulture), (object) num.ToString("00000000", (IFormatProvider) CultureInfo.InvariantCulture)) + "<HTML>\r\n<BODY>\r\n<!--StartFragment-->";
    content.Insert(0, str);
    content.Append("\r\n<!--EndFragment-->\r\n</BODY>\r\n</HTML>");
  }

  private static void FormatPlainText(
    string s,
    bool csv,
    TextWriter output,
    ref bool escapeApplied)
  {
    if (s == null)
      return;
    int length = s.Length;
    for (int index = 0; index < length; ++index)
    {
      char ch = s[index];
      switch (ch)
      {
        case '\t':
          if (!csv)
          {
            output.Write(' ');
            break;
          }
          output.Write('\t');
          break;
        case '"':
          if (csv)
          {
            output.Write("\"\"");
            escapeApplied = true;
            break;
          }
          output.Write('"');
          break;
        case ',':
          if (csv)
            escapeApplied = true;
          output.Write(',');
          break;
        default:
          output.Write(ch);
          break;
      }
    }
    if (!escapeApplied)
      return;
    output.Write('"');
  }

  private static void FormatPlainTextAsHtml(string s, TextWriter output)
  {
    if (s == null)
      return;
    int length = s.Length;
    char ch1 = char.MinValue;
    for (int index = 0; index < length; ++index)
    {
      char ch2 = s[index];
      switch (ch2)
      {
        case '\n':
          output.Write("<br>");
          goto case '\r';
        case '\r':
          ch1 = ch2;
          continue;
        case ' ':
          if (ch1 == ' ')
          {
            output.Write("&nbsp;");
            goto case '\r';
          }
          output.Write(ch2);
          goto case '\r';
        case '"':
          output.Write("&quot;");
          goto case '\r';
        case '&':
          output.Write("&amp;");
          goto case '\r';
        case '<':
          output.Write("&lt;");
          goto case '\r';
        case '>':
          output.Write("&gt;");
          goto case '\r';
        default:
          if (ch2 >= ' ' && ch2 < 'Ā')
          {
            output.Write("&#");
            output.Write(((int) ch2).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
            output.Write(';');
            goto case '\r';
          }
          output.Write(ch2);
          goto case '\r';
      }
    }
  }
}
