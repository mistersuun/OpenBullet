// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonPath.CompositeExpression
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Linq.JsonPath;

internal class CompositeExpression : QueryExpression
{
  public List<QueryExpression> Expressions { get; set; }

  public CompositeExpression() => this.Expressions = new List<QueryExpression>();

  public override bool IsMatch(JToken root, JToken t)
  {
    switch (this.Operator)
    {
      case QueryOperator.And:
        foreach (QueryExpression expression in this.Expressions)
        {
          if (!expression.IsMatch(root, t))
            return false;
        }
        return true;
      case QueryOperator.Or:
        foreach (QueryExpression expression in this.Expressions)
        {
          if (expression.IsMatch(root, t))
            return true;
        }
        return false;
      default:
        throw new ArgumentOutOfRangeException();
    }
  }
}
