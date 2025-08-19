// Decompiled with JetBrains decompiler
// Type: RuriLib.LS.SetParser
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Extreme.Net;
using Microsoft.CSharp.RuntimeBinder;
using RuriLib.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Media;

#nullable disable
namespace RuriLib.LS;

internal class SetParser
{
  public static Action Parse(string line, BotData data)
  {
    string input = line.Trim();
    string field = LineParser.ParseToken(ref input, TokenType.Parameter, true).ToUpper();
    return (Action) (() =>
    {
      string s = field;
      // ISSUE: reference to a compiler-generated method
      switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(s))
      {
        case 218129310:
          if (s == "VAR")
          {
            data.Variables.Set(new CVar(LineParser.ParseLiteral(ref input, "NAME", true, data), LineParser.ParseLiteral(ref input, "VALUE", true, data)));
            break;
          }
          goto default;
        case 395052323:
          if (s == "PROXYTYPE")
          {
            CProxy proxy = data.Proxy;
            // ISSUE: reference to a compiler-generated field
            if (SetParser.\u003C\u003Eo__0.\u003C\u003Ep__1 == null)
            {
              // ISSUE: reference to a compiler-generated field
              SetParser.\u003C\u003Eo__0.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, ProxyType>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (ProxyType), typeof (SetParser)));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            int num = (int) SetParser.\u003C\u003Eo__0.\u003C\u003Ep__1.Target((CallSite) SetParser.\u003C\u003Eo__0.\u003C\u003Ep__1, LineParser.ParseEnum(ref input, "PROXYTYPE", typeof (ProxyType)));
            proxy.Type = (ProxyType) num;
            break;
          }
          goto default;
        case 453382463:
          if (s == "COOKIE")
          {
            data.Cookies.Add(LineParser.ParseLiteral(ref input, "NAME", true, data), LineParser.ParseLiteral(ref input, "VALUE", true, data));
            break;
          }
          goto default;
        case 826721103:
          if (s == "NEWGVAR")
          {
            try
            {
              data.GlobalVariables.SetNew(new CVar(LineParser.ParseLiteral(ref input, "NAME", true, data), LineParser.ParseLiteral(ref input, "VALUE", true, data)));
              break;
            }
            catch
            {
              break;
            }
          }
          else
            goto default;
        case 1122648243:
          if (s == "ADDRESS")
          {
            data.Address = LineParser.ParseLiteral(ref input, "ADDRESS", true, data);
            break;
          }
          goto default;
        case 1162767579:
          if (s == "CAP")
          {
            data.Variables.Set(new CVar(LineParser.ParseLiteral(ref input, "NAME", true, data), LineParser.ParseLiteral(ref input, "VALUE", true, data), true));
            break;
          }
          goto default;
        case 1930367908:
          if (s == "USEPROXY")
          {
            switch (LineParser.ParseToken(ref input, TokenType.Parameter, true).ToUpper())
            {
              case "TRUE":
                data.UseProxies = true;
                break;
              case "FALSE":
                data.UseProxies = false;
                break;
            }
          }
          else
            goto default;
          break;
        case 2328361645:
          if (s == "GCOOKIES")
          {
            data.GlobalCookies.Clear();
            using (Dictionary<string, string>.Enumerator enumerator = data.Cookies.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<string, string> current = enumerator.Current;
                data.GlobalCookies.Add(current.Key, current.Value);
              }
              break;
            }
          }
          goto default;
        case 2374902203:
          if (s == "PROXY")
          {
            data.Proxy = new CProxy(LineParser.ParseLiteral(ref input, "PROXY", true, data), data.Proxy.Type);
            break;
          }
          goto default;
        case 2549462383:
          if (s == "STATUS")
          {
            BotData botData = data;
            // ISSUE: reference to a compiler-generated field
            if (SetParser.\u003C\u003Eo__0.\u003C\u003Ep__0 == null)
            {
              // ISSUE: reference to a compiler-generated field
              SetParser.\u003C\u003Eo__0.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, BotStatus>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (BotStatus), typeof (SetParser)));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            int num = (int) SetParser.\u003C\u003Eo__0.\u003C\u003Ep__0.Target((CallSite) SetParser.\u003C\u003Eo__0.\u003C\u003Ep__0, LineParser.ParseEnum(ref input, "STATUS", typeof (BotStatus)));
            botData.Status = (BotStatus) num;
            if (data.Status == BotStatus.CUSTOM)
            {
              data.CustomStatus = LineParser.ParseLiteral(ref input, "CUSTOM STATUS");
              break;
            }
            break;
          }
          goto default;
        case 3111715480:
          if (s == "SOURCE")
          {
            data.ResponseSource = LineParser.ParseLiteral(ref input, "SOURCE", true, data);
            break;
          }
          goto default;
        case 3120496227:
          if (s == "RESPONSECODE")
          {
            data.ResponseCode = LineParser.ParseInt(ref input, "RESPONSECODE").ToString();
            break;
          }
          goto default;
        case 3288857317:
          if (s == "DATA")
          {
            data.Data = new CData(LineParser.ParseLiteral(ref input, "DATA", true, data), new WordlistType());
            break;
          }
          goto default;
        case 4061452483:
          if (s == "GVAR")
          {
            try
            {
              data.GlobalVariables.Set(new CVar(LineParser.ParseLiteral(ref input, "NAME", true, data), LineParser.ParseLiteral(ref input, "VALUE", true, data)));
              break;
            }
            catch
            {
              break;
            }
          }
          else
            goto default;
        default:
          throw new ArgumentException("Invalid identifier " + field);
      }
      data.Log(new LogEntry("SET command executed on field " + field, Colors.White));
    });
  }
}
