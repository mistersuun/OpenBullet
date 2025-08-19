// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Primitives.SelectAllSelectorItem
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

#nullable disable
namespace Xceed.Wpf.Toolkit.Primitives;

public class SelectAllSelectorItem : SelectorItem
{
  private bool _ignoreSelectorChanges;

  protected override void OnIsSelectedChanged(bool? oldValue, bool? newValue)
  {
    if (this._ignoreSelectorChanges || !(this.TemplatedParent is SelectAllSelector templatedParent) || !newValue.HasValue)
      return;
    if (newValue.Value)
      templatedParent.SelectAll();
    else
      templatedParent.UnSelectAll();
  }

  internal void ModifyCurrentSelection(bool? newSelection)
  {
    this._ignoreSelectorChanges = true;
    this.IsSelected = newSelection;
    this._ignoreSelectorChanges = false;
  }
}
