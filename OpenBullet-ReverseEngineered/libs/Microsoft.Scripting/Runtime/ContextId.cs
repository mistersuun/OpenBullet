// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.ContextId
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Runtime;

[Serializable]
public readonly struct ContextId : IEquatable<ContextId>
{
  private static Dictionary<object, ContextId> _contexts = new Dictionary<object, ContextId>();
  private static int _maxId = 1;
  public static readonly ContextId Empty = new ContextId();

  internal ContextId(int id) => this.Id = id;

  public static ContextId RegisterContext(object identifier)
  {
    lock (ContextId._contexts)
    {
      if (ContextId._contexts.TryGetValue(identifier, out ContextId _))
        throw Error.LanguageRegistered();
      return new ContextId(ContextId._maxId++);
    }
  }

  public static ContextId LookupContext(object identifier)
  {
    lock (ContextId._contexts)
    {
      ContextId contextId;
      if (ContextId._contexts.TryGetValue(identifier, out contextId))
        return contextId;
    }
    return ContextId.Empty;
  }

  public int Id { get; }

  public bool Equals(ContextId other) => this.Id == other.Id;

  public override int GetHashCode() => this.Id;

  public override bool Equals(object obj) => obj is ContextId other && this.Equals(other);

  public static bool operator ==(ContextId self, ContextId other) => self.Equals(other);

  public static bool operator !=(ContextId self, ContextId other) => !self.Equals(other);
}
