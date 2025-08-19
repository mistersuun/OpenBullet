// Decompiled with JetBrains decompiler
// Type: LiteDB.BsonArray
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB;

public class BsonArray : 
  BsonValue,
  IList<BsonValue>,
  ICollection<BsonValue>,
  IEnumerable<BsonValue>,
  IEnumerable
{
  public BsonArray()
    : base(new List<BsonValue>())
  {
  }

  public BsonArray(List<BsonValue> array)
    : base(array)
  {
    if (array == null)
      throw new ArgumentNullException(nameof (array));
  }

  public BsonArray(BsonValue[] array)
    : base(new List<BsonValue>((IEnumerable<BsonValue>) array))
  {
    if (array == null)
      throw new ArgumentNullException(nameof (array));
  }

  public BsonArray(IEnumerable<BsonValue> items)
    : this()
  {
    this.AddRange<BsonValue>(items);
  }

  public BsonArray(IEnumerable<BsonArray> items)
    : this()
  {
    this.AddRange<BsonArray>(items);
  }

  public BsonArray(IEnumerable<BsonDocument> items)
    : this()
  {
    this.AddRange<BsonDocument>(items);
  }

  public List<BsonValue> RawValue => (List<BsonValue>) base.RawValue;

  public BsonValue this[int index]
  {
    get => this.RawValue.ElementAt<BsonValue>(index);
    set
    {
      List<BsonValue> rawValue = this.RawValue;
      int index1 = index;
      BsonValue bsonValue = value;
      if ((object) bsonValue == null)
        bsonValue = BsonValue.Null;
      rawValue[index1] = bsonValue;
    }
  }

  public int Count => this.RawValue.Count;

  public bool IsReadOnly => false;

  public void Add(BsonValue item)
  {
    List<BsonValue> rawValue = this.RawValue;
    BsonValue bsonValue = item;
    if ((object) bsonValue == null)
      bsonValue = BsonValue.Null;
    rawValue.Add(bsonValue);
  }

  public virtual void AddRange<T>(IEnumerable<T> array) where T : BsonValue
  {
    if (array == null)
      throw new ArgumentNullException(nameof (array));
    foreach (T obj in array)
    {
      BsonValue bsonValue = (BsonValue) obj;
      if ((object) bsonValue == null)
        bsonValue = BsonValue.Null;
      this.Add(bsonValue);
    }
  }

  public void Clear() => this.RawValue.Clear();

  public bool Contains(BsonValue item) => this.RawValue.Contains(item);

  public void CopyTo(BsonValue[] array, int arrayIndex) => this.RawValue.CopyTo(array, arrayIndex);

  public IEnumerator<BsonValue> GetEnumerator()
  {
    return (IEnumerator<BsonValue>) this.RawValue.GetEnumerator();
  }

  public int IndexOf(BsonValue item) => this.RawValue.IndexOf(item);

  public void Insert(int index, BsonValue item) => this.RawValue.Insert(index, item);

  public bool Remove(BsonValue item) => this.RawValue.Remove(item);

  public void RemoveAt(int index) => this.RawValue.RemoveAt(index);

  IEnumerator IEnumerable.GetEnumerator()
  {
    foreach (BsonValue bsonValue in this.RawValue)
      yield return (object) new BsonValue(bsonValue);
  }

  public override int CompareTo(BsonValue other)
  {
    if (other.Type != BsonType.Document)
      return this.Type.CompareTo((object) other.Type);
    BsonArray asArray = other.AsArray;
    int num = 0;
    int index1 = 0;
    for (int index2 = Math.Min(this.Count, asArray.Count); num == 0 && index1 < index2; ++index1)
      num = this[index1].CompareTo(asArray[index1]);
    if (num != 0)
      return num;
    if (index1 != this.Count)
      return 1;
    return index1 != asArray.Count ? -1 : 0;
  }

  public override string ToString() => JsonSerializer.Serialize((BsonValue) this);
}
