// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.CommandInfo
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class CommandInfo
{
  public const string PostCommand = "POST";
  public const string GetCommand = "GET";
  public const string DeleteCommand = "DELETE";
  private const string SessionIdPropertyName = "sessionId";
  private string resourcePath;
  private string method;

  public CommandInfo(string method, string resourcePath)
  {
    this.resourcePath = resourcePath;
    this.method = method;
  }

  public string ResourcePath => this.resourcePath;

  public string Method => this.method;

  public Uri CreateCommandUri(Uri baseUri, Command commandToExecute)
  {
    string[] strArray = this.resourcePath.Split(new string[1]
    {
      "/"
    }, StringSplitOptions.RemoveEmptyEntries);
    for (int index = 0; index < strArray.Length; ++index)
    {
      string propertyName = strArray[index];
      if (propertyName.StartsWith("{", StringComparison.OrdinalIgnoreCase) && propertyName.EndsWith("}", StringComparison.OrdinalIgnoreCase))
        strArray[index] = CommandInfo.GetCommandPropertyValue(propertyName, commandToExecute);
    }
    string uriString = string.Join("/", strArray);
    Uri relativeUri = new Uri(uriString, UriKind.Relative);
    Uri result;
    if (!Uri.TryCreate(baseUri, relativeUri, out result))
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to create URI from base {0} and relative path {1}", baseUri == (Uri) null ? (object) string.Empty : (object) baseUri.ToString(), (object) uriString));
    return result;
  }

  private static string GetCommandPropertyValue(string propertyName, Command commandToExecute)
  {
    string empty = string.Empty;
    propertyName = propertyName.Substring(1, propertyName.Length - 2);
    if (propertyName == "sessionId")
    {
      if (commandToExecute.SessionId != null)
        empty = commandToExecute.SessionId.ToString();
    }
    else if (commandToExecute.Parameters != null && commandToExecute.Parameters.Count > 0 && commandToExecute.Parameters.ContainsKey(propertyName) && commandToExecute.Parameters[propertyName] != null)
    {
      empty = commandToExecute.Parameters[propertyName].ToString();
      commandToExecute.Parameters.Remove(propertyName);
    }
    return empty;
  }
}
