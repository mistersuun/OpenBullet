// Decompiled with JetBrains decompiler
// Type: LiteDB.BsonIndexAttribute
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;

#nullable disable
namespace LiteDB;

[Obsolete("Do not use Index attribute, use EnsureIndex on database creation")]
public class BsonIndexAttribute : Attribute
{
  public bool Unique { get; private set; }

  public BsonIndexAttribute()
    : this(false)
  {
  }

  public BsonIndexAttribute(bool unique) => this.Unique = unique;
}
