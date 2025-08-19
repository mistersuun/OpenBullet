// Decompiled with JetBrains decompiler
// Type: RuriLib.Functions.Requests.Request
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Extreme.Net;
using RuriLib.Functions.Formats;
using RuriLib.Models;
using RuriLib.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;

#nullable disable
namespace RuriLib.Functions.Requests;

public class Request
{
  private HttpRequest request = new HttpRequest();
  private HttpContent content;
  private Dictionary<string, string> oldCookies = new Dictionary<string, string>();
  private int timeout = 60000;
  private string url = "";
  private string contentType = "";
  private string authorization = "";
  private HttpResponse response;
  private bool hasContentLength = true;
  private bool isGZipped;

  public Request Setup(
    RLSettingsViewModel settings,
    bool autoRedirect = true,
    int maxRedirects = 8,
    bool acceptEncoding = true)
  {
    this.timeout = settings.General.RequestTimeout * 1000;
    this.request.IgnoreProtocolErrors = true;
    this.request.AllowAutoRedirect = autoRedirect;
    this.request.EnableEncodingContent = acceptEncoding;
    this.request.ReadWriteTimeout = this.timeout;
    this.request.ConnectTimeout = this.timeout;
    this.request.KeepAlive = true;
    this.request.MaximumAutomaticRedirections = maxRedirects;
    return this;
  }

  public Request SetStandardContent(
    string postData,
    string contentType,
    HttpMethod method = HttpMethod.POST,
    bool encodeContent = false,
    List<LogEntry> log = null)
  {
    this.contentType = contentType;
    string content = Regex.Replace(postData, "(?<!\\\\)\\\\n", Environment.NewLine).Replace("\\\\n", "\\n");
    if (Request.CanContainBody(method))
    {
      if (encodeContent)
      {
        int num = new Random().Next(1000000, 9999999);
        content = string.Join("", ((IEnumerable<string>) BlockFunction.SplitInChunks(content.Replace("&", $"{num}&{num}").Replace("=", $"{num}={num}"), 2080)).Select<string, string>((Func<string, string>) (s => Uri.EscapeDataString(s)))).Replace($"{num}%26{num}", "&").Replace($"{num}%3D{num}", "=");
      }
      this.content = (HttpContent) new StringContent(content);
      this.content.ContentType = contentType;
      log?.Add(new LogEntry("Post Data: " + content, Colors.MediumTurquoise));
    }
    return this;
  }

  public Request SetBasicAuth(string username, string password)
  {
    this.authorization = "Basic " + $"{username}:{password}".ToBase64();
    return this;
  }

  public Request SetMultipartContent(
    IEnumerable<MultipartContent> contents,
    string boundary = "",
    List<LogEntry> log = null)
  {
    string str = boundary != "" ? boundary : Request.GenerateMultipartBoundary();
    this.content = (HttpContent) new Extreme.Net.MultipartContent(str);
    Extreme.Net.MultipartContent content1 = this.content as Extreme.Net.MultipartContent;
    if (log != null)
    {
      log.Add(new LogEntry("Content-Type: multipart/form-data; boundary=" + str, Colors.MediumTurquoise));
      log.Add(new LogEntry("Multipart Data:", Colors.MediumTurquoise));
      log.Add(new LogEntry(str, Colors.MediumTurquoise));
    }
    foreach (MultipartContent content2 in contents)
    {
      if (content2.Type == MultipartContentType.String)
      {
        content1.Add((HttpContent) new StringContent(content2.Value), content2.Name);
        log?.Add(new LogEntry($"Content-Disposition: form-data; name=\"{content2.Name}\"{Environment.NewLine}{Environment.NewLine}{content2.Value}", Colors.MediumTurquoise));
      }
      else if (content2.Type == MultipartContentType.File)
      {
        content1.Add((HttpContent) new FileContent(content2.Value), content2.Name, content2.Value, content2.ContentType);
        log?.Add(new LogEntry($"Content-Disposition: form-data; name=\"{content2.Name}\"; filename=\"{content2.Value}\"{Environment.NewLine}Content-Type: {content2.ContentType}{Environment.NewLine}{Environment.NewLine}[FILE CONTENT OMITTED]", Colors.MediumTurquoise));
      }
      log?.Add(new LogEntry(str, Colors.MediumTurquoise));
    }
    return this;
  }

  public Request SetProxy(CProxy proxy)
  {
    this.request.Proxy = proxy.GetClient();
    this.request.Proxy.ReadWriteTimeout = this.timeout;
    this.request.Proxy.ConnectTimeout = this.timeout;
    this.request.Proxy.Username = proxy.Username;
    this.request.Proxy.Password = proxy.Password;
    return this;
  }

  public Request SetCookies(Dictionary<string, string> cookies, List<LogEntry> log = null)
  {
    this.oldCookies = cookies;
    this.request.Cookies = new CookieDictionary();
    foreach (KeyValuePair<string, string> cookie in cookies)
    {
      this.request.Cookies.Add(cookie.Key, cookie.Value);
      log?.Add(new LogEntry($"{cookie.Key}: {cookie.Value}", Colors.MediumTurquoise));
    }
    return this;
  }

