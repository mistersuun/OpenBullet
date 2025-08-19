// Decompiled with JetBrains decompiler
// Type: LiteDB.BsonExpression
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

#nullable disable
namespace LiteDB;

public class BsonExpression
{
  private static Dictionary<string, MethodInfo> _operators = new Dictionary<string, MethodInfo>()
  {
    ["%"] = typeof (ExpressionOperators).GetMethod("MOD"),
    ["/"] = typeof (ExpressionOperators).GetMethod("DIVIDE"),
    ["*"] = typeof (ExpressionOperators).GetMethod("MULTIPLY"),
    ["+"] = typeof (ExpressionOperators).GetMethod("ADD"),
    ["-"] = typeof (ExpressionOperators).GetMethod("MINUS"),
    [">"] = typeof (ExpressionOperators).GetMethod("GT"),
    [">="] = typeof (ExpressionOperators).GetMethod("GTE"),
    ["<"] = typeof (ExpressionOperators).GetMethod("LT"),
    ["<="] = typeof (ExpressionOperators).GetMethod("LTE"),
    ["="] = typeof (ExpressionOperators).GetMethod("EQ"),
    ["!="] = typeof (ExpressionOperators).GetMethod("NEQ"),
    ["&&"] = typeof (ExpressionOperators).GetMethod("AND"),
    ["||"] = typeof (ExpressionOperators).GetMethod("OR")
  };
  private static MethodInfo[] _methods = typeof (BsonExpression).GetMethods(BindingFlags.Static | BindingFlags.Public);
  private Func<BsonDocument, BsonValue, IEnumerable<BsonValue>> _func;
  private static Dictionary<string, Func<BsonDocument, BsonValue, IEnumerable<BsonValue>>> _cache = new Dictionary<string, Func<BsonDocument, BsonValue, IEnumerable<BsonValue>>>();
  private static Regex _reArithmetic = new Regex("^\\s*(\\+|\\-|\\*|\\/|%)\\s*");
  private static Regex _reOperator = new Regex("^\\s*(\\+|\\-|\\*|\\/|%|=|!=|>=|>|<=|<|&&|\\|\\|)\\s*");

  public string Source { get; private set; }

  public BsonExpression(string expression)
  {
    if (expression == null)
      return;
    this.Source = expression;
    this._func = BsonExpression.Compile(expression);
  }

  internal BsonExpression(StringScanner s, bool pathOnly, bool arithmeticOnly)
  {
    int index = s.Index;
    this._func = BsonExpression.Compile(s, pathOnly, arithmeticOnly);
    this.Source = s.Source.Substring(index, s.Index - index);
    if (BsonExpression._cache.ContainsKey(this.Source))
      return;
    lock (BsonExpression._cache)
      BsonExpression._cache[this.Source] = this._func;
  }

  public IEnumerable<BsonValue> Execute(BsonDocument doc, bool includeNullIfEmpty = true)
  {
    return this.Execute(doc, (BsonValue) doc, includeNullIfEmpty);
  }

  public IEnumerable<BsonValue> Execute(
    BsonDocument root,
    BsonValue current,
    bool includeNullIfEmpty = true)
  {
    if (this.Source == null)
      throw new ArgumentNullException("ctor(expression)");
    int index = 0;
    Func<BsonDocument, BsonValue, IEnumerable<BsonValue>> func = this._func;
    BsonDocument bsonDocument = root;
    BsonValue bsonValue1 = current;
    if ((object) bsonValue1 == null)
      bsonValue1 = (BsonValue) root;
    foreach (BsonValue bsonValue2 in func(bsonDocument, bsonValue1))
    {
      ++index;
      yield return bsonValue2;
    }
    if (index == 0 & includeNullIfEmpty)
      yield return BsonValue.Null;
  }

  public static Func<BsonDocument, BsonValue, IEnumerable<BsonValue>> Compile(string expression)
  {
    Func<BsonDocument, BsonValue, IEnumerable<BsonValue>> func1;
    if (BsonExpression._cache.TryGetValue(expression, out func1))
      return func1;
    lock (BsonExpression._cache)
    {
      if (BsonExpression._cache.TryGetValue(expression, out func1))
        return func1;
      Func<BsonDocument, BsonValue, IEnumerable<BsonValue>> func2 = BsonExpression.Compile(new StringScanner(expression), false, true);
      BsonExpression._cache[expression] = func2;
      return func2;
    }
  }

