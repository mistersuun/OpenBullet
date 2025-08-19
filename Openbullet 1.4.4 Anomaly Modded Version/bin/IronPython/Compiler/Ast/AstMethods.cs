// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.AstMethods
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace IronPython.Compiler.Ast;

internal static class AstMethods
{
  public static readonly MethodInfo IsTrue = AstMethods.GetMethod((Delegate) new Func<object, bool>(PythonOps.IsTrue));
  public static readonly MethodInfo RaiseAssertionError = AstMethods.GetMethod((Delegate) new Action<CodeContext, object>(PythonOps.RaiseAssertionError));
  public static readonly MethodInfo RaiseAssertionErrorNoMessage = AstMethods.GetMethod((Delegate) new Action<CodeContext>(PythonOps.RaiseAssertionError));
  public static readonly MethodInfo Repr = AstMethods.GetMethod((Delegate) new Func<CodeContext, object, string>(PythonOps.Repr));
  public static readonly MethodInfo WarnDivision = AstMethods.GetMethod((Delegate) new Action<CodeContext, PythonDivisionOptions, object, object>(PythonOps.WarnDivision));
  public static readonly MethodInfo MakeClass = AstMethods.GetMethod((Delegate) new Func<FunctionCode, Func<CodeContext, CodeContext>, CodeContext, string, object[], string, object>(PythonOps.MakeClass));
  public static readonly MethodInfo UnqualifiedExec = AstMethods.GetMethod((Delegate) new Action<CodeContext, object>(PythonOps.UnqualifiedExec));
  public static readonly MethodInfo QualifiedExec = AstMethods.GetMethod((Delegate) new Action<CodeContext, object, PythonDictionary, object>(PythonOps.QualifiedExec));
  public static readonly MethodInfo PrintExpressionValue = AstMethods.GetMethod((Delegate) new Action<CodeContext, object>(PythonOps.PrintExpressionValue));
  public static readonly MethodInfo PrintCommaWithDest = AstMethods.GetMethod((Delegate) new Action<CodeContext, object, object>(PythonOps.PrintCommaWithDest));
  public static readonly MethodInfo PrintWithDest = AstMethods.GetMethod((Delegate) new Action<CodeContext, object, object>(PythonOps.PrintWithDest));
  public static readonly MethodInfo PrintComma = AstMethods.GetMethod((Delegate) new Action<CodeContext, object>(PythonOps.PrintComma));
  public static readonly MethodInfo Print = AstMethods.GetMethod((Delegate) new Action<CodeContext, object>(PythonOps.Print));
  public static readonly MethodInfo ImportWithNames = AstMethods.GetMethod((Delegate) new Func<CodeContext, string, string[], int, object>(PythonOps.ImportWithNames));
  public static readonly MethodInfo ImportFrom = AstMethods.GetMethod((Delegate) new Func<CodeContext, object, string, object>(PythonOps.ImportFrom));
  public static readonly MethodInfo ImportStar = AstMethods.GetMethod((Delegate) new Action<CodeContext, string, int>(PythonOps.ImportStar));
  public static readonly MethodInfo SaveCurrentException = AstMethods.GetMethod((Delegate) new Func<Exception>(PythonOps.SaveCurrentException));
  public static readonly MethodInfo RestoreCurrentException = AstMethods.GetMethod((Delegate) new Action<Exception>(PythonOps.RestoreCurrentException));
  public static readonly MethodInfo MakeGeneratorExpression = AstMethods.GetMethod((Delegate) new Func<object, object, object>(PythonOps.MakeGeneratorExpression));
  public static readonly MethodInfo ListAddForComprehension = AstMethods.GetMethod((Delegate) new Action<List, object>(PythonOps.ListAddForComprehension));
  public static readonly MethodInfo SetAddForComprehension = AstMethods.GetMethod((Delegate) new Action<SetCollection, object>(PythonOps.SetAddForComprehension));
  public static readonly MethodInfo DictAddForComprehension = AstMethods.GetMethod((Delegate) new Action<PythonDictionary, object, object>(PythonOps.DictAddForComprehension));
  public static readonly MethodInfo MakeEmptyListFromCode = AstMethods.GetMethod((Delegate) new Func<List>(PythonOps.MakeEmptyListFromCode));
  public static readonly MethodInfo CheckUninitialized = AstMethods.GetMethod((Delegate) new Func<object, string, object>(PythonOps.CheckUninitialized));
  public static readonly MethodInfo PrintNewlineWithDest = AstMethods.GetMethod((Delegate) new Action<CodeContext, object>(PythonOps.PrintNewlineWithDest));
  public static readonly MethodInfo PrintNewline = AstMethods.GetMethod((Delegate) new Action<CodeContext>(PythonOps.PrintNewline));
  public static readonly MethodInfo PublishModule = AstMethods.GetMethod((Delegate) new Func<CodeContext, string, object>(PythonOps.PublishModule));
  public static readonly MethodInfo RemoveModule = AstMethods.GetMethod((Delegate) new Action<CodeContext, string, object>(PythonOps.RemoveModule));
  public static readonly MethodInfo ModuleStarted = AstMethods.GetMethod((Delegate) new Action<CodeContext, ModuleOptions>(PythonOps.ModuleStarted));
  public static readonly MethodInfo MakeRethrownException = AstMethods.GetMethod((Delegate) new Func<CodeContext, Exception>(PythonOps.MakeRethrownException));
  public static readonly MethodInfo MakeRethrowExceptionWorker = AstMethods.GetMethod((Delegate) new Func<Exception, Exception>(PythonOps.MakeRethrowExceptionWorker));
  public static readonly MethodInfo MakeException = AstMethods.GetMethod((Delegate) new Func<CodeContext, object, object, object, Exception>(PythonOps.MakeException));
  public static readonly MethodInfo MakeSlice = AstMethods.GetMethod((Delegate) new Func<object, object, object, Slice>(PythonOps.MakeSlice));
  public static readonly MethodInfo ExceptionHandled = AstMethods.GetMethod((Delegate) new Action<CodeContext>(PythonOps.ExceptionHandled));
  public static readonly MethodInfo GetExceptionInfoLocal = AstMethods.GetMethod((Delegate) new Func<CodeContext, Exception, PythonTuple>(PythonOps.GetExceptionInfoLocal));
  public static readonly MethodInfo CheckException = AstMethods.GetMethod((Delegate) new Func<CodeContext, object, object, object>(PythonOps.CheckException));
  public static readonly MethodInfo SetCurrentException = AstMethods.GetMethod((Delegate) new Func<CodeContext, Exception, object>(PythonOps.SetCurrentException));
  public static readonly MethodInfo BuildExceptionInfo = AstMethods.GetMethod((Delegate) new Action<CodeContext, Exception>(PythonOps.BuildExceptionInfo));
  public static readonly MethodInfo MakeTuple = AstMethods.GetMethod((Delegate) new Func<object[], PythonTuple>(PythonOps.MakeTuple));
  public static readonly MethodInfo IsNot = AstMethods.GetMethod((Delegate) new Func<object, object, object>(PythonOps.IsNot));
  public static readonly MethodInfo Is = AstMethods.GetMethod((Delegate) new Func<object, object, object>(PythonOps.Is));
  public static readonly MethodInfo ImportTop = AstMethods.GetMethod((Delegate) new Func<CodeContext, string, int, object>(PythonOps.ImportTop));
  public static readonly MethodInfo ImportBottom = AstMethods.GetMethod((Delegate) new Func<CodeContext, string, int, object>(PythonOps.ImportBottom));
  public static readonly MethodInfo MakeList = AstMethods.GetMethod((Delegate) new Func<List>(PythonOps.MakeList));
  public static readonly MethodInfo MakeListNoCopy = AstMethods.GetMethod((Delegate) new Func<object[], List>(PythonOps.MakeListNoCopy));
  public static readonly MethodInfo GetEnumeratorValues = AstMethods.GetMethod((Delegate) new Func<CodeContext, object, int, object>(PythonOps.GetEnumeratorValues));
  public static readonly MethodInfo GetEnumeratorValuesNoComplexSets = AstMethods.GetMethod((Delegate) new Func<CodeContext, object, int, object>(PythonOps.GetEnumeratorValuesNoComplexSets));
  public static readonly MethodInfo GetGlobalContext = AstMethods.GetMethod((Delegate) new Func<CodeContext, CodeContext>(PythonOps.GetGlobalContext));
  public static readonly MethodInfo GetParentContextFromFunction = AstMethods.GetMethod((Delegate) new Func<PythonFunction, CodeContext>(PythonOps.GetParentContextFromFunction));
  public static readonly MethodInfo MakeFunction = AstMethods.GetMethod((Delegate) new Func<CodeContext, FunctionCode, object, object[], object>(PythonOps.MakeFunction));
  public static readonly MethodInfo MakeFunctionDebug = AstMethods.GetMethod((Delegate) new Func<CodeContext, FunctionCode, object, object[], Delegate, object>(PythonOps.MakeFunctionDebug));
  public static readonly MethodInfo MakeClosureCell = AstMethods.GetMethod((Delegate) new Func<ClosureCell>(PythonOps.MakeClosureCell));
  public static readonly MethodInfo MakeClosureCellWithValue = AstMethods.GetMethod((Delegate) new Func<object, ClosureCell>(PythonOps.MakeClosureCellWithValue));
  public static readonly MethodInfo LookupName = AstMethods.GetMethod((Delegate) new Func<CodeContext, string, object>(PythonOps.LookupName));
  public static readonly MethodInfo RemoveName = AstMethods.GetMethod((Delegate) new Action<CodeContext, string>(PythonOps.RemoveName));
  public static readonly MethodInfo SetName = AstMethods.GetMethod((Delegate) new Func<CodeContext, string, object, object>(PythonOps.SetName));
  public static readonly MethodInfo KeepAlive = AstMethods.GetMethod((Delegate) new Action<object>(GC.KeepAlive));
  public static readonly MethodInfo MakeDict = AstMethods.GetMethod((Delegate) new Func<int, PythonDictionary>(PythonOps.MakeDict));
  public static readonly MethodInfo MakeEmptyDict = AstMethods.GetMethod((Delegate) new Func<PythonDictionary>(PythonOps.MakeEmptyDict));
  public static readonly MethodInfo MakeDictFromItems = AstMethods.GetMethod((Delegate) new Func<object[], PythonDictionary>(PythonOps.MakeDictFromItems));
  public static readonly MethodInfo MakeConstantDict = AstMethods.GetMethod((Delegate) new Func<object, PythonDictionary>(PythonOps.MakeConstantDict));
  public static readonly MethodInfo MakeSet = AstMethods.GetMethod((Delegate) new Func<object[], SetCollection>(PythonOps.MakeSet));
  public static readonly MethodInfo MakeEmptySet = AstMethods.GetMethod((Delegate) new Func<SetCollection>(PythonOps.MakeEmptySet));
  public static readonly MethodInfo MakeHomogeneousDictFromItems = AstMethods.GetMethod((Delegate) new Func<object[], PythonDictionary>(PythonOps.MakeHomogeneousDictFromItems));
  public static readonly MethodInfo CreateLocalContext = AstMethods.GetMethod((Delegate) new Func<CodeContext, MutableTuple, string[], CodeContext>(PythonOps.CreateLocalContext));
  public static readonly MethodInfo UpdateStackTrace = AstMethods.GetMethod((Delegate) new Action<Exception, CodeContext, FunctionCode, int>(PythonOps.UpdateStackTrace));
  public static readonly MethodInfo ForLoopDispose = AstMethods.GetMethod((Delegate) new Action<KeyValuePair<IEnumerator, IDisposable>>(PythonOps.ForLoopDispose));
  public static readonly MethodInfo GetClosureTupleFromContext = AstMethods.GetMethod((Delegate) new Func<CodeContext, MutableTuple>(PythonOps.GetClosureTupleFromContext));
  public static readonly MethodInfo IsUnicode = AstMethods.GetMethod((Delegate) new Func<object, bool>(PythonOps.IsUnicode));
  public static readonly MethodInfo PushFrame = AstMethods.GetMethod((Delegate) new Func<CodeContext, FunctionCode, List<FunctionStack>>(PythonOps.PushFrame));
  public static readonly MethodInfo FormatUnicode = AstMethods.GetMethod((Delegate) new Func<CodeContext, string, object, string>(PythonOps.FormatUnicode));
  public static readonly MethodInfo FormatString = AstMethods.GetMethod((Delegate) new Func<CodeContext, string, object, string>(PythonOps.FormatString));
  public static readonly MethodInfo GetUnicodeFunction = AstMethods.GetMethod((Delegate) new Func<BuiltinFunction>(PythonOps.GetUnicodeFuntion));
  public static readonly MethodInfo GeneratorCheckThrowableAndReturnSendValue = AstMethods.GetMethod((Delegate) new Func<object, object>(PythonOps.GeneratorCheckThrowableAndReturnSendValue));

  private static MethodInfo GetMethod(Delegate x) => RuntimeReflectionExtensions.GetMethodInfo(x);
}
