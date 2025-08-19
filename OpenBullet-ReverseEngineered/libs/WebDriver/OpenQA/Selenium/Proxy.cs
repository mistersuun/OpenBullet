// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Proxy
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium;

[JsonObject(MemberSerialization.OptIn)]
public class Proxy
{
  private ProxyKind proxyKind = ProxyKind.Unspecified;
  private bool isAutoDetect;
  private string ftpProxyLocation;
  private string httpProxyLocation;
  private string proxyAutoConfigUrl;
  private string sslProxyLocation;
  private string socksProxyLocation;
  private string socksUserName;
  private string socksPassword;
  private List<string> noProxyAddresses = new List<string>();

  public Proxy()
  {
  }

  public Proxy(Dictionary<string, object> settings)
  {
    if (settings == null)
      throw new ArgumentNullException(nameof (settings), "settings dictionary cannot be null");
    if (settings.ContainsKey("proxyType"))
      this.Kind = (ProxyKind) Enum.Parse(typeof (ProxyKind), settings["proxyType"].ToString(), true);
    if (settings.ContainsKey("ftpProxy"))
      this.FtpProxy = settings["ftpProxy"].ToString();
    if (settings.ContainsKey("httpProxy"))
      this.HttpProxy = settings["httpProxy"].ToString();
    if (settings.ContainsKey("noProxy"))
    {
      List<string> addressesToAdd = new List<string>();
      if (settings["noProxy"] is string setting2)
      {
        List<string> stringList = addressesToAdd;
        char[] chArray = new char[1]{ ';' };
        string[] collection = setting2.Split(chArray);
        stringList.AddRange((IEnumerable<string>) collection);
      }
      else if (settings["noProxy"] is object[] setting1)
      {
        for (int index = 0; index < setting1.Length; ++index)
        {
          object obj = setting1[index];
          addressesToAdd.Add(obj.ToString());
        }
      }
      this.AddBypassAddresses((IEnumerable<string>) addressesToAdd);
    }
    if (settings.ContainsKey("proxyAutoconfigUrl"))
      this.ProxyAutoConfigUrl = settings["proxyAutoconfigUrl"].ToString();
    if (settings.ContainsKey("sslProxy"))
      this.SslProxy = settings["sslProxy"].ToString();
    if (settings.ContainsKey("socksProxy"))
      this.SocksProxy = settings["socksProxy"].ToString();
    if (settings.ContainsKey("socksUsername"))
      this.SocksUserName = settings["socksUsername"].ToString();
    if (settings.ContainsKey(nameof (socksPassword)))
      this.SocksPassword = settings[nameof (socksPassword)].ToString();
    if (!settings.ContainsKey("autodetect"))
      return;
    this.IsAutoDetect = (bool) settings["autodetect"];
  }

  [JsonIgnore]
  public ProxyKind Kind
  {
    get => this.proxyKind;
    set
    {
      this.VerifyProxyTypeCompatilibily(value);
      this.proxyKind = value;
    }
  }

  [JsonProperty("proxyType")]
  public string SerializableProxyKind
  {
    get
    {
      return this.proxyKind == ProxyKind.ProxyAutoConfigure ? "PAC" : this.proxyKind.ToString().ToUpperInvariant();
    }
  }

  [JsonIgnore]
  public bool IsAutoDetect
  {
    get => this.isAutoDetect;
    set
    {
      if (this.isAutoDetect == value)
        return;
      this.VerifyProxyTypeCompatilibily(ProxyKind.AutoDetect);
      this.proxyKind = ProxyKind.AutoDetect;
      this.isAutoDetect = value;
    }
  }

