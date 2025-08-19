// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.LayoutDocument
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Diagnostics;
using System.Linq;
using System.Xml;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

[Serializable]
public class LayoutDocument : LayoutContent
{
  internal bool _canMove = true;
  private bool _isVisible = true;
  private string _description;

  public bool CanMove
  {
    get => this._canMove;
    set
    {
      if (this._canMove == value)
        return;
      this._canMove = value;
      this.RaisePropertyChanged(nameof (CanMove));
    }
  }

  public bool IsVisible
  {
    get => this._isVisible;
    internal set => this._isVisible = value;
  }

  public string Description
  {
    get => this._description;
    set
    {
      if (!(this._description != value))
        return;
      this._description = value;
      this.RaisePropertyChanged(nameof (Description));
    }
  }

  public override void WriteXml(XmlWriter writer)
  {
    base.WriteXml(writer);
    if (!string.IsNullOrWhiteSpace(this.Description))
      writer.WriteAttributeString("Description", this.Description);
    if (this.CanMove)
      return;
    writer.WriteAttributeString("CanMove", this.CanMove.ToString());
  }

  public override void ReadXml(XmlReader reader)
  {
    if (reader.MoveToAttribute("Description"))
      this.Description = reader.Value;
    if (reader.MoveToAttribute("CanMove"))
      this.CanMove = bool.Parse(reader.Value);
    base.ReadXml(reader);
  }

  public override void Close()
  {
    if (this.Root != null && this.Root.Manager != null)
      this.Root.Manager._ExecuteCloseCommand(this);
    else
      this.CloseDocument();
  }

  public override void ConsoleDump(int tab)
  {
    Trace.Write(new string(' ', tab * 4));
    Trace.WriteLine("Document()");
  }

  protected override void InternalDock()
  {
    LayoutRoot root = this.Root as LayoutRoot;
    LayoutDocumentPane destinationContainer = (LayoutDocumentPane) null;
    if (root.LastFocusedDocument != null && root.LastFocusedDocument != this)
      destinationContainer = root.LastFocusedDocument.Parent as LayoutDocumentPane;
    if (destinationContainer == null)
      destinationContainer = root.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault<LayoutDocumentPane>();
    bool flag = false;
    if (root.Manager.LayoutUpdateStrategy != null)
      flag = root.Manager.LayoutUpdateStrategy.BeforeInsertDocument(root, this, (ILayoutContainer) destinationContainer);
    if (!flag)
    {
      if (destinationContainer == null)
        throw new InvalidOperationException("Layout must contains at least one LayoutDocumentPane in order to host documents");
      destinationContainer.Children.Add((LayoutContent) this);
    }
    if (root.Manager.LayoutUpdateStrategy != null)
      root.Manager.LayoutUpdateStrategy.AfterInsertDocument(root, this);
    base.InternalDock();
  }

  internal bool CloseDocument()
  {
    if (!this.TestCanClose())
      return false;
    this.CloseInternal();
    return true;
  }
}
