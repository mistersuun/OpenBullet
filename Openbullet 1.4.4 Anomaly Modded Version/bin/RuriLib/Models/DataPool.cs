// Decompiled with JetBrains decompiler
// Type: RuriLib.Models.DataPool
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using RuriLib.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace RuriLib.Models;

public class DataPool : ViewModelBase
{
  public IEnumerable<string> List { get; }

  public int Size { get; set; }

  public DataPool(IEnumerable<string> list)
  {
    this.List = list;
    this.Size = this.List.Count<string>();
  }

  public DataPool(string fileName)
  {
    this.List = File.ReadLines(fileName);
    this.Size = this.List.Count<string>();
  }

  public DataPool(string charSet, int length)
  {
    this.List = charSet.Select<char, string>((Func<char, string>) (x => x.ToString()));
    for (int index = 0; index < length; ++index)
      this.List = this.List.SelectMany<string, char, string>((Func<string, IEnumerable<char>>) (x => (IEnumerable<char>) charSet), (Func<string, char, string>) ((x, y) => x + y.ToString()));
    this.Size = (int) Math.Pow((double) charSet.Length, (double) length);
  }
}
