// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonPath.CompositeExpression
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

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
