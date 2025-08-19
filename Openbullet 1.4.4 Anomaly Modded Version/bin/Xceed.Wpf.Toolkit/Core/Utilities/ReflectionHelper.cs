// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.ReflectionHelper
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal static class ReflectionHelper
{
  [Conditional("DEBUG")]
  internal static void ValidatePublicPropertyName(object sourceObject, string propertyName)
  {
    if (sourceObject == null)
      throw new ArgumentNullException(nameof (sourceObject));
    if (propertyName == null)
      throw new ArgumentNullException(nameof (propertyName));
  }

  [Conditional("DEBUG")]
  internal static void ValidatePropertyName(object sourceObject, string propertyName)
  {
    if (sourceObject == null)
      throw new ArgumentNullException(nameof (sourceObject));
    if (propertyName == null)
      throw new ArgumentNullException(nameof (propertyName));
  }

  internal static bool TryGetEnumDescriptionAttributeValue(Enum enumeration, out string description)
  {
    try
    {
      if (enumeration.GetType().GetField(enumeration.ToString()).GetCustomAttributes(typeof (DescriptionAttribute), true) is DescriptionAttribute[] customAttributes)
      {
        if (customAttributes.Length != 0)
        {
          description = customAttributes[0].Description;
          return true;
        }
      }
    }
    catch
    {
    }
    description = string.Empty;
    return false;
  }

  [DebuggerStepThrough]
  internal static string GetPropertyOrFieldName(MemberExpression expression)
  {
    string propertyOrFieldName;
    if (!ReflectionHelper.TryGetPropertyOrFieldName(expression, out propertyOrFieldName))
      throw new InvalidOperationException("Unable to retrieve the property or field name.");
    return propertyOrFieldName;
  }

  [DebuggerStepThrough]
  internal static string GetPropertyOrFieldName<TMember>(Expression<Func<TMember>> expression)
  {
    string propertyOrFieldName;
    if (!ReflectionHelper.TryGetPropertyOrFieldName<TMember>(expression, out propertyOrFieldName))
      throw new InvalidOperationException("Unable to retrieve the property or field name.");
    return propertyOrFieldName;
  }

  [DebuggerStepThrough]
  internal static bool TryGetPropertyOrFieldName(
    MemberExpression expression,
    out string propertyOrFieldName)
  {
    propertyOrFieldName = (string) null;
    if (expression == null)
      return false;
    propertyOrFieldName = expression.Member.Name;
    return true;
  }

  [DebuggerStepThrough]
  internal static bool TryGetPropertyOrFieldName<TMember>(
    Expression<Func<TMember>> expression,
    out string propertyOrFieldName)
  {
    propertyOrFieldName = (string) null;
    return expression != null && ReflectionHelper.TryGetPropertyOrFieldName(expression.Body as MemberExpression, out propertyOrFieldName);
  }

  public static bool IsPublicInstanceProperty(Type type, string propertyName)
  {
    BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
    return type.GetProperty(propertyName, bindingAttr) != (PropertyInfo) null;
  }
}
