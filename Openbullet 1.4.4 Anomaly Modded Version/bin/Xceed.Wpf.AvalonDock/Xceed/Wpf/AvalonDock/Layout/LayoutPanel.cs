// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.LayoutPanel
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

[ContentProperty("Children")]
[Serializable]
public class LayoutPanel : 
  LayoutPositionableGroup<ILayoutPanelElement>,
  ILayoutPanelElement,
  ILayoutElement,
  INotifyPropertyChanged,
  INotifyPropertyChanging,
  ILayoutOrientableGroup,
  ILayoutGroup,
  ILayoutContainer
{
  private Orientation _orientation;

  public LayoutPanel()
  {
  }

  public LayoutPanel(ILayoutPanelElement firstChild) => this.Children.Add(firstChild);

  public Orientation Orientation
  {
    get => this._orientation;
    set
    {
      if (this._orientation == value)
        return;
      this.RaisePropertyChanging(nameof (Orientation));
      this._orientation = value;
      this.RaisePropertyChanged(nameof (Orientation));
    }
  }

  protected override bool GetVisibility()
  {
    return this.Children.Any<ILayoutPanelElement>((Func<ILayoutPanelElement, bool>) (c => c.IsVisible));
  }

  public override void WriteXml(XmlWriter writer)
  {
    writer.WriteAttributeString("Orientation", this.Orientation.ToString());
    base.WriteXml(writer);
  }

  public override void ReadXml(XmlReader reader)
  {
    if (reader.MoveToAttribute("Orientation"))
      this.Orientation = (Orientation) Enum.Parse(typeof (Orientation), reader.Value, true);
    base.ReadXml(reader);
  }

  public override void ConsoleDump(int tab)
  {
    Trace.Write(new string(' ', tab * 4));
    Trace.WriteLine($"Panel({this.Orientation})");
    foreach (LayoutElement child in (Collection<ILayoutPanelElement>) this.Children)
      child.ConsoleDump(tab + 1);
  }
}
