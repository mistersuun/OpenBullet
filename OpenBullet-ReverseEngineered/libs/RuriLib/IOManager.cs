// Decompiled with JetBrains decompiler
// Type: RuriLib.IOManager
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using RuriLib.Models;
using RuriLib.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace RuriLib;

public static class IOManager
{
  private static JsonSerializerSettings settings = new JsonSerializerSettings()
  {
    TypeNameHandling = TypeNameHandling.All
  };

  public static void SaveSettings(string settingsFile, RLSettingsViewModel settings)
  {
    File.WriteAllText(settingsFile, JsonConvert.SerializeObject((object) settings, Formatting.Indented));
  }

  public static RLSettingsViewModel LoadSettings(string settingsFile)
  {
    return JsonConvert.DeserializeObject<RLSettingsViewModel>(File.ReadAllText(settingsFile));
  }

  public static string SerializeBlock(BlockBase block)
  {
    return JsonConvert.SerializeObject((object) block, Formatting.None, IOManager.settings);
  }

  public static BlockBase DeserializeBlock(string block)
  {
    return JsonConvert.DeserializeObject<BlockBase>(block, IOManager.settings);
  }

  public static string SerializeBlocks(List<BlockBase> blocks)
  {
    return JsonConvert.SerializeObject((object) blocks, Formatting.None, IOManager.settings);
  }

  public static List<BlockBase> DeserializeBlocks(string blocks)
  {
    return JsonConvert.DeserializeObject<List<BlockBase>>(blocks, IOManager.settings);
  }

  public static string SerializeConfig(Config config)
  {
    StringWriter stringWriter = new StringWriter();
    stringWriter.WriteLine("[SETTINGS]");
    stringWriter.WriteLine(JsonConvert.SerializeObject((object) config.Settings, Formatting.Indented));
    stringWriter.WriteLine("");
    stringWriter.WriteLine("[SCRIPT]");
    stringWriter.Write(config.Script);
    return stringWriter.ToString();
  }

  public static Config DeserializeConfig(string config)
  {
    string[] strArray = config.Split(new string[2]
    {
      "[SETTINGS]",
      "[SCRIPT]"
    }, StringSplitOptions.RemoveEmptyEntries);
    return new Config(JsonConvert.DeserializeObject<ConfigSettings>(strArray[0]), strArray[1].TrimStart('\r', '\n'));
  }

  public static string SerializeProxies(List<CProxy> proxies)
  {
    return JsonConvert.SerializeObject((object) proxies, Formatting.None);
  }

  public static List<CProxy> DeserializeProxies(string proxies)
  {
    return JsonConvert.DeserializeObject<List<CProxy>>(proxies);
  }

  public static Config LoadConfig(string fileName)
  {
    return IOManager.DeserializeConfig(File.ReadAllText(fileName));
  }

  public static bool SaveConfig(Config config, string fileName)
  {
    try
    {
      File.WriteAllText(fileName, IOManager.SerializeConfig(config));
      return true;
    }
    catch
    {
      return false;
    }
  }

  public static Config CloneConfig(Config config)
  {
    return IOManager.DeserializeConfig(IOManager.SerializeConfig(config));
  }

  public static BlockBase CloneBlock(BlockBase block)
  {
    return IOManager.DeserializeBlock(IOManager.SerializeBlock(block));
  }

  public static List<CProxy> CloneProxies(List<CProxy> proxies)
  {
    return IOManager.DeserializeProxies(IOManager.SerializeProxies(proxies));
  }

  public static EnvironmentSettings ParseEnvironmentSettings(string envFile)
  {
    EnvironmentSettings environmentSettings = new EnvironmentSettings();
    string[] array = ((IEnumerable<string>) File.ReadAllLines(envFile)).Where<string>((Func<string, bool>) (l => !string.IsNullOrEmpty(l))).ToArray<string>();
    for (int index1 = 0; index1 < ((IEnumerable<string>) array).Count<string>(); ++index1)
    {
      string str1 = array[index1];
      if (!str1.StartsWith("#") && str1.StartsWith("["))
      {
        string str2 = str1;
        Type type;
        switch (str1.Trim())
        {
          case "[WLTYPE]":
            type = typeof (WordlistType);
            break;
          case "[CUSTOMKC]":
            type = typeof (CustomKeychain);
            break;
          case "[EXPFORMAT]":
            type = typeof (ExportFormat);
            break;
          default:
            throw new Exception("Unrecognized ini header");
        }
        List<string> parameters = new List<string>();
        int index2;
        for (index2 = index1 + 1; index2 < ((IEnumerable<string>) array).Count<string>(); ++index2)
        {
          string str3 = array[index2];
          if (!str3.StartsWith("["))
          {
            if (!(str3.Trim() == "") && !str3.StartsWith("#"))
              parameters.Add(str3);
          }
          else
            break;
        }
        switch (str2)
        {
          case "[WLTYPE]":
            environmentSettings.WordlistTypes.Add((WordlistType) IOManager.ParseObjectFromIni(type, parameters));
            break;
          case "[CUSTOMKC]":
            environmentSettings.CustomKeychains.Add((CustomKeychain) IOManager.ParseObjectFromIni(type, parameters));
            break;
          case "[EXPFORMAT]":
            environmentSettings.ExportFormats.Add((ExportFormat) IOManager.ParseObjectFromIni(type, parameters));
            break;
        }
        index1 = index2 - 1;
      }
    }
    return environmentSettings;
  }

  private static object ParseObjectFromIni(Type type, List<string> parameters)
  {
    object instance = Activator.CreateInstance(type);
    foreach (string[] strArray in parameters.Where<string>((Func<string, bool>) (p => !string.IsNullOrEmpty(p))).Select<string, string[]>((Func<string, string[]>) (p => p.Split(new char[1]
    {
      '='
    }, 2))))
    {
      string[] pair = strArray;
      PropertyInfo property = type.GetProperty(pair[0]);
      object x1 = property.GetValue(instance);
      object value = (object) null;
      new TypeSwitch().Case<string>((Action<string>) (x => value = (object) pair[1])).Case<int>((Action<int>) (x => value = (object) int.Parse(pair[1]))).Case<bool>((Action<bool>) (x => value = (object) bool.Parse(pair[1]))).Case<List<string>>((Action<List<string>>) (x => value = (object) ((IEnumerable<string>) pair[1].Split(',')).ToList<string>())).Case<System.Windows.Media.Color>((Action<System.Windows.Media.Color>) (x =>
      {
        System.Drawing.Color color = System.Drawing.Color.FromName(pair[1]);
        int r = (int) color.R;
        color = System.Drawing.Color.FromName(pair[1]);
        int g = (int) color.G;
        color = System.Drawing.Color.FromName(pair[1]);
        int b = (int) color.B;
        value = (object) System.Windows.Media.Color.FromRgb((byte) r, (byte) g, (byte) b);
      })).Switch(x1);
      // ISSUE: reference to a compiler-generated field
      if (IOManager.\u003C\u003Eo__17.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        IOManager.\u003C\u003Eo__17.\u003C\u003Ep__0 = CallSite<Action<CallSite, PropertyInfo, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetValue", (IEnumerable<Type>) null, typeof (IOManager), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      IOManager.\u003C\u003Eo__17.\u003C\u003Ep__0.Target((CallSite) IOManager.\u003C\u003Eo__17.\u003C\u003Ep__0, property, instance, value);
    }
    return instance;
  }
}
