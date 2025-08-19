// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Exceptions.PythonExceptions
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Interpreter;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Runtime.Exceptions;

public static class PythonExceptions
{
  private static object _pythonExceptionKey = (object) typeof (PythonExceptions.BaseException);
  internal const string DefaultExceptionModule = "exceptions";
  public const string __doc__ = "Provides the most commonly used exceptions for Python programs";
  private static PythonType GeneratorExitStorage;
  private static PythonType SystemExitStorage;
  private static PythonType KeyboardInterruptStorage;
  private static PythonType ExceptionStorage;
  private static PythonType StopIterationStorage;
  private static PythonType StandardErrorStorage;
  private static PythonType BufferErrorStorage;
  private static PythonType ArithmeticErrorStorage;
  private static PythonType FloatingPointErrorStorage;
  private static PythonType OverflowErrorStorage;
  private static PythonType ZeroDivisionErrorStorage;
  private static PythonType AssertionErrorStorage;
  private static PythonType AttributeErrorStorage;
  private static PythonType EnvironmentErrorStorage;
  private static PythonType IOErrorStorage;
  private static PythonType OSErrorStorage;
  private static PythonType WindowsErrorStorage;
  private static PythonType EOFErrorStorage;
  private static PythonType ImportErrorStorage;
  private static PythonType LookupErrorStorage;
  private static PythonType IndexErrorStorage;
  private static PythonType KeyErrorStorage;
  private static PythonType MemoryErrorStorage;
  private static PythonType NameErrorStorage;
  private static PythonType UnboundLocalErrorStorage;
  private static PythonType ReferenceErrorStorage;
  private static PythonType RuntimeErrorStorage;
  private static PythonType NotImplementedErrorStorage;
  private static PythonType SyntaxErrorStorage;
  private static PythonType IndentationErrorStorage;
  private static PythonType TabErrorStorage;
  private static PythonType SystemErrorStorage;
  private static PythonType TypeErrorStorage;
  private static PythonType ValueErrorStorage;
  private static PythonType UnicodeErrorStorage;
  private static PythonType UnicodeDecodeErrorStorage;
  private static PythonType UnicodeEncodeErrorStorage;
  private static PythonType UnicodeTranslateErrorStorage;
  private static PythonType WarningStorage;
  private static PythonType DeprecationWarningStorage;
  private static PythonType PendingDeprecationWarningStorage;
  private static PythonType RuntimeWarningStorage;
  private static PythonType SyntaxWarningStorage;
  private static PythonType UserWarningStorage;
  private static PythonType FutureWarningStorage;
  private static PythonType ImportWarningStorage;
  private static PythonType UnicodeWarningStorage;
  private static PythonType BytesWarningStorage;

  internal static System.Exception CreateThrowable(PythonType type, params object[] args)
  {
    return PythonExceptions.CreatePythonThrowable(type, args).GetClrException();
  }

  internal static PythonExceptions.BaseException CreatePythonThrowable(
    PythonType type,
    params object[] args)
  {
    PythonExceptions.BaseException pythonThrowable;
    if (type.UnderlyingSystemType == typeof (PythonExceptions.BaseException))
      pythonThrowable = new PythonExceptions.BaseException(type);
    else
      pythonThrowable = (PythonExceptions.BaseException) Activator.CreateInstance(type.UnderlyingSystemType, (object) type);
    pythonThrowable.__init__(args);
    return pythonThrowable;
  }

  internal static System.Exception CreateThrowableForRaise(
    CodeContext context,
    PythonType type,
    object value)
  {
    object obj = !PythonOps.IsInstance(value, type) ? (!(value is PythonTuple) ? (value == null ? PythonCalls.Call(context, (object) type) : PythonCalls.Call(context, (object) type, value)) : PythonOps.CallWithArgsTuple((object) type, ArrayUtils.EmptyObjects, value)) : value;
    return PythonOps.IsInstance(obj, type) ? ((PythonExceptions.BaseException) obj).GetClrException() : (System.Exception) new ObjectException(type, obj);
  }

  internal static System.Exception CreateThrowableForRaise(
    CodeContext context,
    OldClass type,
    object value)
  {
    return (System.Exception) new OldInstanceException(!PythonOps.IsInstance(context, value, (object) type) ? (!(value is PythonTuple) ? (OldInstance) PythonCalls.Call(context, (object) type, value) : (OldInstance) PythonOps.CallWithArgsTuple((object) type, ArrayUtils.EmptyObjects, value)) : (OldInstance) value);
  }

  internal static System.Exception ToClr(object pythonException)
  {
    System.Exception e;
    switch (pythonException)
    {
      case PythonExceptions.BaseException baseException:
        return baseException.GetClrException();
      case OldInstance instance:
        e = (System.Exception) new OldInstanceException(instance);
        break;
      default:
        e = new System.Exception(PythonOps.ToString(pythonException));
        break;
    }
    e.SetPythonException(pythonException);
    return e;
  }

  internal static object ToPython(System.Exception clrException)
  {
    if (clrException is IPythonException pythonException)
      return pythonException.ToPythonException();
    object exception = clrException.GetPythonException();
    if (exception == null)
    {
      if (clrException is SyntaxErrorException e)
        return (object) PythonExceptions.SyntaxErrorToPython(e);
      if (clrException is ThreadAbortException threadAbortException)
      {
        if (threadAbortException.ExceptionState is KeyboardInterruptException exceptionState)
        {
          threadAbortException.Data[(object) typeof (KeyboardInterruptException)] = (object) exceptionState;
          return PythonExceptions.ToPython((System.Exception) exceptionState);
        }
        if (threadAbortException.Data[(object) typeof (KeyboardInterruptException)] is KeyboardInterruptException clrException1)
          return PythonExceptions.ToPython((System.Exception) clrException1);
      }
      if (exception == null)
        exception = (object) PythonExceptions.ToPythonNewStyle(clrException);
      clrException.SetPythonException(exception);
    }
    return exception;
  }

  private static PythonExceptions.BaseException ToPythonNewStyle(System.Exception clrException)
  {
    PythonExceptions.BaseException pythonNewStyle1;
    switch (clrException)
    {
      case InvalidCastException _:
      case ArgumentNullException _:
        pythonNewStyle1 = new PythonExceptions.BaseException(PythonExceptions.TypeError);
        break;
      case Win32Exception _:
        Win32Exception win32Exception = (Win32Exception) clrException;
        int errorCode = win32Exception.ErrorCode;
        PythonExceptions.BaseException pythonNewStyle2 = (PythonExceptions.BaseException) new PythonExceptions._WindowsError();
        if (((long) errorCode & 2147942400L /*0x80070000*/) == 2147942400L /*0x80070000*/)
          pythonNewStyle2.__init__((object) (errorCode & (int) ushort.MaxValue), (object) win32Exception.Message);
        else
          pythonNewStyle2.__init__((object) errorCode, (object) win32Exception.Message);
        return pythonNewStyle2;
      default:
        pythonNewStyle1 = PythonExceptions.ToPythonHelper(clrException);
        break;
    }
    pythonNewStyle1.InitializeFromClr(clrException);
    return pythonNewStyle1;
  }

  private static void SetPythonException(this System.Exception e, object exception)
  {
    if (e is IPythonAwareException pythonAwareException)
      pythonAwareException.PythonException = exception;
    else
      e.SetData(PythonExceptions._pythonExceptionKey, (object) new PythonExceptions.ExceptionDataWrapper(exception));
    if (!(exception is PythonExceptions.BaseException baseException))
      return;
    baseException.clsException = e;
  }

  private static object GetPythonException(this System.Exception e)
  {
    if (e is IPythonAwareException pythonAwareException)
      return pythonAwareException.PythonException;
    return e.GetData(PythonExceptions._pythonExceptionKey) is PythonExceptions.ExceptionDataWrapper data ? data.Value : (object) null;
  }

  internal static List<DynamicStackFrame> GetFrameList(this System.Exception e)
  {
    return e is IPythonAwareException pythonAwareException ? pythonAwareException.Frames : e.GetData((object) typeof (DynamicStackFrame)) as List<DynamicStackFrame>;
  }

  internal static void SetFrameList(this System.Exception e, List<DynamicStackFrame> frames)
  {
    if (e is IPythonAwareException pythonAwareException)
      pythonAwareException.Frames = frames;
    else
      e.SetData((object) typeof (DynamicStackFrame), (object) frames);
  }

  internal static void RemoveFrameList(this System.Exception e)
  {
    if (e is IPythonAwareException pythonAwareException)
      pythonAwareException.Frames = (List<DynamicStackFrame>) null;
    else
      e.RemoveData((object) typeof (DynamicStackFrame));
  }

