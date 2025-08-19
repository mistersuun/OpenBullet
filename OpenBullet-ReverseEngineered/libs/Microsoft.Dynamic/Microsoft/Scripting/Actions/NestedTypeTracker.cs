// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.NestedTypeTracker
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class NestedTypeTracker : TypeTracker
{
  private readonly Type _type;

  public NestedTypeTracker(Type type) => this._type = type;

  public override Type DeclaringType => this._type.DeclaringType;

  public override TrackerTypes MemberType => TrackerTypes.Type;

  public override string Name => this._type.Name;

  public override bool IsPublic => this._type.IsPublic();

  public override Type Type => this._type;

  public override bool IsGenericType => this._type.IsGenericType();

  public override string ToString() => this._type.ToString();
}
