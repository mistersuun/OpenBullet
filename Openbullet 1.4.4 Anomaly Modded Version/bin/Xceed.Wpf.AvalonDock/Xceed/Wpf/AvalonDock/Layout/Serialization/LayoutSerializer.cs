// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.Serialization.LayoutSerializer
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout.Serialization;

public abstract class LayoutSerializer
{
  private DockingManager _manager;
  private LayoutAnchorable[] _previousAnchorables;
  private LayoutDocument[] _previousDocuments;

  public LayoutSerializer(DockingManager manager)
  {
    this._manager = manager != null ? manager : throw new ArgumentNullException(nameof (manager));
    this._previousAnchorables = this._manager.Layout.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>();
    this._previousDocuments = this._manager.Layout.Descendents().OfType<LayoutDocument>().ToArray<LayoutDocument>();
  }

  public DockingManager Manager => this._manager;

  public event EventHandler<LayoutSerializationCallbackEventArgs> LayoutSerializationCallback;

  protected virtual void FixupLayout(LayoutRoot layout)
  {
    foreach (ILayoutPreviousContainer previousContainer in layout.Descendents().OfType<ILayoutPreviousContainer>().Where<ILayoutPreviousContainer>((Func<ILayoutPreviousContainer, bool>) (lc => lc.PreviousContainerId != null)))
    {
      ILayoutPreviousContainer lcToAttach = previousContainer;
      lcToAttach.PreviousContainer = (layout.Descendents().OfType<ILayoutPaneSerializable>().FirstOrDefault<ILayoutPaneSerializable>((Func<ILayoutPaneSerializable, bool>) (lps => lps.Id == lcToAttach.PreviousContainerId)) ?? throw new ArgumentException($"Unable to find a pane with id ='{lcToAttach.PreviousContainerId}'")) as ILayoutContainer;
    }
    foreach (LayoutAnchorable layoutAnchorable1 in layout.Descendents().OfType<LayoutAnchorable>().Where<LayoutAnchorable>((Func<LayoutAnchorable, bool>) (lc => lc.Content == null)).ToArray<LayoutAnchorable>())
    {
      LayoutAnchorable lcToFix = layoutAnchorable1;
      LayoutAnchorable layoutAnchorable2 = (LayoutAnchorable) null;
      if (lcToFix.ContentId != null)
        layoutAnchorable2 = ((IEnumerable<LayoutAnchorable>) this._previousAnchorables).FirstOrDefault<LayoutAnchorable>((Func<LayoutAnchorable, bool>) (a => a.ContentId == lcToFix.ContentId));
      if (this.LayoutSerializationCallback != null)
      {
        LayoutSerializationCallbackEventArgs e = new LayoutSerializationCallbackEventArgs((LayoutContent) lcToFix, layoutAnchorable2?.Content);
        this.LayoutSerializationCallback((object) this, e);
        if (e.Cancel)
          lcToFix.Close();
        else if (e.Content != null)
          lcToFix.Content = e.Content;
        else if (e.Model.Content != null)
          lcToFix.Hide(false);
      }
      else if (layoutAnchorable2 == null)
      {
        lcToFix.Hide(false);
      }
      else
      {
        lcToFix.Content = layoutAnchorable2.Content;
        lcToFix.IconSource = layoutAnchorable2.IconSource;
      }
    }
    foreach (LayoutDocument layoutDocument1 in layout.Descendents().OfType<LayoutDocument>().Where<LayoutDocument>((Func<LayoutDocument, bool>) (lc => lc.Content == null)).ToArray<LayoutDocument>())
    {
      LayoutDocument lcToFix = layoutDocument1;
      LayoutDocument layoutDocument2 = (LayoutDocument) null;
      if (lcToFix.ContentId != null)
        layoutDocument2 = ((IEnumerable<LayoutDocument>) this._previousDocuments).FirstOrDefault<LayoutDocument>((Func<LayoutDocument, bool>) (a => a.ContentId == lcToFix.ContentId));
      if (this.LayoutSerializationCallback != null)
      {
        LayoutSerializationCallbackEventArgs e = new LayoutSerializationCallbackEventArgs((LayoutContent) lcToFix, layoutDocument2?.Content);
        this.LayoutSerializationCallback((object) this, e);
        if (e.Cancel)
          lcToFix.Close();
        else if (e.Content != null)
          lcToFix.Content = e.Content;
        else if (e.Model.Content != null)
          lcToFix.Close();
      }
      else if (layoutDocument2 == null)
      {
        lcToFix.Close();
      }
      else
      {
        lcToFix.Content = layoutDocument2.Content;
        lcToFix.IconSource = layoutDocument2.IconSource;
      }
    }
    layout.CollectGarbage();
  }

  protected void StartDeserialization()
  {
    this.Manager.SuspendDocumentsSourceBinding = true;
    this.Manager.SuspendAnchorablesSourceBinding = true;
  }

  protected void EndDeserialization()
  {
    this.Manager.SuspendDocumentsSourceBinding = false;
    this.Manager.SuspendAnchorablesSourceBinding = false;
  }
}