  [JsonProperty("ftpProxy", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
  public string FtpProxy
  {
    get => this.ftpProxyLocation;
    set
    {
      this.VerifyProxyTypeCompatilibily(ProxyKind.Manual);
      this.proxyKind = ProxyKind.Manual;
      this.ftpProxyLocation = value;
    }
  }

  [JsonProperty("httpProxy", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
  public string HttpProxy
  {
    get => this.httpProxyLocation;
    set
    {
      this.VerifyProxyTypeCompatilibily(ProxyKind.Manual);
      this.proxyKind = ProxyKind.Manual;
      this.httpProxyLocation = value;
    }
  }

  [Obsolete("Add addresses to bypass with the proxy by using the AddBypassAddress method.")]
  public string NoProxy
  {
    get => this.BypassProxyAddresses;
    set => this.AddBypassAddress(value);
  }

  [JsonProperty("noProxy", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
  public string BypassProxyAddresses
  {
    get
    {
      return this.noProxyAddresses.Count == 0 ? (string) null : string.Join(";", this.noProxyAddresses.ToArray());
    }
  }

  [JsonProperty("proxyAutoconfigUrl", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
  public string ProxyAutoConfigUrl
  {
    get => this.proxyAutoConfigUrl;
    set
    {
      this.VerifyProxyTypeCompatilibily(ProxyKind.ProxyAutoConfigure);
      this.proxyKind = ProxyKind.ProxyAutoConfigure;
      this.proxyAutoConfigUrl = value;
    }
  }

  [JsonProperty("sslProxy", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
  public string SslProxy
  {
    get => this.sslProxyLocation;
    set
    {
      this.VerifyProxyTypeCompatilibily(ProxyKind.Manual);
      this.proxyKind = ProxyKind.Manual;
      this.sslProxyLocation = value;
    }
  }

  [JsonProperty("socksProxy", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
  public string SocksProxy
  {
    get => this.socksProxyLocation;
    set
    {
      this.VerifyProxyTypeCompatilibily(ProxyKind.Manual);
      this.proxyKind = ProxyKind.Manual;
      this.socksProxyLocation = value;
    }
  }

  [JsonProperty("socksUsername", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
  public string SocksUserName
  {
    get => this.socksUserName;
    set
    {
      this.VerifyProxyTypeCompatilibily(ProxyKind.Manual);
      this.proxyKind = ProxyKind.Manual;
      this.socksUserName = value;
    }
  }

  [JsonProperty("socksPassword", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
  public string SocksPassword
  {
    get => this.socksPassword;
    set
    {
      this.VerifyProxyTypeCompatilibily(ProxyKind.Manual);
      this.proxyKind = ProxyKind.Manual;
      this.socksPassword = value;
    }
  }

  public void AddBypassAddress(string address)
  {
    if (string.IsNullOrEmpty(address))
      throw new ArgumentException("address must not be null or empty", nameof (address));
    this.AddBypassAddresses(address);
  }

  public void AddBypassAddresses(params string[] addressesToAdd)
  {
    this.AddBypassAddresses((IEnumerable<string>) new List<string>((IEnumerable<string>) addressesToAdd));
  }

  public void AddBypassAddresses(IEnumerable<string> addressesToAdd)
  {
    if (addressesToAdd == null)
      throw new ArgumentNullException(nameof (addressesToAdd), "addressesToAdd must not be null");
    this.VerifyProxyTypeCompatilibily(ProxyKind.Manual);
    this.proxyKind = ProxyKind.Manual;
    this.noProxyAddresses.AddRange(addressesToAdd);
  }

  internal Dictionary<string, object> ToCapability() => this.AsDictionary(true);

  internal Dictionary<string, object> ToLegacyCapability() => this.AsDictionary(false);

  private Dictionary<string, object> AsDictionary(bool isSpecCompliant)
  {
    Dictionary<string, object> dictionary = (Dictionary<string, object>) null;
    if (this.proxyKind != ProxyKind.Unspecified)
    {
      dictionary = new Dictionary<string, object>();
      if (this.proxyKind == ProxyKind.ProxyAutoConfigure)
      {
        dictionary["proxyType"] = (object) "pac";
        if (!string.IsNullOrEmpty(this.proxyAutoConfigUrl))
          dictionary["proxyAutoconfigUrl"] = (object) this.proxyAutoConfigUrl;
      }
      else
        dictionary["proxyType"] = (object) this.proxyKind.ToString().ToLowerInvariant();
      if (!string.IsNullOrEmpty(this.httpProxyLocation))
        dictionary["httpProxy"] = (object) this.httpProxyLocation;
      if (!string.IsNullOrEmpty(this.sslProxyLocation))
        dictionary["sslProxy"] = (object) this.sslProxyLocation;
      if (!string.IsNullOrEmpty(this.ftpProxyLocation))
        dictionary["ftpProxy"] = (object) this.ftpProxyLocation;
      if (!string.IsNullOrEmpty(this.socksProxyLocation))
      {
        string str = string.Empty;
        if (!string.IsNullOrEmpty(this.socksUserName) && !string.IsNullOrEmpty(this.socksPassword))
          str = $"{this.socksUserName}:{this.socksPassword}@";
        dictionary["socksProxy"] = (object) (str + this.socksProxyLocation);
      }
      if (this.noProxyAddresses.Count > 0)
        dictionary["noProxy"] = this.GetNoProxyAddressList(isSpecCompliant);
    }
    return dictionary;
  }

  private object GetNoProxyAddressList(bool isSpecCompliant)
  {
    object proxyAddressList;
    if (isSpecCompliant)
    {
      List<object> objectList = new List<object>();
      foreach (string noProxyAddress in this.noProxyAddresses)
        objectList.Add((object) noProxyAddress);
      proxyAddressList = (object) objectList;
    }
    else
      proxyAddressList = (object) this.BypassProxyAddresses;
    return proxyAddressList;
  }

  private void VerifyProxyTypeCompatilibily(ProxyKind compatibleProxy)
  {
    if (this.proxyKind != ProxyKind.Unspecified && this.proxyKind != compatibleProxy)
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Specified proxy type {0} is not compatible with current setting {1}", (object) compatibleProxy.ToString().ToUpperInvariant(), (object) this.proxyKind.ToString().ToUpperInvariant()));
  }
}
