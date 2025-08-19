// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.EditorDefinition
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

[Obsolete("Use EditorTemplateDefinition instead of EditorDefinition.  (XAML Ex: <t:EditorTemplateDefinition TargetProperties=\"FirstName,LastName\" .../> OR <t:EditorTemplateDefinition TargetProperties=\"{x:Type l:MyType}\" .../> )")]
public class EditorDefinition : EditorTemplateDefinition
{
  private const string UsageEx = " (XAML Ex: <t:EditorTemplateDefinition TargetProperties=\"FirstName,LastName\" .../> OR <t:EditorTemplateDefinition TargetProperties=\"{x:Type l:MyType}\" .../> )";
  private PropertyDefinitionCollection _properties = new PropertyDefinitionCollection();

  public EditorDefinition()
  {
    Trace.TraceWarning($"{typeof (EditorDefinition)} is obsolete. Instead use {typeof (EditorTemplateDefinition)}." + " (XAML Ex: <t:EditorTemplateDefinition TargetProperties=\"FirstName,LastName\" .../> OR <t:EditorTemplateDefinition TargetProperties=\"{x:Type l:MyType}\" .../> )");
  }

  public DataTemplate EditorTemplate { get; set; }

  public PropertyDefinitionCollection PropertiesDefinitions
  {
    get => this._properties;
    set => this._properties = value;
  }

  public Type TargetType { get; set; }

  internal override void Lock()
  {
    if (this.EditingTemplate != null)
      throw new InvalidOperationException($"Use a EditorTemplateDefinition instead of EditorDefinition in order to use the '{"EditingTemplate"}' property.");
    if (this.TargetProperties != null && this.TargetProperties.Count > 0)
      throw new InvalidOperationException($"Use a EditorTemplateDefinition instead of EditorDefinition in order to use the '{"TargetProperties"}' property.");
    List<object> objectList = new List<object>();
    if (this.PropertiesDefinitions != null)
    {
      foreach (PropertyDefinition propertiesDefinition in (Collection<PropertyDefinition>) this.PropertiesDefinitions)
      {
        if (propertiesDefinition.TargetProperties != null)
          objectList.AddRange(propertiesDefinition.TargetProperties.Cast<object>());
      }
    }
    this.TargetProperties = (IList) objectList;
    this.EditingTemplate = this.EditorTemplate;
    base.Lock();
  }
}
