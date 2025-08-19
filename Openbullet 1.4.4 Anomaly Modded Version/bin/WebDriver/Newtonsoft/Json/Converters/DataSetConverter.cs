// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.DataSetConverter
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Serialization;
using System;
using System.Data;

#nullable disable
namespace Newtonsoft.Json.Converters;

internal class DataSetConverter : JsonConverter
{
  public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
  {
    DataSet dataSet = (DataSet) value;
    DefaultContractResolver contractResolver = serializer.ContractResolver as DefaultContractResolver;
    DataTableConverter dataTableConverter = new DataTableConverter();
    writer.WriteStartObject();
    foreach (DataTable table in (InternalDataCollectionBase) dataSet.Tables)
    {
      writer.WritePropertyName(contractResolver != null ? contractResolver.GetResolvedPropertyName(table.TableName) : table.TableName);
      dataTableConverter.WriteJson(writer, (object) table, serializer);
    }
    writer.WriteEndObject();
  }

  public override object ReadJson(
    JsonReader reader,
    Type objectType,
    object existingValue,
    JsonSerializer serializer)
  {
    if (reader.TokenType == JsonToken.Null)
      return (object) null;
    DataSet dataSet = objectType == typeof (DataSet) ? new DataSet() : (DataSet) Activator.CreateInstance(objectType);
    DataTableConverter dataTableConverter = new DataTableConverter();
    reader.ReadAndAssert();
    while (reader.TokenType == JsonToken.PropertyName)
    {
      DataTable table1 = dataSet.Tables[(string) reader.Value];
      int num = table1 != null ? 1 : 0;
      DataTable table2 = (DataTable) dataTableConverter.ReadJson(reader, typeof (DataTable), (object) table1, serializer);
      if (num == 0)
        dataSet.Tables.Add(table2);
      reader.ReadAndAssert();
    }
    return (object) dataSet;
  }

  public override bool CanConvert(Type valueType) => typeof (DataSet).IsAssignableFrom(valueType);
}
