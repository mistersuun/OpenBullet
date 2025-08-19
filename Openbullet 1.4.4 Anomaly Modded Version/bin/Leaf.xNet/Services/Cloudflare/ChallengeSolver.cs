// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.Services.Cloudflare.ChallengeSolver
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace Leaf.xNet.Services.Cloudflare;

public static class ChallengeSolver
{
  private const string IntegerSolutionTag = "parseInt(";
  private const string ScriptPattern = "<script\\b[^>]*>(?<Content>.*?)<\\/script>";
  private const string ZeroPattern = "\\[\\]";
  private const string OnePattern = "\\!\\+\\[\\]|\\!\\!\\[\\]";
  private const string DigitPattern = "\\(?(\\+?(\\!\\+\\[\\]|\\!\\!\\[\\]|\\[\\]))+\\)?";
  private const string NumberPattern = "\\+?\\(?(?<Digits>\\+?\\(?(\\+?(\\!\\+\\[\\]|\\!\\!\\[\\]|\\[\\]))+\\)?)+\\)?";
  private const string OperatorPattern = "(?<Operator>[\\+|\\-|\\*|\\/])\\=?";
  private const string StepPattern = "((?<Operator>[\\+|\\-|\\*|\\/])\\=?)??(?<Number>\\+?\\(?(?<Digits>\\+?\\(?(\\+?(\\!\\+\\[\\]|\\!\\!\\[\\]|\\[\\]))+\\)?)+\\)?)";

  public static ChallengeSolution Solve(
    string challengePageContent,
    string targetHost,
    int targetPort)
  {
    bool containsIntegerTag;
    double num1 = ChallengeSolver.DecodeSecretNumber(challengePageContent, targetHost, targetPort, out containsIntegerTag);
    string str1 = Regex.Match(challengePageContent, "name=\"jschl_vc\" value=\"(?<jschl_vc>[^\"]+)").Groups["jschl_vc"].Value;
    string str2 = Regex.Match(challengePageContent, "name=\"pass\" value=\"(?<pass>[^\"]+)").Groups["pass"].Value;
    string clearancePage = Regex.Match(challengePageContent, "id=\"challenge-form\" action=\"(?<action>[^\"]+)").Groups["action"].Value;
    string str3 = Regex.Match(challengePageContent, "name=\"s\" value=\"(?<s>[^\"]+)").Groups["s"].Value;
    string verificationCode = str1;
    string pass = str2;
    double answer = num1;
    string s = str3;
    int num2 = containsIntegerTag ? 1 : 0;
    return new ChallengeSolution(clearancePage, verificationCode, pass, answer, s, num2 != 0);
  }

  private static double DecodeSecretNumber(
    string challengePageContent,
    string targetHost,
    int targetPort,
    out bool containsIntegerTag)
  {
    string str = Regex.Matches(challengePageContent, "<script\\b[^>]*>(?<Content>.*?)<\\/script>", RegexOptions.Singleline).Cast<Match>().Select<Match, string>((Func<Match, string>) (m => m.Groups["Content"].Value)).First<string>((Func<string, bool>) (c => c.Contains("jschl-answer")));
    List<Tuple<string, double>> list = ((IEnumerable<string>) str.Split(';')).Select<string, IEnumerable<Tuple<string, double>>>(new Func<string, IEnumerable<Tuple<string, double>>>(ChallengeSolver.GetSteps)).Where<IEnumerable<Tuple<string, double>>>((Func<IEnumerable<Tuple<string, double>>, bool>) (g => g.Any<Tuple<string, double>>())).ToList<IEnumerable<Tuple<string, double>>>().Select<IEnumerable<Tuple<string, double>>, Tuple<string, double>>(new Func<IEnumerable<Tuple<string, double>>, Tuple<string, double>>(ChallengeSolver.ResolveStepGroup)).ToList<Tuple<string, double>>();
    double seed = list.First<Tuple<string, double>>().Item2;
    double num = Math.Round(list.Skip<Tuple<string, double>>(1).Aggregate<Tuple<string, double>, double>(seed, new Func<double, Tuple<string, double>, double>(ChallengeSolver.ApplyDecodingStep)), 10) + (double) targetHost.Length;
    if (targetPort != 80 /*0x50*/ && targetPort != 443)
      num += (double) (targetPort.ToString().Length + 1);
    containsIntegerTag = str.Contains("parseInt(");
    return !containsIntegerTag ? num : (double) (int) num;
  }

  private static Tuple<string, double> ResolveStepGroup(IEnumerable<Tuple<string, double>> group)
  {
    List<Tuple<string, double>> list = group.ToList<Tuple<string, double>>();
    string str = list.First<Tuple<string, double>>().Item1;
    double seed = list.First<Tuple<string, double>>().Item2;
    double num = list.Skip<Tuple<string, double>>(1).Aggregate<Tuple<string, double>, double>(seed, new Func<double, Tuple<string, double>, double>(ChallengeSolver.ApplyDecodingStep));
    return Tuple.Create<string, double>(str, num);
  }

  private static IEnumerable<Tuple<string, double>> GetSteps(string text)
  {
    return (IEnumerable<Tuple<string, double>>) Regex.Matches(text, "((?<Operator>[\\+|\\-|\\*|\\/])\\=?)??(?<Number>\\+?\\(?(?<Digits>\\+?\\(?(\\+?(\\!\\+\\[\\]|\\!\\!\\[\\]|\\[\\]))+\\)?)+\\)?)").Cast<Match>().Select<Match, Tuple<string, double>>((Func<Match, Tuple<string, double>>) (s => Tuple.Create<string, double>(s.Groups["Operator"].Value, ChallengeSolver.DeobfuscateNumber(s.Groups["Number"].Value)))).ToList<Tuple<string, double>>();
  }

  private static double DeobfuscateNumber(string obfuscatedNumber)
  {
    IEnumerable<int> values = Regex.Match(obfuscatedNumber, "\\+?\\(?(?<Digits>\\+?\\(?(\\+?(\\!\\+\\[\\]|\\!\\!\\[\\]|\\[\\]))+\\)?)+\\)?").Groups["Digits"].Captures.Cast<Capture>().Select<Capture, int>((Func<Capture, int>) (c => Regex.Matches(c.Value, "\\!\\+\\[\\]|\\!\\!\\[\\]").Count));
    return double.Parse(string.Join<int>(string.Empty, values));
  }

  private static double ApplyDecodingStep(double number, Tuple<string, double> step)
  {
    string str = step.Item1;
    double num = step.Item2;
    switch (str)
    {
      case "+":
        return number + num;
      case "-":
        return number - num;
      case "*":
        return number * num;
      case "/":
        return number / num;
      default:
        throw new ArgumentOutOfRangeException("Unknown operator: " + str);
    }
  }
}
