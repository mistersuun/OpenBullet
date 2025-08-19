// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Configuration.Section
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;

#nullable disable
namespace Microsoft.Scripting.Hosting.Configuration;

public class Section : ConfigurationSection
{
  public static readonly string SectionName = "microsoft.scripting";
  private const string _DebugMode = "debugMode";
  private const string _PrivateBinding = "privateBinding";
  private const string _Languages = "languages";
  private const string _Options = "options";
  private static ConfigurationPropertyCollection _Properties = new ConfigurationPropertyCollection()
  {
    new ConfigurationProperty("debugMode", typeof (bool?), (object) null),
    new ConfigurationProperty("privateBinding", typeof (bool?), (object) null),
    new ConfigurationProperty("languages", typeof (LanguageElementCollection), (object) null, ConfigurationPropertyOptions.IsDefaultCollection),
    new ConfigurationProperty("options", typeof (OptionElementCollection), (object) null)
  };

  protected override ConfigurationPropertyCollection Properties => Section._Properties;

  public bool? DebugMode
  {
    get => (bool?) this["debugMode"];
    set => this["debugMode"] = (object) value;
  }

  public bool? PrivateBinding
  {
    get => (bool?) this["privateBinding"];
    set => this["privateBinding"] = (object) value;
  }

  public IEnumerable<LanguageElement> GetLanguages()
  {
    if (this["languages"] is LanguageElementCollection elementCollection)
    {
      foreach (LanguageElement language in (ConfigurationElementCollection) elementCollection)
        yield return language;
    }
  }

  public IEnumerable<OptionElement> GetOptions()
  {
    if (this["options"] is OptionElementCollection elementCollection)
    {
      foreach (OptionElement option in (ConfigurationElementCollection) elementCollection)
        yield return option;
    }
  }

  private static Section LoadFromFile(Stream configFileStream)
  {
    Section section = new Section();
    using (XmlReader reader = XmlReader.Create(configFileStream))
    {
      if (!reader.ReadToDescendant("configuration") || !reader.ReadToDescendant(Section.SectionName))
        return (Section) null;
      section.DeserializeElement(reader, false);
    }
    return section;
  }

  internal static void LoadRuntimeSetup(ScriptRuntimeSetup setup, Stream configFileStream)
  {
    Section section = configFileStream == null ? ConfigurationManager.GetSection(Section.SectionName) as Section : Section.LoadFromFile(configFileStream);
    if (section == null)
      return;
    bool? nullable;
    if (section.DebugMode.HasValue)
    {
      ScriptRuntimeSetup scriptRuntimeSetup = setup;
      nullable = section.DebugMode;
      int num = nullable.Value ? 1 : 0;
      scriptRuntimeSetup.DebugMode = num != 0;
    }
    nullable = section.PrivateBinding;
    if (nullable.HasValue)
    {
      ScriptRuntimeSetup scriptRuntimeSetup = setup;
      nullable = section.PrivateBinding;
      int num = nullable.Value ? 1 : 0;
      scriptRuntimeSetup.PrivateBinding = num != 0;
    }
    foreach (LanguageElement language in section.GetLanguages())
    {
      string type = language.Type;
      string[] namesArray = language.GetNamesArray();
      string[] extensionsArray = language.GetExtensionsArray();
      string displayName = language.DisplayName ?? (namesArray.Length != 0 ? namesArray[0] : language.Type);
      bool flag = false;
      foreach (LanguageSetup languageSetup in (IEnumerable<LanguageSetup>) setup.LanguageSetups)
      {
        if (languageSetup.TypeName == type)
        {
          languageSetup.Names.Clear();
          foreach (string str in namesArray)
            languageSetup.Names.Add(str);
          languageSetup.FileExtensions.Clear();
          foreach (string str in extensionsArray)
            languageSetup.FileExtensions.Add(str);
          languageSetup.DisplayName = displayName;
          flag = true;
          break;
        }
      }
      if (!flag)
        setup.LanguageSetups.Add(new LanguageSetup(type, displayName, (IEnumerable<string>) namesArray, (IEnumerable<string>) extensionsArray));
    }
    foreach (OptionElement option1 in section.GetOptions())
    {
      OptionElement option = option1;
      if (string.IsNullOrEmpty(option.Language))
      {
        setup.Options[option.Name] = (object) option.Value;
      }
      else
      {
        bool flag = false;
        foreach (LanguageSetup languageSetup in (IEnumerable<LanguageSetup>) setup.LanguageSetups)
        {
          if (languageSetup.Names.Any<string>((Func<string, bool>) (s => DlrConfiguration.LanguageNameComparer.Equals(s, option.Language))))
          {
            languageSetup.Options[option.Name] = (object) option.Value;
            flag = true;
            break;
          }
        }
        if (!flag)
          throw new ConfigurationErrorsException($"Unknown language name: '{option.Language}'");
      }
    }
  }
}
