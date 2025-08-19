// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Firefox.Preferences
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

#nullable disable
namespace OpenQA.Selenium.Firefox;

internal class Preferences
{
  private Dictionary<string, string> preferences = new Dictionary<string, string>();
  private Dictionary<string, string> immutablePreferences = new Dictionary<string, string>();

  public Preferences(
    Dictionary<string, object> defaultImmutablePreferences,
    Dictionary<string, object> defaultPreferences)
  {
    if (defaultImmutablePreferences != null)
    {
      foreach (KeyValuePair<string, object> immutablePreference in defaultImmutablePreferences)
      {
        this.SetPreferenceValue(immutablePreference.Key, immutablePreference.Value);
        this.immutablePreferences.Add(immutablePreference.Key, immutablePreference.Value.ToString());
      }
    }
    if (defaultPreferences == null)
      return;
    foreach (KeyValuePair<string, object> defaultPreference in defaultPreferences)
      this.SetPreferenceValue(defaultPreference.Key, defaultPreference.Value);
  }

  internal void SetPreference(string key, string value)
  {
    this.SetPreferenceValue(key, (object) value);
  }

  internal void SetPreference(string key, int value)
  {
    this.SetPreferenceValue(key, (object) value);
  }

  internal void SetPreference(string key, bool value)
  {
    this.SetPreferenceValue(key, (object) value);
  }

  internal string GetPreference(string preferenceName)
  {
    return this.preferences.ContainsKey(preferenceName) ? this.preferences[preferenceName] : string.Empty;
  }

  internal void AppendPreferences(Dictionary<string, string> preferencesToAdd)
  {
    foreach (KeyValuePair<string, string> keyValuePair in preferencesToAdd)
    {
      if (this.IsSettablePreference(keyValuePair.Key))
        this.preferences[keyValuePair.Key] = keyValuePair.Value;
    }
  }

  internal void WriteToFile(string filePath)
  {
    using (TextWriter text = (TextWriter) File.CreateText(filePath))
    {
      foreach (KeyValuePair<string, string> preference in this.preferences)
      {
        string str = preference.Value.Replace("\\", "\\\\");
        text.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "user_pref(\"{0}\", {1});", (object) preference.Key, (object) str));
      }
    }
  }

  private static bool IsWrappedAsString(string value)
  {
    return value.StartsWith("\"", StringComparison.OrdinalIgnoreCase) && value.EndsWith("\"", StringComparison.OrdinalIgnoreCase);
  }

  private bool IsSettablePreference(string preferenceName)
  {
    return !this.immutablePreferences.ContainsKey(preferenceName);
  }

  private void SetPreferenceValue(string key, object value)
  {
    if (!this.IsSettablePreference(key))
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Preference {0} may not be overridden: frozen value={1}, requested value={2}", (object) key, (object) this.immutablePreferences[key], (object) value.ToString()));
    switch (value)
    {
      case string str:
        if (Preferences.IsWrappedAsString(str))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Preference values must be plain strings: {0}: {1}", (object) key, value));
        this.preferences[key] = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", value);
        break;
      case bool _:
        this.preferences[key] = Convert.ToBoolean(value, (IFormatProvider) CultureInfo.InvariantCulture).ToString().ToLowerInvariant();
        break;
      case int _:
      case long _:
        this.preferences[key] = Convert.ToInt32(value, (IFormatProvider) CultureInfo.InvariantCulture).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        break;
      default:
        throw new WebDriverException("Value must be string, int or boolean");
    }
  }
}
