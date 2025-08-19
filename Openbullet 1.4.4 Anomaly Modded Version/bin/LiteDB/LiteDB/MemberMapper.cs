// Decompiled with JetBrains decompiler
// Type: LiteDB.MemberMapper
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;

#nullable disable
namespace LiteDB;

public class MemberMapper
{
  public bool AutoId { get; set; }

  public string MemberName { get; set; }

  public Type DataType { get; set; }

  public string FieldName { get; set; }

  public GenericGetter Getter { get; set; }

  public GenericSetter Setter { get; set; }

  public Func<object, BsonMapper, BsonValue> Serialize { get; set; }

  public Func<BsonValue, BsonMapper, object> Deserialize { get; set; }

  public bool IsDbRef { get; set; }

  public bool IsList { get; set; }

  public Type UnderlyingType { get; set; }
}
