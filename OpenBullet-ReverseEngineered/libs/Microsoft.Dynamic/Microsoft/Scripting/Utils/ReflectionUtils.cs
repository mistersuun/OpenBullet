// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.ReflectionUtils
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Metadata;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Utils;

public static class ReflectionUtils
{
  public static BindingFlags AllMembers = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
  public static Type[] EmptyTypes = new Type[0];
  public const char GenericArityDelimiter = '`';
  private const MethodAttributes MethodAttributesToEraseInOveride = MethodAttributes.ReservedMask | MethodAttributes.Abstract;
  private static Dictionary<Assembly, Dictionary<string, List<ExtensionMethodInfo>>> _extensionMethodsCache;

  public static bool IsPublic(this PropertyInfo property)
  {
    return property.GetGetMethod(false) != (MethodInfo) null || property.GetSetMethod(false) != (MethodInfo) null;
  }

  public static bool IsStatic(this PropertyInfo property)
  {
    MethodInfo getMethod = property.GetGetMethod(true);
    MethodInfo setMethod = property.GetSetMethod(true);
    if (getMethod != (MethodInfo) null && getMethod.IsStatic)
      return true;
    return setMethod != (MethodInfo) null && setMethod.IsStatic;
  }

  public static bool IsStatic(this EventInfo evnt)
  {
    MethodInfo addMethod = evnt.GetAddMethod(true);
    MethodInfo removeMethod = evnt.GetRemoveMethod(true);
    if (addMethod != (MethodInfo) null && addMethod.IsStatic)
      return true;
    return removeMethod != (MethodInfo) null && removeMethod.IsStatic;
  }

  public static bool IsPrivate(this PropertyInfo property)
  {
    MethodInfo getMethod = property.GetGetMethod(true);
    MethodInfo setMethod = property.GetSetMethod(true);
    if (!(getMethod == (MethodInfo) null) && !getMethod.IsPrivate)
      return false;
    return setMethod == (MethodInfo) null || setMethod.IsPrivate;
  }

  public static bool IsPrivate(this EventInfo evnt)
  {
    MethodInfo addMethod = evnt.GetAddMethod(true);
    MethodInfo removeMethod = evnt.GetRemoveMethod(true);
    if (!(addMethod == (MethodInfo) null) && !addMethod.IsPrivate)
      return false;
    return removeMethod == (MethodInfo) null || removeMethod.IsPrivate;
  }

  private static bool MatchesFlags(ConstructorInfo member, BindingFlags flags)
  {
    return ((member.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic) & flags) != BindingFlags.Default && ((member.IsStatic ? BindingFlags.Static : BindingFlags.Instance) & flags) != 0;
  }

  private static bool MatchesFlags(MethodInfo member, BindingFlags flags)
  {
    return ((member.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic) & flags) != BindingFlags.Default && ((member.IsStatic ? BindingFlags.Static : BindingFlags.Instance) & flags) != 0;
  }

  private static bool MatchesFlags(FieldInfo member, BindingFlags flags)
  {
    return ((member.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic) & flags) != BindingFlags.Default && ((member.IsStatic ? BindingFlags.Static : BindingFlags.Instance) & flags) != 0;
  }

  private static bool MatchesFlags(PropertyInfo member, BindingFlags flags)
  {
    return ((member.IsPublic() ? BindingFlags.Public : BindingFlags.NonPublic) & flags) != BindingFlags.Default && ((member.IsStatic() ? BindingFlags.Static : BindingFlags.Instance) & flags) != 0;
  }

  private static bool MatchesFlags(EventInfo member, BindingFlags flags)
  {
    MethodInfo addMethod = member.GetAddMethod();
    MethodInfo removeMethod = member.GetRemoveMethod();
    MethodInfo raiseMethod = member.GetRaiseMethod();
    int num = addMethod != (MethodInfo) null && addMethod.IsPublic || removeMethod != (MethodInfo) null && removeMethod.IsPublic ? 1 : (!(raiseMethod != (MethodInfo) null) ? 0 : (raiseMethod.IsPublic ? 1 : 0));
    bool flag = addMethod != (MethodInfo) null && addMethod.IsStatic || removeMethod != (MethodInfo) null && removeMethod.IsStatic || raiseMethod != (MethodInfo) null && raiseMethod.IsStatic;
    return ((num != 0 ? BindingFlags.Public : BindingFlags.NonPublic) & flags) != BindingFlags.Default && ((flag ? BindingFlags.Static : BindingFlags.Instance) & flags) != 0;
  }

  private static bool MatchesFlags(Type member, BindingFlags flags)
  {
    return ((member.IsPublic || member.IsNestedPublic ? BindingFlags.Public : BindingFlags.NonPublic) & flags) != 0;
  }

  private static bool MatchesFlags(MemberInfo member, BindingFlags flags)
  {
    MethodInfo member1;
    if ((member1 = member as MethodInfo) != (MethodInfo) null)
      return ReflectionUtils.MatchesFlags(member1, flags);
    FieldInfo member2;
    if ((member2 = member as FieldInfo) != (FieldInfo) null)
      return ReflectionUtils.MatchesFlags(member2, flags);
    ConstructorInfo member3;
    if ((member3 = member as ConstructorInfo) != (ConstructorInfo) null)
      return ReflectionUtils.MatchesFlags(member3, flags);
    EventInfo member4;
    if ((member4 = member as EventInfo) != (EventInfo) null)
      return ReflectionUtils.MatchesFlags(member4, flags);
    PropertyInfo member5;
    return (member5 = member as PropertyInfo) != (PropertyInfo) null ? ReflectionUtils.MatchesFlags(member5, flags) : ReflectionUtils.MatchesFlags((Type) member, flags);
  }

  private static IEnumerable<T> WithBindingFlags<T>(
    this IEnumerable<T> members,
    Func<T, BindingFlags, bool> matchFlags,
    BindingFlags flags)
    where T : MemberInfo
  {
    return members.Where<T>((Func<T, bool>) (member => matchFlags(member, flags)));
  }

  public static IEnumerable<MemberInfo> WithBindingFlags(
    this IEnumerable<MemberInfo> members,
    BindingFlags flags)
  {
    return members.WithBindingFlags<MemberInfo>(new Func<MemberInfo, BindingFlags, bool>(ReflectionUtils.MatchesFlags), flags);
  }

