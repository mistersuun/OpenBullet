// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonSelect
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Modules;

public static class PythonSelect
{
  public const string __doc__ = "Provides support for asynchronous socket operations.";

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    context.EnsureModuleException((object) "selecterror", dict, "error", "select");
  }

  [Documentation("select(iwtd, owtd, ewtd[, timeout]) -> readlist, writelist, errlist\n\nBlock until sockets are available for reading or writing, until an error\noccurs, or until a the timeout expires. The first three parameters are\nsequences of socket objects (opened using the socket module). The last is a\ntimeout value, given in seconds as a float. If timeout is omitted, select()\nblocks until at least one socket is ready. A timeout of zero never blocks, but\ncan be used for polling.\n\nThe return value is a tuple of lists of sockets that are ready (subsets of\niwtd, owtd, and ewtd). If the timeout occurs before any sockets are ready, a\ntuple of three empty lists is returned.\n\nNote that select() on IronPython works only with sockets; it will not work with\nfiles or other objects.")]
  public static PythonTuple select(
    CodeContext context,
    object iwtd,
    object owtd,
    object ewtd,
    object timeout = null)
  {
    IronPython.Runtime.List socketList1;
    Dictionary<Socket, object> socketToOriginal1;
    PythonSelect.ProcessSocketSequence(context, iwtd, out socketList1, out socketToOriginal1);
    IronPython.Runtime.List socketList2;
    Dictionary<Socket, object> socketToOriginal2;
    PythonSelect.ProcessSocketSequence(context, owtd, out socketList2, out socketToOriginal2);
    IronPython.Runtime.List socketList3;
    Dictionary<Socket, object> socketToOriginal3;
    PythonSelect.ProcessSocketSequence(context, ewtd, out socketList3, out socketToOriginal3);
    int microSeconds;
    if (timeout == null)
    {
      microSeconds = -2;
    }
    else
    {
      double result;
      if (!Converter.TryConvertToDouble(timeout, out result))
        throw PythonOps.TypeErrorForTypeMismatch("float or None", timeout);
      microSeconds = (int) (1000000.0 * result);
    }
    try
    {
      Socket.Select((IList) socketList1, (IList) socketList2, (IList) socketList3, microSeconds);
    }
    catch (ArgumentNullException ex)
    {
      throw PythonSelect.MakeException(context, (object) PythonSelect.SocketExceptionToTuple(new SocketException(10022)));
    }
    catch (SocketException ex)
    {
      throw PythonSelect.MakeException(context, (object) PythonSelect.SocketExceptionToTuple(ex));
    }
    for (int index = 0; index < socketList1.__len__(); ++index)
      socketList1[index] = socketToOriginal1[(Socket) socketList1[index]];
    for (int index = 0; index < socketList2.__len__(); ++index)
      socketList2[index] = socketToOriginal2[(Socket) socketList2[index]];
    for (int index = 0; index < socketList3.__len__(); ++index)
      socketList3[index] = socketToOriginal3[(Socket) socketList3[index]];
    return PythonTuple.MakeTuple((object) socketList1, (object) socketList2, (object) socketList3);
  }

  private static PythonTuple SocketExceptionToTuple(SocketException e)
  {
    return PythonTuple.MakeTuple((object) (int) e.SocketErrorCode, (object) e.Message);
  }

  private static Exception MakeException(CodeContext context, object value)
  {
    return PythonExceptions.CreateThrowable((PythonType) context.LanguageContext.GetModuleState((object) "selecterror"), value);
  }

  private static void ProcessSocketSequence(
    CodeContext context,
    object sequence,
    out IronPython.Runtime.List socketList,
    out Dictionary<Socket, object> socketToOriginal)
  {
    socketToOriginal = new Dictionary<Socket, object>();
    socketList = new IronPython.Runtime.List();
    IEnumerator enumerator = PythonOps.GetEnumerator(sequence);
    while (enumerator.MoveNext())
    {
      object current = enumerator.Current;
      Socket socket = PythonSelect.ObjectToSocket(context, current);
      socketList.append((object) socket);
      socketToOriginal[socket] = current;
    }
  }

  private static Socket ObjectToSocket(CodeContext context, object obj)
  {
    if (obj is PythonSocket.socket socket1)
      return socket1._socket;
    long result;
    if (!Converter.TryConvertToInt64(obj, out result))
    {
      object o = obj;
      object boundAttr = PythonOps.GetBoundAttr(context, o, "fileno");
      result = Converter.ConvertToInt64(PythonCalls.Call(context, boundAttr));
    }
    Socket socket2 = result >= 0L ? PythonSocket.socket.HandleToSocket(result) : throw PythonOps.ValueError("file descriptor cannot be a negative number ({0})", (object) result);
    if (socket2 == null)
    {
      SocketException e = new SocketException(10038);
      throw PythonExceptions.CreateThrowable((PythonType) context.LanguageContext.GetModuleState((object) "selecterror"), (object) PythonSelect.SocketExceptionToTuple(e));
    }
    return socket2;
  }
}
