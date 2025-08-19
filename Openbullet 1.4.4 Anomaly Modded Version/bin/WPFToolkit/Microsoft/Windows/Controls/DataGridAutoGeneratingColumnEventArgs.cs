// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridAutoGeneratingColumnEventArgs
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.ComponentModel;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGridAutoGeneratingColumnEventArgs : EventArgs
{
  private DataGridColumn _column;
  private string _propertyName;
  private Type _propertyType;
  private object _propertyDescriptor;
  private bool _cancel;

  public DataGridAutoGeneratingColumnEventArgs(
    string propertyName,
    Type propertyType,
    DataGridColumn column)
    : this(column, propertyName, propertyType, (object) null)
  {
  }

  internal DataGridAutoGeneratingColumnEventArgs(
    DataGridColumn column,
    ItemPropertyInfo itemPropertyInfo)
    : this(column, itemPropertyInfo.Name, itemPropertyInfo.PropertyType, itemPropertyInfo.Descriptor)
  {
  }

  internal DataGridAutoGeneratingColumnEventArgs(
    DataGridColumn column,
    string propertyName,
    Type propertyType,
    object propertyDescriptor)
  {
    this._column = column;
    this._propertyName = propertyName;
    this._propertyType = propertyType;
    this.PropertyDescriptor = propertyDescriptor;
  }

  public DataGridColumn Column
  {
    get => this._column;
    set => this._column = value;
  }

  public string PropertyName => this._propertyName;

  public Type PropertyType => this._propertyType;

  public object PropertyDescriptor
  {
    get => this._propertyDescriptor;
    private set
    {
      if (value == null)
        this._propertyDescriptor = (object) null;
      else
        this._propertyDescriptor = value;
    }
  }

  public bool Cancel
  {
    get => this._cancel;
    set => this._cancel = value;
  }
}
