// Decompiled with JetBrains decompiler
// Type: RuriLib.BlockRequest
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Extreme.Net;
using Microsoft.CSharp.RuntimeBinder;
using RuriLib.Functions.Requests;
using RuriLib.LS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;

#nullable disable
namespace RuriLib;

public class BlockRequest : BlockBase
{
  private string url = "https://google.com";
  private RequestType requestType;
  private string authUser = "";
  private string authPass = "";
  private string postData = "";
  private HttpMethod method;
  private string contentType = "application/x-www-form-urlencoded";
  private bool autoRedirect = true;
  private bool readResponseSource = true;
  private bool encodeContent;
  private bool acceptEncoding = true;
  private string multipartBoundary = "";
  private ResponseType responseType;
  private string downloadPath = "";
  private bool saveAsScreenshot;

  public string Url
  {
    get => this.url;
    set
    {
      this.url = value;
      this.OnPropertyChanged(nameof (Url));
    }
  }

  public RequestType RequestType
  {
    get => this.requestType;
    set
    {
      this.requestType = value;
      this.OnPropertyChanged(nameof (RequestType));
    }
  }

  public string AuthUser
  {
    get => this.authUser;
    set
    {
      this.authUser = value;
      this.OnPropertyChanged(nameof (AuthUser));
    }
  }

  public string AuthPass
  {
    get => this.authPass;
    set
    {
      this.authPass = value;
      this.OnPropertyChanged(nameof (AuthPass));
    }
  }

  public string PostData
  {
    get => this.postData;
    set
    {
      this.postData = value;
      this.OnPropertyChanged(nameof (PostData));
    }
  }

  public HttpMethod Method
  {
    get => this.method;
    set
    {
      this.method = value;
      this.OnPropertyChanged(nameof (Method));
    }
  }

  public Dictionary<string, string> CustomHeaders { get; set; } = new Dictionary<string, string>()
  {
    {
      "User-Agent",
      "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko"
    },
    {
      "Pragma",
      "no-cache"
    },
    {
      "Accept",
      "*/*"
    }
  };

  public Dictionary<string, string> CustomCookies { get; set; } = new Dictionary<string, string>();

  public string ContentType
  {
    get => this.contentType;
    set
    {
      this.contentType = value;
      this.OnPropertyChanged(nameof (ContentType));
    }
  }

  public bool AutoRedirect
  {
    get => this.autoRedirect;
    set
    {
      this.autoRedirect = value;
      this.OnPropertyChanged(nameof (AutoRedirect));
    }
  }

  public bool ReadResponseSource
  {
    get => this.readResponseSource;
    set
    {
      this.readResponseSource = value;
      this.OnPropertyChanged(nameof (ReadResponseSource));
    }
  }

  public bool EncodeContent
  {
    get => this.encodeContent;
    set
    {
      this.encodeContent = value;
      this.OnPropertyChanged(nameof (EncodeContent));
    }
  }

  public bool AcceptEncoding
  {
    get => this.acceptEncoding;
    set
    {
      this.acceptEncoding = value;
      this.OnPropertyChanged(nameof (AcceptEncoding));
    }
  }

  public string MultipartBoundary
  {
    get => this.multipartBoundary;
    set
    {
      this.multipartBoundary = value;
      this.OnPropertyChanged(nameof (MultipartBoundary));
    }
  }

  public List<RuriLib.Functions.Requests.MultipartContent> MultipartContents { get; set; } = new List<RuriLib.Functions.Requests.MultipartContent>();

  public ResponseType ResponseType
  {
    get => this.responseType;
    set
    {
      this.responseType = value;
      this.OnPropertyChanged(nameof (ResponseType));
    }
  }

  public string DownloadPath
  {
    get => this.downloadPath;
    set
    {
      this.downloadPath = value;
      this.OnPropertyChanged(nameof (DownloadPath));
    }
  }

  public bool SaveAsScreenshot
  {
    get => this.saveAsScreenshot;
    set
    {
      this.saveAsScreenshot = value;
      this.OnPropertyChanged(nameof (SaveAsScreenshot));
    }
  }

  public BlockRequest() => this.Label = "REQUEST";