  private static Func<BsonDocument, BsonValue, IEnumerable<BsonValue>> Compile(
    StringScanner s,
    bool pathOnly,
    bool arithmeticOnly)
  {
    ParameterExpression root = Expression.Parameter(typeof (BsonDocument), "root");
    ParameterExpression current = Expression.Parameter(typeof (BsonValue), "current");
    Expression body;
    if (pathOnly)
    {
      s.Scan("\\$\\.?");
      body = BsonExpression.ParseSingleExpression(s, root, current, true);
    }
    else
      body = BsonExpression.ParseExpression(s, root, current, arithmeticOnly);
    return Expression.Lambda<Func<BsonDocument, BsonValue, IEnumerable<BsonValue>>>(body, root, current).Compile();
  }

  internal static Expression ParseExpression(
    StringScanner s,
    ParameterExpression root,
    ParameterExpression current,
    bool arithmeticOnly)
  {
    List<Expression> source = new List<Expression>()
    {
      BsonExpression.ParseSingleExpression(s, root, current, false)
    };
    List<string> stringList = new List<string>();
    while (!s.HasTerminated)
    {
      string str = s.Scan(arithmeticOnly ? BsonExpression._reArithmetic : BsonExpression._reOperator, 1);
      if (str.Length != 0)
      {
        Expression singleExpression = BsonExpression.ParseSingleExpression(s, root, current, false);
        source.Add(singleExpression);
        stringList.Add(str);
      }
      else
        break;
    }
    int index1 = 0;
    while (source.Count >= 2)
    {
      KeyValuePair<string, MethodInfo> keyValuePair = BsonExpression._operators.ElementAt<KeyValuePair<string, MethodInfo>>(index1);
      int index2 = stringList.IndexOf(keyValuePair.Key);
      if (index2 == -1)
      {
        ++index1;
      }
      else
      {
        Expression expression1 = source.ElementAt<Expression>(index2);
        Expression expression2 = source.ElementAt<Expression>(index2 + 1);
        MethodCallExpression methodCallExpression = Expression.Call(keyValuePair.Value, expression1, expression2);
        source.Insert(index2, (Expression) methodCallExpression);
        source.RemoveRange(index2 + 1, 2);
        stringList.RemoveAt(index2);
      }
    }
    return source.Single<Expression>();
  }

