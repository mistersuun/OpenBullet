// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.LocalVariable
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

public sealed class LocalVariable
{
  private const int IsBoxedFlag = 1;
  private const int InClosureFlag = 2;
  public readonly int Index;
  private int _flags;

  public bool IsBoxed
  {
    get => (this._flags & 1) != 0;
    set
    {
      if (value)
        this._flags |= 1;
      else
        this._flags &= -2;
    }
  }

  public bool InClosure => (this._flags & 2) != 0;

  public bool InClosureOrBoxed => this.InClosure | this.IsBoxed;

  internal LocalVariable(int index, bool closure, bool boxed)
  {
    this.Index = index;
    this._flags = (closure ? 2 : 0) | (boxed ? 1 : 0);
  }

  internal Expression LoadFromArray(Expression frameData, Expression closure)
  {
    Expression expression = (Expression) Expression.ArrayAccess(this.InClosure ? closure : frameData, (Expression) Expression.Constant((object) this.Index));
    return !this.IsBoxed ? expression : (Expression) Expression.Convert(expression, typeof (StrongBox<object>));
  }

  public override string ToString()
  {
    return $"{this.Index}: {(this.IsBoxed ? (object) "boxed" : (object) (string) null)} {(this.InClosure ? (object) "in closure" : (object) (string) null)}";
  }
}
