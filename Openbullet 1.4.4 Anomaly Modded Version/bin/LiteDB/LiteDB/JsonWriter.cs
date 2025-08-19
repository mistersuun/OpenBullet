// Decompiled with JetBrains decompiler
// Type: LiteDB.JsonWriter
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

#nullable disable
namespace LiteDB;

internal class JsonWriter
{
  private const int INDENT_SIZE = 4;
  private TextWriter _writer;
  private int _indent;
  private string _spacer = "";

  public bool Pretty { get; set; }

  public bool WriteBinary { get; set; }

  public JsonWriter(TextWriter writer) => this._writer = writer;

  public void Serialize(BsonValue value)
  {
    this._indent = 0;
    this._spacer = this.Pretty ? " " : "";
    BsonValue bsonValue = value;
    if ((object) bsonValue == null)
      bsonValue = BsonValue.Null;
    this.WriteValue(bsonValue);
  }

  private void WriteValue(BsonValue value)
  {
    switch (value.Type)
    {
      case BsonType.MinValue:
        this.WriteExtendDataType("$minValue", "1");
        break;
      case BsonType.Null:
        this._writer.Write("null");
        break;
      case BsonType.Int32:
        this._writer.Write((int) value.RawValue);
        break;
      case BsonType.Int64:
        this.WriteExtendDataType("$numberLong", ((long) value.RawValue).ToString());
        break;
      case BsonType.Double:
        this._writer.Write(((double) value.RawValue).ToString("0.0########", (IFormatProvider) NumberFormatInfo.InvariantInfo));
        break;
      case BsonType.Decimal:
        this.WriteExtendDataType("$numberDecimal", ((Decimal) value.RawValue).ToString());
        break;
      case BsonType.String:
        this.WriteString((string) value.RawValue);
        break;
      case BsonType.Document:
        this.WriteObject(new BsonDocument((Dictionary<string, BsonValue>) value.RawValue));
        break;
      case BsonType.Array:
        this.WriteArray(new BsonArray((List<BsonValue>) value.RawValue));
        break;
      case BsonType.Binary:
        byte[] rawValue = (byte[]) value.RawValue;
        this.WriteExtendDataType("$binary", this.WriteBinary ? Convert.ToBase64String(rawValue, 0, rawValue.Length) : $"-- {(object) rawValue.Length} bytes --");
        break;
      case BsonType.ObjectId:
        this.WriteExtendDataType("$oid", ((ObjectId) value.RawValue).ToString());
        break;
      case BsonType.Guid:
        this.WriteExtendDataType("$guid", ((Guid) value.RawValue).ToString());
        break;
      case BsonType.Boolean:
        this._writer.Write(((bool) value.RawValue).ToString().ToLower());
        break;
      case BsonType.DateTime:
        DateTime dateTime = (DateTime) value.RawValue;
        dateTime = dateTime.ToUniversalTime();
        this.WriteExtendDataType("$date", dateTime.ToString("o"));
        break;
      case BsonType.MaxValue:
        this.WriteExtendDataType("$maxValue", "1");
        break;
    }
  }

  private void WriteObject(BsonDocument obj)
  {
    int num1 = obj.Keys.Count<string>();
    bool hasData = num1 > 0;
    this.WriteStartBlock("{", hasData);
    int num2 = 0;
    foreach (string key in (IEnumerable<string>) obj.Keys)
      this.WriteKeyValue(key, obj[key], num2++ < num1 - 1);
    this.WriteEndBlock("}", hasData);
  }

  private void WriteArray(BsonArray arr)
  {
    bool hasData = arr.Count > 0;
    this.WriteStartBlock("[", hasData);
    for (int index = 0; index < arr.Count; ++index)
    {
      BsonValue bsonValue1 = arr[index];
      if (this.Pretty && (!bsonValue1.IsDocument || !bsonValue1.AsDocument.Keys.Any<string>()) && (!bsonValue1.IsArray || bsonValue1.AsArray.Count <= 0))
        this.WriteIndent();
      BsonValue bsonValue2 = bsonValue1;
      if ((object) bsonValue2 == null)
        bsonValue2 = BsonValue.Null;
      this.WriteValue(bsonValue2);
      if (index < arr.Count - 1)
        this._writer.Write(',');
      this.WriteNewLine();
    }
    this.WriteEndBlock("]", hasData);
  }

  private void WriteString(string s)
  {
    this._writer.Write('"');
    int length = s.Length;
    for (int index = 0; index < length; ++index)
    {
      char ch = s[index];
      switch (ch)
      {
        case '\b':
          this._writer.Write("\\b");
          break;
        case '\t':
          this._writer.Write("\\t");
          break;
        case '\n':
          this._writer.Write("\\n");
          break;
        case '\f':
          this._writer.Write("\\f");
          break;
        case '\r':
          this._writer.Write("\\r");
          break;
        case '"':
          this._writer.Write("\\\"");
          break;
        case '\\':
          this._writer.Write("\\\\");
          break;
        default:
          int num = (int) ch;
          if (num < 32 /*0x20*/ || num > (int) sbyte.MaxValue)
          {
            this._writer.Write("\\u");
            this._writer.Write(num.ToString("x04"));
            break;
          }
          this._writer.Write(ch);
          break;
      }
    }
    this._writer.Write('"');
  }

  private void WriteExtendDataType(string type, string value)
  {
    this._writer.Write("{\"");
    this._writer.Write(type);
    this._writer.Write("\":");
    this._writer.Write(this._spacer);
    this._writer.Write("\"");
    this._writer.Write(value);
    this._writer.Write("\"}");
  }

  private void WriteKeyValue(string key, BsonValue value, bool comma)
  {
    this.WriteIndent();
    this._writer.Write('"');
    this._writer.Write(key);
    this._writer.Write("\":");
    if (this.Pretty)
    {
      this._writer.Write(' ');
      if (value.IsDocument && value.AsDocument.Keys.Any<string>() || value.IsArray && value.AsArray.Count > 0)
        this.WriteNewLine();
    }
    BsonValue bsonValue = value;
    if ((object) bsonValue == null)
      bsonValue = BsonValue.Null;
    this.WriteValue(bsonValue);
    if (comma)
      this._writer.Write(',');
    this.WriteNewLine();
  }

  private void WriteStartBlock(string str, bool hasData)
  {
    if (hasData)
    {
      this.WriteIndent();
      this._writer.Write(str);
      this.WriteNewLine();
      ++this._indent;
    }
    else
      this._writer.Write(str);
  }

  private void WriteEndBlock(string str, bool hasData)
  {
    if (hasData)
    {
      --this._indent;
      this.WriteIndent();
      this._writer.Write(str);
    }
    else
      this._writer.Write(str);
  }

  private void WriteNewLine()
  {
    if (!this.Pretty)
      return;
    this._writer.WriteLine();
  }

  private void WriteIndent()
  {
    if (!this.Pretty)
      return;
    this._writer.Write("".PadRight(this._indent * 4, ' '));
  }
}
