// Decompiled with JetBrains decompiler
// Type: LiteDB.QueryVisitor`1
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#nullable disable
namespace LiteDB;

internal class QueryVisitor<T>
{
  private BsonMapper _mapper;
  private Type _type;
  private Dictionary<ParameterExpression, BsonValue> _parameters = new Dictionary<ParameterExpression, BsonValue>();
  private ParameterExpression _param;

  public QueryVisitor(BsonMapper mapper)
  {
    this._mapper = mapper;
    this._type = typeof (T);
  }

  public Query Visit(Expression<Func<T, bool>> predicate)
  {
    LambdaExpression lambdaExpression = (LambdaExpression) predicate;
    this._param = lambdaExpression.Parameters[0];
    return this.VisitExpression(lambdaExpression.Body);
  }

  private Query VisitExpression(Expression expr, string prefix = null)
  {
    try
    {
      if (expr is MemberExpression && expr.Type == typeof (bool))
        return Query.EQ(this.GetField(expr, prefix), new BsonValue(true));
      if (expr.NodeType == ExpressionType.Not)
        return Query.Not(this.VisitExpression((expr as UnaryExpression).Operand, prefix));
      if (expr.NodeType == ExpressionType.Equal)
      {
        BinaryExpression binaryExpression = expr as BinaryExpression;
        return (Query) new QueryEquals(this.GetField(binaryExpression.Left, prefix), this.VisitValue(binaryExpression.Right, binaryExpression.Left));
      }
      if (expr.NodeType == ExpressionType.NotEqual)
      {
        BinaryExpression binaryExpression = expr as BinaryExpression;
        return Query.Not(this.GetField(binaryExpression.Left, prefix), this.VisitValue(binaryExpression.Right, binaryExpression.Left));
      }
      if (expr.NodeType == ExpressionType.LessThan)
      {
        BinaryExpression binaryExpression = expr as BinaryExpression;
        return Query.LT(this.GetField(binaryExpression.Left, prefix), this.VisitValue(binaryExpression.Right, binaryExpression.Left));
      }
      if (expr.NodeType == ExpressionType.LessThanOrEqual)
      {
        BinaryExpression binaryExpression = expr as BinaryExpression;
        return Query.LTE(this.GetField(binaryExpression.Left, prefix), this.VisitValue(binaryExpression.Right, binaryExpression.Left));
      }
      if (expr.NodeType == ExpressionType.GreaterThan)
      {
        BinaryExpression binaryExpression = expr as BinaryExpression;
        return Query.GT(this.GetField(binaryExpression.Left, prefix), this.VisitValue(binaryExpression.Right, binaryExpression.Left));
      }
      if (expr.NodeType == ExpressionType.GreaterThanOrEqual)
      {
        BinaryExpression binaryExpression = expr as BinaryExpression;
        return Query.GTE(this.GetField(binaryExpression.Left, prefix), this.VisitValue(binaryExpression.Right, binaryExpression.Left));
      }
      if (expr.NodeType == ExpressionType.AndAlso)
      {
        BinaryExpression binaryExpression = expr as BinaryExpression;
        return Query.And(this.VisitExpression(binaryExpression.Left, prefix), this.VisitExpression(binaryExpression.Right, prefix));
      }
      if (expr.NodeType == ExpressionType.OrElse)
      {
        BinaryExpression binaryExpression = expr as BinaryExpression;
        return Query.Or(this.VisitExpression(binaryExpression.Left), this.VisitExpression(binaryExpression.Right));
      }
      if (expr.NodeType == ExpressionType.Constant)
      {
        ConstantExpression constantExpression = expr as ConstantExpression;
        if (constantExpression.Value is bool)
          return (bool) constantExpression.Value ? Query.All() : (Query) new QueryEmpty();
      }
      else
      {
        if (expr.NodeType == ExpressionType.Invoke)
          return this.VisitExpression(((expr as InvocationExpression).Expression as LambdaExpression).Body);
        if (expr is MethodCallExpression)
        {
          MethodCallExpression expr1 = expr as MethodCallExpression;
          string name = expr1.Method.Name;
          Type declaringType = expr1.Method.DeclaringType;
          ExpressionType? nullable1 = expr1.Arguments[0] is MemberExpression ? new ExpressionType?((expr1.Arguments[0] as MemberExpression).Expression.NodeType) : new ExpressionType?();
          switch (name)
          {
            case "StartsWith":
              BsonValue bsonValue1 = this.VisitValue(expr1.Arguments[0], (Expression) null);
              return Query.StartsWith(this.GetField(expr1.Object, prefix), (string) bsonValue1);
            case "Equals":
              BsonValue bsonValue2 = this.VisitValue(expr1.Arguments[0], (Expression) null);
              return Query.EQ(this.GetField(expr1.Object, prefix), bsonValue2);
            default:
              if (name == "Contains" && declaringType == typeof (string))
              {
                BsonValue bsonValue3 = this.VisitValue(expr1.Arguments[0], (Expression) null);
                return Query.Contains(this.GetField(expr1.Object, prefix), (string) bsonValue3);
              }
              if (name == "Contains" && declaringType == typeof (Enumerable))
                return Query.EQ(this.GetField(expr1.Arguments[0], prefix), this.VisitValue(expr1.Arguments[1], (Expression) null));
              if (name == "Any" && declaringType == typeof (Enumerable))
              {
                ExpressionType? nullable2 = nullable1;
                ExpressionType expressionType = ExpressionType.Parameter;
                if ((nullable2.GetValueOrDefault() == expressionType ? (nullable2.HasValue ? 1 : 0) : 0) != 0)
                {
                  string field = this.GetField(expr1.Arguments[0]);
                  return this.VisitExpression((expr1.Arguments[1] as LambdaExpression).Body, field + ".");
                }
              }
              if (declaringType == typeof (Enumerable))
                return this.ParseEnumerableExpression(expr1);
              break;
          }
        }
      }
      return (Query) new QueryLinq<T>(expr, this._param, this._mapper);
    }
    catch (NotSupportedException ex)
    {
      return (Query) new QueryLinq<T>(expr, this._param, this._mapper);
    }
  }

