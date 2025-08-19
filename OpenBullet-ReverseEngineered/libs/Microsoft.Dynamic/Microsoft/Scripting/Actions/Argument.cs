// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Argument
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Actions;

public struct Argument : IEquatable<Argument>
{
  private readonly ArgumentType _kind;
  private readonly string _name;
  public static readonly Argument Simple = new Argument(ArgumentType.Simple, (string) null);

  public ArgumentType Kind => this._kind;

  public string Name => this._name;

  public Argument(string name)
  {
    this._kind = ArgumentType.Named;
    this._name = name;
  }

  public Argument(ArgumentType kind)
  {
    this._kind = kind;
    this._name = (string) null;
  }

  public Argument(ArgumentType kind, string name)
  {
    ContractUtils.Requires(kind == ArgumentType.Named ^ name == null, nameof (kind));
    this._kind = kind;
    this._name = name;
  }

  public override bool Equals(object obj) => obj is Argument other && this.Equals(other);

  public bool Equals(Argument other) => this._kind == other._kind && this._name == other._name;

  public static bool operator ==(Argument left, Argument right) => left.Equals(right);

  public static bool operator !=(Argument left, Argument right) => !left.Equals(right);

  public override int GetHashCode()
  {
    return this._name == null ? (int) this._kind : (int) ((ArgumentType) this._name.GetHashCode() ^ this._kind);
  }

  public bool IsSimple => this.Equals(Argument.Simple);

  public override string ToString()
  {
    return this._name != null ? $"{this._kind.ToString()}:{this._name}" : this._kind.ToString();
  }

  internal Expression CreateExpression()
  {
    return (Expression) Expression.New(typeof (Argument).GetConstructor(new Type[2]
    {
      typeof (ArgumentType),
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) this._kind), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) this._name, typeof (string)));
  }
}
