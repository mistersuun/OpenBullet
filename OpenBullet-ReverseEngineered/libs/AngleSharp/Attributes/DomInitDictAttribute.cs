// Decompiled with JetBrains decompiler
// Type: AngleSharp.Attributes.DomInitDictAttribute
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;

#nullable disable
namespace AngleSharp.Attributes;

[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
public sealed class DomInitDictAttribute : Attribute
{
  public DomInitDictAttribute(int offset = 0, bool optional = false)
  {
    this.Offset = offset;
    this.IsOptional = optional;
  }

  public int Offset { get; private set; }

  public bool IsOptional { get; private set; }
}
