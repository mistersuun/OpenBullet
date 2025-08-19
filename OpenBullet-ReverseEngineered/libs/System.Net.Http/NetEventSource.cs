// Decompiled with JetBrains decompiler
// Type: System.Net.NetEventSource
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Diagnostics.Tracing;
using System.Security;

#nullable disable
namespace System.Net;

[EventSource(Name = "Microsoft-System-Net", Guid = "501a994a-eb63-5415-9af1-1b031260f16c")]
[SecuritySafeCritical]
internal sealed class NetEventSource : EventSource
{
  private const int FunctionStartId = 1;
  private const int FunctionStopId = 2;
  private const int CriticalExceptionId = 3;
  private const int CriticalErrorId = 4;
  private static readonly NetEventSource s_log = new NetEventSource();

  private NetEventSource()
  {
  }

  public static NetEventSource Log => NetEventSource.s_log;

  [NonEvent]
  internal static void Enter(
    NetEventSource.ComponentType componentType,
    object obj,
    string method,
    object paramObject)
  {
    if (!NetEventSource.s_log.IsEnabled())
      return;
    string parametersName = "";
    int parametersHash = 0;
    string callerName;
    int callerHash;
    if (obj is string)
    {
      callerName = obj as string;
      callerHash = 0;
    }
    else
    {
      callerName = LoggingHash.GetObjectName(obj);
      callerHash = LoggingHash.HashInt(obj);
    }
    if (paramObject is string)
    {
      parametersName = paramObject as string;
      parametersHash = 0;
    }
    else if (paramObject != null)
    {
      parametersName = LoggingHash.GetObjectName(paramObject);
      parametersHash = LoggingHash.HashInt(paramObject);
    }
    NetEventSource.s_log.FunctionStart(callerName, callerHash, LoggingHash.GetObjectName((object) method), parametersName, parametersHash, componentType);
  }

