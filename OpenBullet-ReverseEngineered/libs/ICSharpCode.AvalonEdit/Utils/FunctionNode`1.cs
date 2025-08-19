// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.FunctionNode`1
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

internal sealed class FunctionNode<T> : RopeNode<T>
{
  private Func<Rope<T>> initializer;
  private RopeNode<T> cachedResults;

  public FunctionNode(int length, Func<Rope<T>> initializer)
  {
    this.length = length;
    this.initializer = initializer;
    this.isShared = true;
  }

  internal override RopeNode<T> GetContentNode()
  {
    lock (this)
    {
      if (this.cachedResults == null)
      {
        Func<Rope<T>> func = this.initializer != null ? this.initializer : throw new InvalidOperationException("Trying to load this node recursively; or: a previous call to a rope initializer failed.");
        this.initializer = (Func<Rope<T>>) null;
        RopeNode<T> root = (func() ?? throw new InvalidOperationException("Rope initializer returned null.")).root;
        root.Publish();
        if (root.length != this.length)
          throw new InvalidOperationException("Rope initializer returned rope with incorrect length.");
        this.cachedResults = root.height != (byte) 0 || root.contents != null ? root : root.GetContentNode();
      }
      return this.cachedResults;
    }
  }
}
