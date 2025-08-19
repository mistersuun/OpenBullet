// Decompiled with JetBrains decompiler
// Type: LiteDB.BsonValue
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace LiteDB;

public class BsonValue : IComparable<BsonValue>, IEquatable<BsonValue>
{
  public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
  public static readonly BsonValue Null = new BsonValue();
  public static readonly BsonValue MinValue = new BsonValue()
  {
    Type = BsonType.MinValue,
    RawValue = (object) "-oo"
  };
  public static readonly BsonValue MaxValue = new BsonValue()
  {
    Type = BsonType.MaxValue,
    RawValue = (object) "+oo"
  };
  internal Action Destroy = (Action) (() => { });
  internal int? Length;

  public BsonType Type { get; private set; }

  public virtual object RawValue { get; private set; }

  public BsonValue()
  {
    this.Type = BsonType.Null;
    this.RawValue = (object) null;
  }

  public BsonValue(int value)
  {
    this.Type = BsonType.Int32;
    this.RawValue = (object) value;
  }

  public BsonValue(long value)
  {
    this.Type = BsonType.Int64;
    this.RawValue = (object) value;
  }

  public BsonValue(double value)
  {
    this.Type = BsonType.Double;
    this.RawValue = (object) value;
  }

  public BsonValue(Decimal value)
  {
    this.Type = BsonType.Decimal;
    this.RawValue = (object) value;
  }

  public BsonValue(string value)
  {
    this.Type = value == null ? BsonType.Null : BsonType.String;
    this.RawValue = (object) value;
  }

  public BsonValue(Dictionary<string, BsonValue> value)
  {
    this.Type = value == null ? BsonType.Null : BsonType.Document;
    this.RawValue = (object) value;
  }

  public BsonValue(List<BsonValue> value)
  {
    this.Type = value == null ? BsonType.Null : BsonType.Array;
    this.RawValue = (object) value;
  }

  public BsonValue(byte[] value)
  {
    this.Type = value == null ? BsonType.Null : BsonType.Binary;
    this.RawValue = (object) value;
  }

  public BsonValue(ObjectId value)
  {
    this.Type = value == (ObjectId) null ? BsonType.Null : BsonType.ObjectId;
    this.RawValue = (object) value;
  }

  public BsonValue(Guid value)
  {
    this.Type = BsonType.Guid;
    this.RawValue = (object) value;
  }

  public BsonValue(bool value)
  {
    this.Type = BsonType.Boolean;
    this.RawValue = (object) value;
  }

  public BsonValue(DateTime value)
  {
    this.Type = BsonType.DateTime;
    this.RawValue = (object) value.Truncate();
  }

  public BsonValue(BsonValue value)
  {
    this.Type = value == (BsonValue) null ? BsonType.Null : value.Type;
    this.RawValue = value.RawValue;
  }

  public BsonValue(object value)
  {
    this.RawValue = value;
    switch (value)
    {
      case null:
        this.Type = BsonType.Null;
        break;
      case int _:
        this.Type = BsonType.Int32;
        break;
      case long _:
        this.Type = BsonType.Int64;
        break;
      case double _:
        this.Type = BsonType.Double;
        break;
      case Decimal _:
        this.Type = BsonType.Decimal;
        break;
      case string _:
        this.Type = BsonType.String;
        break;
      case Dictionary<string, BsonValue> _:
        this.Type = BsonType.Document;
        break;
      case List<BsonValue> _:
        this.Type = BsonType.Array;
        break;
      case byte[] _:
        this.Type = BsonType.Binary;
        break;
      default:
        if ((object) (value as ObjectId) != null)
        {
          this.Type = BsonType.ObjectId;
          break;
        }
        switch (value)
        {
          case Guid _:
            this.Type = BsonType.Guid;
            return;
          case bool _:
            this.Type = BsonType.Boolean;
            return;
          case DateTime dt:
            this.Type = BsonType.DateTime;
            this.RawValue = (object) dt.Truncate();
            return;
          default:
            if ((object) (value as BsonValue) != null)
            {
              BsonValue bsonValue = (BsonValue) value;
              this.Type = bsonValue.Type;
              this.RawValue = bsonValue.RawValue;
              return;
            }
            IEnumerable enumerable = value as IEnumerable;
            if (value is IDictionary dictionary1)
            {
              Dictionary<string, BsonValue> dictionary = new Dictionary<string, BsonValue>();
              foreach (object key in (IEnumerable) dictionary1.Keys)
                dictionary.Add(key.ToString(), new BsonValue(dictionary1[key]));
              this.Type = BsonType.Document;
              this.RawValue = (object) dictionary;
              return;
            }
            if (enumerable == null)
              throw new InvalidCastException("Value is not a valid BSON data type - Use Mapper.ToDocument for more complex types converts");
            List<BsonValue> bsonValueList = new List<BsonValue>();
            foreach (object obj in enumerable)
              bsonValueList.Add(new BsonValue(obj));
            this.Type = BsonType.Array;
            this.RawValue = (object) bsonValueList;
            return;
        }
    }
  }

