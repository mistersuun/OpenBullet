// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.ContainerHelperBase
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

internal abstract class ContainerHelperBase
{
  protected readonly IPropertyContainer PropertyContainer;
  internal static readonly DependencyProperty IsGeneratedProperty = DependencyProperty.RegisterAttached("IsGenerated", typeof (bool), typeof (ContainerHelperBase), new PropertyMetadata((object) false));

  public ContainerHelperBase(IPropertyContainer propertyContainer)
  {
    this.PropertyContainer = propertyContainer != null ? propertyContainer : throw new ArgumentNullException(nameof (propertyContainer));
    if (!(propertyContainer is INotifyPropertyChanged notifyPropertyChanged))
      return;
    notifyPropertyChanged.PropertyChanged += new PropertyChangedEventHandler(this.OnPropertyContainerPropertyChanged);
  }

  internal static bool GetIsGenerated(DependencyObject obj)
  {
    return (bool) obj.GetValue(ContainerHelperBase.IsGeneratedProperty);
  }

  internal static void SetIsGenerated(DependencyObject obj, bool value)
  {
    obj.SetValue(ContainerHelperBase.IsGeneratedProperty, (object) value);
  }

  public abstract IList Properties { get; }

  internal ItemsControl ChildrenItemsControl { get; set; }

  internal bool IsCleaning { get; private set; }

  public virtual void ClearHelper()
  {
    this.IsCleaning = true;
    if (this.PropertyContainer is INotifyPropertyChanged propertyContainer)
      propertyContainer.PropertyChanged -= new PropertyChangedEventHandler(this.OnPropertyContainerPropertyChanged);
    if (this.ChildrenItemsControl != null)
      ((IItemContainerGenerator) this.ChildrenItemsControl.ItemContainerGenerator).RemoveAll();
    this.IsCleaning = false;
  }

  public virtual void PrepareChildrenPropertyItem(PropertyItemBase propertyItem, object item)
  {
    propertyItem.ParentNode = this.PropertyContainer;
    Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.RaisePreparePropertyItemEvent((UIElement) this.PropertyContainer, propertyItem, item);
  }

  public virtual void ClearChildrenPropertyItem(PropertyItemBase propertyItem, object item)
  {
    propertyItem.ParentNode = (IPropertyContainer) null;
    Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.RaiseClearPropertyItemEvent((UIElement) this.PropertyContainer, propertyItem, item);
  }

  protected FrameworkElement GenerateCustomEditingElement(
    Type definitionKey,
    PropertyItemBase propertyItem)
  {
    return this.PropertyContainer.EditorDefinitions == null ? (FrameworkElement) null : this.CreateCustomEditor(this.PropertyContainer.EditorDefinitions.GetRecursiveBaseTypes(definitionKey), propertyItem);
  }

  protected FrameworkElement GenerateCustomEditingElement(
    object definitionKey,
    PropertyItemBase propertyItem)
  {
    return this.PropertyContainer.EditorDefinitions == null ? (FrameworkElement) null : this.CreateCustomEditor(this.PropertyContainer.EditorDefinitions[definitionKey], propertyItem);
  }

  protected FrameworkElement CreateCustomEditor(
    EditorDefinitionBase customEditor,
    PropertyItemBase propertyItem)
  {
    return customEditor?.GenerateEditingElementInternal(propertyItem);
  }

  protected virtual void OnPropertyContainerPropertyChanged(
    object sender,
    PropertyChangedEventArgs e)
  {
    string propertyName = e.PropertyName;
    IPropertyContainer ps = (IPropertyContainer) null;
    if (propertyName == ReflectionHelper.GetPropertyOrFieldName<FilterInfo>((Expression<Func<FilterInfo>>) (() => ps.FilterInfo)))
      this.OnFilterChanged();
    else if (propertyName == ReflectionHelper.GetPropertyOrFieldName<bool>((Expression<Func<bool>>) (() => ps.IsCategorized)))
      this.OnCategorizationChanged();
    else if (propertyName == ReflectionHelper.GetPropertyOrFieldName<bool>((Expression<Func<bool>>) (() => ps.AutoGenerateProperties)))
      this.OnAutoGeneratePropertiesChanged();
    else if (propertyName == ReflectionHelper.GetPropertyOrFieldName<bool>((Expression<Func<bool>>) (() => ps.HideInheritedProperties)))
      this.OnHideInheritedPropertiesChanged();
    else if (propertyName == ReflectionHelper.GetPropertyOrFieldName<EditorDefinitionCollection>((Expression<Func<EditorDefinitionCollection>>) (() => ps.EditorDefinitions)))
    {
      this.OnEditorDefinitionsChanged();
    }
    else
    {
      if (!(propertyName == ReflectionHelper.GetPropertyOrFieldName<PropertyDefinitionCollection>((Expression<Func<PropertyDefinitionCollection>>) (() => ps.PropertyDefinitions))))
        return;
      this.OnPropertyDefinitionsChanged();
    }
  }

  protected virtual void OnCategorizationChanged()
  {
  }

  protected virtual void OnFilterChanged()
  {
  }

  protected virtual void OnAutoGeneratePropertiesChanged()
  {
  }

  protected virtual void OnHideInheritedPropertiesChanged()
  {
  }

  protected virtual void OnEditorDefinitionsChanged()
  {
  }

  protected virtual void OnPropertyDefinitionsChanged()
  {
  }

  public virtual void OnEndInit()
  {
  }

  public abstract PropertyItemBase ContainerFromItem(object item);

  public abstract object ItemFromContainer(PropertyItemBase container);

  public abstract Binding CreateChildrenDefaultBinding(PropertyItemBase propertyItem);

  public virtual void NotifyEditorDefinitionsCollectionChanged()
  {
  }

  public virtual void NotifyPropertyDefinitionsCollectionChanged()
  {
  }

  public abstract void UpdateValuesFromSource();

  protected internal virtual void SetPropertiesExpansion(bool isExpanded)
  {
    foreach (object property in (IEnumerable) this.Properties)
    {
      if (property is PropertyItemBase propertyItemBase && propertyItemBase.IsExpandable)
      {
        if (propertyItemBase.ContainerHelper != null)
          propertyItemBase.ContainerHelper.SetPropertiesExpansion(isExpanded);
        propertyItemBase.IsExpanded = isExpanded;
      }
    }
  }

  protected internal virtual void SetPropertiesExpansion(string propertyName, bool isExpanded)
  {
    foreach (object property in (IEnumerable) this.Properties)
    {
      if (property is PropertyItemBase propertyItemBase && propertyItemBase.IsExpandable)
      {
        if (propertyItemBase.DisplayName == propertyName)
        {
          propertyItemBase.IsExpanded = isExpanded;
          break;
        }
        if (propertyItemBase.ContainerHelper != null)
          propertyItemBase.ContainerHelper.SetPropertiesExpansion(propertyName, isExpanded);
      }
    }
  }
}
