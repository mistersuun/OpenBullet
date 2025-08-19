// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.EnumValue`1
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace Newtonsoft.Json.Utilities;

internal class EnumValue<T> where T : struct
{
  private readonly string _name;
  private readonly T _value;

  public string Name => this._name;

  public T Value => this._value;

  public EnumValue(string name, T value)
  {
    this._name = name;
    this._value = value;
  }
}
