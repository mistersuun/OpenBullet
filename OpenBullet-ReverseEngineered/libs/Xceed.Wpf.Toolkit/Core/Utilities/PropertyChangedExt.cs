// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.PropertyChangedExt
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;
using System.Linq.Expressions;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal static class PropertyChangedExt
{
  public static void Notify<TMember>(
    this INotifyPropertyChanged sender,
    PropertyChangedEventHandler handler,
    Expression<Func<TMember>> expression)
  {
    if (sender == null)
      throw new ArgumentNullException(nameof (sender));
    if (expression == null)
      throw new ArgumentNullException(nameof (expression));
    if (!(expression.Body is MemberExpression body))
      throw new ArgumentException("The expression must target a property or field.", nameof (expression));
    string propertyName = PropertyChangedExt.GetPropertyName(body, sender.GetType());
    PropertyChangedExt.NotifyCore(sender, handler, propertyName);
  }

  public static void Notify(
    this INotifyPropertyChanged sender,
    PropertyChangedEventHandler handler,
    string propertyName)
  {
    if (sender == null)
      throw new ArgumentNullException(nameof (sender));
    if (propertyName == null)
      throw new ArgumentNullException(nameof (propertyName));
    PropertyChangedExt.NotifyCore(sender, handler, propertyName);
  }

  private static void NotifyCore(
    INotifyPropertyChanged sender,
    PropertyChangedEventHandler handler,
    string propertyName)
  {
    if (handler == null)
      return;
    handler((object) sender, new PropertyChangedEventArgs(propertyName));
  }

  internal static bool PropertyChanged(
    string propertyName,
    PropertyChangedEventArgs e,
    bool targetPropertyOnly)
  {
    string propertyName1 = e.PropertyName;
    if (propertyName1 == propertyName)
      return true;
    return !targetPropertyOnly && string.IsNullOrEmpty(propertyName1);
  }

  internal static bool PropertyChanged<TOwner, TMember>(
    Expression<Func<TMember>> expression,
    PropertyChangedEventArgs e,
    bool targetPropertyOnly)
  {
    if (!(expression.Body is MemberExpression body))
      throw new ArgumentException("The expression must target a property or field.", nameof (expression));
    return PropertyChangedExt.PropertyChanged(body, typeof (TOwner), e, targetPropertyOnly);
  }

  internal static bool PropertyChanged<TOwner, TMember>(
    Expression<Func<TOwner, TMember>> expression,
    PropertyChangedEventArgs e,
    bool targetPropertyOnly)
  {
    if (!(expression.Body is MemberExpression body))
      throw new ArgumentException("The expression must target a property or field.", nameof (expression));
    return PropertyChangedExt.PropertyChanged(body, typeof (TOwner), e, targetPropertyOnly);
  }

  private static bool PropertyChanged(
    MemberExpression expression,
    Type ownerType,
    PropertyChangedEventArgs e,
    bool targetPropertyOnly)
  {
    return PropertyChangedExt.PropertyChanged(PropertyChangedExt.GetPropertyName(expression, ownerType), e, targetPropertyOnly);
  }

  private static string GetPropertyName(MemberExpression expression, Type ownerType)
  {
    if (!expression.Expression.Type.IsAssignableFrom(ownerType))
      throw new ArgumentException("The expression must target a property or field on the appropriate owner.", nameof (expression));
    return ReflectionHelper.GetPropertyOrFieldName(expression);
  }
}
