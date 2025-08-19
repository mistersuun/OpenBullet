// Decompiled with JetBrains decompiler
// Type: LiteDB.JsonToken
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

#nullable disable
namespace LiteDB;

internal class JsonToken
{
  public string Token { get; set; }

  public JsonTokenType TokenType { get; set; }

  public void Expect(JsonTokenType type)
  {
    if (this.TokenType != type)
      throw LiteException.UnexpectedToken(this.Token);
  }

  public void Expect(JsonTokenType type1, JsonTokenType type2)
  {
    if (this.TokenType != type1 && this.TokenType != type2)
      throw LiteException.UnexpectedToken(this.Token);
  }
}