  [Event(1)]
  internal unsafe void FunctionStart(
    string callerName,
    int callerHash,
    string method,
    string parametersName,
    int parametersHash,
    NetEventSource.ComponentType componentType)
  {
    fixed (char* chPtr1 = callerName)
      fixed (char* chPtr2 = method)
        fixed (char* chPtr3 = parametersName)
        {
          EventSource.EventData* eventDataPtr = stackalloc EventSource.EventData[6];
          ((EventSource.EventData) (IntPtr) eventDataPtr).DataPointer = (IntPtr) (void*) chPtr1;
          ((EventSource.EventData) (IntPtr) eventDataPtr).Size = (callerName.Length + 1) * 2;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 1)).DataPointer = (IntPtr) (void*) &callerHash;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 1)).Size = 4;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 2)).DataPointer = (IntPtr) (void*) chPtr2;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 2)).Size = (method.Length + 1) * 2;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 3)).DataPointer = (IntPtr) (void*) chPtr3;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 3)).Size = (parametersName.Length + 1) * 2;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 4)).DataPointer = (IntPtr) (void*) &parametersHash;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 4)).Size = 4;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 5)).DataPointer = (IntPtr) (void*) &componentType;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 5)).Size = 4;
          this.WriteEventCore(1, 6, eventDataPtr);
        }
  }

  [NonEvent]
  internal static void Exit(
    NetEventSource.ComponentType componentType,
    object obj,
    string method,
    object retObject)
  {
    if (!NetEventSource.s_log.IsEnabled())
      return;
    string parametersName = "";
    int parametersHash = 0;
    string callerName;
    int callerHash;
    if (obj is string)
    {
      callerName = obj as string;
      callerHash = 0;
    }
    else
    {
      callerName = LoggingHash.GetObjectName(obj);
      callerHash = LoggingHash.HashInt(obj);
    }
    if (retObject is string)
    {
      parametersName = retObject as string;
      parametersHash = 0;
    }
    else if (retObject != null)
    {
      parametersName = LoggingHash.GetObjectName(retObject);
      parametersHash = LoggingHash.HashInt(retObject);
    }
    NetEventSource.s_log.FunctionStop(callerName, callerHash, LoggingHash.GetObjectName((object) method), parametersName, parametersHash, componentType);
  }

  [Event(2)]
  internal unsafe void FunctionStop(
    string callerName,
    int callerHash,
    string method,
    string parametersName,
    int parametersHash,
    NetEventSource.ComponentType componentType)
  {
    fixed (char* chPtr1 = callerName)
      fixed (char* chPtr2 = method)
        fixed (char* chPtr3 = parametersName)
        {
          EventSource.EventData* eventDataPtr = stackalloc EventSource.EventData[6];
          ((EventSource.EventData) (IntPtr) eventDataPtr).DataPointer = (IntPtr) (void*) chPtr1;
          ((EventSource.EventData) (IntPtr) eventDataPtr).Size = (callerName.Length + 1) * 2;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 1)).DataPointer = (IntPtr) (void*) &callerHash;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 1)).Size = 4;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 2)).DataPointer = (IntPtr) (void*) chPtr2;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 2)).Size = (method.Length + 1) * 2;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 3)).DataPointer = (IntPtr) (void*) chPtr3;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 3)).Size = (parametersName.Length + 1) * 2;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 4)).DataPointer = (IntPtr) (void*) &parametersHash;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 4)).Size = 4;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 5)).DataPointer = (IntPtr) (void*) &componentType;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 5)).Size = 4;
          this.WriteEventCore(2, 6, eventDataPtr);
        }
  }

  [NonEvent]
  internal static void Exception(
    NetEventSource.ComponentType componentType,
    object obj,
    string method,
    System.Exception e)
  {
    NetEventSource.s_log.CriticalException(LoggingHash.GetObjectName(obj), LoggingHash.GetObjectName((object) method), LoggingHash.GetObjectName((object) e.Message), LoggingHash.HashInt(obj), LoggingHash.GetObjectName((object) e.StackTrace), componentType);
  }

  [Event(3)]
  internal unsafe void CriticalException(
    string objName,
    string method,
    string message,
    int objHash,
    string stackTrace,
    NetEventSource.ComponentType componentType)
  {
    fixed (char* chPtr1 = objName)
      fixed (char* chPtr2 = method)
        fixed (char* chPtr3 = message)
          fixed (char* chPtr4 = stackTrace)
          {
            EventSource.EventData* eventDataPtr = stackalloc EventSource.EventData[6];
            ((EventSource.EventData) (IntPtr) eventDataPtr).DataPointer = (IntPtr) (void*) chPtr1;
            ((EventSource.EventData) (IntPtr) eventDataPtr).Size = (objName.Length + 1) * 2;
            ((EventSource.EventData) (IntPtr) (eventDataPtr + 1)).DataPointer = (IntPtr) (void*) chPtr2;
            ((EventSource.EventData) (IntPtr) (eventDataPtr + 1)).Size = (method.Length + 1) * 2;
            ((EventSource.EventData) (IntPtr) (eventDataPtr + 2)).DataPointer = (IntPtr) (void*) chPtr3;
            ((EventSource.EventData) (IntPtr) (eventDataPtr + 2)).Size = (message.Length + 1) * 2;
            ((EventSource.EventData) (IntPtr) (eventDataPtr + 3)).DataPointer = (IntPtr) (void*) &objHash;
            ((EventSource.EventData) (IntPtr) (eventDataPtr + 3)).Size = 4;
            ((EventSource.EventData) (IntPtr) (eventDataPtr + 4)).DataPointer = (IntPtr) (void*) chPtr4;
            ((EventSource.EventData) (IntPtr) (eventDataPtr + 4)).Size = (stackTrace.Length + 1) * 2;
            ((EventSource.EventData) (IntPtr) (eventDataPtr + 5)).DataPointer = (IntPtr) (void*) &componentType;
            ((EventSource.EventData) (IntPtr) (eventDataPtr + 5)).Size = 4;
            this.WriteEventCore(3, 6, eventDataPtr);
          }
  }

  [NonEvent]
  internal static void PrintError(NetEventSource.ComponentType componentType, string msg)
  {
    if (msg == null)
      return;
    NetEventSource.s_log.CriticalError(LoggingHash.GetObjectName((object) msg), "", "", 0, componentType);
  }

  [NonEvent]
  internal static void PrintError(
    NetEventSource.ComponentType componentType,
    object obj,
    string method,
    string msg)
  {
    NetEventSource.s_log.CriticalError(LoggingHash.GetObjectName((object) msg), LoggingHash.GetObjectName((object) method), LoggingHash.GetObjectName(obj), LoggingHash.HashInt(obj), componentType);
  }

  [Event(4)]
  internal unsafe void CriticalError(
    string message,
    string method,
    string objName,
    int objHash,
    NetEventSource.ComponentType componentType)
  {
    fixed (char* chPtr1 = message)
      fixed (char* chPtr2 = method)
        fixed (char* chPtr3 = objName)
        {
          EventSource.EventData* eventDataPtr = stackalloc EventSource.EventData[5];
          ((EventSource.EventData) (IntPtr) eventDataPtr).DataPointer = (IntPtr) (void*) chPtr1;
          ((EventSource.EventData) (IntPtr) eventDataPtr).Size = (message.Length + 1) * 2;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 1)).DataPointer = (IntPtr) (void*) chPtr2;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 1)).Size = (method.Length + 1) * 2;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 2)).DataPointer = (IntPtr) (void*) chPtr3;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 2)).Size = (objName.Length + 1) * 2;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 3)).DataPointer = (IntPtr) (void*) &objHash;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 3)).Size = 4;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 4)).DataPointer = (IntPtr) (void*) &componentType;
          ((EventSource.EventData) (IntPtr) (eventDataPtr + 4)).Size = 4;
          this.WriteEventCore(4, 5, eventDataPtr);
        }
  }

  public class Keywords
  {
    public const EventKeywords Default = (EventKeywords) 1;
    public const EventKeywords Debug = (EventKeywords) 2;
    public const EventKeywords FunctionEntryExit = (EventKeywords) 4;
  }

  public enum ComponentType
  {
    Socket,
    Http,
    WebSocket,
    Security,
    NetworkInformation,
    Requests,
  }
}
