// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.NoThrowGetBinderMember
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

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
