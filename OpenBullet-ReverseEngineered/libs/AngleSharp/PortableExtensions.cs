// Decompiled with JetBrains decompiler
// Type: AngleSharp.PortableExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;
using System.Linq;
using System.Reflection;

#nullable disable
namespace AngleSharp;

internal static class PortableExtensions
{
  public static bool Implements<T>(this Type type)
  {
    return IntrospectionExtensions.GetTypeInfo(type).ImplementedInterfaces.Contains<Type>(typeof (T));
  }

  public static bool IsAbstractClass(this Type type)
  {
    return ((Type) IntrospectionExtensions.GetTypeInfo(type)).IsAbstract;
  }

  public static Assembly GetAssembly(this Type type)
  {
    return ((Type) IntrospectionExtensions.GetTypeInfo(type)).Assembly;
  }
}
