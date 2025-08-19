// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.CollectionControl
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.PropertyGrid;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[TemplatePart(Name = "PART_NewItemTypesComboBox", Type = typeof (ComboBox))]
[TemplatePart(Name = "PART_PropertyGrid", Type = typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid))]
[TemplatePart(Name = "PART_ListBox", Type = typeof (ListBox))]
public class CollectionControl : Control
{
  private const string PART_NewItemTypesComboBox = "PART_NewItemTypesComboBox";
  private const string PART_PropertyGrid = "PART_PropertyGrid";
  private const string PART_ListBox = "PART_ListBox";
  private ComboBox _newItemTypesComboBox;
  private Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid _propertyGrid;
  private ListBox _listBox;
  public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof (IsReadOnly), typeof (bool), typeof (CollectionControl), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof (Items), typeof (ObservableCollection<object>), typeof (CollectionControl), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof (ItemsSource), typeof (IEnumerable), typeof (CollectionControl), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(CollectionControl.OnItemsSourceChanged)));
  public static readonly DependencyProperty ItemsSourceTypeProperty = DependencyProperty.Register(nameof (ItemsSourceType), typeof (Type), typeof (CollectionControl), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty NewItemTypesProperty = DependencyProperty.Register(nameof (NewItemTypes), typeof (IList), typeof (CollectionControl), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty PropertiesLabelProperty = DependencyProperty.Register(nameof (PropertiesLabel), typeof (object), typeof (CollectionControl), (PropertyMetadata) new UIPropertyMetadata((object) "Properties:"));
  public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof (SelectedItem), typeof (object), typeof (CollectionControl), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty TypeSelectionLabelProperty = DependencyProperty.Register(nameof (TypeSelectionLabel), typeof (object), typeof (CollectionControl), (PropertyMetadata) new UIPropertyMetadata((object) "Select type:"));
  public static readonly DependencyProperty EditorDefinitionsProperty = DependencyProperty.Register(nameof (EditorDefinitions), typeof (EditorDefinitionCollection), typeof (CollectionControl), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly RoutedEvent ItemDeletingEvent = EventManager.RegisterRoutedEvent("ItemDeleting", RoutingStrategy.Bubble, typeof (CollectionControl.ItemDeletingRoutedEventHandler), typeof (CollectionControl));
  public static readonly RoutedEvent ItemDeletedEvent = EventManager.RegisterRoutedEvent("ItemDeleted", RoutingStrategy.Bubble, typeof (CollectionControl.ItemDeletedRoutedEventHandler), typeof (CollectionControl));
  public static readonly RoutedEvent ItemAddingEvent = EventManager.RegisterRoutedEvent("ItemAdding", RoutingStrategy.Bubble, typeof (CollectionControl.ItemAddingRoutedEventHandler), typeof (CollectionControl));
  public static readonly RoutedEvent ItemAddedEvent = EventManager.RegisterRoutedEvent("ItemAdded", RoutingStrategy.Bubble, typeof (CollectionControl.ItemAddedRoutedEventHandler), typeof (CollectionControl));
  public static readonly RoutedEvent ItemMovedDownEvent = EventManager.RegisterRoutedEvent("ItemMovedDown", RoutingStrategy.Bubble, typeof (CollectionControl.ItemMovedDownRoutedEventHandler), typeof (CollectionControl));
  public static readonly RoutedEvent ItemMovedUpEvent = EventManager.RegisterRoutedEvent("ItemMovedUp", RoutingStrategy.Bubble, typeof (CollectionControl.ItemMovedUpRoutedEventHandler), typeof (CollectionControl));

  public bool IsReadOnly
  {
    get => (bool) this.GetValue(CollectionControl.IsReadOnlyProperty);
    set => this.SetValue(CollectionControl.IsReadOnlyProperty, (object) value);
  }

  public ObservableCollection<object> Items
  {
    get => (ObservableCollection<object>) this.GetValue(CollectionControl.ItemsProperty);
    set => this.SetValue(CollectionControl.ItemsProperty, (object) value);
  }

  public IEnumerable ItemsSource
  {
    get => (IEnumerable) this.GetValue(CollectionControl.ItemsSourceProperty);
    set => this.SetValue(CollectionControl.ItemsSourceProperty, (object) value);
  }

  private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((CollectionControl) d)?.OnItemSourceChanged((IEnumerable) e.OldValue, (IEnumerable) e.NewValue);
  }

  public void OnItemSourceChanged(IEnumerable oldValue, IEnumerable newValue)
  {
    if (newValue == null)
      return;
    if (newValue is IDictionary dictionary)
    {
      foreach (DictionaryEntry dictionaryEntry in dictionary)
      {
        Type keyType = dictionaryEntry.Key != null ? dictionaryEntry.Key.GetType() : (((IEnumerable<Type>) dictionary.GetType().GetGenericArguments()).Count<Type>() > 0 ? dictionary.GetType().GetGenericArguments()[0] : typeof (object));
        Type valueType = dictionaryEntry.Value != null ? dictionaryEntry.Value.GetType() : (((IEnumerable<Type>) dictionary.GetType().GetGenericArguments()).Count<Type>() > 1 ? dictionary.GetType().GetGenericArguments()[1] : typeof (object));
        this.Items.Add(ListUtilities.CreateEditableKeyValuePair(dictionaryEntry.Key, keyType, dictionaryEntry.Value, valueType));
      }
    }
    else
    {
      foreach (object obj in newValue)
      {
        if (obj != null)
          this.Items.Add(obj);
      }
    }
  }

  public Type ItemsSourceType
  {
    get => (Type) this.GetValue(CollectionControl.ItemsSourceTypeProperty);
    set => this.SetValue(CollectionControl.ItemsSourceTypeProperty, (object) value);
  }

  public IList<Type> NewItemTypes
  {
    get => (IList<Type>) this.GetValue(CollectionControl.NewItemTypesProperty);
    set => this.SetValue(CollectionControl.NewItemTypesProperty, (object) value);
  }

  public object PropertiesLabel
  {
    get => this.GetValue(CollectionControl.PropertiesLabelProperty);
    set => this.SetValue(CollectionControl.PropertiesLabelProperty, value);
  }

  public object SelectedItem
  {
    get => this.GetValue(CollectionControl.SelectedItemProperty);
    set => this.SetValue(CollectionControl.SelectedItemProperty, value);
  }

  public object TypeSelectionLabel
  {
    get => this.GetValue(CollectionControl.TypeSelectionLabelProperty);
    set => this.SetValue(CollectionControl.TypeSelectionLabelProperty, value);
  }

  public EditorDefinitionCollection EditorDefinitions
  {
    get => (EditorDefinitionCollection) this.GetValue(CollectionControl.EditorDefinitionsProperty);
    set => this.SetValue(CollectionControl.EditorDefinitionsProperty, (object) value);
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    if (this._newItemTypesComboBox != null)
      this._newItemTypesComboBox.Loaded -= new RoutedEventHandler(this.NewItemTypesComboBox_Loaded);
    this._newItemTypesComboBox = this.GetTemplateChild("PART_NewItemTypesComboBox") as ComboBox;
    if (this._newItemTypesComboBox != null)
      this._newItemTypesComboBox.Loaded += new RoutedEventHandler(this.NewItemTypesComboBox_Loaded);
    this._listBox = this.GetTemplateChild("PART_ListBox") as ListBox;
    if (this._propertyGrid != null)
      this._propertyGrid.PropertyValueChanged -= new PropertyValueChangedEventHandler(this.PropertyGrid_PropertyValueChanged);
    this._propertyGrid = this.GetTemplateChild("PART_PropertyGrid") as Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid;
    if (this._propertyGrid == null)
      return;
    this._propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(this.PropertyGrid_PropertyValueChanged);
  }

  public Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid PropertyGrid
  {
    get
    {
      if (this._propertyGrid == null)
        this.ApplyTemplate();
      return this._propertyGrid;
    }
  }

  static CollectionControl()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (CollectionControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (CollectionControl)));
  }

  public CollectionControl()
  {
    this.Items = new ObservableCollection<object>();
    this.CommandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.New, new ExecutedRoutedEventHandler(this.AddNew), new CanExecuteRoutedEventHandler(this.CanAddNew)));
    this.CommandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.Delete, new ExecutedRoutedEventHandler(this.Delete), new CanExecuteRoutedEventHandler(this.CanDelete)));
    this.CommandBindings.Add(new CommandBinding((ICommand) ApplicationCommands.Copy, new ExecutedRoutedEventHandler(this.Duplicate), new CanExecuteRoutedEventHandler(this.CanDuplicate)));
    this.CommandBindings.Add(new CommandBinding((ICommand) ComponentCommands.MoveDown, new ExecutedRoutedEventHandler(this.MoveDown), new CanExecuteRoutedEventHandler(this.CanMoveDown)));
    this.CommandBindings.Add(new CommandBinding((ICommand) ComponentCommands.MoveUp, new ExecutedRoutedEventHandler(this.MoveUp), new CanExecuteRoutedEventHandler(this.CanMoveUp)));
  }

  public event CollectionControl.ItemDeletingRoutedEventHandler ItemDeleting
  {
    add => this.AddHandler(CollectionControl.ItemDeletingEvent, (Delegate) value);
    remove => this.RemoveHandler(CollectionControl.ItemDeletingEvent, (Delegate) value);
  }

  public event CollectionControl.ItemDeletedRoutedEventHandler ItemDeleted
  {
    add => this.AddHandler(CollectionControl.ItemDeletedEvent, (Delegate) value);
    remove => this.RemoveHandler(CollectionControl.ItemDeletedEvent, (Delegate) value);
  }

  public event CollectionControl.ItemAddingRoutedEventHandler ItemAdding
  {
    add => this.AddHandler(CollectionControl.ItemAddingEvent, (Delegate) value);
    remove => this.RemoveHandler(CollectionControl.ItemAddingEvent, (Delegate) value);
  }

  public event CollectionControl.ItemAddedRoutedEventHandler ItemAdded
  {
    add => this.AddHandler(CollectionControl.ItemAddedEvent, (Delegate) value);
    remove => this.RemoveHandler(CollectionControl.ItemAddedEvent, (Delegate) value);
  }

  public event CollectionControl.ItemMovedDownRoutedEventHandler ItemMovedDown
  {
    add => this.AddHandler(CollectionControl.ItemMovedDownEvent, (Delegate) value);
    remove => this.RemoveHandler(CollectionControl.ItemMovedDownEvent, (Delegate) value);
  }

  public event CollectionControl.ItemMovedUpRoutedEventHandler ItemMovedUp
  {
    add => this.AddHandler(CollectionControl.ItemMovedUpEvent, (Delegate) value);
    remove => this.RemoveHandler(CollectionControl.ItemMovedUpEvent, (Delegate) value);
  }

  private void NewItemTypesComboBox_Loaded(object sender, RoutedEventArgs e)
  {
    if (this._newItemTypesComboBox == null)
      return;
    this._newItemTypesComboBox.SelectedIndex = 0;
  }

  private void PropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
  {
    if (this._listBox == null)
      return;
    this._listBox.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Delegate) (() => this._listBox.Items.Refresh()));
  }

  private void AddNew(object sender, ExecutedRoutedEventArgs e)
  {
    this.AddNewCore(this.CreateNewItem((Type) e.Parameter));
  }

  private void CanAddNew(object sender, CanExecuteRoutedEventArgs e)
  {
    this.CanAddNewCore(e.Parameter as Type, e);
  }

  private void CanAddNewCore(Type t, CanExecuteRoutedEventArgs e)
  {
    if (!(t != (Type) null) || this.IsReadOnly || (!t.IsValueType || t.IsEnum ? 0 : (!t.IsPrimitive ? 1 : 0)) == 0 && !(t.GetConstructor(Type.EmptyTypes) != (ConstructorInfo) null))
      return;
    e.CanExecute = true;
  }

  private void AddNewCore(object newItem)
  {
    ItemAddingEventArgs e = newItem != null ? new ItemAddingEventArgs(CollectionControl.ItemAddingEvent, newItem) : throw new ArgumentNullException(nameof (newItem));
    this.RaiseEvent((RoutedEventArgs) e);
    if (e.Cancel)
      return;
    newItem = e.Item;
    this.Items.Add(newItem);
    this.RaiseEvent((RoutedEventArgs) new ItemEventArgs(CollectionControl.ItemAddedEvent, newItem));
    this.SelectedItem = newItem;
  }

  private void Delete(object sender, ExecutedRoutedEventArgs e)
  {
    ItemDeletingEventArgs e1 = new ItemDeletingEventArgs(CollectionControl.ItemDeletingEvent, e.Parameter);
    this.RaiseEvent((RoutedEventArgs) e1);
    if (e1.Cancel)
      return;
    this.Items.Remove(e.Parameter);
    this.RaiseEvent((RoutedEventArgs) new ItemEventArgs(CollectionControl.ItemDeletedEvent, e.Parameter));
  }

  private void CanDelete(object sender, CanExecuteRoutedEventArgs e)
  {
    e.CanExecute = e.Parameter != null && !this.IsReadOnly;
  }

  private void Duplicate(object sender, ExecutedRoutedEventArgs e)
  {
    this.AddNewCore(this.DuplicateItem(e));
  }

  private void CanDuplicate(object sender, CanExecuteRoutedEventArgs e)
  {
    this.CanAddNewCore(e.Parameter != null ? e.Parameter.GetType() : (Type) null, e);
  }

  private object DuplicateItem(ExecutedRoutedEventArgs e)
  {
    object obj = e != null ? e.Parameter : throw new ArgumentNullException(nameof (e));
    Type type1 = obj.GetType();
    object newItem = this.CreateNewItem(type1);
    for (Type type2 = type1; type2 != (Type) null; type2 = type2.BaseType)
    {
      foreach (FieldInfo field in type2.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        field.SetValue(newItem, field.GetValue(obj));
    }
    return newItem;
  }

  private void MoveDown(object sender, ExecutedRoutedEventArgs e)
  {
    object parameter = e.Parameter;
    int index = this.Items.IndexOf(parameter);
    this.Items.RemoveAt(index);
    int num;
    this.Items.Insert(num = index + 1, parameter);
    this.RaiseEvent((RoutedEventArgs) new ItemEventArgs(CollectionControl.ItemMovedDownEvent, parameter));
    this.SelectedItem = parameter;
  }

  private void CanMoveDown(object sender, CanExecuteRoutedEventArgs e)
  {
    if (e.Parameter == null || this.Items.IndexOf(e.Parameter) >= this.Items.Count - 1 || this.IsReadOnly)
      return;
    e.CanExecute = true;
  }

  private void MoveUp(object sender, ExecutedRoutedEventArgs e)
  {
    object parameter = e.Parameter;
    int index = this.Items.IndexOf(parameter);
    this.Items.RemoveAt(index);
    int num;
    this.Items.Insert(num = index - 1, parameter);
    this.RaiseEvent((RoutedEventArgs) new ItemEventArgs(CollectionControl.ItemMovedUpEvent, parameter));
    this.SelectedItem = parameter;
  }

  private void CanMoveUp(object sender, CanExecuteRoutedEventArgs e)
  {
    if (e.Parameter == null || this.Items.IndexOf(e.Parameter) <= 0 || this.IsReadOnly)
      return;
    e.CanExecute = true;
  }

  public void PersistChanges() => this.PersistChanges((IList) this.Items);

  internal void PersistChanges(IList sourceList)
  {
    IEnumerable itemsSource = this.ComputeItemsSource();
    switch (itemsSource)
    {
      case null:
        break;
      case IDictionary _:
        IDictionary dictionary = (IDictionary) itemsSource;
        dictionary.Clear();
        IEnumerator enumerator1 = sourceList.GetEnumerator();
        try
        {
          while (enumerator1.MoveNext())
          {
            object current = enumerator1.Current;
            PropertyInfo property1 = current.GetType().GetProperty("Key");
            PropertyInfo property2 = current.GetType().GetProperty("Value");
            if (property1 != (PropertyInfo) null && property2 != (PropertyInfo) null)
              dictionary.Add(property1.GetValue(current, (object[]) null), property2.GetValue(current, (object[]) null));
          }
          break;
        }
        finally
        {
          if (enumerator1 is IDisposable disposable)
            disposable.Dispose();
        }
      case IList _:
        IList list = (IList) itemsSource;
        list.Clear();
        if (list.IsFixedSize)
        {
          if (sourceList.Count > list.Count)
            throw new IndexOutOfRangeException("Exceeding array size.");
          for (int index = 0; index < sourceList.Count; ++index)
            list[index] = sourceList[index];
          break;
        }
        IEnumerator enumerator2 = sourceList.GetEnumerator();
        try
        {
          while (enumerator2.MoveNext())
          {
            object current = enumerator2.Current;
            list.Add(current);
          }
          break;
        }
        finally
        {
          if (enumerator2 is IDisposable disposable)
            disposable.Dispose();
        }
      default:
        Type type1 = ((IEnumerable<Type>) itemsSource.GetType().GetInterfaces()).FirstOrDefault<Type>((Func<Type, bool>) (x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (ICollection<>)));
        if (!(type1 != (Type) null))
          break;
        Type type2 = ((IEnumerable<Type>) type1.GetGenericArguments()).FirstOrDefault<Type>();
        if (!(type2 != (Type) null))
          break;
        Type type3 = typeof (ICollection<>).MakeGenericType(type2);
        type3.GetMethod("Clear").Invoke((object) itemsSource, (object[]) null);
        IEnumerator enumerator3 = sourceList.GetEnumerator();
        try
        {
          while (enumerator3.MoveNext())
          {
            object current = enumerator3.Current;
            type3.GetMethod("Add").Invoke((object) itemsSource, new object[1]
            {
              current
            });
          }
          break;
        }
        finally
        {
          if (enumerator3 is IDisposable disposable)
            disposable.Dispose();
        }
    }
  }

  private IEnumerable CreateItemsSource()
  {
    IEnumerable itemsSource = (IEnumerable) null;
    if (this.ItemsSourceType != (Type) null)
    {
      ConstructorInfo constructor = this.ItemsSourceType.GetConstructor(Type.EmptyTypes);
      if (constructor != (ConstructorInfo) null)
        itemsSource = (IEnumerable) constructor.Invoke((object[]) null);
      else if (this.ItemsSourceType.IsArray)
        itemsSource = (IEnumerable) Array.CreateInstance(this.ItemsSourceType.GetElementType(), this.Items.Count);
    }
    return itemsSource;
  }

  private object CreateNewItem(Type type) => Activator.CreateInstance(type);

  private IEnumerable ComputeItemsSource()
  {
    if (this.ItemsSource == null)
      this.ItemsSource = this.CreateItemsSource();
    return this.ItemsSource;
  }

  public delegate void ItemDeletingRoutedEventHandler(object sender, ItemDeletingEventArgs e);

  public delegate void ItemDeletedRoutedEventHandler(object sender, ItemEventArgs e);

  public delegate void ItemAddingRoutedEventHandler(object sender, ItemAddingEventArgs e);

  public delegate void ItemAddedRoutedEventHandler(object sender, ItemEventArgs e);

  public delegate void ItemMovedDownRoutedEventHandler(object sender, ItemEventArgs e);

  public delegate void ItemMovedUpRoutedEventHandler(object sender, ItemEventArgs e);
}
