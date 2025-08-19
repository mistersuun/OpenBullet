// Decompiled with JetBrains decompiler
// Type: LiteDB.JsonReader
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

#nullable disable
namespace LiteDB;

internal class JsonReader
{
  private JsonTokenizer _tokenizer;

  public long Position => this._tokenizer.Position;

  public JsonReader(TextReader reader)
  {
    this._tokenizer = reader != null ? new JsonTokenizer(reader) : throw new ArgumentNullException(nameof (reader));
  }

  public BsonValue Deserialize()
  {
    JsonToken token = this._tokenizer.ReadToken();
    return token.TokenType == JsonTokenType.EOF ? BsonValue.Null : this.ReadValue(token);
  }

  public IEnumerable<BsonValue> DeserializeArray()
  {
    JsonToken jsonToken = this._tokenizer.ReadToken();
    if (jsonToken.TokenType != JsonTokenType.EOF)
    {
      jsonToken.Expect(JsonTokenType.BeginArray);
      JsonToken token = this._tokenizer.ReadToken();
      while (token.TokenType != JsonTokenType.EndArray)
      {
        yield return this.ReadValue(token);
        token = this._tokenizer.ReadToken();
        if (token.TokenType == JsonTokenType.Comma)
          token = this._tokenizer.ReadToken();
      }
      token.Expect(JsonTokenType.EndArray);
    }
  }

  internal BsonValue ReadValue(JsonToken token)
  {
    switch (token.TokenType)
    {
      case JsonTokenType.BeginDoc:
        return this.ReadObject();
      case JsonTokenType.BeginArray:
        return (BsonValue) this.ReadArray();
      case JsonTokenType.String:
        return (BsonValue) token.Token;
      case JsonTokenType.Number:
        return !token.Token.Contains(".") ? new BsonValue(Convert.ToInt32(token.Token)) : new BsonValue(Convert.ToDouble(token.Token, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat));
      case JsonTokenType.Word:
        switch (token.Token)
        {
          case "null":
            return BsonValue.Null;
          case "true":
            return (BsonValue) true;
          case "false":
            return (BsonValue) false;
          default:
            throw LiteException.UnexpectedToken(token.Token);
        }
      default:
        throw LiteException.UnexpectedToken(token.Token);
    }
  }

  private BsonValue ReadObject()
  {
    BsonDocument bsonDocument = new BsonDocument();
    JsonToken jsonToken = this._tokenizer.ReadToken();
    while (jsonToken.TokenType != JsonTokenType.EndDoc)
    {
      jsonToken.Expect(JsonTokenType.String, JsonTokenType.Word);
      string token1 = jsonToken.Token;
      this._tokenizer.ReadToken().Expect(JsonTokenType.Colon);
      JsonToken token2 = this._tokenizer.ReadToken();
      if (token1[0] == '$' && bsonDocument.Count == 0)
      {
        BsonValue bsonValue = this.ReadExtendedDataType(token1, token2.Token);
        if (!bsonValue.IsNull)
          return bsonValue;
      }
      bsonDocument[token1] = this.ReadValue(token2);
      jsonToken = this._tokenizer.ReadToken();
      if (jsonToken.TokenType == JsonTokenType.Comma)
        jsonToken = this._tokenizer.ReadToken();
    }
    return (BsonValue) bsonDocument;
  }

  private BsonArray ReadArray()
  {
    BsonArray bsonArray = new BsonArray();
    JsonToken token = this._tokenizer.ReadToken();
    while (token.TokenType != JsonTokenType.EndArray)
    {
      BsonValue bsonValue = this.ReadValue(token);
      bsonArray.Add(bsonValue);
      token = this._tokenizer.ReadToken();
      if (token.TokenType == JsonTokenType.Comma)
        token = this._tokenizer.ReadToken();
    }
    return bsonArray;
  }

  private BsonValue ReadExtendedDataType(string key, string value)
  {
    BsonValue bsonValue;
    switch (key)
    {
      case "$binary":
        bsonValue = new BsonValue(Convert.FromBase64String(value));
        break;
      case "$date":
        bsonValue = new BsonValue(DateTime.Parse(value).ToLocalTime());
        break;
      case "$guid":
        bsonValue = new BsonValue(new Guid(value));
        break;
      case "$maxValue":
        bsonValue = BsonValue.MaxValue;
        break;
      case "$minValue":
        bsonValue = BsonValue.MinValue;
        break;
      case "$numberDecimal":
        bsonValue = new BsonValue(Convert.ToDecimal(value));
        break;
      case "$numberLong":
        bsonValue = new BsonValue(Convert.ToInt64(value));
        break;
      case "$oid":
        bsonValue = new BsonValue(new ObjectId(value));
        break;
      default:
        return BsonValue.Null;
    }
    this._tokenizer.ReadToken().Expect(JsonTokenType.EndDoc);
    return bsonValue;
  }
}
