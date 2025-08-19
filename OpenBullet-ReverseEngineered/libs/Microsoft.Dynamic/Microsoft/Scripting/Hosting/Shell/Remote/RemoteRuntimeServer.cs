// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Shell.Remote.RemoteRuntimeServer
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Serialization.Formatters;

#nullable disable
namespace Microsoft.Scripting.Hosting.Shell.Remote;

public static class RemoteRuntimeServer
{
  internal const string CommandDispatcherUri = "CommandDispatcherUri";
  internal const string RemoteRuntimeArg = "-X:RemoteRuntimeChannel";

  private static TimeSpan GetSevenDays() => new TimeSpan(7, 0, 0, 0);

  internal static IpcChannel CreateChannel(string channelName, string portName)
  {
    BinaryServerFormatterSinkProvider serverSinkProvider = new BinaryServerFormatterSinkProvider();
    serverSinkProvider.TypeFilterLevel = TypeFilterLevel.Full;
    Hashtable properties = new Hashtable();
    properties[(object) "name"] = (object) channelName;
    properties[(object) nameof (portName)] = (object) portName;
    properties[(object) "exclusiveAddressUse"] = (object) false;
    return new IpcChannel((IDictionary) properties, (IClientChannelSinkProvider) null, (IServerChannelSinkProvider) serverSinkProvider);
  }

  internal static void StartServer(string remoteRuntimeChannelName, ScriptScope scope)
  {
    IpcChannel channel = RemoteRuntimeServer.CreateChannel("ipc", remoteRuntimeChannelName);
    LifetimeServices.LeaseTime = RemoteRuntimeServer.GetSevenDays();
    LifetimeServices.LeaseManagerPollTime = RemoteRuntimeServer.GetSevenDays();
    LifetimeServices.RenewOnCallTime = RemoteRuntimeServer.GetSevenDays();
    LifetimeServices.SponsorshipTimeout = RemoteRuntimeServer.GetSevenDays();
    ChannelServices.RegisterChannel((IChannel) channel, false);
    try
    {
      RemotingServices.Marshal((MarshalByRefObject) new RemoteCommandDispatcher(scope), "CommandDispatcherUri");
      Console.WriteLine("{7FF032BB-DB03-4255-89DE-641CA195E5FA}");
      Console.ReadLine();
    }
    finally
    {
      ChannelServices.UnregisterChannel((IChannel) channel);
    }
  }
}
