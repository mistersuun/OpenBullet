// Decompiled with JetBrains decompiler
// Type: RuriLib.Models.EnvironmentSettings
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace RuriLib.Models;

public class EnvironmentSettings
{
  public List<WordlistType> WordlistTypes { get; set; } = new List<WordlistType>();

  public List<CustomKeychain> CustomKeychains { get; set; } = new List<CustomKeychain>();

  public List<ExportFormat> ExportFormats { get; set; } = new List<ExportFormat>();

  public List<string> GetWordlistTypeNames()
  {
    return this.WordlistTypes.Select<WordlistType, string>((Func<WordlistType, string>) (w => w.Name)).ToList<string>();
  }

  public string RecognizeWordlistType(string data)
  {
    foreach (WordlistType wordlistType in this.WordlistTypes)
    {
      if (Regex.Match(data, wordlistType.Regex).Success)
        return wordlistType.Name;
    }
    return this.WordlistTypes.First<WordlistType>().Name;
  }

  public List<string> GetCustomKeychainNames()
  {
    return this.CustomKeychains.Select<CustomKeychain, string>((Func<CustomKeychain, string>) (c => c.Name)).ToList<string>();
  }

  public CustomKeychain GetCustomKeychain(string name)
  {
    try
    {
      return this.CustomKeychains.First<CustomKeychain>((Func<CustomKeychain, bool>) (w => w.Name == name));
    }
    catch
    {
      return new CustomKeychain();
    }
  }

  public WordlistType GetWordlistType(string name)
  {
    try
    {
      return this.WordlistTypes.FirstOrDefault<WordlistType>((Func<WordlistType, bool>) (w => w.Name == name));
    }
    catch
    {
      return new WordlistType();
    }
  }
}
