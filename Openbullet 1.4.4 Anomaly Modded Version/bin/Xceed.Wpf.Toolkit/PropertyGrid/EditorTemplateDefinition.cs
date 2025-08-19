// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.EditorTemplateDefinition
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

public class EditorTemplateDefinition : EditorDefinitionBase
{
  public static readonly DependencyProperty EditingTemplateProperty = DependencyProperty.Register(nameof (EditingTemplate), typeof (DataTemplate), typeof (EditorTemplateDefinition), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));

  public DataTemplate EditingTemplate
  {
    get => (DataTemplate) this.GetValue(EditorTemplateDefinition.EditingTemplateProperty);
    set => this.SetValue(EditorTemplateDefinition.EditingTemplateProperty, (object) value);
  }

  protected sealed override FrameworkElement GenerateEditingElement(PropertyItemBase propertyItem)
  {
    return this.EditingTemplate == null ? (FrameworkElement) null : this.EditingTemplate.LoadContent() as FrameworkElement;
  }
}
