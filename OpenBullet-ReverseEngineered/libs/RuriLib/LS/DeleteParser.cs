// Decompiled with JetBrains decompiler
// Type: RuriLib.LS.DeleteParser
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Microsoft.CSharp.RuntimeBinder;
using RuriLib.Functions.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;

#nullable disable
namespace RuriLib.LS;

internal class DeleteParser
{
  public static Action Parse(string line, BotData data)
  {
    string input = line.Trim();
    string field = LineParser.ParseToken(ref input, TokenType.Parameter, true).ToUpper();
    return (Action) (() =>
    {
      Comparer comparer = Comparer.EqualTo;
      switch (field)
      {
        case "COOKIE":
          if (LineParser.Lookahead(ref input) == TokenType.Parameter)
          {
            // ISSUE: reference to a compiler-generated field
            if (DeleteParser.\u003C\u003Eo__0.\u003C\u003Ep__0 == null)
            {
              // ISSUE: reference to a compiler-generated field
              DeleteParser.\u003C\u003Eo__0.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, Comparer>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (Comparer), typeof (DeleteParser)));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            comparer = DeleteParser.\u003C\u003Eo__0.\u003C\u003Ep__0.Target((CallSite) DeleteParser.\u003C\u003Eo__0.\u003C\u003Ep__0, LineParser.ParseEnum(ref input, "TYPE", typeof (Comparer)));
          }
          string literal1 = LineParser.ParseLiteral(ref input, "NAME");
          for (int index = 0; index < data.Cookies.Count; ++index)
          {
            string key = data.Cookies.ToList<KeyValuePair<string, string>>()[index].Key;
            if (Condition.ReplaceAndVerify(key, comparer, literal1, data))
              data.Cookies.Remove(key);
          }
          break;
        case "VAR":
          if (LineParser.Lookahead(ref input) == TokenType.Parameter)
          {
            // ISSUE: reference to a compiler-generated field
            if (DeleteParser.\u003C\u003Eo__0.\u003C\u003Ep__1 == null)
            {
              // ISSUE: reference to a compiler-generated field
              DeleteParser.\u003C\u003Eo__0.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, Comparer>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (Comparer), typeof (DeleteParser)));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            comparer = DeleteParser.\u003C\u003Eo__0.\u003C\u003Ep__1.Target((CallSite) DeleteParser.\u003C\u003Eo__0.\u003C\u003Ep__1, LineParser.ParseEnum(ref input, "TYPE", typeof (Comparer)));
          }
          string literal2 = LineParser.ParseLiteral(ref input, "NAME");
          data.Variables.Remove(comparer, literal2, data);
          break;
        case "GVAR":
          if (LineParser.Lookahead(ref input) == TokenType.Parameter)
          {
            // ISSUE: reference to a compiler-generated field
            if (DeleteParser.\u003C\u003Eo__0.\u003C\u003Ep__2 == null)
            {
              // ISSUE: reference to a compiler-generated field
              DeleteParser.\u003C\u003Eo__0.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, Comparer>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (Comparer), typeof (DeleteParser)));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            comparer = DeleteParser.\u003C\u003Eo__0.\u003C\u003Ep__2.Target((CallSite) DeleteParser.\u003C\u003Eo__0.\u003C\u003Ep__2, LineParser.ParseEnum(ref input, "TYPE", typeof (Comparer)));
          }
          string literal3 = LineParser.ParseLiteral(ref input, "NAME");
          try
          {
            data.GlobalVariables.Remove(comparer, literal3, data);
            break;
          }
          catch
          {
            break;
          }
        default:
          throw new ArgumentException("Invalid identifier " + field);
      }
      data.Log(new LogEntry("DELETE command executed on field " + field, Colors.White));
    });
  }
}
