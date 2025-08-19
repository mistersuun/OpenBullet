// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Symbols
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using System;

#nullable disable
namespace IronPython.Runtime;

public static class Symbols
{
  internal static string OperatorToSymbol(PythonOperationKind op)
  {
    switch (op)
    {
      case PythonOperationKind.Compare:
        return "__cmp__";
      case PythonOperationKind.DivMod:
        return "__divmod__";
      case PythonOperationKind.AbsoluteValue:
        return "__abs__";
      case PythonOperationKind.Positive:
        return "__pos__";
      case PythonOperationKind.Negate:
        return "__neg__";
      case PythonOperationKind.OnesComplement:
        return "__invert__";
      case PythonOperationKind.Add:
        return "__add__";
      case PythonOperationKind.Subtract:
        return "__sub__";
      case PythonOperationKind.Power:
        return "__pow__";
      case PythonOperationKind.Multiply:
        return "__mul__";
      case PythonOperationKind.FloorDivide:
        return "__floordiv__";
      case PythonOperationKind.Divide:
        return "__div__";
      case PythonOperationKind.TrueDivide:
        return "__truediv__";
      case PythonOperationKind.Mod:
        return "__mod__";
      case PythonOperationKind.LeftShift:
        return "__lshift__";
      case PythonOperationKind.RightShift:
        return "__rshift__";
      case PythonOperationKind.BitwiseAnd:
        return "__and__";
      case PythonOperationKind.BitwiseOr:
        return "__or__";
      case PythonOperationKind.ExclusiveOr:
        return "__xor__";
      case PythonOperationKind.LessThan:
        return "__lt__";
      case PythonOperationKind.GreaterThan:
        return "__gt__";
      case PythonOperationKind.LessThanOrEqual:
        return "__le__";
      case PythonOperationKind.GreaterThanOrEqual:
        return "__ge__";
      case PythonOperationKind.Equal:
        return "__eq__";
      case PythonOperationKind.NotEqual:
        return "__ne__";
      case PythonOperationKind.LessThanGreaterThan:
        return "__lg__";
      case PythonOperationKind.ReverseDivMod:
        return "__rdivmod__";
      case PythonOperationKind.ReverseAdd:
        return "__radd__";
      case PythonOperationKind.ReverseSubtract:
        return "__rsub__";
      case PythonOperationKind.ReversePower:
        return "__rpow__";
      case PythonOperationKind.ReverseMultiply:
        return "__rmul__";
      case PythonOperationKind.ReverseFloorDivide:
        return "__rfloordiv__";
      case PythonOperationKind.ReverseDivide:
        return "__rdiv__";
      case PythonOperationKind.ReverseTrueDivide:
        return "__rtruediv__";
      case PythonOperationKind.ReverseMod:
        return "__rmod__";
      case PythonOperationKind.ReverseLeftShift:
        return "__rlshift__";
      case PythonOperationKind.ReverseRightShift:
        return "__rrshift__";
      case PythonOperationKind.ReverseBitwiseAnd:
        return "__rand__";
      case PythonOperationKind.ReverseBitwiseOr:
        return "__ror__";
      case PythonOperationKind.ReverseExclusiveOr:
        return "__rxor__";
      case PythonOperationKind.InPlaceAdd:
        return "__iadd__";
      case PythonOperationKind.InPlaceSubtract:
        return "__isub__";
      case PythonOperationKind.InPlacePower:
        return "__ipow__";
      case PythonOperationKind.InPlaceMultiply:
        return "__imul__";
      case PythonOperationKind.InPlaceFloorDivide:
        return "__ifloordiv__";
      case PythonOperationKind.InPlaceDivide:
        return "__idiv__";
      case PythonOperationKind.InPlaceTrueDivide:
        return "__itruediv__";
      case PythonOperationKind.InPlaceMod:
        return "__imod__";
      case PythonOperationKind.InPlaceLeftShift:
        return "__ilshift__";
      case PythonOperationKind.InPlaceRightShift:
        return "__irshift__";
      case PythonOperationKind.InPlaceBitwiseAnd:
        return "__iand__";
      case PythonOperationKind.InPlaceBitwiseOr:
        return "__ior__";
      case PythonOperationKind.InPlaceExclusiveOr:
        return "__ixor__";
      default:
        throw new InvalidOperationException(op.ToString());
    }
  }

  internal static string OperatorToReversedSymbol(PythonOperationKind op)
  {
    switch (op)
    {
      case PythonOperationKind.LessThan:
        return "__gt__";
      case PythonOperationKind.GreaterThan:
        return "__lt__";
      case PythonOperationKind.LessThanOrEqual:
        return "__ge__";
      case PythonOperationKind.GreaterThanOrEqual:
        return "__le__";
      case PythonOperationKind.Equal:
        return "__eq__";
      case PythonOperationKind.NotEqual:
        return "__ne__";
      default:
        return (op & PythonOperationKind.Reversed) != PythonOperationKind.None ? Symbols.OperatorToSymbol(op & ~PythonOperationKind.Reversed) : Symbols.OperatorToSymbol(op | PythonOperationKind.Reversed);
    }
  }

  internal static PythonOperationKind OperatorToReverseOperator(PythonOperationKind op)
  {
    switch (op)
    {
      case PythonOperationKind.DivMod:
        return PythonOperationKind.ReverseDivMod;
      case PythonOperationKind.LessThan:
        return PythonOperationKind.GreaterThan;
      case PythonOperationKind.GreaterThan:
        return PythonOperationKind.LessThan;
      case PythonOperationKind.LessThanOrEqual:
        return PythonOperationKind.GreaterThanOrEqual;
      case PythonOperationKind.GreaterThanOrEqual:
        return PythonOperationKind.LessThanOrEqual;
      case PythonOperationKind.Equal:
        return PythonOperationKind.Equal;
      case PythonOperationKind.NotEqual:
        return PythonOperationKind.NotEqual;
      default:
        return (op & PythonOperationKind.Reversed) != PythonOperationKind.None ? op & ~PythonOperationKind.Reversed : op | PythonOperationKind.Reversed;
    }
  }
}
