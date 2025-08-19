// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.PropertyAccessors
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public struct PropertyAccessors
{
  private readonly PropertyDef m_property;
  private readonly MetadataToken m_getter;
  private readonly MetadataToken m_setter;

  internal PropertyAccessors(PropertyDef propertyDef, MetadataToken getter, MetadataToken setter)
  {
    this.m_getter = getter;
    this.m_setter = setter;
    this.m_property = propertyDef;
  }

  public PropertyDef DeclaringProperty => this.m_property;

  public bool HasGetter => !this.m_getter.IsNull;

  public bool HasSetter => !this.m_setter.IsNull;

  public MethodDef Getter
  {
    get => new MetadataRecord(this.m_getter, this.m_property.Record.Tables).MethodDef;
  }

  public MethodDef Setter
  {
    get => new MetadataRecord(this.m_setter, this.m_property.Record.Tables).MethodDef;
  }

  public IEnumerable<MethodDef> Others => throw new NotImplementedException();
}
