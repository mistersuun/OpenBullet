// Decompiled with JetBrains decompiler
// Type: RuriLib.Models.Wordlist
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;
using System.IO;
using System.Linq;

#nullable disable
namespace RuriLib.Models;

public class Wordlist
{
  public Guid Id { get; set; }

  public string Name { get; set; }

  public string Path { get; set; }

  public string Type { get; set; }

  public string Purpose { get; set; }

  public int Total { get; set; }

  public bool Temporary { get; set; }

  public Wordlist()
  {
  }

  public Wordlist(
    string name,
    string path,
    string type,
    string purpose,
    bool countLines = true,
    bool temporary = false)
  {
    this.Name = name;
    this.Path = path;
    this.Type = type;
    this.Purpose = purpose;
    this.Total = 0;
    this.Temporary = temporary;
    if (!countLines)
      return;
    try
    {
      this.Total = File.ReadLines(path).Count<string>();
    }
    catch
    {
    }
  }
}