  public override BlockBase FromLS(string line)
  {
    string input = line.Trim();
    if (input.StartsWith("#"))
      this.Label = LineParser.ParseLabel(ref input);
    // ISSUE: reference to a compiler-generated field
    if (BlockRequest.\u003C\u003Eo__73.\u003C\u003Ep__0 == null)
    {
      // ISSUE: reference to a compiler-generated field
      BlockRequest.\u003C\u003Eo__73.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, HttpMethod>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (HttpMethod), typeof (BlockRequest)));
    }
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.Method = BlockRequest.\u003C\u003Eo__73.\u003C\u003Ep__0.Target((CallSite) BlockRequest.\u003C\u003Eo__73.\u003C\u003Ep__0, LineParser.ParseEnum(ref input, "METHOD", typeof (HttpMethod)));
    this.Url = LineParser.ParseLiteral(ref input, "URL");
    while (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
      LineParser.SetBool(ref input, (object) this);
    this.CustomHeaders.Clear();
    RuriLib.Functions.Requests.MultipartContent multipartContent1;
    while (input != "" && !input.StartsWith("->"))
    {
      switch (LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Parameter, true).ToUpper())
      {
        case "BASICAUTH":
          this.RequestType = RequestType.BasicAuth;
          continue;
        case "BOUNDARY":
          this.MultipartBoundary = LineParser.ParseLiteral(ref input, "BOUNDARY");
          continue;
        case "CONTENT":
          this.PostData = LineParser.ParseLiteral(ref input, "POST DATA");
          continue;
        case "CONTENTTYPE":
          this.ContentType = LineParser.ParseLiteral(ref input, "CONTENT TYPE");
          continue;
        case "COOKIE":
          string[] strArray1 = BlockRequest.ParseString(LineParser.ParseLiteral(ref input, "COOKIE VALUE"), ':', 2);
          this.CustomCookies[strArray1[0]] = strArray1[1];
          continue;
        case "FILECONTENT":
          string[] strArray2 = BlockRequest.ParseString(LineParser.ParseLiteral(ref input, "FILE CONTENT"), ':', 3);
          List<RuriLib.Functions.Requests.MultipartContent> multipartContents1 = this.MultipartContents;
          multipartContent1 = new RuriLib.Functions.Requests.MultipartContent();
          multipartContent1.Type = MultipartContentType.File;
          multipartContent1.Name = strArray2[0];
          multipartContent1.Value = strArray2[1];
          multipartContent1.ContentType = strArray2[2];
          RuriLib.Functions.Requests.MultipartContent multipartContent2 = multipartContent1;
          multipartContents1.Add(multipartContent2);
          continue;
        case "HEADER":
          string[] strArray3 = BlockRequest.ParseString(LineParser.ParseLiteral(ref input, "HEADER VALUE"), ':', 2);
          this.CustomHeaders[strArray3[0]] = strArray3[1];
          continue;
        case "MULTIPART":
          this.RequestType = RequestType.Multipart;
          continue;
        case "PASSWORD":
          this.AuthPass = LineParser.ParseLiteral(ref input, "PASSWORD");
          continue;
        case "STANDARD":
          this.RequestType = RequestType.Standard;
          continue;
        case "STRINGCONTENT":
          string[] strArray4 = BlockRequest.ParseString(LineParser.ParseLiteral(ref input, "STRING CONTENT"), ':', 2);
          List<RuriLib.Functions.Requests.MultipartContent> multipartContents2 = this.MultipartContents;
          multipartContent1 = new RuriLib.Functions.Requests.MultipartContent();
          multipartContent1.Type = MultipartContentType.String;
          multipartContent1.Name = strArray4[0];
          multipartContent1.Value = strArray4[1];
          RuriLib.Functions.Requests.MultipartContent multipartContent3 = multipartContent1;
          multipartContents2.Add(multipartContent3);
          continue;
        case "USERNAME":
          this.AuthUser = LineParser.ParseLiteral(ref input, "USERNAME");
          continue;
        default:
          continue;
      }
    }
    if (input.StartsWith("->"))
    {
      LineParser.EnsureIdentifier(ref input, "->");
      string token = LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Parameter, true);
      if (token.ToUpper() == "STRING")
        this.ResponseType = ResponseType.String;
      else if (token.ToUpper() == "FILE")
      {
        this.ResponseType = ResponseType.File;
        this.DownloadPath = LineParser.ParseLiteral(ref input, "DOWNLOAD PATH");
        while (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
          LineParser.SetBool(ref input, (object) this);
      }
    }
    return (BlockBase) this;
  }

  public static string[] ParseString(string input, char separator, int count)
  {
    return ((IEnumerable<string>) input.Split(new char[1]
    {
      separator
    }, count)).Select<string, string>((Func<string, string>) (s => s.Trim())).ToArray<string>();
  }

  public override string ToLS(bool indent = true)
  {
    BlockWriter blockWriter = new BlockWriter(this.GetType(), indent, this.Disabled);
    blockWriter.Label(this.Label).Token((object) "REQUEST").Token((object) this.Method).Literal(this.Url).Boolean(this.AcceptEncoding, "AcceptEncoding").Boolean(this.AutoRedirect, "AutoRedirect").Boolean(this.ReadResponseSource, "ReadResponseSource").Boolean(this.EncodeContent, "EncodeContent").Token((object) this.RequestType, "RequestType").Indent();
    switch (this.RequestType)
    {
      case RequestType.Standard:
        if (Request.CanContainBody(this.method))
        {
          blockWriter.Token((object) "CONTENT").Literal(this.PostData).Indent().Token((object) "CONTENTTYPE").Literal(this.ContentType);
          break;
        }
        break;
      case RequestType.BasicAuth:
        blockWriter.Token((object) "USERNAME").Literal(this.AuthUser).Token((object) "PASSWORD").Literal(this.AuthPass).Indent();
        break;
      case RequestType.Multipart:
        foreach (RuriLib.Functions.Requests.MultipartContent multipartContent in this.MultipartContents)
        {
          blockWriter.Indent().Token((object) (multipartContent.Type.ToString().ToUpper() + "CONTENT"));
          if (multipartContent.Type == MultipartContentType.String)
            blockWriter.Literal($"{multipartContent.Name}: {multipartContent.Value}");
          else if (multipartContent.Type == MultipartContentType.File)
            blockWriter.Literal($"{multipartContent.Name}: {multipartContent.Value}: {multipartContent.ContentType}");
        }
        if (!blockWriter.CheckDefault((object) this.MultipartBoundary, "MultipartBoundary"))
        {
          blockWriter.Indent().Token((object) "BOUNDARY").Literal(this.MultipartBoundary);
          break;
        }
        break;
    }
    foreach (KeyValuePair<string, string> customCookie in this.CustomCookies)
      blockWriter.Indent().Token((object) "COOKIE").Literal($"{customCookie.Key}: {customCookie.Value}");
    foreach (KeyValuePair<string, string> customHeader in this.CustomHeaders)
      blockWriter.Indent().Token((object) "HEADER").Literal($"{customHeader.Key}: {customHeader.Value}");
    if (this.ResponseType == ResponseType.File)
      blockWriter.Indent().Arrow().Token((object) "FILE").Literal(this.DownloadPath).Boolean(this.SaveAsScreenshot, "SaveAsScreenshot");
    return blockWriter.ToString();
  }

  public override void Process(BotData data)
  {
    base.Process(data);
    Request request = new Request();
    request.Setup(data.GlobalSettings, this.AutoRedirect, data.ConfigSettings.MaxRedirects, this.AcceptEncoding);
    string url = BlockBase.ReplaceValues(this.Url, data);
    data.Log(new LogEntry("Calling URL: " + url, Colors.MediumTurquoise));
    switch (this.RequestType)
    {
      case RequestType.Standard:
        request.SetStandardContent(BlockBase.ReplaceValues(this.PostData, data), BlockBase.ReplaceValues(this.ContentType, data), this.Method, this.EncodeContent, data.LogBuffer);
        break;
      case RequestType.BasicAuth:
        request.SetBasicAuth(BlockBase.ReplaceValues(this.AuthUser, data), BlockBase.ReplaceValues(this.AuthPass, data));
        break;
      case RequestType.Multipart:
        IEnumerable<RuriLib.Functions.Requests.MultipartContent> contents = this.MultipartContents.Select<RuriLib.Functions.Requests.MultipartContent, RuriLib.Functions.Requests.MultipartContent>((Func<RuriLib.Functions.Requests.MultipartContent, RuriLib.Functions.Requests.MultipartContent>) (m => new RuriLib.Functions.Requests.MultipartContent()
        {
          Name = BlockBase.ReplaceValues(m.Name, data),
          Value = BlockBase.ReplaceValues(m.Value, data),
          ContentType = BlockBase.ReplaceValues(m.Value, data),
          Type = m.Type
        }));
        request.SetMultipartContent(contents, BlockBase.ReplaceValues(this.MultipartBoundary, data), data.LogBuffer);
        break;
    }
    if (data.UseProxies)
      request.SetProxy(data.Proxy);
    data.Log(new LogEntry("Sent Headers:", Colors.DarkTurquoise));
    Dictionary<string, string> dictionary1 = this.CustomHeaders.Select<KeyValuePair<string, string>, KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, KeyValuePair<string, string>>) (h => new KeyValuePair<string, string>(BlockBase.ReplaceValues(h.Key, data), BlockBase.ReplaceValues(h.Value, data)))).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (h => h.Key), (Func<KeyValuePair<string, string>, string>) (h => h.Value));
    request.SetHeaders(dictionary1, this.AcceptEncoding, data.LogBuffer);
    data.Log(new LogEntry("Sent Cookies:", Colors.MediumTurquoise));
    foreach (KeyValuePair<string, string> customCookie in this.CustomCookies)
      data.Cookies[BlockBase.ReplaceValues(customCookie.Key, data)] = BlockBase.ReplaceValues(customCookie.Value, data);
    request.SetCookies(data.Cookies, data.LogBuffer);
    data.LogNewLine();
    BotData botData1 = data;
    BotData botData2 = data;
    BotData botData3 = data;
    BotData botData4 = data;
    (string, string, Dictionary<string, string>, Dictionary<string, string>) tuple = request.Perform(url, this.Method, data.ConfigSettings.IgnoreResponseErrors, data.LogBuffer);
    string str1;
    string str2 = str1 = tuple.Item1;
    botData1.Address = str1;
    botData2.ResponseCode = str2 = tuple.Item2;
    Dictionary<string, string> dictionary2;
    botData3.ResponseHeaders = dictionary2 = tuple.Item3;
    botData4.Cookies = dictionary2 = tuple.Item4;
    switch (this.ResponseType)
    {
      case ResponseType.String:
        data.ResponseSource = request.SaveString(this.ReadResponseSource, data.ResponseHeaders, data.LogBuffer);
        break;
      case ResponseType.File:
        if (this.SaveAsScreenshot)
        {
          RuriLib.Functions.Files.Files.SaveScreenshot(request.GetResponseStream(), data);
          data.Log(new LogEntry("File saved as screenshot", Colors.Green));
          break;
        }
        request.SaveFile(BlockBase.ReplaceValues(this.DownloadPath, data), data.LogBuffer);
        break;
    }
  }

  public string GetCustomCookies()
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (KeyValuePair<string, string> customCookie in this.CustomCookies)
    {
      stringBuilder.Append($"{customCookie.Key}: {customCookie.Value}");
      if (!customCookie.Equals((object) this.CustomCookies.Last<KeyValuePair<string, string>>()))
        stringBuilder.Append(Environment.NewLine);
    }
    return stringBuilder.ToString();
  }

  public void SetCustomCookies(string[] lines)
  {
    this.CustomCookies.Clear();
    foreach (string line in lines)
    {
      if (line.Contains<char>(':'))
      {
        string[] strArray = line.Split(new char[1]{ ':' }, 2);
        this.CustomCookies[strArray[0].Trim()] = strArray[1].Trim();
      }
    }
  }

  public string GetCustomHeaders()
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (KeyValuePair<string, string> customHeader in this.CustomHeaders)
    {
      stringBuilder.Append($"{customHeader.Key}: {customHeader.Value}");
      if (!customHeader.Equals((object) this.CustomHeaders.Last<KeyValuePair<string, string>>()))
        stringBuilder.Append(Environment.NewLine);
    }
    return stringBuilder.ToString();
  }

  public void SetCustomHeaders(string[] lines)
  {
    this.CustomHeaders.Clear();
    foreach (string line in lines)
    {
      if (line.Contains<char>(':'))
      {
        string[] strArray = line.Split(new char[1]{ ':' }, 2);
        this.CustomHeaders[strArray[0].Trim()] = strArray[1].Trim();
      }
    }
  }

  public string GetMultipartContents()
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (RuriLib.Functions.Requests.MultipartContent multipartContent in this.MultipartContents)
    {
      stringBuilder.Append($"{multipartContent.Type.ToString().ToUpper()}: {multipartContent.Name}: {multipartContent.Value}");
      if (!multipartContent.Equals((object) this.MultipartContents.Last<RuriLib.Functions.Requests.MultipartContent>()))
        stringBuilder.Append(Environment.NewLine);
    }
    return stringBuilder.ToString();
  }

  public void SetMultipartContents(string[] lines)
  {
    this.MultipartContents.Clear();
    foreach (string line in lines)
    {
      try
      {
        string[] strArray = line.Split(new char[1]{ ':' }, 3);
        this.MultipartContents.Add(new RuriLib.Functions.Requests.MultipartContent()
        {
          Type = (MultipartContentType) Enum.Parse(typeof (MultipartContentType), strArray[0].Trim(), true),
          Name = strArray[1].Trim(),
          Value = strArray[2].Trim()
        });
      }
      catch
      {
      }
    }
  }
}
