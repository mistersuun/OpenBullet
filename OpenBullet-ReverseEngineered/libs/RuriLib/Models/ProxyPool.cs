// Decompiled with JetBrains decompiler
// Type: RuriLib.Models.ProxyPool
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Extreme.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#nullable disable
namespace RuriLib.Models;

public class ProxyPool
{
  private static Random rng = new Random();

  public List<CProxy> Proxies { get; } = new List<CProxy>();

  public List<CProxy> Alive
  {
    get
    {
      return this.Proxies.Where<CProxy>((Func<CProxy, bool>) (p => p.Status == Status.AVAILABLE || p.Status == Status.BUSY)).ToList<CProxy>();
    }
  }

  public List<CProxy> Available
  {
    get
    {
      return this.Proxies.Where<CProxy>((Func<CProxy, bool>) (p => p.Status == Status.AVAILABLE)).ToList<CProxy>();
    }
  }

  public List<CProxy> Banned
  {
    get
    {
      return this.Proxies.Where<CProxy>((Func<CProxy, bool>) (p => p.Status == Status.BANNED)).ToList<CProxy>();
    }
  }

  public List<CProxy> Bad
  {
    get
    {
      return this.Proxies.Where<CProxy>((Func<CProxy, bool>) (p => p.Status == Status.BAD)).ToList<CProxy>();
    }
  }

  private bool Locked { get; set; }

  public ProxyPool(IEnumerable<string> proxies, ProxyType type, bool shuffle)
  {
    this.Proxies = proxies.Select<string, CProxy>((Func<string, CProxy>) (p => new CProxy(p, type))).ToList<CProxy>();
    if (!shuffle)
      return;
    ProxyPool.Shuffle<CProxy>((IList<CProxy>) this.Proxies);
  }

  public ProxyPool(List<CProxy> proxies, bool shuffle = false)
  {
    this.Proxies = IOManager.CloneProxies(proxies);
    this.UnbanAll();
    if (!shuffle)
      return;
    ProxyPool.Shuffle<CProxy>((IList<CProxy>) this.Proxies);
  }

  public void ClearCF()
  {
    foreach (CProxy proxy in this.Proxies)
    {
      proxy.Clearance = "";
      proxy.Cfduid = "";
    }
  }

  public void UnbanAll()
  {
    foreach (CProxy proxy in this.Proxies)
    {
      if (proxy.Status == Status.BANNED || proxy.Status == Status.BAD)
      {
        proxy.Status = Status.AVAILABLE;
        proxy.Hooked = 0;
        proxy.Uses = 0;
      }
    }
    this.ClearCF();
  }

  public CProxy GetProxy(bool evenBusy, int maxUses, bool neverBan = false)
  {
    while (this.Locked)
      Thread.Sleep(10);
    this.Locked = true;
    CProxy proxy = !evenBusy ? (maxUses != 0 ? this.Available.Where<CProxy>((Func<CProxy, bool>) (p => p.Uses < maxUses)).FirstOrDefault<CProxy>() : this.Available.FirstOrDefault<CProxy>()) : (maxUses != 0 ? this.Alive.Where<CProxy>((Func<CProxy, bool>) (p => p.Uses < maxUses)).FirstOrDefault<CProxy>() : this.Alive.FirstOrDefault<CProxy>());
    if (proxy != null)
    {
      if (maxUses > 0 && proxy.Uses > maxUses && !neverBan)
      {
        proxy.Status = Status.BANNED;
        this.Locked = false;
        return (CProxy) null;
      }
      proxy.Status = Status.BUSY;
      ++proxy.Hooked;
    }
    this.Locked = false;
    return proxy;
  }

  public void RemoveDuplicates()
  {
    foreach (CProxy cproxy in this.Proxies.GroupBy<CProxy, string>((Func<CProxy, string>) (p => p.Proxy)).Where<IGrouping<string, CProxy>>((Func<IGrouping<string, CProxy>, bool>) (grp => grp.Count<CProxy>() > 1)).Select<IGrouping<string, CProxy>, CProxy>((Func<IGrouping<string, CProxy>, CProxy>) (grp => grp.First<CProxy>())))
      this.Proxies.Remove(cproxy);
  }

  private static void Shuffle<T>(IList<T> list)
  {
    int count = list.Count;
    while (count > 1)
    {
      --count;
      int index = ProxyPool.rng.Next(count + 1);
      T obj = list[index];
      list[index] = list[count];
      list[count] = obj;
    }
  }
}
