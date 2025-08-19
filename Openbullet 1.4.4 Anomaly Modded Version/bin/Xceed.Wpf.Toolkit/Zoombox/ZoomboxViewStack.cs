// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Zoombox.ZoomboxViewStack
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Xceed.Wpf.Toolkit.Core;

#nullable disable
namespace Xceed.Wpf.Toolkit.Zoombox;

public sealed class ZoomboxViewStack : Collection<ZoomboxView>, IWeakEventListener
{
  private IEnumerable _source;
  private WeakReference _zoomboxRef;
  private BitVector32 _cacheBits = new BitVector32(0);

  public ZoomboxViewStack(Xceed.Wpf.Toolkit.Zoombox.Zoombox zoombox)
  {
    this._zoomboxRef = new WeakReference((object) zoombox);
  }

  public ZoomboxView SelectedView
  {
    get
    {
      int viewStackIndex = this.Zoombox.ViewStackIndex;
      return viewStackIndex >= 0 && viewStackIndex <= this.Count - 1 ? this[viewStackIndex] : ZoomboxView.Empty;
    }
  }

  internal bool AreViewsFromSource
  {
    get => this._cacheBits[1];
    set => this._cacheBits[1] = value;
  }

  internal IEnumerable Source => this._source;

  private bool IsChangeFromSource
  {
    get => this._cacheBits[2];
    set => this._cacheBits[2] = value;
  }

  private bool IsMovingViews
  {
    get => this._cacheBits[8];
    set => this._cacheBits[8] = value;
  }

  private bool IsResettingViews
  {
    get => this._cacheBits[4];
    set => this._cacheBits[4] = value;
  }

  private bool IsSettingInitialViewAfterClear
  {
    get => this._cacheBits[16 /*0x10*/];
    set => this._cacheBits[16 /*0x10*/] = value;
  }

  private Xceed.Wpf.Toolkit.Zoombox.Zoombox Zoombox => this._zoomboxRef.Target as Xceed.Wpf.Toolkit.Zoombox.Zoombox;

