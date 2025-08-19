// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.AbstractFreezable
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

[Serializable]
internal abstract class AbstractFreezable : IFreezable
{
  private bool isFrozen;

  public bool IsFrozen => this.isFrozen;

  public void Freeze()
  {
    if (this.isFrozen)
      return;
    this.FreezeInternal();
    this.isFrozen = true;
  }

  protected virtual void FreezeInternal()
  {
  }
}
