// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.ICommand
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB.Shell;

internal interface ICommand
{
  bool IsCommand(StringScanner s);

  IEnumerable<BsonValue> Execute(StringScanner s, LiteEngine engine);
}
