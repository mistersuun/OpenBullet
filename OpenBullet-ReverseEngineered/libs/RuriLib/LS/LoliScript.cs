// Decompiled with JetBrains decompiler
// Type: RuriLib.LS.LoliScript
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using IronPython.Compiler;
using IronPython.Hosting;
using IronPython.Runtime;
using Jint;
using Jint.Native;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Scripting.Hosting;
using RuriLib.Functions.Conditions;
using RuriLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Media;

#nullable disable
namespace RuriLib.LS;

public class LoliScript
{
  private int i;
  private string[] lines = new string[0];
  private string otherScript = "";
  private ScriptingLanguage language;

  public string Script { get; set; }

  private string[] CompressedLines
  {
    get
    {
      int index1 = 0;
      bool flag = false;
      List<string> list = ((IEnumerable<string>) this.Script.Split(new string[1]
      {
        Environment.NewLine
      }, StringSplitOptions.None)).ToList<string>();
      while (index1 < list.Count - 1)
      {
        if (!flag && BlockParser.IsBlock(list[index1]) && (list[index1 + 1].StartsWith(" ") || list[index1 + 1].StartsWith("\t")))
        {
          List<string> stringList = list;
          int index2 = index1;
          stringList[index2] = $"{stringList[index2]} {list[index1 + 1].Trim()}";
          list.RemoveAt(index1 + 1);
        }
        else if (!flag && BlockParser.IsBlock(list[index1]) && (list[index1 + 1].StartsWith("! ") || list[index1 + 1].StartsWith("!\t")))
        {
          List<string> stringList = list;
          int index3 = index1;
          stringList[index3] = $"{stringList[index3]} {list[index1 + 1].Substring(1).Trim()}";
          list.RemoveAt(index1 + 1);
        }
        else
        {
          if (list[index1].StartsWith("BEGIN SCRIPT"))
            flag = true;
          else if (list[index1].StartsWith("END SCRIPT"))
            flag = false;
          ++index1;
        }
      }
      return list.ToArray();
    }
  }

  public string CurrentLine { get; set; } = "";

  public string NextBlock
  {
    get
    {
      for (int i = this.i; i < ((IEnumerable<string>) this.lines).Count<string>(); ++i)
      {
        string line = this.lines[i];
        if (!LoliScript.IsEmptyOrCommentOrDisabled(line) && BlockParser.IsBlock(line))
        {
          string str = "";
          if (this.lines[i].StartsWith("#"))
            str = LineParser.ParseLabel(ref line);
          string token = LineParser.ParseToken(ref line, TokenType.Parameter, false, false);
          return str != "" ? $"{token} ({str})" : token;
        }
      }
      return "";
    }
  }

  public string CurrentBlock { get; set; } = "";

  public bool CanProceed
  {
    get
    {
      return this.i < ((IEnumerable<string>) this.lines).Count<string>() && ((IEnumerable<string>) this.lines).Skip<string>(this.i).Any<string>((Func<string, bool>) (l => !LoliScript.IsEmptyOrCommentOrDisabled(l)));
    }
  }

  public LoliScript() => this.Script = "";

  public LoliScript(string script) => this.Script = script;

  public List<BlockBase> ToBlocks()
  {
    List<BlockBase> blocks = new List<BlockBase>();
    string[] compressedLines = this.CompressedLines;
    List<string> lines = new List<string>();
    bool flag = false;
    foreach (string str in ((IEnumerable<string>) compressedLines).Where<string>((Func<string, bool>) (c => !string.IsNullOrEmpty(c.Trim()))))
    {
      if (!flag && BlockParser.IsBlock(str))
      {
        if (lines.Count > 0)
        {
          BlockLSCode blockLsCode = new BlockLSCode();
          blocks.Add(blockLsCode.FromLS(lines));
          lines.Clear();
        }
        try
        {
          blocks.Add(BlockParser.Parse(str));
        }
        catch (Exception ex)
        {
          throw new Exception($"Exception while parsing block {BlockBase.TruncatePretty(str, 50)}\nReason: {ex.Message}");
        }
      }
      else
      {
        lines.Add(str);
        if (str.StartsWith("BEGIN SCRIPT"))
          flag = true;
        else if (str.StartsWith("END SCRIPT"))
          flag = false;
      }
    }
    if (lines.Count > 0)
    {
      BlockLSCode blockLsCode = new BlockLSCode();
      blocks.Add(blockLsCode.FromLS(lines));
      lines.Clear();
    }
    return blocks;
  }

