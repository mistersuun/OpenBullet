// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlFormControlElementWithState
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;

#nullable disable
namespace AngleSharp.Html.Dom;

internal abstract class HtmlFormControlElementWithState : HtmlFormControlElement
{
  public HtmlFormControlElementWithState(
    Document owner,
    string name,
    string prefix,
    NodeFlags flags = NodeFlags.None)
    : base(owner, name, prefix, flags)
  {
    this.CanContainRangeEndpoint = false;
  }

  internal bool CanContainRangeEndpoint { get; private set; }

  internal bool ShouldSaveAndRestoreFormControlState { get; private set; }

  internal abstract FormControlState SaveControlState();

  internal abstract void RestoreFormControlState(FormControlState state);
}
