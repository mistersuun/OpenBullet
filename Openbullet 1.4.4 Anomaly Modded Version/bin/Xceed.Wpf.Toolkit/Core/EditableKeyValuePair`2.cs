// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.EditableKeyValuePair`2
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core;

public class EditableKeyValuePair<TKey, TValue> : CustomTypeDescriptor
{
  private PropertyDescriptorCollection _properties;

  public TKey Key { get; set; }

  public TValue Value { get; set; }

  public EditableKeyValuePair()
  {
    this._properties = new PropertyDescriptorCollection(new List<PropertyDescriptor>()
    {
      TypeDescriptor.CreateProperty(this.GetType(), nameof (Key), typeof (TKey)),
      TypeDescriptor.CreateProperty(this.GetType(), nameof (Value), typeof (TValue))
    }.ToArray());
  }

  public EditableKeyValuePair(TKey key, TValue value)
    : this()
  {
    this.Key = key;
    this.Value = value;
  }

  public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
  {
    return this.GetProperties();
  }

  public override PropertyDescriptorCollection GetProperties() => this._properties;

  public override object GetPropertyOwner(PropertyDescriptor pd) => (object) this;

  public override string ToString() => $"[{(object) this.Key},{(object) this.Value}]";
}
