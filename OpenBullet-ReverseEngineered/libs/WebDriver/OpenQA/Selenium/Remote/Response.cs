// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.Response
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class Response
{
  private object responseValue;
  private string responseSessionId;
  private WebDriverResult responseStatus;
  private bool isSpecificationCompliant;

  public Response()
  {
  }

  public Response(OpenQA.Selenium.Remote.SessionId sessionId)
  {
    if (sessionId == null)
      return;
    this.responseSessionId = sessionId.ToString();
  }

  private Response(Dictionary<string, object> rawResponse)
  {
    if (rawResponse.ContainsKey("sessionId") && rawResponse["sessionId"] != null)
      this.responseSessionId = rawResponse["sessionId"].ToString();
    if (rawResponse.ContainsKey("value"))
      this.responseValue = rawResponse["value"];
    if (rawResponse.ContainsKey("status"))
    {
      this.responseStatus = (WebDriverResult) Convert.ToInt32(rawResponse["status"], (IFormatProvider) CultureInfo.InvariantCulture);
    }
    else
    {
      this.isSpecificationCompliant = true;
      if (!rawResponse.ContainsKey("value") && this.responseValue == null)
        this.responseValue = !rawResponse.ContainsKey("capabilities") ? (object) rawResponse : rawResponse["capabilities"];
      if (!(this.responseValue is Dictionary<string, object> responseValue))
        return;
      if (responseValue.ContainsKey("sessionId"))
      {
        this.responseSessionId = responseValue["sessionId"].ToString();
        if (responseValue.ContainsKey("capabilities"))
          this.responseValue = responseValue["capabilities"];
        else
          this.responseValue = responseValue["value"];
      }
      else
      {
        if (!responseValue.ContainsKey("error"))
          return;
        this.responseStatus = WebDriverError.ResultFromError(responseValue["error"].ToString());
      }
    }
  }

  public object Value
  {
    get => this.responseValue;
    set => this.responseValue = value;
  }

  public string SessionId
  {
    get => this.responseSessionId;
    set => this.responseSessionId = value;
  }

  public WebDriverResult Status
  {
    get => this.responseStatus;
    set => this.responseStatus = value;
  }

  public bool IsSpecificationCompliant => this.isSpecificationCompliant;

  public static Response FromJson(string value)
  {
    return new Response(JsonConvert.DeserializeObject<Dictionary<string, object>>(value, (JsonConverter) new ResponseValueJsonConverter()));
  }

  public string ToJson() => JsonConvert.SerializeObject((object) this);

  public override string ToString()
  {
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0} {1}: {2})", (object) this.SessionId, (object) this.Status, this.Value);
  }
}
