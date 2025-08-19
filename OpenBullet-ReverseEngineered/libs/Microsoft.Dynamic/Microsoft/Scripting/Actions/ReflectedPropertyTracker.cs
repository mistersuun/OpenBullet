// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.ReflectedPropertyTracker
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class ReflectedPropertyTracker : PropertyTracker
{
  private readonly PropertyInfo _propInfo;

  public ReflectedPropertyTracker(PropertyInfo property) => this._propInfo = property;

  public override string Name => this._propInfo.Name;

  public override Type DeclaringType => this._propInfo.DeclaringType;

  public override bool IsStatic
  {
    get
    {
      MethodInfo methodInfo = this.GetGetMethod(true);
      if ((object) methodInfo == null)
        methodInfo = this.GetSetMethod(true);
      return methodInfo.IsStatic;
    }
  }

  public override Type PropertyType => this._propInfo.PropertyType;

  public override MethodInfo GetGetMethod() => this._propInfo.GetGetMethod();

  public override MethodInfo GetSetMethod() => this._propInfo.GetSetMethod();

  public override MethodInfo GetGetMethod(bool privateMembers)
  {
    return this._propInfo.GetGetMethod(privateMembers);
  }

  public override MethodInfo GetSetMethod(bool privateMembers)
  {
    return this._propInfo.GetSetMethod(privateMembers);
  }

  public override MethodInfo GetDeleteMethod() => this.GetDeleteMethod(false);

  public override MethodInfo GetDeleteMethod(bool privateMembers)
  {
    MethodInfo method = this._propInfo.DeclaringType.GetMethod("Delete" + this._propInfo.Name, (BindingFlags) ((privateMembers ? 32 /*0x20*/ : 0) | 8 | 4 | 16 /*0x10*/));
    return method != (MethodInfo) null && method.IsSpecialName && method.IsDefined(typeof (PropertyMethodAttribute), true) ? method : (MethodInfo) null;
  }

  public override ParameterInfo[] GetIndexParameters() => this._propInfo.GetIndexParameters();

  public PropertyInfo Property => this._propInfo;

  public override string ToString() => this._propInfo.ToString();
}
