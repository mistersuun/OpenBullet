// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.CachedGetKey
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;

#nullable disable
namespace IronPython.Runtime.Types;

internal abstract class CachedGetKey : IEquatable<CachedGetKey>
{
  public readonly string Name;

  public CachedGetKey(string name) => this.Name = name;

  public static CachedGetKey Make(string name, ExtensionMethodSet set)
  {
    return set.Id != int.MinValue ? (CachedGetKey) new CachedGetIdIntExtensionMethod(name, set.Id) : (CachedGetKey) new CachedGetIdWeakRefExtensionMethod(name, new WeakReference((object) set));
  }

  public override bool Equals(object obj) => obj is CachedGetKey key && this.Equals(key);

  public override int GetHashCode() => this.Name.GetHashCode();

  public abstract bool Equals(CachedGetKey key);
}
