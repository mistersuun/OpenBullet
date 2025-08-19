// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JRaw
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Newtonsoft.Json.Linq;

internal class JRaw : JValue
{
  public static async Task<JRaw> CreateAsync(JsonReader reader, CancellationToken cancellationToken = default (CancellationToken))
  {
    JRaw async;
    using (StringWriter sw = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
    {
      using (JsonTextWriter jsonWriter = new JsonTextWriter((TextWriter) sw))
      {
        await jsonWriter.WriteTokenSyncReadingAsync(reader, cancellationToken).ConfigureAwait(false);
        async = new JRaw((object) sw.ToString());
      }
    }
    return async;
  }

  public JRaw(JRaw other)
    : base((JValue) other)
  {
  }

  public JRaw(object rawJson)
    : base(rawJson, JTokenType.Raw)
  {
  }

  public static JRaw Create(JsonReader reader)
  {
    using (StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
    {
      using (JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter) stringWriter))
      {
        jsonTextWriter.WriteToken(reader);
        return new JRaw((object) stringWriter.ToString());
      }
    }
  }

  internal override JToken CloneToken() => (JToken) new JRaw(this);
}
