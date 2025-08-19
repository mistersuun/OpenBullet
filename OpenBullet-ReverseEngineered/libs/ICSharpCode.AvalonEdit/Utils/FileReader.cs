// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.FileReader
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.IO;
using System.Text;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

public static class FileReader
{
  private static readonly Encoding UTF8NoBOM = (Encoding) new UTF8Encoding(false);

  public static bool IsUnicode(Encoding encoding)
  {
    if (encoding == null)
      throw new ArgumentNullException(nameof (encoding));
    switch (encoding.CodePage)
    {
      case 1200:
      case 1201:
      case 12000:
      case 12001:
      case 65000:
      case 65001:
        return true;
      default:
        return false;
    }
  }

  private static bool IsASCIICompatible(Encoding encoding)
  {
    byte[] bytes = encoding.GetBytes("Az");
    return bytes.Length == 2 && bytes[0] == (byte) 65 && bytes[1] == (byte) 122;
  }

  private static Encoding RemoveBOM(Encoding encoding)
  {
    return encoding.CodePage == 65001 ? FileReader.UTF8NoBOM : encoding;
  }

  public static string ReadFileContent(Stream stream, Encoding defaultEncoding)
  {
    using (StreamReader streamReader = FileReader.OpenStream(stream, defaultEncoding))
      return streamReader.ReadToEnd();
  }

  public static string ReadFileContent(string fileName, Encoding defaultEncoding)
  {
    using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
      return FileReader.ReadFileContent((Stream) fileStream, defaultEncoding);
  }

  public static StreamReader OpenFile(string fileName, Encoding defaultEncoding)
  {
    FileStream fileStream = fileName != null ? new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read) : throw new ArgumentNullException(nameof (fileName));
    try
    {
      return FileReader.OpenStream((Stream) fileStream, defaultEncoding);
    }
    catch
    {
      fileStream.Dispose();
      throw;
    }
  }

  public static StreamReader OpenStream(Stream stream, Encoding defaultEncoding)
  {
    if (stream == null)
      throw new ArgumentNullException(nameof (stream));
    if (stream.Position != 0L)
      throw new ArgumentException("stream is not positioned at beginning.", nameof (stream));
    if (defaultEncoding == null)
      throw new ArgumentNullException(nameof (defaultEncoding));
    if (stream.Length >= 2L)
    {
      int firstByte = stream.ReadByte();
      int secondByte = stream.ReadByte();
      switch (firstByte << 8 | secondByte)
      {
        case 0:
        case 61371:
        case 65279:
        case 65534:
          stream.Position = 0L;
          return new StreamReader(stream);
        default:
          return FileReader.AutoDetect(stream, (byte) firstByte, (byte) secondByte, defaultEncoding);
      }
    }
    else
      return defaultEncoding != null ? new StreamReader(stream, defaultEncoding) : new StreamReader(stream);
  }

  private static StreamReader AutoDetect(
    Stream fs,
    byte firstByte,
    byte secondByte,
    Encoding defaultEncoding)
  {
    int num1 = (int) Math.Min(fs.Length, 500000L);
    int num2 = 0;
    int num3 = 0;
    for (int index = 0; index < num1; ++index)
    {
      byte num4;
      switch (index)
      {
        case 0:
          num4 = firstByte;
          break;
        case 1:
          num4 = secondByte;
          break;
        default:
          num4 = (byte) fs.ReadByte();
          break;
      }
      if (num4 < (byte) 128 /*0x80*/)
      {
        if (num2 == 3)
        {
          num2 = 1;
          break;
        }
      }
      else if (num4 < (byte) 192 /*0xC0*/)
      {
        if (num2 == 3)
        {
          --num3;
          if (num3 < 0)
          {
            num2 = 1;
            break;
          }
          if (num3 == 0)
            num2 = 2;
        }
        else
        {
          num2 = 1;
          break;
        }
      }
      else if (num4 >= (byte) 194 && num4 < (byte) 245)
      {
        if (num2 == 2 || num2 == 0)
        {
          num2 = 3;
          num3 = num4 >= (byte) 224 /*0xE0*/ ? (num4 >= (byte) 240 /*0xF0*/ ? 3 : 2) : 1;
        }
        else
        {
          num2 = 1;
          break;
        }
      }
      else
      {
        num2 = 1;
        break;
      }
    }
    fs.Position = 0L;
    switch (num2)
    {
      case 0:
        return new StreamReader(fs, FileReader.IsASCIICompatible(defaultEncoding) ? FileReader.RemoveBOM(defaultEncoding) : Encoding.ASCII);
      case 1:
        if (FileReader.IsUnicode(defaultEncoding))
          defaultEncoding = Encoding.Default;
        return new StreamReader(fs, FileReader.RemoveBOM(defaultEncoding));
      default:
        return new StreamReader(fs, FileReader.UTF8NoBOM);
    }
  }
}