  public static IEnumerable<MethodInfo> WithBindingFlags(
    this IEnumerable<MethodInfo> members,
    BindingFlags flags)
  {
    return members.WithBindingFlags<MethodInfo>(new Func<MethodInfo, BindingFlags, bool>(ReflectionUtils.MatchesFlags), flags);
  }

  public static IEnumerable<ConstructorInfo> WithBindingFlags(
    this IEnumerable<ConstructorInfo> members,
    BindingFlags flags)
  {
    return members.WithBindingFlags<ConstructorInfo>(new Func<ConstructorInfo, BindingFlags, bool>(ReflectionUtils.MatchesFlags), flags);
  }

  public static IEnumerable<FieldInfo> WithBindingFlags(
    this IEnumerable<FieldInfo> members,
    BindingFlags flags)
  {
    return members.WithBindingFlags<FieldInfo>(new Func<FieldInfo, BindingFlags, bool>(ReflectionUtils.MatchesFlags), flags);
  }

  public static IEnumerable<PropertyInfo> WithBindingFlags(
    this IEnumerable<PropertyInfo> members,
    BindingFlags flags)
  {
    return members.WithBindingFlags<PropertyInfo>(new Func<PropertyInfo, BindingFlags, bool>(ReflectionUtils.MatchesFlags), flags);
  }

  public static IEnumerable<EventInfo> WithBindingFlags(
    this IEnumerable<EventInfo> members,
    BindingFlags flags)
  {
    return members.WithBindingFlags<EventInfo>(new Func<EventInfo, BindingFlags, bool>(ReflectionUtils.MatchesFlags), flags);
  }

  public static IEnumerable<Type> WithBindingFlags(
    this IEnumerable<Type> members,
    BindingFlags flags)
  {
    return members.WithBindingFlags<Type>(new Func<Type, BindingFlags, bool>(ReflectionUtils.MatchesFlags), flags);
  }

  public static MemberInfo WithBindingFlags(this MemberInfo member, BindingFlags flags)
  {
    return !(member != (MemberInfo) null) || !ReflectionUtils.MatchesFlags(member, flags) ? (MemberInfo) null : member;
  }

  public static MethodInfo WithBindingFlags(this MethodInfo member, BindingFlags flags)
  {
    return !(member != (MethodInfo) null) || !ReflectionUtils.MatchesFlags(member, flags) ? (MethodInfo) null : member;
  }

  public static ConstructorInfo WithBindingFlags(this ConstructorInfo member, BindingFlags flags)
  {
    return !(member != (ConstructorInfo) null) || !ReflectionUtils.MatchesFlags(member, flags) ? (ConstructorInfo) null : member;
  }

  public static FieldInfo WithBindingFlags(this FieldInfo member, BindingFlags flags)
  {
    return !(member != (FieldInfo) null) || !ReflectionUtils.MatchesFlags(member, flags) ? (FieldInfo) null : member;
  }

  public static PropertyInfo WithBindingFlags(this PropertyInfo member, BindingFlags flags)
  {
    return !(member != (PropertyInfo) null) || !ReflectionUtils.MatchesFlags(member, flags) ? (PropertyInfo) null : member;
  }

  public static EventInfo WithBindingFlags(this EventInfo member, BindingFlags flags)
  {
    return !(member != (EventInfo) null) || !ReflectionUtils.MatchesFlags(member, flags) ? (EventInfo) null : member;
  }

  public static Type WithBindingFlags(this Type member, BindingFlags flags)
  {
    return !(member != (Type) null) || !ReflectionUtils.MatchesFlags(member, flags) ? (Type) null : member;
  }

  public static IEnumerable<MethodInfo> WithSignature(
    this IEnumerable<MethodInfo> members,
    Type[] parameterTypes)
  {
    return members.Where<MethodInfo>((Func<MethodInfo, bool>) (c =>
    {
      ParameterInfo[] parameters = c.GetParameters();
      if (parameters.Length != parameterTypes.Length)
        return false;
      for (int index = 0; index < parameters.Length; ++index)
      {
        if (parameterTypes[index] != parameters[index].ParameterType)
          return false;
      }
      return true;
    }));
  }

  public static IEnumerable<ConstructorInfo> WithSignature(
    this IEnumerable<ConstructorInfo> members,
    Type[] parameterTypes)
  {
    return members.Where<ConstructorInfo>((Func<ConstructorInfo, bool>) (c =>
    {
      ParameterInfo[] parameters = c.GetParameters();
      if (parameters.Length != parameterTypes.Length)
        return false;
      for (int index = 0; index < parameters.Length; ++index)
      {
        if (parameterTypes[index] != parameters[index].ParameterType)
          return false;
      }
      return true;
    }));
  }

  public static IEnumerable<MethodInfo> GetInheritedMethods(
    this Type type,
    string name = null,
    bool flattenHierarchy = false)
  {
    while (type.IsGenericParameter)
      type = type.GetBaseType();
    HashSet<MethodInfo> baseDefinitions = new HashSet<MethodInfo>((IEqualityComparer<MethodInfo>) ReferenceEqualityComparer<MethodInfo>.Instance);
    foreach (Type ancestor in type.Ancestors())
    {
      foreach (MethodInfo declaredMethod in ReflectionUtils.GetDeclaredMethods(ancestor, name))
      {
        if (declaredMethod != (MethodInfo) null && ReflectionUtils.IncludeMethod(declaredMethod, type, baseDefinitions, flattenHierarchy))
          yield return declaredMethod;
      }
    }
  }

  private static bool IncludeMethod(
    MethodInfo member,
    Type reflectedType,
    HashSet<MethodInfo> baseDefinitions,
    bool flattenHierarchy)
  {
    if (member.IsVirtual)
    {
      if (baseDefinitions.Add(RuntimeReflectionExtensions.GetRuntimeBaseDefinition(member)))
        return true;
    }
    else if (member.DeclaringType == reflectedType || !member.IsPrivate && !member.IsStatic | flattenHierarchy)
      return true;
    return false;
  }

