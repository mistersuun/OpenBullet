// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ComEventSinkProxy
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal sealed class ComEventSinkProxy : RealProxy
{
  private Guid _sinkIid;
  private ComEventSink _sink;
  private static readonly MethodInfo _methodInfoInvokeMember = typeof (ComEventSink).GetMethod("InvokeMember", BindingFlags.Instance | BindingFlags.Public);

  private ComEventSinkProxy()
  {
  }

  public ComEventSinkProxy(ComEventSink sink, Guid sinkIid)
    : base(typeof (ComEventSink))
  {
    this._sink = sink;
    this._sinkIid = sinkIid;
  }

  public override IntPtr SupportsInterface(ref Guid iid)
  {
    if (!(iid == this._sinkIid))
      return base.SupportsInterface(ref iid);
    IntPtr zero = IntPtr.Zero;
    return Marshal.GetIDispatchForObject((object) this._sink);
  }

  public override IMessage Invoke(IMessage msg)
  {
    ContractUtils.RequiresNotNull((object) msg, nameof (msg));
    if (!(msg is IMethodCallMessage methodCallMessage))
      throw new NotSupportedException();
    if (!((MethodInfo) methodCallMessage.MethodBase == ComEventSinkProxy._methodInfoInvokeMember))
      return (IMessage) RemotingServices.ExecuteMessage((MarshalByRefObject) this._sink, methodCallMessage);
    object ret;
    try
    {
      ret = this._sink.InvokeMember(methodCallMessage.Args[0] as string, (BindingFlags) methodCallMessage.Args[1], methodCallMessage.Args[2] as Binder, (object) null, methodCallMessage.Args[4] as object[], methodCallMessage.Args[5] as ParameterModifier[], methodCallMessage.Args[6] as CultureInfo, (string[]) null);
    }
    catch (Exception ex)
    {
      return (IMessage) new ReturnMessage(ex.InnerException, methodCallMessage);
    }
    return (IMessage) new ReturnMessage(ret, methodCallMessage.Args, methodCallMessage.ArgCount, (LogicalCallContext) null, methodCallMessage);
  }
}
