// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.StringReference
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace Newtonsoft.Json.Utilities;

internal struct StringReference(char[] chars, int startIndex, int length)
{
  private readonly char[] _chars = chars;
  private readonly int _startIndex = startIndex;
  private readonly int _length = length;

  public char this[int i] => this._chars[i];

  public char[] Chars => this._chars;

  public int StartIndex => this._startIndex;

  public int Length => this._length;

  public override string ToString() => new string(this._chars, this._startIndex, this._length);
}