  internal static Expression ParseSingleExpression(
    StringScanner s,
    ParameterExpression root,
    ParameterExpression current,
    bool isRoot)
  {
    if (s.Match("[\\$@]") | isRoot)
    {
      string str = s.Scan("[\\$@]");
      MethodInfo method = typeof (BsonExpression).GetMethod("Root");
      ConstantExpression constantExpression1 = Expression.Constant((object) s.Scan("\\.?([\\$\\-\\w]+)", 1));
      ParameterExpression parameterExpression = str == "@" ? current : root;
      ConstantExpression constantExpression2 = constantExpression1;
      Expression expr = (Expression) Expression.Call(method, (Expression) parameterExpression, (Expression) constantExpression2);
      while (!s.HasTerminated)
      {
        Expression path = BsonExpression.ParsePath(s, expr, root);
        if (path != null)
          expr = path;
        else
          break;
      }
      return expr;
    }
    if (s.Match("-?\\d*\\.\\d+"))
      return (Expression) Expression.NewArrayInit(typeof (BsonValue), (Expression) Expression.Constant((object) new BsonValue(Convert.ToDouble(s.Scan("-?\\d*\\.\\d+"), (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat))));
    if (s.Match("-?\\d+"))
      return (Expression) Expression.NewArrayInit(typeof (BsonValue), (Expression) Expression.Constant((object) new BsonValue(Convert.ToInt32(s.Scan("-?\\d+"), (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat))));
    if (s.Match("(true|false)"))
      return (Expression) Expression.NewArrayInit(typeof (BsonValue), (Expression) Expression.Constant((object) new BsonValue(Convert.ToBoolean(s.Scan("(true|false)")))));
    if (s.Match("null"))
      return (Expression) Expression.NewArrayInit(typeof (BsonValue), (Expression) Expression.Constant((object) BsonValue.Null));
    if (s.Match("'"))
      return (Expression) Expression.NewArrayInit(typeof (BsonValue), (Expression) Expression.Constant((object) new BsonValue(s.Scan("'([\\s\\S]*?)'", 1))));
    if (s.Scan("\\{\\s*").Length > 0)
    {
      MethodInfo method = typeof (ExpressionOperators).GetMethod("DOCUMENT");
      List<Expression> expressionList1 = new List<Expression>();
      List<Expression> expressionList2 = new List<Expression>();
      while (!s.HasTerminated)
      {
        string str = s.Scan("(.+?)\\s*:\\s*", 1).ThrowIfEmpty("Invalid token", s);
        Expression expression = BsonExpression.ParseExpression(s, root, current, false);
        expressionList1.Add((Expression) Expression.Constant((object) new BsonValue(str)));
        expressionList2.Add(expression);
        if (s.Scan("\\s*,\\s*").Length <= 0)
        {
          if (s.Scan("\\s*\\}\\s*").Length <= 0)
            throw LiteException.SyntaxError(s);
          break;
        }
      }
      NewArrayExpression newArrayExpression1 = Expression.NewArrayInit(typeof (BsonValue), expressionList1.ToArray());
      NewArrayExpression newArrayExpression2 = Expression.NewArrayInit(typeof (IEnumerable<BsonValue>), expressionList2.ToArray());
      return (Expression) Expression.Call(method, new Expression[2]
      {
        (Expression) newArrayExpression1,
        (Expression) newArrayExpression2
      });
    }
    if (s.Scan("\\[\\s*").Length > 0)
    {
      MethodInfo method = typeof (ExpressionOperators).GetMethod("ARRAY");
      List<Expression> expressionList = new List<Expression>();
      while (!s.HasTerminated)
      {
        Expression expression = BsonExpression.ParseExpression(s, root, current, false);
        expressionList.Add(expression);
        if (s.Scan("\\s*,\\s*").Length <= 0)
        {
          if (s.Scan("\\s*\\]\\s*").Length <= 0)
            throw LiteException.SyntaxError(s);
          break;
        }
      }
      NewArrayExpression newArrayExpression = Expression.NewArrayInit(typeof (IEnumerable<BsonValue>), expressionList.ToArray());
      return (Expression) Expression.Call(method, new Expression[1]
      {
        (Expression) newArrayExpression
      });
    }
    if (s.Scan("\\(\\s*").Length > 0)
    {
      Expression expression = BsonExpression.ParseExpression(s, root, current, false);
      if (s.Scan("\\s*\\)").Length != 0)
        return expression;
      throw LiteException.SyntaxError(s);
    }
    string name = s.Match("\\w+\\s*\\(") ? s.Scan("(\\w+)\\s*\\(", 1).ToUpper() : throw LiteException.SyntaxError(s);
    List<Expression> parameters = new List<Expression>();
    if (s.Scan("\\s*\\)\\s*").Length == 0)
    {
      while (!s.HasTerminated)
      {
        Expression expression = BsonExpression.ParseExpression(s, root, current, false);
        parameters.Add(expression);
        if (s.Scan("\\s*,\\s*").Length <= 0)
        {
          if (s.Scan("\\s*\\)\\s*").Length <= 0)
            throw LiteException.SyntaxError(s);
          break;
        }
      }
    }
    MethodInfo method1 = ((IEnumerable<MethodInfo>) BsonExpression._methods).FirstOrDefault<MethodInfo>((Func<MethodInfo, bool>) (x => x.Name == name && ((IEnumerable<ParameterInfo>) x.GetParameters()).Count<ParameterInfo>() == parameters.Count));
    return !(method1 == (MethodInfo) null) ? (Expression) Expression.Call(method1, parameters.ToArray()) : throw LiteException.SyntaxError(s, $"Method {name} not exist or invalid parameter count");
  }

  private static Expression ParsePath(StringScanner s, Expression expr, ParameterExpression root)
  {
    if (s.Match("\\.[\\$\\-\\w]+"))
    {
      MethodInfo method = typeof (BsonExpression).GetMethod("Member");
      ConstantExpression constantExpression1 = Expression.Constant((object) s.Scan("\\.([\\$\\-\\w]+)", 1));
      Expression expression = expr;
      ConstantExpression constantExpression2 = constantExpression1;
      return (Expression) Expression.Call(method, expression, (Expression) constantExpression2);
    }
    if (!s.Match("\\["))
      return (Expression) null;
    MethodInfo method1 = typeof (BsonExpression).GetMethod("Array");
    string str = s.Scan("\\[\\s*(-?[\\d+\\*])\\s*\\]", 1);
    int num = !(str != "*") || !(str != "") ? int.MaxValue : Convert.ToInt32(str);
    BsonExpression bsonExpression = new BsonExpression((string) null);
    if (str == "")
    {
      s.Scan("\\[\\s*");
      bsonExpression = BsonExpression.ReadExpression(s, true, false, false);
      if (bsonExpression == null)
        throw LiteException.SyntaxError(s, "Invalid expression formula");
      s.Scan("\\s*\\]");
    }
    Expression expression1 = expr;
    ConstantExpression constantExpression3 = Expression.Constant((object) num);
    ConstantExpression constantExpression4 = Expression.Constant((object) bsonExpression);
    ParameterExpression parameterExpression = root;
    return (Expression) Expression.Call(method1, expression1, (Expression) constantExpression3, (Expression) constantExpression4, (Expression) parameterExpression);
  }

  internal static BsonExpression ReadExpression(
    StringScanner s,
    bool required,
    bool pathOnly,
    bool arithmeticOnly = true)
  {
    int index = s.Index;
    try
    {
      return new BsonExpression(s, pathOnly, arithmeticOnly);
    }
    catch (LiteException ex) when (!required && ex.ErrorCode == (int) sbyte.MaxValue)
    {
      s.Index = index;
      return (BsonExpression) null;
    }
  }

  public static IEnumerable<BsonValue> Root(BsonValue value, string name)
  {
    if (string.IsNullOrEmpty(name))
    {
      yield return value;
    }
    else
    {
      BsonValue bsonValue;
      if (value.IsDocument && value.AsDocument.TryGetValue(name, out bsonValue))
      {
        bsonValue.Destroy = (Action) (() => value.AsDocument.Remove(name));
        yield return bsonValue;
      }
    }
  }

  public static IEnumerable<BsonValue> Member(IEnumerable<BsonValue> values, string name)
  {
    foreach (BsonDocument bsonDocument in values.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsDocument)).Select<BsonValue, BsonDocument>((Func<BsonValue, BsonDocument>) (x => x.AsDocument)))
    {
      BsonDocument doc = bsonDocument;
      BsonValue bsonValue;
      if (doc.TryGetValue(name, out bsonValue))
      {
        bsonValue.Destroy = (Action) (() => doc.Remove(name));
        yield return bsonValue;
      }
    }
  }

