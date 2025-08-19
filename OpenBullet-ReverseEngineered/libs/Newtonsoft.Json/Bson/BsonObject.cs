// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonObject
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Bson;

internal class BsonObject : BsonToken, IEnumerable<BsonProperty>, IEnumerable
{
  private readonly List<BsonProperty> _children = new List<BsonProperty>();

  public void Add(string name, BsonToken token)
  {
    this._children.Add(new BsonProperty()
    {
      Name = new BsonString((object) name, false),
      Value = token
    });
    token.Parent = (BsonToken) this;
  }

  public override BsonType Type => BsonType.Object;

  public IEnumerator<BsonProperty> GetEnumerator()
  {
    return (IEnumerator<BsonProperty>) this._children.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
