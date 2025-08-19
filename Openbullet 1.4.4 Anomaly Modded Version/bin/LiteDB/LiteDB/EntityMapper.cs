// Decompiled with JetBrains decompiler
// Type: LiteDB.EntityMapper
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#nullable disable
namespace LiteDB;

public class EntityMapper
{
  public List<MemberMapper> Members { get; set; }

  public MemberMapper Id
  {
    get
    {
      return this.Members.SingleOrDefault<MemberMapper>((Func<MemberMapper, bool>) (x => x.FieldName == "_id"));
    }
  }

  public Type ForType { get; set; }

  public MemberMapper GetMember(Expression expr)
  {
    return this.Members.FirstOrDefault<MemberMapper>((Func<MemberMapper, bool>) (x => x.MemberName == expr.GetPath()));
  }
}