  internal static TraceBack GetTraceBack(this System.Exception e)
  {
    return e is IPythonAwareException pythonAwareException ? pythonAwareException.TraceBack : e.GetData((object) typeof (TraceBack)) as TraceBack;
  }

  internal static void SetTraceBack(this System.Exception e, TraceBack traceback)
  {
    if (e is IPythonAwareException pythonAwareException)
      pythonAwareException.TraceBack = traceback;
    else
      e.SetData((object) typeof (TraceBack), (object) traceback);
  }

  internal static void RemoveTraceBack(this System.Exception e)
  {
    if (e is IPythonAwareException pythonAwareException)
      pythonAwareException.TraceBack = (TraceBack) null;
    else
      e.RemoveData((object) typeof (TraceBack));
  }

  private static PythonExceptions.BaseException SyntaxErrorToPython(SyntaxErrorException e)
  {
    PythonExceptions._SyntaxError exception = !(e.GetType() == typeof (IndentationException)) ? (!(e.GetType() == typeof (TabException)) ? new PythonExceptions._SyntaxError() : new PythonExceptions._SyntaxError(PythonExceptions.TabError)) : new PythonExceptions._SyntaxError(PythonExceptions.IndentationError);
    string sourceLine = PythonContext.GetSourceLine(e);
    string symbolDocumentName = e.GetSymbolDocumentName();
    object column = e.Column == 0 || e.GetData(PythonContext._syntaxErrorNoCaret) != null ? (object) null : (object) e.Column;
    exception.args = (object) PythonTuple.MakeTuple((object) e.Message, (object) PythonTuple.MakeTuple((object) symbolDocumentName, (object) e.Line, column, (object) sourceLine));
    exception.filename = (object) symbolDocumentName;
    exception.lineno = (object) e.Line;
    exception.offset = column;
    exception.text = (object) sourceLine;
    exception.msg = (object) e.Message;
    e.SetPythonException((object) exception);
    return (PythonExceptions.BaseException) exception;
  }

  [PythonHidden(new PlatformID[] {})]
  public static PythonType CreateSubType(
    PythonContext context,
    PythonType baseType,
    string name,
    string module,
    string documentation,
    Func<string, System.Exception> exceptionMaker)
  {
    PythonType parent = new PythonType(context, baseType, name, module, documentation, exceptionMaker);
    parent.SetCustomMember(context.SharedContext, "__weakref__", (object) new PythonTypeWeakRefSlot(parent));
    parent.IsWeakReferencable = true;
    return parent;
  }

  [PythonHidden(new PlatformID[] {})]
  public static PythonType CreateSubType(
    PythonContext context,
    PythonType baseType,
    Type underlyingType,
    string name,
    string module,
    string documentation,
    Func<string, System.Exception> exceptionMaker)
  {
    PythonType parent = new PythonType(context, new PythonType[1]
    {
      baseType
    }, underlyingType, name, module, documentation, exceptionMaker);
    parent.SetCustomMember(context.SharedContext, "__weakref__", (object) new PythonTypeWeakRefSlot(parent));
    parent.IsWeakReferencable = true;
    return parent;
  }

  [PythonHidden(new PlatformID[] {})]
  public static PythonType CreateSubType(
    PythonContext context,
    PythonType[] baseTypes,
    Type underlyingType,
    string name,
    string module,
    string documentation,
    Func<string, System.Exception> exceptionMaker)
  {
    PythonType parent = new PythonType(context, baseTypes, underlyingType, name, module, documentation, exceptionMaker);
    parent.SetCustomMember(context.SharedContext, "__weakref__", (object) new PythonTypeWeakRefSlot(parent));
    parent.IsWeakReferencable = true;
    return parent;
  }

  private static PythonType CreateSubType(
    PythonType baseType,
    string name,
    Func<string, System.Exception> exceptionMaker)
  {
    return new PythonType(baseType, name, exceptionMaker);
  }

  private static PythonType CreateSubType(
    PythonType baseType,
    Type concreteType,
    Func<string, System.Exception> exceptionMaker)
  {
    PythonType pythonTypeFromType = DynamicHelpers.GetPythonTypeFromType(concreteType);
    pythonTypeFromType.ResolutionOrder = (IList<PythonType>) Mro.Calculate(pythonTypeFromType, (IList<PythonType>) new PythonType[1]
    {
      baseType
    });
    pythonTypeFromType.BaseTypes = (IList<PythonType>) new PythonType[1]
    {
      baseType
    };
    pythonTypeFromType.HasDictionary = true;
    pythonTypeFromType._makeException = exceptionMaker;
    return pythonTypeFromType;
  }

  internal static DynamicStackFrame[] GetDynamicStackFrames(System.Exception e)
  {
    List<DynamicStackFrame> frameList = e.GetFrameList();
    if (frameList == null)
      return new DynamicStackFrame[0];
    List<DynamicStackFrame> dynamicStackFrameList = new List<DynamicStackFrame>((IEnumerable<DynamicStackFrame>) frameList);
    List<DynamicStackFrame> collection = new List<DynamicStackFrame>();
    try
    {
      StackTrace stackTrace1 = new StackTrace(e, false);
      IList<StackTrace> stackTraceList = ExceptionHelpers.GetExceptionStackTraces(e) ?? (IList<StackTrace>) new List<StackTrace>();
      List<StackFrame> stackTrace2 = new List<StackFrame>();
      foreach (StackTrace stackTrace3 in (IEnumerable<StackTrace>) stackTraceList)
        stackTrace2.AddRange((IEnumerable<StackFrame>) (stackTrace3.GetFrames() ?? new StackFrame[0]));
      stackTrace2.AddRange((IEnumerable<StackFrame>) (stackTrace1.GetFrames() ?? new StackFrame[0]));
      int num = 0;
      foreach (StackFrame groupStackFrame in InterpretedFrame.GroupStackFrames((IEnumerable<StackFrame>) stackTrace2))
      {
        MethodBase method1 = groupStackFrame.GetMethod();
        for (int index = num; index < dynamicStackFrameList.Count; ++index)
        {
          MethodBase method2 = dynamicStackFrameList[index].GetMethod();
          if (PythonExceptions.MethodsMatch(method1, method2))
          {
            collection.Add(dynamicStackFrameList[index]);
            dynamicStackFrameList.RemoveAt(index);
            num = index;
            break;
          }
        }
      }
    }
    catch (MemberAccessException ex)
    {
    }
    dynamicStackFrameList.AddRange((IEnumerable<DynamicStackFrame>) collection);
    return dynamicStackFrameList.ToArray();
  }

  private static bool MethodsMatch(MethodBase method, MethodBase other)
  {
    return method.Module == other.Module && method.DeclaringType == other.DeclaringType && method.Name == other.Name;
  }

  public static PythonType GeneratorExit
  {
    get
    {
      if (PythonExceptions.GeneratorExitStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.GeneratorExitStorage, PythonExceptions.CreateSubType(DynamicHelpers.GetPythonTypeFromType(typeof (PythonExceptions.BaseException)), nameof (GeneratorExit), (Func<string, System.Exception>) (msg => (System.Exception) new GeneratorExitException(msg))), (PythonType) null);
      return PythonExceptions.GeneratorExitStorage;
    }
  }

  public static PythonType SystemExit
  {
    get
    {
      if (PythonExceptions.SystemExitStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.SystemExitStorage, PythonExceptions.CreateSubType(DynamicHelpers.GetPythonTypeFromType(typeof (PythonExceptions.BaseException)), typeof (PythonExceptions._SystemExit), (Func<string, System.Exception>) (msg => (System.Exception) new SystemExitException(msg))), (PythonType) null);
      return PythonExceptions.SystemExitStorage;
    }
  }

  public static PythonType KeyboardInterrupt
  {
    get
    {
      if (PythonExceptions.KeyboardInterruptStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.KeyboardInterruptStorage, PythonExceptions.CreateSubType(DynamicHelpers.GetPythonTypeFromType(typeof (PythonExceptions.BaseException)), nameof (KeyboardInterrupt), (Func<string, System.Exception>) (msg => (System.Exception) new KeyboardInterruptException(msg))), (PythonType) null);
      return PythonExceptions.KeyboardInterruptStorage;
    }
  }

  public static PythonType Exception
  {
    get
    {
      if (PythonExceptions.ExceptionStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.ExceptionStorage, PythonExceptions.CreateSubType(DynamicHelpers.GetPythonTypeFromType(typeof (PythonExceptions.BaseException)), nameof (Exception), (Func<string, System.Exception>) (msg => (System.Exception) new PythonException(msg))), (PythonType) null);
      return PythonExceptions.ExceptionStorage;
    }
  }

