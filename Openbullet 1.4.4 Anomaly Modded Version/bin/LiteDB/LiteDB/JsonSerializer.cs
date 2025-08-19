// Decompiled with JetBrains decompiler
// Type: LiteDB.JsonSerializer
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#nullable disable
namespace LiteDB;

public class JsonSerializer
{
  public static string Serialize(BsonValue value, bool pretty = false, bool writeBinary = true)
  {
    StringBuilder sb = new StringBuilder();
    using (StringWriter writer = new StringWriter(sb))
    {
      BsonValue bsonValue = value;
      if ((object) bsonValue == null)
        bsonValue = BsonValue.Null;
      JsonSerializer.Serialize(bsonValue, (TextWriter) writer, pretty, writeBinary);
    }
    return sb.ToString();
  }

  public static void Serialize(BsonValue value, TextWriter writer, bool pretty = false, bool writeBinary = true)
  {
    JsonWriter jsonWriter = new JsonWriter(writer);
    jsonWriter.Pretty = pretty;
    jsonWriter.WriteBinary = writeBinary;
    BsonValue bsonValue = value;
    if ((object) bsonValue == null)
      bsonValue = BsonValue.Null;
    jsonWriter.Serialize(bsonValue);
  }

  public static BsonValue Deserialize(string json)
  {
    if (json == null)
      throw new ArgumentNullException(nameof (json));
    using (StringReader reader = new StringReader(json))
      return new JsonReader((TextReader) reader).Deserialize();
  }

  public static BsonValue Deserialize(TextReader reader)
  {
    return reader != null ? new JsonReader(reader).Deserialize() : throw new ArgumentNullException(nameof (reader));
  }

  public static BsonValue Deserialize(StringScanner s)
  {
    if (s == null)
      throw new ArgumentNullException(nameof (s));
    if (s.HasTerminated)
      return BsonValue.Null;
    using (StringReader reader = new StringReader(s.ToString()))
    {
      JsonReader jsonReader = new JsonReader((TextReader) reader);
      BsonValue bsonValue = jsonReader.Deserialize();
      s.Seek((int) (jsonReader.Position - 1L));
      return bsonValue;
    }
  }

  public static IEnumerable<BsonValue> DeserializeArray(string json)
  {
    return json != null ? new JsonReader((TextReader) new StringReader(json)).DeserializeArray() : throw new ArgumentNullException(nameof (json));
  }

  public static IEnumerable<BsonValue> DeserializeArray(TextReader reader)
  {
    return reader != null ? new JsonReader(reader).DeserializeArray() : throw new ArgumentNullException(nameof (reader));
  }
}
