// Decompiled with JetBrains decompiler
// Type: IronPython.SQLite.StatementType
// Assembly: IronPython.SQLite, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7222B477-FDAF-4AA1-A0E3-CD8AE6ED7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.SQLite.dll

#nullable disable
namespace IronPython.SQLite;

internal enum StatementType
{
  Unknown,
  Select,
  Insert,
  Update,
  Delete,
  Replace,
  Other,
}
