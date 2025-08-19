// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonTypeAttribute
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;

#nullable disable
namespace IronPython.Runtime;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
public sealed class PythonTypeAttribute : Attribute
{
  private readonly string _name;

  public PythonTypeAttribute()
  {
  }

  public PythonTypeAttribute(string name) => this._name = name;

  public string Name => this._name;
}
