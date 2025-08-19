// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JPropertyDescriptor
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using System;
using System.ComponentModel;

#nullable disable
namespace Newtonsoft.Json.Linq;

public class JPropertyDescriptor(string name) : PropertyDescriptor(name, (Attribute[]) null)
{
  private static JObject CastInstance(object instance) => (JObject) instance;

  public override bool CanResetValue(object component) => false;

  public override object GetValue(object component)
  {
    // ISSUE: explicit non-virtual call
    return !(component is JObject jobject) ? (object) null : (object) __nonvirtual (jobject[this.Name]);
  }

  public override void ResetValue(object component)
  {
  }

  public override void SetValue(object component, object value)
  {
    if (!(component is JObject jobject))
      return;
    if (!(value is JToken jtoken1))
      jtoken1 = (JToken) new JValue(value);
    JToken jtoken2 = jtoken1;
    jobject[this.Name] = jtoken2;
  }

  public override bool ShouldSerializeValue(object component) => false;

  public override Type ComponentType => typeof (JObject);

  public override bool IsReadOnly => false;

  public override Type PropertyType => typeof (object);

  protected override int NameHashCode => base.NameHashCode;
}
