// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Window
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Browser;
using AngleSharp.Browser.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Html.Dom;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class Window : 
  EventTarget,
  IWindow,
  IEventTarget,
  IGlobalEventHandlers,
  IWindowEventHandlers,
  IWindowTimers,
  IDisposable
{
  private readonly AngleSharp.Dom.Document _document;
  private string _name;
  private int _outerHeight;
  private int _outerWidth;
  private int _screenX;
  private int _screenY;
  private string _status;
  private bool _closed;
  private INavigator _navigator;

  public Window(AngleSharp.Dom.Document document) => this._document = document;

  public IWindow Proxy => this._document.Context.Current;

  public INavigator Navigator
  {
    get => this._navigator ?? (this._navigator = this._document.Context.GetService<INavigator>());
  }

  public IDocument Document => (IDocument) this._document;

  public string Name
  {
    get => this._name;
    set => this._name = value;
  }

  public int OuterHeight
  {
    get => this._outerHeight;
    set => this._outerHeight = value;
  }

  public int OuterWidth
  {
    get => this._outerWidth;
    set => this._outerWidth = value;
  }

  public int ScreenX
  {
    get => this._screenX;
    set => this._screenX = value;
  }

  public int ScreenY
  {
    get => this._screenY;
    set => this._screenY = value;
  }

  public ILocation Location => this.Document.Location;

  public string Status
  {
    get => this._status;
    set => this._status = value;
  }

  public bool IsClosed => this._closed;

  event DomEventHandler IGlobalEventHandlers.Aborted
  {
    add => this.AddEventListener(EventNames.Abort, value, false);
    remove => this.RemoveEventListener(EventNames.Abort, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Blurred
  {
    add => this.AddEventListener(EventNames.Blur, value, false);
    remove => this.RemoveEventListener(EventNames.Blur, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Cancelled
  {
    add => this.AddEventListener(EventNames.Cancel, value, false);
    remove => this.RemoveEventListener(EventNames.Cancel, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.CanPlay
  {
    add => this.AddEventListener(EventNames.CanPlay, value, false);
    remove => this.RemoveEventListener(EventNames.CanPlay, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.CanPlayThrough
  {
    add => this.AddEventListener(EventNames.CanPlayThrough, value, false);
    remove => this.RemoveEventListener(EventNames.CanPlayThrough, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Changed
  {
    add => this.AddEventListener(EventNames.Change, value, false);
    remove => this.RemoveEventListener(EventNames.Change, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Clicked
  {
    add => this.AddEventListener(EventNames.Click, value, false);
    remove => this.RemoveEventListener(EventNames.Click, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.CueChanged
  {
    add => this.AddEventListener(EventNames.CueChange, value, false);
    remove => this.RemoveEventListener(EventNames.CueChange, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.DoubleClick
  {
    add => this.AddEventListener(EventNames.DblClick, value, false);
    remove => this.RemoveEventListener(EventNames.DblClick, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Drag
  {
    add => this.AddEventListener(EventNames.Drag, value, false);
    remove => this.RemoveEventListener(EventNames.Drag, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.DragEnd
  {
    add => this.AddEventListener(EventNames.DragEnd, value, false);
    remove => this.RemoveEventListener(EventNames.DragEnd, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.DragEnter
  {
    add => this.AddEventListener(EventNames.DragEnter, value, false);
    remove => this.RemoveEventListener(EventNames.DragEnter, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.DragExit
  {
    add => this.AddEventListener(EventNames.DragExit, value, false);
    remove => this.RemoveEventListener(EventNames.DragExit, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.DragLeave
  {
    add => this.AddEventListener(EventNames.DragLeave, value, false);
    remove => this.RemoveEventListener(EventNames.DragLeave, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.DragOver
  {
    add => this.AddEventListener(EventNames.DragOver, value, false);
    remove => this.RemoveEventListener(EventNames.DragOver, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.DragStart
  {
    add => this.AddEventListener(EventNames.DragStart, value, false);
    remove => this.RemoveEventListener(EventNames.DragStart, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Dropped
  {
    add => this.AddEventListener(EventNames.Drop, value, false);
    remove => this.RemoveEventListener(EventNames.Drop, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.DurationChanged
  {
    add => this.AddEventListener(EventNames.DurationChange, value, false);
    remove => this.RemoveEventListener(EventNames.DurationChange, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Emptied
  {
    add => this.AddEventListener(EventNames.Emptied, value, false);
    remove => this.RemoveEventListener(EventNames.Emptied, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Ended
  {
    add => this.AddEventListener(EventNames.Ended, value, false);
    remove => this.RemoveEventListener(EventNames.Ended, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Error
  {
    add => this.AddEventListener(EventNames.Error, value, false);
    remove => this.RemoveEventListener(EventNames.Error, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Focused
  {
    add => this.AddEventListener(EventNames.Focus, value, false);
    remove => this.RemoveEventListener(EventNames.Focus, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Input
  {
    add => this.AddEventListener(EventNames.Input, value, false);
    remove => this.RemoveEventListener(EventNames.Input, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Invalid
  {
    add => this.AddEventListener(EventNames.Invalid, value, false);
    remove => this.RemoveEventListener(EventNames.Invalid, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.KeyDown
  {
    add => this.AddEventListener(EventNames.Keydown, value, false);
    remove => this.RemoveEventListener(EventNames.Keydown, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.KeyPress
  {
    add => this.AddEventListener(EventNames.Keypress, value, false);
    remove => this.RemoveEventListener(EventNames.Keypress, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.KeyUp
  {
    add => this.AddEventListener(EventNames.Keyup, value, false);
    remove => this.RemoveEventListener(EventNames.Keyup, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Loaded
  {
    add => this.AddEventListener(EventNames.Load, value, false);
    remove => this.RemoveEventListener(EventNames.Load, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.LoadedData
  {
    add => this.AddEventListener(EventNames.LoadedData, value, false);
    remove => this.RemoveEventListener(EventNames.LoadedData, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.LoadedMetadata
  {
    add => this.AddEventListener(EventNames.LoadedMetaData, value, false);
    remove => this.RemoveEventListener(EventNames.LoadedMetaData, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Loading
  {
    add => this.AddEventListener(EventNames.LoadStart, value, false);
    remove => this.RemoveEventListener(EventNames.LoadStart, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.MouseDown
  {
    add => this.AddEventListener(EventNames.Mousedown, value, false);
    remove => this.RemoveEventListener(EventNames.Mousedown, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.MouseEnter
  {
    add => this.AddEventListener(EventNames.Mouseenter, value, false);
    remove => this.RemoveEventListener(EventNames.Mouseenter, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.MouseLeave
  {
    add => this.AddEventListener(EventNames.Mouseleave, value, false);
    remove => this.RemoveEventListener(EventNames.Mouseleave, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.MouseMove
  {
    add => this.AddEventListener(EventNames.Mousemove, value, false);
    remove => this.RemoveEventListener(EventNames.Mousemove, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.MouseOut
  {
    add => this.AddEventListener(EventNames.Mouseout, value, false);
    remove => this.RemoveEventListener(EventNames.Mouseout, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.MouseOver
  {
    add => this.AddEventListener(EventNames.Mouseover, value, false);
    remove => this.RemoveEventListener(EventNames.Mouseover, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.MouseUp
  {
    add => this.AddEventListener(EventNames.Mouseup, value, false);
    remove => this.RemoveEventListener(EventNames.Mouseup, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.MouseWheel
  {
    add => this.AddEventListener(EventNames.Wheel, value, false);
    remove => this.RemoveEventListener(EventNames.Wheel, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Paused
  {
    add => this.AddEventListener(EventNames.Pause, value, false);
    remove => this.RemoveEventListener(EventNames.Pause, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Played
  {
    add => this.AddEventListener(EventNames.Play, value, false);
    remove => this.RemoveEventListener(EventNames.Play, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Playing
  {
    add => this.AddEventListener(EventNames.Playing, value, false);
    remove => this.RemoveEventListener(EventNames.Playing, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Progress
  {
    add => this.AddEventListener(EventNames.Progress, value, false);
    remove => this.RemoveEventListener(EventNames.Progress, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.RateChanged
  {
    add => this.AddEventListener(EventNames.RateChange, value, false);
    remove => this.RemoveEventListener(EventNames.RateChange, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Resetted
  {
    add => this.AddEventListener(EventNames.Reset, value, false);
    remove => this.RemoveEventListener(EventNames.Reset, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Resized
  {
    add => this.AddEventListener(EventNames.Resize, value, false);
    remove => this.RemoveEventListener(EventNames.Resize, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Scrolled
  {
    add => this.AddEventListener(EventNames.Scroll, value, false);
    remove => this.RemoveEventListener(EventNames.Scroll, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Seeked
  {
    add => this.AddEventListener(EventNames.Seeked, value, false);
    remove => this.RemoveEventListener(EventNames.Seeked, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Seeking
  {
    add => this.AddEventListener(EventNames.Seeking, value, false);
    remove => this.RemoveEventListener(EventNames.Seeking, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Selected
  {
    add => this.AddEventListener(EventNames.Select, value, false);
    remove => this.RemoveEventListener(EventNames.Select, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Shown
  {
    add => this.AddEventListener(EventNames.Show, value, false);
    remove => this.RemoveEventListener(EventNames.Show, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Stalled
  {
    add => this.AddEventListener(EventNames.Stalled, value, false);
    remove => this.RemoveEventListener(EventNames.Stalled, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Submitted
  {
    add => this.AddEventListener(EventNames.Submit, value, false);
    remove => this.RemoveEventListener(EventNames.Submit, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Suspended
  {
    add => this.AddEventListener(EventNames.Suspend, value, false);
    remove => this.RemoveEventListener(EventNames.Suspend, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.TimeUpdated
  {
    add => this.AddEventListener(EventNames.TimeUpdate, value, false);
    remove => this.RemoveEventListener(EventNames.TimeUpdate, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Toggled
  {
    add => this.AddEventListener(EventNames.Toggle, value, false);
    remove => this.RemoveEventListener(EventNames.Toggle, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.VolumeChanged
  {
    add => this.AddEventListener(EventNames.VolumeChange, value, false);
    remove => this.RemoveEventListener(EventNames.VolumeChange, value, false);
  }

  event DomEventHandler IGlobalEventHandlers.Waiting
  {
    add => this.AddEventListener(EventNames.Waiting, value, false);
    remove => this.RemoveEventListener(EventNames.Waiting, value, false);
  }

  event DomEventHandler IWindowEventHandlers.Printed
  {
    add => this.AddEventListener(EventNames.AfterPrint, value, false);
    remove => this.RemoveEventListener(EventNames.AfterPrint, value, false);
  }

  event DomEventHandler IWindowEventHandlers.Printing
  {
    add => this.AddEventListener(EventNames.BeforePrint, value, false);
    remove => this.RemoveEventListener(EventNames.BeforePrint, value, false);
  }

  event DomEventHandler IWindowEventHandlers.Unloading
  {
    add => this.AddEventListener(EventNames.Unloading, value, false);
    remove => this.RemoveEventListener(EventNames.Unloading, value, false);
  }

  event DomEventHandler IWindowEventHandlers.HashChanged
  {
    add => this.AddEventListener(EventNames.HashChange, value, false);
    remove => this.RemoveEventListener(EventNames.HashChange, value, false);
  }

  event DomEventHandler IWindowEventHandlers.MessageReceived
  {
    add => this.AddEventListener(EventNames.Message, value, false);
    remove => this.RemoveEventListener(EventNames.Message, value, false);
  }

  event DomEventHandler IWindowEventHandlers.WentOffline
  {
    add => this.AddEventListener(EventNames.Offline, value, false);
    remove => this.RemoveEventListener(EventNames.Offline, value, false);
  }

  event DomEventHandler IWindowEventHandlers.WentOnline
  {
    add => this.AddEventListener(EventNames.Online, value, false);
    remove => this.RemoveEventListener(EventNames.Online, value, false);
  }

  event DomEventHandler IWindowEventHandlers.PageHidden
  {
    add => this.AddEventListener(EventNames.PageHide, value, false);
    remove => this.RemoveEventListener(EventNames.PageHide, value, false);
  }

  event DomEventHandler IWindowEventHandlers.PageShown
  {
    add => this.AddEventListener(EventNames.PageShow, value, false);
    remove => this.RemoveEventListener(EventNames.PageShow, value, false);
  }

  event DomEventHandler IWindowEventHandlers.PopState
  {
    add => this.AddEventListener(EventNames.PopState, value, false);
    remove => this.RemoveEventListener(EventNames.PopState, value, false);
  }

  event DomEventHandler IWindowEventHandlers.Storage
  {
    add => this.AddEventListener(EventNames.Storage, value, false);
    remove => this.RemoveEventListener(EventNames.Storage, value, false);
  }

  event DomEventHandler IWindowEventHandlers.Unloaded
  {
    add => this.AddEventListener(EventNames.Unload, value, false);
    remove => this.RemoveEventListener(EventNames.Unload, value, false);
  }

  IHistory IWindow.History => this._document.Context.SessionHistory;

  IWindow IWindow.Open(string url, string name, string features, string replace)
  {
    HtmlDocument htmlDocument = new HtmlDocument(this._document.Context.CreateChild(name, Sandboxes.None));
    htmlDocument.Location.Href = url;
    return (IWindow) new Window((AngleSharp.Dom.Document) htmlDocument)
    {
      Name = name
    };
  }

  void IWindow.Close() => this._closed = true;

  void IWindow.Stop()
  {
  }

  void IWindow.Focus()
  {
  }

  void IWindow.Blur()
  {
  }

  void IWindow.Alert(string message)
  {
  }

  bool IWindow.Confirm(string message) => false;

  void IWindow.Print()
  {
  }

  int IWindowTimers.SetTimeout(Action<IWindow> handler, int timeout)
  {
    return this.QueueTask(new Func<Action<IWindow>, int, CancellationTokenSource, Task>(this.DoTimeoutAsync), handler, timeout);
  }

  void IWindowTimers.ClearTimeout(int handle) => this.Clear(handle);

  void IWindowTimers.ClearInterval(int handle) => this.Clear(handle);

  int IWindowTimers.SetInterval(Action<IWindow> handler, int timeout)
  {
    return this.QueueTask(new Func<Action<IWindow>, int, CancellationTokenSource, Task>(this.DoIntervalAsync), handler, timeout);
  }

  private async Task DoTimeoutAsync(
    Action<IWindow> callback,
    int timeout,
    CancellationTokenSource cts)
  {
    CancellationToken token = cts.Token;
    await Task.Delay(timeout, token).ConfigureAwait(false);
    if (token.IsCancellationRequested)
      return;
    this._document.QueueTask((Action) (() => callback((IWindow) this)));
  }

  private async Task DoIntervalAsync(
    Action<IWindow> callback,
    int timeout,
    CancellationTokenSource cts)
  {
    CancellationToken token = cts.Token;
    while (!token.IsCancellationRequested)
      await this.DoTimeoutAsync(callback, timeout, cts).ConfigureAwait(false);
  }

  private int QueueTask(
    Func<Action<IWindow>, int, CancellationTokenSource, Task> taskCreator,
    Action<IWindow> callback,
    int timeout)
  {
    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    Task task = taskCreator(callback, timeout, cancellationTokenSource);
    this._document.AttachReference((object) cancellationTokenSource);
    return cancellationTokenSource.GetHashCode();
  }

  private void Clear(int handle)
  {
    CancellationTokenSource cancellationTokenSource = this._document.GetAttachedReferences<CancellationTokenSource>().Where<CancellationTokenSource>((Func<CancellationTokenSource, bool>) (m => m.GetHashCode() == handle)).FirstOrDefault<CancellationTokenSource>();
    if (cancellationTokenSource == null || cancellationTokenSource.IsCancellationRequested)
      return;
    cancellationTokenSource.Cancel();
  }

  public void Dispose()
  {
    foreach (CancellationTokenSource attachedReference in this._document.GetAttachedReferences<CancellationTokenSource>())
      attachedReference.Cancel();
  }
}
