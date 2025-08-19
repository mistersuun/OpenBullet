// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonArray
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Bson;

internal class BsonArray : BsonToken, IEnumerable<BsonToken>, IEnumerable
{
  private readonly List<BsonToken> _children = new List<BsonToken>();

  public void Add(BsonToken token)
  {
    this._children.Add(token);
    token.Parent = (BsonToken) this;
  }

  public override BsonType Type => BsonType.Array;

  public IEnumerator<BsonToken> GetEnumerator()
  {
    return (IEnumerator<BsonToken>) this._children.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
