// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonWarnings
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace IronPython.Modules;

public static class PythonWarnings
{
  public const string __doc__ = "Provides low-level functionality for reporting warnings";
  private static readonly object _keyFields = new object();
  private static readonly string _keyDefaultAction = "default_action";
  private static readonly string _keyFilters = "filters";
  private static readonly string _keyOnceRegistry = "once_registry";
  public static PythonDictionary MODULE_STATE;

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    IronPython.Runtime.List defaultFilters = new IronPython.Runtime.List();
    if (!context.PythonOptions.WarnPython30 && context.PythonOptions.DivisionOptions != PythonDivisionOptions.Warn && context.PythonOptions.DivisionOptions != PythonDivisionOptions.WarnAll)
      defaultFilters.AddNoLock((object) PythonTuple.MakeTuple((object) "ignore", null, (object) PythonExceptions.DeprecationWarning, null, (object) 0));
    defaultFilters.AddNoLock((object) PythonTuple.MakeTuple((object) "ignore", null, (object) PythonExceptions.PendingDeprecationWarning, null, (object) 0));
    defaultFilters.AddNoLock((object) PythonTuple.MakeTuple((object) "ignore", null, (object) PythonExceptions.ImportWarning, null, (object) 0));
    defaultFilters.AddNoLock((object) PythonTuple.MakeTuple((object) "ignore", null, (object) PythonExceptions.BytesWarning, null, (object) 0));
    context.GetOrCreateModuleState<PythonDictionary>(PythonWarnings._keyFields, (Func<PythonDictionary>) (() =>
    {
      dict.Add((object) PythonWarnings._keyDefaultAction, (object) "default");
      dict.Add((object) PythonWarnings._keyOnceRegistry, (object) new PythonDictionary());
      dict.Add((object) PythonWarnings._keyFilters, (object) defaultFilters);
      return dict;
    }));
  }

  public static void warn(
    CodeContext context,
    object message,
    PythonType category = null,
    int stacklevel = 1)
  {
    IronPython.Runtime.List systemStateValue = context.LanguageContext.GetSystemStateValue("argv") as IronPython.Runtime.List;
    if (PythonOps.IsInstance(message, PythonExceptions.Warning))
      category = DynamicHelpers.GetPythonType(message);
    if (category == null)
      category = PythonExceptions.UserWarning;
    if (!category.IsSubclassOf(PythonExceptions.Warning))
      throw PythonOps.ValueError("category is not a subclass of Warning");
    TraceBackFrame traceBackFrame = (TraceBackFrame) null;
    if (context.LanguageContext.PythonOptions.Frames)
    {
      try
      {
        traceBackFrame = SysModule._getframeImpl(context, stacklevel - 1);
      }
      catch (ValueErrorException ex)
      {
      }
    }
    PythonDictionary module_globals;
    int lineno;
    if (traceBackFrame == null)
    {
      module_globals = Builtin.globals(context);
      lineno = 1;
    }
    else
    {
      module_globals = traceBackFrame.f_globals;
      lineno = (int) traceBackFrame.f_lineno;
    }
    string module = module_globals == null || !module_globals.ContainsKey((object) "__name__") ? "<string>" : (string) module_globals.get((object) "__name__");
    if (!(module_globals.get((object) "__file__") is string filename) || filename == "")
    {
      if (module == "__main__")
        filename = systemStateValue == null || systemStateValue.Count <= 0 ? "__main__" : systemStateValue[0] as string;
      if (filename == null || filename == "")
        filename = module;
    }
    PythonDictionary registry = (PythonDictionary) module_globals.setdefault((object) "__warningregistry__", (object) new PythonDictionary());
    PythonWarnings.warn_explicit(context, message, category, filename, lineno, module, registry, (object) module_globals);
  }

  public static void warn_explicit(
    CodeContext context,
    object message,
    PythonType category,
    string filename,
    int lineno,
    string module = null,
    PythonDictionary registry = null,
    object module_globals = null)
  {
    PythonContext languageContext = context.LanguageContext;
    PythonDictionary moduleState = (PythonDictionary) languageContext.GetModuleState(PythonWarnings._keyFields);
    object warningsModule = languageContext.GetWarningsModule();
    if (string.IsNullOrEmpty(module))
    {
      module = filename == null || filename == "" ? "<unknown>" : filename;
      if (module.EndsWith(".py"))
        module = module.Substring(0, module.Length - 3);
    }
    if (registry == null)
      registry = new PythonDictionary();
    PythonExceptions.BaseException baseException;
    string text;
    if (PythonOps.IsInstance(message, PythonExceptions.Warning))
    {
      baseException = (PythonExceptions.BaseException) message;
      text = baseException.ToString();
      category = DynamicHelpers.GetPythonType((object) baseException);
    }
    else
    {
      text = message.ToString();
      baseException = PythonExceptions.CreatePythonThrowable(category, (object) message.ToString());
    }
    PythonTuple key1 = PythonTuple.MakeTuple((object) text, (object) category, (object) lineno);
    if (registry.ContainsKey((object) key1))
      return;
    string str = Converter.ConvertToString(moduleState[(object) PythonWarnings._keyDefaultAction]);
    PythonTuple pythonTuple1 = (PythonTuple) null;
    bool flag = false;
    boundAttr1 = (IronPython.Runtime.List) moduleState[(object) PythonWarnings._keyFilters];
    if (warningsModule != null && !(PythonOps.GetBoundAttr(context, warningsModule, "filters") is IronPython.Runtime.List boundAttr1))
      throw PythonOps.ValueError("_warnings.filters must be a list");
    foreach (PythonTuple pythonTuple2 in boundAttr1)
    {
      pythonTuple1 = pythonTuple2;
      str = (string) pythonTuple2._data[0];
      PythonRegex.RE_Pattern rePattern1 = (PythonRegex.RE_Pattern) pythonTuple2._data[1];
      PythonType other = (PythonType) pythonTuple2._data[2];
      PythonRegex.RE_Pattern rePattern2 = (PythonRegex.RE_Pattern) pythonTuple2._data[3];
      int num = !(pythonTuple2._data[4] is int) ? (int) (Extensible<int>) pythonTuple2._data[4] : (int) pythonTuple2._data[4];
      if ((rePattern1 == null || rePattern1.match((object) text) != null) && category.IsSubclassOf(other) && (rePattern2 == null || rePattern2.match((object) module) != null) && (num == 0 || num == lineno))
      {
        flag = true;
        break;
      }
    }
    if (!flag)
      str = Converter.ConvertToString(moduleState[(object) PythonWarnings._keyDefaultAction]);
    switch (str)
    {
      case "ignore":
        registry.Add((object) key1, (object) 1);
        break;
      case "error":
        throw baseException.GetClrException();
      case "once":
        registry.Add((object) key1, (object) 1);
        PythonTuple key2 = PythonTuple.MakeTuple((object) text, (object) category);
        PythonDictionary pythonDictionary = (PythonDictionary) moduleState[(object) PythonWarnings._keyOnceRegistry];
        if (pythonDictionary.ContainsKey((object) key2))
          break;
        pythonDictionary.Add((object) key1, (object) 1);
        goto case "always";
      case "always":
        if (warningsModule != null)
        {
          object boundAttr2 = PythonOps.GetBoundAttr(context, warningsModule, "showwarning");
          if (boundAttr2 != null)
          {
            PythonCalls.Call(context, boundAttr2, (object) baseException, (object) category, (object) filename, (object) lineno, null, null);
            break;
          }
          PythonWarnings.showwarning(context, (object) baseException, category, filename, lineno, (object) null, (string) null);
          break;
        }
        PythonWarnings.showwarning(context, (object) baseException, category, filename, lineno, (object) null, (string) null);
        break;
      case nameof (module):
        registry.Add((object) key1, (object) 1);
        PythonTuple key3 = PythonTuple.MakeTuple((object) text, (object) category, (object) 0);
        if (registry.ContainsKey((object) key3))
          break;
        registry.Add((object) key3, (object) 1);
        goto case "always";
      case "default":
        registry.Add((object) key1, (object) 1);
        goto case "always";
      default:
        throw PythonOps.RuntimeError("Unrecognized action ({0}) in warnings.filters:\n {1}", (object) str, (object) pythonTuple1);
    }
  }

  internal static string formatwarning(
    object message,
    PythonType category,
    string filename,
    int lineno,
    string line = null)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendFormat("{0}:{1}: {2}: {3}\n", (object) filename, (object) lineno, (object) category.Name, message);
    if (line == null && lineno > 0 && File.Exists(filename))
    {
      using (FileStream fileStream = File.OpenRead(filename))
      {
        using (StreamReader streamReader = new StreamReader((Stream) fileStream))
        {
          for (int index = 0; index < lineno - 1; ++index)
            streamReader.ReadLine();
          line = streamReader.ReadLine();
        }
      }
    }
    if (line != null)
      stringBuilder.AppendFormat("  {0}\n", (object) line.strip());
    return stringBuilder.ToString();
  }

  internal static void showwarning(
    CodeContext context,
    object message,
    PythonType category,
    string filename,
    int lineno,
    object file = null,
    string line = null)
  {
    string s = PythonWarnings.formatwarning(message, category, filename, lineno, line);
    try
    {
      switch (file)
      {
        case null:
          if (context.LanguageContext.GetSystemStateValue("stderr") is PythonFile systemStateValue)
          {
            systemStateValue.write(s);
            break;
          }
          Console.Error.Write(s);
          break;
        case PythonFile _:
          ((PythonFile) file).write(s);
          break;
        case TextWriter _:
          ((TextWriter) file).Write(s);
          break;
      }
    }
    catch (IOException ex)
    {
    }
  }
}
