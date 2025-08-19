// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.NavigatorWindow
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Themes;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

[TemplatePart(Name = "PART_AnchorableListBox", Type = typeof (ListBox))]
[TemplatePart(Name = "PART_DocumentListBox", Type = typeof (ListBox))]
public class NavigatorWindow : Window
{
  private const string PART_AnchorableListBox = "PART_AnchorableListBox";
  private const string PART_DocumentListBox = "PART_DocumentListBox";
  private ResourceDictionary currentThemeResourceDictionary;
  private DockingManager _manager;
  private bool _isSelectingDocument;
  private ListBox _anchorableListBox;
  private ListBox _documentListBox;
  private bool _internalSetSelectedDocument;
  private bool _internalSetSelectedAnchorable;
  private static readonly DependencyPropertyKey DocumentsPropertyKey = DependencyProperty.RegisterReadOnly(nameof (Documents), typeof (IEnumerable<LayoutDocumentItem>), typeof (NavigatorWindow), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty DocumentsProperty = NavigatorWindow.DocumentsPropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey AnchorablesPropertyKey = DependencyProperty.RegisterReadOnly(nameof (Anchorables), typeof (IEnumerable<LayoutAnchorableItem>), typeof (NavigatorWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
  public static readonly DependencyProperty AnchorablesProperty = NavigatorWindow.AnchorablesPropertyKey.DependencyProperty;
  public static readonly DependencyProperty SelectedDocumentProperty = DependencyProperty.Register(nameof (SelectedDocument), typeof (LayoutDocumentItem), typeof (NavigatorWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(NavigatorWindow.OnSelectedDocumentChanged)));
  public static readonly DependencyProperty SelectedAnchorableProperty = DependencyProperty.Register(nameof (SelectedAnchorable), typeof (LayoutAnchorableItem), typeof (NavigatorWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(NavigatorWindow.OnSelectedAnchorableChanged)));

  static NavigatorWindow()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (NavigatorWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (NavigatorWindow)));
    Window.ShowActivatedProperty.OverrideMetadata(typeof (NavigatorWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    Window.ShowInTaskbarProperty.OverrideMetadata(typeof (NavigatorWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  }

  internal NavigatorWindow(DockingManager manager)
  {
    this._manager = manager;
    this._internalSetSelectedDocument = true;
    this.SetAnchorables((IEnumerable<LayoutAnchorableItem>) this._manager.Layout.Descendents().OfType<LayoutAnchorable>().Where<LayoutAnchorable>((Func<LayoutAnchorable, bool>) (a => a.IsVisible)).Select<LayoutAnchorable, LayoutAnchorableItem>((Func<LayoutAnchorable, LayoutAnchorableItem>) (d => (LayoutAnchorableItem) this._manager.GetLayoutItemFromModel((LayoutContent) d))).ToArray<LayoutAnchorableItem>());
    this.SetDocuments(this._manager.Layout.Descendents().OfType<LayoutDocument>().OrderByDescending<LayoutDocument, DateTime>((Func<LayoutDocument, DateTime>) (d => d.LastActivationTimeStamp.GetValueOrDefault())).Select<LayoutDocument, LayoutDocumentItem>((Func<LayoutDocument, LayoutDocumentItem>) (d => (LayoutDocumentItem) this._manager.GetLayoutItemFromModel((LayoutContent) d))).ToArray<LayoutDocumentItem>());
    this._internalSetSelectedDocument = false;
    if (this.Documents.Length > 1)
    {
      this.InternalSetSelectedDocument(this.Documents[1]);
      this._isSelectingDocument = true;
    }
    else if (this.Anchorables.Count<LayoutAnchorableItem>() > 1)
    {
      this.InternalSetSelectedAnchorable(this.Anchorables.ToArray<LayoutAnchorableItem>()[1]);
      this._isSelectingDocument = false;
    }
    this.DataContext = (object) this;
    this.Loaded += new RoutedEventHandler(this.OnLoaded);
    this.Unloaded += new RoutedEventHandler(this.OnUnloaded);
    this.UpdateThemeResources();
  }

  public LayoutDocumentItem[] Documents
  {
    get => (LayoutDocumentItem[]) this.GetValue(NavigatorWindow.DocumentsProperty);
  }

  public IEnumerable<LayoutAnchorableItem> Anchorables
  {
    get => (IEnumerable<LayoutAnchorableItem>) this.GetValue(NavigatorWindow.AnchorablesProperty);
  }

  public LayoutDocumentItem SelectedDocument
  {
    get => (LayoutDocumentItem) this.GetValue(NavigatorWindow.SelectedDocumentProperty);
    set => this.SetValue(NavigatorWindow.SelectedDocumentProperty, (object) value);
  }

  private static void OnSelectedDocumentChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((NavigatorWindow) d).OnSelectedDocumentChanged(e);
  }

  protected virtual void OnSelectedDocumentChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this._internalSetSelectedDocument || this.SelectedDocument == null || !this.SelectedDocument.ActivateCommand.CanExecute((object) null))
      return;
    this.Hide();
    this.SelectedDocument.ActivateCommand.Execute((object) null);
  }

  public LayoutAnchorableItem SelectedAnchorable
  {
    get => (LayoutAnchorableItem) this.GetValue(NavigatorWindow.SelectedAnchorableProperty);
    set => this.SetValue(NavigatorWindow.SelectedAnchorableProperty, (object) value);
  }

  private static void OnSelectedAnchorableChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((NavigatorWindow) d).OnSelectedAnchorableChanged(e);
  }

  protected virtual void OnSelectedAnchorableChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this._internalSetSelectedAnchorable)
      return;
    object newValue = e.NewValue;
    if (this.SelectedAnchorable == null || !this.SelectedAnchorable.ActivateCommand.CanExecute((object) null))
      return;
    this.Close();
    this.SelectedAnchorable.ActivateCommand.Execute((object) null);
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this._anchorableListBox = this.GetTemplateChild("PART_AnchorableListBox") as ListBox;
    this._documentListBox = this.GetTemplateChild("PART_DocumentListBox") as ListBox;
  }

  protected override void OnPreviewKeyDown(KeyEventArgs e)
  {
    bool flag = false;
    if (e.Key == Key.Tab)
    {
      if (this._isSelectingDocument)
      {
        if (this.SelectedDocument != null)
        {
          if (this.Documents.IndexOf<LayoutDocumentItem>(this.SelectedDocument) < this.Documents.Length - 1)
          {
            this.SelectNextDocument();
            flag = true;
          }
          else if (this.Anchorables.Count<LayoutAnchorableItem>() > 0)
          {
            this._isSelectingDocument = false;
            this.InternalSetSelectedDocument((LayoutDocumentItem) null);
            this.InternalSetSelectedAnchorable(this.Anchorables.First<LayoutAnchorableItem>());
            flag = true;
          }
        }
        else if (this.Documents.Length != 0)
        {
          this.InternalSetSelectedDocument(this.Documents[0]);
          flag = true;
        }
      }
      else if (this.SelectedAnchorable != null)
      {
        if (this.Anchorables.ToArray<LayoutAnchorableItem>().IndexOf<LayoutAnchorableItem>(this.SelectedAnchorable) < this.Anchorables.Count<LayoutAnchorableItem>() - 1)
        {
          this.SelectNextAnchorable();
          flag = true;
        }
        else if (this.Documents.Length != 0)
        {
          this._isSelectingDocument = true;
          this.InternalSetSelectedAnchorable((LayoutAnchorableItem) null);
          this.InternalSetSelectedDocument(this.Documents[0]);
          flag = true;
        }
      }
      else if (this.Anchorables.Count<LayoutAnchorableItem>() > 0)
      {
        this.InternalSetSelectedAnchorable(this.Anchorables.ToArray<LayoutAnchorableItem>()[0]);
        flag = true;
      }
    }
    if (flag)
      e.Handled = true;
    base.OnPreviewKeyDown(e);
  }

  protected override void OnPreviewKeyUp(KeyEventArgs e)
  {
    if (e.Key != Key.Tab)
    {
      this.Close();
      if (this.SelectedDocument != null && this.SelectedDocument.ActivateCommand.CanExecute((object) null))
        this.SelectedDocument.ActivateCommand.Execute((object) null);
      if (this.SelectedDocument == null && this.SelectedAnchorable != null && this.SelectedAnchorable.ActivateCommand.CanExecute((object) null))
        this.SelectedAnchorable.ActivateCommand.Execute((object) null);
      e.Handled = true;
    }
    base.OnPreviewKeyUp(e);
  }

  protected void SetAnchorables(IEnumerable<LayoutAnchorableItem> value)
  {
    this.SetValue(NavigatorWindow.AnchorablesPropertyKey, (object) value);
  }

  protected void SetDocuments(LayoutDocumentItem[] value)
  {
    this.SetValue(NavigatorWindow.DocumentsPropertyKey, (object) value);
  }

  internal void UpdateThemeResources(Theme oldTheme = null)
  {
    if (oldTheme != null)
    {
      if (oldTheme is DictionaryTheme)
      {
        if (this.currentThemeResourceDictionary != null)
        {
          this.Resources.MergedDictionaries.Remove(this.currentThemeResourceDictionary);
          this.currentThemeResourceDictionary = (ResourceDictionary) null;
        }
      }
      else
      {
        ResourceDictionary resourceDictionary = this.Resources.MergedDictionaries.FirstOrDefault<ResourceDictionary>((Func<ResourceDictionary, bool>) (r => r.Source == oldTheme.GetResourceUri()));
        if (resourceDictionary != null)
          this.Resources.MergedDictionaries.Remove(resourceDictionary);
      }
    }
    if (this._manager.Theme == null)
      return;
    if (this._manager.Theme is DictionaryTheme)
    {
      this.currentThemeResourceDictionary = ((DictionaryTheme) this._manager.Theme).ThemeResourceDictionary;
      this.Resources.MergedDictionaries.Add(this.currentThemeResourceDictionary);
    }
    else
      this.Resources.MergedDictionaries.Add(new ResourceDictionary()
      {
        Source = this._manager.Theme.GetResourceUri()
      });
  }

  internal void SelectNextDocument()
  {
    if (this.SelectedDocument == null)
      return;
    int index = this.Documents.IndexOf<LayoutDocumentItem>(this.SelectedDocument) + 1;
    if (index == this.Documents.Length)
      index = 0;
    this.InternalSetSelectedDocument(this.Documents[index]);
  }

  internal void SelectNextAnchorable()
  {
    if (this.SelectedAnchorable == null)
      return;
    LayoutAnchorableItem[] array = this.Anchorables.ToArray<LayoutAnchorableItem>();
    int index = array.IndexOf<LayoutAnchorableItem>(this.SelectedAnchorable) + 1;
    if (index == this.Anchorables.Count<LayoutAnchorableItem>())
      index = 0;
    this.InternalSetSelectedAnchorable(array[index]);
  }

  private void InternalSetSelectedAnchorable(LayoutAnchorableItem anchorableToSelect)
  {
    this._internalSetSelectedAnchorable = true;
    this.SelectedAnchorable = anchorableToSelect;
    this._internalSetSelectedAnchorable = false;
    if (this._anchorableListBox == null)
      return;
    this._anchorableListBox.Focus();
  }

  private void InternalSetSelectedDocument(LayoutDocumentItem documentToSelect)
  {
    this._internalSetSelectedDocument = true;
    this.SelectedDocument = documentToSelect;
    this._internalSetSelectedDocument = false;
    if (this._documentListBox == null || documentToSelect == null)
      return;
    this._documentListBox.Focus();
  }

  private void OnLoaded(object sender, RoutedEventArgs e)
  {
    this.Loaded -= new RoutedEventHandler(this.OnLoaded);
    if (this._documentListBox != null && this.SelectedDocument != null)
      this._documentListBox.Focus();
    else if (this._anchorableListBox != null && this.SelectedAnchorable != null)
      this._anchorableListBox.Focus();
    this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
  }

  private void OnUnloaded(object sender, RoutedEventArgs e)
  {
    this.Unloaded -= new RoutedEventHandler(this.OnUnloaded);
  }
}
