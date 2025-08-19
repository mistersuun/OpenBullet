// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.SnakeCaseNamingStrategy
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Utilities;

#nullable disable
namespace Newtonsoft.Json.Serialization;

internal class SnakeCaseNamingStrategy : NamingStrategy
{
  public SnakeCaseNamingStrategy(bool processDictionaryKeys, bool overrideSpecifiedNames)
  {
    this.ProcessDictionaryKeys = processDictionaryKeys;
    this.OverrideSpecifiedNames = overrideSpecifiedNames;
  }

  public SnakeCaseNamingStrategy(
    bool processDictionaryKeys,
    bool overrideSpecifiedNames,
    bool processExtensionDataNames)
    : this(processDictionaryKeys, overrideSpecifiedNames)
  {
    this.ProcessExtensionDataNames = processExtensionDataNames;
  }

  public SnakeCaseNamingStrategy()
  {
  }

  protected override string ResolvePropertyName(string name) => StringUtils.ToSnakeCase(name);
}
