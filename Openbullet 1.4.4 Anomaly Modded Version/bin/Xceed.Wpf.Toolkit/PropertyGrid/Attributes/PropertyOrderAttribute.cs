// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Attributes.PropertyOrderAttribute
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public class PropertyOrderAttribute : Attribute
{
  public int Order { get; set; }

  public UsageContextEnum UsageContext { get; set; }

  public override object TypeId => (object) this;

  public PropertyOrderAttribute(int order)
    : this(order, UsageContextEnum.Both)
  {
  }

  public PropertyOrderAttribute(int order, UsageContextEnum usageContext)
  {
    this.Order = order;
    this.UsageContext = usageContext;
  }
}
