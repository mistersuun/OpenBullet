// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.ExtensionPropertyInfo
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Reflection;

#nullable disable
namespace IronPython.Runtime.Types;

public class ExtensionPropertyInfo
{
  private MethodInfo _getter;
  private MethodInfo _setter;
  private MethodInfo _deleter;
  private Type _declaringType;

  public ExtensionPropertyInfo(Type logicalDeclaringType, MethodInfo mi)
  {
    this._declaringType = logicalDeclaringType;
    string methodName = mi.Name;
    string prefix = "";
    if (methodName.StartsWith("#base#"))
    {
      methodName = methodName.Substring("#base#".Length);
      prefix = "#base#";
    }
    if (methodName.StartsWith("Get") || methodName.StartsWith("Set"))
      this.GetPropertyMethods(mi, methodName, prefix, "Get", "Set", "Delete");
    else if (methodName.StartsWith("get_") || methodName.StartsWith("set_"))
    {
      this.GetPropertyMethods(mi, methodName, prefix, "get_", "set_", (string) null);
    }
    else
    {
      if (!methodName.StartsWith("#field_get#") && !methodName.StartsWith("#field_set#"))
        return;
      this.GetPropertyMethods(mi, methodName, prefix, "#field_get#", "#field_set#", (string) null);
    }
  }

  private void GetPropertyMethods(
    MethodInfo mi,
    string methodName,
    string prefix,
    string get,
    string set,
    string delete)
  {
    string str = methodName.Substring(get.Length);
    if (string.Compare(mi.Name, 0, get, 0, get.Length) == 0)
    {
      this._getter = mi;
      this._setter = mi.DeclaringType.GetMethod(prefix + set + str);
    }
    else
    {
      this._getter = mi.DeclaringType.GetMethod(prefix + get + str);
      this._setter = mi;
    }
    if (delete == null)
      return;
    this._deleter = mi.DeclaringType.GetMethod(prefix + delete + str);
  }

  public MethodInfo Getter => this._getter;

  public MethodInfo Setter => this._setter;

  public MethodInfo Deleter => this._deleter;

  public Type DeclaringType => this._declaringType;

  public string Name
  {
    get
    {
      return this._getter != (MethodInfo) null ? this._getter.Name.Substring(3) : this._setter.Name.Substring(3);
    }
  }
}
