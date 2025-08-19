// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.StreamReaderExtensions
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Collections.Generic;
using System.IO;

#nullable disable
namespace IronPython.Modules;

internal static class StreamReaderExtensions
{
  public static IEnumerable<string> ReadLines(this StreamReader reader)
  {
    string str;
    while ((str = reader.ReadLine()) != null)
      yield return str;
  }
}
