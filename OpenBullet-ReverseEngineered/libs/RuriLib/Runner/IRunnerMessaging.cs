// Decompiled with JetBrains decompiler
// Type: RuriLib.Runner.IRunnerMessaging
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using RuriLib.Models;
using System;

#nullable disable
namespace RuriLib.Runner;

public interface IRunnerMessaging
{
  event Action<IRunnerMessaging, RuriLib.LogLevel, string, bool, int> MessageArrived;

  event Action<IRunnerMessaging> WorkerStatusChanged;

  event Action<IRunnerMessaging, Hit> FoundHit;

  event Action<IRunnerMessaging> ReloadProxies;

  event Action<IRunnerMessaging, Action> DispatchAction;

  event Action<IRunnerMessaging> SaveProgress;

  event Action<IRunnerMessaging> AskCustomInputs;
}
