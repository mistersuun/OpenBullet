// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.BaseCollection
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#nullable disable
namespace LiteDB.Shell;

internal class BaseCollection
{
  public Regex FieldPattern = new Regex("^[\\$\\w](\\.?[\\w\\$][\\w-]*)*\\s*", RegexOptions.Compiled);

  public string ReadCollection(LiteEngine db, StringScanner s)
  {
    return s.Scan("db\\.([\\w-]+)\\.\\w+\\s*", 1);
  }

  public bool IsCollectionCommand(StringScanner s, string command)
  {
    return s.Match("db\\.[\\w-]+\\." + command);
  }

  public KeyValuePair<int, int> ReadSkipLimit(StringScanner s)
  {
    int key = 0;
    int num = int.MaxValue;
    if (s.Match("\\s*skip\\s+\\d+"))
      key = Convert.ToInt32(s.Scan("\\s*skip\\s+(\\d+)\\s*", 1));
    if (s.Match("\\s*limit\\s+\\d+"))
      num = Convert.ToInt32(s.Scan("\\s*limit\\s+(\\d+)\\s*", 1));
    if (s.Match("\\s*skip\\s+\\d+"))
      key = Convert.ToInt32(s.Scan("\\s*skip\\s+(\\d+)\\s*", 1));
    return new KeyValuePair<int, int>(key, num);
  }

  public string[] ReadIncludes(StringScanner s)
  {
    if (s.Scan("\\s*include[s]?\\s+").Length <= 0)
      return new string[0];
    List<string> stringList = new List<string>();
    for (BsonExpression bsonExpression = BsonExpression.ReadExpression(s, true, true); bsonExpression != null; bsonExpression = s.Scan("\\s*,\\s*").Length > 0 ? BsonExpression.ReadExpression(s, true, true) : (BsonExpression) null)
      stringList.Add(bsonExpression.Source);
    return stringList.ToArray();
  }

  public Query ReadQuery(StringScanner s, bool required)
  {
    s.Scan("\\s*");
    if (required && s.HasTerminated)
      throw LiteException.SyntaxError(s, "Unexpected finish of line");
    return s.HasTerminated || s.Match("skip\\s+\\d") || s.Match("limit\\s+\\d") || s.Match("include[s]?\\s+[\\$\\w]") ? Query.All() : this.ReadInlineQuery(s);
  }

  private Query ReadInlineQuery(StringScanner s)
  {
    Query left = this.ReadOneQuery(s);
    string str = s.Scan("\\s+(and|or)\\s+").ToLower().Trim();
    if (str.Length == 0)
      return left;
    Query right = this.ReadInlineQuery(s);
    return !(str == "and") ? Query.Or(left, right) : Query.And(left, right);
  }

  private Query ReadOneQuery(StringScanner s)
  {
    string field = BsonExpression.ReadExpression(s, false, false)?.Source ?? s.Scan(this.FieldPattern).Trim().ThrowIfEmpty("Invalid field", s);
    string str = s.Scan("\\s*(=|!=|>=|<=|>|<|like|starts[Ww]ith|in|between|contains)\\s*").Trim().ToLower().ThrowIfEmpty("Invalid query operator", s);
    BsonValue bsonValue = !s.HasTerminated ? JsonSerializer.Deserialize(s) : throw LiteException.SyntaxError(s, "Missing value");
    switch (str)
    {
      case "!=":
        return Query.Not(field, bsonValue);
      case "<":
        return Query.LT(field, bsonValue);
      case "<=":
        return Query.LTE(field, bsonValue);
      case "=":
        return Query.EQ(field, bsonValue);
      case ">":
        return Query.GT(field, bsonValue);
      case ">=":
        return Query.GTE(field, bsonValue);
      case "between":
        return Query.Between(field, bsonValue.AsArray[0], bsonValue.AsArray[1]);
      case "contains":
        return Query.Contains(field, (string) bsonValue);
      case "in":
        return Query.In(field, bsonValue.AsArray);
      case "like":
      case "startswith":
        return Query.StartsWith(field, (string) bsonValue);
      default:
        throw new LiteException("Invalid query operator");
    }
  }

  public BsonValue ReadBsonValue(StringScanner s)
  {
    int index = s.Index;
    try
    {
      return JsonSerializer.Deserialize(s);
    }
    catch (LiteException ex) when (ex.ErrorCode == 203)
    {
      s.Index = index;
      return (BsonValue) null;
    }
  }
}