  public static PythonType StopIteration
  {
    get
    {
      if (PythonExceptions.StopIterationStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.StopIterationStorage, PythonExceptions.CreateSubType(PythonExceptions.Exception, nameof (StopIteration), (Func<string, System.Exception>) (msg => (System.Exception) new StopIterationException(msg))), (PythonType) null);
      return PythonExceptions.StopIterationStorage;
    }
  }

  public static PythonType StandardError
  {
    get
    {
      if (PythonExceptions.StandardErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.StandardErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.Exception, nameof (StandardError), (Func<string, System.Exception>) (msg => (System.Exception) new ApplicationException(msg))), (PythonType) null);
      return PythonExceptions.StandardErrorStorage;
    }
  }

  public static PythonType BufferError
  {
    get
    {
      if (PythonExceptions.BufferErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.BufferErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.StandardError, nameof (BufferError), (Func<string, System.Exception>) (msg => (System.Exception) new BufferException(msg))), (PythonType) null);
      return PythonExceptions.BufferErrorStorage;
    }
  }

  public static PythonType ArithmeticError
  {
    get
    {
      if (PythonExceptions.ArithmeticErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.ArithmeticErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.StandardError, nameof (ArithmeticError), (Func<string, System.Exception>) (msg => (System.Exception) new ArithmeticException(msg))), (PythonType) null);
      return PythonExceptions.ArithmeticErrorStorage;
    }
  }

  public static PythonType FloatingPointError
  {
    get
    {
      if (PythonExceptions.FloatingPointErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.FloatingPointErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.ArithmeticError, nameof (FloatingPointError), (Func<string, System.Exception>) (msg => (System.Exception) new FloatingPointException(msg))), (PythonType) null);
      return PythonExceptions.FloatingPointErrorStorage;
    }
  }

  public static PythonType OverflowError
  {
    get
    {
      if (PythonExceptions.OverflowErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.OverflowErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.ArithmeticError, nameof (OverflowError), (Func<string, System.Exception>) (msg => (System.Exception) new OverflowException(msg))), (PythonType) null);
      return PythonExceptions.OverflowErrorStorage;
    }
  }

  public static PythonType ZeroDivisionError
  {
    get
    {
      if (PythonExceptions.ZeroDivisionErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.ZeroDivisionErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.ArithmeticError, nameof (ZeroDivisionError), (Func<string, System.Exception>) (msg => (System.Exception) new DivideByZeroException(msg))), (PythonType) null);
      return PythonExceptions.ZeroDivisionErrorStorage;
    }
  }

  public static PythonType AssertionError
  {
    get
    {
      if (PythonExceptions.AssertionErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.AssertionErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.StandardError, nameof (AssertionError), (Func<string, System.Exception>) (msg => (System.Exception) new AssertionException(msg))), (PythonType) null);
      return PythonExceptions.AssertionErrorStorage;
    }
  }

  public static PythonType AttributeError
  {
    get
    {
      if (PythonExceptions.AttributeErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.AttributeErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.StandardError, nameof (AttributeError), (Func<string, System.Exception>) (msg => (System.Exception) new AttributeErrorException(msg))), (PythonType) null);
      return PythonExceptions.AttributeErrorStorage;
    }
  }

  public static PythonType EnvironmentError
  {
    get
    {
      if (PythonExceptions.EnvironmentErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.EnvironmentErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.StandardError, typeof (PythonExceptions._EnvironmentError), (Func<string, System.Exception>) (msg => (System.Exception) new ExternalException(msg))), (PythonType) null);
      return PythonExceptions.EnvironmentErrorStorage;
    }
  }

  public static PythonType IOError
  {
    get
    {
      if (PythonExceptions.IOErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.IOErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.EnvironmentError, nameof (IOError), (Func<string, System.Exception>) (msg => (System.Exception) new IOException(msg))), (PythonType) null);
      return PythonExceptions.IOErrorStorage;
    }
  }

  public static PythonType OSError
  {
    get
    {
      if (PythonExceptions.OSErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.OSErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.EnvironmentError, nameof (OSError), (Func<string, System.Exception>) (msg => (System.Exception) new OSException(msg))), (PythonType) null);
      return PythonExceptions.OSErrorStorage;
    }
  }

  public static PythonType WindowsError
  {
    get
    {
      if (PythonExceptions.WindowsErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.WindowsErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.OSError, typeof (PythonExceptions._WindowsError), (Func<string, System.Exception>) (msg => (System.Exception) new Win32Exception(msg))), (PythonType) null);
      return PythonExceptions.WindowsErrorStorage;
    }
  }

  public static PythonType EOFError
  {
    get
    {
      if (PythonExceptions.EOFErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.EOFErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.StandardError, nameof (EOFError), (Func<string, System.Exception>) (msg => (System.Exception) new EndOfStreamException(msg))), (PythonType) null);
      return PythonExceptions.EOFErrorStorage;
    }
  }

  public static PythonType ImportError
  {
    get
    {
      if (PythonExceptions.ImportErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.ImportErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.StandardError, nameof (ImportError), (Func<string, System.Exception>) (msg => (System.Exception) new ImportException(msg))), (PythonType) null);
      return PythonExceptions.ImportErrorStorage;
    }
  }

  public static PythonType LookupError
  {
    get
    {
      if (PythonExceptions.LookupErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.LookupErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.StandardError, nameof (LookupError), (Func<string, System.Exception>) (msg => (System.Exception) new LookupException(msg))), (PythonType) null);
      return PythonExceptions.LookupErrorStorage;
    }
  }

  public static PythonType IndexError
  {
    get
    {
      if (PythonExceptions.IndexErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.IndexErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.LookupError, nameof (IndexError), (Func<string, System.Exception>) (msg => (System.Exception) new IndexOutOfRangeException(msg))), (PythonType) null);
      return PythonExceptions.IndexErrorStorage;
    }
  }

  public static PythonType KeyError
  {
    get
    {
      if (PythonExceptions.KeyErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.KeyErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.LookupError, nameof (KeyError), (Func<string, System.Exception>) (msg => (System.Exception) new KeyNotFoundException(msg))), (PythonType) null);
      return PythonExceptions.KeyErrorStorage;
    }
  }

  public static PythonType MemoryError
  {
    get
    {
      if (PythonExceptions.MemoryErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.MemoryErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.StandardError, nameof (MemoryError), (Func<string, System.Exception>) (msg => (System.Exception) new OutOfMemoryException(msg))), (PythonType) null);
      return PythonExceptions.MemoryErrorStorage;
    }
  }

  public static PythonType NameError
  {
    get
    {
      if (PythonExceptions.NameErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.NameErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.StandardError, nameof (NameError), (Func<string, System.Exception>) (msg => (System.Exception) new UnboundNameException(msg))), (PythonType) null);
      return PythonExceptions.NameErrorStorage;
    }
  }

  public static PythonType UnboundLocalError
  {
    get
    {
      if (PythonExceptions.UnboundLocalErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.UnboundLocalErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.NameError, nameof (UnboundLocalError), (Func<string, System.Exception>) (msg => (System.Exception) new UnboundLocalException(msg))), (PythonType) null);
      return PythonExceptions.UnboundLocalErrorStorage;
    }
  }

  public static PythonType ReferenceError
  {
    get
    {
      if (PythonExceptions.ReferenceErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.ReferenceErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.StandardError, nameof (ReferenceError), (Func<string, System.Exception>) (msg => (System.Exception) new ReferenceException(msg))), (PythonType) null);
      return PythonExceptions.ReferenceErrorStorage;
    }
  }

  public static PythonType RuntimeError
  {
    get
    {
      if (PythonExceptions.RuntimeErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.RuntimeErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.StandardError, nameof (RuntimeError), (Func<string, System.Exception>) (msg => (System.Exception) new RuntimeException(msg))), (PythonType) null);
      return PythonExceptions.RuntimeErrorStorage;
    }
  }

  public static PythonType NotImplementedError
  {
    get
    {
      if (PythonExceptions.NotImplementedErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.NotImplementedErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.RuntimeError, nameof (NotImplementedError), (Func<string, System.Exception>) (msg => (System.Exception) new NotImplementedException(msg))), (PythonType) null);
      return PythonExceptions.NotImplementedErrorStorage;
    }
  }

  public static PythonType SyntaxError
  {
    get
    {
      if (PythonExceptions.SyntaxErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.SyntaxErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.StandardError, typeof (PythonExceptions._SyntaxError), (Func<string, System.Exception>) (msg => (System.Exception) new SyntaxErrorException(msg))), (PythonType) null);
      return PythonExceptions.SyntaxErrorStorage;
    }
  }

  public static PythonType IndentationError
  {
    get
    {
      if (PythonExceptions.IndentationErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.IndentationErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.SyntaxError, nameof (IndentationError), (Func<string, System.Exception>) (msg => (System.Exception) new IndentationException(msg))), (PythonType) null);
      return PythonExceptions.IndentationErrorStorage;
    }
  }

  public static PythonType TabError
  {
    get
    {
      if (PythonExceptions.TabErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.TabErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.IndentationError, nameof (TabError), (Func<string, System.Exception>) (msg => (System.Exception) new TabException(msg))), (PythonType) null);
      return PythonExceptions.TabErrorStorage;
    }
  }

  public static PythonType SystemError
  {
    get
    {
      if (PythonExceptions.SystemErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.SystemErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.StandardError, nameof (SystemError), (Func<string, System.Exception>) (msg => (System.Exception) new SystemException(msg))), (PythonType) null);
      return PythonExceptions.SystemErrorStorage;
    }
  }

  public static PythonType TypeError
  {
    get
    {
      if (PythonExceptions.TypeErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.TypeErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.StandardError, nameof (TypeError), (Func<string, System.Exception>) (msg => (System.Exception) new TypeErrorException(msg))), (PythonType) null);
      return PythonExceptions.TypeErrorStorage;
    }
  }

  public static PythonType ValueError
  {
    get
    {
      if (PythonExceptions.ValueErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.ValueErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.StandardError, nameof (ValueError), (Func<string, System.Exception>) (msg => (System.Exception) new ValueErrorException(msg))), (PythonType) null);
      return PythonExceptions.ValueErrorStorage;
    }
  }

  public static PythonType UnicodeError
  {
    get
    {
      if (PythonExceptions.UnicodeErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.UnicodeErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.ValueError, nameof (UnicodeError), (Func<string, System.Exception>) (msg => (System.Exception) new UnicodeException(msg))), (PythonType) null);
      return PythonExceptions.UnicodeErrorStorage;
    }
  }

  public static PythonType UnicodeDecodeError
  {
    get
    {
      if (PythonExceptions.UnicodeDecodeErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.UnicodeDecodeErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.UnicodeError, typeof (PythonExceptions._UnicodeDecodeError), (Func<string, System.Exception>) (msg => (System.Exception) new DecoderFallbackException(msg))), (PythonType) null);
      return PythonExceptions.UnicodeDecodeErrorStorage;
    }
  }

  public static PythonType UnicodeEncodeError
  {
    get
    {
      if (PythonExceptions.UnicodeEncodeErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.UnicodeEncodeErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.UnicodeError, typeof (PythonExceptions._UnicodeEncodeError), (Func<string, System.Exception>) (msg => (System.Exception) new EncoderFallbackException(msg))), (PythonType) null);
      return PythonExceptions.UnicodeEncodeErrorStorage;
    }
  }

  public static PythonType UnicodeTranslateError
  {
    get
    {
      if (PythonExceptions.UnicodeTranslateErrorStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.UnicodeTranslateErrorStorage, PythonExceptions.CreateSubType(PythonExceptions.UnicodeError, typeof (PythonExceptions._UnicodeTranslateError), (Func<string, System.Exception>) (msg => (System.Exception) new UnicodeTranslateException(msg))), (PythonType) null);
      return PythonExceptions.UnicodeTranslateErrorStorage;
    }
  }

  public static PythonType Warning
  {
    get
    {
      if (PythonExceptions.WarningStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.WarningStorage, PythonExceptions.CreateSubType(PythonExceptions.Exception, nameof (Warning), (Func<string, System.Exception>) (msg => (System.Exception) new WarningException(msg))), (PythonType) null);
      return PythonExceptions.WarningStorage;
    }
  }

  public static PythonType DeprecationWarning
  {
    get
    {
      if (PythonExceptions.DeprecationWarningStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.DeprecationWarningStorage, PythonExceptions.CreateSubType(PythonExceptions.Warning, nameof (DeprecationWarning), (Func<string, System.Exception>) (msg => (System.Exception) new DeprecationWarningException(msg))), (PythonType) null);
      return PythonExceptions.DeprecationWarningStorage;
    }
  }

  public static PythonType PendingDeprecationWarning
  {
    get
    {
      if (PythonExceptions.PendingDeprecationWarningStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.PendingDeprecationWarningStorage, PythonExceptions.CreateSubType(PythonExceptions.Warning, nameof (PendingDeprecationWarning), (Func<string, System.Exception>) (msg => (System.Exception) new PendingDeprecationWarningException(msg))), (PythonType) null);
      return PythonExceptions.PendingDeprecationWarningStorage;
    }
  }

  public static PythonType RuntimeWarning
  {
    get
    {
      if (PythonExceptions.RuntimeWarningStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.RuntimeWarningStorage, PythonExceptions.CreateSubType(PythonExceptions.Warning, nameof (RuntimeWarning), (Func<string, System.Exception>) (msg => (System.Exception) new RuntimeWarningException(msg))), (PythonType) null);
      return PythonExceptions.RuntimeWarningStorage;
    }
  }

  public static PythonType SyntaxWarning
  {
    get
    {
      if (PythonExceptions.SyntaxWarningStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.SyntaxWarningStorage, PythonExceptions.CreateSubType(PythonExceptions.Warning, nameof (SyntaxWarning), (Func<string, System.Exception>) (msg => (System.Exception) new SyntaxWarningException(msg))), (PythonType) null);
      return PythonExceptions.SyntaxWarningStorage;
    }
  }

  public static PythonType UserWarning
  {
    get
    {
      if (PythonExceptions.UserWarningStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.UserWarningStorage, PythonExceptions.CreateSubType(PythonExceptions.Warning, nameof (UserWarning), (Func<string, System.Exception>) (msg => (System.Exception) new UserWarningException(msg))), (PythonType) null);
      return PythonExceptions.UserWarningStorage;
    }
  }

  public static PythonType FutureWarning
  {
    get
    {
      if (PythonExceptions.FutureWarningStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.FutureWarningStorage, PythonExceptions.CreateSubType(PythonExceptions.Warning, nameof (FutureWarning), (Func<string, System.Exception>) (msg => (System.Exception) new FutureWarningException(msg))), (PythonType) null);
      return PythonExceptions.FutureWarningStorage;
    }
  }

  public static PythonType ImportWarning
  {
    get
    {
      if (PythonExceptions.ImportWarningStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.ImportWarningStorage, PythonExceptions.CreateSubType(PythonExceptions.Warning, nameof (ImportWarning), (Func<string, System.Exception>) (msg => (System.Exception) new ImportWarningException(msg))), (PythonType) null);
      return PythonExceptions.ImportWarningStorage;
    }
  }

  public static PythonType UnicodeWarning
  {
    get
    {
      if (PythonExceptions.UnicodeWarningStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.UnicodeWarningStorage, PythonExceptions.CreateSubType(PythonExceptions.Warning, nameof (UnicodeWarning), (Func<string, System.Exception>) (msg => (System.Exception) new UnicodeWarningException(msg))), (PythonType) null);
      return PythonExceptions.UnicodeWarningStorage;
    }
  }

  public static PythonType BytesWarning
  {
    get
    {
      if (PythonExceptions.BytesWarningStorage == null)
        Interlocked.CompareExchange<PythonType>(ref PythonExceptions.BytesWarningStorage, PythonExceptions.CreateSubType(PythonExceptions.Warning, nameof (BytesWarning), (Func<string, System.Exception>) (msg => (System.Exception) new BytesWarningException(msg))), (PythonType) null);
      return PythonExceptions.BytesWarningStorage;
    }
  }

  private static PythonExceptions.BaseException ToPythonHelper(System.Exception clrException)
  {
    switch (clrException)
    {
      case BytesWarningException _:
        return new PythonExceptions.BaseException(PythonExceptions.BytesWarning);
      case DecoderFallbackException _:
        return (PythonExceptions.BaseException) new PythonExceptions._UnicodeDecodeError();
      case DeprecationWarningException _:
        return new PythonExceptions.BaseException(PythonExceptions.DeprecationWarning);
      case DivideByZeroException _:
        return new PythonExceptions.BaseException(PythonExceptions.ZeroDivisionError);
      case EncoderFallbackException _:
        return (PythonExceptions.BaseException) new PythonExceptions._UnicodeEncodeError();
      case EndOfStreamException _:
        return new PythonExceptions.BaseException(PythonExceptions.EOFError);
      case FutureWarningException _:
        return new PythonExceptions.BaseException(PythonExceptions.FutureWarning);
      case ImportWarningException _:
        return new PythonExceptions.BaseException(PythonExceptions.ImportWarning);
      case MissingMemberException _:
        return new PythonExceptions.BaseException(PythonExceptions.AttributeError);
      case OverflowException _:
        return new PythonExceptions.BaseException(PythonExceptions.OverflowError);
      case PendingDeprecationWarningException _:
        return new PythonExceptions.BaseException(PythonExceptions.PendingDeprecationWarning);
      case RuntimeWarningException _:
        return new PythonExceptions.BaseException(PythonExceptions.RuntimeWarning);
      case SyntaxWarningException _:
        return new PythonExceptions.BaseException(PythonExceptions.SyntaxWarning);
      case TabException _:
        return (PythonExceptions.BaseException) new PythonExceptions._SyntaxError(PythonExceptions.TabError);
      case UnicodeWarningException _:
        return new PythonExceptions.BaseException(PythonExceptions.UnicodeWarning);
      case UserWarningException _:
        return new PythonExceptions.BaseException(PythonExceptions.UserWarning);
      case Win32Exception _:
        return (PythonExceptions.BaseException) new PythonExceptions._WindowsError();
      case ArgumentException _:
        return new PythonExceptions.BaseException(PythonExceptions.ValueError);
      case ArithmeticException _:
        return new PythonExceptions.BaseException(PythonExceptions.ArithmeticError);
      case ExternalException _:
        return (PythonExceptions.BaseException) new PythonExceptions._EnvironmentError();
      case IOException _:
        return (PythonExceptions.BaseException) new PythonExceptions._EnvironmentError(PythonExceptions.IOError);
      case IndentationException _:
        return (PythonExceptions.BaseException) new PythonExceptions._SyntaxError(PythonExceptions.IndentationError);
      case IndexOutOfRangeException _:
        return new PythonExceptions.BaseException(PythonExceptions.IndexError);
      case KeyNotFoundException _:
        return new PythonExceptions.BaseException(PythonExceptions.KeyError);
      case NotImplementedException _:
        return new PythonExceptions.BaseException(PythonExceptions.NotImplementedError);
      case OSException _:
        return (PythonExceptions.BaseException) new PythonExceptions._EnvironmentError(PythonExceptions.OSError);
      case OutOfMemoryException _:
        return new PythonExceptions.BaseException(PythonExceptions.MemoryError);
      case UnboundLocalException _:
        return new PythonExceptions.BaseException(PythonExceptions.UnboundLocalError);
      case UnicodeTranslateException _:
        return (PythonExceptions.BaseException) new PythonExceptions._UnicodeTranslateError();
      case WarningException _:
        return new PythonExceptions.BaseException(PythonExceptions.Warning);
      case ApplicationException _:
        return new PythonExceptions.BaseException(PythonExceptions.StandardError);
      case ArgumentTypeException _:
        return new PythonExceptions.BaseException(PythonExceptions.TypeError);
      case AssertionException _:
        return new PythonExceptions.BaseException(PythonExceptions.AssertionError);
      case BufferException _:
        return new PythonExceptions.BaseException(PythonExceptions.BufferError);
      case FloatingPointException _:
        return new PythonExceptions.BaseException(PythonExceptions.FloatingPointError);
      case GeneratorExitException _:
        return new PythonExceptions.BaseException(PythonExceptions.GeneratorExit);
      case ImportException _:
        return new PythonExceptions.BaseException(PythonExceptions.ImportError);
      case KeyboardInterruptException _:
        return new PythonExceptions.BaseException(PythonExceptions.KeyboardInterrupt);
      case LookupException _:
        return new PythonExceptions.BaseException(PythonExceptions.LookupError);
      case PythonException _:
        return new PythonExceptions.BaseException(PythonExceptions.Exception);
      case ReferenceException _:
        return new PythonExceptions.BaseException(PythonExceptions.ReferenceError);
      case RuntimeException _:
        return new PythonExceptions.BaseException(PythonExceptions.RuntimeError);
      case StopIterationException _:
        return new PythonExceptions.BaseException(PythonExceptions.StopIteration);
      case SyntaxErrorException _:
        return (PythonExceptions.BaseException) new PythonExceptions._SyntaxError();
      case SystemException _:
        return new PythonExceptions.BaseException(PythonExceptions.SystemError);
      case SystemExitException _:
        return (PythonExceptions.BaseException) new PythonExceptions._SystemExit();
      case UnboundNameException _:
        return new PythonExceptions.BaseException(PythonExceptions.NameError);
      case UnicodeException _:
        return new PythonExceptions.BaseException(PythonExceptions.UnicodeError);
      default:
        return new PythonExceptions.BaseException(PythonExceptions.Exception);
    }
  }

  [PythonType("BaseException")]
  [DynamicBaseType]
  [Serializable]
  public class BaseException : 
    ICodeFormattable,
    IPythonObject,
    IDynamicMetaObjectProvider,
    IWeakReferenceable
  {
    private PythonType _type;
    private object _message = (object) string.Empty;
    private PythonTuple _args;
    private PythonDictionary _dict;
    private System.Exception _clrException;
    private object[] _slots;
    public static string __doc__ = "Common base class for all non-exit exceptions.";

    public BaseException(PythonType type)
    {
      ContractUtils.RequiresNotNull((object) type, nameof (type));
      this._type = type;
    }

    public static object __new__(PythonType cls, params object[] argsø)
    {
      if (cls.UnderlyingSystemType == typeof (PythonExceptions.BaseException))
        return (object) new PythonExceptions.BaseException(cls);
      return Activator.CreateInstance(cls.UnderlyingSystemType, (object) cls);
    }

    public static object __new__(
      PythonType cls,
      [ParamDictionary] IDictionary<object, object> kwArgsø,
      params object[] argsø)
    {
      if (cls.UnderlyingSystemType == typeof (PythonExceptions.BaseException))
        return (object) new PythonExceptions.BaseException(cls);
      return Activator.CreateInstance(cls.UnderlyingSystemType, (object) cls);
    }

    public virtual void __init__(params object[] argsø)
    {
      this._args = PythonTuple.MakeTuple(argsø ?? new object[1]);
      if (this._args.__len__() != 1)
        return;
      this._message = this._args[0];
    }

    public object message
    {
      [Python3Warning("BaseException.message has been deprecated as of Python 2.6")] get
      {
        return this._message;
      }
      [Python3Warning("BaseException.message has been deprecated as of Python 2.6")] set
      {
        this._message = value;
      }
    }

    public object args
    {
      get => (object) this._args ?? (object) PythonTuple.EMPTY;
      set => this._args = PythonTuple.Make(value);
    }

    public virtual object __reduce__()
    {
      return (object) PythonTuple.MakeTuple((object) DynamicHelpers.GetPythonType((object) this), this.args);
    }

    public virtual object __reduce_ex__(int protocol) => this.__reduce__();

    public object this[int index]
    {
      [Python3Warning("__getitem__ not supported for exception classes in 3.x; use args attribute")] get
      {
        return ((PythonTuple) this.args)[index];
      }
    }

    [Python3Warning("__getslice__ not supported for exception classes in 3.x; use args attribute")]
    public PythonTuple __getslice__(int start, int stop)
    {
      PythonTuple args = (PythonTuple) this.args;
      Slice.FixSliceArguments(args._data.Length, ref start, ref stop);
      return PythonTuple.MakeTuple(ArrayOps.GetSlice(args._data, start, stop));
    }

    public PythonDictionary __dict__
    {
      get
      {
        this.EnsureDict();
        return this._dict;
      }
      set
      {
        this._dict = this._dict != null ? value : throw PythonOps.TypeError("__dict__ must be a dictionary");
      }
    }

    public void __setstate__(PythonDictionary state)
    {
      foreach (KeyValuePair<object, object> keyValuePair in state)
        this.__dict__[keyValuePair.Key] = keyValuePair.Value;
    }

    public System.Exception clsException
    {
      [PythonHidden(new PlatformID[] {})] get => this.GetClrException();
      internal set => this._clrException = value;
    }

    public override string ToString()
    {
      if (this._args == null || this._args.__len__() == 0)
        return string.Empty;
      if (this._args.__len__() != 1)
        return this._args.ToString();
      if (this._args[0] is string str)
        return str;
      return this._args[0] is Extensible<string> extensible ? extensible.Value : PythonOps.ToString(this._args[0]);
    }

    public string __unicode__() => this.ToString();

    [SpecialName]
    public object GetBoundMember(string name)
    {
      object obj;
      return this._dict != null && this._dict.TryGetValue((object) name, out obj) ? obj : (object) OperationFailed.Value;
    }

    [SpecialName]
    public void SetMemberAfter(string name, object value)
    {
      this.EnsureDict();
      this._dict[(object) name] = value;
    }

    [SpecialName]
    public bool DeleteCustomMember(string name)
    {
      if (name == "message")
      {
        this._message = (object) null;
        return true;
      }
      return this._dict != null && this._dict.Remove((object) name);
    }

    private void EnsureDict()
    {
      if (this._dict != null)
        return;
      Interlocked.CompareExchange<PythonDictionary>(ref this._dict, PythonDictionary.MakeSymbolDictionary(), (PythonDictionary) null);
    }

    public virtual string __repr__(CodeContext context)
    {
      return this._type.Name + ((ICodeFormattable) this.args).__repr__(context);
    }

    PythonDictionary IPythonObject.Dict => this._dict;

    PythonDictionary IPythonObject.SetDict(PythonDictionary dict)
    {
      Interlocked.CompareExchange<PythonDictionary>(ref this._dict, dict, (PythonDictionary) null);
      return this._dict;
    }

    bool IPythonObject.ReplaceDict(PythonDictionary dict)
    {
      return Interlocked.CompareExchange<PythonDictionary>(ref this._dict, dict, (PythonDictionary) null) == null;
    }

    PythonType IPythonObject.PythonType => this._type;

    void IPythonObject.SetPythonType(PythonType newType)
    {
      if (this._type.IsSystemType || newType.IsSystemType)
        throw PythonOps.TypeError("__class__ assignment can only be performed on user defined types");
      this._type = newType;
    }

    object[] IPythonObject.GetSlots() => this._slots;

    object[] IPythonObject.GetSlotsCreate()
    {
      if (this._slots == null)
        Interlocked.CompareExchange<object[]>(ref this._slots, new object[1], (object[]) null);
      return this._slots;
    }

    [PythonHidden(new PlatformID[] {})]
    protected internal virtual void InitializeFromClr(System.Exception exception)
    {
      if (exception.Message != null)
        this.__init__((object) exception.Message);
      else
        this.__init__();
    }

    internal System.Exception GetClrException()
    {
      if (this._clrException != null)
        return this._clrException;
      string str = this._message as string;
      if (string.IsNullOrEmpty(str))
        str = this._type.Name;
      System.Exception e = this._type._makeException(str);
      e.SetPythonException((object) this);
      Interlocked.CompareExchange<System.Exception>(ref this._clrException, e, (System.Exception) null);
      return this._clrException;
    }

    internal System.Exception InitAndGetClrException(params object[] args)
    {
      this.__init__(args);
      return this.GetClrException();
    }

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
    {
      return (DynamicMetaObject) new MetaUserObject(parameter, BindingRestrictions.Empty, (DynamicMetaObject) null, (IPythonObject) this);
    }

    WeakRefTracker IWeakReferenceable.GetWeakRef()
    {
      return UserTypeOps.GetWeakRefHelper((IPythonObject) this);
    }

    bool IWeakReferenceable.SetWeakRef(WeakRefTracker value)
    {
      return UserTypeOps.SetWeakRefHelper((IPythonObject) this, value);
    }

    void IWeakReferenceable.SetFinalizer(WeakRefTracker value)
    {
      UserTypeOps.SetFinalizerHelper((IPythonObject) this, value);
    }
  }

  [PythonType("SyntaxError")]
  [PythonHidden(new PlatformID[] {})]
  [DynamicBaseType]
  [Serializable]
  public class _SyntaxError : PythonExceptions.BaseException
  {
    private object _text;
    private object _print_file_and_line;
    private object _filename;
    private object _lineno;
    private object _offset;
    private object _msg;

    public override string ToString()
    {
      PythonTuple args = (PythonTuple) this.args;
      if (args == null)
        return string.Empty;
      switch (args.__len__())
      {
        case 0:
          return PythonOps.ToString((object) null);
        case 1:
          return PythonOps.ToString(args[0]);
        case 2:
          if (args[0] is string str)
            return str;
          break;
      }
      return PythonOps.ToString((object) args);
    }

    public override void __init__(params object[] args)
    {
      base.__init__(args);
      if (args == null || args.Length == 0)
        return;
      this.msg = args[0];
      if (args.Length < 2 || !(args[1] is PythonTuple pythonTuple))
        return;
      this.filename = pythonTuple.__len__() == 4 ? pythonTuple[0] : throw PythonOps.IndexError("SyntaxError expected tuple with 4 arguments, got {0}", (object) pythonTuple.__len__());
      this.lineno = pythonTuple[1];
      this.offset = pythonTuple[2];
      this.text = pythonTuple[3];
    }

    public _SyntaxError()
      : base(PythonExceptions.SyntaxError)
    {
    }

    public _SyntaxError(PythonType type)
      : base(type)
    {
    }

    public new static object __new__(
      PythonType cls,
      [ParamDictionary] IDictionary<object, object> kwArgs,
      params object[] args)
    {
      return Activator.CreateInstance(cls.UnderlyingSystemType, (object) cls);
    }

    public object text
    {
      get => this._text;
      set => this._text = value;
    }

    public object print_file_and_line
    {
      get => this._print_file_and_line;
      set => this._print_file_and_line = value;
    }

    public object filename
    {
      get => this._filename;
      set => this._filename = value;
    }

    public object lineno
    {
      get => this._lineno;
      set => this._lineno = value;
    }

    public object offset
    {
      get => this._offset;
      set => this._offset = value;
    }

    public object msg
    {
      get => this._msg;
      set => this._msg = value;
    }
  }

  [PythonType("EnvironmentError")]
  [PythonHidden(new PlatformID[] {})]
  [DynamicBaseType]
  [Serializable]
  public class _EnvironmentError : PythonExceptions.BaseException
  {
    private const int EACCES = 13;
    private const int ENOENT = 2;
    private const int EPIPE = 32 /*0x20*/;
    private object _errno;
    private object _strerror;
    private object _filename;

    public override object __reduce__()
    {
      if (this._filename == null)
        return base.__reduce__();
      return (object) PythonTuple.MakeTuple((object) DynamicHelpers.GetPythonType((object) this), (object) PythonTuple.MakeTuple(ArrayUtils.Append<object>(((PythonTuple) this.args)._data, this._filename)));
    }

    public override void __init__(params object[] args)
    {
      if (args != null)
      {
        switch (args.Length)
        {
          case 2:
            this._errno = args[0];
            this._strerror = args[1];
            break;
          case 3:
            this._errno = args[0];
            this._strerror = args[1];
            this._filename = args[2];
            args = ArrayUtils.RemoveLast<object>(args);
            break;
        }
      }
      base.__init__(args);
    }

    public override string ToString()
    {
      return this._errno != null && this._strerror != null ? (this._filename != null ? $"[Errno {this._errno}] {this._strerror}: {this._filename}" : $"[Errno {this._errno}] {this._strerror}") : (this.args is PythonTuple && ((PythonTuple) this.args).Count > 0 ? PythonOps.ToString(((PythonTuple) this.args)[0]) : string.Empty);
    }

    [PythonHidden(new PlatformID[] {})]
    protected internal override void InitializeFromClr(System.Exception exception)
    {
      switch (exception)
      {
        case FileNotFoundException _:
        case DirectoryNotFoundException _:
        case PathTooLongException _:
        case DriveNotFoundException _:
          this.__init__((object) 2, (object) exception.Message);
          return;
        case UnauthorizedAccessException _:
          this.__init__((object) 13, (object) exception.Message);
          return;
        case IOException _:
          try
          {
            uint hrForException = (uint) PythonExceptions._EnvironmentError.GetHRForException(exception);
            if (((int) hrForException & -65536) == -2147024896 /*0x80070000*/)
            {
              if (((int) hrForException & (int) ushort.MaxValue) == 109)
              {
                this.__init__((object) 32 /*0x20*/, (object) exception.Message);
                return;
              }
              this.__init__((object) (uint) ((int) hrForException & (int) ushort.MaxValue), (object) exception.Message);
              return;
            }
            break;
          }
          catch (MethodAccessException ex)
          {
            break;
          }
          catch (SecurityException ex)
          {
            break;
          }
      }
      this.InitAndGetClrException((object) exception);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int GetHRForException(System.Exception exception)
    {
      return Marshal.GetHRForException(exception);
    }

    public _EnvironmentError()
      : base(PythonExceptions.EnvironmentError)
    {
    }

    public _EnvironmentError(PythonType type)
      : base(type)
    {
    }

    public new static object __new__(
      PythonType cls,
      [ParamDictionary] IDictionary<object, object> kwArgs,
      params object[] args)
    {
      return Activator.CreateInstance(cls.UnderlyingSystemType, (object) cls);
    }

    public object errno
    {
      get => this._errno;
      set => this._errno = value;
    }

    public object strerror
    {
      get => this._strerror;
      set => this._strerror = value;
    }

    public object filename
    {
      get => this._filename;
      set => this._filename = value;
    }
  }

  [PythonType("UnicodeTranslateError")]
  [PythonHidden(new PlatformID[] {})]
  [DynamicBaseType]
  [Serializable]
  public class _UnicodeTranslateError : PythonExceptions.BaseException
  {
    private object _start;
    private object _reason;
    private object _object;
    private object _end;
    private object _encoding;

    public override void __init__(params object[] args)
    {
      if (args.Length != 4)
        throw PythonOps.TypeError("function takes exactly 4 arguments ({0} given)", (object) args.Length);
      this.@object = args[0] is string || args[0] is Extensible<string> ? args[0] : throw PythonOps.TypeError("argument 4 must be unicode, not {0}", (object) DynamicHelpers.GetPythonType(args[0]).Name);
      this.start = args[1];
      this.end = args[2];
      this.reason = args[3] is string || args[3] is Extensible<string> ? args[3] : throw PythonOps.TypeError("argument 4 must be str, not {0}", (object) DynamicHelpers.GetPythonType(args[3]).Name);
      base.__init__(args);
    }

    public _UnicodeTranslateError()
      : base(PythonExceptions.UnicodeTranslateError)
    {
    }

    public _UnicodeTranslateError(PythonType type)
      : base(type)
    {
    }

    public new static object __new__(
      PythonType cls,
      [ParamDictionary] IDictionary<object, object> kwArgs,
      params object[] args)
    {
      return Activator.CreateInstance(cls.UnderlyingSystemType, (object) cls);
    }

    public object start
    {
      get => this._start;
      set => this._start = value;
    }

    public object reason
    {
      get => this._reason;
      set => this._reason = value;
    }

    public object @object
    {
      get => this._object;
      set => this._object = value;
    }

    public object end
    {
      get => this._end;
      set => this._end = value;
    }

    public object encoding
    {
      get => this._encoding;
      set => this._encoding = value;
    }
  }

  [PythonType("WindowsError")]
  [PythonHidden(new PlatformID[] {})]
  [DynamicBaseType]
  [Serializable]
  public class _WindowsError : PythonExceptions._EnvironmentError
  {
    internal const int ERROR_FILE_NOT_FOUND = 2;
    internal const int ERROR_PATH_NOT_FOUND = 3;
    internal const int ERROR_TOO_MANY_OPEN_FILES = 4;
    internal const int ERROR_ACCESS_DENIED = 5;
    internal const int ERROR_INVALID_HANDLE = 6;
    internal const int ERROR_ARENA_TRASHED = 7;
    internal const int ERROR_NOT_ENOUGH_MEMORY = 8;
    internal const int ERROR_INVALID_BLOCK = 9;
    internal const int ERROR_BAD_ENVIRONMENT = 10;
    internal const int ERROR_BAD_FORMAT = 11;
    internal const int ERROR_INVALID_DRIVE = 15;
    internal const int ERROR_CURRENT_DIRECTORY = 16 /*0x10*/;
    internal const int ERROR_NOT_SAME_DEVICE = 17;
    internal const int ERROR_NO_MORE_FILES = 18;
    internal const int ERROR_WRITE_PROTECT = 19;
    internal const int ERROR_BAD_UNIT = 20;
    internal const int ERROR_NOT_READY = 21;
    internal const int ERROR_BAD_COMMAND = 22;
    internal const int ERROR_CRC = 23;
    internal const int ERROR_BAD_LENGTH = 24;
    internal const int ERROR_SEEK = 25;
    internal const int ERROR_NOT_DOS_DISK = 26;
    internal const int ERROR_SECTOR_NOT_FOUND = 27;
    internal const int ERROR_OUT_OF_PAPER = 28;
    internal const int ERROR_WRITE_FAULT = 29;
    internal const int ERROR_READ_FAULT = 30;
    internal const int ERROR_GEN_FAILURE = 31 /*0x1F*/;
    internal const int ERROR_SHARING_VIOLATION = 32 /*0x20*/;
    internal const int ERROR_LOCK_VIOLATION = 33;
    internal const int ERROR_WRONG_DISK = 34;
    internal const int ERROR_SHARING_BUFFER_EXCEEDED = 36;
    internal const int ERROR_BAD_NETPATH = 53;
    internal const int ERROR_NETWORK_ACCESS_DENIED = 65;
    internal const int ERROR_BAD_NET_NAME = 67;
    internal const int ERROR_FILE_EXISTS = 80 /*0x50*/;
    internal const int ERROR_CANNOT_MAKE = 82;
    internal const int ERROR_FAIL_I24 = 83;
    internal const int ERROR_NO_PROC_SLOTS = 89;
    internal const int ERROR_DRIVE_LOCKED = 108;
    internal const int ERROR_BROKEN_PIPE = 109;
    internal const int ERROR_DISK_FULL = 112 /*0x70*/;
    internal const int ERROR_INVALID_TARGET_HANDLE = 114;
    internal const int ERROR_WAIT_NO_CHILDREN = 128 /*0x80*/;
    internal const int ERROR_CHILD_NOT_COMPLETE = 129;
    internal const int ERROR_DIRECT_ACCESS_HANDLE = 130;
    internal const int ERROR_SEEK_ON_DEVICE = 132;
    internal const int ERROR_DIR_NOT_EMPTY = 145;
    internal const int ERROR_NOT_LOCKED = 158;
    internal const int ERROR_BAD_PATHNAME = 161;
    internal const int ERROR_MAX_THRDS_REACHED = 164;
    internal const int ERROR_LOCK_FAILED = 167;
    internal const int ERROR_ALREADY_EXISTS = 183;
    internal const int ERROR_INVALID_STARTING_CODESEG = 188;
    internal const int ERROR_INVALID_STACKSEG = 189;
    internal const int ERROR_INVALID_MODULETYPE = 190;
    internal const int ERROR_INVALID_EXE_SIGNATURE = 191;
    internal const int ERROR_EXE_MARKED_INVALID = 192 /*0xC0*/;
    internal const int ERROR_BAD_EXE_FORMAT = 193;
    internal const int ERROR_ITERATED_DATA_EXCEEDS_64k = 194;
    internal const int ERROR_INVALID_MINALLOCSIZE = 195;
    internal const int ERROR_DYNLINK_FROM_INVALID_RING = 196;
    internal const int ERROR_IOPL_NOT_ENABLED = 197;
    internal const int ERROR_INVALID_SEGDPL = 198;
    internal const int ERROR_AUTODATASEG_EXCEEDS_64k = 199;
    internal const int ERROR_RING2SEG_MUST_BE_MOVABLE = 200;
    internal const int ERROR_RELOC_CHAIN_XEEDS_SEGLIM = 201;
    internal const int ERROR_INFLOOP_IN_RELOC_CHAIN = 202;
    internal const int ERROR_FILENAME_EXCED_RANGE = 206;
    internal const int ERROR_NESTING_NOT_ALLOWED = 215;
    internal const int ERROR_NO_DATA = 232;
    internal const int ERROR_NOT_ENOUGH_QUOTA = 1816;
    internal const int ERROR_INVALID_PARAMETER = 87;
    internal const int ERROR_INVALID_NAME = 123;
    internal const int ERROR_FILE_INVALID = 1006;
    internal const int ERROR_MAPPED_ALIGNMENT = 1132;
    private object _winerror;

    public override void __init__(params object[] args)
    {
      if ((args.Length == 2 || args.Length == 3) && !(args[0] is int))
        throw PythonOps.TypeError("an integer is required for the 1st argument of WindowsError");
      base.__init__(args);
      if (args != null && (args.Length == 2 || args.Length == 3))
        this.winerror = args[0];
      if (!(this.errno is int errno))
        return;
      switch (errno)
      {
        case 2:
        case 3:
        case 15:
        case 18:
        case 53:
        case 67:
        case 161:
        case 206:
          this.errno = (object) 2;
          break;
        case 4:
          this.errno = (object) 24;
          break;
        case 5:
        case 16 /*0x10*/:
        case 19:
        case 20:
        case 21:
        case 22:
        case 23:
        case 24:
        case 25:
        case 26:
        case 27:
        case 28:
        case 29:
        case 30:
        case 31 /*0x1F*/:
        case 32 /*0x20*/:
        case 33:
        case 34:
        case 36:
        case 65:
        case 82:
        case 83:
        case 108:
        case 132:
        case 158:
        case 167:
          this.errno = (object) 13;
          break;
        case 6:
        case 114:
        case 130:
          this.errno = (object) 9;
          break;
        case 7:
        case 8:
        case 9:
        case 1816:
          this.errno = (object) 12;
          break;
        case 10:
          this.errno = (object) 7;
          break;
        case 11:
        case 188:
        case 189:
        case 190:
        case 191:
        case 192 /*0xC0*/:
        case 193:
        case 194:
        case 195:
        case 196:
        case 197:
        case 198:
        case 199:
        case 200:
        case 201:
        case 202:
          this.errno = (object) 8;
          break;
        case 17:
          this.errno = (object) 18;
          break;
        case 80 /*0x50*/:
        case 183:
          this.errno = (object) 17;
          break;
        case 89:
        case 164:
        case 215:
          this.errno = (object) 11;
          break;
        case 109:
        case 232:
          this.errno = (object) 32 /*0x20*/;
          break;
        case 112 /*0x70*/:
          this.errno = (object) 28;
          break;
        case 128 /*0x80*/:
        case 129:
          this.errno = (object) 10;
          break;
        case 145:
          this.errno = (object) 41;
          break;
        default:
          this.errno = (object) 22;
          break;
      }
    }

    public _WindowsError()
      : base(PythonExceptions.WindowsError)
    {
    }

    public _WindowsError(PythonType type)
      : base(type)
    {
    }

    public new static object __new__(
      PythonType cls,
      [ParamDictionary] IDictionary<object, object> kwArgs,
      params object[] args)
    {
      return Activator.CreateInstance(cls.UnderlyingSystemType, (object) cls);
    }

    public object winerror
    {
      get => this._winerror;
      set => this._winerror = value;
    }
  }

  [PythonType("UnicodeDecodeError")]
  [PythonHidden(new PlatformID[] {})]
  [DynamicBaseType]
  [Serializable]
  public class _UnicodeDecodeError : PythonExceptions.BaseException
  {
    private object _start;
    private object _reason;
    private object _object;
    private object _end;
    private object _encoding;

    [PythonHidden(new PlatformID[] {})]
    protected internal override void InitializeFromClr(System.Exception exception)
    {
      if (exception is DecoderFallbackException fallbackException)
      {
        StringBuilder stringBuilder = new StringBuilder();
        if (fallbackException.BytesUnknown != null)
        {
          for (int index = 0; index < fallbackException.BytesUnknown.Length; ++index)
            stringBuilder.Append((char) fallbackException.BytesUnknown[index]);
        }
        this.__init__(fallbackException.Data.Contains((object) "encoding") ? fallbackException.Data[(object) "encoding"] : (object) "unknown", (object) stringBuilder.ToString(), (object) fallbackException.Index, (object) (fallbackException.Index + stringBuilder.Length), (object) fallbackException.Message);
      }
      else
        base.InitializeFromClr(exception);
    }

    public override string ToString()
    {
      if (!(this._object is string str))
        return this._reason.ToString();
      return str.Length == 1 ? $"'{this._encoding}' codec can't decode byte 0x{(ValueType) (byte) str[0]:x2} in position {this._start}: {this._reason}" : $"'{this._encoding}' codec can't decode bytes in position {this._start}-{this._end}: {this._reason}";
    }

    public _UnicodeDecodeError()
      : base(PythonExceptions.UnicodeDecodeError)
    {
    }

    public _UnicodeDecodeError(PythonType type)
      : base(type)
    {
    }

    public new static object __new__(
      PythonType cls,
      [ParamDictionary] IDictionary<object, object> kwArgs,
      params object[] args)
    {
      return Activator.CreateInstance(cls.UnderlyingSystemType, (object) cls);
    }

    public void __init__(
      object encoding,
      object @object,
      object start,
      object end,
      object reason)
    {
      this._encoding = encoding;
      this._object = @object;
      this._start = start;
      this._end = end;
      this._reason = reason;
      this.args = (object) PythonTuple.MakeTuple(encoding, @object, start, end, reason);
    }

    public override void __init__(params object[] args)
    {
      if (args == null || args.Length != 5)
        throw PythonOps.TypeError("__init__ takes exactly 5 arguments ({0} given)", (object) args.Length);
      this.__init__(this.encoding, this.@object, this.start, this.end, this.reason);
    }

    public object start
    {
      get => this._start;
      set => this._start = value;
    }

    public object reason
    {
      get => this._reason;
      set => this._reason = value;
    }

    public object @object
    {
      get => this._object;
      set => this._object = value;
    }

    public object end
    {
      get => this._end;
      set => this._end = value;
    }

    public object encoding
    {
      get => this._encoding;
      set => this._encoding = value;
    }
  }

  [PythonType("UnicodeEncodeError")]
  [PythonHidden(new PlatformID[] {})]
  [DynamicBaseType]
  [Serializable]
  public class _UnicodeEncodeError : PythonExceptions.BaseException
  {
    private object _start;
    private object _reason;
    private object _object;
    private object _end;
    private object _encoding;

    [PythonHidden(new PlatformID[] {})]
    protected internal override void InitializeFromClr(System.Exception exception)
    {
      if (exception is EncoderFallbackException fallbackException)
        this.__init__(exception.Data.Contains((object) "encoding") ? exception.Data[(object) "encoding"] : (object) "unknown", (object) new string(fallbackException.CharUnknown, 1), (object) fallbackException.Index, (object) (fallbackException.Index + 1), (object) exception.Message);
      else
        base.InitializeFromClr(exception);
    }

    public override string ToString() => this.reason.ToString();

    public _UnicodeEncodeError()
      : base(PythonExceptions.UnicodeEncodeError)
    {
    }

    public _UnicodeEncodeError(PythonType type)
      : base(type)
    {
    }

    public new static object __new__(
      PythonType cls,
      [ParamDictionary] IDictionary<object, object> kwArgs,
      params object[] args)
    {
      return Activator.CreateInstance(cls.UnderlyingSystemType, (object) cls);
    }

    public void __init__(
      object encoding,
      object @object,
      object start,
      object end,
      object reason)
    {
      this._encoding = encoding;
      this._object = @object;
      this._start = start;
      this._end = end;
      this._reason = reason;
      this.args = (object) PythonTuple.MakeTuple(encoding, @object, start, end, reason);
    }

    public override void __init__(params object[] args)
    {
      if (args == null || args.Length != 5)
        throw PythonOps.TypeError("__init__ takes exactly 5 arguments ({0} given)", (object) args.Length);
      this.__init__(this.encoding, this.@object, this.start, this.end, this.reason);
    }

    public object start
    {
      get => this._start;
      set => this._start = value;
    }

    public object reason
    {
      get => this._reason;
      set => this._reason = value;
    }

    public object @object
    {
      get => this._object;
      set => this._object = value;
    }

    public object end
    {
      get => this._end;
      set => this._end = value;
    }

    public object encoding
    {
      get => this._encoding;
      set => this._encoding = value;
    }
  }

  [PythonType("SystemExit")]
  [PythonHidden(new PlatformID[] {})]
  [DynamicBaseType]
  [Serializable]
  public class _SystemExit : PythonExceptions.BaseException
  {
    private object _code;

    public override void __init__(params object[] args)
    {
      base.__init__(args);
      if (args == null || args.Length == 0)
        return;
      this.code = this.message;
    }

    public _SystemExit()
      : base(PythonExceptions.SystemExit)
    {
    }

    public _SystemExit(PythonType type)
      : base(type)
    {
    }

    public new static object __new__(
      PythonType cls,
      [ParamDictionary] IDictionary<object, object> kwArgs,
      params object[] args)
    {
      return Activator.CreateInstance(cls.UnderlyingSystemType, (object) cls);
    }

    public object code
    {
      get => this._code;
      set => this._code = value;
    }
  }

  [Serializable]
  private class ExceptionDataWrapper : MarshalByRefObject
  {
    private readonly object _value;

    public ExceptionDataWrapper(object value) => this._value = value;

    public object Value => this._value;
  }
}
