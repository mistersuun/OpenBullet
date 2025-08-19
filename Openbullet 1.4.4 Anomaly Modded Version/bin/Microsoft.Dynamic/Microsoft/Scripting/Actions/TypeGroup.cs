// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.TypeGroup
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Actions;

public sealed class TypeGroup : TypeTracker
{
  private readonly Dictionary<int, Type> _typesByArity;
  private readonly string _name;

  private TypeGroup(Type t1, int arity1, Type t2, int arity2)
  {
    this._typesByArity = new Dictionary<int, Type>();
    this._typesByArity[arity1] = t1;
    this._typesByArity[arity2] = t2;
    this._name = ReflectionUtils.GetNormalizedTypeName(t1);
  }

  private TypeGroup(Type t1, TypeGroup existingTypes)
  {
    this._typesByArity = new Dictionary<int, Type>((IDictionary<int, Type>) existingTypes._typesByArity);
    this._typesByArity[TypeGroup.GetGenericArity(t1)] = t1;
    this._name = existingTypes.Name;
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder(base.ToString());
    stringBuilder.Append($":{this.Name}(");
    bool flag = false;
    foreach (Type type in this.Types)
    {
      if (flag)
        stringBuilder.Append(", ");
      stringBuilder.Append(type.Name);
      flag = true;
    }
    stringBuilder.Append(")");
    return stringBuilder.ToString();
  }

  public override IList<string> GetMemberNames()
  {
    HashSet<string> stringSet = new HashSet<string>();
    foreach (Type type in this.Types)
      TypeTracker.GetMemberNames(type, stringSet);
    return (IList<string>) stringSet.ToArray<string>();
  }

  public TypeTracker GetTypeForArity(int arity)
  {
    Type type;
    return !this._typesByArity.TryGetValue(arity, out type) ? (TypeTracker) null : TypeTracker.GetTypeTracker(type);
  }

  public static TypeTracker UpdateTypeEntity(TypeTracker existingTypeEntity, TypeTracker newType)
  {
    if (existingTypeEntity == null)
      return newType;
    NestedTypeTracker nestedTypeTracker = existingTypeEntity as NestedTypeTracker;
    TypeGroup existingTypes = existingTypeEntity as TypeGroup;
    if (nestedTypeTracker == null)
      return (TypeTracker) new TypeGroup(newType.Type, existingTypes);
    int genericArity1 = TypeGroup.GetGenericArity(nestedTypeTracker.Type);
    int genericArity2 = TypeGroup.GetGenericArity(newType.Type);
    return genericArity1 == genericArity2 ? newType : (TypeTracker) new TypeGroup(nestedTypeTracker.Type, genericArity1, newType.Type, genericArity2);
  }

  private static int GetGenericArity(Type type)
  {
    return !type.IsGenericType() ? 0 : type.GetGenericArguments().Length;
  }

  public Type GetNonGenericType()
  {
    Type nonGenericType;
    if (this.TryGetNonGenericType(out nonGenericType))
      return nonGenericType;
    throw Microsoft.Scripting.Error.NonGenericWithGenericGroup((object) this.Name);
  }

  public bool TryGetNonGenericType(out Type nonGenericType)
  {
    return this._typesByArity.TryGetValue(0, out nonGenericType);
  }

  private Type SampleType
  {
    get
    {
      using (IEnumerator<Type> enumerator = this.Types.GetEnumerator())
      {
        enumerator.MoveNext();
        return enumerator.Current;
      }
    }
  }

  public IEnumerable<Type> Types => (IEnumerable<Type>) this._typesByArity.Values;

  public IDictionary<int, Type> TypesByArity
  {
    get
    {
      return (IDictionary<int, Type>) new ReadOnlyDictionary<int, Type>((IDictionary<int, Type>) this._typesByArity);
    }
  }

  public override TrackerTypes MemberType => TrackerTypes.TypeGroup;

  public override Type DeclaringType => this.SampleType.DeclaringType;

  public override string Name => this._name;

  public override Type Type => this.GetNonGenericType();

  public override bool IsGenericType => this._typesByArity.Count > 0;

  public override bool IsPublic => this.GetNonGenericType().IsPublic();
}
