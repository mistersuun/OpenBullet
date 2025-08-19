// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.CollectionControlButton
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class CollectionControlButton : Button
{
  public static readonly DependencyProperty EditorDefinitionsProperty = DependencyProperty.Register(nameof (EditorDefinitions), typeof (EditorDefinitionCollection), typeof (CollectionControlButton), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof (IsReadOnly), typeof (bool), typeof (CollectionControlButton), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof (ItemsSource), typeof (IEnumerable), typeof (CollectionControlButton), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty ItemsSourceTypeProperty = DependencyProperty.Register(nameof (ItemsSourceType), typeof (Type), typeof (CollectionControlButton), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty NewItemTypesProperty = DependencyProperty.Register(nameof (NewItemTypes), typeof (IList), typeof (CollectionControlButton), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));

  static CollectionControlButton()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (CollectionControlButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (CollectionControlButton)));
  }

  public CollectionControlButton()
  {
    this.Click += new RoutedEventHandler(this.CollectionControlButton_Click);
  }

  public EditorDefinitionCollection EditorDefinitions
  {
    get
    {
      return (EditorDefinitionCollection) this.GetValue(CollectionControlButton.EditorDefinitionsProperty);
    }
    set => this.SetValue(CollectionControlButton.EditorDefinitionsProperty, (object) value);
  }

  public bool IsReadOnly
  {
    get => (bool) this.GetValue(CollectionControlButton.IsReadOnlyProperty);
    set => this.SetValue(CollectionControlButton.IsReadOnlyProperty, (object) value);
  }

  public IEnumerable ItemsSource
  {
    get => (IEnumerable) this.GetValue(CollectionControlButton.ItemsSourceProperty);
    set => this.SetValue(CollectionControlButton.ItemsSourceProperty, (object) value);
  }

  public Type ItemsSourceType
  {
    get => (Type) this.GetValue(CollectionControlButton.ItemsSourceTypeProperty);
    set => this.SetValue(CollectionControlButton.ItemsSourceTypeProperty, (object) value);
  }

  public IList<Type> NewItemTypes
  {
    get => (IList<Type>) this.GetValue(CollectionControlButton.NewItemTypesProperty);
    set => this.SetValue(CollectionControlButton.NewItemTypesProperty, (object) value);
  }

  private void CollectionControlButton_Click(object sender, RoutedEventArgs e)
  {
    CollectionControlDialog target = new CollectionControlDialog();
    Binding binding = new Binding("ItemsSource")
    {
      Source = (object) this,
      Mode = BindingMode.TwoWay
    };
    BindingOperations.SetBinding((DependencyObject) target, CollectionControlDialog.ItemsSourceProperty, (BindingBase) binding);
    target.NewItemTypes = this.NewItemTypes;
    target.ItemsSourceType = this.ItemsSourceType;
    target.IsReadOnly = this.IsReadOnly;
    target.EditorDefinitions = this.EditorDefinitions;
    target.ShowDialog();
  }
}
