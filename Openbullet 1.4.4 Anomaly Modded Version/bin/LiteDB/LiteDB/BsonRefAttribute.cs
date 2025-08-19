// Decompiled with JetBrains decompiler
// Type: LiteDB.BsonRefAttribute
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;

#nullable disable
namespace LiteDB;

public class BsonRefAttribute : Attribute
{
  public string Collection { get; set; }

  public BsonRefAttribute(string collection) => this.Collection = collection;

  public BsonRefAttribute() => this.Collection = (string) null;
}
