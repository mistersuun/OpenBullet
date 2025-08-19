// Decompiled with JetBrains decompiler
// Type: LiteDB.BsonDocument
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB;

public class BsonDocument : 
  BsonValue,
  IDictionary<string, BsonValue>,
  ICollection<KeyValuePair<string, BsonValue>>,
  IEnumerable<KeyValuePair<string, BsonValue>>,
  IEnumerable
{
  public BsonDocument()
    : base(new Dictionary<string, BsonValue>())
  {
  }

  public BsonDocument(Dictionary<string, BsonValue> dict)
    : base(dict)
  {
    if (dict == null)
      throw new ArgumentNullException(nameof (dict));
  }

  public Dictionary<string, BsonValue> RawValue => (Dictionary<string, BsonValue>) base.RawValue;

  public BsonValue this[string name]
  {
    get => this.RawValue.GetOrDefault<string, BsonValue>(name, BsonValue.Null);
    set
    {
      if (!BsonDocument.IsValidFieldName(name))
        throw new ArgumentException($"Field '{name}' has an invalid name.");
      Dictionary<string, BsonValue> rawValue = this.RawValue;
      string key = name;
      BsonValue bsonValue = value;
      if ((object) bsonValue == null)
        bsonValue = BsonValue.Null;
      rawValue[key] = bsonValue;
    }
  }

  internal static bool IsValidFieldName(string field)
  {
    if (string.IsNullOrEmpty(field))
      return false;
    for (int index = 0; index < field.Length; ++index)
    {
      char c = field[index];
      if (!char.IsLetterOrDigit(c) && c != '_' && (c != '$' || index != 0) && (c != '-' || index <= 0))
        return false;
    }
    return true;
  }

  public IEnumerable<BsonValue> Get(string path, bool includeNullIfEmpty = false)
  {
    return new BsonExpression(new StringScanner(path), true, true).Execute(this, includeNullIfEmpty);
  }

  public bool Set(string path, BsonExpression expr)
  {
    BsonValue bsonValue = expr != null ? expr.Execute(this).First<BsonValue>() : throw new ArgumentNullException(nameof (expr));
    return this.Set(path, bsonValue);
  }

  public bool Set(string path, BsonValue value)
  {
    if (string.IsNullOrEmpty(path))
      throw new ArgumentNullException(nameof (value));
    if (value == (BsonValue) null)
      throw new ArgumentNullException(nameof (value));
    string str = path.StartsWith("$") ? path : "$." + path;
    string expression = str.Substring(0, str.LastIndexOf('.'));
    string name = str.Substring(str.LastIndexOf('.') + 1);
    BsonExpression bsonExpression = new BsonExpression(expression);
    bool flag = false;
    foreach (BsonValue bsonValue in bsonExpression.Execute(this, false).Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsDocument)))
    {
      BsonDocument asDocument = bsonValue.AsDocument;
      if (asDocument[name] != value)
      {
        asDocument[name] = value;
        flag = true;
      }
    }
    return flag;
  }

  public bool Set(string path, BsonExpression expr, bool addInArray)
  {
    BsonValue bsonValue = expr != null ? expr.Execute(this).First<BsonValue>() : throw new ArgumentNullException(nameof (expr));
    return this.Set(path, bsonValue, addInArray);
  }

  public bool Set(string path, BsonValue value, bool addInArray)
  {
    if (string.IsNullOrEmpty(path))
      throw new ArgumentNullException(nameof (value));
    if (value == (BsonValue) null)
      throw new ArgumentNullException(nameof (value));
    if (!addInArray)
      return this.Set(path, value);
    BsonExpression bsonExpression = new BsonExpression(path.StartsWith("$") ? path : "$." + path);
    bool flag = false;
    foreach (BsonValue bsonValue in bsonExpression.Execute(this, false).Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsArray)))
    {
      bsonValue.AsArray.Add(value);
      flag = true;
    }
    return flag;
  }

  public override int CompareTo(BsonValue other)
  {
    if (other.Type != BsonType.Document)
      return this.Type.CompareTo((object) other.Type);
    string[] array = this.Keys.ToArray<string>();
    int length1 = array.Length;
    BsonDocument asDocument = other.AsDocument;
    int length2 = asDocument.Keys.ToArray<string>().Length;
    int num = 0;
    int index1 = 0;
    for (int index2 = Math.Min(length1, length2); num == 0 && index1 < index2; ++index1)
      num = this[array[index1]].CompareTo(asDocument[array[index1]]);
    if (num != 0)
      return num;
    if (index1 != length1)
      return 1;
    return index1 != length2 ? -1 : 0;
  }

  public override string ToString() => JsonSerializer.Serialize((BsonValue) this);

  public ICollection<string> Keys
  {
    get
    {
      return (ICollection<string>) this.RawValue.Keys.OrderBy<string, int>((Func<string, int>) (x => !(x == "_id") ? 2 : 1)).ToList<string>();
    }
  }

  public ICollection<BsonValue> Values => (ICollection<BsonValue>) this.RawValue.Values;

  public int Count => this.RawValue.Count;

  public bool IsReadOnly => false;

  public bool ContainsKey(string key) => this.RawValue.ContainsKey(key);

  public void Add(string key, BsonValue value) => this[key] = value;

  public bool Remove(string key) => this.RawValue.Remove(key);

  public bool TryGetValue(string key, out BsonValue value)
  {
    return this.RawValue.TryGetValue(key, out value);
  }

  public void Add(KeyValuePair<string, BsonValue> item) => this[item.Key] = item.Value;

  public void Clear() => this.RawValue.Clear();

  public bool Contains(KeyValuePair<string, BsonValue> item)
  {
    return this.RawValue.Contains<KeyValuePair<string, BsonValue>>(item);
  }

  public void CopyTo(KeyValuePair<string, BsonValue>[] array, int arrayIndex)
  {
    ((ICollection<KeyValuePair<string, BsonValue>>) this.RawValue).CopyTo(array, arrayIndex);
  }

  public void CopyTo(BsonDocument doc)
  {
    Dictionary<string, BsonValue> rawValue1 = this.RawValue;
    Dictionary<string, BsonValue> rawValue2 = doc.RawValue;
    foreach (string key in rawValue1.Keys)
      rawValue2[key] = rawValue1[key];
  }

  public bool Remove(KeyValuePair<string, BsonValue> item) => this.RawValue.Remove(item.Key);

  public IEnumerator<KeyValuePair<string, BsonValue>> GetEnumerator()
  {
    return (IEnumerator<KeyValuePair<string, BsonValue>>) this.RawValue.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.RawValue.GetEnumerator();
}