  public BsonArray AsArray
  {
    get
    {
      if (!this.IsArray)
        return (BsonArray) null;
      BsonArray asArray = new BsonArray((List<BsonValue>) this.RawValue);
      asArray.Length = this.Length;
      asArray.Destroy = this.Destroy;
      return asArray;
    }
  }

  public BsonDocument AsDocument
  {
    get
    {
      if (!this.IsDocument)
        return (BsonDocument) null;
      BsonDocument asDocument = new BsonDocument((Dictionary<string, BsonValue>) this.RawValue);
      asDocument.Length = this.Length;
      asDocument.Destroy = this.Destroy;
      return asDocument;
    }
  }

  public byte[] AsBinary => this.Type != BsonType.Binary ? (byte[]) null : (byte[]) this.RawValue;

  public bool AsBoolean => this.Type == BsonType.Boolean && (bool) this.RawValue;

  public string AsString => this.Type == BsonType.Null ? (string) null : this.RawValue.ToString();

  public int AsInt32 => !this.IsNumber ? 0 : Convert.ToInt32(this.RawValue);

  public long AsInt64 => !this.IsNumber ? 0L : Convert.ToInt64(this.RawValue);

  public double AsDouble => !this.IsNumber ? 0.0 : Convert.ToDouble(this.RawValue);

  public Decimal AsDecimal => !this.IsNumber ? 0M : Convert.ToDecimal(this.RawValue);

  public DateTime AsDateTime
  {
    get => this.Type != BsonType.DateTime ? new DateTime() : (DateTime) this.RawValue;
  }

  public ObjectId AsObjectId
  {
    get => this.Type != BsonType.ObjectId ? (ObjectId) null : (ObjectId) this.RawValue;
  }

  public Guid AsGuid => this.Type != BsonType.Guid ? new Guid() : (Guid) this.RawValue;

  public bool IsNull => this.Type == BsonType.Null;

  public bool IsArray => this.Type == BsonType.Array;

  public bool IsDocument => this.Type == BsonType.Document;

  public bool IsInt32 => this.Type == BsonType.Int32;

  public bool IsInt64 => this.Type == BsonType.Int64;

  public bool IsDouble => this.Type == BsonType.Double;

  public bool IsDecimal => this.Type == BsonType.Decimal;

  public bool IsNumber => this.IsInt32 || this.IsInt64 || this.IsDouble || this.IsDecimal;

  public bool IsBinary => this.Type == BsonType.Binary;

  public bool IsBoolean => this.Type == BsonType.Boolean;

  public bool IsString => this.Type == BsonType.String;

  public bool IsObjectId => this.Type == BsonType.ObjectId;

  public bool IsGuid => this.Type == BsonType.Guid;

  public bool IsDateTime => this.Type == BsonType.DateTime;

  public bool IsMinValue => this.Type == BsonType.MinValue;

