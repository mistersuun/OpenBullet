// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.Command
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class Command
{
  private SessionId commandSessionId;
  private string commandName;
  private Dictionary<string, object> commandParameters = new Dictionary<string, object>();

  public Command(string name, string jsonParameters)
    : this((SessionId) null, name, Command.ConvertParametersFromJson(jsonParameters))
  {
  }

  public Command(SessionId sessionId, string name, Dictionary<string, object> parameters)
  {
    this.commandSessionId = sessionId;
    if (parameters != null)
      this.commandParameters = parameters;
    this.commandName = name;
  }

  [JsonProperty("sessionId")]
  public SessionId SessionId => this.commandSessionId;

  [JsonProperty("name")]
  public string Name => this.commandName;

  [JsonProperty("parameters")]
  public Dictionary<string, object> Parameters => this.commandParameters;

  public string ParametersAsJsonString
  {
    get
    {
      string parametersAsJsonString = string.Empty;
      if (this.commandParameters != null && this.commandParameters.Count > 0)
        parametersAsJsonString = JsonConvert.SerializeObject((object) this.commandParameters);
      if (string.IsNullOrEmpty(parametersAsJsonString))
        parametersAsJsonString = "{}";
      return parametersAsJsonString;
    }
  }

  public override string ToString()
  {
    return $"[{(object) this.SessionId}]: {this.Name} {this.Parameters.ToString()}";
  }

  private static Dictionary<string, object> ConvertParametersFromJson(string value)
  {
    return JsonConvert.DeserializeObject<Dictionary<string, object>>(value, (JsonConverter) new ResponseValueJsonConverter());
  }
}
