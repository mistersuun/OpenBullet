// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonObject
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

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
