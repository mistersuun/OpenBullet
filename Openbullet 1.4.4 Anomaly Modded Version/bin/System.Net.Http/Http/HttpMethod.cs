// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpMethod
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

#nullable disable
namespace System.Net.Http;

public class HttpMethod : IEquatable<HttpMethod>
{
  private readonly string _method;
  private int _hashcode;
  private static readonly HttpMethod s_getMethod = new HttpMethod("GET");
  private static readonly HttpMethod s_putMethod = new HttpMethod("PUT");
  private static readonly HttpMethod s_postMethod = new HttpMethod("POST");
  private static readonly HttpMethod s_deleteMethod = new HttpMethod("DELETE");
  private static readonly HttpMethod s_headMethod = new HttpMethod("HEAD");
  private static readonly HttpMethod s_optionsMethod = new HttpMethod("OPTIONS");
  private static readonly HttpMethod s_traceMethod = new HttpMethod("TRACE");

  public static HttpMethod Get => HttpMethod.s_getMethod;

  public static HttpMethod Put => HttpMethod.s_putMethod;

  public static HttpMethod Post => HttpMethod.s_postMethod;

  public static HttpMethod Delete => HttpMethod.s_deleteMethod;

  public static HttpMethod Head => HttpMethod.s_headMethod;

  public static HttpMethod Options => HttpMethod.s_optionsMethod;

  public static HttpMethod Trace => HttpMethod.s_traceMethod;

  public string Method => this._method;

  public HttpMethod(string method)
  {
    if (string.IsNullOrEmpty(method))
      throw new ArgumentException(SR.net_http_argument_empty_string, nameof (method));
    this._method = HttpRuleParser.GetTokenLength(method, 0) == method.Length ? method : throw new FormatException(SR.net_http_httpmethod_format_error);
  }

  public bool Equals(HttpMethod other)
  {
    if ((object) other == null)
      return false;
    return (object) this._method == (object) other._method || string.Equals(this._method, other._method, StringComparison.OrdinalIgnoreCase);
  }

  public override bool Equals(object obj) => this.Equals(obj as HttpMethod);

  public override int GetHashCode()
  {
    if (this._hashcode == 0)
      this._hashcode = HttpMethod.IsUpperAscii(this._method) ? this._method.GetHashCode() : this._method.ToUpperInvariant().GetHashCode();
    return this._hashcode;
  }

  public override string ToString() => this._method.ToString();

  public static bool operator ==(HttpMethod left, HttpMethod right)
  {
    if ((object) left == null)
      return (object) right == null;
    return (object) right == null ? (object) left == null : left.Equals(right);
  }

  public static bool operator !=(HttpMethod left, HttpMethod right) => !(left == right);

  private static bool IsUpperAscii(string value)
  {
    for (int index = 0; index < value.Length; ++index)
    {
      char ch = value[index];
      if (ch < 'A' || ch > 'Z')
        return false;
    }
    return true;
  }
}
