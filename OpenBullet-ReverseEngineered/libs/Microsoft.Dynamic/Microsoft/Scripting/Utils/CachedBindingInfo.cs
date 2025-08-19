// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.CachedBindingInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Dynamic;

#nullable disable
namespace Microsoft.Scripting.Utils;

internal abstract class CachedBindingInfo
{
  public readonly DynamicMetaObjectBinder Binder;
  public int CompilationThreshold;

  public CachedBindingInfo(DynamicMetaObjectBinder binder, int compilationThreshold)
  {
    this.Binder = binder;
    this.CompilationThreshold = compilationThreshold;
  }

  public abstract bool CheckCompiled();
}
