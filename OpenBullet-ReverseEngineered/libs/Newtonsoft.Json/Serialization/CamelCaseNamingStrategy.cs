// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.CamelCaseNamingStrategy
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using Newtonsoft.Json.Utilities;

#nullable disable
namespace Newtonsoft.Json.Serialization;

public class CamelCaseNamingStrategy : NamingStrategy
{
  public CamelCaseNamingStrategy(bool processDictionaryKeys, bool overrideSpecifiedNames)
  {
    this.ProcessDictionaryKeys = processDictionaryKeys;
    this.OverrideSpecifiedNames = overrideSpecifiedNames;
  }

  public CamelCaseNamingStrategy(
    bool processDictionaryKeys,
    bool overrideSpecifiedNames,
    bool processExtensionDataNames)
    : this(processDictionaryKeys, overrideSpecifiedNames)
  {
    this.ProcessExtensionDataNames = processExtensionDataNames;
  }

  public CamelCaseNamingStrategy()
  {
  }

  protected override string ResolvePropertyName(string name) => StringUtils.ToCamelCase(name);
}
