// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.DefinitionBase
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

public abstract class DefinitionBase : DependencyObject
{
  private bool _isLocked;

  internal bool IsLocked => this._isLocked;

  internal void ThrowIfLocked<TMember>(Expression<Func<TMember>> propertyExpression)
  {
    if (!DesignerProperties.GetIsInDesignMode((DependencyObject) this) && this.IsLocked)
      throw new InvalidOperationException($"Cannot modify {ReflectionHelper.GetPropertyOrFieldName<TMember>(propertyExpression)} once the definition has beed added to a collection.");
  }

  internal virtual void Lock()
  {
    if (this._isLocked)
      return;
    this._isLocked = true;
  }
}
