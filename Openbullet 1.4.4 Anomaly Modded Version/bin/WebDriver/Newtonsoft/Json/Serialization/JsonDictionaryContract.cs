// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.JsonDictionaryContract
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

#nullable disable
namespace Newtonsoft.Json.Serialization;

internal class JsonDictionaryContract : JsonContainerContract
{
  private readonly Type _genericCollectionDefinitionType;
  private Type _genericWrapperType;
  private ObjectConstructor<object> _genericWrapperCreator;
  private Func<object> _genericTemporaryDictionaryCreator;
  private readonly ConstructorInfo _parameterizedConstructor;
  private ObjectConstructor<object> _overrideCreator;
  private ObjectConstructor<object> _parameterizedCreator;

  public Func<string, string> DictionaryKeyResolver { get; set; }

  public Type DictionaryKeyType { get; }

  public Type DictionaryValueType { get; }

  internal JsonContract KeyContract { get; set; }

  internal bool ShouldCreateWrapper { get; }

  internal ObjectConstructor<object> ParameterizedCreator
  {
    get
    {
      if (this._parameterizedCreator == null)
        this._parameterizedCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor((MethodBase) this._parameterizedConstructor);
      return this._parameterizedCreator;
    }
  }

  public ObjectConstructor<object> OverrideCreator
  {
    get => this._overrideCreator;
    set => this._overrideCreator = value;
  }

  public bool HasParameterizedCreator { get; set; }

  internal bool HasParameterizedCreatorInternal
  {
    get
    {
      return this.HasParameterizedCreator || this._parameterizedCreator != null || this._parameterizedConstructor != (ConstructorInfo) null;
    }
  }

  public JsonDictionaryContract(Type underlyingType)
    : base(underlyingType)
  {
    this.ContractType = JsonContractType.Dictionary;
    Type keyType;
    Type valueType;
    if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof (IDictionary<,>), out this._genericCollectionDefinitionType))
    {
      keyType = this._genericCollectionDefinitionType.GetGenericArguments()[0];
      valueType = this._genericCollectionDefinitionType.GetGenericArguments()[1];
      if (ReflectionUtils.IsGenericDefinition(this.UnderlyingType, typeof (IDictionary<,>)))
        this.CreatedType = typeof (Dictionary<,>).MakeGenericType(keyType, valueType);
      this.IsReadOnlyOrFixedSize = ReflectionUtils.InheritsGenericDefinition(underlyingType, typeof (ReadOnlyDictionary<,>));
    }
    else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof (IReadOnlyDictionary<,>), out this._genericCollectionDefinitionType))
    {
      keyType = this._genericCollectionDefinitionType.GetGenericArguments()[0];
      valueType = this._genericCollectionDefinitionType.GetGenericArguments()[1];
      if (ReflectionUtils.IsGenericDefinition(this.UnderlyingType, typeof (IReadOnlyDictionary<,>)))
        this.CreatedType = typeof (ReadOnlyDictionary<,>).MakeGenericType(keyType, valueType);
      this.IsReadOnlyOrFixedSize = true;
    }
    else
    {
      ReflectionUtils.GetDictionaryKeyValueTypes(this.UnderlyingType, out keyType, out valueType);
      if (this.UnderlyingType == typeof (IDictionary))
        this.CreatedType = typeof (Dictionary<object, object>);
    }
    if (keyType != (Type) null && valueType != (Type) null)
    {
      this._parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(this.CreatedType, typeof (KeyValuePair<,>).MakeGenericType(keyType, valueType), typeof (IDictionary<,>).MakeGenericType(keyType, valueType));
      if (!this.HasParameterizedCreatorInternal && underlyingType.Name == "FSharpMap`2")
      {
        FSharpUtils.EnsureInitialized(underlyingType.Assembly());
        this._parameterizedCreator = FSharpUtils.CreateMap(keyType, valueType);
      }
    }
    this.ShouldCreateWrapper = !typeof (IDictionary).IsAssignableFrom(this.CreatedType);
    this.DictionaryKeyType = keyType;
    this.DictionaryValueType = valueType;
    Type createdType;
    ObjectConstructor<object> parameterizedCreator;
    if (!ImmutableCollectionsUtils.TryBuildImmutableForDictionaryContract(underlyingType, this.DictionaryKeyType, this.DictionaryValueType, out createdType, out parameterizedCreator))
      return;
    this.CreatedType = createdType;
    this._parameterizedCreator = parameterizedCreator;
    this.IsReadOnlyOrFixedSize = true;
  }

  internal IWrappedDictionary CreateWrapper(object dictionary)
  {
    if (this._genericWrapperCreator == null)
    {
      this._genericWrapperType = typeof (DictionaryWrapper<,>).MakeGenericType(this.DictionaryKeyType, this.DictionaryValueType);
      this._genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor((MethodBase) this._genericWrapperType.GetConstructor(new Type[1]
      {
        this._genericCollectionDefinitionType
      }));
    }
    return (IWrappedDictionary) this._genericWrapperCreator(dictionary);
  }

  internal IDictionary CreateTemporaryDictionary()
  {
    if (this._genericTemporaryDictionaryCreator == null)
    {
      Type type1 = typeof (Dictionary<,>);
      Type[] typeArray = new Type[2];
      Type type2 = this.DictionaryKeyType;
      if ((object) type2 == null)
        type2 = typeof (object);
      typeArray[0] = type2;
      Type type3 = this.DictionaryValueType;
      if ((object) type3 == null)
        type3 = typeof (object);
      typeArray[1] = type3;
      this._genericTemporaryDictionaryCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(type1.MakeGenericType(typeArray));
    }
    return (IDictionary) this._genericTemporaryDictionaryCreator();
  }
}