  public static IEnumerable<PropertyInfo> GetInheritedProperties(
    this Type type,
    string name = null,
    bool flattenHierarchy = false)
  {
    while (type.IsGenericParameter)
      type = type.GetBaseType();
    HashSet<MethodInfo> baseDefinitions = new HashSet<MethodInfo>((IEqualityComparer<MethodInfo>) ReferenceEqualityComparer<MethodInfo>.Instance);
    foreach (Type ancestor in type.Ancestors())
    {
      if (name != null)
      {
        PropertyInfo declaredProperty = ancestor.GetDeclaredProperty(name);
        if (declaredProperty != (PropertyInfo) null && ReflectionUtils.IncludeProperty(declaredProperty, type, baseDefinitions, flattenHierarchy))
          yield return declaredProperty;
      }
      else
      {
        foreach (PropertyInfo declaredProperty in ancestor.GetDeclaredProperties())
        {
          if (ReflectionUtils.IncludeProperty(declaredProperty, type, baseDefinitions, flattenHierarchy))
            yield return declaredProperty;
        }
      }
    }
  }

  private static bool IncludeProperty(
    PropertyInfo member,
    Type reflectedType,
    HashSet<MethodInfo> baseDefinitions,
    bool flattenHierarchy)
  {
    MethodInfo getMethod = member.GetGetMethod(true);
    MethodInfo setMethod = member.GetSetMethod(true);
    MethodInfo methodInfo = !(getMethod != (MethodInfo) null) || !getMethod.IsVirtual ? (!(setMethod != (MethodInfo) null) || !setMethod.IsVirtual ? (MethodInfo) null : setMethod) : getMethod;
    if (methodInfo != (MethodInfo) null)
    {
      if (baseDefinitions.Add(RuntimeReflectionExtensions.GetRuntimeBaseDefinition(methodInfo)))
        return true;
    }
    else if (member.DeclaringType == reflectedType || !member.IsPrivate() && !member.IsStatic() | flattenHierarchy)
      return true;
    return false;
  }

  public static IEnumerable<EventInfo> GetInheritedEvents(
    this Type type,
    string name = null,
    bool flattenHierarchy = false)
  {
    while (type.IsGenericParameter)
      type = type.GetBaseType();
    HashSet<MethodInfo> baseDefinitions = new HashSet<MethodInfo>((IEqualityComparer<MethodInfo>) ReferenceEqualityComparer<MethodInfo>.Instance);
    foreach (Type ancestor in type.Ancestors())
    {
      if (name != null)
      {
        EventInfo declaredEvent = ancestor.GetDeclaredEvent(name);
        if (declaredEvent != (EventInfo) null && ReflectionUtils.IncludeEvent(declaredEvent, type, baseDefinitions, flattenHierarchy))
          yield return declaredEvent;
      }
      else
      {
        foreach (EventInfo declaredEvent in ancestor.GetDeclaredEvents())
        {
          if (ReflectionUtils.IncludeEvent(declaredEvent, type, baseDefinitions, flattenHierarchy))
            yield return declaredEvent;
        }
      }
    }
  }

  private static bool IncludeEvent(
    EventInfo member,
    Type reflectedType,
    HashSet<MethodInfo> baseDefinitions,
    bool flattenHierarchy)
  {
    MethodInfo addMethod = member.GetAddMethod(true);
    MethodInfo removeMethod = member.GetRemoveMethod(true);
    MethodInfo methodInfo = !(addMethod != (MethodInfo) null) || !addMethod.IsVirtual ? (!(removeMethod != (MethodInfo) null) || !removeMethod.IsVirtual ? (MethodInfo) null : removeMethod) : addMethod;
    if (methodInfo != (MethodInfo) null)
    {
      if (baseDefinitions.Add(RuntimeReflectionExtensions.GetRuntimeBaseDefinition(methodInfo)))
        return true;
    }
    else if (member.DeclaringType == reflectedType || !member.IsPrivate() && !member.IsStatic() | flattenHierarchy)
      return true;
    return false;
  }

  public static IEnumerable<FieldInfo> GetInheritedFields(
    this Type type,
    string name = null,
    bool flattenHierarchy = false)
  {
    while (type.IsGenericParameter)
      type = type.GetBaseType();
    foreach (Type ancestor in type.Ancestors())
    {
      if (name != null)
      {
        FieldInfo declaredField = ancestor.GetDeclaredField(name);
        if (declaredField != (FieldInfo) null && ReflectionUtils.IncludeField(declaredField, type, flattenHierarchy))
          yield return declaredField;
      }
      else
      {
        foreach (FieldInfo declaredField in ancestor.GetDeclaredFields())
        {
          if (ReflectionUtils.IncludeField(declaredField, type, flattenHierarchy))
            yield return declaredField;
        }
      }
    }
  }

  private static bool IncludeField(FieldInfo member, Type reflectedType, bool flattenHierarchy)
  {
    return member.DeclaringType == reflectedType || !member.IsPrivate && !member.IsStatic | flattenHierarchy;
  }

  public static IEnumerable<MemberInfo> GetInheritedMembers(
    this Type type,
    string name = null,
    bool flattenHierarchy = false)
  {
    IEnumerable<MemberInfo> first = type.GetInheritedMethods(name, flattenHierarchy).Cast<MethodInfo, MemberInfo>().Concat<MemberInfo>(type.GetInheritedProperties(name, flattenHierarchy).Cast<PropertyInfo, MemberInfo>().Concat<MemberInfo>(type.GetInheritedEvents(name, flattenHierarchy).Cast<EventInfo, MemberInfo>().Concat<MemberInfo>(type.GetInheritedFields(name, flattenHierarchy).Cast<FieldInfo, MemberInfo>())));
    if (name == null)
      return first.Concat<MemberInfo>(type.GetDeclaredConstructors().Cast<ConstructorInfo, MemberInfo>().Concat<MemberInfo>(type.GetDeclaredNestedTypes().Cast<Type, MemberInfo>()));
    Type declaredNestedType = type.GetDeclaredNestedType(name);
    if (!(declaredNestedType != (Type) null))
      return first;
    return first.Concat<MemberInfo>((IEnumerable<MemberInfo>) new Type[1]
    {
      declaredNestedType
    });
  }

  public static IEnumerable<ConstructorInfo> GetDeclaredConstructors(this Type type)
  {
    return (IEnumerable<ConstructorInfo>) type.GetConstructors(BindingFlags.DeclaredOnly | ReflectionUtils.AllMembers);
  }

  public static IEnumerable<MethodInfo> GetDeclaredMethods(this Type type, string name = null)
  {
    return name == null ? (IEnumerable<MethodInfo>) type.GetMethods(BindingFlags.DeclaredOnly | ReflectionUtils.AllMembers) : type.GetMember(name, MemberTypes.Method, BindingFlags.DeclaredOnly | ReflectionUtils.AllMembers).OfType<MethodInfo>();
  }

