// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Html5.ISessionStorage
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Collections.ObjectModel;

#nullable disable
namespace OpenQA.Selenium.Html5;

public interface ISessionStorage
{
  int Count { get; }

  string GetItem(string key);

  ReadOnlyCollection<string> KeySet();

  void SetItem(string key, string value);

  string RemoveItem(string key);

  void Clear();
}
