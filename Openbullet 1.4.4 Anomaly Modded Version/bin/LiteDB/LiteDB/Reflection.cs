// Decompiled with JetBrains decompiler
// Type: LiteDB.Reflection
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace LiteDB;

internal class Reflection
{
  private static Dictionary<Type, CreateObject> _cacheCtor = new Dictionary<Type, CreateObject>();

  public static object CreateInstance(Type type)
  {
    try
    {
      CreateObject createObject;
      if (LiteDB.Reflection._cacheCtor.TryGetValue(type, out createObject))
        return createObject();
    }
    catch (Exception ex)
    {
      throw LiteException.InvalidCtor(type, ex);
    }
    lock (LiteDB.Reflection._cacheCtor)
    {
      try
      {
        CreateObject createObject1;
        if (LiteDB.Reflection._cacheCtor.TryGetValue(type, out createObject1))
          return createObject1();
        CreateObject createObject2;
        if (type.GetTypeInfo().IsClass)
        {
          LiteDB.Reflection._cacheCtor.Add(type, createObject2 = LiteDB.Reflection.CreateClass(type));
        }
        else
        {
          if (type.GetTypeInfo().IsInterface)
          {
            if (type.GetTypeInfo().IsGenericType)
            {
              Type genericTypeDefinition = type.GetGenericTypeDefinition();
              if (genericTypeDefinition == typeof (IList<>) || genericTypeDefinition == typeof (ICollection<>) || genericTypeDefinition == typeof (IEnumerable<>))
                return LiteDB.Reflection.CreateInstance(LiteDB.Reflection.GetGenericListOfType(LiteDB.Reflection.UnderlyingTypeOf(type)));
              if (genericTypeDefinition == typeof (IDictionary<,>))
                return LiteDB.Reflection.CreateInstance(LiteDB.Reflection.GetGenericDictionaryOfType(type.GetTypeInfo().GetGenericArguments()[0], type.GetTypeInfo().GetGenericArguments()[1]));
            }
            throw LiteException.InvalidCtor(type, (Exception) null);
          }
          LiteDB.Reflection._cacheCtor.Add(type, createObject2 = LiteDB.Reflection.CreateStruct(type));
        }
        return createObject2();
      }
      catch (Exception ex)
      {
        throw LiteException.InvalidCtor(type, ex);
      }
    }
  }

  public static bool IsNullable(Type type)
  {
    return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition().Equals(typeof (Nullable<>));
  }

  public static Type UnderlyingTypeOf(Type type)
  {
    type.GetTypeInfo();
    return !type.GetTypeInfo().IsGenericType ? type : type.GetTypeInfo().GetGenericArguments()[0];
  }

  public static Type GetGenericListOfType(Type type) => typeof (List<>).MakeGenericType(type);

  public static Type GetGenericDictionaryOfType(Type k, Type v)
  {
    return typeof (Dictionary<,>).MakeGenericType(k, v);
  }

  public static Type GetListItemType(Type listType)
  {
    if (listType.IsArray)
      return listType.GetElementType();
    foreach (Type type in listType.GetInterfaces())
    {
      if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof (IEnumerable<>))
        return type.GetTypeInfo().GetGenericArguments()[0];
      if (listType.GetTypeInfo().IsGenericType && type == typeof (IEnumerable))
        return listType.GetTypeInfo().GetGenericArguments()[0];
    }
    return typeof (object);
  }

  public static bool IsList(Type type)
  {
    if (type.IsArray)
      return true;
    if (type == typeof (string))
      return false;
    foreach (Type type1 in type.GetInterfaces())
    {
      if (type1.GetTypeInfo().IsGenericType && type1.GetGenericTypeDefinition() == typeof (IEnumerable<>))
        return true;
    }
    return false;
  }

  public static MemberInfo SelectMember(
    IEnumerable<MemberInfo> members,
    params Func<MemberInfo, bool>[] predicates)
  {
    foreach (Func<MemberInfo, bool> predicate in predicates)
    {
      MemberInfo memberInfo = members.FirstOrDefault<MemberInfo>(predicate);
      if (memberInfo != (MemberInfo) null)
        return memberInfo;
    }
    return (MemberInfo) null;
  }

  public static CreateObject CreateClass(Type type)
  {
    return ((Expression<CreateObject>) (() => Expression.New(type))).Compile();
  }

  public static CreateObject CreateStruct(Type type)
  {
    return ((Expression<CreateObject>) (() => (object) Expression.New(type))).Compile();
  }

  public static GenericGetter CreateGenericGetter(Type type, MemberInfo memberInfo)
  {
    if (memberInfo == (MemberInfo) null)
      throw new ArgumentNullException(nameof (memberInfo));
    if ((object) (memberInfo as PropertyInfo) != null && !(memberInfo as PropertyInfo).CanRead)
      return (GenericGetter) null;
    return ((Expression<GenericGetter>) (o => (object) Expression.MakeMemberAccess((Expression) Expression.Convert(o, memberInfo.DeclaringType), memberInfo))).Compile();
  }

  public static GenericSetter CreateGenericSetter(Type type, MemberInfo memberInfo)
  {
    FieldInfo field = !(memberInfo == (MemberInfo) null) ? memberInfo as FieldInfo : throw new ArgumentNullException(nameof (memberInfo));
    PropertyInfo propertyInfo = memberInfo as PropertyInfo;
    if ((object) (memberInfo as PropertyInfo) != null && !propertyInfo.CanWrite)
      return (GenericSetter) null;
    if (type.GetTypeInfo().IsValueType)
      return (object) (memberInfo as FieldInfo) == null ? (GenericSetter) ((t, v) => propertyInfo.SetValue(t, v, (object[]) null)) : new GenericSetter(field.SetValue);
    Type type1 = (object) (memberInfo as PropertyInfo) != null ? propertyInfo.PropertyType : field.FieldType;
    ParameterExpression parameterExpression3 = Expression.Parameter(typeof (object), "obj");
    ParameterExpression parameterExpression4 = Expression.Parameter(typeof (object), "val");
    UnaryExpression unaryExpression = Expression.Convert((Expression) parameterExpression3, type);
    UnaryExpression right = Expression.ConvertChecked((Expression) parameterExpression4, type1);
    return ((Expression<GenericSetter>) ((parameterExpression1, parameterExpression2) => (object) Expression.Assign((object) (memberInfo as PropertyInfo) != null ? (Expression) Expression.Property((Expression) unaryExpression, propertyInfo) : (Expression) Expression.Field((Expression) unaryExpression, field), (Expression) right))).Compile();
  }
}