  public void FromBlocks(List<BlockBase> blocks)
  {
    this.Script = "";
    foreach (BlockBase block in blocks)
      this.Script = this.Script + block.ToLS() + Environment.NewLine + Environment.NewLine;
  }

  public void Reset()
  {
    this.i = 0;
    this.otherScript = "";
    this.language = ScriptingLanguage.JavaScript;
    this.lines = Regex.Split(this.Script, "\r\n|\r|\n");
  }

  public void TakeStep(BotData data)
  {
    data.LogBuffer.Clear();
    if (data.Status != BotStatus.NONE && data.Status != BotStatus.SUCCESS)
    {
      this.i = ((IEnumerable<string>) this.lines).Count<string>();
    }
    else
    {
      while (true)
      {
        this.CurrentLine = this.lines[this.i];
        if (LoliScript.IsEmptyOrCommentOrDisabled(this.CurrentLine))
          ++this.i;
        else
          break;
      }
      int num1;
      for (num1 = 0; this.i + 1 + num1 < ((IEnumerable<string>) this.lines).Count<string>(); ++num1)
      {
        string line = this.lines[this.i + 1 + num1];
        if (line.StartsWith(" ") || line.StartsWith("\t"))
          this.CurrentLine = $"{this.CurrentLine} {line.Trim()}";
        else
          break;
      }
      try
      {
        if (BlockParser.IsBlock(this.CurrentLine))
        {
          BlockBase blockBase = (BlockBase) null;
          try
          {
            blockBase = BlockParser.Parse(this.CurrentLine);
            this.CurrentBlock = blockBase.Label;
            if (!blockBase.Disabled)
              blockBase.Process(data);
          }
          catch (Exception ex)
          {
            data.LogBuffer.Add(new LogEntry("ERROR: " + ex.Message, Colors.Tomato));
            if (blockBase != null)
            {
              if (!(blockBase.GetType() == typeof (BlockRequest)) && !(blockBase.GetType() == typeof (BlockBypassCF)) && !(blockBase.GetType() == typeof (BlockImageCaptcha)))
              {
                if (!(blockBase.GetType() == typeof (BlockRecaptcha)))
                  goto label_53;
              }
              data.Status = BotStatus.ERROR;
              throw new BlockProcessingException(ex.Message);
            }
          }
        }
        else if (CommandParser.IsCommand(this.CurrentLine))
        {
          try
          {
            Action action = CommandParser.Parse(this.CurrentLine, data);
            if (action != null)
              action();
          }
          catch (Exception ex)
          {
            data.LogBuffer.Add(new LogEntry("ERROR: " + ex.Message, Colors.Tomato));
            data.Status = BotStatus.ERROR;
          }
        }
        else
        {
          string currentLine = this.CurrentLine;
          string upper = LineParser.ParseToken(ref currentLine, TokenType.Parameter, false).ToUpper();
          // ISSUE: reference to a compiler-generated method
          switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(upper))
          {
            case 1058172254:
              if (upper == "BEGIN")
              {
                if (LineParser.ParseToken(ref currentLine, TokenType.Parameter, true).ToUpper() == "SCRIPT")
                {
                  // ISSUE: reference to a compiler-generated field
                  if (LoliScript.\u003C\u003Eo__27.\u003C\u003Ep__0 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    LoliScript.\u003C\u003Eo__27.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, ScriptingLanguage>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (ScriptingLanguage), typeof (LoliScript)));
                  }
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  this.language = LoliScript.\u003C\u003Eo__27.\u003C\u003Ep__0.Target((CallSite) LoliScript.\u003C\u003Eo__27.\u003C\u003Ep__0, LineParser.ParseEnum(ref currentLine, "LANGUAGE", typeof (ScriptingLanguage)));
                  int num2;
                  try
                  {
                    num2 = LoliScript.ScanFor(this.lines, this.i, true, new string[1]
                    {
                      "END"
                    }) - 1;
                  }
                  catch
                  {
                    throw new Exception("No 'END SCRIPT' specified");
                  }
                  this.otherScript = string.Join(Environment.NewLine, ((IEnumerable<string>) this.lines).Skip<string>(this.i + 1).Take<string>(num2 - this.i));
                  this.i = num2;
                  data.LogBuffer.Add(new LogEntry($"Jumping to line {this.i + 2}", Colors.White));
                  break;
                }
                break;
              }
              break;
            case 1348155789:
              if (upper == "JUMP")
              {
                string str = "";
                try
                {
                  str = LineParser.ParseToken(ref currentLine, TokenType.Label, true);
                  this.i = LoliScript.ScanFor(this.lines, -1, true, new string[1]
                  {
                    str ?? ""
                  }) - 1;
                  data.LogBuffer.Add(new LogEntry($"Jumping to line {this.i + 2}", Colors.White));
                  break;
                }
                catch
                {
                  throw new Exception($"No block with label {str} was found");
                }
              }
              else
                break;
            case 1491660422:
              if (upper == "IF")
              {
                if (!LoliScript.ParseCheckCondition(ref currentLine, data))
                {
                  this.i = LoliScript.ScanFor(this.lines, this.i, true, new string[2]
                  {
                    "ENDIF",
                    "ELSE"
                  });
                  data.LogBuffer.Add(new LogEntry($"Jumping to line {this.i + 1}", Colors.White));
                  break;
                }
                break;
              }
              break;
            case 1777562416:
              if (upper == "ELSE")
              {
                this.i = LoliScript.ScanFor(this.lines, this.i, true, new string[1]
                {
                  "ENDIF"
                });
                data.LogBuffer.Add(new LogEntry($"Jumping to line {this.i + 1}", Colors.White));
                break;
              }
              break;
            case 2583867822:
              if (upper == "WHILE")
              {
                if (!LoliScript.ParseCheckCondition(ref currentLine, data))
                {
                  this.i = LoliScript.ScanFor(this.lines, this.i, true, new string[1]
                  {
                    "ENDWHILE"
                  });
                  data.LogBuffer.Add(new LogEntry($"Jumping to line {this.i + 1}", Colors.White));
                  break;
                }
                break;
              }
              break;
            case 2844291375:
              if (upper == "ENDWHILE")
              {
                this.i = LoliScript.ScanFor(this.lines, this.i, false, new string[1]
                {
                  "WHILE"
                }) - 1;
                data.LogBuffer.Add(new LogEntry($"Jumping to line {this.i + 1}", Colors.White));
                break;
              }
              break;
            case 2940441098:
              if (upper == "END")
              {
                if (LineParser.ParseToken(ref currentLine, TokenType.Parameter, true).ToUpper() == "SCRIPT")
                {
                  LineParser.EnsureIdentifier(ref currentLine, "->");
                  LineParser.EnsureIdentifier(ref currentLine, "VARS");
                  string literal = LineParser.ParseLiteral(ref currentLine, "OUTPUTS");
                  try
                  {
                    if (this.otherScript != "")
                    {
                      this.RunScript(this.otherScript, this.language, literal, data);
                      break;
                    }
                    break;
                  }
                  catch (Exception ex)
                  {
                    data.LogBuffer.Add(new LogEntry("The script failed to be executed: " + ex.Message, Colors.Tomato));
                    break;
                  }
                }
                else
                  break;
              }
              else
                break;
            case 4075851821:
              if (upper == "ENDIF")
                break;
              break;
          }
        }
      }
      catch (BlockProcessingException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new Exception($"Parsing Exception on line {this.i + 1}: {ex.Message}");
      }
