// Decompiled with JetBrains decompiler
// Type: AngleSharp.Attributes.DomPutForwardsAttribute
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;

#nullable disable
namespace AngleSharp.Attributes;

[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class DomPutForwardsAttribute : Attribute
{
  public DomPutForwardsAttribute(string propertyName) => this.PropertyName = propertyName;

  public string PropertyName { get; private set; }
}
