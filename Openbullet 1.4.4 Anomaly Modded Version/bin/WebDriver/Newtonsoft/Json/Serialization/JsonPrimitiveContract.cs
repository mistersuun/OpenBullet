// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.JsonPrimitiveContract
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Serialization;

internal class JsonPrimitiveContract : JsonContract
{
  private static readonly Dictionary<Type, ReadType> ReadTypeMap = new Dictionary<Type, ReadType>()
  {
    [typeof (byte[])] = ReadType.ReadAsBytes,
    [typeof (byte)] = ReadType.ReadAsInt32,
    [typeof (short)] = ReadType.ReadAsInt32,
    [typeof (int)] = ReadType.ReadAsInt32,
    [typeof (Decimal)] = ReadType.ReadAsDecimal,
    [typeof (bool)] = ReadType.ReadAsBoolean,
    [typeof (string)] = ReadType.ReadAsString,
    [typeof (DateTime)] = ReadType.ReadAsDateTime,
    [typeof (DateTimeOffset)] = ReadType.ReadAsDateTimeOffset,
    [typeof (float)] = ReadType.ReadAsDouble,
    [typeof (double)] = ReadType.ReadAsDouble
  };

  internal PrimitiveTypeCode TypeCode { get; set; }

  public JsonPrimitiveContract(Type underlyingType)
    : base(underlyingType)
  {
    this.ContractType = JsonContractType.Primitive;
    this.TypeCode = ConvertUtils.GetTypeCode(underlyingType);
    this.IsReadOnlyOrFixedSize = true;
    ReadType readType;
    if (!JsonPrimitiveContract.ReadTypeMap.TryGetValue(this.NonNullableUnderlyingType, out readType))
      return;
    this.InternalReadType = readType;
  }
}
