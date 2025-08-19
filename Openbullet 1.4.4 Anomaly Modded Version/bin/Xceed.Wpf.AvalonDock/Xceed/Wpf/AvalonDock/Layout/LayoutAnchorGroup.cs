// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.LayoutAnchorGroup
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Serialization;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

[ContentProperty("Children")]
[Serializable]
public class LayoutAnchorGroup : 
  LayoutGroup<LayoutAnchorable>,
  ILayoutPreviousContainer,
  ILayoutPaneSerializable
{
  [NonSerialized]
  private ILayoutContainer _previousContainer;
  private string _id;

  protected override bool GetVisibility() => this.Children.Count > 0;

  public override void WriteXml(XmlWriter writer)
  {
    if (this._id != null)
      writer.WriteAttributeString("Id", this._id);
    if (this._previousContainer != null && this._previousContainer is ILayoutPaneSerializable previousContainer)
      writer.WriteAttributeString("PreviousContainerId", previousContainer.Id);
    base.WriteXml(writer);
  }

  public override void ReadXml(XmlReader reader)
  {
    if (reader.MoveToAttribute("Id"))
      this._id = reader.Value;
    if (reader.MoveToAttribute("PreviousContainerId"))
      ((ILayoutPreviousContainer) this).PreviousContainerId = reader.Value;
    base.ReadXml(reader);
  }

  [XmlIgnore]
  ILayoutContainer ILayoutPreviousContainer.PreviousContainer
  {
    get => this._previousContainer;
    set
    {
      if (this._previousContainer == value)
        return;
      this._previousContainer = value;
      this.RaisePropertyChanged("PreviousContainer");
      if (!(this._previousContainer is ILayoutPaneSerializable previousContainer) || previousContainer.Id != null)
        return;
      previousContainer.Id = Guid.NewGuid().ToString();
    }
  }

  string ILayoutPreviousContainer.PreviousContainerId { get; set; }

  string ILayoutPaneSerializable.Id
  {
    get => this._id;
    set => this._id = value;
  }
}
