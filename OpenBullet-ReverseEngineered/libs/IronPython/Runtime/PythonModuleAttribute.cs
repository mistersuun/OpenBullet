// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonModuleAttribute
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace IronPython.Runtime;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public sealed class PythonModuleAttribute : PlatformsAttribute
{
  public PythonModuleAttribute(string name, Type type, params PlatformID[] validPlatforms)
  {
    ContractUtils.RequiresNotNull((object) name, nameof (name));
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    this.Name = name;
    this.Type = type;
    this.ValidPlatforms = validPlatforms;
  }

  public PythonModuleAttribute(
    string name,
    Type type,
    PlatformsAttribute.PlatformFamily validPlatformFamily)
  {
    ContractUtils.RequiresNotNull((object) name, nameof (name));
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    this.Name = name;
    this.Type = type;
    this.SetValidPlatforms(validPlatformFamily);
  }

  public string Name { get; }

  public Type Type { get; }
}