  private BsonValue VisitValue(Expression expr, Expression left)
  {
    Func<Type, object, BsonValue> func1 = (Func<Type, object, BsonValue>) ((type, value) =>
    {
      Type type1 = !(left is UnaryExpression) ? (Type) null : (left as UnaryExpression).Operand.Type;
      if (!(type1 != (Type) null) || !type1.GetTypeInfo().IsEnum)
        return this._mapper.Serialize(type, value, 0);
      return this._mapper.Serialize(typeof (string), (object) Enum.GetName(type1, value), 0);
    });
    BsonValue bsonValue1;
    switch (expr)
    {
      case ConstantExpression _:
        ConstantExpression constantExpression = expr as ConstantExpression;
        return func1(constantExpression.Type, constantExpression.Value);
      case MemberExpression _ when this._parameters.Count > 0:
        MemberExpression memberExpression = (MemberExpression) expr;
        BsonValue bsonValue2 = this.VisitValue(memberExpression.Expression, left).AsDocument[memberExpression.Member.Name];
        return func1(typeof (object), (object) bsonValue2);
      case ParameterExpression _ when this._parameters.TryGetValue((ParameterExpression) expr, out bsonValue1):
        return bsonValue1;
      default:
        Func<object> func2 = Expression.Lambda<Func<object>>((Expression) Expression.Convert(expr, typeof (object))).Compile();
        return func1(typeof (object), func2());
    }
  }

  private Query CreateAndQuery(ref Query[] queries, int startIndex = 0)
  {
    switch (queries.Length - startIndex)
    {
      case 0:
        return (Query) new QueryEmpty();
      case 1:
        return queries[startIndex];
      default:
        return Query.And(queries[startIndex], this.CreateOrQuery(ref queries, ++startIndex));
    }
  }

  private Query CreateOrQuery(ref Query[] queries, int startIndex = 0)
  {
    switch (queries.Length - startIndex)
    {
      case 0:
        return (Query) new QueryEmpty();
      case 1:
        return queries[startIndex];
      default:
        return Query.Or(queries[startIndex], this.CreateOrQuery(ref queries, ++startIndex));
    }
  }

  private Query ParseEnumerableExpression(MethodCallExpression expr)
  {
    if (expr.Method.DeclaringType != typeof (Enumerable))
      throw new NotSupportedException("Cannot parse methods outside the System.Linq.Enumerable class.");
    LambdaExpression lambdaExpression = (LambdaExpression) expr.Arguments[1];
    BsonArray asArray = this.VisitValue(expr.Arguments[0], (Expression) null).AsArray;
    Query[] queries = new Query[asArray.Count];
    for (int index = 0; index < queries.Length; ++index)
    {
      this._parameters[lambdaExpression.Parameters[0]] = asArray[index];
      queries[index] = this.VisitExpression(lambdaExpression.Body);
    }
    this._parameters.Remove(lambdaExpression.Parameters[0]);
    if (expr.Method.Name == "Any")
      return this.CreateOrQuery(ref queries);
    if (expr.Method.Name == "All")
      return this.CreateAndQuery(ref queries);
    throw new NotSupportedException("Not implemented System.Linq.Enumerable method");
  }

  public string GetField(Expression expr, string prefix = "", bool showArrayItems = false)
  {
    string str = prefix + expr.GetPath();
    string[] strArray1 = str.Split('.');
    string[] strArray2 = new string[strArray1.Length];
    Type type = this._type;
    bool flag = false;
    for (int index = 0; index < strArray1.Length; ++index)
    {
      EntityMapper entityMapper = this._mapper.GetEntityMapper(type);
      string part = strArray1[index];
      MemberMapper memberMapper = entityMapper.Members.Find((Predicate<MemberMapper>) (x => x.MemberName == part));
      type = memberMapper != null ? memberMapper.UnderlyingType : throw new NotSupportedException($"{str} not mapped in {type.Name}");
      strArray2[index] = memberMapper.FieldName;
      if (showArrayItems && memberMapper.IsList)
      {
        // ISSUE: explicit reference operation
        ^ref strArray2[index] += "[*]";
      }
      if (memberMapper.FieldName == "_id" & flag)
      {
        flag = false;
        strArray2[index] = "$id";
      }
      if (memberMapper.IsDbRef)
        flag = true;
    }
    return string.Join(".", strArray2);
  }

  public string GetPath(Expression expr) => "$." + this.GetField(expr, showArrayItems: true);
}
