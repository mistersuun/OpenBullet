// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.CallSignature
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Actions;

public struct CallSignature : IEquatable<CallSignature>
{
  private readonly Argument[] _infos;
  private readonly int _argumentCount;

  public bool IsSimple => this._infos == null;

  public int ArgumentCount => this._argumentCount;

  public CallSignature(CallSignature signature)
  {
    this._infos = signature.GetArgumentInfos();
    this._argumentCount = signature._argumentCount;
  }

  public CallSignature(int argumentCount)
  {
    ContractUtils.Requires(argumentCount >= 0, nameof (argumentCount));
    this._argumentCount = argumentCount;
    this._infos = (Argument[]) null;
  }

  public CallSignature(params Argument[] infos)
  {
    bool flag = true;
    if (infos != null)
    {
      this._argumentCount = infos.Length;
      for (int index = 0; index < infos.Length; ++index)
      {
        if (infos[index].Kind != ArgumentType.Simple)
        {
          flag = false;
          break;
        }
      }
    }
    else
      this._argumentCount = 0;
    this._infos = !flag ? infos : (Argument[]) null;
  }

  public CallSignature(params ArgumentType[] kinds)
  {
    bool flag = true;
    if (kinds != null)
    {
      this._argumentCount = kinds.Length;
      for (int index = 0; index < kinds.Length; ++index)
      {
        if (kinds[index] != ArgumentType.Simple)
        {
          flag = false;
          break;
        }
      }
    }
    else
      this._argumentCount = 0;
    if (!flag)
    {
      this._infos = new Argument[kinds.Length];
      for (int index = 0; index < kinds.Length; ++index)
        this._infos[index] = new Argument(kinds[index]);
    }
    else
      this._infos = (Argument[]) null;
  }

  public bool Equals(CallSignature other)
  {
    if (this._infos == null)
      return other._infos == null && other._argumentCount == this._argumentCount;
    if (other._infos == null || this._infos.Length != other._infos.Length)
      return false;
    for (int index = 0; index < this._infos.Length; ++index)
    {
      if (!this._infos[index].Equals(other._infos[index]))
        return false;
    }
    return true;
  }

  public override bool Equals(object obj) => obj is CallSignature other && this.Equals(other);

  public static bool operator ==(CallSignature left, CallSignature right) => left.Equals(right);

  public static bool operator !=(CallSignature left, CallSignature right) => !left.Equals(right);

  public override string ToString()
  {
    if (this._infos == null)
      return "Simple";
    StringBuilder stringBuilder = new StringBuilder("(");
    for (int index = 0; index < this._infos.Length; ++index)
    {
      if (index > 0)
        stringBuilder.Append(", ");
      stringBuilder.Append(this._infos[index].ToString());
    }
    stringBuilder.Append(")");
    return stringBuilder.ToString();
  }

  public override int GetHashCode()
  {
    int hashCode = 6551;
    if (this._infos != null)
    {
      foreach (Argument info in this._infos)
        hashCode ^= hashCode << 5 ^ info.GetHashCode();
    }
    return hashCode;
  }

  public Argument[] GetArgumentInfos()
  {
    return this._infos == null ? CompilerHelpers.MakeRepeatedArray<Argument>(Argument.Simple, this._argumentCount) : ArrayUtils.Copy<Argument>(this._infos);
  }

  public CallSignature InsertArgument(Argument info) => this.InsertArgumentAt(0, info);

  public CallSignature InsertArgumentAt(int index, Argument info)
  {
    if (this.IsSimple)
    {
      if (info.IsSimple)
        return new CallSignature(this._argumentCount + 1);
      return new CallSignature(ArrayUtils.InsertAt<Argument>(this.GetArgumentInfos(), index, info));
    }
    return new CallSignature(ArrayUtils.InsertAt<Argument>(this._infos, index, info));
  }

  public CallSignature RemoveFirstArgument() => this.RemoveArgumentAt(0);

  public CallSignature RemoveArgumentAt(int index)
  {
    if (this._argumentCount == 0)
      throw new InvalidOperationException();
    return this.IsSimple ? new CallSignature(this._argumentCount - 1) : new CallSignature(ArrayUtils.RemoveAt<Argument>(this._infos, index));
  }

  public int IndexOf(ArgumentType kind)
  {
    if (this._infos == null)
      return kind != ArgumentType.Simple || this._argumentCount <= 0 ? -1 : 0;
    for (int index = 0; index < this._infos.Length; ++index)
    {
      if (this._infos[index].Kind == kind)
        return index;
    }
    return -1;
  }

  public bool HasDictionaryArgument() => this.IndexOf(ArgumentType.Dictionary) > -1;

  public bool HasInstanceArgument() => this.IndexOf(ArgumentType.Instance) > -1;

  public bool HasListArgument() => this.IndexOf(ArgumentType.List) > -1;

  internal bool HasNamedArgument() => this.IndexOf(ArgumentType.Named) > -1;

  public bool HasKeywordArgument()
  {
    if (this._infos != null)
    {
      foreach (Argument info in this._infos)
      {
        if (info.Kind == ArgumentType.Dictionary || info.Kind == ArgumentType.Named)
          return true;
      }
    }
    return false;
  }

  public ArgumentType GetArgumentKind(int index)
  {
    Argument[] infos = this._infos;
    return infos == null ? ArgumentType.Simple : infos[index].Kind;
  }

  public string GetArgumentName(int index)
  {
    ContractUtils.Requires(index >= 0 && index < this._argumentCount);
    return this._infos?[index].Name;
  }

  public int GetProvidedPositionalArgumentCount()
  {
    int argumentCount = this._argumentCount;
    if (this._infos != null)
    {
      for (int index = 0; index < this._infos.Length; ++index)
      {
        switch (this._infos[index].Kind)
        {
          case ArgumentType.Named:
          case ArgumentType.List:
          case ArgumentType.Dictionary:
            --argumentCount;
            break;
        }
      }
    }
    return argumentCount;
  }

  public string[] GetArgumentNames()
  {
    if (this._infos == null)
      return ArrayUtils.EmptyStrings;
    List<string> stringList = new List<string>();
    foreach (Argument info in this._infos)
    {
      if (info.Name != null)
        stringList.Add(info.Name);
    }
    return stringList.ToArray();
  }

  public Expression CreateExpression()
  {
    if (this._infos == null)
      return (Expression) Expression.New(typeof (CallSignature).GetConstructor(new Type[1]
      {
        typeof (int)
      }), Microsoft.Scripting.Ast.Utils.Constant((object) this.ArgumentCount));
    Expression[] expressionArray = new Expression[this._infos.Length];
    for (int index = 0; index < expressionArray.Length; ++index)
      expressionArray[index] = this._infos[index].CreateExpression();
    return (Expression) Expression.New(typeof (CallSignature).GetConstructor(new Type[1]
    {
      typeof (Argument[])
    }), (Expression) Expression.NewArrayInit(typeof (Argument), expressionArray));
  }
}
