// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Configuration.LanguageElement
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;
using System.Configuration;

#nullable disable
namespace Microsoft.Scripting.Hosting.Configuration;

public class LanguageElement : ConfigurationElement
{
  private const string _Names = "names";
  private const string _Extensions = "extensions";
  private const string _Type = "type";
  private const string _DisplayName = "displayName";
  private static ConfigurationPropertyCollection _Properties = new ConfigurationPropertyCollection()
  {
    new ConfigurationProperty("names", typeof (string), (object) null),
    new ConfigurationProperty("extensions", typeof (string), (object) null),
    new ConfigurationProperty("type", typeof (string), (object) null, ConfigurationPropertyOptions.IsRequired),
    new ConfigurationProperty("displayName", typeof (string), (object) null)
  };

  protected override ConfigurationPropertyCollection Properties => LanguageElement._Properties;

  public string Names
  {
    get => (string) this["names"];
    set => this["names"] = (object) value;
  }

  public string Extensions
  {
    get => (string) this["extensions"];
    set => this["extensions"] = (object) value;
  }

  public string Type
  {
    get => (string) this["type"];
    set => this["type"] = (object) value;
  }

  public string DisplayName
  {
    get => (string) this["displayName"];
    set => this["displayName"] = (object) value;
  }

  public string[] GetNamesArray() => LanguageElement.Split(this.Names);

  public string[] GetExtensionsArray() => LanguageElement.Split(this.Extensions);

  private static string[] Split(string str)
  {
    if (str == null)
      return ArrayUtils.EmptyStrings;
    return str.Split(new char[2]{ ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
  }
}
