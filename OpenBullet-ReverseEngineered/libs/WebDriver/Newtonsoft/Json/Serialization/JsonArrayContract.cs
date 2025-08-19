// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.JsonArrayContract
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

internal class JsonArrayContract : JsonContainerContract
{
  private readonly Type _genericCollectionDefinitionType;
  private Type _genericWrapperType;
  private ObjectConstructor<object> _genericWrapperCreator;
  private Func<object> _genericTemporaryCollectionCreator;
  private readonly ConstructorInfo _parameterizedConstructor;
  private ObjectConstructor<object> _parameterizedCreator;
  private ObjectConstructor<object> _overrideCreator;

  public Type CollectionItemType { get; }

  public bool IsMultidimensionalArray { get; }

  internal bool IsArray { get; }

  internal bool ShouldCreateWrapper { get; }

  internal bool CanDeserialize { get; private set; }

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
    set
    {
      this._overrideCreator = value;
      this.CanDeserialize = true;
    }
  }

  public bool HasParameterizedCreator { get; set; }

  internal bool HasParameterizedCreatorInternal
  {
    get
    {
      return this.HasParameterizedCreator || this._parameterizedCreator != null || this._parameterizedConstructor != (ConstructorInfo) null;
    }
  }

  public JsonArrayContract(Type underlyingType)
    : base(underlyingType)
  {
    this.ContractType = JsonContractType.Array;
    this.IsArray = this.CreatedType.IsArray;
    bool flag;
    if (this.IsArray)
    {
      this.CollectionItemType = ReflectionUtils.GetCollectionItemType(this.UnderlyingType);
      this.IsReadOnlyOrFixedSize = true;
      this._genericCollectionDefinitionType = typeof (List<>).MakeGenericType(this.CollectionItemType);
      flag = true;
      this.IsMultidimensionalArray = this.IsArray && this.UnderlyingType.GetArrayRank() > 1;
    }
    else if (typeof (IList).IsAssignableFrom(underlyingType))
    {
      this.CollectionItemType = !ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof (ICollection<>), out this._genericCollectionDefinitionType) ? ReflectionUtils.GetCollectionItemType(underlyingType) : this._genericCollectionDefinitionType.GetGenericArguments()[0];
      if (underlyingType == typeof (IList))
        this.CreatedType = typeof (List<object>);
      if (this.CollectionItemType != (Type) null)
        this._parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(underlyingType, this.CollectionItemType);
      this.IsReadOnlyOrFixedSize = ReflectionUtils.InheritsGenericDefinition(underlyingType, typeof (ReadOnlyCollection<>));
      flag = true;
    }
    else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof (ICollection<>), out this._genericCollectionDefinitionType))
    {
      this.CollectionItemType = this._genericCollectionDefinitionType.GetGenericArguments()[0];
      if (ReflectionUtils.IsGenericDefinition(underlyingType, typeof (ICollection<>)) || ReflectionUtils.IsGenericDefinition(underlyingType, typeof (IList<>)))
        this.CreatedType = typeof (List<>).MakeGenericType(this.CollectionItemType);
      if (ReflectionUtils.IsGenericDefinition(underlyingType, typeof (ISet<>)))
        this.CreatedType = typeof (HashSet<>).MakeGenericType(this.CollectionItemType);
      this._parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(underlyingType, this.CollectionItemType);
      flag = true;
      this.ShouldCreateWrapper = true;
    }
    else
    {
      Type implementingType;
      if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof (IReadOnlyCollection<>), out implementingType))
      {
        this.CollectionItemType = implementingType.GetGenericArguments()[0];
        if (ReflectionUtils.IsGenericDefinition(underlyingType, typeof (IReadOnlyCollection<>)) || ReflectionUtils.IsGenericDefinition(underlyingType, typeof (IReadOnlyList<>)))
          this.CreatedType = typeof (ReadOnlyCollection<>).MakeGenericType(this.CollectionItemType);
        this._genericCollectionDefinitionType = typeof (List<>).MakeGenericType(this.CollectionItemType);
        this._parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(this.CreatedType, this.CollectionItemType);
        this.StoreFSharpListCreatorIfNecessary(underlyingType);
        this.IsReadOnlyOrFixedSize = true;
        flag = this.HasParameterizedCreatorInternal;
      }
      else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof (IEnumerable<>), out implementingType))
      {
        this.CollectionItemType = implementingType.GetGenericArguments()[0];
        if (ReflectionUtils.IsGenericDefinition(this.UnderlyingType, typeof (IEnumerable<>)))
          this.CreatedType = typeof (List<>).MakeGenericType(this.CollectionItemType);
        this._parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(underlyingType, this.CollectionItemType);
        this.StoreFSharpListCreatorIfNecessary(underlyingType);
        if (underlyingType.IsGenericType() && underlyingType.GetGenericTypeDefinition() == typeof (IEnumerable<>))
        {
          this._genericCollectionDefinitionType = implementingType;
          this.IsReadOnlyOrFixedSize = false;
          this.ShouldCreateWrapper = false;
          flag = true;
        }
        else
        {
          this._genericCollectionDefinitionType = typeof (List<>).MakeGenericType(this.CollectionItemType);
          this.IsReadOnlyOrFixedSize = true;
          this.ShouldCreateWrapper = true;
          flag = this.HasParameterizedCreatorInternal;
        }
      }
      else
      {
        flag = false;
        this.ShouldCreateWrapper = true;
      }
    }
    this.CanDeserialize = flag;
    Type createdType;
    ObjectConstructor<object> parameterizedCreator;
    if (!ImmutableCollectionsUtils.TryBuildImmutableForArrayContract(underlyingType, this.CollectionItemType, out createdType, out parameterizedCreator))
      return;
    this.CreatedType = createdType;
    this._parameterizedCreator = parameterizedCreator;
    this.IsReadOnlyOrFixedSize = true;
    this.CanDeserialize = true;
  }

  internal IWrappedCollection CreateWrapper(object list)
  {
    if (this._genericWrapperCreator == null)
    {
      this._genericWrapperType = typeof (CollectionWrapper<>).MakeGenericType(this.CollectionItemType);
      Type type;
      if (ReflectionUtils.InheritsGenericDefinition(this._genericCollectionDefinitionType, typeof (List<>)) || this._genericCollectionDefinitionType.GetGenericTypeDefinition() == typeof (IEnumerable<>))
        type = typeof (ICollection<>).MakeGenericType(this.CollectionItemType);
      else
        type = this._genericCollectionDefinitionType;
      this._genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor((MethodBase) this._genericWrapperType.GetConstructor(new Type[1]
      {
        type
      }));
    }
    return (IWrappedCollection) this._genericWrapperCreator(list);
  }

  internal IList CreateTemporaryCollection()
  {
    if (this._genericTemporaryCollectionCreator == null)
      this._genericTemporaryCollectionCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(typeof (List<>).MakeGenericType(this.IsMultidimensionalArray || this.CollectionItemType == (Type) null ? typeof (object) : this.CollectionItemType));
    return (IList) this._genericTemporaryCollectionCreator();
  }

  private void StoreFSharpListCreatorIfNecessary(Type underlyingType)
  {
    if (this.HasParameterizedCreatorInternal || !(underlyingType.Name == "FSharpList`1"))
      return;
    FSharpUtils.EnsureInitialized(underlyingType.Assembly());
    this._parameterizedCreator = FSharpUtils.CreateSeq(this.CollectionItemType);
  }
}
