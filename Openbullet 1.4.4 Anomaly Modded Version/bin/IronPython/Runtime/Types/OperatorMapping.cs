// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.OperatorMapping
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using System;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime.Types;

internal class OperatorMapping
{
  private static readonly OperatorMapping[] _infos = OperatorMapping.MakeOperatorTable();
  private readonly PythonOperationKind _operator;
  private readonly string _name;
  private readonly string _altName;
  private readonly Type _altExpectedType;

  private OperatorMapping(PythonOperationKind op, string name, string altName)
  {
    this._operator = op;
    this._name = name;
    this._altName = altName;
  }

  private OperatorMapping(
    PythonOperationKind op,
    string name,
    string altName,
    Type alternateExpectedType)
  {
    this._operator = op;
    this._name = name;
    this._altName = altName;
    this._altExpectedType = alternateExpectedType;
  }

  public static OperatorMapping GetOperatorMapping(PythonOperationKind op)
  {
    foreach (OperatorMapping info in OperatorMapping._infos)
    {
      if (info._operator == op)
        return info;
    }
    return (OperatorMapping) null;
  }

  public static OperatorMapping GetOperatorMapping(string name)
  {
    foreach (OperatorMapping info in OperatorMapping._infos)
    {
      if (info.Name == name || info.AlternateName == name)
        return info;
    }
    return (OperatorMapping) null;
  }

  public PythonOperationKind Operator => this._operator;

  public string Name => this._name;

  public string AlternateName => this._altName;

  public Type AlternateExpectedType => this._altExpectedType;

  private static OperatorMapping[] MakeOperatorTable()
  {
    return new List<OperatorMapping>()
    {
      new OperatorMapping(PythonOperationKind.Negate, "op_UnaryNegation", "Negate"),
      new OperatorMapping(PythonOperationKind.Positive, "op_UnaryPlus", "Plus"),
      new OperatorMapping(PythonOperationKind.Not, "op_LogicalNot", (string) null),
      new OperatorMapping(PythonOperationKind.OnesComplement, "op_OnesComplement", "OnesComplement"),
      new OperatorMapping(PythonOperationKind.Add, "op_Addition", "Add"),
      new OperatorMapping(PythonOperationKind.Subtract, "op_Subtraction", "Subtract"),
      new OperatorMapping(PythonOperationKind.Multiply, "op_Multiply", "Multiply"),
      new OperatorMapping(PythonOperationKind.Divide, "op_Division", "Divide"),
      new OperatorMapping(PythonOperationKind.Mod, "op_Modulus", "Mod"),
      new OperatorMapping(PythonOperationKind.ExclusiveOr, "op_ExclusiveOr", "ExclusiveOr"),
      new OperatorMapping(PythonOperationKind.BitwiseAnd, "op_BitwiseAnd", "BitwiseAnd"),
      new OperatorMapping(PythonOperationKind.BitwiseOr, "op_BitwiseOr", "BitwiseOr"),
      new OperatorMapping(PythonOperationKind.LeftShift, "op_LeftShift", "LeftShift"),
      new OperatorMapping(PythonOperationKind.RightShift, "op_RightShift", "RightShift"),
      new OperatorMapping(PythonOperationKind.Equal, "op_Equality", "Equals"),
      new OperatorMapping(PythonOperationKind.GreaterThan, "op_GreaterThan", "GreaterThan"),
      new OperatorMapping(PythonOperationKind.LessThan, "op_LessThan", "LessThan"),
      new OperatorMapping(PythonOperationKind.NotEqual, "op_Inequality", "NotEquals"),
      new OperatorMapping(PythonOperationKind.GreaterThanOrEqual, "op_GreaterThanOrEqual", "GreaterThanOrEqual"),
      new OperatorMapping(PythonOperationKind.LessThanOrEqual, "op_LessThanOrEqual", "LessThanOrEqual"),
      new OperatorMapping(PythonOperationKind.InPlaceMultiply, "op_MultiplicationAssignment", "InPlaceMultiply"),
      new OperatorMapping(PythonOperationKind.InPlaceSubtract, "op_SubtractionAssignment", "InPlaceSubtract"),
      new OperatorMapping(PythonOperationKind.InPlaceExclusiveOr, "op_ExclusiveOrAssignment", "InPlaceExclusiveOr"),
      new OperatorMapping(PythonOperationKind.InPlaceLeftShift, "op_LeftShiftAssignment", "InPlaceLeftShift"),
      new OperatorMapping(PythonOperationKind.InPlaceRightShift, "op_RightShiftAssignment", "InPlaceRightShift"),
      new OperatorMapping(PythonOperationKind.InPlaceMod, "op_ModulusAssignment", "InPlaceMod"),
      new OperatorMapping(PythonOperationKind.InPlaceAdd, "op_AdditionAssignment", "InPlaceAdd"),
      new OperatorMapping(PythonOperationKind.InPlaceBitwiseAnd, "op_BitwiseAndAssignment", "InPlaceBitwiseAnd"),
      new OperatorMapping(PythonOperationKind.InPlaceBitwiseOr, "op_BitwiseOrAssignment", "InPlaceBitwiseOr"),
      new OperatorMapping(PythonOperationKind.InPlaceDivide, "op_DivisionAssignment", "InPlaceDivide"),
      new OperatorMapping(PythonOperationKind.GetItem, "get_Item", "GetItem"),
      new OperatorMapping(PythonOperationKind.SetItem, "set_Item", "SetItem"),
      new OperatorMapping(PythonOperationKind.DeleteItem, "del_Item", "DeleteItem"),
      new OperatorMapping(PythonOperationKind.Compare, "op_Compare", "Compare", typeof (int)),
      new OperatorMapping(PythonOperationKind.CallSignatures, "GetCallSignatures", (string) null),
      new OperatorMapping(PythonOperationKind.Documentation, "GetDocumentation", (string) null),
      new OperatorMapping(PythonOperationKind.IsCallable, "IsCallable", (string) null)
    }.ToArray();
  }
}
