// Decompiled with JetBrains decompiler
// Type: RuriLib.LS.CommandParser
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;

#nullable disable
namespace RuriLib.LS;

public class CommandParser
{
  public static bool IsCommand(string line)
  {
    GroupCollection groups = Regex.Match(line, "^([^ ]*)").Groups;
    return ((IEnumerable<string>) Enum.GetNames(typeof (CommandParser.CommandName))).Select<string, string>((Func<string, string>) (n => n.ToUpper())).Contains<string>(groups[1].Value.ToUpper());
  }

  public static Action Parse(string line, BotData data)
  {
    string input = line.Trim();
    if (input == "")
      throw new ArgumentNullException();
    LineParser.ParseToken(ref input, TokenType.Label, false);
    string token;
    try
    {
      token = LineParser.ParseToken(ref input, TokenType.Parameter, true);
    }
    catch
    {
      throw new ArgumentException("Missing identifier");
    }
    switch ((CommandParser.CommandName) Enum.Parse(typeof (CommandParser.CommandName), token, true))
    {
      case CommandParser.CommandName.PRINT:
        return (Action) (() => data.Log(new LogEntry(BlockBase.ReplaceValues(input, data), Colors.White)));
      case CommandParser.CommandName.SET:
        return SetParser.Parse(input, data);
      case CommandParser.CommandName.DELETE:
        return DeleteParser.Parse(input, data);
      case CommandParser.CommandName.MOUSEACTION:
        return MouseActionParser.Parse(input, data);
      default:
        throw new ArgumentException($"Invalid identifier '{token}'");
    }
  }

  public enum CommandName
  {
    PRINT,
    SET,
    DELETE,
    MOUSEACTION,
  }
}
