// Decompiled with JetBrains decompiler
// Type: RuriLib.LS.LineParser
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;
using System.Reflection;
using System.Text.RegularExpressions;

#nullable disable
namespace RuriLib.LS;

public static class LineParser
{
  public static string ParseToken(ref string input, TokenType type, bool essential, bool proceed = true)
  {
    string pattern = LineParser.GetPattern(type);
    string token = "";
    Match match = new Regex(pattern).Match(input);
    if (match.Success)
    {
      token = match.Value;
      if (proceed)
        input = input.Substring(token.Length).Trim();
      if (type == TokenType.Literal)
        token = token.Substring(1, token.Length - 2).Replace("\\\\", "\\").Replace("\\\"", "\"");
    }
    else if (essential)
      throw new ArgumentException("Cannot parse token");
    return token;
  }

  public static void SetBool(ref string input, object instance)
  {
    string[] strArray = LineParser.ParseToken(ref input, TokenType.Parameter, true).Split('=');
    PropertyInfo property;
    try
    {
      property = instance.GetType().GetProperty(strArray[0]);
    }
    catch
    {
      throw new ArgumentException($"There is no property called {strArray[0]} in the type {instance.GetType().ToString()}");
    }
    if (property.GetValue(instance).GetType() != typeof (bool))
      throw new InvalidCastException($"The property {strArray[0]} is not a boolean");
    switch (strArray[1].ToUpper())
    {
      case "TRUE":
        property.SetValue(instance, (object) true);
        break;
      case "FALSE":
        property.SetValue(instance, (object) false);
        break;
      default:
        throw new ArgumentException($"Expected bool value for '{property.Name}'");
    }
  }

  public static object ParseEnum(ref string input, string label, Type enumType)
  {
    string token;
    try
    {
      token = LineParser.ParseToken(ref input, TokenType.Parameter, true);
    }
    catch
    {
      throw new ArgumentException($"Missing '{label}'");
    }
    try
    {
      return Enum.Parse(enumType, token, true);
    }
    catch
    {
      throw new ArgumentException($"Invalid '{label}'");
    }
  }

  public static string ParseLiteral(ref string input, string label, bool replace = false, BotData data = null)
  {
    try
    {
      return replace ? BlockBase.ReplaceValues(LineParser.ParseToken(ref input, TokenType.Literal, true), data) : LineParser.ParseToken(ref input, TokenType.Literal, true);
    }
    catch
    {
      throw new ArgumentException($"Expected Literal value for '{label}'");
    }
  }

  public static int ParseInt(ref string input, string label)
  {
    try
    {
      return int.Parse(LineParser.ParseToken(ref input, TokenType.Parameter, true));
    }
    catch
    {
      throw new ArgumentException($"Expected Integer value for '{label}'");
    }
  }

  public static float ParseFloat(ref string input, string label)
  {
    try
    {
      return float.Parse(LineParser.ParseToken(ref input, TokenType.Parameter, true));
    }
    catch
    {
      throw new ArgumentException($"Expected Integer value for '{label}'");
    }
  }

  public static string ParseLabel(ref string input)
  {
    return LineParser.ParseToken(ref input, TokenType.Label, false).Substring(1);
  }

  public static void EnsureIdentifier(ref string input, string id)
  {
    if (LineParser.ParseToken(ref input, TokenType.Parameter, true).ToUpper() != id.ToUpper())
      throw new ArgumentException($"Expected identifier '{id}'");
  }

  public static TokenType Lookahead(ref string input)
  {
    int result = 0;
    string token = LineParser.ParseToken(ref input, TokenType.Parameter, true, false);
    if (token.Contains("\""))
      return TokenType.Literal;
    if (token == "->")
      return TokenType.Arrow;
    if (token.StartsWith("#"))
      return TokenType.Label;
    if (token.ToUpper().Contains("=TRUE") || token.ToUpper().Contains("=FALSE"))
      return TokenType.Boolean;
    return int.TryParse(token, out result) ? TokenType.Integer : TokenType.Parameter;
  }

  public static bool CheckIdentifier(ref string input, string id)
  {
    try
    {
      return LineParser.ParseToken(ref input, TokenType.Parameter, true, false).ToUpper() == id.ToUpper();
    }
    catch
    {
      return false;
    }
  }

  private static string GetPattern(TokenType type)
  {
    switch (type)
    {
      case TokenType.Label:
        return "^#[^ ]*";
      case TokenType.Parameter:
        return "^[^ ]*";
      case TokenType.Literal:
        return "\"(\\\\.|[^\\\"])*\"";
      case TokenType.Arrow:
        return "^->";
      default:
        return "";
    }
  }
}
