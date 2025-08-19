// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Generation.ConstantCheck
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Generation;

public static class ConstantCheck
{
  public static bool Check(Expression expression, object value)
  {
    ContractUtils.RequiresNotNull((object) expression, nameof (expression));
    return ConstantCheck.IsConstant(expression, value);
  }

  internal static bool IsConstant(Expression e, object value)
  {
    switch (e.NodeType)
    {
      case ExpressionType.AndAlso:
        return ConstantCheck.CheckAndAlso((BinaryExpression) e, value);
      case ExpressionType.Constant:
        return ConstantCheck.CheckConstant((ConstantExpression) e, value);
      case ExpressionType.OrElse:
        return ConstantCheck.CheckOrElse((BinaryExpression) e, value);
      case ExpressionType.TypeIs:
        return ConstantCheck.Check((TypeBinaryExpression) e, value);
      default:
        return false;
    }
  }

  internal static bool IsNull(Expression e) => ConstantCheck.IsConstant(e, (object) null);

  private static bool CheckAndAlso(BinaryExpression node, object value)
  {
    object obj;
    if (node.Method != (MethodInfo) null || node.Conversion != null || !((obj = value) is bool))
      return false;
    if (!(bool) obj)
      return ConstantCheck.IsConstant(node.Left, (object) false);
    return ConstantCheck.IsConstant(node.Left, (object) true) && ConstantCheck.IsConstant(node.Right, (object) true);
  }

  private static bool CheckOrElse(BinaryExpression node, object value)
  {
    object obj;
    if (node.Method != (MethodInfo) null || !((obj = value) is bool))
      return false;
    if ((bool) obj)
      return ConstantCheck.IsConstant(node.Left, (object) true);
    return ConstantCheck.IsConstant(node.Left, (object) false) && ConstantCheck.IsConstant(node.Right, (object) false);
  }

  private static bool CheckConstant(ConstantExpression node, object value)
  {
    return value == null ? node.Value == null : value.Equals(node.Value);
  }

  private static bool Check(TypeBinaryExpression node, object value)
  {
    object obj;
    bool flag;
    int num1;
    if ((obj = value) is bool)
    {
      flag = (bool) obj;
      num1 = 1;
    }
    else
      num1 = 0;
    int num2 = flag ? 1 : 0;
    return (num1 & num2) != 0 && node.TypeOperand.IsAssignableFrom(node.Expression.Type);
  }
}