  public bool IsMaxValue => this.Type == BsonType.MaxValue;

  public static implicit operator int(BsonValue value) => (int) value.RawValue;

  public static implicit operator BsonValue(int value) => new BsonValue(value);

  public static implicit operator long(BsonValue value) => (long) value.RawValue;

  public static implicit operator BsonValue(long value) => new BsonValue(value);

  public static implicit operator double(BsonValue value) => (double) value.RawValue;

  public static implicit operator BsonValue(double value) => new BsonValue(value);

  public static implicit operator Decimal(BsonValue value) => (Decimal) value.RawValue;

  public static implicit operator BsonValue(Decimal value) => new BsonValue(value);

  public static implicit operator ulong(BsonValue value) => (ulong) value.RawValue;

  public static implicit operator BsonValue(ulong value) => new BsonValue((double) value);

  public static implicit operator string(BsonValue value) => (string) value.RawValue;

  public static implicit operator BsonValue(string value) => new BsonValue(value);

  public static implicit operator Dictionary<string, BsonValue>(BsonValue value)
  {
    return (Dictionary<string, BsonValue>) value.RawValue;
  }

  public static implicit operator BsonValue(Dictionary<string, BsonValue> value)
  {
    return new BsonValue(value);
  }

  public static implicit operator List<BsonValue>(BsonValue value)
  {
    return (List<BsonValue>) value.RawValue;
  }

  public static implicit operator BsonValue(List<BsonValue> value) => new BsonValue(value);

  public static implicit operator byte[](BsonValue value) => (byte[]) value.RawValue;

  public static implicit operator BsonValue(byte[] value) => new BsonValue(value);

  public static implicit operator ObjectId(BsonValue value) => (ObjectId) value.RawValue;

  public static implicit operator BsonValue(ObjectId value) => new BsonValue(value);

  public static implicit operator Guid(BsonValue value) => (Guid) value.RawValue;

  public static implicit operator BsonValue(Guid value) => new BsonValue(value);

  public static implicit operator bool(BsonValue value) => (bool) value.RawValue;

  public static implicit operator BsonValue(bool value) => new BsonValue(value);

  public static implicit operator DateTime(BsonValue value) => (DateTime) value.RawValue;

  public static implicit operator BsonValue(DateTime value) => new BsonValue(value);

  public static BsonValue operator +(BsonValue left, BsonValue right)
  {
    if (!left.IsNumber || !right.IsNumber)
      return BsonValue.Null;
    if (left.IsInt32 && right.IsInt32)
      return (BsonValue) (left.AsInt32 + right.AsInt32);
    if (left.IsInt64 && right.IsInt64)
      return (BsonValue) (left.AsInt64 + right.AsInt64);
    if (left.IsDouble && right.IsDouble)
      return (BsonValue) (left.AsDouble + right.AsDouble);
    if (left.IsDecimal && right.IsDecimal)
      return (BsonValue) (left.AsDecimal + right.AsDecimal);
    Decimal num = left.AsDecimal + right.AsDecimal;
    switch ((BsonType) Math.Max((int) left.Type, (int) right.Type))
    {
      case BsonType.Int64:
        return new BsonValue((long) num);
      case BsonType.Double:
        return new BsonValue((double) num);
      default:
        return new BsonValue(num);
    }
  }

  public static BsonValue operator -(BsonValue left, BsonValue right)
  {
    if (!left.IsNumber || !right.IsNumber)
      return BsonValue.Null;
    if (left.IsInt32 && right.IsInt32)
      return (BsonValue) (left.AsInt32 - right.AsInt32);
    if (left.IsInt64 && right.IsInt64)
      return (BsonValue) (left.AsInt64 - right.AsInt64);
    if (left.IsDouble && right.IsDouble)
      return (BsonValue) (left.AsDouble - right.AsDouble);
    if (left.IsDecimal && right.IsDecimal)
      return (BsonValue) (left.AsDecimal - right.AsDecimal);
    Decimal num = left.AsDecimal - right.AsDecimal;
    switch ((BsonType) Math.Max((int) left.Type, (int) right.Type))
    {
      case BsonType.Int64:
        return new BsonValue((long) num);
      case BsonType.Double:
        return new BsonValue((double) num);
      default:
        return new BsonValue(num);
    }
  }

