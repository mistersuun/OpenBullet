// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.OperatorInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Actions;

internal class OperatorInfo
{
  private static Dictionary<ExpressionType, OperatorInfo> _infos = OperatorInfo.MakeOperatorTable();

  private OperatorInfo(ExpressionType op, string name, string altName)
  {
    this.Operator = op;
    this.Name = name;
    this.AlternateName = altName;
  }

  public static OperatorInfo GetOperatorInfo(ExpressionType op)
  {
    OperatorInfo operatorInfo;
    OperatorInfo._infos.TryGetValue(op, out operatorInfo);
    return operatorInfo;
  }

  public static OperatorInfo GetOperatorInfo(string name)
  {
    foreach (OperatorInfo operatorInfo in OperatorInfo._infos.Values)
    {
      if (operatorInfo.Name == name || operatorInfo.AlternateName == name)
        return operatorInfo;
    }
    return (OperatorInfo) null;
  }

  public ExpressionType Operator { get; }

  public string Name { get; }

  public string AlternateName { get; }

  private static Dictionary<ExpressionType, OperatorInfo> MakeOperatorTable()
  {
    return new Dictionary<ExpressionType, OperatorInfo>()
    {
      [ExpressionType.Decrement] = new OperatorInfo(ExpressionType.Decrement, "op_Decrement", "Decrement"),
      [ExpressionType.Increment] = new OperatorInfo(ExpressionType.Increment, "op_Increment", "Increment"),
      [ExpressionType.Negate] = new OperatorInfo(ExpressionType.Negate, "op_UnaryNegation", "Negate"),
      [ExpressionType.UnaryPlus] = new OperatorInfo(ExpressionType.UnaryPlus, "op_UnaryPlus", "Plus"),
      [ExpressionType.Not] = new OperatorInfo(ExpressionType.Not, "op_LogicalNot", (string) null),
      [ExpressionType.IsTrue] = new OperatorInfo(ExpressionType.IsTrue, "op_True", (string) null),
      [ExpressionType.IsFalse] = new OperatorInfo(ExpressionType.IsFalse, "op_False", (string) null),
      [ExpressionType.OnesComplement] = new OperatorInfo(ExpressionType.OnesComplement, "op_OnesComplement", "OnesComplement"),
      [ExpressionType.Add] = new OperatorInfo(ExpressionType.Add, "op_Addition", "Add"),
      [ExpressionType.Subtract] = new OperatorInfo(ExpressionType.Subtract, "op_Subtraction", "Subtract"),
      [ExpressionType.Multiply] = new OperatorInfo(ExpressionType.Multiply, "op_Multiply", "Multiply"),
      [ExpressionType.Divide] = new OperatorInfo(ExpressionType.Divide, "op_Division", "Divide"),
      [ExpressionType.Modulo] = new OperatorInfo(ExpressionType.Modulo, "op_Modulus", "Mod"),
      [ExpressionType.ExclusiveOr] = new OperatorInfo(ExpressionType.ExclusiveOr, "op_ExclusiveOr", "ExclusiveOr"),
      [ExpressionType.And] = new OperatorInfo(ExpressionType.And, "op_BitwiseAnd", "BitwiseAnd"),
      [ExpressionType.Or] = new OperatorInfo(ExpressionType.Or, "op_BitwiseOr", "BitwiseOr"),
      [ExpressionType.And] = new OperatorInfo(ExpressionType.And, "op_LogicalAnd", "And"),
      [ExpressionType.Or] = new OperatorInfo(ExpressionType.Or, "op_LogicalOr", "Or"),
      [ExpressionType.LeftShift] = new OperatorInfo(ExpressionType.LeftShift, "op_LeftShift", "LeftShift"),
      [ExpressionType.RightShift] = new OperatorInfo(ExpressionType.RightShift, "op_RightShift", "RightShift"),
      [ExpressionType.Equal] = new OperatorInfo(ExpressionType.Equal, "op_Equality", "Equals"),
      [ExpressionType.GreaterThan] = new OperatorInfo(ExpressionType.GreaterThan, "op_GreaterThan", "GreaterThan"),
      [ExpressionType.LessThan] = new OperatorInfo(ExpressionType.LessThan, "op_LessThan", "LessThan"),
      [ExpressionType.NotEqual] = new OperatorInfo(ExpressionType.NotEqual, "op_Inequality", "NotEquals"),
      [ExpressionType.GreaterThanOrEqual] = new OperatorInfo(ExpressionType.GreaterThanOrEqual, "op_GreaterThanOrEqual", "GreaterThanOrEqual"),
      [ExpressionType.LessThanOrEqual] = new OperatorInfo(ExpressionType.LessThanOrEqual, "op_LessThanOrEqual", "LessThanOrEqual"),
      [ExpressionType.MultiplyAssign] = new OperatorInfo(ExpressionType.MultiplyAssign, "op_MultiplicationAssignment", "InPlaceMultiply"),
      [ExpressionType.SubtractAssign] = new OperatorInfo(ExpressionType.SubtractAssign, "op_SubtractionAssignment", "InPlaceSubtract"),
      [ExpressionType.ExclusiveOrAssign] = new OperatorInfo(ExpressionType.ExclusiveOrAssign, "op_ExclusiveOrAssignment", "InPlaceExclusiveOr"),
      [ExpressionType.LeftShiftAssign] = new OperatorInfo(ExpressionType.LeftShiftAssign, "op_LeftShiftAssignment", "InPlaceLeftShift"),
      [ExpressionType.RightShiftAssign] = new OperatorInfo(ExpressionType.RightShiftAssign, "op_RightShiftAssignment", "InPlaceRightShift"),
      [ExpressionType.ModuloAssign] = new OperatorInfo(ExpressionType.ModuloAssign, "op_ModulusAssignment", "InPlaceMod"),
      [ExpressionType.AddAssign] = new OperatorInfo(ExpressionType.AddAssign, "op_AdditionAssignment", "InPlaceAdd"),
      [ExpressionType.AndAssign] = new OperatorInfo(ExpressionType.AndAssign, "op_BitwiseAndAssignment", "InPlaceBitwiseAnd"),
      [ExpressionType.OrAssign] = new OperatorInfo(ExpressionType.OrAssign, "op_BitwiseOrAssignment", "InPlaceBitwiseOr"),
      [ExpressionType.DivideAssign] = new OperatorInfo(ExpressionType.DivideAssign, "op_DivisionAssignment", "InPlaceDivide")
    };
  }
}
