// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Configuration.OptionElementCollection
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System.Configuration;

#nullable disable
namespace Microsoft.Scripting.Hosting.Configuration;

public class OptionElementCollection : ConfigurationElementCollection
{
  public OptionElementCollection() => this.AddElementName = "set";

  public override ConfigurationElementCollectionType CollectionType
  {
    get => ConfigurationElementCollectionType.AddRemoveClearMap;
  }

  protected override ConfigurationElement CreateNewElement()
  {
    return (ConfigurationElement) new OptionElement();
  }

  protected override bool ThrowOnDuplicate => false;

  protected override object GetElementKey(ConfigurationElement element)
  {
    return ((OptionElement) element).GetKey();
  }
}
