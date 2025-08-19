// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.ConstructorTracker
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class ConstructorTracker : MemberTracker
{
  private readonly ConstructorInfo _ctor;

  public ConstructorTracker(ConstructorInfo ctor) => this._ctor = ctor;

  public override Type DeclaringType => this._ctor.DeclaringType;

  public override TrackerTypes MemberType => TrackerTypes.Constructor;

  public override string Name => this._ctor.Name;

  public bool IsPublic => this._ctor.IsPublic;

  public override string ToString() => this._ctor.ToString();
}