  public static IEnumerable<PropertyInfo> GetDeclaredProperties(this Type type)
  {
    return (IEnumerable<PropertyInfo>) type.GetProperties(BindingFlags.DeclaredOnly | ReflectionUtils.AllMembers);
  }

  public static PropertyInfo GetDeclaredProperty(this Type type, string name)
  {
    return type.GetProperty(name, BindingFlags.DeclaredOnly | ReflectionUtils.AllMembers);
  }

  public static IEnumerable<EventInfo> GetDeclaredEvents(this Type type)
  {
    return (IEnumerable<EventInfo>) type.GetEvents(BindingFlags.DeclaredOnly | ReflectionUtils.AllMembers);
  }

  public static EventInfo GetDeclaredEvent(this Type type, string name)
  {
    return type.GetEvent(name, BindingFlags.DeclaredOnly | ReflectionUtils.AllMembers);
  }

  public static IEnumerable<FieldInfo> GetDeclaredFields(this Type type)
  {
    return (IEnumerable<FieldInfo>) type.GetFields(BindingFlags.DeclaredOnly | ReflectionUtils.AllMembers);
  }

  public static FieldInfo GetDeclaredField(this Type type, string name)
  {
    return type.GetField(name, BindingFlags.DeclaredOnly | ReflectionUtils.AllMembers);
  }

  public static IEnumerable<Type> GetDeclaredNestedTypes(this Type type)
  {
    return (IEnumerable<Type>) type.GetNestedTypes(BindingFlags.DeclaredOnly | ReflectionUtils.AllMembers);
  }

  public static Type GetDeclaredNestedType(this Type type, string name)
  {
    return type.GetNestedType(name, BindingFlags.DeclaredOnly | ReflectionUtils.AllMembers);
  }

  public static IEnumerable<MemberInfo> GetDeclaredMembers(this Type type, string name = null)
  {
    return name == null ? (IEnumerable<MemberInfo>) type.GetMembers(BindingFlags.DeclaredOnly | ReflectionUtils.AllMembers) : (IEnumerable<MemberInfo>) type.GetMember(name, BindingFlags.DeclaredOnly | ReflectionUtils.AllMembers);
  }

  public static Type[] GetGenericTypeArguments(this Type type)
  {
    return !type.IsGenericType || type.IsGenericTypeDefinition ? (Type[]) null : type.GetGenericArguments();
  }

  public static Type[] GetGenericTypeParameters(this Type type)
  {
    return !type.IsGenericTypeDefinition ? (Type[]) null : type.GetGenericArguments();
  }

  public static IEnumerable<Module> GetModules(this Assembly assembly)
  {
    return (IEnumerable<Module>) assembly.GetModules();
  }

  public static IEnumerable<Type> GetImplementedInterfaces(this Type type)
  {
    return (IEnumerable<Type>) type.GetInterfaces();
  }

  public static TypeCode GetTypeCode(this Type type) => Type.GetTypeCode(type);

  public static MethodInfo GetMethod(this Delegate d)
  {
    return RuntimeReflectionExtensions.GetMethodInfo(d);
  }

  public static bool IsDefined(this Assembly assembly, Type attributeType)
  {
    return assembly.IsDefined(attributeType, false);
  }

  public static T GetCustomAttribute<T>(this Assembly assembly, bool inherit = false) where T : Attribute
  {
    return (T) Attribute.GetCustomAttribute(assembly, typeof (T), inherit);
  }

  public static T GetCustomAttribute<T>(this MemberInfo member, bool inherit = false) where T : Attribute
  {
    return (T) Attribute.GetCustomAttribute(member, typeof (T), inherit);
  }

  public static bool ContainsGenericParameters(this Type type) => type.ContainsGenericParameters;

  public static bool IsInterface(this Type type) => type.IsInterface;

  public static bool IsClass(this Type type) => type.IsClass;

  public static bool IsGenericType(this Type type) => type.IsGenericType;

  public static bool IsGenericTypeDefinition(this Type type) => type.IsGenericTypeDefinition;

  public static bool IsSealed(this Type type) => type.IsSealed;

  public static bool IsAbstract(this Type type) => type.IsAbstract;

  public static bool IsPublic(this Type type) => type.IsPublic;

  public static bool IsVisible(this Type type) => type.IsVisible;

  public static Type GetBaseType(this Type type) => type.BaseType;

  public static bool IsValueType(this Type type) => type.IsValueType;

  public static bool IsEnum(this Type type) => type.IsEnum;

  public static bool IsPrimitive(this Type type) => type.IsPrimitive;

  public static GenericParameterAttributes GetGenericParameterAttributes(this Type type)
  {
    return type.GenericParameterAttributes;
  }

  public static object GetRawConstantValue(this FieldInfo field)
  {
    object obj = field.IsLiteral ? field.GetValue((object) null) : throw new ArgumentException(field.ToString() + " not a literal.");
    return !field.FieldType.IsEnum() ? obj : ReflectionUtils.UnwrapEnumValue(obj);
  }

  public static object UnwrapEnumValue(object value)
  {
    if (value == null)
      throw new ArgumentNullException(nameof (value));
    switch (value.GetType().GetTypeCode())
    {
      case TypeCode.SByte:
        return (object) Convert.ToSByte(value);
      case TypeCode.Byte:
        return (object) Convert.ToByte(value);
      case TypeCode.Int16:
        return (object) Convert.ToInt16(value);
      case TypeCode.UInt16:
        return (object) Convert.ToUInt16(value);
      case TypeCode.Int32:
        return (object) Convert.ToInt32(value);
      case TypeCode.UInt32:
        return (object) Convert.ToUInt32(value);
      case TypeCode.Int64:
        return (object) Convert.ToInt64(value);
      case TypeCode.UInt64:
        return (object) Convert.ToUInt64(value);
      default:
        throw new ArgumentException("Value must be a boxed enum.", nameof (value));
    }
  }

  public static AssemblyBuilder DefineDynamicAssembly(
    AssemblyName name,
    AssemblyBuilderAccess access)
  {
    return AssemblyBuilder.DefineDynamicAssembly(name, access);
  }

  public static StringBuilder FormatSignature(StringBuilder result, MethodBase method)
  {
    return ReflectionUtils.FormatSignature(result, method, (Func<Type, string>) (t => t.FullName));
  }