  public static BsonValue operator *(BsonValue left, BsonValue right)
  {
    if (!left.IsNumber || !right.IsNumber)
      return BsonValue.Null;
    if (left.IsInt32 && right.IsInt32)
      return (BsonValue) (left.AsInt32 * right.AsInt32);
    if (left.IsInt64 && right.IsInt64)
      return (BsonValue) (left.AsInt64 * right.AsInt64);
    if (left.IsDouble && right.IsDouble)
      return (BsonValue) (left.AsDouble * right.AsDouble);
    if (left.IsDecimal && right.IsDecimal)
      return (BsonValue) (left.AsDecimal * right.AsDecimal);
    Decimal num = left.AsDecimal * right.AsDecimal;
    switch ((BsonType) Math.Max((int) left.Type, (int) right.Type))
    {
      case BsonType.Int64:
        return new BsonValue((long) num);
      case BsonType.Double:
        return new BsonValue((double) num);
      default:
        return new BsonValue(num);
    }
  }

  public static BsonValue operator /(BsonValue left, BsonValue right)
  {
    if (!left.IsNumber || !right.IsNumber)
      return BsonValue.Null;
    return left.IsDecimal || right.IsDecimal ? (BsonValue) (left.AsDecimal / right.AsDecimal) : (BsonValue) (left.AsDouble / right.AsDouble);
  }

  public override string ToString() => JsonSerializer.Serialize(this);

  public virtual int CompareTo(BsonValue other)
  {
    if (this.Type != other.Type)
      return this.IsNumber && other.IsNumber ? Convert.ToDecimal(this.RawValue).CompareTo(Convert.ToDecimal(other.RawValue)) : this.Type.CompareTo((object) other.Type);
    switch (this.Type)
    {
      case BsonType.MinValue:
      case BsonType.Null:
      case BsonType.MaxValue:
        return 0;
      case BsonType.Int32:
        return ((int) this.RawValue).CompareTo((int) other.RawValue);
      case BsonType.Int64:
        return ((long) this.RawValue).CompareTo((long) other.RawValue);
      case BsonType.Double:
        return ((double) this.RawValue).CompareTo((double) other.RawValue);
      case BsonType.Decimal:
        return ((Decimal) this.RawValue).CompareTo((Decimal) other.RawValue);
      case BsonType.String:
        return string.Compare((string) this.RawValue, (string) other.RawValue);
      case BsonType.Document:
        return this.AsDocument.CompareTo(other);
      case BsonType.Array:
        return this.AsArray.CompareTo(other);
      case BsonType.Binary:
        return ((byte[]) this.RawValue).BinaryCompareTo((byte[]) other.RawValue);
      case BsonType.ObjectId:
        return ((ObjectId) this.RawValue).CompareTo((ObjectId) other.RawValue);
      case BsonType.Guid:
        return ((Guid) this.RawValue).CompareTo((Guid) other.RawValue);
      case BsonType.Boolean:
        return ((bool) this.RawValue).CompareTo((bool) other.RawValue);
      case BsonType.DateTime:
        DateTime dateTime1 = (DateTime) this.RawValue;
        DateTime dateTime2 = (DateTime) other.RawValue;
        if (dateTime1.Kind != DateTimeKind.Utc)
          dateTime1 = dateTime1.ToUniversalTime();
        if (dateTime2.Kind != DateTimeKind.Utc)
          dateTime2 = dateTime2.ToUniversalTime();
        return dateTime1.CompareTo(dateTime2);
      default:
        throw new NotImplementedException();
    }
  }

  public bool Equals(BsonValue other) => this.CompareTo(other) == 0;

