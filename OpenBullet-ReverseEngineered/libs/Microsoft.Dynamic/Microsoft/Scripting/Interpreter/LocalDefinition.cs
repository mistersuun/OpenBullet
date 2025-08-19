// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.LocalDefinition
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

public struct LocalDefinition
{
  internal LocalDefinition(int localIndex, ParameterExpression parameter)
  {
    this.Index = localIndex;
    this.Parameter = parameter;
  }

  public int Index { get; }

  public ParameterExpression Parameter { get; }

  public override bool Equals(object obj)
  {
    return obj is LocalDefinition localDefinition && localDefinition.Index == this.Index && localDefinition.Parameter == this.Parameter;
  }

  public override int GetHashCode()
  {
    return this.Parameter == null ? 0 : this.Parameter.GetHashCode() ^ this.Index.GetHashCode();
  }

  public static bool operator ==(LocalDefinition self, LocalDefinition other)
  {
    return self.Index == other.Index && self.Parameter == other.Parameter;
  }

  public static bool operator !=(LocalDefinition self, LocalDefinition other)
  {
    return self.Index != other.Index || self.Parameter != other.Parameter;
  }
}
