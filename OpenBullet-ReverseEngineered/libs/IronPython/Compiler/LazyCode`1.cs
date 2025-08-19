// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.LazyCode`1
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler;

internal sealed class LazyCode<T> : IExpressionSerializable where T : class
{
  public Expression<T> Code;
  private T Delegate;
  private readonly bool _shouldInterpret;
  private readonly int _compilationThreshold;

  public LazyCode(Expression<T> code, bool shouldInterpret, int compilationThreshold)
  {
    this.Code = code;
    this._shouldInterpret = shouldInterpret;
    this._compilationThreshold = compilationThreshold;
  }

  public T EnsureDelegate()
  {
    if ((object) this.Delegate == null)
    {
      lock (this)
      {
        if ((object) this.Delegate == null)
        {
          this.Delegate = this.Compile();
          this.Code = (Expression<T>) null;
        }
      }
    }
    return this.Delegate;
  }

  private T Compile()
  {
    return this._shouldInterpret ? this.Code.LightCompile<T>(this._compilationThreshold) : this.Code.Compile();
  }

  public Expression CreateExpression() => (Expression) this.Code;
}