  public static StringBuilder FormatSignature(
    StringBuilder result,
    MethodBase method,
    Func<Type, string> nameDispenser)
  {
    ContractUtils.RequiresNotNull((object) result, nameof (result));
    ContractUtils.RequiresNotNull((object) method, nameof (method));
    ContractUtils.RequiresNotNull((object) nameDispenser, nameof (nameDispenser));
    MethodInfo methodInfo = method as MethodInfo;
    if (methodInfo != (MethodInfo) null)
    {
      ReflectionUtils.FormatTypeName(result, methodInfo.ReturnType, nameDispenser);
      result.Append(' ');
    }
    MethodBuilder methodBuilder = method as MethodBuilder;
    if ((MethodInfo) methodBuilder != (MethodInfo) null)
    {
      result.Append(methodBuilder.Signature);
      return result;
    }
    ConstructorBuilder constructorBuilder = method as ConstructorBuilder;
    if ((ConstructorInfo) constructorBuilder != (ConstructorInfo) null)
    {
      result.Append(constructorBuilder.Signature);
      return result;
    }
    ReflectionUtils.FormatTypeName(result, method.DeclaringType, nameDispenser);
    result.Append("::");
    result.Append(method.Name);
    if (!method.IsConstructor)
      ReflectionUtils.FormatTypeArgs(result, method.GetGenericArguments(), nameDispenser);
    result.Append("(");
    if (!method.ContainsGenericParameters)
    {
      ParameterInfo[] parameters = method.GetParameters();
      for (int index = 0; index < parameters.Length; ++index)
      {
        if (index > 0)
          result.Append(", ");
        ReflectionUtils.FormatTypeName(result, parameters[index].ParameterType, nameDispenser);
        if (!string.IsNullOrEmpty(parameters[index].Name))
        {
          result.Append(" ");
          result.Append(parameters[index].Name);
        }
      }
    }
    else
      result.Append("?");
    result.Append(")");
    return result;
  }

  public static StringBuilder FormatTypeName(StringBuilder result, Type type)
  {
    return ReflectionUtils.FormatTypeName(result, type, (Func<Type, string>) (t => t.FullName));
  }

  public static StringBuilder FormatTypeName(
    StringBuilder result,
    Type type,
    Func<Type, string> nameDispenser)
  {
    ContractUtils.RequiresNotNull((object) result, nameof (result));
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    ContractUtils.RequiresNotNull((object) nameDispenser, nameof (nameDispenser));
    if (type.IsGenericType())
    {
      Type genericTypeDefinition = type.GetGenericTypeDefinition();
      string str = nameDispenser(genericTypeDefinition).Replace('+', '.');
      int length = str.IndexOf('`');
      result.Append(length != -1 ? str.Substring(0, length) : str);
      Type[] genericArguments = type.GetGenericArguments();
      if (type.IsGenericTypeDefinition())
      {
        result.Append('<');
        result.Append(',', genericArguments.Length - 1);
        result.Append('>');
      }
      else
        ReflectionUtils.FormatTypeArgs(result, genericArguments, nameDispenser);
    }
    else if (type.IsGenericParameter)
      result.Append(type.Name);
    else
      result.Append(nameDispenser(type).Replace('+', '.'));
    return result;
  }

  public static StringBuilder FormatTypeArgs(StringBuilder result, Type[] types)
  {
    return ReflectionUtils.FormatTypeArgs(result, types, (Func<Type, string>) (t => t.FullName));
  }

  public static StringBuilder FormatTypeArgs(
    StringBuilder result,
    Type[] types,
    Func<Type, string> nameDispenser)
  {
    ContractUtils.RequiresNotNull((object) result, nameof (result));
    ContractUtils.RequiresNotNullItems<Type>((IList<Type>) types, nameof (types));
    ContractUtils.RequiresNotNull((object) nameDispenser, nameof (nameDispenser));
    if (types.Length != 0)
    {
      result.Append("<");
      for (int index = 0; index < types.Length; ++index)
      {
        if (index > 0)
          result.Append(", ");
        ReflectionUtils.FormatTypeName(result, types[index], nameDispenser);
      }
      result.Append(">");
    }
    return result;
  }

  internal static string ToValidTypeName(string str)
  {
    if (string.IsNullOrEmpty(str))
      return "_";
    StringBuilder stringBuilder = new StringBuilder(str);
    for (int index = 0; index < str.Length; ++index)
    {
      if (str[index] == char.MinValue || str[index] == '.' || str[index] == '*' || str[index] == '+' || str[index] == '[' || str[index] == ']' || str[index] == '\\')
        stringBuilder[index] = '_';
    }
    return stringBuilder.ToString();
  }

  public static string GetNormalizedTypeName(Type type)
  {
    string name = type.Name;
    return type.IsGenericType() ? ReflectionUtils.GetNormalizedTypeName(name) : name;
  }

  public static string GetNormalizedTypeName(string typeName)
  {
    int length = typeName.IndexOf('`');
    return length != -1 ? typeName.Substring(0, length) : typeName;
  }

