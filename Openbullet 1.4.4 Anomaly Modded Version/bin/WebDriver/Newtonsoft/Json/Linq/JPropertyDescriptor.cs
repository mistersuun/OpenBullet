// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JPropertyDescriptor
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.ComponentModel;

#nullable disable
namespace Newtonsoft.Json.Linq;

internal class JPropertyDescriptor(string name) : PropertyDescriptor(name, (Attribute[]) null)
{
  private static JObject CastInstance(object instance) => (JObject) instance;

  public override bool CanResetValue(object component) => false;

  public override object GetValue(object component)
  {
    return (object) JPropertyDescriptor.CastInstance(component)[this.Name];
  }

  public override void ResetValue(object component)
  {
  }

  public override void SetValue(object component, object value)
  {
    if (!(value is JToken jtoken1))
      jtoken1 = (JToken) new JValue(value);
    JToken jtoken2 = jtoken1;
    JPropertyDescriptor.CastInstance(component)[this.Name] = jtoken2;
  }

  public override bool ShouldSerializeValue(object component) => false;

  public override Type ComponentType => typeof (JObject);

  public override bool IsReadOnly => false;

  public override Type PropertyType => typeof (object);

  protected override int NameHashCode => base.NameHashCode;
}
