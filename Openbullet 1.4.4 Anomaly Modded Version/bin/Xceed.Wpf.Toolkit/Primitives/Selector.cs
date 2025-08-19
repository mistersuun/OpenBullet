// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Primitives.Selector
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.Primitives;

public class Selector : ItemsControl, IWeakEventListener
{
  private bool _surpressItemSelectionChanged;
  private bool _ignoreSelectedItemChanged;
  private bool _ignoreSelectedValueChanged;
  private int _ignoreSelectedItemsCollectionChanged;
  private int _ignoreSelectedMemberPathValuesChanged;
  private IList _selectedItems;
  private IList _removedItems = (IList) new ObservableCollection<object>();
  private object[] _internalSelectedItems;
  private ValueChangeHelper _selectedMemberPathValuesHelper;
  private ValueChangeHelper _valueMemberPathValuesHelper;
  public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof (Command), typeof (ICommand), typeof (Selector), new PropertyMetadata((object) null));
  public static readonly DependencyProperty DelimiterProperty = DependencyProperty.Register(nameof (Delimiter), typeof (string), typeof (Selector), (PropertyMetadata) new UIPropertyMetadata((object) ",", new PropertyChangedCallback(Selector.OnDelimiterChanged)));
  public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof (SelectedItem), typeof (object), typeof (Selector), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(Selector.OnSelectedItemChanged)));
  public static readonly DependencyProperty SelectedItemsOverrideProperty = DependencyProperty.Register(nameof (SelectedItemsOverride), typeof (IList), typeof (Selector), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(Selector.SelectedItemsOverrideChanged)));
  public static readonly DependencyProperty SelectedMemberPathProperty = DependencyProperty.Register(nameof (SelectedMemberPath), typeof (string), typeof (Selector), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(Selector.OnSelectedMemberPathChanged)));
  public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register(nameof (SelectedValue), typeof (string), typeof (Selector), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(Selector.OnSelectedValueChanged)));
  public static readonly DependencyProperty ValueMemberPathProperty = DependencyProperty.Register(nameof (ValueMemberPath), typeof (string), typeof (Selector), (PropertyMetadata) new UIPropertyMetadata(new PropertyChangedCallback(Selector.OnValueMemberPathChanged)));
  public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent(nameof (SelectedEvent), RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (Selector));
  public static readonly RoutedEvent UnSelectedEvent = EventManager.RegisterRoutedEvent(nameof (UnSelectedEvent), RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (Selector));
  public static readonly RoutedEvent ItemSelectionChangedEvent = EventManager.RegisterRoutedEvent("ItemSelectionChanged", RoutingStrategy.Bubble, typeof (ItemSelectionChangedEventHandler), typeof (Selector));

  public Selector()
  {
    this.SelectedItems = (IList) new ObservableCollection<object>();
    this.AddHandler(Selector.SelectedEvent, (Delegate) ((s, args) => this.OnItemSelectionChangedCore(args, false)));
    this.AddHandler(Selector.UnSelectedEvent, (Delegate) ((s, args) => this.OnItemSelectionChangedCore(args, true)));
    this._selectedMemberPathValuesHelper = new ValueChangeHelper(new Action(this.OnSelectedMemberPathValuesChanged));
    this._valueMemberPathValuesHelper = new ValueChangeHelper(new Action(this.OnValueMemberPathValuesChanged));
  }

  [TypeConverter(typeof (CommandConverter))]
  public ICommand Command
  {
    get => (ICommand) this.GetValue(Selector.CommandProperty);
    set => this.SetValue(Selector.CommandProperty, (object) value);
  }

  public string Delimiter
  {
    get => (string) this.GetValue(Selector.DelimiterProperty);
    set => this.SetValue(Selector.DelimiterProperty, (object) value);
  }

  private static void OnDelimiterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    ((Selector) o).OnSelectedItemChanged((string) e.OldValue, (string) e.NewValue);
  }

  protected virtual void OnSelectedItemChanged(string oldValue, string newValue)
  {
    if (!this.IsInitialized)
      return;
    this.UpdateSelectedValue();
  }

  public object SelectedItem
  {
    get => this.GetValue(Selector.SelectedItemProperty);
    set => this.SetValue(Selector.SelectedItemProperty, value);
  }

  private static void OnSelectedItemChanged(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs args)
  {
    ((Selector) sender).OnSelectedItemChanged(args.OldValue, args.NewValue);
  }

  protected virtual void OnSelectedItemChanged(object oldValue, object newValue)
  {
    if (!this.IsInitialized || this._ignoreSelectedItemChanged)
      return;
    ++this._ignoreSelectedItemsCollectionChanged;
    this.SelectedItems.Clear();
    if (newValue != null)
      this.SelectedItems.Add(newValue);
    this.UpdateFromSelectedItems();
    --this._ignoreSelectedItemsCollectionChanged;
  }

  public IList SelectedItems
  {
    get => this._selectedItems;
    private set
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      INotifyCollectionChanged selectedItems1 = this._selectedItems as INotifyCollectionChanged;
      INotifyCollectionChanged source = value as INotifyCollectionChanged;
      if (selectedItems1 != null)
        CollectionChangedEventManager.RemoveListener(selectedItems1, (IWeakEventListener) this);
      if (source != null)
        CollectionChangedEventManager.AddListener(source, (IWeakEventListener) this);
      IList list = value;
      IList selectedItems2 = this._selectedItems;
      if (selectedItems2 != null)
      {
        foreach (object parameter in (IEnumerable) selectedItems2)
        {
          if (list != null && !list.Contains(parameter) || list == null)
          {
            this.OnItemSelectionChanged(new ItemSelectionChangedEventArgs(Selector.ItemSelectionChangedEvent, (object) this, parameter, false));
            if (this.Command != null)
              this.Command.Execute(parameter);
          }
        }
      }
      if (list != null)
      {
        foreach (object parameter in (IEnumerable) list)
        {
          this.OnItemSelectionChanged(new ItemSelectionChangedEventArgs(Selector.ItemSelectionChangedEvent, (object) this, parameter, true));
          if ((selectedItems2 != null && !selectedItems2.Contains(parameter) || selectedItems2 == null) && this.Command != null)
            this.Command.Execute(parameter);
        }
      }
      this._selectedItems = value;
    }
  }

  public IList SelectedItemsOverride
  {
    get => (IList) this.GetValue(Selector.SelectedItemsOverrideProperty);
    set => this.SetValue(Selector.SelectedItemsOverrideProperty, (object) value);
  }

  private static void SelectedItemsOverrideChanged(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs args)
  {
    ((Selector) sender).OnSelectedItemsOverrideChanged((IList) args.OldValue, (IList) args.NewValue);
  }

  protected virtual void OnSelectedItemsOverrideChanged(IList oldValue, IList newValue)
  {
    if (!this.IsInitialized)
      return;
    this.SelectedItems = newValue != null ? newValue : (IList) new ObservableCollection<object>();
    this.UpdateFromSelectedItems();
  }

  public string SelectedMemberPath
  {
    get => (string) this.GetValue(Selector.SelectedMemberPathProperty);
    set => this.SetValue(Selector.SelectedMemberPathProperty, (object) value);
  }

  private static void OnSelectedMemberPathChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    ((Selector) o).OnSelectedMemberPathChanged((string) e.OldValue, (string) e.NewValue);
  }

  protected virtual void OnSelectedMemberPathChanged(string oldValue, string newValue)
  {
    if (!this.IsInitialized)
      return;
    this.UpdateSelectedMemberPathValuesBindings();
  }

  public string SelectedValue
  {
    get => (string) this.GetValue(Selector.SelectedValueProperty);
    set => this.SetValue(Selector.SelectedValueProperty, (object) value);
  }

  private static void OnSelectedValueChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is Selector selector))
      return;
    selector.OnSelectedValueChanged((string) e.OldValue, (string) e.NewValue);
  }

  protected virtual void OnSelectedValueChanged(string oldValue, string newValue)
  {
    if (!this.IsInitialized || this._ignoreSelectedValueChanged)
      return;
    this.UpdateFromSelectedValue();
  }

  public string ValueMemberPath
  {
    get => (string) this.GetValue(Selector.ValueMemberPathProperty);
    set => this.SetValue(Selector.ValueMemberPathProperty, (object) value);
  }

  private static void OnValueMemberPathChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    ((Selector) o).OnValueMemberPathChanged((string) e.OldValue, (string) e.NewValue);
  }

  protected virtual void OnValueMemberPathChanged(string oldValue, string newValue)
  {
    if (!this.IsInitialized)
      return;
    this.UpdateValueMemberPathValuesBindings();
  }

  protected IEnumerable ItemsCollection
  {
    get => this.ItemsSource ?? (IEnumerable) this.Items ?? (IEnumerable) new object[0];
  }

  protected override bool IsItemItsOwnContainerOverride(object item) => item is SelectorItem;

  protected override DependencyObject GetContainerForItemOverride()
  {
    return (DependencyObject) new SelectorItem();
  }

  protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
  {
    base.PrepareContainerForItemOverride(element, item);
    this._surpressItemSelectionChanged = true;
    (element as FrameworkElement).SetValue(SelectorItem.IsSelectedProperty, (object) this.SelectedItems.Contains(item));
    this._surpressItemSelectionChanged = false;
  }

  protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
  {
    base.OnItemsSourceChanged(oldValue, newValue);
    INotifyCollectionChanged source1 = oldValue as INotifyCollectionChanged;
    INotifyCollectionChanged source2 = newValue as INotifyCollectionChanged;
    if (source1 != null)
      CollectionChangedEventManager.RemoveListener(source1, (IWeakEventListener) this);
    if (source2 != null)
      CollectionChangedEventManager.AddListener(source2, (IWeakEventListener) this);
    if (!this.IsInitialized)
      return;
    if (!VirtualizingStackPanel.GetIsVirtualizing((DependencyObject) this) || VirtualizingStackPanel.GetIsVirtualizing((DependencyObject) this) && newValue != null)
      this.RemoveUnavailableSelectedItems();
    this.UpdateSelectedMemberPathValuesBindings();
    this.UpdateValueMemberPathValuesBindings();
  }

  protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
  {
    base.OnItemsChanged(e);
    this.RemoveUnavailableSelectedItems();
  }

  public override void EndInit()
  {
    base.EndInit();
    if (this.SelectedItemsOverride != null)
      this.OnSelectedItemsOverrideChanged((IList) null, this.SelectedItemsOverride);
    else if (this.SelectedMemberPath != null)
      this.OnSelectedMemberPathChanged((string) null, this.SelectedMemberPath);
    else if (this.SelectedValue != null)
      this.OnSelectedValueChanged((string) null, this.SelectedValue);
    else if (this.SelectedItem != null)
      this.OnSelectedItemChanged((object) null, this.SelectedItem);
    if (this.ValueMemberPath == null)
      return;
    this.OnValueMemberPathChanged((string) null, this.ValueMemberPath);
  }

  public event ItemSelectionChangedEventHandler ItemSelectionChanged
  {
    add => this.AddHandler(Selector.ItemSelectionChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(Selector.ItemSelectionChangedEvent, (Delegate) value);
  }

  protected object GetPathValue(object item, string propertyPath)
  {
    if (item == null)
      throw new ArgumentNullException(nameof (item));
    if (string.IsNullOrEmpty(propertyPath) || propertyPath == ".")
      return item;
    PropertyInfo property = item.GetType().GetProperty(propertyPath);
    return !(property != (PropertyInfo) null) ? (object) null : property.GetValue(item, (object[]) null);
  }

  protected object GetItemValue(object item)
  {
    return item == null ? (object) null : this.GetPathValue(item, this.ValueMemberPath);
  }

  protected object ResolveItemByValue(string value)
  {
    if (!string.IsNullOrEmpty(this.ValueMemberPath))
    {
      foreach (object items in this.ItemsCollection)
      {
        PropertyInfo property = items.GetType().GetProperty(this.ValueMemberPath);
        if (property != (PropertyInfo) null)
        {
          object obj = property.GetValue(items, (object[]) null);
          if (value.Equals(obj.ToString(), StringComparison.InvariantCultureIgnoreCase))
            return items;
        }
      }
    }
    return (object) value;
  }

  internal void UpdateFromList(List<string> selectedValues, Func<object, object> GetItemfunction)
  {
    ++this._ignoreSelectedItemsCollectionChanged;
    this.SelectedItems.Clear();
    if (selectedValues != null && selectedValues.Count > 0)
    {
      Selector.ValueEqualityComparer comparer = new Selector.ValueEqualityComparer();
      foreach (object items in this.ItemsCollection)
      {
        object obj = GetItemfunction(items);
        if ((obj == null ? 0 : (selectedValues.Contains<string>(obj.ToString(), (IEqualityComparer<string>) comparer) ? 1 : 0)) != 0)
          this.SelectedItems.Add(items);
      }
    }
    --this._ignoreSelectedItemsCollectionChanged;
    this.UpdateFromSelectedItems();
  }

  private bool? GetSelectedMemberPathValue(object item)
  {
    if (string.IsNullOrEmpty(this.SelectedMemberPath))
      return new bool?();
    if (item == null)
      return new bool?();
    string[] source = this.SelectedMemberPath.Split('.');
    if (source.Length == 1)
    {
      PropertyInfo property = item.GetType().GetProperty(this.SelectedMemberPath);
      return property != (PropertyInfo) null && property.PropertyType == typeof (bool) ? property.GetValue(item, (object[]) null) as bool? : new bool?();
    }
    for (int index = 0; index < ((IEnumerable<string>) source).Count<string>(); ++index)
    {
      PropertyInfo property = item.GetType().GetProperty(source[index]);
      if (property == (PropertyInfo) null)
        return new bool?();
      if (index == ((IEnumerable<string>) source).Count<string>() - 1)
      {
        if (property.PropertyType == typeof (bool))
          return property.GetValue(item, (object[]) null) as bool?;
      }
      else
        item = property.GetValue(item, (object[]) null);
    }
    return new bool?();
  }

  private void SetSelectedMemberPathValue(object item, bool value)
  {
    if (string.IsNullOrEmpty(this.SelectedMemberPath) || item == null)
      return;
    string[] source = this.SelectedMemberPath.Split('.');
    if (source.Length == 1)
    {
      PropertyInfo property = item.GetType().GetProperty(this.SelectedMemberPath);
      if (!(property != (PropertyInfo) null) || !(property.PropertyType == typeof (bool)))
        return;
      property.SetValue(item, (object) value, (object[]) null);
    }
    else
    {
      for (int index = 0; index < ((IEnumerable<string>) source).Count<string>(); ++index)
      {
        PropertyInfo property = item.GetType().GetProperty(source[index]);
        if (property == (PropertyInfo) null)
          break;
        if (index == ((IEnumerable<string>) source).Count<string>() - 1)
        {
          if (property.PropertyType == typeof (bool))
            property.SetValue(item, (object) value, (object[]) null);
        }
        else
          item = property.GetValue(item, (object[]) null);
      }
    }
  }

  protected virtual void OnSelectedItemsCollectionChanged(
    object sender,
    NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == NotifyCollectionChangedAction.Reset && this._internalSelectedItems != null)
    {
      foreach (object internalSelectedItem in this._internalSelectedItems)
      {
        this.OnItemSelectionChanged(new ItemSelectionChangedEventArgs(Selector.ItemSelectionChangedEvent, (object) this, internalSelectedItem, false));
        if (this.Command != null)
          this.Command.Execute(internalSelectedItem);
      }
    }
    if (e.OldItems != null)
    {
      foreach (object oldItem in (IEnumerable) e.OldItems)
      {
        this.OnItemSelectionChanged(new ItemSelectionChangedEventArgs(Selector.ItemSelectionChangedEvent, (object) this, oldItem, false));
        if (this.Command != null)
          this.Command.Execute(oldItem);
      }
    }
    if (e.NewItems != null)
    {
      foreach (object newItem in (IEnumerable) e.NewItems)
      {
        this.OnItemSelectionChanged(new ItemSelectionChangedEventArgs(Selector.ItemSelectionChangedEvent, (object) this, newItem, true));
        if (this.Command != null)
          this.Command.Execute(newItem);
      }
    }
    if (this._ignoreSelectedItemsCollectionChanged > 0)
      return;
    this.UpdateFromSelectedItems();
  }

  private void OnItemSelectionChangedCore(RoutedEventArgs args, bool unselected)
  {
    object obj = this.ItemContainerGenerator.ItemFromContainer((DependencyObject) args.OriginalSource);
    if (obj == DependencyProperty.UnsetValue)
      obj = args.OriginalSource;
    if (unselected)
    {
      while (this.SelectedItems.Contains(obj))
        this.SelectedItems.Remove(obj);
    }
    else
    {
      if (this.SelectedItems.Contains(obj))
        return;
      this.SelectedItems.Add(obj);
    }
  }

  private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
  {
    this.RemoveUnavailableSelectedItems();
    this.AddAvailableRemovedItems();
    this.UpdateSelectedMemberPathValuesBindings();
    this.UpdateValueMemberPathValuesBindings();
  }

  private void OnSelectedMemberPathValuesChanged()
  {
    if (this._ignoreSelectedMemberPathValuesChanged > 0)
      return;
    this.UpdateFromSelectedMemberPathValues();
  }

  private void OnValueMemberPathValuesChanged() => this.UpdateSelectedValue();

  private void UpdateSelectedMemberPathValuesBindings()
  {
    this._selectedMemberPathValuesHelper.UpdateValueSource(this.ItemsCollection, this.SelectedMemberPath);
    this.UpdateFromSelectedMemberPathValues();
  }

  private void UpdateValueMemberPathValuesBindings()
  {
    this._valueMemberPathValuesHelper.UpdateValueSource(this.ItemsCollection, this.ValueMemberPath);
  }

  protected virtual void OnItemSelectionChanged(ItemSelectionChangedEventArgs args)
  {
    if (this._surpressItemSelectionChanged)
      return;
    this.RaiseEvent((RoutedEventArgs) args);
  }

  private void UpdateSelectedValue()
  {
    string str = string.Join<object>(this.Delimiter, this.SelectedItems.Cast<object>().Select<object, object>((Func<object, object>) (x => this.GetItemValue(x))));
    if (!string.IsNullOrEmpty(this.SelectedValue) && this.SelectedValue.Equals(str))
      return;
    this._ignoreSelectedValueChanged = true;
    this.SelectedValue = str;
    this._ignoreSelectedValueChanged = false;
  }

  private void UpdateSelectedItem()
  {
    if (this.SelectedItems.Contains(this.SelectedItem))
      return;
    this._ignoreSelectedItemChanged = true;
    this.SelectedItem = this.SelectedItems.Count > 0 ? this.SelectedItems[0] : (object) null;
    this._ignoreSelectedItemChanged = false;
  }

  private void UpdateFromSelectedMemberPathValues()
  {
    ++this._ignoreSelectedItemsCollectionChanged;
    foreach (object items in this.ItemsCollection)
    {
      bool? selectedMemberPathValue = this.GetSelectedMemberPathValue(items);
      if (selectedMemberPathValue.HasValue)
      {
        if (selectedMemberPathValue.Value)
        {
          if (!this.SelectedItems.Contains(items))
            this.SelectedItems.Add(items);
        }
        else if (this.SelectedItems.Contains(items))
          this.SelectedItems.Remove(items);
      }
    }
    --this._ignoreSelectedItemsCollectionChanged;
    this.UpdateFromSelectedItems();
  }

  internal void UpdateSelectedItems(IList selectedItems)
  {
    if (selectedItems == null)
      throw new ArgumentNullException(nameof (selectedItems));
    if (selectedItems.Count == this.SelectedItems.Count && selectedItems.Cast<object>().SequenceEqual<object>(this.SelectedItems.Cast<object>()))
      return;
    ++this._ignoreSelectedItemsCollectionChanged;
    this.SelectedItems.Clear();
    foreach (object selectedItem in (IEnumerable) selectedItems)
      this.SelectedItems.Add(selectedItem);
    --this._ignoreSelectedItemsCollectionChanged;
    this.UpdateFromSelectedItems();
  }

  private void UpdateFromSelectedItems()
  {
    foreach (object items in this.ItemsCollection)
    {
      bool flag = this.SelectedItems.Contains(items);
      ++this._ignoreSelectedMemberPathValuesChanged;
      this.SetSelectedMemberPathValue(items, flag);
      --this._ignoreSelectedMemberPathValuesChanged;
      if (this.ItemContainerGenerator.ContainerFromItem(items) is SelectorItem selectorItem)
        selectorItem.IsSelected = new bool?(flag);
    }
    this.UpdateSelectedItem();
    this.UpdateSelectedValue();
    this._internalSelectedItems = new object[this.SelectedItems.Count];
    this.SelectedItems.CopyTo((Array) this._internalSelectedItems, 0);
  }

  private void RemoveUnavailableSelectedItems()
  {
    ++this._ignoreSelectedItemsCollectionChanged;
    HashSet<object> objectSet = new HashSet<object>(this.ItemsCollection.Cast<object>());
    for (int index = 0; index < this.SelectedItems.Count; ++index)
    {
      if (!objectSet.Contains(this.SelectedItems[index]))
      {
        this._removedItems.Add(this.SelectedItems[index]);
        this.SelectedItems.RemoveAt(index);
        --index;
      }
    }
    --this._ignoreSelectedItemsCollectionChanged;
    this.UpdateSelectedItem();
    this.UpdateSelectedValue();
  }

  private void AddAvailableRemovedItems()
  {
    HashSet<object> objectSet = new HashSet<object>(this.ItemsCollection.Cast<object>());
    for (int index = 0; index < this._removedItems.Count; ++index)
    {
      if (objectSet.Contains(this._removedItems[index]))
      {
        this.SelectedItems.Add(this._removedItems[index]);
        this._removedItems.RemoveAt(index);
        --index;
      }
    }
  }

  private void UpdateFromSelectedValue()
  {
    List<string> selectedValues = (List<string>) null;
    if (!string.IsNullOrEmpty(this.SelectedValue))
      selectedValues = ((IEnumerable<string>) this.SelectedValue.Split(new string[1]
      {
        this.Delimiter
      }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
    this.UpdateFromList(selectedValues, new Func<object, object>(this.GetItemValue));
  }

  public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
  {
    if (managerType == typeof (CollectionChangedEventManager))
    {
      if (this._selectedItems == sender)
      {
        this.OnSelectedItemsCollectionChanged(sender, (NotifyCollectionChangedEventArgs) e);
        return true;
      }
      if (this.ItemsCollection == sender)
      {
        this.OnItemsSourceCollectionChanged(sender, (NotifyCollectionChangedEventArgs) e);
        return true;
      }
    }
    return false;
  }

  private class ValueEqualityComparer : IEqualityComparer<string>
  {
    public bool Equals(string x, string y)
    {
      return string.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);
    }

    public int GetHashCode(string obj) => 1;
  }
}
