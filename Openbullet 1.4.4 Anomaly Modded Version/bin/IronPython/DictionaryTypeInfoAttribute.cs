// Decompiled with JetBrains decompiler
// Type: IronPython.DictionaryTypeInfoAttribute
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;

#nullable disable
namespace IronPython;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false)]
public sealed class DictionaryTypeInfoAttribute : Attribute
{
  private readonly Type _keyType;
  private readonly Type _valueType;

  public DictionaryTypeInfoAttribute(Type keyType, Type valueType)
  {
    this._keyType = keyType;
    this._valueType = valueType;
  }

  public Type KeyType => this._keyType;

  public Type ValueType => this._valueType;
}