label_53:
      this.i += 1 + num1;
    }
  }

  private static bool IsEmptyOrCommentOrDisabled(string line)
  {
    try
    {
      return line.Trim() == "" || line.StartsWith("##") || line.StartsWith("!");
    }
    catch
    {
      return true;
    }
  }

  public static int ScanFor(string[] lines, int current, bool downwards, string[] options)
  {
    int index = downwards ? current + 1 : current - 1;
    bool flag = false;
    while (index >= 0)
    {
      if (index < ((IEnumerable<string>) lines).Count<string>())
      {
        try
        {
          string token = LineParser.ParseToken(ref lines[index], TokenType.Parameter, false, false);
          if (((IEnumerable<string>) options).Any<string>((Func<string, bool>) (o => token.ToUpper() == o.ToUpper())))
          {
            flag = true;
            break;
          }
        }
        catch
        {
        }
        if (downwards)
          ++index;
        else
          --index;
      }
      else
        break;
    }
    if (flag)
      return index;
    throw new Exception("Not found");
  }

  public static bool ParseCheckCondition(ref string cfLine, BotData data)
  {
    string literal = LineParser.ParseLiteral(ref cfLine, "STRING");
    // ISSUE: reference to a compiler-generated field
    if (LoliScript.\u003C\u003Eo__30.\u003C\u003Ep__0 == null)
    {
      // ISSUE: reference to a compiler-generated field
      LoliScript.\u003C\u003Eo__30.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, Comparer>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (Comparer), typeof (LoliScript)));
    }
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    Comparer comparer = LoliScript.\u003C\u003Eo__30.\u003C\u003Ep__0.Target((CallSite) LoliScript.\u003C\u003Eo__30.\u003C\u003Ep__0, LineParser.ParseEnum(ref cfLine, "Comparer", typeof (Comparer)));
    string str = "";
    if (comparer != Comparer.Exists && comparer != Comparer.DoesNotExist)
      str = LineParser.ParseLiteral(ref cfLine, "STRING");
    int num = (int) comparer;
    string right = str;
    BotData data1 = data;
    return Condition.ReplaceAndVerify(literal, (Comparer) num, right, data1);
  }

  private void RunScript(string script, ScriptingLanguage language, string outputs, BotData data)
  {
    StringWriter stringWriter = new StringWriter();
    Console.SetOut((TextWriter) stringWriter);
    Console.SetError((TextWriter) stringWriter);
    List<string> stringList = new List<string>();
    if (outputs != "")
    {
      try
      {
        stringList = ((IEnumerable<string>) outputs.Split(',')).Select<string, string>((Func<string, string>) (x => x.Trim())).ToList<string>();
      }
      catch
      {
      }
    }
    DateTime now = DateTime.Now;
    try
    {
      switch (language)
      {
        case ScriptingLanguage.JavaScript:
          Engine engine1 = new Engine().SetValue("log", (Delegate) new Action<object>(Console.WriteLine));
          foreach (CVar cvar in data.Variables.All)
          {
            try
            {
              if (cvar.Type == CVar.VarType.List)
              {
                engine1.SetValue(cvar.Name, (object) (cvar.Value as List<string>).ToArray());
              }
              else
              {
                // ISSUE: reference to a compiler-generated field
                if (LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__0 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__0 = CallSite<Action<CallSite, Engine, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetValue", (IEnumerable<Type>) null, typeof (LoliScript), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__0.Target((CallSite) LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__0, engine1, cvar.Name, cvar.Value);
              }
            }
            catch
            {
            }
          }
          engine1.Execute(script);
          data.Log(new LogEntry("DEBUG LOG: " + stringWriter.ToString(), Colors.White));
          data.Log(new LogEntry($"Parsing {stringList.Count} variables", Colors.White));
          foreach (string str in stringList)
          {
            try
            {
              JsValue jsValue = engine1.Global.GetProperty(str).Value;
              if (jsValue.IsArray())
                data.Variables.Set(new CVar(str, CVar.VarType.List, (object) jsValue.TryCast<List<string>>()));
              else
                data.Variables.Set(new CVar(str, CVar.VarType.Single, (object) jsValue.ToString()));
              data.Log(new LogEntry($"SET VARIABLE {str} WITH VALUE {jsValue.ToString()}", Colors.Yellow));
            }
            catch
            {
              data.Log(new LogEntry("COULD NOT FIND VARIABLE " + str, Colors.Tomato));
            }
          }
          if (engine1.GetCompletionValue() != (JsValue) null)
          {
            data.Log(new LogEntry($"Completion value: {engine1.GetCompletionValue()}", Colors.White));
            break;
          }
          break;
        case ScriptingLanguage.IronPython:
          ScriptEngine engine2 = Python.CreateRuntime().GetEngine("py");
          ((PythonCompilerOptions) engine2.GetCompilerOptions()).Module &= ~ModuleOptions.Optimized;
          ScriptScope scope = engine2.CreateScope();
          ScriptSource sourceFromString = engine2.CreateScriptSourceFromString(script);
          foreach (CVar cvar in data.Variables.All)
          {
            try
            {
              // ISSUE: reference to a compiler-generated field
              if (LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__1 == null)
              {
                // ISSUE: reference to a compiler-generated field
                LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__1 = CallSite<Action<CallSite, ScriptScope, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetVariable", (IEnumerable<Type>) null, typeof (LoliScript), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__1.Target((CallSite) LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__1, scope, cvar.Name, cvar.Value);
            }
            catch
            {
            }
          }
          object obj1 = sourceFromString.Execute(scope);
          data.Log(new LogEntry("DEBUG LOG: " + stringWriter.ToString(), Colors.White));
          data.Log(new LogEntry($"Parsing {stringList.Count} variables", Colors.White));
          foreach (string name in stringList)
          {
            try
            {
              object variable1 = scope.GetVariable(name);
              VariableList variables = data.Variables;
              // ISSUE: reference to a compiler-generated field
              if (LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__6 == null)
              {
                // ISSUE: reference to a compiler-generated field
                LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__6 = CallSite<Func<CallSite, Type, string, CVar.VarType, object, CVar>>.Create(Binder.InvokeConstructor(CSharpBinderFlags.None, typeof (LoliScript), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, Type, string, CVar.VarType, object, CVar> target1 = LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__6.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, Type, string, CVar.VarType, object, CVar>> p6 = LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__6;
              Type type1 = typeof (CVar);
              string str = name;
              // ISSUE: reference to a compiler-generated field
              if (LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__4 == null)
              {
                // ISSUE: reference to a compiler-generated field
                LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (LoliScript), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, object, bool> target2 = LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__4.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, object, bool>> p4 = LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__4;
              // ISSUE: reference to a compiler-generated field
              if (LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__3 == null)
              {
                // ISSUE: reference to a compiler-generated field
                LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, Type, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (LoliScript), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, object, Type, object> target3 = LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__3.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, object, Type, object>> p3 = LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__3;
              // ISSUE: reference to a compiler-generated field
              if (LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__2 == null)
              {
                // ISSUE: reference to a compiler-generated field
                LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "GetType", (IEnumerable<Type>) null, typeof (LoliScript), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              object obj2 = LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__2.Target((CallSite) LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__2, variable1);
              Type type2 = typeof (string[]);
              object obj3 = target3((CallSite) p3, obj2, type2);
              int num = target2((CallSite) p4, obj3) ? 1 : 0;
              // ISSUE: reference to a compiler-generated field
              if (LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__5 == null)
              {
                // ISSUE: reference to a compiler-generated field
                LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", (IEnumerable<Type>) null, typeof (LoliScript), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              object obj4 = LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__5.Target((CallSite) LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__5, variable1);
              CVar variable2 = target1((CallSite) p6, type1, str, (CVar.VarType) num, obj4);
              variables.Set(variable2);
              data.Log(new LogEntry($"SET VARIABLE {name} WITH VALUE {variable1}", Colors.Yellow));
            }
            catch
            {
              data.Log(new LogEntry("COULD NOT FIND VARIABLE " + name, Colors.Tomato));
            }
          }
          // ISSUE: reference to a compiler-generated field
          if (LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__8 == null)
          {
            // ISSUE: reference to a compiler-generated field
            LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (LoliScript), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, bool> target = LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__8.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, bool>> p8 = LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__8;
          // ISSUE: reference to a compiler-generated field
          if (LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__7 == null)
          {
            // ISSUE: reference to a compiler-generated field
            LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (LoliScript), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj5 = LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__7.Target((CallSite) LoliScript.\u003C\u003Eo__31.\u003C\u003Ep__7, obj1, (object) null);
          if (target((CallSite) p8, obj5))
          {
            data.Log(new LogEntry($"Completion value: {obj1}", Colors.White));
            break;
          }
          break;
      }
      data.Log(new LogEntry($"Execution completed in {(DateTime.Now - now).TotalSeconds} seconds", Colors.GreenYellow));
    }
    catch (Exception ex)
    {
      data.Log(new LogEntry("[ERROR] INFO: " + ex.Message, Colors.White));
    }
    finally
    {
      StreamWriter newOut = new StreamWriter(Console.OpenStandardOutput());
      StreamWriter newError = new StreamWriter(Console.OpenStandardError());
      newOut.AutoFlush = true;
      newError.AutoFlush = true;
      Console.SetOut((TextWriter) newOut);
      Console.SetError((TextWriter) newError);
    }
  }
}
