// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.CustomTypeDescHelpers
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions.Calls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class CustomTypeDescHelpers
{
  public static AttributeCollection GetAttributes(object self) => AttributeCollection.Empty;

  public static string GetClassName(object self)
  {
    object ret;
    return PythonOps.TryGetBoundAttr(DefaultContext.DefaultCLS, self, "__class__", out ret) ? PythonOps.GetBoundAttr(DefaultContext.DefaultCLS, ret, "__name__").ToString() : (string) null;
  }

  public static string GetComponentName(object self) => (string) null;

  public static TypeConverter GetConverter(object self)
  {
    return (TypeConverter) new CustomTypeDescHelpers.TypeConv(self);
  }

  public static EventDescriptor GetDefaultEvent(object self) => (EventDescriptor) null;

  public static PropertyDescriptor GetDefaultProperty(object self) => (PropertyDescriptor) null;

  public static object GetEditor(object self, Type editorBaseType) => (object) null;

  public static EventDescriptorCollection GetEvents(object self, Attribute[] attributes)
  {
    return attributes == null || attributes.Length == 0 ? CustomTypeDescHelpers.GetEvents(self) : EventDescriptorCollection.Empty;
  }

  public static EventDescriptorCollection GetEvents(object self) => EventDescriptorCollection.Empty;

  public static PropertyDescriptorCollection GetProperties(object self)
  {
    return new PropertyDescriptorCollection(CustomTypeDescHelpers.GetPropertiesImpl(self, new Attribute[0]));
  }

  public static PropertyDescriptorCollection GetProperties(object self, Attribute[] attributes)
  {
    return new PropertyDescriptorCollection(CustomTypeDescHelpers.GetPropertiesImpl(self, attributes));
  }

  private static PropertyDescriptor[] GetPropertiesImpl(object self, Attribute[] attributes)
  {
    IList<object> attrNames = PythonOps.GetAttrNames(DefaultContext.DefaultCLS, self);
    List<PropertyDescriptor> propertyDescriptorList = new List<PropertyDescriptor>();
    if (attrNames != null)
    {
      foreach (object obj in (IEnumerable<object>) attrNames)
      {
        if (obj is string str)
        {
          PythonTypeSlot slot = (PythonTypeSlot) null;
          object ret;
          if (self is OldInstance)
          {
            if (((OldInstance) self)._class.TryLookupSlot(str, out ret))
              slot = ret as PythonTypeSlot;
            else if (!((OldInstance) self).Dictionary.TryGetValue((object) str, out ret))
              ret = ObjectOps.__getattribute__(DefaultContext.DefaultCLS, self, str);
          }
          else
          {
            DynamicHelpers.GetPythonType(self).TryResolveSlot(DefaultContext.DefaultCLS, str, out slot);
            ret = ObjectOps.__getattribute__(DefaultContext.DefaultCLS, self, str);
          }
          Type propertyType = ret == null ? typeof (NoneTypeOps) : ret.GetType();
          if (slot != null && CustomTypeDescHelpers.ShouldIncludeProperty(slot, attributes) || slot == null && CustomTypeDescHelpers.ShouldIncludeInstanceMember(str, attributes))
            propertyDescriptorList.Add((PropertyDescriptor) new CustomTypeDescHelpers.SuperDynamicObjectPropertyDescriptor(str, propertyType, self.GetType()));
        }
      }
    }
    return propertyDescriptorList.ToArray();
  }

  private static bool ShouldIncludeInstanceMember(string memberName, Attribute[] attributes)
  {
    bool flag = true;
    foreach (object attribute in attributes)
    {
      if (attribute.GetType() == typeof (BrowsableAttribute))
      {
        if (memberName.StartsWith("__") && memberName.EndsWith("__"))
          flag = false;
      }
      else
        flag = false;
    }
    return flag;
  }

  private static bool ShouldIncludeProperty(PythonTypeSlot attrSlot, Attribute[] attributes)
  {
    bool flag = true;
    foreach (Attribute attribute in attributes)
    {
      if (attrSlot is ReflectedProperty reflectedProperty && reflectedProperty.Info != (PropertyInfo) null)
        flag &= reflectedProperty.Info.IsDefined(attribute.GetType(), true);
      else if (attribute.GetType() == typeof (BrowsableAttribute))
      {
        switch (attrSlot)
        {
          case PythonTypeUserDescriptorSlot userDescriptorSlot:
            if (!(userDescriptorSlot.Value is IPythonObject))
            {
              flag = false;
              continue;
            }
            continue;
          case PythonProperty _:
            continue;
          default:
            flag = false;
            continue;
        }
      }
      else
        flag = false;
    }
    return flag;
  }

  public static object GetPropertyOwner(object self, PropertyDescriptor pd) => self;

  private class SuperDynamicObjectPropertyDescriptor : PropertyDescriptor
  {
    private string _name;
    private Type _propertyType;
    private Type _componentType;

    internal SuperDynamicObjectPropertyDescriptor(
      string name,
      Type propertyType,
      Type componentType)
      : base(name, (Attribute[]) null)
    {
      this._name = name;
      this._propertyType = propertyType;
      this._componentType = componentType;
    }

    public override object GetValue(object component)
    {
      return PythonOps.GetBoundAttr(DefaultContext.DefaultCLS, component, this._name);
    }

    public override void SetValue(object component, object value)
    {
      PythonOps.SetAttr(DefaultContext.DefaultCLS, component, this._name, value);
    }

    public override bool CanResetValue(object component) => true;

    public override Type ComponentType => this._componentType;

    public override bool IsReadOnly => false;

    public override Type PropertyType => this._propertyType;

    public override void ResetValue(object component)
    {
      PythonOps.DeleteAttr(DefaultContext.DefaultCLS, component, this._name);
    }

    public override bool ShouldSerializeValue(object component)
    {
      return PythonOps.TryGetBoundAttr(component, this._name, out object _);
    }
  }

  private class TypeConv : TypeConverter
  {
    private object convObj;

    public TypeConv(object self) => this.convObj = self;

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return Converter.TryConvert(this.convObj, destinationType, out object _);
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return Converter.CanConvertFrom(sourceType, this.convObj.GetType(), NarrowingLevel.All);
    }

    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
    {
      return Converter.Convert(value, this.convObj.GetType());
    }

    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
    {
      return Converter.Convert(this.convObj, destinationType);
    }

    public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) => false;

    public override bool GetPropertiesSupported(ITypeDescriptorContext context) => false;

    public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => false;

    public override bool IsValid(ITypeDescriptorContext context, object value)
    {
      return Converter.TryConvert(value, this.convObj.GetType(), out object _);
    }
  }
}
