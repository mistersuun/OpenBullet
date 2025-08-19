// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Configuration.OptionElement
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Runtime;
using System;
using System.Configuration;

#nullable disable
namespace Microsoft.Scripting.Hosting.Configuration;

public class OptionElement : ConfigurationElement
{
  private const string _Option = "option";
  private const string _Value = "value";
  private const string _Language = "language";
  private static ConfigurationPropertyCollection _Properties = new ConfigurationPropertyCollection()
  {
    new ConfigurationProperty("option", typeof (string), (object) string.Empty, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey),
    new ConfigurationProperty("value", typeof (string), (object) string.Empty, ConfigurationPropertyOptions.IsRequired),
    new ConfigurationProperty("language", typeof (string), (object) string.Empty, ConfigurationPropertyOptions.IsKey)
  };

  protected override ConfigurationPropertyCollection Properties => OptionElement._Properties;

  public string Name
  {
    get => (string) this["option"];
    set => this["option"] = (object) value;
  }

  public string Value
  {
    get => (string) this["value"];
    set => this[nameof (value)] = (object) value;
  }

  public string Language
  {
    get => (string) this["language"];
    set => this["language"] = (object) value;
  }

  internal object GetKey() => (object) new OptionElement.Key(this.Name, this.Language);

  internal sealed class Key : IEquatable<OptionElement.Key>
  {
    private readonly string _option;
    private readonly string _language;

    public string Option => this._option;

    public string Language => this._language;

    public Key(string option, string language)
    {
      this._option = option;
      this._language = language;
    }

    public override bool Equals(object obj) => this.Equals(obj as OptionElement.Key);

    public bool Equals(OptionElement.Key other)
    {
      return other != null && DlrConfiguration.OptionNameComparer.Equals(this._option, other._option) && DlrConfiguration.LanguageNameComparer.Equals(this._language, other._language);
    }

    public override int GetHashCode()
    {
      return this._option.GetHashCode() ^ (this._language ?? string.Empty).GetHashCode();
    }

    public override string ToString()
    {
      return (string.IsNullOrEmpty(this._language) ? string.Empty : this._language + ":") + this._option;
    }
  }
}
