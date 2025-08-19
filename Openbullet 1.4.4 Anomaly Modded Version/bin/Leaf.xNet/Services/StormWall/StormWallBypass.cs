// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.Services.StormWall.StormWallBypass
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.Net;

#nullable disable
namespace Leaf.xNet.Services.StormWall;

public static class StormWallBypass
{
  private static StormWallSolver _solver;

  private static StormWallSolver Solver
  {
    get => StormWallBypass._solver ?? (StormWallBypass._solver = new StormWallSolver());
  }

  public static bool IsStormWalled(this HttpResponse rawResp) => rawResp.ToString().IsStormWalled();

  public static bool IsStormWalled(this string resp)
  {
    return resp.Contains("<h1>Stormwall DDoS protection</h1>") || resp.Contains("://reports.stormwall.pro");
  }

  public static HttpResponse GetThroughStormWall(
    this HttpRequest req,
    string url,
    HttpResponse rawResp)
  {
    return req.GetThroughStormWall(url, rawResp.ToString());
  }

  public static HttpResponse GetThroughStormWall(this HttpRequest req, string url, string resp = null)
  {
    if (resp == null)
    {
      HttpResponse throughStormWall = req.Get(url);
      resp = throughStormWall.ToString();
      if (!resp.IsStormWalled())
        return throughStormWall;
    }
    string jsConstValue1 = resp.ParseJsConstValue("cE");
    int result;
    if (!int.TryParse(resp.ParseJsConstValue("cK", false), out result))
      StormWallBypass.ThrowNotFoundJsValue("cK");
    string jsConstValue2 = resp.ParseJsConstValue("cN");
    string jsVariableValue = resp.ParseJsVariableValue("abc");
    StormWallBypass.Solver.Init(jsConstValue1, result, jsVariableValue);
    string str = StormWallBypass.Solver.Solve();
    string host = new Uri(url).Host;
    Cookie cookie = new Cookie(jsConstValue2, str, "/", host)
    {
      Expires = DateTime.Now.AddSeconds(30.0)
    };
    req.Cookies.Container.Add(cookie);
    string referer = req.Referer;
    req.Referer = url;
    HttpResponse rawResp = req.Get(url);
    req.Referer = referer;
    return !rawResp.IsStormWalled() ? rawResp : throw new StormWallException("Unable to pass StormWall at URL: " + url);
  }

  private static void ThrowNotFoundJsValue(string variable)
  {
    throw new StormWallException($"Not found \"{variable}\" variable or const in StormWall code");
  }

  private static string ParseJsConstValue(this string resp, string variable, bool isString = true)
  {
    string format;
    string right;
    if (isString)
    {
      format = "const {0} = \"";
      right = "\";";
    }
    else
    {
      format = "const {0} = ";
      right = ";";
    }
    string jsConstValue = resp.Substring(string.Format(format, (object) variable), right);
    if (jsConstValue != null)
      return jsConstValue;
    StormWallBypass.ThrowNotFoundJsValue(variable);
    return jsConstValue;
  }

  private static string ParseJsVariableValue(this string resp, string variable)
  {
    string jsVariableValue = resp.Substring($"var {variable}=\"", "\",");
    if (jsVariableValue != null)
      return jsVariableValue;
    StormWallBypass.ThrowNotFoundJsValue(variable);
    return jsVariableValue;
  }
}
