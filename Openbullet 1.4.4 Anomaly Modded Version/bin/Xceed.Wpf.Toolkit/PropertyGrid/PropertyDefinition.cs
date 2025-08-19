// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.PropertyDefinition
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

public class PropertyDefinition : PropertyDefinitionBase
{
  private string _name;
  private bool? _isBrowsable = new bool?(true);
  private bool? _isExpandable;
  private string _displayName;
  private string _description;
  private string _category;
  private int? _displayOrder;

  [Obsolete("Use 'TargetProperties' instead of 'Name'")]
  public string Name
  {
    get => this._name;
    set
    {
      Trace.TraceWarning("{0}: 'Name' property is obsolete. Instead use 'TargetProperties'. (XAML example: <t:PropertyDefinition TargetProperties=\"FirstName,LastName\" />)", (object) typeof (PropertyDefinition));
      this._name = value;
    }
  }

  public string Category
  {
    get => this._category;
    set
    {
      this.ThrowIfLocked<string>((Expression<Func<string>>) (() => this.Category));
      this._category = value;
    }
  }

  public string DisplayName
  {
    get => this._displayName;
    set
    {
      this.ThrowIfLocked<string>((Expression<Func<string>>) (() => this.DisplayName));
      this._displayName = value;
    }
  }

  public string Description
  {
    get => this._description;
    set
    {
      this.ThrowIfLocked<string>((Expression<Func<string>>) (() => this.Description));
      this._description = value;
    }
  }

  public int? DisplayOrder
  {
    get => this._displayOrder;
    set
    {
      this.ThrowIfLocked<int?>((Expression<Func<int?>>) (() => this.DisplayOrder));
      this._displayOrder = value;
    }
  }

  public bool? IsBrowsable
  {
    get => this._isBrowsable;
    set
    {
      this.ThrowIfLocked<bool?>((Expression<Func<bool?>>) (() => this.IsBrowsable));
      this._isBrowsable = value;
    }
  }

  public bool? IsExpandable
  {
    get => this._isExpandable;
    set
    {
      this.ThrowIfLocked<bool?>((Expression<Func<bool?>>) (() => this.IsExpandable));
      this._isExpandable = value;
    }
  }

  internal override void Lock()
  {
    if (this._name != null && this.TargetProperties != null && this.TargetProperties.Count > 0)
      throw new InvalidOperationException($"{typeof (PropertyDefinition)}: When using 'TargetProperties' property, do not use 'Name' property.");
    if (this._name != null)
      this.TargetProperties = (IList) new List<object>()
      {
        (object) this._name
      };
    base.Lock();
  }
}
