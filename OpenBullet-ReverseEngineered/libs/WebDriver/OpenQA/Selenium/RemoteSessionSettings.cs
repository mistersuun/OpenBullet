// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.RemoteSessionSettings
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json;
using OpenQA.Selenium.Remote;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium;

public class RemoteSessionSettings : ICapabilities
{
  private const string FirstMatchCapabilityName = "firstMatch";
  private const string AlwaysMatchCapabilityName = "alwaysMatch";
  private readonly List<string> reservedSettingNames = new List<string>()
  {
    "firstMatch",
    "alwaysMatch"
  };
  private DriverOptions mustMatchDriverOptions;
  private List<DriverOptions> firstMatchOptions = new List<DriverOptions>();
  private Dictionary<string, object> remoteMetadataSettings = new Dictionary<string, object>();

  public RemoteSessionSettings()
  {
  }

  public RemoteSessionSettings(
    DriverOptions mustMatchDriverOptions,
    params DriverOptions[] firstMatchDriverOptions)
  {
    this.mustMatchDriverOptions = mustMatchDriverOptions;
    foreach (DriverOptions matchDriverOption in firstMatchDriverOptions)
      this.AddFirstMatchDriverOption(matchDriverOption);
  }

  internal DriverOptions MustMatchDriverOptions => this.mustMatchDriverOptions;

  internal int FirstMatchOptionsCount => this.firstMatchOptions.Count;

  public object this[string capabilityName]
  {
    get
    {
      switch (capabilityName)
      {
        case "alwaysMatch":
          return (object) this.GetAlwaysMatchOptionsAsSerializableDictionary();
        case "firstMatch":
          return (object) this.GetFirstMatchOptionsAsSerializableList();
        default:
          return this.remoteMetadataSettings.ContainsKey(capabilityName) ? this.remoteMetadataSettings[capabilityName] : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The capability {0} is not present in this set of capabilities", (object) capabilityName));
      }
    }
  }

  public void AddMetadataSetting(string settingName, object settingValue)
  {
    if (string.IsNullOrEmpty(settingName))
      throw new ArgumentException("Metadata setting name cannot be null or empty", nameof (settingName));
    if (this.reservedSettingNames.Contains(settingName))
      throw new ArgumentException($"'{settingName}' is a reserved name for a metadata setting, and cannot be used as a name.", nameof (settingName));
    this.remoteMetadataSettings[settingName] = this.IsJsonSerializable(settingValue) ? settingValue : throw new ArgumentException("Metadata setting value must be JSON serializable.", nameof (settingValue));
  }

  public void AddFirstMatchDriverOption(DriverOptions options)
  {
    if (this.mustMatchDriverOptions != null)
    {
      DriverOptionsMergeResult mergeResult = this.mustMatchDriverOptions.GetMergeResult(options);
      if (mergeResult.IsMergeConflict)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "You cannot request the same capability in both must-match and first-match capabilities. You are attempting to add a first-match driver options object that defines a capability, '{0}', that is already defined in the must-match driver options.", (object) mergeResult.MergeConflictOptionName), nameof (options));
    }
    this.firstMatchOptions.Add(options);
  }

  public void SetMustMatchDriverOptions(DriverOptions options)
  {
    if (this.firstMatchOptions.Count > 0)
    {
      int num = 0;
      foreach (DriverOptions firstMatchOption in this.firstMatchOptions)
      {
        DriverOptionsMergeResult mergeResult = firstMatchOption.GetMergeResult(options);
        if (mergeResult.IsMergeConflict)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "You cannot request the same capability in both must-match and first-match capabilities. You are attempting to add a must-match driver options object that defines a capability, '{0}', that is already defined in the first-match driver options with index {1}.", (object) mergeResult.MergeConflictOptionName, (object) num), nameof (options));
        ++num;
      }
    }
    this.mustMatchDriverOptions = options;
  }

  public bool HasCapability(string capability)
  {
    return capability == "alwaysMatch" || capability == "firstMatch" || this.remoteMetadataSettings.ContainsKey(capability);
  }

  public object GetCapability(string capability)
  {
    switch (capability)
    {
      case "alwaysMatch":
        return (object) this.GetAlwaysMatchOptionsAsSerializableDictionary();
      case "firstMatch":
        return (object) this.GetFirstMatchOptionsAsSerializableList();
      default:
        return this.remoteMetadataSettings.ContainsKey(capability) ? this.remoteMetadataSettings[capability] : (object) null;
    }
  }

  public Dictionary<string, object> ToDictionary()
  {
    Dictionary<string, object> dictionary = new Dictionary<string, object>();
    foreach (KeyValuePair<string, object> remoteMetadataSetting in this.remoteMetadataSettings)
      dictionary[remoteMetadataSetting.Key] = remoteMetadataSetting.Value;
    if (this.mustMatchDriverOptions != null)
      dictionary["alwaysMatch"] = (object) this.GetAlwaysMatchOptionsAsSerializableDictionary();
    if (this.firstMatchOptions.Count > 0)
    {
      List<object> serializableList = this.GetFirstMatchOptionsAsSerializableList();
      dictionary["firstMatch"] = (object) serializableList;
    }
    return dictionary;
  }

  public override string ToString()
  {
    return JsonConvert.SerializeObject((object) this.ToDictionary(), Formatting.Indented);
  }

  internal DriverOptions GetFirstMatchDriverOptions(int firstMatchIndex)
  {
    if (firstMatchIndex < 0 || firstMatchIndex >= this.firstMatchOptions.Count)
      throw new ArgumentException("Requested index must be greater than zero and less than the count of firstMatch options added.");
    return this.firstMatchOptions[firstMatchIndex];
  }

  private Dictionary<string, object> GetAlwaysMatchOptionsAsSerializableDictionary()
  {
    return this.mustMatchDriverOptions.ToDictionary();
  }

  private List<object> GetFirstMatchOptionsAsSerializableList()
  {
    List<object> serializableList = new List<object>();
    foreach (DriverOptions firstMatchOption in this.firstMatchOptions)
      serializableList.Add((object) firstMatchOption.ToDictionary());
    return serializableList;
  }

  private bool IsJsonSerializable(object arg)
  {
    IEnumerable enumerable = arg as IEnumerable;
    IDictionary dictionary = arg as IDictionary;
    switch (arg)
    {
      case string _:
      case float _:
      case double _:
      case int _:
      case long _:
      case bool _:
      case null:
        return true;
      default:
        if (dictionary != null)
        {
          foreach (object key in (IEnumerable) dictionary.Keys)
          {
            if (!(key is string))
              return false;
          }
          foreach (object obj in (IEnumerable) dictionary.Values)
          {
            if (!this.IsJsonSerializable(obj))
              return false;
          }
        }
        else
        {
          if (enumerable == null)
            return false;
          foreach (object obj in enumerable)
          {
            if (!this.IsJsonSerializable(obj))
              return false;
          }
        }
        return true;
    }
  }
}
