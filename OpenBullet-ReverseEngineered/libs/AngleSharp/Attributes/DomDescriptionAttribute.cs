// Decompiled with JetBrains decompiler
// Type: AngleSharp.Attributes.DomDescriptionAttribute
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;

#nullable disable
namespace AngleSharp.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
public sealed class DomDescriptionAttribute : Attribute
{
  public DomDescriptionAttribute(string description) => this.Description = description;

  public string Description { get; private set; }
}