  public static bool operator ==(BsonValue lhs, BsonValue rhs)
  {
    if ((object) lhs == null)
      return (object) rhs == null;
    return (object) rhs != null && lhs.Equals(rhs);
  }

  public static bool operator !=(BsonValue lhs, BsonValue rhs) => !(lhs == rhs);

  public static bool operator >=(BsonValue lhs, BsonValue rhs) => lhs.CompareTo(rhs) >= 0;

  public static bool operator >(BsonValue lhs, BsonValue rhs) => lhs.CompareTo(rhs) > 0;

  public static bool operator <(BsonValue lhs, BsonValue rhs) => lhs.CompareTo(rhs) < 0;

  public static bool operator <=(BsonValue lhs, BsonValue rhs) => lhs.CompareTo(rhs) <= 0;

  public override bool Equals(object obj) => this.Equals(new BsonValue(obj));

  public override int GetHashCode()
  {
    return 37 * (37 * 17 + this.Type.GetHashCode()) + this.RawValue.GetHashCode();
  }

  public int GetBytesCount(bool recalc)
  {
    if (!recalc && this.Length.HasValue)
      return this.Length.Value;
    switch (this.Type)
    {
      case BsonType.MinValue:
      case BsonType.Null:
      case BsonType.MaxValue:
        this.Length = new int?(0);
        break;
      case BsonType.Int32:
        this.Length = new int?(4);
        break;
      case BsonType.Int64:
        this.Length = new int?(8);
        break;
      case BsonType.Double:
        this.Length = new int?(8);
        break;
      case BsonType.Decimal:
        this.Length = new int?(16 /*0x10*/);
        break;
      case BsonType.String:
        this.Length = new int?(Encoding.UTF8.GetByteCount((string) this.RawValue));
        break;
      case BsonType.Document:
        Dictionary<string, BsonValue> rawValue1 = (Dictionary<string, BsonValue>) this.RawValue;
        this.Length = new int?(5);
        using (Dictionary<string, BsonValue>.KeyCollection.Enumerator enumerator = rawValue1.Keys.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            string current = enumerator.Current;
            int? length = this.Length;
            string key = current;
            BsonValue bsonValue = rawValue1[current];
            if ((object) bsonValue == null)
              bsonValue = BsonValue.Null;
            int num = recalc ? 1 : 0;
            int bytesCountElement = this.GetBytesCountElement(key, bsonValue, num != 0);
            this.Length = length.HasValue ? new int?(length.GetValueOrDefault() + bytesCountElement) : new int?();
          }
          break;
        }
      case BsonType.Array:
        List<BsonValue> rawValue2 = (List<BsonValue>) this.RawValue;
        this.Length = new int?(5);
        for (int index = 0; index < rawValue2.Count; ++index)
        {
          int? length = this.Length;
          string key = index.ToString();
          BsonValue bsonValue = rawValue2[index];
          if ((object) bsonValue == null)
            bsonValue = BsonValue.Null;
          int num = recalc ? 1 : 0;
          int bytesCountElement = this.GetBytesCountElement(key, bsonValue, num != 0);
          this.Length = length.HasValue ? new int?(length.GetValueOrDefault() + bytesCountElement) : new int?();
        }
        break;
      case BsonType.Binary:
        this.Length = new int?(((byte[]) this.RawValue).Length);
        break;
      case BsonType.ObjectId:
        this.Length = new int?(12);
        break;
      case BsonType.Guid:
        this.Length = new int?(16 /*0x10*/);
        break;
      case BsonType.Boolean:
        this.Length = new int?(1);
        break;
      case BsonType.DateTime:
        this.Length = new int?(8);
        break;
    }
    return this.Length.Value;
  }

  private int GetBytesCountElement(string key, BsonValue value, bool recalc)
  {
    return 1 + Encoding.UTF8.GetByteCount(key) + 1 + value.GetBytesCount(recalc) + (value.Type == BsonType.String || value.Type == BsonType.Binary || value.Type == BsonType.Guid ? 5 : 0);
  }
}
