// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.StringReference
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

#nullable disable
namespace Newtonsoft.Json.Utilities;

internal readonly struct StringReference(char[] chars, int startIndex, int length)
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
