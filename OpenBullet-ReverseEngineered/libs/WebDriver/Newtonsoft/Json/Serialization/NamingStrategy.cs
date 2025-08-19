// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.NamingStrategy
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace Newtonsoft.Json.Serialization;

internal abstract class NamingStrategy
{
  public bool ProcessDictionaryKeys { get; set; }

  public bool ProcessExtensionDataNames { get; set; }

  public bool OverrideSpecifiedNames { get; set; }

  public virtual string GetPropertyName(string name, bool hasSpecifiedName)
  {
    return hasSpecifiedName && !this.OverrideSpecifiedNames ? name : this.ResolvePropertyName(name);
  }

  public virtual string GetExtensionDataName(string name)
  {
    return !this.ProcessExtensionDataNames ? name : this.ResolvePropertyName(name);
  }

  public virtual string GetDictionaryKey(string key)
  {
    return !this.ProcessDictionaryKeys ? key : this.ResolvePropertyName(key);
  }

  protected abstract string ResolvePropertyName(string name);
}
