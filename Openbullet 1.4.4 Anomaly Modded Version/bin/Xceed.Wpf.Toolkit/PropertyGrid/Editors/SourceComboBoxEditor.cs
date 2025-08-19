// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Editors.SourceComboBoxEditor
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Collections;
using System.ComponentModel;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Editors;

public class SourceComboBoxEditor : ComboBoxEditor
{
  private ICollection _collection;
  private TypeConverter _typeConverter;

  public SourceComboBoxEditor(ICollection collection, TypeConverter typeConverter)
  {
    this._collection = collection;
    this._typeConverter = typeConverter;
  }

  protected override IEnumerable CreateItemsSource(PropertyItem propertyItem)
  {
    return (IEnumerable) this._collection;
  }

  protected override IValueConverter CreateValueConverter()
  {
    return this._typeConverter != null && this._typeConverter is StringConverter ? (IValueConverter) new SourceComboBoxEditorConverter(this._typeConverter) : (IValueConverter) null;
  }
}
