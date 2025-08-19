// Decompiled with JetBrains decompiler
// Type: RuriLib.Utils.Parsing.Parse
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace RuriLib.Utils.Parsing;

public static class Parse
{
  public static IEnumerable<string> LR(
    string input,
    string left,
    string right,
    bool recursive = false,
    bool useRegex = false)
  {
    if (left == "" && right == "")
      return (IEnumerable<string>) new string[1]{ input };
    if (left != "" && !input.Contains(left) || right != "" && !input.Contains(right))
      return (IEnumerable<string>) new string[0];
    string input1 = input;
    List<string> stringList = new List<string>();
    if (recursive)
    {
      if (useRegex)
      {
        try
        {
          string pattern = Parse.BuildLRPattern(left, right);
          foreach (Match match in Regex.Matches(input1, pattern))
            stringList.Add(match.Value);
        }
        catch
        {
        }
      }
      else
      {
        try
        {
          while (true)
          {
            if (!(left == ""))
              goto label_17;
label_15:
            int startIndex = left == "" ? 0 : input1.IndexOf(left) + left.Length;
            string str1 = input1.Substring(startIndex);
            int length = right == "" ? str1.Length - 1 : str1.IndexOf(right);
            string str2 = str1.Substring(0, length);
            stringList.Add(str2);
            input1 = str1.Substring(str2.Length + right.Length);
            continue;
label_17:
            if (input1.Contains(left))
            {
              if (!(right == ""))
              {
                if (input1.Contains(right))
                  goto label_15;
                break;
              }
              goto label_15;
            }
            break;
          }
        }
        catch
        {
        }
      }
    }
    else if (useRegex)
    {
      string pattern = Parse.BuildLRPattern(left, right);
      MatchCollection matchCollection = Regex.Matches(input1, pattern);
      if (matchCollection.Count > 0)
        stringList.Add(matchCollection[0].Value);
    }
    else
    {
      try
      {
        int startIndex = left == "" ? 0 : input1.IndexOf(left) + left.Length;
        string str = input1.Substring(startIndex);
        int length = right == "" ? str.Length : str.IndexOf(right);
        stringList.Add(str.Substring(0, length));
      }
      catch
      {
      }
    }
    return (IEnumerable<string>) stringList;
  }

  public static IEnumerable<string> CSS(
    string input,
    string selector,
    string attribute,
    int index = 0,
    bool recursive = false)
  {
    IHtmlDocument document = new HtmlParser().ParseDocument(input);
    List<string> stringList = new List<string>();
    if (recursive)
    {
      foreach (IElement element in (IEnumerable<IElement>) document.QuerySelectorAll(selector))
      {
        switch (attribute)
        {
          case "innerHTML":
            stringList.Add(element.InnerHtml);
            continue;
          case "outerHTML":
            stringList.Add(element.OuterHtml);
            continue;
          default:
            using (IEnumerator<IAttr> enumerator = element.Attributes.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                IAttr current = enumerator.Current;
                if (current.Name == attribute)
                {
                  stringList.Add(current.Value);
                  break;
                }
              }
              continue;
            }
        }
      }
    }
    else
    {
      switch (attribute)
      {
        case "innerHTML":
          stringList.Add(document.QuerySelectorAll(selector)[index].InnerHtml);
          break;
        case "outerHTML":
          stringList.Add(document.QuerySelectorAll(selector)[index].OuterHtml);
          break;
        default:
          using (IEnumerator<IAttr> enumerator = document.QuerySelectorAll(selector)[index].Attributes.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IAttr current = enumerator.Current;
              if (current.Name == attribute)
              {
                stringList.Add(current.Value);
                break;
              }
            }
            break;
          }
      }
    }
    return (IEnumerable<string>) stringList;
  }

  public static IEnumerable<string> JSON(
    string input,
    string field,
    bool recursive = false,
    bool useJToken = false)
  {
    List<string> source = new List<string>();
    if (useJToken)
    {
      if (recursive)
      {
        if (input.Trim().StartsWith("["))
        {
          foreach (JToken selectToken in JArray.Parse(input).SelectTokens(field, false))
            source.Add(selectToken.ToString());
        }
        else
        {
          foreach (JToken selectToken in JObject.Parse(input).SelectTokens(field, false))
            source.Add(selectToken.ToString());
        }
      }
      else if (input.Trim().StartsWith("["))
      {
        JArray jarray = JArray.Parse(input);
        source.Add(jarray.SelectToken(field, false).ToString());
      }
      else
      {
        JObject jobject = JObject.Parse(input);
        source.Add(jobject.SelectToken(field, false).ToString());
      }
    }
    else
    {
      List<KeyValuePair<string, string>> jsonlist = new List<KeyValuePair<string, string>>();
      Parse.parseJSON("", input, jsonlist);
      foreach (KeyValuePair<string, string> keyValuePair in jsonlist)
      {
        if (keyValuePair.Key == field)
          source.Add(keyValuePair.Value);
      }
      if (!recursive && source.Count > 1)
        source = new List<string>()
        {
          source.First<string>()
        };
    }
    return (IEnumerable<string>) source;
  }

  public static IEnumerable<string> REGEX(
    string input,
    string pattern,
    string output,
    bool recursive = false)
  {
    List<string> stringList = new List<string>();
    if (recursive)
    {
      foreach (Match match in Regex.Matches(input, pattern))
      {
        string str = output;
        for (int groupnum = 0; groupnum < match.Groups.Count; ++groupnum)
          str = str.Replace($"[{(object) groupnum}]", match.Groups[groupnum].Value);
        stringList.Add(str);
      }
    }
    else
    {
      Match match = Regex.Match(input, pattern);
      if (match.Success)
      {
        string str = output;
        for (int groupnum = 0; groupnum < match.Groups.Count; ++groupnum)
          str = str.Replace($"[{(object) groupnum}]", match.Groups[groupnum].Value);
        stringList.Add(str);
      }
    }
    return (IEnumerable<string>) stringList;
  }

  private static string BuildLRPattern(string ls, string rs)
  {
    return $"(?<={(string.IsNullOrEmpty(ls) ? "^" : Regex.Escape(ls))}).+?(?={(string.IsNullOrEmpty(rs) ? "$" : Regex.Escape(rs))})";
  }

  private static void parseJSON(string A, string B, List<KeyValuePair<string, string>> jsonlist)
  {
    jsonlist.Add(new KeyValuePair<string, string>(A, B));
    if (B.StartsWith("["))
    {
      JArray jarray;
      try
      {
        jarray = JArray.Parse(B);
      }
      catch
      {
        return;
      }
      foreach (object child in jarray.Children())
        Parse.parseJSON("", child.ToString(), jsonlist);
    }
    if (!B.Contains("{"))
      return;
    JObject jobject;
    try
    {
      jobject = JObject.Parse(B);
    }
    catch
    {
      return;
    }
    foreach (KeyValuePair<string, JToken> keyValuePair in jobject)
      Parse.parseJSON(keyValuePair.Key, keyValuePair.Value.ToString(), jsonlist);
  }
}
