// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonSignal
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable disable
namespace IronPython.Modules;

public static class PythonSignal
{
  public const string __doc__ = "This module provides mechanisms to use signal handlers in Python.\r\n\r\nFunctions:\r\n\r\nsignal() -- set the action for a given signal\r\ngetsignal() -- get the signal action for a given signal\r\ndefault_int_handler() -- default SIGINT handler\r\n\r\nsignal constants:\r\nSIG_DFL -- used to refer to the system default handler\r\nSIG_IGN -- used to ignore the signal\r\nNSIG -- number of defined signals\r\nSIGINT, SIGTERM, etc. -- signal numbers\r\n\r\n*** IMPORTANT NOTICE ***\r\nA signal handler function is called with two arguments:\r\nthe first is the signal number, the second is the interrupted stack frame.";
  public const int NSIG = 23;
  public const int SIGABRT = 22;
  public const int SIGBREAK = 21;
  public const int SIGFPE = 8;
  public const int SIGILL = 4;
  public const int SIGINT = 2;
  public const int SIGSEGV = 11;
  public const int SIGTERM = 15;
  public const int SIG_DFL = 0;
  public const int SIG_IGN = 1;
  public const int CTRL_C_EVENT = 0;
  public const int CTRL_BREAK_EVENT = 1;
  public const int CTRL_CLOSE_EVENT = 2;
  public const int CTRL_LOGOFF_EVENT = 5;
  public const int CTRL_SHUTDOWN_EVENT = 6;
  public static BuiltinFunction default_int_handler = BuiltinFunction.MakeFunction(nameof (default_int_handler), ArrayUtils.ConvertAll<MemberInfo, MethodBase>(typeof (PythonSignal).GetMember("default_int_handlerImpl"), (Func<MemberInfo, MethodBase>) (x => (MethodBase) x)), typeof (PythonSignal));
  private static readonly object _PythonSignalStateKey = new object();
  private static readonly int[] _PySupportedSignals = new int[8]
  {
    22,
    21,
    8,
    4,
    2,
    11,
    15,
    6
  };

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    context.SetModuleState(PythonSignal._PythonSignalStateKey, (object) PythonSignal.MakeSignalState(context));
  }

  private static PythonSignal.PythonSignalState MakeSignalState(PythonContext context)
  {
    return Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX ? PythonSignal.MakePosixSignalState(context) : PythonSignal.MakeNtSignalState(context);
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static PythonSignal.PythonSignalState MakeNtSignalState(PythonContext context)
  {
    return (PythonSignal.PythonSignalState) new PythonSignal.NtSignalState(context);
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static PythonSignal.PythonSignalState MakePosixSignalState(PythonContext context)
  {
    return (PythonSignal.PythonSignalState) new PythonSignal.SimpleSignalState(context);
  }

  [PythonHidden(new PlatformID[] {})]
  [Documentation("default_int_handler(...)\r\n\r\nThe default handler for SIGINT installed by Python.\r\nIt raises KeyboardInterrupt.")]
  public static object default_int_handlerImpl(int signalnum, TraceBackFrame frame)
  {
    throw new KeyboardInterruptException("");
  }

  [Documentation("getsignal(sig) -> action\r\n\r\nReturn the current action for the given signal.  The return value can be:\r\nSIG_IGN -- if the signal is being ignored\r\nSIG_DFL -- if the default action for the signal is in effect\r\nNone -- if an unknown handler is in effect\r\nanything else -- the callable Python object used as a handler")]
  public static object getsignal(CodeContext context, int signalnum)
  {
    lock (PythonSignal.GetPythonSignalState(context).PySignalToPyHandler)
    {
      if (signalnum < 1 || signalnum > 22)
        throw PythonOps.ValueError("signal number out of range");
      return !PythonSignal.GetPythonSignalState(context).PySignalToPyHandler.ContainsKey(signalnum) ? (object) null : PythonSignal.GetPythonSignalState(context).PySignalToPyHandler[signalnum];
    }
  }

  [Documentation("signal(sig, action) -> action\r\n\r\nSet the action for the given signal.  The action can be SIG_DFL,\r\nSIG_IGN, or a callable Python object.  The previous action is\r\nreturned.  See getsignal() for possible return values.\r\n\r\n*** IMPORTANT NOTICE ***\r\nA signal handler function is called with two arguments:\r\nthe first is the signal number, the second is the interrupted stack frame.")]
  public static object signal(CodeContext context, int sig, object action)
  {
    if (sig < 1 || sig >= 23)
      throw PythonOps.ValueError("signal number out of range");
    if (Array.IndexOf<int>(PythonSignal._PySupportedSignals, sig) == -1)
      throw new RuntimeException("no IronPython support for given signal");
    if (action == null)
      throw PythonOps.TypeError("signal handler must be signal.SIG_IGN, signal.SIG_DFL, or a callable object");
    if (action.GetType() == typeof (int))
    {
      switch ((int) action)
      {
        case 0:
        case 1:
          break;
        default:
          throw PythonOps.TypeError("signal handler must be signal.SIG_IGN, signal.SIG_DFL, or a callable object");
      }
    }
    else if (action != PythonSignal.default_int_handler && !(action is PythonFunction) && !PythonOps.IsCallable(context, action))
      throw PythonOps.TypeError("signal handler must be signal.SIG_IGN, signal.SIG_DFL, or a callable object");
    object obj = (object) null;
    lock (PythonSignal.GetPythonSignalState(context).PySignalToPyHandler)
    {
      obj = PythonSignal.getsignal(context, sig);
      PythonSignal.GetPythonSignalState(context).PySignalToPyHandler[sig] = action;
    }
    return obj;
  }

  [Documentation("NOT YET IMPLEMENTED\r\n\r\nset_wakeup_fd(fd) -> fd\r\n\r\nSets the fd to be written to (with '\\0') when a signal\r\ncomes in.  A library can use this to wakeup select or poll.\r\nThe previous fd is returned.\r\n\r\nThe fd must be non-blocking.")]
  public static void set_wakeup_fd(CodeContext context, uint fd)
  {
    throw new NotImplementedException();
  }

  private static PythonSignal.PythonSignalState GetPythonSignalState(CodeContext context)
  {
    return (PythonSignal.PythonSignalState) context.LanguageContext.GetModuleState(PythonSignal._PythonSignalStateKey);
  }

  private static void SetPythonSignalState(CodeContext context, PythonSignal.PythonSignalState pss)
  {
    context.LanguageContext.SetModuleState(PythonSignal._PythonSignalStateKey, (object) pss);
  }

  internal class NtSignalState : PythonSignal.PythonSignalState
  {
    public PythonSignal.NativeSignal.WinSignalsHandler WinAllSignalsHandlerDelegate;

    public NtSignalState(PythonContext pc)
      : base(pc)
    {
      this.WinAllSignalsHandlerDelegate = new PythonSignal.NativeSignal.WinSignalsHandler(this.WindowsEventHandler);
      PythonSignal.NativeSignal.SetConsoleCtrlHandler(this.WinAllSignalsHandlerDelegate, true);
    }

    private bool WindowsEventHandler(uint winSignal)
    {
      int num;
      switch (winSignal)
      {
        case 0:
          num = 2;
          break;
        case 1:
          num = 21;
          break;
        case 2:
          num = 21;
          break;
        case 5:
          num = 21;
          break;
        case 6:
          num = 21;
          break;
        default:
          throw new Exception("unreachable");
      }
      bool flag;
      lock (this.PySignalToPyHandler)
      {
        if (this.PySignalToPyHandler[num].GetType() == typeof (int))
        {
          switch ((int) this.PySignalToPyHandler[num])
          {
            case 0:
              flag = false;
              break;
            case 1:
              flag = true;
              break;
            default:
              throw new Exception("unreachable");
          }
        }
        else if (this.PySignalToPyHandler[num] == PythonSignal.default_int_handler)
        {
          if (num != 2)
          {
            flag = true;
            PythonSignal.default_int_handlerImpl(num, (TraceBackFrame) null);
          }
          else
            flag = false;
        }
        else
        {
          flag = true;
          PythonSignal.PySignalHandler pySignalHandler = (PythonSignal.PySignalHandler) Converter.ConvertToDelegate(this.PySignalToPyHandler[num], typeof (PythonSignal.PySignalHandler));
          try
          {
            if (this.SignalPythonContext.PythonOptions.Frames)
            {
              object obj1 = pySignalHandler(num, SysModule._getframeImpl((CodeContext) null, 0, this.SignalPythonContext._mainThreadFunctionStack));
            }
            else
            {
              object obj2 = pySignalHandler(num, (TraceBackFrame) null);
            }
          }
          catch (Exception ex)
          {
            Console.WriteLine(this.SignalPythonContext.FormatException(ex));
          }
        }
      }
      return flag;
    }
  }

  internal static class NativeSignal
  {
    [DllImport("Kernel32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetConsoleCtrlHandler(
      PythonSignal.NativeSignal.WinSignalsHandler Handler,
      [MarshalAs(UnmanagedType.Bool)] bool Add);

    [DllImport("Kernel32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);

    internal delegate bool WinSignalsHandler(uint winSignal);
  }

  internal class PythonSignalState
  {
    public PythonContext SignalPythonContext;
    public Dictionary<int, object> PySignalToPyHandler;

    public PythonSignalState(PythonContext pc)
    {
      this.SignalPythonContext = pc;
      this.PySignalToPyHandler = new Dictionary<int, object>()
      {
        {
          22,
          (object) 0
        },
        {
          21,
          (object) 0
        },
        {
          8,
          (object) 0
        },
        {
          4,
          (object) 0
        },
        {
          2,
          (object) PythonSignal.default_int_handler
        },
        {
          11,
          (object) 0
        },
        {
          15,
          (object) 0
        }
      };
    }
  }

  private delegate object PySignalHandler(int signalnum, TraceBackFrame frame);

  internal class SimpleSignalState : PythonSignal.PythonSignalState
  {
    public SimpleSignalState(PythonContext pc)
      : base(pc)
    {
      Console.CancelKeyPress += new ConsoleCancelEventHandler(this.Console_CancelKeyPress);
    }

    private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
      int num;
      switch (e.SpecialKey)
      {
        case ConsoleSpecialKey.ControlC:
          num = 2;
          break;
        case ConsoleSpecialKey.ControlBreak:
          num = 21;
          break;
        default:
          throw new InvalidOperationException("unreachable");
      }
      lock (this.PySignalToPyHandler)
      {
        if (this.PySignalToPyHandler[num].GetType() == typeof (int))
        {
          switch ((int) this.PySignalToPyHandler[num])
          {
            case 0:
              break;
            case 1:
              e.Cancel = false;
              break;
            default:
              throw new Exception("unreachable");
          }
        }
        else if (this.PySignalToPyHandler[num] == PythonSignal.default_int_handler)
        {
          if (num == 2)
            return;
          e.Cancel = true;
          PythonSignal.default_int_handlerImpl(num, (TraceBackFrame) null);
        }
        else
        {
          PythonSignal.PySignalHandler pySignalHandler = (PythonSignal.PySignalHandler) Converter.ConvertToDelegate(this.PySignalToPyHandler[num], typeof (PythonSignal.PySignalHandler));
          try
          {
            if (this.SignalPythonContext.PythonOptions.Frames)
            {
              object obj1 = pySignalHandler(num, SysModule._getframeImpl((CodeContext) null, 0, this.SignalPythonContext._mainThreadFunctionStack));
            }
            else
            {
              object obj2 = pySignalHandler(num, (TraceBackFrame) null);
            }
          }
          catch (Exception ex)
          {
            Console.WriteLine(this.SignalPythonContext.FormatException(ex));
          }
          e.Cancel = true;
        }
      }
    }
  }
}
