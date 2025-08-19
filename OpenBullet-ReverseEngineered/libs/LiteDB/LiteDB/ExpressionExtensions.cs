// Decompiled with JetBrains decompiler
// Type: LiteDB.ExpressionExtensions
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Linq.Expressions;
using System.Text.RegularExpressions;

#nullable disable
namespace LiteDB;

internal static class ExpressionExtensions
{
  private static Regex _removeSelect = new Regex("\\.Select\\s*\\(\\s*\\w+\\s*=>\\s*\\w+\\.", RegexOptions.Compiled);
  private static Regex _removeList = new Regex("\\.get_Item\\(\\d+\\)", RegexOptions.Compiled);
  private static Regex _removeArray = new Regex("\\[\\d+\\]", RegexOptions.Compiled);

  public static string GetPath(this Expression expr)
  {
    while (expr.NodeType == ExpressionType.Convert || expr.NodeType == ExpressionType.ConvertChecked)
      expr = ((UnaryExpression) expr).Operand;
    while (expr.NodeType == ExpressionType.Lambda && ((LambdaExpression) expr).Body is UnaryExpression body)
      expr = body.Operand;
    string str1 = expr.ToString();
    int num = str1.IndexOf('.');
    string str2;
    if (num >= 0)
      str2 = str1.Substring(num + 1).TrimEnd(')');
    else
      str2 = str1;
    string input1 = str2;
    string input2 = ExpressionExtensions._removeList.Replace(input1, "");
    string input3 = ExpressionExtensions._removeArray.Replace(input2, "");
    return ExpressionExtensions._removeSelect.Replace(input3, ".").Replace(")", "");
  }
}
