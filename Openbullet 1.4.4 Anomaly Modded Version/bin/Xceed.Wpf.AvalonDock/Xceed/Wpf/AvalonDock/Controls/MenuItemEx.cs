// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.MenuItemEx
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class MenuItemEx : MenuItem
{
  private bool _reentrantFlag;
  public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register(nameof (IconTemplate), typeof (DataTemplate), typeof (MenuItemEx), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(MenuItemEx.OnIconTemplateChanged)));
  public static readonly DependencyProperty IconTemplateSelectorProperty = DependencyProperty.Register(nameof (IconTemplateSelector), typeof (DataTemplateSelector), typeof (MenuItemEx), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(MenuItemEx.OnIconTemplateSelectorChanged)));

  static MenuItemEx()
  {
    MenuItem.IconProperty.OverrideMetadata(typeof (MenuItemEx), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(MenuItemEx.OnIconPropertyChanged)));
  }

  public DataTemplate IconTemplate
  {
    get => (DataTemplate) this.GetValue(MenuItemEx.IconTemplateProperty);
    set => this.SetValue(MenuItemEx.IconTemplateProperty, (object) value);
  }

  private static void OnIconTemplateChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((MenuItemEx) d).OnIconTemplateChanged(e);
  }

  protected virtual void OnIconTemplateChanged(DependencyPropertyChangedEventArgs e)
  {
    this.UpdateIcon();
  }

  public DataTemplateSelector IconTemplateSelector
  {
    get => (DataTemplateSelector) this.GetValue(MenuItemEx.IconTemplateSelectorProperty);
    set => this.SetValue(MenuItemEx.IconTemplateSelectorProperty, (object) value);
  }

  private static void OnIconTemplateSelectorChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((MenuItemEx) d).OnIconTemplateSelectorChanged(e);
  }

  protected virtual void OnIconTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
  {
    this.UpdateIcon();
  }

  private static void OnIconPropertyChanged(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs e)
  {
    if (e.NewValue == null)
      return;
    ((MenuItemEx) sender).UpdateIcon();
  }

  private void UpdateIcon()
  {
    if (this._reentrantFlag)
      return;
    this._reentrantFlag = true;
    if (this.IconTemplateSelector != null)
    {
      DataTemplate dataTemplate = this.IconTemplateSelector.SelectTemplate(this.Icon, (DependencyObject) this);
      if (dataTemplate != null)
        this.Icon = (object) dataTemplate.LoadContent();
    }
    else if (this.IconTemplate != null)
      this.Icon = (object) this.IconTemplate.LoadContent();
    this._reentrantFlag = false;
  }
}
