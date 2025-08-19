// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.ExtensionPropertyTracker
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class ExtensionPropertyTracker : PropertyTracker
{
  private MethodInfo _getter;
  private MethodInfo _setter;
  private MethodInfo _deleter;

  public ExtensionPropertyTracker(
    string name,
    MethodInfo getter,
    MethodInfo setter,
    MethodInfo deleter,
    Type declaringType)
  {
    this.Name = name;
    this._getter = getter;
    this._setter = setter;
    this._deleter = deleter;
    this.DeclaringType = declaringType;
  }

  public override string Name { get; }

  public override Type DeclaringType { get; }

  public override bool IsStatic
  {
    get
    {
      MethodInfo method = this.GetGetMethod(true);
      if ((object) method == null)
        method = this.GetSetMethod(true);
      return ExtensionPropertyTracker.IsStaticProperty(method);
    }
  }

  public override MethodInfo GetGetMethod()
  {
    return this._getter != (MethodInfo) null && this._getter.IsPrivate ? (MethodInfo) null : this._getter;
  }

  public override MethodInfo GetSetMethod()
  {
    return this._setter != (MethodInfo) null && this._setter.IsPrivate ? (MethodInfo) null : this._setter;
  }

  public override MethodInfo GetGetMethod(bool privateMembers)
  {
    return privateMembers ? this._getter : this.GetGetMethod();
  }

  public override MethodInfo GetSetMethod(bool privateMembers)
  {
    return privateMembers ? this._setter : this.GetSetMethod();
  }

  public override MethodInfo GetDeleteMethod()
  {
    return this._deleter != (MethodInfo) null && this._deleter.IsPrivate ? (MethodInfo) null : this._deleter;
  }

  public override MethodInfo GetDeleteMethod(bool privateMembers)
  {
    return privateMembers ? this._deleter : this.GetDeleteMethod();
  }

  public override ParameterInfo[] GetIndexParameters() => new ParameterInfo[0];

  private static bool IsStaticProperty(MethodInfo method)
  {
    return method.IsDefined(typeof (StaticExtensionMethodAttribute), false);
  }

  public override Type PropertyType
  {
    get
    {
      if (this._getter != (MethodInfo) null)
        return this._getter.ReturnType;
      ParameterInfo[] parameters = this._setter.GetParameters();
      return parameters[parameters.Length - 1].ParameterType;
    }
  }
}
