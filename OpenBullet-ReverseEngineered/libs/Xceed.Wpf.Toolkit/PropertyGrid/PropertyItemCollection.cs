// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.PropertyItemCollection
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

public class PropertyItemCollection : ReadOnlyObservableCollection<PropertyItem>
{
  internal static readonly string CategoryPropertyName;
  internal static readonly string CategoryOrderPropertyName;
  internal static readonly string PropertyOrderPropertyName;
  internal static readonly string DisplayNamePropertyName;
  private bool _preventNotification;

  static PropertyItemCollection()
  {
    PropertyItem p = (PropertyItem) null;
    PropertyItemCollection.CategoryPropertyName = ReflectionHelper.GetPropertyOrFieldName<string>((Expression<Func<string>>) (() => p.Category));
    PropertyItemCollection.CategoryOrderPropertyName = ReflectionHelper.GetPropertyOrFieldName<int>((Expression<Func<int>>) (() => p.CategoryOrder));
    PropertyItemCollection.PropertyOrderPropertyName = ReflectionHelper.GetPropertyOrFieldName<int>((Expression<Func<int>>) (() => p.PropertyOrder));
    PropertyItemCollection.DisplayNamePropertyName = ReflectionHelper.GetPropertyOrFieldName<string>((Expression<Func<string>>) (() => p.DisplayName));
  }

  public PropertyItemCollection(
    ObservableCollection<PropertyItem> editableCollection)
    : base(editableCollection)
  {
    this.EditableCollection = editableCollection;
  }

  internal Predicate<object> FilterPredicate
  {
    get => this.GetDefaultView().Filter;
    set => this.GetDefaultView().Filter = value;
  }

  public ObservableCollection<PropertyItem> EditableCollection { get; private set; }

  private ICollectionView GetDefaultView() => CollectionViewSource.GetDefaultView((object) this);

  public void GroupBy(string name)
  {
    this.GetDefaultView().GroupDescriptions.Add((GroupDescription) new PropertyGroupDescription(name));
  }

  public void SortBy(string name, ListSortDirection sortDirection)
  {
    this.GetDefaultView().SortDescriptions.Add(new SortDescription(name, sortDirection));
  }

  public void Filter(string text)
  {
    Predicate<object> filter = PropertyItemCollection.CreateFilter(text, this.Items, (IPropertyContainer) null);
    this.GetDefaultView().Filter = filter;
  }

  protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
  {
    if (this._preventNotification)
      return;
    base.OnCollectionChanged(args);
  }

  internal void UpdateItems(IEnumerable<PropertyItem> newItems)
  {
    if (newItems == null)
      throw new ArgumentNullException(nameof (newItems));
    this._preventNotification = true;
    using (this.GetDefaultView().DeferRefresh())
    {
      this.EditableCollection.Clear();
      foreach (PropertyItem newItem in newItems)
        this.EditableCollection.Add(newItem);
    }
    this._preventNotification = false;
    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
  }

  internal void UpdateCategorization(
    GroupDescription groupDescription,
    bool isPropertyGridCategorized,
    bool sortAlphabetically)
  {
    foreach (PropertyItem propertyItem in (IEnumerable<PropertyItem>) this.Items)
    {
      propertyItem.DescriptorDefinition.DisplayOrder = propertyItem.DescriptorDefinition.ComputeDisplayOrderInternal(isPropertyGridCategorized);
      propertyItem.PropertyOrder = propertyItem.DescriptorDefinition.DisplayOrder;
    }
    ICollectionView defaultView = this.GetDefaultView();
    using (defaultView.DeferRefresh())
    {
      defaultView.GroupDescriptions.Clear();
      defaultView.SortDescriptions.Clear();
      if (groupDescription != null)
      {
        defaultView.GroupDescriptions.Add(groupDescription);
        if (sortAlphabetically)
        {
          this.SortBy(PropertyItemCollection.CategoryOrderPropertyName, ListSortDirection.Ascending);
          this.SortBy(PropertyItemCollection.CategoryPropertyName, ListSortDirection.Ascending);
        }
      }
      if (!sortAlphabetically)
        return;
      this.SortBy(PropertyItemCollection.PropertyOrderPropertyName, ListSortDirection.Ascending);
      this.SortBy(PropertyItemCollection.DisplayNamePropertyName, ListSortDirection.Ascending);
    }
  }

  internal void RefreshView() => this.GetDefaultView().Refresh();

  internal static Predicate<object> CreateFilter(
    string text,
    IList<PropertyItem> PropertyItems,
    IPropertyContainer propertyContainer)
  {
    Predicate<object> filter = (Predicate<object>) null;
    if (!string.IsNullOrEmpty(text))
      filter = (Predicate<object>) (item =>
      {
        PropertyItem propertyItem = item as PropertyItem;
        if (propertyItem.DisplayName == null)
          return false;
        DisplayAttribute attribute = PropertyGridUtilities.GetAttribute<DisplayAttribute>(propertyItem.PropertyDescriptor);
        if (attribute != null)
        {
          bool? autoGenerateFilter = attribute.GetAutoGenerateFilter();
          if (autoGenerateFilter.HasValue && !autoGenerateFilter.Value)
            return false;
        }
        propertyItem.HighlightedText = propertyItem.DisplayName.ToLower().Contains(text.ToLower()) ? text : (string) null;
        return propertyItem.HighlightedText != null;
      });
    return filter;
  }
}
