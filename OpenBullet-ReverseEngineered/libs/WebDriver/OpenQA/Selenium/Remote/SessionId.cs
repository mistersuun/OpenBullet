// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.SessionId
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace OpenQA.Selenium.Remote;

public class SessionId
{
  private string sessionOpaqueKey;

  public SessionId(string opaqueKey) => this.sessionOpaqueKey = opaqueKey;

  public override string ToString() => this.sessionOpaqueKey;

  public override int GetHashCode() => this.sessionOpaqueKey.GetHashCode();

  public override bool Equals(object obj)
  {
    bool flag = false;
    if (obj is SessionId sessionId)
      flag = this.sessionOpaqueKey.Equals(sessionId.sessionOpaqueKey);
    return flag;
  }
}
