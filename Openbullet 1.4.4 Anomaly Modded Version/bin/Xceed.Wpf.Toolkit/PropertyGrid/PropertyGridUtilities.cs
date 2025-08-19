// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.PropertyGridUtilities
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

internal class PropertyGridUtilities
{
  internal static T GetAttribute<T>(PropertyDescriptor property) where T : Attribute
  {
    return property.Attributes.OfType<T>().FirstOrDefault<T>();
  }

  internal static ITypeEditor CreateDefaultEditor(
    Type propertyType,
    TypeConverter typeConverter,
    PropertyItem propertyItem)
  {
    PropertyGridUtilities.EditorTypeDescriptorContext context = new PropertyGridUtilities.EditorTypeDescriptorContext((IContainer) null, propertyItem.Instance, propertyItem.PropertyDescriptor);
    ITypeEditor defaultEditor;
    if (typeConverter != null && typeConverter.GetStandardValuesSupported((ITypeDescriptorContext) context) && typeConverter.GetStandardValuesExclusive((ITypeDescriptorContext) context) && propertyType != typeof (bool) && propertyType != typeof (bool?))
      defaultEditor = (ITypeEditor) new SourceComboBoxEditor((ICollection) typeConverter.GetStandardValues((ITypeDescriptorContext) context), typeConverter);
    else if (propertyType == typeof (string))
      defaultEditor = (ITypeEditor) new TextBoxEditor();
    else if (propertyType == typeof (bool) || propertyType == typeof (bool?))
      defaultEditor = (ITypeEditor) new CheckBoxEditor();
    else if (propertyType == typeof (Decimal) || propertyType == typeof (Decimal?))
      defaultEditor = (ITypeEditor) new DecimalUpDownEditor();
    else if (propertyType == typeof (double) || propertyType == typeof (double?))
      defaultEditor = (ITypeEditor) new DoubleUpDownEditor();
    else if (propertyType == typeof (int) || propertyType == typeof (int?))
      defaultEditor = (ITypeEditor) new IntegerUpDownEditor();
    else if (propertyType == typeof (short) || propertyType == typeof (short?))
      defaultEditor = (ITypeEditor) new ShortUpDownEditor();
    else if (propertyType == typeof (long) || propertyType == typeof (long?))
      defaultEditor = (ITypeEditor) new LongUpDownEditor();
    else if (propertyType == typeof (float) || propertyType == typeof (float?))
      defaultEditor = (ITypeEditor) new SingleUpDownEditor();
    else if (propertyType == typeof (byte) || propertyType == typeof (byte?))
      defaultEditor = (ITypeEditor) new ByteUpDownEditor();
    else if (propertyType == typeof (sbyte) || propertyType == typeof (sbyte?))
      defaultEditor = (ITypeEditor) new SByteUpDownEditor();
    else if (propertyType == typeof (uint) || propertyType == typeof (uint?))
      defaultEditor = (ITypeEditor) new UIntegerUpDownEditor();
    else if (propertyType == typeof (ulong) || propertyType == typeof (ulong?))
      defaultEditor = (ITypeEditor) new ULongUpDownEditor();
    else if (propertyType == typeof (ushort) || propertyType == typeof (ushort?))
      defaultEditor = (ITypeEditor) new UShortUpDownEditor();
    else if (propertyType == typeof (DateTime) || propertyType == typeof (DateTime?))
      defaultEditor = (ITypeEditor) new DateTimeUpDownEditor();
    else if (propertyType == typeof (Color) || propertyType == typeof (Color?))
      defaultEditor = (ITypeEditor) new ColorEditor();
    else if (propertyType.IsEnum)
      defaultEditor = (ITypeEditor) new EnumComboBoxEditor();
    else if (propertyType == typeof (TimeSpan) || propertyType == typeof (TimeSpan?))
      defaultEditor = (ITypeEditor) new TimeSpanUpDownEditor();
    else if (propertyType == typeof (FontFamily) || propertyType == typeof (FontWeight) || propertyType == typeof (FontStyle) || propertyType == typeof (FontStretch))
      defaultEditor = (ITypeEditor) new FontComboBoxEditor();
    else if (propertyType == typeof (Guid) || propertyType == typeof (Guid?))
      defaultEditor = (ITypeEditor) new MaskedTextBoxEditor()
      {
        ValueDataType = propertyType,
        Mask = "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"
      };
    else if (propertyType == typeof (char) || propertyType == typeof (char?))
      defaultEditor = (ITypeEditor) new MaskedTextBoxEditor()
      {
        ValueDataType = propertyType,
        Mask = "&"
      };
    else if (propertyType == typeof (object))
    {
      defaultEditor = (ITypeEditor) new TextBoxEditor();
    }
    else
    {
      Type listItemType = ListUtilities.GetListItemType(propertyType);
      if (listItemType != (Type) null)
      {
        defaultEditor = listItemType.IsPrimitive || listItemType.Equals(typeof (string)) || listItemType.IsEnum ? (ITypeEditor) new PrimitiveTypeCollectionEditor() : (ITypeEditor) new CollectionEditor();
      }
      else
      {
        Type[] dictionaryItemsType = ListUtilities.GetDictionaryItemsType(propertyType);
        Type collectionItemType = ListUtilities.GetCollectionItemType(propertyType);
        defaultEditor = dictionaryItemsType != null || collectionItemType != (Type) null || typeof (ICollection).IsAssignableFrom(propertyType) ? (ITypeEditor) new CollectionEditor() : (typeConverter == null || !typeConverter.CanConvertFrom(typeof (string)) ? (ITypeEditor) new TextBlockEditor() : (ITypeEditor) new TextBoxEditor());
      }
    }
    return defaultEditor;
  }

  private class EditorTypeDescriptorContext : ITypeDescriptorContext, IServiceProvider
  {
    private IContainer _container;
    private object _instance;
    private PropertyDescriptor _propertyDescriptor;

    internal EditorTypeDescriptorContext(
      IContainer container,
      object instance,
      PropertyDescriptor pd)
    {
      this._container = container;
      this._instance = instance;
      this._propertyDescriptor = pd;
    }

    IContainer ITypeDescriptorContext.Container => this._container;

    object ITypeDescriptorContext.Instance => this._instance;

    PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor => this._propertyDescriptor;

    void ITypeDescriptorContext.OnComponentChanged()
    {
    }

    bool ITypeDescriptorContext.OnComponentChanging() => false;

    object IServiceProvider.GetService(Type serviceType) => (object) null;
  }
}