  public static IEnumerable<BsonValue> Array(
    IEnumerable<BsonValue> values,
    int index,
    BsonExpression expr,
    BsonDocument root)
  {
    foreach (BsonValue bsonValue1 in values)
    {
      if (bsonValue1.IsArray)
      {
        BsonArray arr = bsonValue1.AsArray;
        if (expr.Source != null)
        {
          foreach (BsonValue bsonValue2 in arr)
          {
            BsonValue item = bsonValue2;
            BsonValue bsonValue3 = expr.Execute(root, item).First<BsonValue>();
            if (bsonValue3.IsBoolean && bsonValue3.AsBoolean)
            {
              item.Destroy = (Action) (() => arr.Remove(item));
              yield return item;
            }
          }
        }
        else if (index == int.MaxValue)
        {
          foreach (BsonValue bsonValue4 in arr)
          {
            BsonValue item = bsonValue4;
            item.Destroy = (Action) (() => arr.Remove(item));
            yield return item;
          }
        }
        else
        {
          int index1 = index < 0 ? arr.Count + index : index;
          if (arr.Count > index1)
          {
            BsonValue item = arr[index1];
            item.Destroy = (Action) (() => arr.Remove(item));
            yield return item;
          }
        }
      }
    }
  }

  public override string ToString() => this.Source;

  public static IEnumerable<BsonValue> COUNT(IEnumerable<BsonValue> values)
  {
    yield return (BsonValue) values.Count<BsonValue>();
  }

