// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Html.Dom.Events;
using AngleSharp.Text;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Html.Dom;

public class HtmlElement(Document owner, string localName, string prefix = null, NodeFlags flags = NodeFlags.None) : 
  Element(owner, HtmlElement.Combine(prefix, localName), localName, prefix, NamespaceNames.HtmlUri, flags | NodeFlags.HtmlMember),
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers
{
  private StringMap _dataset;
  private IHtmlMenuElement _menu;
  private SettableTokenList _dropZone;

  public event DomEventHandler Aborted
  {
    add => this.AddEventListener(EventNames.Abort, value, false);
    remove => this.RemoveEventListener(EventNames.Abort, value, false);
  }

  public event DomEventHandler Blurred
  {
    add => this.AddEventListener(EventNames.Blur, value, false);
    remove => this.RemoveEventListener(EventNames.Blur, value, false);
  }

  public event DomEventHandler Cancelled
  {
    add => this.AddEventListener(EventNames.Cancel, value, false);
    remove => this.RemoveEventListener(EventNames.Cancel, value, false);
  }

  public event DomEventHandler CanPlay
  {
    add => this.AddEventListener(EventNames.CanPlay, value, false);
    remove => this.RemoveEventListener(EventNames.CanPlay, value, false);
  }

  public event DomEventHandler CanPlayThrough
  {
    add => this.AddEventListener(EventNames.CanPlayThrough, value, false);
    remove => this.RemoveEventListener(EventNames.CanPlayThrough, value, false);
  }

  public event DomEventHandler Changed
  {
    add => this.AddEventListener(EventNames.Change, value, false);
    remove => this.RemoveEventListener(EventNames.Change, value, false);
  }

  public event DomEventHandler Clicked
  {
    add => this.AddEventListener(EventNames.Click, value, false);
    remove => this.RemoveEventListener(EventNames.Click, value, false);
  }

  public event DomEventHandler CueChanged
  {
    add => this.AddEventListener(EventNames.CueChange, value, false);
    remove => this.RemoveEventListener(EventNames.CueChange, value, false);
  }

  public event DomEventHandler DoubleClick
  {
    add => this.AddEventListener(EventNames.DblClick, value, false);
    remove => this.RemoveEventListener(EventNames.DblClick, value, false);
  }

  public event DomEventHandler Drag
  {
    add => this.AddEventListener(EventNames.Drag, value, false);
    remove => this.RemoveEventListener(EventNames.Drag, value, false);
  }

  public event DomEventHandler DragEnd
  {
    add => this.AddEventListener(EventNames.DragEnd, value, false);
    remove => this.RemoveEventListener(EventNames.DragEnd, value, false);
  }

  public event DomEventHandler DragEnter
  {
    add => this.AddEventListener(EventNames.DragEnter, value, false);
    remove => this.RemoveEventListener(EventNames.DragEnter, value, false);
  }

  public event DomEventHandler DragExit
  {
    add => this.AddEventListener(EventNames.DragExit, value, false);
    remove => this.RemoveEventListener(EventNames.DragExit, value, false);
  }

  public event DomEventHandler DragLeave
  {
    add => this.AddEventListener(EventNames.DragLeave, value, false);
    remove => this.RemoveEventListener(EventNames.DragLeave, value, false);
  }

  public event DomEventHandler DragOver
  {
    add => this.AddEventListener(EventNames.DragOver, value, false);
    remove => this.RemoveEventListener(EventNames.DragOver, value, false);
  }

  public event DomEventHandler DragStart
  {
    add => this.AddEventListener(EventNames.DragStart, value, false);
    remove => this.RemoveEventListener(EventNames.DragStart, value, false);
  }

  public event DomEventHandler Dropped
  {
    add => this.AddEventListener(EventNames.Drop, value, false);
    remove => this.RemoveEventListener(EventNames.Drop, value, false);
  }

  public event DomEventHandler DurationChanged
  {
    add => this.AddEventListener(EventNames.DurationChange, value, false);
    remove => this.RemoveEventListener(EventNames.DurationChange, value, false);
  }

  public event DomEventHandler Emptied
  {
    add => this.AddEventListener(EventNames.Emptied, value, false);
    remove => this.RemoveEventListener(EventNames.Emptied, value, false);
  }

  public event DomEventHandler Ended
  {
    add => this.AddEventListener(EventNames.Ended, value, false);
    remove => this.RemoveEventListener(EventNames.Ended, value, false);
  }

  public event DomEventHandler Error
  {
    add => this.AddEventListener(EventNames.Error, value, false);
    remove => this.RemoveEventListener(EventNames.Error, value, false);
  }

  public event DomEventHandler Focused
  {
    add => this.AddEventListener(EventNames.Focus, value, false);
    remove => this.RemoveEventListener(EventNames.Focus, value, false);
  }

  public event DomEventHandler Input
  {
    add => this.AddEventListener(EventNames.Input, value, false);
    remove => this.RemoveEventListener(EventNames.Input, value, false);
  }

  public event DomEventHandler Invalid
  {
    add => this.AddEventListener(EventNames.Invalid, value, false);
    remove => this.RemoveEventListener(EventNames.Invalid, value, false);
  }

  public event DomEventHandler KeyDown
  {
    add => this.AddEventListener(EventNames.Keydown, value, false);
    remove => this.RemoveEventListener(EventNames.Keydown, value, false);
  }

  public event DomEventHandler KeyPress
  {
    add => this.AddEventListener(EventNames.Keypress, value, false);
    remove => this.RemoveEventListener(EventNames.Keypress, value, false);
  }

  public event DomEventHandler KeyUp
  {
    add => this.AddEventListener(EventNames.Keyup, value, false);
    remove => this.RemoveEventListener(EventNames.Keyup, value, false);
  }

  public event DomEventHandler Loaded
  {
    add => this.AddEventListener(EventNames.Load, value, false);
    remove => this.RemoveEventListener(EventNames.Load, value, false);
  }

  public event DomEventHandler LoadedData
  {
    add => this.AddEventListener(EventNames.LoadedData, value, false);
    remove => this.RemoveEventListener(EventNames.LoadedData, value, false);
  }

  public event DomEventHandler LoadedMetadata
  {
    add => this.AddEventListener(EventNames.LoadedMetaData, value, false);
    remove => this.RemoveEventListener(EventNames.LoadedMetaData, value, false);
  }

  public event DomEventHandler Loading
  {
    add => this.AddEventListener(EventNames.LoadStart, value, false);
    remove => this.RemoveEventListener(EventNames.LoadStart, value, false);
  }

  public event DomEventHandler MouseDown
  {
    add => this.AddEventListener(EventNames.Mousedown, value, false);
    remove => this.RemoveEventListener(EventNames.Mousedown, value, false);
  }

  public event DomEventHandler MouseEnter
  {
    add => this.AddEventListener(EventNames.Mouseenter, value, false);
    remove => this.RemoveEventListener(EventNames.Mouseenter, value, false);
  }

  public event DomEventHandler MouseLeave
  {
    add => this.AddEventListener(EventNames.Mouseleave, value, false);
    remove => this.RemoveEventListener(EventNames.Mouseleave, value, false);
  }

  public event DomEventHandler MouseMove
  {
    add => this.AddEventListener(EventNames.Mousemove, value, false);
    remove => this.RemoveEventListener(EventNames.Mousemove, value, false);
  }

  public event DomEventHandler MouseOut
  {
    add => this.AddEventListener(EventNames.Mouseout, value, false);
    remove => this.RemoveEventListener(EventNames.Mouseout, value, false);
  }

  public event DomEventHandler MouseOver
  {
    add => this.AddEventListener(EventNames.Mouseover, value, false);
    remove => this.RemoveEventListener(EventNames.Mouseover, value, false);
  }

  public event DomEventHandler MouseUp
  {
    add => this.AddEventListener(EventNames.Mouseup, value, false);
    remove => this.RemoveEventListener(EventNames.Mouseup, value, false);
  }

  public event DomEventHandler MouseWheel
  {
    add => this.AddEventListener(EventNames.Wheel, value, false);
    remove => this.RemoveEventListener(EventNames.Wheel, value, false);
  }

  public event DomEventHandler Paused
  {
    add => this.AddEventListener(EventNames.Pause, value, false);
    remove => this.RemoveEventListener(EventNames.Pause, value, false);
  }

  public event DomEventHandler Played
  {
    add => this.AddEventListener(EventNames.Play, value, false);
    remove => this.RemoveEventListener(EventNames.Play, value, false);
  }

  public event DomEventHandler Playing
  {
    add => this.AddEventListener(EventNames.Playing, value, false);
    remove => this.RemoveEventListener(EventNames.Playing, value, false);
  }

  public event DomEventHandler Progress
  {
    add => this.AddEventListener(EventNames.Progress, value, false);
    remove => this.RemoveEventListener(EventNames.Progress, value, false);
  }

  public event DomEventHandler RateChanged
  {
    add => this.AddEventListener(EventNames.RateChange, value, false);
    remove => this.RemoveEventListener(EventNames.RateChange, value, false);
  }

  public event DomEventHandler Resetted
  {
    add => this.AddEventListener(EventNames.Reset, value, false);
    remove => this.RemoveEventListener(EventNames.Reset, value, false);
  }

  public event DomEventHandler Resized
  {
    add => this.AddEventListener(EventNames.Resize, value, false);
    remove => this.RemoveEventListener(EventNames.Resize, value, false);
  }

  public event DomEventHandler Scrolled
  {
    add => this.AddEventListener(EventNames.Scroll, value, false);
    remove => this.RemoveEventListener(EventNames.Scroll, value, false);
  }

  public event DomEventHandler Seeked
  {
    add => this.AddEventListener(EventNames.Seeked, value, false);
    remove => this.RemoveEventListener(EventNames.Seeked, value, false);
  }

  public event DomEventHandler Seeking
  {
    add => this.AddEventListener(EventNames.Seeking, value, false);
    remove => this.RemoveEventListener(EventNames.Seeking, value, false);
  }

  public event DomEventHandler Selected
  {
    add => this.AddEventListener(EventNames.Select, value, false);
    remove => this.RemoveEventListener(EventNames.Select, value, false);
  }

  public event DomEventHandler Shown
  {
    add => this.AddEventListener(EventNames.Show, value, false);
    remove => this.RemoveEventListener(EventNames.Show, value, false);
  }

  public event DomEventHandler Stalled
  {
    add => this.AddEventListener(EventNames.Stalled, value, false);
    remove => this.RemoveEventListener(EventNames.Stalled, value, false);
  }

  public event DomEventHandler Submitted
  {
    add => this.AddEventListener(EventNames.Submit, value, false);
    remove => this.RemoveEventListener(EventNames.Submit, value, false);
  }

  public event DomEventHandler Suspended
  {
    add => this.AddEventListener(EventNames.Suspend, value, false);
    remove => this.RemoveEventListener(EventNames.Suspend, value, false);
  }

  public event DomEventHandler TimeUpdated
  {
    add => this.AddEventListener(EventNames.TimeUpdate, value, false);
    remove => this.RemoveEventListener(EventNames.TimeUpdate, value, false);
  }

  public event DomEventHandler Toggled
  {
    add => this.AddEventListener(EventNames.Toggle, value, false);
    remove => this.RemoveEventListener(EventNames.Toggle, value, false);
  }

  public event DomEventHandler VolumeChanged
  {
    add => this.AddEventListener(EventNames.VolumeChange, value, false);
    remove => this.RemoveEventListener(EventNames.VolumeChange, value, false);
  }

  public event DomEventHandler Waiting
  {
    add => this.AddEventListener(EventNames.Waiting, value, false);
    remove => this.RemoveEventListener(EventNames.Waiting, value, false);
  }

  public bool IsHidden
  {
    get => this.GetBoolAttribute(AttributeNames.Hidden);
    set => this.SetBoolAttribute(AttributeNames.Hidden, value);
  }

  public IHtmlMenuElement ContextMenu
  {
    get
    {
      if (this._menu == null)
      {
        string ownAttribute = this.GetOwnAttribute(AttributeNames.ContextMenu);
        if (!string.IsNullOrEmpty(ownAttribute))
          return this.Owner.GetElementById(ownAttribute) as IHtmlMenuElement;
      }
      return this._menu;
    }
    set => this._menu = value;
  }

  public ISettableTokenList DropZone
  {
    get
    {
      if (this._dropZone == null)
      {
        this._dropZone = new SettableTokenList(this.GetOwnAttribute(AttributeNames.DropZone));
        this._dropZone.Changed += (Action<string>) (value => this.UpdateAttribute(AttributeNames.DropZone, value));
      }
      return (ISettableTokenList) this._dropZone;
    }
  }

  public bool IsDraggable
  {
    get => this.GetOwnAttribute(AttributeNames.Draggable).ToBoolean();
    set => this.SetOwnAttribute(AttributeNames.Draggable, value.ToString());
  }

  public string AccessKey
  {
    get => this.GetOwnAttribute(AttributeNames.AccessKey) ?? string.Empty;
    set => this.SetOwnAttribute(AttributeNames.AccessKey, value);
  }

  public string AccessKeyLabel => this.AccessKey;

  public string Language
  {
    get => this.GetOwnAttribute(AttributeNames.Lang) ?? this.GetDefaultLanguage();
    set => this.SetOwnAttribute(AttributeNames.Lang, value);
  }

  public string Title
  {
    get => this.GetOwnAttribute(AttributeNames.Title);
    set => this.SetOwnAttribute(AttributeNames.Title, value);
  }

  public string Direction
  {
    get => this.GetOwnAttribute(AttributeNames.Dir);
    set => this.SetOwnAttribute(AttributeNames.Dir, value);
  }

  public bool IsSpellChecked
  {
    get => this.GetOwnAttribute(AttributeNames.Spellcheck).ToBoolean();
    set => this.SetOwnAttribute(AttributeNames.Spellcheck, value.ToString());
  }

  public int TabIndex
  {
    get => this.GetOwnAttribute(AttributeNames.TabIndex).ToInteger(0);
    set => this.SetOwnAttribute(AttributeNames.TabIndex, value.ToString());
  }

  public IStringMap Dataset
  {
    get
    {
      return (IStringMap) this._dataset ?? (IStringMap) (this._dataset = new StringMap("data-", (Element) this));
    }
  }

  public string ContentEditable
  {
    get => this.GetOwnAttribute(AttributeNames.ContentEditable);
    set => this.SetOwnAttribute(AttributeNames.ContentEditable, value);
  }

  public bool IsContentEditable
  {
    get
    {
      ContentEditableMode contentEditableMode = this.ContentEditable.ToEnum<ContentEditableMode>(ContentEditableMode.Inherited);
      if (contentEditableMode == ContentEditableMode.True)
        return true;
      IHtmlElement parentElement = this.ParentElement as IHtmlElement;
      return contentEditableMode == ContentEditableMode.Inherited && parentElement != null && parentElement.IsContentEditable;
    }
  }

  public bool IsTranslated
  {
    get
    {
      return this.GetOwnAttribute(AttributeNames.Translate).ToEnum<SimpleChoice>(SimpleChoice.Yes) == SimpleChoice.Yes;
    }
    set => this.SetOwnAttribute(AttributeNames.Translate, value ? AngleSharp.Common.Keywords.Yes : AngleSharp.Common.Keywords.No);
  }

  public override IElement ParseSubtree(string html) => this.ParseHtmlSubtree(html);

  public void DoSpellCheck() => this.Context.GetSpellCheck(this.Language);

  public virtual void DoClick() => this.IsClickedCancelled();

  public virtual void DoFocus()
  {
  }

  public virtual void DoBlur()
  {
  }

  public override Node Clone(Document owner, bool deep)
  {
    HtmlElement htmlElement = this.Context.GetFactory<IElementFactory<Document, HtmlElement>>().Create(owner, this.LocalName, this.Prefix);
    this.CloneElement((Element) htmlElement, owner, deep);
    return (Node) htmlElement;
  }

  internal void UpdateDropZone(string value) => this._dropZone?.Update(value);

  protected Task<bool> IsClickedCancelled()
  {
    return this.Owner.QueueTaskAsync<bool>((Func<CancellationToken, bool>) (_ => this.Fire<MouseEvent>((Action<MouseEvent>) (m => m.Init(EventNames.Click, true, true, this.Owner.DefaultView, 0, 0, 0, 0, 0, false, false, false, false, MouseButton.Primary, (IEventTarget) this)))));
  }

  protected IHtmlFormElement GetAssignedForm()
  {
    INode assignedForm = (INode) this.Parent;
    while (true)
    {
      switch (assignedForm)
      {
        case null:
        case IHtmlFormElement _:
          goto label_3;
        default:
          assignedForm = (INode) assignedForm.ParentElement;
          continue;
      }
    }
label_3:
    if (assignedForm == null)
    {
      string ownAttribute = this.GetOwnAttribute(AttributeNames.Form);
      Document owner = this.Owner;
      if (owner == null || string.IsNullOrEmpty(ownAttribute))
        return (IHtmlFormElement) null;
      assignedForm = (INode) owner.GetElementById(ownAttribute);
    }
    return assignedForm as IHtmlFormElement;
  }

  private string GetDefaultLanguage()
  {
    return !(this.ParentElement is IHtmlElement parentElement) ? this.Context.GetLanguage() : parentElement.Language;
  }

  private static string Combine(string prefix, string localName)
  {
    return (prefix != null ? $"{prefix}:{localName}" : localName).ToUpperInvariant();
  }
}
