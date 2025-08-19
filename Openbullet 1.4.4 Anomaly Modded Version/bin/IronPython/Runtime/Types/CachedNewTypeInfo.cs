// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.CachedNewTypeInfo
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime.Types;

public class CachedNewTypeInfo
{
  private readonly Type _type;
  private readonly Dictionary<string, string[]> _specialNames;
  private readonly Type[] _interfaceTypes;

  public CachedNewTypeInfo(
    Type type,
    Dictionary<string, string[]> specialNames,
    Type[] interfaceTypes)
  {
    this._type = type;
    this._specialNames = specialNames;
    this._interfaceTypes = interfaceTypes ?? ReflectionUtils.EmptyTypes;
  }

  public IList<Type> InterfaceTypes => (IList<Type>) this._interfaceTypes;

  public Type Type => this._type;

  public Dictionary<string, string[]> SpecialNames => this._specialNames;
}