  public static IEnumerable<BsonValue> MIN(IEnumerable<BsonValue> values)
  {
    BsonValue bsonValue1 = BsonValue.MaxValue;
    foreach (BsonValue bsonValue2 in values.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsNumber)))
      bsonValue1 = bsonValue2 < bsonValue1 ? bsonValue2 : bsonValue1;
    yield return bsonValue1 == BsonValue.MaxValue ? BsonValue.MinValue : bsonValue1;
  }

  public static IEnumerable<BsonValue> MAX(IEnumerable<BsonValue> values)
  {
    BsonValue bsonValue1 = BsonValue.MinValue;
    foreach (BsonValue bsonValue2 in values.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsNumber)))
      bsonValue1 = bsonValue2 > bsonValue1 ? bsonValue2 : bsonValue1;
    yield return bsonValue1 == BsonValue.MinValue ? BsonValue.MaxValue : bsonValue1;
  }

  public static IEnumerable<BsonValue> FIRST(IEnumerable<BsonValue> values)
  {
    yield return values.FirstOrDefault<BsonValue>();
  }

  public static IEnumerable<BsonValue> LAST(IEnumerable<BsonValue> values)
  {
    yield return values.LastOrDefault<BsonValue>();
  }

  public static IEnumerable<BsonValue> AVG(IEnumerable<BsonValue> values)
  {
    BsonValue bsonValue1 = new BsonValue(0);
    int num = 0;
    foreach (BsonValue bsonValue2 in values.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsNumber)))
    {
      bsonValue1 += bsonValue2;
      ++num;
    }
    if (num > 0)
      yield return bsonValue1 / (BsonValue) num;
  }

  public static IEnumerable<BsonValue> SUM(IEnumerable<BsonValue> values)
  {
    BsonValue bsonValue1 = new BsonValue(0);
    foreach (BsonValue bsonValue2 in values.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsNumber)))
      bsonValue1 += bsonValue2;
    yield return bsonValue1;
  }

  public static IEnumerable<BsonValue> ALL(IEnumerable<BsonValue> values)
  {
    foreach (bool flag in values.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsBoolean)).Select<BsonValue, bool>((Func<BsonValue, bool>) (x => x.AsBoolean)))
    {
      if (!flag)
      {
        yield return (BsonValue) false;
        yield break;
      }
    }
    yield return (BsonValue) true;
  }

  public static IEnumerable<BsonValue> JOIN(IEnumerable<BsonValue> values)
  {
    return BsonExpression.JOIN(values, (IEnumerable<BsonValue>) null);
  }

  public static IEnumerable<BsonValue> JOIN(
    IEnumerable<BsonValue> values,
    IEnumerable<BsonValue> separator = null)
  {
    IEnumerable<BsonValue> source = separator;
    yield return (BsonValue) string.Join((source != null ? source.FirstOrDefault<BsonValue>().AsString : (string) null) ?? ",", values.Select<BsonValue, string>((Func<BsonValue, string>) (x => x.AsString)).ToArray<string>());
  }

  public static IEnumerable<BsonValue> JSON(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue json in values.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsString)))
      yield return JsonSerializer.Deserialize((string) json);
  }

  public static IEnumerable<BsonValue> EXTEND(
    IEnumerable<BsonValue> source,
    IEnumerable<BsonValue> extend)
  {
    foreach (ZipValues zipValue in source.ZipValues(extend))
    {
      if (zipValue.First.IsDocument && zipValue.Second.IsDocument)
      {
        BsonDocument asDocument = zipValue.First.AsDocument;
        zipValue.Second.AsDocument.CopyTo(asDocument);
        yield return (BsonValue) asDocument;
      }
    }
  }

  public static IEnumerable<BsonValue> ITEMS(IEnumerable<BsonValue> array)
  {
    foreach (BsonArray bsonArray in array.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsArray)).Select<BsonValue, BsonArray>((Func<BsonValue, BsonArray>) (x => x.AsArray)))
    {
      foreach (BsonValue bsonValue in bsonArray)
        yield return bsonValue;
    }
  }

  public static IEnumerable<BsonValue> MINVALUE()
  {
    yield return BsonValue.MinValue;
  }

  public static IEnumerable<BsonValue> INT32(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue value in values)
    {
      if (value.IsNumber)
      {
        yield return (BsonValue) value.AsInt32;
      }
      else
      {
        int result;
        if (int.TryParse(value.AsString, out result))
          yield return (BsonValue) result;
      }
    }
  }

  public static IEnumerable<BsonValue> INT64(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue value in values)
    {
      if (value.IsNumber)
      {
        yield return (BsonValue) value.AsInt64;
      }
      else
      {
        long result;
        if (long.TryParse(value.AsString, out result))
          yield return (BsonValue) result;
      }
    }
  }

  public static IEnumerable<BsonValue> DOUBLE(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue value in values)
    {
      if (value.IsNumber)
      {
        yield return (BsonValue) value.AsDouble;
      }
      else
      {
        double result;
        if (double.TryParse(value.AsString, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out result))
          yield return (BsonValue) result;
      }
    }
  }

  public static IEnumerable<BsonValue> DECIMAL(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue value in values)
    {
      if (value.IsNumber)
      {
        yield return (BsonValue) value.AsDecimal;
      }
      else
      {
        Decimal result;
        if (Decimal.TryParse(value.AsString, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out result))
          yield return (BsonValue) result;
      }
    }
  }

  public static IEnumerable<BsonValue> STRING(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.AsString;
  }

  public static IEnumerable<BsonValue> ARRAY(IEnumerable<BsonValue> values)
  {
    yield return (BsonValue) new BsonArray(values);
  }

  public static IEnumerable<BsonValue> OBJECTID()
  {
    yield return (BsonValue) ObjectId.NewObjectId();
  }

  public static IEnumerable<BsonValue> OBJECTID(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue value in values)
    {
      if (value.IsObjectId)
      {
        yield return (BsonValue) value.AsObjectId;
      }
      else
      {
        ObjectId objectId = ObjectId.Empty;
        bool flag = false;
        try
        {
          objectId = new ObjectId(value.AsString);
          flag = true;
        }
        catch
        {
        }
        if (flag)
          yield return (BsonValue) objectId;
      }
    }
  }

  public static IEnumerable<BsonValue> GUID()
  {
    yield return (BsonValue) Guid.NewGuid();
  }

  public static IEnumerable<BsonValue> GUID(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue value in values)
    {
      if (value.IsGuid)
      {
        yield return (BsonValue) value.AsGuid;
      }
      else
      {
        Guid guid = Guid.Empty;
        bool flag = false;
        try
        {
          guid = new Guid(value.AsString);
          flag = true;
        }
        catch
        {
        }
        if (flag)
          yield return (BsonValue) guid;
      }
    }
  }

  public static IEnumerable<BsonValue> DATETIME()
  {
    yield return (BsonValue) DateTime.Now;
  }

  public static IEnumerable<BsonValue> DATETIME(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue value in values)
    {
      if (value.IsDateTime)
      {
        yield return (BsonValue) value.AsDateTime;
      }
      else
      {
        DateTime result;
        if (DateTime.TryParse(value.AsString, (IFormatProvider) CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None, out result))
          yield return (BsonValue) result;
      }
    }
  }

  public static IEnumerable<BsonValue> DATETIME(
    IEnumerable<BsonValue> year,
    IEnumerable<BsonValue> month,
    IEnumerable<BsonValue> day)
  {
    foreach (ZipValues zipValue in year.ZipValues(month, day))
    {
      if (zipValue.First.IsNumber && zipValue.Second.IsNumber && zipValue.Third.IsNumber)
        yield return (BsonValue) new DateTime(zipValue.First.AsInt32, zipValue.Second.AsInt32, zipValue.Third.AsInt32);
    }
  }

  public static IEnumerable<BsonValue> MAXVALUE()
  {
    yield return BsonValue.MaxValue;
  }

  public static IEnumerable<BsonValue> IS_MINVALUE(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.IsMinValue;
  }

  public static IEnumerable<BsonValue> IS_NULL(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.IsNull;
  }

  public static IEnumerable<BsonValue> IS_INT32(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.AsInt32;
  }

  public static IEnumerable<BsonValue> IS_INT64(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.IsInt64;
  }

  public static IEnumerable<BsonValue> IS_DOUBLE(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.IsDouble;
  }

  public static IEnumerable<BsonValue> IS_DECIMAL(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.IsDecimal;
  }

  public static IEnumerable<BsonValue> IS_NUMBER(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.IsNumber;
  }

  public static IEnumerable<BsonValue> IS_STRING(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.IsString;
  }

  public static IEnumerable<BsonValue> IS_DOCUMENT(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.IsDocument;
  }

  public static IEnumerable<BsonValue> IS_ARRAY(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.IsArray;
  }

  public static IEnumerable<BsonValue> IS_BINARY(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.IsBinary;
  }

  public static IEnumerable<BsonValue> IS_OBJECTID(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.IsObjectId;
  }

  public static IEnumerable<BsonValue> IS_GUID(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.IsGuid;
  }

  public static IEnumerable<BsonValue> IS_BOOLEAN(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.IsBoolean;
  }

  public static IEnumerable<BsonValue> IS_DATETIME(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.IsDateTime;
  }

  public static IEnumerable<BsonValue> IS_MAXVALUE(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.IsMaxValue;
  }

  public static IEnumerable<BsonValue> INT(IEnumerable<BsonValue> values)
  {
    return BsonExpression.INT32(values);
  }

  public static IEnumerable<BsonValue> LONG(IEnumerable<BsonValue> values)
  {
    return BsonExpression.INT64(values);
  }

  public static IEnumerable<BsonValue> DATE() => BsonExpression.DATETIME();

  public static IEnumerable<BsonValue> DATE(IEnumerable<BsonValue> values)
  {
    return BsonExpression.DATETIME(values);
  }

  public static IEnumerable<BsonValue> DATE(
    IEnumerable<BsonValue> year,
    IEnumerable<BsonValue> month,
    IEnumerable<BsonValue> day)
  {
    return BsonExpression.DATETIME(year, month, day);
  }

  public static IEnumerable<BsonValue> IS_INT(IEnumerable<BsonValue> values)
  {
    return BsonExpression.IS_INT32(values);
  }

  public static IEnumerable<BsonValue> IS_LONG(IEnumerable<BsonValue> values)
  {
    return BsonExpression.IS_INT64(values);
  }

  public static IEnumerable<BsonValue> IS_BOOL(IEnumerable<BsonValue> values)
  {
    return BsonExpression.IS_BOOLEAN(values);
  }

  public static IEnumerable<BsonValue> IS_DATE(IEnumerable<BsonValue> values)
  {
    return BsonExpression.IS_DATETIME(values);
  }

  public static IEnumerable<BsonValue> YEAR(IEnumerable<BsonValue> values)
  {
    foreach (DateTime dateTime in values.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsDateTime)).Select<BsonValue, DateTime>((Func<BsonValue, DateTime>) (x => x.AsDateTime)))
      yield return (BsonValue) dateTime.Year;
  }

  public static IEnumerable<BsonValue> MONTH(IEnumerable<BsonValue> values)
  {
    foreach (DateTime dateTime in values.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsDateTime)).Select<BsonValue, DateTime>((Func<BsonValue, DateTime>) (x => x.AsDateTime)))
      yield return (BsonValue) dateTime.Month;
  }

  public static IEnumerable<BsonValue> DAY(IEnumerable<BsonValue> values)
  {
    foreach (DateTime dateTime in values.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsDateTime)).Select<BsonValue, DateTime>((Func<BsonValue, DateTime>) (x => x.AsDateTime)))
      yield return (BsonValue) dateTime.Day;
  }

  public static IEnumerable<BsonValue> HOUR(IEnumerable<BsonValue> values)
  {
    foreach (DateTime dateTime in values.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsDateTime)).Select<BsonValue, DateTime>((Func<BsonValue, DateTime>) (x => x.AsDateTime)))
      yield return (BsonValue) dateTime.Hour;
  }

  public static IEnumerable<BsonValue> MINUTE(IEnumerable<BsonValue> values)
  {
    foreach (DateTime dateTime in values.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsDateTime)).Select<BsonValue, DateTime>((Func<BsonValue, DateTime>) (x => x.AsDateTime)))
      yield return (BsonValue) dateTime.Minute;
  }

  public static IEnumerable<BsonValue> SECOND(IEnumerable<BsonValue> values)
  {
    foreach (DateTime dateTime in values.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsDateTime)).Select<BsonValue, DateTime>((Func<BsonValue, DateTime>) (x => x.AsDateTime)))
      yield return (BsonValue) dateTime.Second;
  }

  public static IEnumerable<BsonValue> DATEADD(
    IEnumerable<BsonValue> dateParts,
    IEnumerable<BsonValue> numbers,
    IEnumerable<BsonValue> values)
  {
    foreach (ZipValues zipValue in dateParts.ZipValues(numbers, values))
    {
      if (zipValue.First.IsString && zipValue.Second.IsNumber && zipValue.Third.IsDateTime)
      {
        string datePart = zipValue.First.AsString;
        int number = zipValue.Second.AsInt32;
        DateTime date = zipValue.Third.AsDateTime;
        datePart = datePart == "M" ? "month" : datePart.ToLower();
        if (datePart == "y" || datePart == "year")
          yield return (BsonValue) date.AddYears(number);
        else if (datePart == "month")
          yield return (BsonValue) date.AddMonths(number);
        else if (datePart == "d" || datePart == "day")
          yield return (BsonValue) date.AddDays((double) number);
        else if (datePart == "h" || datePart == "hour")
          yield return (BsonValue) date.AddHours((double) number);
        else if (datePart == "m" || datePart == "minute")
          yield return (BsonValue) date.AddMinutes((double) number);
        else if (datePart == "s" || datePart == "second")
          yield return (BsonValue) date.AddSeconds((double) number);
        datePart = (string) null;
      }
    }
  }

  public static IEnumerable<BsonValue> DATEDIFF(
    IEnumerable<BsonValue> dateParts,
    IEnumerable<BsonValue> starts,
    IEnumerable<BsonValue> ends)
  {
    foreach (ZipValues zipValue in dateParts.ZipValues(starts, ends))
    {
      if (zipValue.First.IsString && zipValue.Second.IsDateTime && zipValue.Third.IsDateTime)
      {
        string datePart = zipValue.First.AsString;
        DateTime start = zipValue.Second.AsDateTime;
        DateTime end = zipValue.Third.AsDateTime;
        datePart = datePart == "M" ? "month" : datePart.ToLower();
        if (datePart == "y" || datePart == "year")
          yield return (BsonValue) start.YearDifference(end);
        else if (datePart == "month")
          yield return (BsonValue) start.MonthDifference(end);
        else if (datePart == "d" || datePart == "day")
          yield return (BsonValue) Convert.ToInt32(Math.Truncate(end.Subtract(start).TotalDays));
        else if (datePart == "h" || datePart == "hour")
          yield return (BsonValue) Convert.ToInt32(Math.Truncate(end.Subtract(start).TotalHours));
        else if (datePart == "m" || datePart == "minute")
          yield return (BsonValue) Convert.ToInt32(Math.Truncate(end.Subtract(start).TotalMinutes));
        else if (datePart == "s" || datePart == "second")
          yield return (BsonValue) Convert.ToInt32(Math.Truncate(end.Subtract(start).TotalSeconds));
        datePart = (string) null;
      }
    }
  }

  public static IEnumerable<BsonValue> KEYS(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsDocument)))
    {
      foreach (string key in (IEnumerable<string>) bsonValue.AsDocument.Keys)
        yield return (BsonValue) key;
    }
  }

  public static IEnumerable<BsonValue> IIF(
    IEnumerable<BsonValue> condition,
    IEnumerable<BsonValue> ifTrue,
    IEnumerable<BsonValue> ifFalse)
  {
    foreach (ZipValues zipValues in condition.ZipValues(ifTrue, ifFalse).Where<ZipValues>((Func<ZipValues, bool>) (x => x.First.IsBoolean)))
      yield return zipValues.First.AsBoolean ? zipValues.Second : zipValues.Third;
  }

  public static IEnumerable<BsonValue> LENGTH(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue value in values)
    {
      if (value.IsString)
        yield return (BsonValue) value.AsString.Length;
      else if (value.IsBinary)
        yield return (BsonValue) value.AsBinary.Length;
      else if (value.IsArray)
        yield return (BsonValue) value.AsArray.Count;
      else if (value.IsDocument)
        yield return (BsonValue) value.AsDocument.Keys.Count;
    }
  }

  public static IEnumerable<BsonValue> LOWER(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
    {
      if (bsonValue.IsString)
        yield return (BsonValue) bsonValue.AsString.ToLower();
    }
  }

  public static IEnumerable<BsonValue> UPPER(IEnumerable<BsonValue> values)
  {
    foreach (BsonValue bsonValue in values)
    {
      if (bsonValue.IsString)
        yield return (BsonValue) bsonValue.AsString.ToUpper();
    }
  }

  public static IEnumerable<BsonValue> SUBSTRING(
    IEnumerable<BsonValue> values,
    IEnumerable<BsonValue> index)
  {
    IEnumerable<BsonValue> source = index;
    int idx = (source != null ? source.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsInt32)).FirstOrDefault<BsonValue>()?.AsInt32 : new int?()) ?? 0;
    foreach (BsonValue bsonValue in values)
    {
      if (bsonValue.IsString)
        yield return (BsonValue) bsonValue.AsString.Substring(idx);
    }
  }

  public static IEnumerable<BsonValue> SUBSTRING(
    IEnumerable<BsonValue> values,
    IEnumerable<BsonValue> index,
    IEnumerable<BsonValue> length)
  {
    IEnumerable<BsonValue> source1 = index;
    int? nullable = source1 != null ? source1.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsInt32)).FirstOrDefault<BsonValue>()?.AsInt32 : new int?();
    int idx = nullable ?? 0;
    IEnumerable<BsonValue> source2 = length;
    nullable = source2 != null ? source2.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsInt32)).FirstOrDefault<BsonValue>()?.AsInt32 : new int?();
    int len = nullable ?? 0;
    foreach (BsonValue bsonValue in values)
    {
      if (bsonValue.IsString)
        yield return (BsonValue) bsonValue.AsString.Substring(idx, len);
    }
  }

  public static IEnumerable<BsonValue> LPAD(
    IEnumerable<BsonValue> values,
    IEnumerable<BsonValue> totalWidth,
    IEnumerable<BsonValue> paddingChar)
  {
    IEnumerable<BsonValue> source1 = totalWidth;
    int width = (source1 != null ? source1.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsInt32)).FirstOrDefault<BsonValue>()?.AsInt32 : new int?()) ?? 0;
    IEnumerable<BsonValue> source2 = paddingChar;
    char pchar = (char) ((source2 != null ? (int) source2.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsString)).FirstOrDefault<BsonValue>()?.AsString.ToCharArray()[0] : (int) new char?()) ?? 48 /*0x30*/);
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.AsString.PadLeft(width, pchar);
  }

  public static IEnumerable<BsonValue> RPAD(
    IEnumerable<BsonValue> values,
    IEnumerable<BsonValue> totalWidth,
    IEnumerable<BsonValue> paddingChar)
  {
    IEnumerable<BsonValue> source1 = totalWidth;
    int width = (source1 != null ? source1.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsInt32)).FirstOrDefault<BsonValue>()?.AsInt32 : new int?()) ?? 0;
    IEnumerable<BsonValue> source2 = paddingChar;
    char pchar = (char) ((source2 != null ? (int) source2.Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsString)).FirstOrDefault<BsonValue>()?.AsString.ToCharArray()[0] : (int) new char?()) ?? 48 /*0x30*/);
    foreach (BsonValue bsonValue in values)
      yield return (BsonValue) bsonValue.AsString.PadRight(width, pchar);
  }

  public static IEnumerable<BsonValue> FORMAT(
    IEnumerable<BsonValue> values,
    IEnumerable<BsonValue> format)
  {
    foreach (ZipValues zipValues in values.ZipValues(format).Where<ZipValues>((Func<ZipValues, bool>) (x => x.Second.IsString)))
      yield return (BsonValue) string.Format($"{{0:{zipValues.Second.AsString}}}", zipValues.First.RawValue);
  }
}
