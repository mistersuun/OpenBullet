// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.LabelScopeInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class LabelScopeInfo
{
  private HybridReferenceDictionary<LabelTarget, LabelInfo> Labels;
  internal readonly LabelScopeKind Kind;
  internal readonly LabelScopeInfo Parent;

  internal LabelScopeInfo(LabelScopeInfo parent, LabelScopeKind kind)
  {
    this.Parent = parent;
    this.Kind = kind;
  }

  internal bool CanJumpInto
  {
    get
    {
      switch (this.Kind)
      {
        case LabelScopeKind.Statement:
        case LabelScopeKind.Block:
        case LabelScopeKind.Switch:
        case LabelScopeKind.Lambda:
          return true;
        default:
          return false;
      }
    }
  }

  internal bool ContainsTarget(LabelTarget target)
  {
    return this.Labels != null && this.Labels.ContainsKey(target);
  }

  internal bool TryGetLabelInfo(LabelTarget target, out LabelInfo info)
  {
    if (this.Labels != null)
      return this.Labels.TryGetValue(target, out info);
    info = (LabelInfo) null;
    return false;
  }

  internal void AddLabelInfo(LabelTarget target, LabelInfo info)
  {
    if (this.Labels == null)
      this.Labels = new HybridReferenceDictionary<LabelTarget, LabelInfo>();
    this.Labels[target] = info;
  }
}
