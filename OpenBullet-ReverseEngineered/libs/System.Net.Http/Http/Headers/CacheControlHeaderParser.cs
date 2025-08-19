// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.CacheControlHeaderParser
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

#nullable disable
namespace System.Net.Http.Headers;

internal class CacheControlHeaderParser : BaseHeaderParser
{
  internal static readonly CacheControlHeaderParser Parser = new CacheControlHeaderParser();

  private CacheControlHeaderParser()
    : base(true)
  {
  }

  protected override int GetParsedValueLength(
    string value,
    int startIndex,
    object storeValue,
    out object parsedValue)
  {
    CacheControlHeaderValue parsedValue1 = storeValue as CacheControlHeaderValue;
    int cacheControlLength = CacheControlHeaderValue.GetCacheControlLength(value, startIndex, parsedValue1, out parsedValue1);
    parsedValue = (object) parsedValue1;
    return cacheControlLength;
  }
}
