// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.NoThrowGetBinderMember
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using System.Dynamic;

#nullable disable
namespace Newtonsoft.Json.Utilities;

internal class NoThrowGetBinderMember : GetMemberBinder
{
  private readonly GetMemberBinder _innerBinder;

  public NoThrowGetBinderMember(GetMemberBinder innerBinder)
    : base(innerBinder.Name, innerBinder.IgnoreCase)
  {
    this._innerBinder = innerBinder;
  }

  public override DynamicMetaObject FallbackGetMember(
    DynamicMetaObject target,
    DynamicMetaObject errorSuggestion)
  {
    DynamicMetaObject dynamicMetaObject = this._innerBinder.Bind(target, CollectionUtils.ArrayEmpty<DynamicMetaObject>());
    return new DynamicMetaObject(new NoThrowExpressionVisitor().Visit(dynamicMetaObject.Expression), dynamicMetaObject.Restrictions);
  }
}
