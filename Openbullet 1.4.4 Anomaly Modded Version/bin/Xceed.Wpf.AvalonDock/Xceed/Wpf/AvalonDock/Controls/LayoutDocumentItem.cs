// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutDocumentItem
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class LayoutDocumentItem : LayoutItem
{
  private LayoutDocument _document;
  public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof (Description), typeof (string), typeof (LayoutDocumentItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutDocumentItem.OnDescriptionChanged)));

  internal LayoutDocumentItem()
  {
  }

  public string Description
  {
    get => (string) this.GetValue(LayoutDocumentItem.DescriptionProperty);
    set => this.SetValue(LayoutDocumentItem.DescriptionProperty, (object) value);
  }

  private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((LayoutDocumentItem) d).OnDescriptionChanged(e);
  }

  protected virtual void OnDescriptionChanged(DependencyPropertyChangedEventArgs e)
  {
    this._document.Description = (string) e.NewValue;
  }

  protected override void Close()
  {
    if (this._document.Root == null || this._document.Root.Manager == null)
      return;
    this._document.Root.Manager._ExecuteCloseCommand(this._document);
  }

  protected override void OnVisibilityChanged()
  {
    if (this._document != null && this._document.Root != null)
    {
      this._document.IsVisible = this.Visibility == Visibility.Visible;
      if (this._document.Parent is LayoutDocumentPane)
        ((LayoutGroup<LayoutContent>) this._document.Parent).ComputeVisibility();
    }
    base.OnVisibilityChanged();
  }

  internal override void Attach(LayoutContent model)
  {
    this._document = model as LayoutDocument;
    base.Attach(model);
  }

  internal override void Detach()
  {
    this._document = (LayoutDocument) null;
    base.Detach();
  }
}
