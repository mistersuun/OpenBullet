// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonPath.QueryOperator
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace Newtonsoft.Json.Linq.JsonPath;

internal enum QueryOperator
{
  None,
  Equals,
  NotEquals,
  Exists,
  LessThan,
  LessThanOrEquals,
  GreaterThan,
  GreaterThanOrEquals,
  And,
  Or,
}