  public static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType)
  {
    return methodInfo.CreateDelegate(delegateType, (object) null);
  }

  public static Delegate CreateDelegate(
    this MethodInfo methodInfo,
    Type delegateType,
    object target)
  {
    return methodInfo is DynamicMethod dynamicMethod ? ((MethodInfo) dynamicMethod).CreateDelegate(delegateType, target) : Delegate.CreateDelegate(delegateType, target, methodInfo);
  }

  public static bool IsDynamicMethod(MethodBase method)
  {
    return ReflectionUtils.IsDynamicMethodInternal(method);
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static bool IsDynamicMethodInternal(MethodBase method) => method is DynamicMethod;

  public static void GetDelegateSignature(
    Type delegateType,
    out ParameterInfo[] parameterInfos,
    out ParameterInfo returnInfo)
  {
    ContractUtils.RequiresNotNull((object) delegateType, nameof (delegateType));
    MethodInfo method = delegateType.GetMethod("Invoke");
    ContractUtils.Requires(method != (MethodInfo) null, nameof (delegateType), Microsoft.Scripting.Strings.InvalidDelegate);
    parameterInfos = method.GetParameters();
    returnInfo = method.ReturnParameter;
  }

  public static Type GetObjectCallSiteDelegateType(int paramCnt)
  {
    switch (paramCnt)
    {
      case 0:
        return typeof (Func<CallSite, object, object>);
      case 1:
        return typeof (Func<CallSite, object, object, object>);
      case 2:
        return typeof (Func<CallSite, object, object, object, object>);
      case 3:
        return typeof (Func<CallSite, object, object, object, object, object>);
      case 4:
        return typeof (Func<CallSite, object, object, object, object, object, object>);
      case 5:
        return typeof (Func<CallSite, object, object, object, object, object, object, object>);
      case 6:
        return typeof (Func<CallSite, object, object, object, object, object, object, object, object>);
      case 7:
        return typeof (Func<CallSite, object, object, object, object, object, object, object, object, object>);
      case 8:
        return typeof (Func<CallSite, object, object, object, object, object, object, object, object, object, object>);
      case 9:
        return typeof (Func<CallSite, object, object, object, object, object, object, object, object, object, object, object>);
      case 10:
        return typeof (Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object>);
      case 11:
        return typeof (Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object>);
      case 12:
        return typeof (Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object, object>);
      case 13:
        return typeof (Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>);
      case 14:
        return typeof (Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>);
      default:
        Type[] typeArray = new Type[paramCnt + 2];
        typeArray[0] = typeof (CallSite);
        typeArray[1] = typeof (object);
        for (int index = 0; index < paramCnt; ++index)
          typeArray[index + 2] = typeof (object);
        return Snippets.Shared.DefineDelegate("InvokeDelegate" + (object) paramCnt, typeof (object), typeArray);
    }
  }

  internal static DynamicMethod RawCreateDynamicMethod(
    string name,
    Type returnType,
    Type[] parameterTypes)
  {
    return new DynamicMethod(name, returnType, parameterTypes, true);
  }

  public static MethodBase[] GetMethodInfos(MemberInfo[] members)
  {
    return ArrayUtils.ConvertAll<MemberInfo, MethodBase>(members, (Func<MemberInfo, MethodBase>) (inp => (MethodBase) inp));
  }

  public static Type[] GetParameterTypes(ParameterInfo[] parameterInfos)
  {
    return ReflectionUtils.GetParameterTypes((IList<ParameterInfo>) parameterInfos);
  }

  public static Type[] GetParameterTypes(IList<ParameterInfo> parameterInfos)
  {
    Type[] parameterTypes = new Type[parameterInfos.Count];
    for (int index = 0; index < parameterTypes.Length; ++index)
      parameterTypes[index] = parameterInfos[index].ParameterType;
    return parameterTypes;
  }

  public static Type GetReturnType(this MethodBase mi)
  {
    return !mi.IsConstructor ? ((MethodInfo) mi).ReturnType : mi.DeclaringType;
  }

  public static bool SignatureEquals(MethodInfo method, params Type[] requiredSignature)
  {
    ContractUtils.RequiresNotNull((object) method, nameof (method));
    Type[] parameterTypes = ReflectionUtils.GetParameterTypes(method.GetParameters());
    int index;
    for (index = 0; index < parameterTypes.Length; ++index)
    {
      if (parameterTypes[index] != requiredSignature[index])
        return false;
    }
    return method.ReturnType == requiredSignature[index];
  }

  public static bool IsExtension(this MemberInfo member)
  {
    Type attributeType = typeof (ExtensionAttribute);
    return member.IsDefined(attributeType, false);
  }

  public static bool IsOutParameter(this ParameterInfo pi)
  {
    return pi.ParameterType.IsByRef && (pi.Attributes & (ParameterAttributes.In | ParameterAttributes.Out)) == ParameterAttributes.Out;
  }

  public static bool IsMandatory(this ParameterInfo pi)
  {
    return (pi.Attributes & ParameterAttributes.Optional) == ParameterAttributes.None && !pi.HasDefaultValue();
  }

  public static bool HasDefaultValue(this ParameterInfo pi)
  {
    return (pi.Attributes & ParameterAttributes.HasDefault) != 0;
  }

  public static bool ProhibitsNull(this ParameterInfo parameter)
  {
    return parameter.IsDefined(typeof (NotNullAttribute), false);
  }

  public static bool ProhibitsNullItems(this ParameterInfo parameter)
  {
    return parameter.IsDefined(typeof (NotNullItemsAttribute), false);
  }

  public static bool IsParamArray(this ParameterInfo parameter)
  {
    return parameter.IsDefined(typeof (ParamArrayAttribute), false);
  }

  public static bool IsParamDictionary(this ParameterInfo parameter)
  {
    return parameter.IsDefined(typeof (ParamDictionaryAttribute), false);
  }

  public static bool IsParamsMethod(MethodBase method)
  {
    return ReflectionUtils.IsParamsMethod(method.GetParameters());
  }

  public static bool IsParamsMethod(ParameterInfo[] pis)
  {
    foreach (ParameterInfo pi in pis)
    {
      if (pi.IsParamArray() || pi.IsParamDictionary())
        return true;
    }
    return false;
  }

  public static object GetDefaultValue(this ParameterInfo info) => info.DefaultValue;

  public static IEnumerable<Type> Ancestors(this Type type)
  {
    do
    {
      yield return type;
      type = type.BaseType;
    }
    while (type != (Type) null);
  }

  public static List<Type> GetDeclaredInterfaces(Type type)
  {
    IEnumerable<Type> source = type.GetBaseType() != (Type) null ? (IEnumerable<Type>) type.GetBaseType().GetInterfaces() : (IEnumerable<Type>) ReflectionUtils.EmptyTypes;
    List<Type> declaredInterfaces = new List<Type>();
    foreach (Type type1 in type.GetInterfaces())
    {
      if (!source.Contains<Type>(type1))
        declaredInterfaces.Add(type1);
    }
    return declaredInterfaces;
  }

  internal static IEnumerable<Type> GetAllTypesFromAssembly(Assembly asm)
  {
    Module[] moduleArray = asm.GetModules();
    for (int index1 = 0; index1 < moduleArray.Length; ++index1)
    {
      Module module = moduleArray[index1];
      Type[] types;
      try
      {
        types = module.GetTypes();
      }
      catch (ReflectionTypeLoadException ex)
      {
        types = ex.Types;
      }
      Type[] typeArray = types;
      for (int index2 = 0; index2 < typeArray.Length; ++index2)
      {
        Type type = typeArray[index2];
        if (type != (Type) null)
          yield return type;
      }
      typeArray = (Type[]) null;
    }
    moduleArray = (Module[]) null;
  }

  internal static IEnumerable<Type> GetAllTypesFromAssembly(
    Assembly assembly,
    bool includePrivateTypes)
  {
    ContractUtils.RequiresNotNull((object) assembly, nameof (assembly));
    if (includePrivateTypes)
      return ReflectionUtils.GetAllTypesFromAssembly(assembly);
    try
    {
      return (IEnumerable<Type>) assembly.GetExportedTypes();
    }
    catch (NotSupportedException ex)
    {
    }
    catch (Exception ex)
    {
    }
    return ReflectionUtils.GetAllTypesFromAssembly(assembly).Where<Type>((Func<Type, bool>) (type => type.IsPublic));
  }

  public static MethodBuilder DefineMethodOverride(
    TypeBuilder tb,
    MethodAttributes extra,
    MethodInfo decl)
  {
    MethodAttributes attributes = decl.Attributes & ~(MethodAttributes.ReservedMask | MethodAttributes.Abstract) | extra;
    if (!decl.DeclaringType.IsInterface)
      attributes &= ~MethodAttributes.VtableLayoutMask;
    if ((extra & MethodAttributes.MemberAccessMask) != MethodAttributes.PrivateScope)
      attributes = attributes & ~MethodAttributes.MemberAccessMask | extra;
    MethodBuilder to = tb.DefineMethod(decl.Name, attributes, decl.CallingConvention);
    ReflectionUtils.CopyMethodSignature(decl, to, false);
    return to;
  }

  public static void CopyMethodSignature(
    MethodInfo from,
    MethodBuilder to,
    bool substituteDeclaringType)
  {
    ParameterInfo[] parameters = from.GetParameters();
    Type[] parameterTypes = new Type[parameters.Length];
    Type[][] parameterTypeRequiredCustomModifiers = (Type[][]) null;
    Type[][] parameterTypeOptionalCustomModifiers = (Type[][]) null;
    Type[] requiredCustomModifiers1 = from.ReturnParameter.GetRequiredCustomModifiers();
    Type[] optionalCustomModifiers1 = from.ReturnParameter.GetOptionalCustomModifiers();
    for (int index = 0; index < parameters.Length; ++index)
    {
      parameterTypes[index] = !substituteDeclaringType || !(parameters[index].ParameterType == from.DeclaringType) ? parameters[index].ParameterType : to.DeclaringType;
      Type[] requiredCustomModifiers2 = parameters[index].GetRequiredCustomModifiers();
      if (requiredCustomModifiers2.Length != 0)
      {
        if (parameterTypeRequiredCustomModifiers == null)
          parameterTypeRequiredCustomModifiers = new Type[parameters.Length][];
        parameterTypeRequiredCustomModifiers[index] = requiredCustomModifiers2;
      }
      Type[] optionalCustomModifiers2 = parameters[index].GetOptionalCustomModifiers();
      if (optionalCustomModifiers2.Length != 0)
      {
        if (parameterTypeOptionalCustomModifiers == null)
          parameterTypeOptionalCustomModifiers = new Type[parameters.Length][];
        parameterTypeOptionalCustomModifiers[index] = optionalCustomModifiers2;
      }
    }
    to.SetSignature(from.ReturnType, requiredCustomModifiers1, optionalCustomModifiers1, parameterTypes, parameterTypeRequiredCustomModifiers, parameterTypeOptionalCustomModifiers);
    ReflectionUtils.CopyGenericMethodAttributes(from, to);
    for (int index = 0; index < parameters.Length; ++index)
      to.DefineParameter(index + 1, parameters[index].Attributes, parameters[index].Name);
  }

  private static void CopyGenericMethodAttributes(MethodInfo from, MethodBuilder to)
  {
    if (!from.IsGenericMethodDefinition)
      return;
    Type[] genericArguments = from.GetGenericArguments();
    string[] strArray = new string[genericArguments.Length];
    for (int index = 0; index < genericArguments.Length; ++index)
      strArray[index] = genericArguments[index].Name;
    GenericTypeParameterBuilder[] parameterBuilderArray = to.DefineGenericParameters(strArray);
    for (int index = 0; index < genericArguments.Length; ++index)
    {
      parameterBuilderArray[index].SetGenericParameterAttributes(genericArguments[index].GetGenericParameterAttributes());
      Type[] parameterConstraints = genericArguments[index].GetGenericParameterConstraints();
      List<Type> typeList = new List<Type>(parameterConstraints.Length);
      foreach (Type type in parameterConstraints)
      {
        if (type.IsInterface())
          typeList.Add(type);
        else
          parameterBuilderArray[index].SetBaseTypeConstraint(type);
      }
      if (typeList.Count > 0)
        parameterBuilderArray[index].SetInterfaceConstraints(typeList.ToArray());
    }
  }

  public static IEnumerable<MethodInfo> GetVisibleExtensionMethods(Assembly assembly)
  {
    if (!assembly.IsDynamic)
    {
      if (AppDomain.CurrentDomain.IsFullyTrusted)
      {
        try
        {
          return ReflectionUtils.GetVisibleExtensionMethodsFast(assembly);
        }
        catch (SecurityException ex)
        {
        }
      }
    }
    return ReflectionUtils.GetVisibleExtensionMethodsSlow(assembly);
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static IEnumerable<MethodInfo> GetVisibleExtensionMethodsFast(Assembly assembly)
  {
    return (IEnumerable<MethodInfo>) MetadataServices.GetVisibleExtensionMethodInfos(assembly);
  }

  public static IEnumerable<MethodInfo> GetVisibleExtensionMethodsSlow(Assembly assembly)
  {
    Type ea = typeof (ExtensionAttribute);
    if (assembly.IsDefined(ea))
    {
      foreach (Type type in ReflectionUtils.GetAllTypesFromAssembly(assembly))
      {
        if ((type.IsPublic || type.IsNestedPublic) && type.IsAbstract && type.IsSealed && type.IsDefined(ea, false))
        {
          foreach (MethodInfo declaredMethod in type.GetDeclaredMethods())
          {
            if (declaredMethod.IsPublic && declaredMethod.IsStatic && declaredMethod.IsDefined(ea, false))
              yield return declaredMethod;
          }
        }
      }
    }
  }

  public static IEnumerable<KeyValuePair<string, IEnumerable<ExtensionMethodInfo>>> GetVisibleExtensionMethodGroups(
    Assembly assembly,
    bool useCache)
  {
    useCache &= !assembly.IsDynamic;
    if (useCache)
    {
      if (ReflectionUtils._extensionMethodsCache == null)
        ReflectionUtils._extensionMethodsCache = new Dictionary<Assembly, Dictionary<string, List<ExtensionMethodInfo>>>();
      lock (ReflectionUtils._extensionMethodsCache)
      {
        Dictionary<string, List<ExtensionMethodInfo>> dict;
        if (ReflectionUtils._extensionMethodsCache.TryGetValue(assembly, out dict))
          return ReflectionUtils.EnumerateExtensionMethods(dict);
      }
    }
    Dictionary<string, List<ExtensionMethodInfo>> dict1 = (Dictionary<string, List<ExtensionMethodInfo>>) null;
    foreach (MethodInfo method in ReflectionUtils.GetVisibleExtensionMethodsSlow(assembly))
    {
      if (!(method.DeclaringType == (Type) null) && !method.DeclaringType.IsGenericTypeDefinition())
      {
        ParameterInfo[] parameters = method.GetParameters();
        if (parameters.Length != 0)
        {
          Type parameterType = parameters[0].ParameterType;
          if (!parameterType.IsByRef && !parameterType.IsPointer)
          {
            string key = method.DeclaringType.Namespace ?? string.Empty;
            if (dict1 == null)
              dict1 = new Dictionary<string, List<ExtensionMethodInfo>>();
            List<ExtensionMethodInfo> extensionMethodInfoList;
            if (!dict1.TryGetValue(key, out extensionMethodInfoList))
              dict1.Add(key, extensionMethodInfoList = new List<ExtensionMethodInfo>());
            extensionMethodInfoList.Add(new ExtensionMethodInfo(parameterType, method));
          }
        }
      }
    }
    if (useCache)
    {
      lock (ReflectionUtils._extensionMethodsCache)
        ReflectionUtils._extensionMethodsCache[assembly] = dict1;
    }
    return ReflectionUtils.EnumerateExtensionMethods(dict1);
  }

  private static IEnumerable<KeyValuePair<string, IEnumerable<ExtensionMethodInfo>>> EnumerateExtensionMethods(
    Dictionary<string, List<ExtensionMethodInfo>> dict)
  {
    if (dict != null)
    {
      foreach (KeyValuePair<string, List<ExtensionMethodInfo>> keyValuePair in dict)
        yield return new KeyValuePair<string, IEnumerable<ExtensionMethodInfo>>(keyValuePair.Key, (IEnumerable<ExtensionMethodInfo>) new ReadOnlyCollection<ExtensionMethodInfo>((IList<ExtensionMethodInfo>) keyValuePair.Value));
    }
  }

  internal static Dictionary<Type, Type> BindGenericParameters(
    Type openType,
    Type closedType,
    bool ignoreUnboundParameters)
  {
    Dictionary<Type, Type> binding = new Dictionary<Type, Type>();
    ReflectionUtils.BindGenericParameters(openType, closedType, (Func<Type, Type, bool>) ((parameter, type) =>
    {
      Type type1;
      if (binding.TryGetValue(parameter, out type1))
        return type == type1;
      binding[parameter] = type;
      return true;
    }));
    return !ReflectionUtils.ConstraintsViolated(binding, ignoreUnboundParameters) ? binding : (Dictionary<Type, Type>) null;
  }

  internal static bool BindGenericParameters(
    Type openType,
    Type closedType,
    Func<Type, Type, bool> binder)
  {
    if (openType.IsGenericParameter)
      return binder(openType, closedType);
    if (openType.IsArray)
      return closedType.IsArray && ReflectionUtils.BindGenericParameters(openType.GetElementType(), closedType.GetElementType(), binder);
    if (!openType.IsGenericType() || !closedType.IsGenericType())
      return openType == closedType;
    if (openType.GetGenericTypeDefinition() != closedType.GetGenericTypeDefinition())
      return false;
    Type[] genericArguments1 = closedType.GetGenericArguments();
    Type[] genericArguments2 = openType.GetGenericArguments();
    for (int index = 0; index < genericArguments2.Length; ++index)
    {
      if (!ReflectionUtils.BindGenericParameters(genericArguments2[index], genericArguments1[index], binder))
        return false;
    }
    return true;
  }

  internal static bool ConstraintsViolated(
    Dictionary<Type, Type> binding,
    bool ignoreUnboundParameters)
  {
    foreach (KeyValuePair<Type, Type> keyValuePair in binding)
    {
      if (ReflectionUtils.ConstraintsViolated(keyValuePair.Key, keyValuePair.Value, binding, ignoreUnboundParameters))
        return true;
    }
    return false;
  }

  internal static bool ConstraintsViolated(
    Type genericParameter,
    Type closedType,
    Dictionary<Type, Type> binding,
    bool ignoreUnboundParameters)
  {
    if ((genericParameter.GetGenericParameterAttributes() & GenericParameterAttributes.ReferenceTypeConstraint) != GenericParameterAttributes.None && closedType.IsValueType() || (genericParameter.GetGenericParameterAttributes() & GenericParameterAttributes.NotNullableValueTypeConstraint) != GenericParameterAttributes.None && (!closedType.IsValueType() || closedType.IsGenericType() && closedType.GetGenericTypeDefinition() == typeof (Nullable<>)) || (genericParameter.GetGenericParameterAttributes() & GenericParameterAttributes.DefaultConstructorConstraint) != GenericParameterAttributes.None && !closedType.IsValueType() && closedType.GetConstructor(ReflectionUtils.EmptyTypes) == (ConstructorInfo) null)
      return true;
    foreach (Type parameterConstraint in genericParameter.GetGenericParameterConstraints())
    {
      Type type = ReflectionUtils.InstantiateConstraint(parameterConstraint, binding);
      if (type == (Type) null)
      {
        if (!ignoreUnboundParameters)
          return true;
      }
      else if (!type.IsAssignableFrom(closedType))
        return true;
    }
    return false;
  }

  internal static Type InstantiateConstraint(Type constraint, Dictionary<Type, Type> binding)
  {
    if (!constraint.ContainsGenericParameters())
      return constraint;
    if (constraint.IsGenericParameter)
    {
      Type type;
      return !binding.TryGetValue(constraint, out type) ? (Type) null : type;
    }
    Type[] genericArguments = constraint.GetGenericArguments();
    for (int index = 0; index < genericArguments.Length; ++index)
    {
      if ((genericArguments[index] = ReflectionUtils.InstantiateConstraint(genericArguments[index], binding)) == (Type) null)
        return (Type) null;
    }
    return constraint.GetGenericTypeDefinition().MakeGenericType(genericArguments);
  }
}
