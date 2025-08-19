// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.CollectionControlDialog
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.PropertyGrid;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class CollectionControlDialog : CollectionControlDialogBase, IComponentConnector
{
  private IList originalData = (IList) new List<object>();
  public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof (ItemsSource), typeof (IEnumerable), typeof (CollectionControlDialog), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty ItemsSourceTypeProperty = DependencyProperty.Register(nameof (ItemsSourceType), typeof (Type), typeof (CollectionControlDialog), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty NewItemTypesProperty = DependencyProperty.Register(nameof (NewItemTypes), typeof (IList), typeof (CollectionControlDialog), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof (IsReadOnly), typeof (bool), typeof (CollectionControlDialog), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty EditorDefinitionsProperty = DependencyProperty.Register(nameof (EditorDefinitions), typeof (EditorDefinitionCollection), typeof (CollectionControlDialog), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  internal CollectionControl _collectionControl;
  private bool _contentLoaded;

  public IEnumerable ItemsSource
  {
    get => (IEnumerable) this.GetValue(CollectionControlDialog.ItemsSourceProperty);
    set => this.SetValue(CollectionControlDialog.ItemsSourceProperty, (object) value);
  }

  public Type ItemsSourceType
  {
    get => (Type) this.GetValue(CollectionControlDialog.ItemsSourceTypeProperty);
    set => this.SetValue(CollectionControlDialog.ItemsSourceTypeProperty, (object) value);
  }

  public IList<Type> NewItemTypes
  {
    get => (IList<Type>) this.GetValue(CollectionControlDialog.NewItemTypesProperty);
    set => this.SetValue(CollectionControlDialog.NewItemTypesProperty, (object) value);
  }

  public bool IsReadOnly
  {
    get => (bool) this.GetValue(CollectionControlDialog.IsReadOnlyProperty);
    set => this.SetValue(CollectionControlDialog.IsReadOnlyProperty, (object) value);
  }

  public EditorDefinitionCollection EditorDefinitions
  {
    get
    {
      return (EditorDefinitionCollection) this.GetValue(CollectionControlDialog.EditorDefinitionsProperty);
    }
    set => this.SetValue(CollectionControlDialog.EditorDefinitionsProperty, (object) value);
  }

  public CollectionControl CollectionControl => this._collectionControl;

  public CollectionControlDialog() => this.InitializeComponent();

  public CollectionControlDialog(Type itemsourceType)
    : this()
  {
    this.ItemsSourceType = itemsourceType;
  }

  public CollectionControlDialog(Type itemsourceType, IList<Type> newItemTypes)
    : this(itemsourceType)
  {
    this.NewItemTypes = newItemTypes;
  }

  protected override void OnSourceInitialized(EventArgs e)
  {
    base.OnSourceInitialized(e);
    if (this.ItemsSource == null)
      return;
    foreach (object source in this.ItemsSource)
      this.originalData.Add(this.Clone(source));
  }

  private void OkButton_Click(object sender, RoutedEventArgs e)
  {
    if (this.ItemsSource is IDictionary && !this.AreDictionaryKeysValid())
    {
      int num = (int) MessageBox.Show("All dictionary items should have distinct non-null Key values.", "Warning");
    }
    else
    {
      this._collectionControl.PersistChanges();
      this.DialogResult = new bool?(true);
      this.Close();
    }
  }

  private void CancelButton_Click(object sender, RoutedEventArgs e)
  {
    this._collectionControl.PersistChanges(this.originalData);
    this.DialogResult = new bool?(false);
    this.Close();
  }

  [SecuritySafeCritical]
  private object Clone(object source)
  {
    if (source == null)
      return (object) null;
    object obj1 = (object) null;
    Type type1 = source.GetType();
    if (source is Array)
    {
      using (MemoryStream serializationStream = new MemoryStream())
      {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize((Stream) serializationStream, source);
        serializationStream.Seek(0L, SeekOrigin.Begin);
        obj1 = (object) (Array) binaryFormatter.Deserialize((Stream) serializationStream);
      }
    }
    else if (this.ItemsSource is IDictionary && type1.IsGenericType && typeof (KeyValuePair<,>).IsAssignableFrom(type1.GetGenericTypeDefinition()))
    {
      obj1 = this.GenerateEditableKeyValuePair(source);
    }
    else
    {
      try
      {
        obj1 = FormatterServices.GetUninitializedObject(type1);
      }
      catch (Exception ex)
      {
      }
      ConstructorInfo constructor = type1.GetConstructor(Type.EmptyTypes);
      if (constructor != (ConstructorInfo) null)
        constructor.Invoke(obj1, (object[]) null);
      else
        obj1 = source;
    }
    if (obj1 != null)
    {
      foreach (PropertyInfo property in type1.GetProperties())
      {
        ParameterInfo[] indexParameters = property.GetIndexParameters();
        object[] objArray;
        if (indexParameters.GetLength(0) != 0)
          objArray = new object[1]
          {
            (object) (indexParameters.GetLength(0) - 1)
          };
        else
          objArray = (object[]) null;
        object[] index = objArray;
        object source1 = property.GetValue(source, index);
        if (property.CanWrite)
        {
          if (property.PropertyType.IsClass && property.PropertyType != typeof (Transform) && !property.PropertyType.Equals(typeof (string)))
          {
            if (property.PropertyType.IsGenericType)
            {
              Type type2 = ((IEnumerable<Type>) property.PropertyType.GetGenericArguments()).FirstOrDefault<Type>();
              if (type2 != (Type) null && !type2.IsPrimitive && !type2.Equals(typeof (string)) && !type2.IsEnum)
              {
                object obj2 = this.Clone(source1);
                property.SetValue(obj1, obj2, (object[]) null);
              }
              else
                property.SetValue(obj1, source1, (object[]) null);
            }
            else
            {
              object obj3 = this.Clone(source1);
              if (obj3 != null)
              {
                if (index != null)
                  obj1.GetType().GetMethod("Add").Invoke(obj1, new object[1]
                  {
                    obj3
                  });
                else
                  property.SetValue(obj1, obj3, (object[]) null);
              }
            }
          }
          else if (index != null)
            obj1.GetType().GetMethod("Add").Invoke(obj1, new object[1]
            {
              source1
            });
          else
            property.SetValue(obj1, source1, (object[]) null);
        }
      }
    }
    return obj1;
  }

  private object GenerateEditableKeyValuePair(object source)
  {
    Type type = source.GetType();
    if (type.GetGenericArguments() == null || type.GetGenericArguments().GetLength(0) != 2)
      return (object) null;
    PropertyInfo property1 = type.GetProperty("Key");
    PropertyInfo property2 = type.GetProperty("Value");
    return property1 != (PropertyInfo) null && property2 != (PropertyInfo) null ? ListUtilities.CreateEditableKeyValuePair(property1.GetValue(source, (object[]) null), type.GetGenericArguments()[0], property2.GetValue(source, (object[]) null), type.GetGenericArguments()[1]) : (object) null;
  }

  private bool AreDictionaryKeysValid()
  {
    IEnumerable<object> source = this._collectionControl.Items.Select<object, object>((Func<object, object>) (x =>
    {
      PropertyInfo property = x.GetType().GetProperty("Key");
      return property != (PropertyInfo) null ? property.GetValue(x, (object[]) null) : (object) null;
    }));
    return source.Distinct<object>().Count<object>() == this._collectionControl.Items.Count && source.All<object>((Func<object, bool>) (x => x != null));
  }

  [DebuggerNonUserCode]
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public void InitializeComponent()
  {
    if (this._contentLoaded)
      return;
    this._contentLoaded = true;
    Application.LoadComponent((object) this, new Uri("/Xceed.Wpf.Toolkit;component/collectioncontrol/implementation/collectioncontroldialog.xaml", UriKind.Relative));
  }

  [DebuggerNonUserCode]
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  internal Delegate _CreateDelegate(Type delegateType, string handler)
  {
    return Delegate.CreateDelegate(delegateType, (object) this, handler);
  }

  [DebuggerNonUserCode]
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  void IComponentConnector.Connect(int connectionId, object target)
  {
    switch (connectionId)
    {
      case 1:
        this._collectionControl = (CollectionControl) target;
        break;
      case 2:
        ((ButtonBase) target).Click += new RoutedEventHandler(this.OkButton_Click);
        break;
      case 3:
        ((ButtonBase) target).Click += new RoutedEventHandler(this.CancelButton_Click);
        break;
      default:
        this._contentLoaded = true;
        break;
    }
  }
}
