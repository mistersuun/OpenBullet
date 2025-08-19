// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.SequenceTypeInfoAttribute
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace IronPython.Runtime;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false)]
public sealed class SequenceTypeInfoAttribute : Attribute
{
  private readonly ReadOnlyCollection<Type> _types;

  public SequenceTypeInfoAttribute(params Type[] types)
  {
    this._types = new ReadOnlyCollection<Type>((IList<Type>) types);
  }

  public ReadOnlyCollection<Type> Types => this._types;
}