  public Request SetHeaders(
    Dictionary<string, string> headers,
    bool acceptEncoding = true,
    List<LogEntry> log = null)
  {
    foreach (KeyValuePair<string, string> header in headers)
    {
      try
      {
        string lower = header.Key.Replace("-", "").ToLower();
        if (lower == "contenttype")
        {
          if (this.content != null)
            continue;
        }
        if (!(lower == "acceptencoding" & acceptEncoding))
        {
          this.request.AddHeader(header.Key, header.Value);
          log?.Add(new LogEntry($"{header.Key}: {header.Value}", Colors.MediumTurquoise));
        }
      }
      catch
      {
      }
    }
    if (this.authorization != "")
    {
      this.request.AddHeader("Authorization", this.authorization);
      log?.Add(new LogEntry("Authorization: " + this.authorization, Colors.MediumTurquoise));
    }
    if (this.contentType != "" && log != null)
      log.Add(new LogEntry("Content-Type: " + this.contentType, Colors.MediumTurquoise));
    return this;
  }

  public (string, string, Dictionary<string, string>, Dictionary<string, string>) Perform(
    string url,
    HttpMethod method,
    bool ignoreErrors = false,
    List<LogEntry> log = null)
  {
    string str1 = "";
    string str2 = "0";
    Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
    Dictionary<string, string> dictionary2 = this.oldCookies;
    try
    {
      this.response = this.request.Raw(method, url, this.content);
      str1 = this.response.Address.ToString();
      log?.Add(new LogEntry("Address: " + str1, Colors.Cyan));
      str2 = ((int) this.response.StatusCode).ToString();
      log?.Add(new LogEntry($"Response code: {str2} ({this.response.StatusCode})", Colors.Cyan));
      log?.Add(new LogEntry("Received headers:", Colors.DeepPink));
      List<KeyValuePair<string, string>> source = new List<KeyValuePair<string, string>>();
      Dictionary<string, string>.Enumerator enumerator = this.response.EnumerateHeaders();
      while (enumerator.MoveNext())
      {
        KeyValuePair<string, string> current = enumerator.Current;
        source.Add(new KeyValuePair<string, string>(current.Key, current.Value));
        log?.Add(new LogEntry($"{current.Key}: {current.Value}", Colors.LightPink));
      }
      dictionary1 = source.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (h => h.Key), (Func<KeyValuePair<string, string>, string>) (h => h.Value));
      this.hasContentLength = dictionary1.ContainsKey("Content-Length");
      this.isGZipped = dictionary1.ContainsKey("Content-Encoding") && dictionary1["Content-Encoding"].Contains("gzip");
      dictionary2 = (Dictionary<string, string>) this.response.Cookies;
      if (log != null)
      {
        log.Add(new LogEntry("Received cookies:", Colors.Goldenrod));
        foreach (KeyValuePair<string, string> cookie in (Dictionary<string, string>) this.response.Cookies)
        {
          if (!this.oldCookies.ContainsKey(cookie.Key) || !(this.oldCookies[cookie.Key] == cookie.Value))
            log.Add(new LogEntry($"{cookie.Key}: {cookie.Value}", Colors.LightGoldenrodYellow));
        }
      }
    }
    catch (Exception ex)
    {
      log?.Add(new LogEntry(ex.Message, Colors.White));
      if (ex.GetType() == typeof (HttpException))
      {
        str2 = ((HttpException) ex).HttpStatusCode.ToString();
        log?.Add(new LogEntry("Status code: " + str2, Colors.Cyan));
      }
      if (!ignoreErrors)
        throw;
    }
    return (str1, str2, dictionary1, dictionary2);
  }

  public string SaveString(
    bool readResponseSource,
    Dictionary<string, string> headers = null,
    List<LogEntry> log = null)
  {
    string logString = "";
    string str = this.response.ToString();
    log?.Add(new LogEntry("Response Source:", Colors.Green));
    if (readResponseSource)
    {
      logString = str;
      log?.Add(new LogEntry(logString, Colors.GreenYellow));
    }
    else
      log?.Add(new LogEntry("[SKIPPED]", Colors.GreenYellow));
    if (!this.hasContentLength && headers != null)
    {
      headers["Content-Length"] = str.Length != 0 ? (!this.isGZipped ? str.Length.ToString() : GZip.Zip(str).Length.ToString()) : "0";
      log?.Add(new LogEntry("Calculated header: Content-Length: " + headers["Content-Length"], Colors.LightPink));
    }
    return logString;
  }

  public MemoryStream GetResponseStream() => this.response.ToMemoryStream();

  public void SaveFile(string path, List<LogEntry> log = null)
  {
    string directoryName = Path.GetDirectoryName(path);
    if (directoryName != "")
      directoryName += Path.DirectorySeparatorChar.ToString();
    string withoutExtension = Path.GetFileNameWithoutExtension(path);
    string extension = Path.GetExtension(path);
    string path1 = directoryName + RuriLib.Functions.Files.Files.MakeValidFileName(withoutExtension) + extension;
    using (FileStream destination = File.Create(path1))
      this.response.ToMemoryStream().CopyTo((Stream) destination);
    log?.Add(new LogEntry("File saved as " + path1, Colors.Green));
  }

  internal static string GenerateMultipartBoundary()
  {
    StringBuilder stringBuilder = new StringBuilder();
    Random random = new Random();
    for (int index = 0; index < 16 /*0x10*/; ++index)
    {
      char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26.0 * random.NextDouble() + 65.0)));
      stringBuilder.Append(ch);
    }
    return "------WebKitFormBoundary" + stringBuilder.ToString().ToLower();
  }

  public static bool CanContainBody(HttpMethod method)
  {
    return method == HttpMethod.POST || method == HttpMethod.PUT || method == HttpMethod.DELETE;
  }
}
