// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.NameConverter
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using Microsoft.Scripting.Runtime;
using System;
using System.Reflection;
using System.Text;

#nullable disable
namespace IronPython.Runtime.Types;

public static class NameConverter
{
  public static NameType TryGetName(PythonType dt, MethodInfo mi, out string name)
  {
    name = mi.Name;
    return NameConverter.GetNameFromMethod(dt, mi, NameType.Method, ref name);
  }

  public static NameType TryGetName(
    PythonType dt,
    EventInfo ei,
    MethodInfo eventMethod,
    out string name)
  {
    name = ei.Name;
    NameType res = dt.IsPythonType ? NameType.PythonEvent : NameType.Event;
    return NameConverter.GetNameFromMethod(dt, eventMethod, res, ref name);
  }

  public static NameType TryGetName(
    PythonType dt,
    PropertyInfo pi,
    MethodInfo prop,
    out string name)
  {
    if (PythonHiddenAttribute.IsHidden((MemberInfo) pi))
    {
      name = (string) null;
      return NameType.None;
    }
    name = pi.Name;
    return NameConverter.GetNameFromMethod(dt, prop, NameType.Property, ref name);
  }

  public static string GetTypeName(Type t)
  {
    if (t.IsArray)
      return $"Array[{NameConverter.GetTypeName(t.GetElementType())}]";
    string typeNameInternal = PythonBinder.GetTypeNameInternal(t);
    int length;
    if (typeNameInternal != t.Name || (length = typeNameInternal.IndexOf('`')) == -1)
      return typeNameInternal;
    string str = typeNameInternal.Substring(0, length);
    Type[] genericArguments = t.GetGenericArguments();
    StringBuilder stringBuilder = new StringBuilder(str);
    stringBuilder.Append('[');
    bool flag = true;
    foreach (Type t1 in genericArguments)
    {
      if (flag)
        flag = false;
      else
        stringBuilder.Append(", ");
      stringBuilder.Append(NameConverter.GetTypeName(t1));
    }
    stringBuilder.Append(']');
    typeNameInternal = stringBuilder.ToString();
    return typeNameInternal;
  }

  internal static NameType GetNameFromMethod(
    PythonType dt,
    MethodInfo mi,
    NameType res,
    ref string name)
  {
    string str = (string) null;
    if (mi.IsPrivate || mi.IsAssembly && !mi.IsFamilyOrAssembly)
    {
      if (!mi.IsPrivate || !mi.IsFinal || !mi.IsHideBySig || !mi.IsVirtual)
      {
        str = $"_{dt.Name}__";
      }
      else
      {
        int num = name.LastIndexOf('.');
        if (num != -1)
          name = name.Substring(num + 1);
      }
    }
    if (mi.IsDefined(typeof (ClassMethodAttribute), false))
      res |= NameType.ClassMember;
    if (str != null)
      name = str + name;
    if (mi.DeclaringType.IsDefined(typeof (PythonTypeAttribute), false) || !mi.DeclaringType.IsAssignableFrom(dt.UnderlyingSystemType))
      res |= NameType.Python;
    if (mi.IsDefined(typeof (PropertyMethodAttribute), false))
      res = res & ~NameType.BaseTypeMask | NameType.Property;
    return res;
  }
}