  internal void ClearViewStackSource()
  {
    if (!this.AreViewsFromSource)
      return;
    this.AreViewsFromSource = false;
    this.MonitorSource(false);
    this._source = (IEnumerable) null;
    using (new ZoomboxViewStack.SourceAccess(this))
      this.Clear();
    this.Zoombox.CoerceValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewStackModeProperty);
  }

  internal void PushView(ZoomboxView view)
  {
    int viewStackIndex = this.Zoombox.ViewStackIndex;
    while (this.Count - 1 > viewStackIndex)
      this.RemoveAt(this.Count - 1);
    this.Add(view);
  }

  internal void SetViewStackSource(IEnumerable source)
  {
    if (this._source == source)
      return;
    this.MonitorSource(false);
    this._source = source;
    this.MonitorSource(true);
    this.AreViewsFromSource = true;
    this.Zoombox.CoerceValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewStackModeProperty);
    this.ResetViews();
  }

  protected override void ClearItems()
  {
    this.VerifyStackModification();
    bool flag = this.Zoombox.CurrentViewIndex >= 0;
    base.ClearItems();
    this.Zoombox.SetViewStackCount(this.Count);
    if (this.IsResettingViews)
      return;
    if (this.Zoombox.EffectiveViewStackMode == ZoomboxViewStackMode.Auto && this.Zoombox.CurrentView != ZoomboxView.Empty)
    {
      this.IsSettingInitialViewAfterClear = true;
      try
      {
        this.Add(this.Zoombox.CurrentView);
      }
      finally
      {
        this.IsSettingInitialViewAfterClear = false;
      }
      this.Zoombox.ViewStackIndex = 0;
      if (!flag)
        return;
      this.Zoombox.SetCurrentViewIndex(0);
    }
    else
    {
      this.Zoombox.ViewStackIndex = -1;
      this.Zoombox.SetCurrentViewIndex(-1);
    }
  }

  protected override void InsertItem(int index, ZoomboxView view)
  {
    this.VerifyStackModification();
    if (this.Zoombox.HasArrangedContentPresenter && this.Zoombox.ViewStackIndex >= index && !this.IsSettingInitialViewAfterClear && !this.IsResettingViews && !this.IsMovingViews)
    {
      bool isUpdatingView = this.Zoombox.IsUpdatingView;
      this.Zoombox.IsUpdatingView = true;
      try
      {
        ++this.Zoombox.ViewStackIndex;
        if (this.Zoombox.CurrentViewIndex != -1)
          this.Zoombox.SetCurrentViewIndex(this.Zoombox.CurrentViewIndex + 1);
      }
      finally
      {
        this.Zoombox.IsUpdatingView = isUpdatingView;
      }
    }
    base.InsertItem(index, view);
    this.Zoombox.SetViewStackCount(this.Count);
  }

  protected override void RemoveItem(int index)
  {
    this.VerifyStackModification();
    bool flag = this.Zoombox.ViewStackIndex == index;
    if (!this.IsMovingViews && this.Zoombox.HasArrangedContentPresenter && (this.Zoombox.ViewStackIndex > index || flag && this.Zoombox.ViewStackIndex == this.Zoombox.ViewStack.Count - 1))
    {
      if (flag && this.Zoombox.ViewStack.Count == 1)
      {
        this.Clear();
        return;
      }
      bool isUpdatingView = this.Zoombox.IsUpdatingView;
      this.Zoombox.IsUpdatingView = true;
      try
      {
        --this.Zoombox.ViewStackIndex;
        if (this.Zoombox.CurrentViewIndex != -1)
          this.Zoombox.SetCurrentViewIndex(this.Zoombox.CurrentViewIndex - 1);
      }
      finally
      {
        this.Zoombox.IsUpdatingView = isUpdatingView;
      }
    }
    base.RemoveItem(index);
    if (!this.IsMovingViews & flag && this.Zoombox.CurrentViewIndex != -1)
      this.Zoombox.RefocusView();
    this.Zoombox.SetViewStackCount(this.Count);
  }

  protected override void SetItem(int index, ZoomboxView view)
  {
    this.VerifyStackModification();
    base.SetItem(index, view);
    if (index != this.Zoombox.CurrentViewIndex)
      return;
    this.Zoombox.RefocusView();
  }

  private static ZoomboxView GetViewFromSourceItem(object item)
  {
    ZoomboxView zoomboxView = (object) (item as ZoomboxView) != null ? item as ZoomboxView : ZoomboxViewConverter.Converter.ConvertFrom(item) as ZoomboxView;
    return !(zoomboxView == (ZoomboxView) null) ? zoomboxView : throw new InvalidCastException(string.Format(ErrorMessages.GetMessage("UnableToConvertToZoomboxView"), item));
  }

  private void InsertViews(int index, IList newItems)
  {
    using (new ZoomboxViewStack.SourceAccess(this))
    {
      foreach (object newItem in (IEnumerable) newItems)
      {
        ZoomboxView viewFromSourceItem = ZoomboxViewStack.GetViewFromSourceItem(newItem);
        if (index >= this.Count)
          this.Add(viewFromSourceItem);
        else
          this.Insert(index, viewFromSourceItem);
        ++index;
      }
    }
  }

  private void MonitorSource(bool monitor)
  {
    if (this._source == null || !(this._source is INotifyCollectionChanged))
      return;
    if (monitor)
      CollectionChangedEventManager.AddListener(this._source as INotifyCollectionChanged, (IWeakEventListener) this);
    else
      CollectionChangedEventManager.RemoveListener(this._source as INotifyCollectionChanged, (IWeakEventListener) this);
  }

  private void MoveViews(int oldIndex, int newIndex, IList movedItems)
  {
    using (new ZoomboxViewStack.SourceAccess(this))
    {
      int viewStackIndex = this.Zoombox.ViewStackIndex;
      int num = viewStackIndex;
      if ((oldIndex >= viewStackIndex || newIndex >= viewStackIndex) && (oldIndex <= viewStackIndex || newIndex <= viewStackIndex))
      {
        if (viewStackIndex >= oldIndex && viewStackIndex < oldIndex + movedItems.Count)
          num += newIndex - oldIndex;
        else if (viewStackIndex >= newIndex)
          num += movedItems.Count;
      }
      this.IsMovingViews = true;
      try
      {
        for (int index = 0; index < movedItems.Count; ++index)
          this.RemoveAt(oldIndex);
        for (int index = 0; index < movedItems.Count; ++index)
          this.Insert(newIndex + index, ZoomboxViewStack.GetViewFromSourceItem(movedItems[index]));
        if (num == viewStackIndex)
          return;
        this.Zoombox.ViewStackIndex = num;
        this.Zoombox.SetCurrentViewIndex(num);
      }
      finally
      {
        this.IsMovingViews = false;
      }
    }
  }

  private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
  {
    switch (e.Action)
    {
      case NotifyCollectionChangedAction.Add:
        this.InsertViews(e.NewStartingIndex, e.NewItems);
        break;
      case NotifyCollectionChangedAction.Remove:
        this.RemoveViews(e.OldStartingIndex, e.OldItems);
        break;
      case NotifyCollectionChangedAction.Replace:
        this.ResetViews();
        break;
      case NotifyCollectionChangedAction.Move:
        this.MoveViews(e.OldStartingIndex, e.NewStartingIndex, e.OldItems);
        break;
      case NotifyCollectionChangedAction.Reset:
        this.ResetViews();
        break;
    }
  }

  private void ResetViews()
  {
    using (new ZoomboxViewStack.SourceAccess(this))
    {
      int viewStackIndex = this.Zoombox.ViewStackIndex;
      this.IsResettingViews = true;
      try
      {
        this.Clear();
        foreach (object obj in this._source)
          this.Add(ZoomboxViewStack.GetViewFromSourceItem(obj));
        int num = Math.Min(Math.Max(0, viewStackIndex), this.Count - 1);
        this.Zoombox.ViewStackIndex = num;
        this.Zoombox.SetCurrentViewIndex(num);
        this.Zoombox.RefocusView();
      }
      finally
      {
        this.IsResettingViews = false;
      }
    }
  }

  private void RemoveViews(int index, IList removedItems)
  {
    using (new ZoomboxViewStack.SourceAccess(this))
    {
      for (int index1 = 0; index1 < removedItems.Count; ++index1)
        this.RemoveAt(index);
    }
  }

  private void VerifyStackModification()
  {
    if (this.AreViewsFromSource && !this.IsChangeFromSource)
      throw new InvalidOperationException(ErrorMessages.GetMessage("ViewStackCannotBeManipulatedNow"));
  }

  public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
  {
    if (!(managerType == typeof (CollectionChangedEventManager)))
      return false;
    this.OnSourceCollectionChanged(sender, (NotifyCollectionChangedEventArgs) e);
    return true;
  }

  private sealed class SourceAccess : IDisposable
  {
    private ZoomboxViewStack _viewStack;

    public SourceAccess(ZoomboxViewStack viewStack)
    {
      this._viewStack = viewStack;
      this._viewStack.IsChangeFromSource = true;
    }

    ~SourceAccess() => this.Dispose();

    public void Dispose()
    {
      this._viewStack.IsChangeFromSource = false;
      this._viewStack = (ZoomboxViewStack) null;
      GC.SuppressFinalize((object) this);
    }
  }

  private enum CacheBits
  {
    AreViewsFromSource = 1,
    IsChangeFromSource = 2,
    IsResettingViews = 4,
    IsMovingViews = 8,
    IsSettingInitialViewAfterClear = 16, // 0x00000010
  }
}
