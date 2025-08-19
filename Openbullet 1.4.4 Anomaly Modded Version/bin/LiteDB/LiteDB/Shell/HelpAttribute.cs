// Decompiled with JetBrains decompiler
// Type: LiteDB.Shell.HelpAttribute
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;

#nullable disable
namespace LiteDB.Shell;

public class HelpAttribute : Attribute
{
  public string Category { get; set; }

  public string Name { get; set; }

  public string Syntax { get; set; }

  public string Description { get; set; }

  public string[] Examples { get; set; } = new string[0];
}
