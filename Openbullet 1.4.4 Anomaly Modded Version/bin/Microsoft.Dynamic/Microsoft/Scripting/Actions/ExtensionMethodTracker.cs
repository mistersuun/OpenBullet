// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.ExtensionMethodTracker
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class ExtensionMethodTracker : MethodTracker
{
  internal ExtensionMethodTracker(MethodInfo method, bool isStatic, Type declaringType)
    : base(method, isStatic)
  {
    ContractUtils.RequiresNotNull((object) declaringType, nameof (declaringType));
    this.DeclaringType = declaringType;
  }

  public override Type DeclaringType { get; }
}
