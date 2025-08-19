// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.CachedBindingInfo`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Scripting.Utils;

internal class CachedBindingInfo<T>(DynamicMetaObjectBinder binder, int compilationThreshold) : 
  CachedBindingInfo(binder, compilationThreshold)
  where T : class
{
  public T CompiledTarget;
  public Expression<T> Target;
  [ThreadStatic]
  public static CachedBindingInfo<T> LastInterpretedFailure;

  public override bool CheckCompiled()
  {
    if (this.Target != null)
    {
      Expression<T> lambda = Interlocked.Exchange<Expression<T>>(ref this.Target, (Expression<T>) null);
      if (lambda != null)
        new Task((Action) (() => this.CompiledTarget = lambda.Compile())).Start();
    }
    if ((object) this.CompiledTarget == null)
      return true;
    CachedBindingInfo<T>.LastInterpretedFailure = this;
    return false;
  }
}
