// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.PageObjects.DefaultPageObjectMemberDecorator
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

#nullable disable
namespace OpenQA.Selenium.Support.PageObjects;

public class DefaultPageObjectMemberDecorator : IPageObjectMemberDecorator
{
  private static List<Type> interfacesToBeProxied;
  private static Type interfaceProxyType;

  private static List<Type> InterfacesToBeProxied
  {
    get
    {
      if (DefaultPageObjectMemberDecorator.interfacesToBeProxied == null)
      {
        DefaultPageObjectMemberDecorator.interfacesToBeProxied = new List<Type>();
        DefaultPageObjectMemberDecorator.interfacesToBeProxied.Add(typeof (IWebElement));
        DefaultPageObjectMemberDecorator.interfacesToBeProxied.Add(typeof (ILocatable));
        DefaultPageObjectMemberDecorator.interfacesToBeProxied.Add(typeof (IWrapsElement));
      }
      return DefaultPageObjectMemberDecorator.interfacesToBeProxied;
    }
  }

  private static Type InterfaceProxyType
  {
    get
    {
      if (DefaultPageObjectMemberDecorator.interfaceProxyType == (Type) null)
        DefaultPageObjectMemberDecorator.interfaceProxyType = DefaultPageObjectMemberDecorator.CreateTypeForASingleElement();
      return DefaultPageObjectMemberDecorator.interfaceProxyType;
    }
  }

  public object Decorate(MemberInfo member, IElementLocator locator)
  {
    FieldInfo fieldInfo = member as FieldInfo;
    PropertyInfo propertyInfo = member as PropertyInfo;
    Type memberType = (Type) null;
    if (fieldInfo != (FieldInfo) null)
      memberType = fieldInfo.FieldType;
    bool flag = false;
    if (propertyInfo != (PropertyInfo) null)
    {
      flag = propertyInfo.CanWrite;
      memberType = propertyInfo.PropertyType;
    }
    if (((fieldInfo == (FieldInfo) null ? 1 : 0) & (propertyInfo == (PropertyInfo) null ? 1 : (!flag ? 1 : 0))) != 0)
      return (object) null;
    IList<By> locatorList = (IList<By>) DefaultPageObjectMemberDecorator.CreateLocatorList(member);
    if (locatorList.Count <= 0)
      return (object) null;
    bool cache = DefaultPageObjectMemberDecorator.ShouldCacheLookup(member);
    return DefaultPageObjectMemberDecorator.CreateProxyObject(memberType, locator, (IEnumerable<By>) locatorList, cache);
  }

  protected static bool ShouldCacheLookup(MemberInfo member)
  {
    if (member == (MemberInfo) null)
      throw new ArgumentNullException(nameof (member), "memeber cannot be null");
    Type attributeType = typeof (CacheLookupAttribute);
    return member.GetCustomAttributes(attributeType, true).Length != 0 || member.DeclaringType.GetCustomAttributes(attributeType, true).Length != 0;
  }

  protected static ReadOnlyCollection<By> CreateLocatorList(MemberInfo member)
  {
    if (member == (MemberInfo) null)
      throw new ArgumentNullException(nameof (member), "memeber cannot be null");
    bool flag1 = Attribute.GetCustomAttributes(member, typeof (FindsBySequenceAttribute), true).Length != 0;
    bool flag2 = Attribute.GetCustomAttributes(member, typeof (FindsByAllAttribute), true).Length != 0;
    if (flag1 & flag2)
      throw new ArgumentException("Cannot specify FindsBySequence and FindsByAll on the same member");
    List<By> byList = new List<By>();
    Attribute[] customAttributes = Attribute.GetCustomAttributes(member, typeof (FindsByAttribute), true);
    if (customAttributes.Length != 0)
    {
      Array.Sort<Attribute>(customAttributes);
      foreach (FindsByAttribute findsByAttribute in customAttributes)
      {
        if (findsByAttribute.Using == null)
          findsByAttribute.Using = member.Name;
        byList.Add(findsByAttribute.Finder);
      }
      if (flag1)
      {
        ByChained byChained = new ByChained(byList.ToArray());
        byList.Clear();
        byList.Add((By) byChained);
      }
      if (flag2)
      {
        ByAll byAll = new ByAll(byList.ToArray());
        byList.Clear();
        byList.Add((By) byAll);
      }
    }
    return byList.AsReadOnly();
  }

  private static object CreateProxyObject(
    Type memberType,
    IElementLocator locator,
    IEnumerable<By> bys,
    bool cache)
  {
    object proxyObject = (object) null;
    if (memberType == typeof (IList<IWebElement>))
    {
      foreach (Type type in DefaultPageObjectMemberDecorator.InterfacesToBeProxied)
      {
        if (typeof (IList<>).MakeGenericType(type).Equals(memberType))
        {
          proxyObject = WebElementListProxy.CreateProxy(memberType, locator, bys, cache);
          break;
        }
      }
    }
    else
    {
      if (!(memberType == typeof (IWebElement)))
        throw new ArgumentException($"Type of member '{memberType.Name}' is not IWebElement or IList<IWebElement>");
      proxyObject = WebElementProxy.CreateProxy(DefaultPageObjectMemberDecorator.InterfaceProxyType, locator, bys, cache);
    }
    return proxyObject;
  }

  private static Type CreateTypeForASingleElement()
  {
    AssemblyName name = new AssemblyName(Guid.NewGuid().ToString());
    TypeBuilder typeBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run).DefineDynamicModule(name.Name).DefineType(typeof (IWebElement).FullName, TypeAttributes.Public | TypeAttributes.ClassSemanticsMask | TypeAttributes.Abstract);
    foreach (Type interfaceType in DefaultPageObjectMemberDecorator.InterfacesToBeProxied)
      typeBuilder.AddInterfaceImplementation(interfaceType);
    return typeBuilder.CreateType();
  }
}
