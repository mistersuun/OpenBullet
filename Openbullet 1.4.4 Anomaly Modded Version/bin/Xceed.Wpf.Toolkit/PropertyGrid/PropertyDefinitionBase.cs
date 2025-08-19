// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.PropertyDefinitionBase
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

public abstract class PropertyDefinitionBase : DefinitionBase
{
  private IList _targetProperties;
  private PropertyDefinitionCollection _propertyDefinitions;

  internal PropertyDefinitionBase()
  {
    this._targetProperties = (IList) new List<object>();
    this.PropertyDefinitions = new PropertyDefinitionCollection();
  }

  [TypeConverter(typeof (ListConverter))]
  public IList TargetProperties
  {
    get => this._targetProperties;
    set
    {
      this.ThrowIfLocked<IList>((Expression<Func<IList>>) (() => this.TargetProperties));
      this._targetProperties = value;
    }
  }

  public PropertyDefinitionCollection PropertyDefinitions
  {
    get => this._propertyDefinitions;
    set
    {
      this.ThrowIfLocked<PropertyDefinitionCollection>((Expression<Func<PropertyDefinitionCollection>>) (() => this.PropertyDefinitions));
      this._propertyDefinitions = value;
    }
  }

  internal override void Lock()
  {
    if (this.IsLocked)
      return;
    base.Lock();
    List<object> list = new List<object>();
    if (this._targetProperties != null)
    {
      foreach (object targetProperty in (IEnumerable) this._targetProperties)
      {
        object obj = targetProperty;
        if (obj is TargetPropertyType targetPropertyType)
          obj = (object) targetPropertyType.Type;
        list.Add(obj);
      }
    }
    this._targetProperties = DesignerProperties.GetIsInDesignMode((DependencyObject) this) ? (IList) new Collection<object>((IList<object>) list) : (IList) new ReadOnlyCollection<object>((IList<object>) list);
  }
}
