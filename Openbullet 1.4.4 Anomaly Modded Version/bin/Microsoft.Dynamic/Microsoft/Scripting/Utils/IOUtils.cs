// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.IOUtils
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.IO;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Utils;

public static class IOUtils
{
  public static bool SeekLine(TextReader reader, int line)
  {
    ContractUtils.RequiresNotNull((object) reader, nameof (reader));
    if (line < 1)
      throw new ArgumentOutOfRangeException(nameof (line));
    if (line == 1)
      return true;
    int num1 = 1;
    int num2;
    do
    {
      num2 = reader.Read();
      if (num2 == 13)
      {
        if (reader.Peek() == 10)
          reader.Read();
        ++num1;
        if (num1 == line)
          return true;
      }
      else if (num2 == 10)
      {
        ++num1;
        if (num1 == line)
          return true;
      }
    }
    while (num2 != -1);
    return false;
  }

  public static string ReadTo(TextReader reader, char terminator)
  {
    ContractUtils.RequiresNotNull((object) reader, nameof (reader));
    StringBuilder stringBuilder = new StringBuilder();
    while (true)
    {
      int num = reader.Read();
      if (num != -1)
      {
        if (num != (int) terminator)
          stringBuilder.Append((char) num);
        else
          break;
      }
      else
        goto label_5;
    }
    return stringBuilder.ToString();
label_5:
    return stringBuilder.Length <= 0 ? (string) null : stringBuilder.ToString();
  }

  public static bool SeekTo(TextReader reader, char c)
  {
    ContractUtils.RequiresNotNull((object) reader, nameof (reader));
    int num;
    do
    {
      num = reader.Read();
      if (num == -1)
        return false;
    }
    while (num != (int) c);
    return true;
  }

  public static string ToValidPath(string path) => IOUtils.ToValidPath(path, false, true);

  public static string ToValidPath(string path, bool isMask)
  {
    return IOUtils.ToValidPath(path, isMask, true);
  }

  public static string ToValidFileName(string path) => IOUtils.ToValidPath(path, false, false);

  private static string ToValidPath(string path, bool isMask, bool isPath)
  {
    if (string.IsNullOrEmpty(path))
      return "_";
    StringBuilder stringBuilder = new StringBuilder(path);
    if (isPath)
    {
      foreach (char invalidPathChar in Path.GetInvalidPathChars())
        stringBuilder.Replace(invalidPathChar, '_');
    }
    else
    {
      foreach (char invalidFileNameChar in Path.GetInvalidFileNameChars())
        stringBuilder.Replace(invalidFileNameChar, '_');
    }
    if (!isMask)
      stringBuilder.Replace('*', '_').Replace('?', '_');
    return stringBuilder.ToString();
  }
}
