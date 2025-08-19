// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.RestrictedArguments
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

public sealed class RestrictedArguments
{
  private readonly DynamicMetaObject[] _objects;
  private readonly Type[] _types;

  public RestrictedArguments(
    DynamicMetaObject[] objects,
    Type[] types,
    bool hasUntypedRestrictions)
  {
    this._objects = objects;
    this._types = types;
    this.HasUntypedRestrictions = hasUntypedRestrictions;
  }

  public int Length => this._objects.Length;

  public DynamicMetaObject GetObject(int i) => this._objects[i];

  public Type GetType(int i) => this._types[i];

  public bool HasUntypedRestrictions { get; }

  public BindingRestrictions GetAllRestrictions()
  {
    return BindingRestrictions.Combine((IList<DynamicMetaObject>) this._objects);
  }

  public IList<DynamicMetaObject> GetObjects()
  {
    return (IList<DynamicMetaObject>) new ReadOnlyCollection<DynamicMetaObject>((IList<DynamicMetaObject>) this._objects);
  }

  public IList<Type> GetTypes()
  {
    return (IList<Type>) new ReadOnlyCollection<Type>((IList<Type>) this._types);
  }
}
