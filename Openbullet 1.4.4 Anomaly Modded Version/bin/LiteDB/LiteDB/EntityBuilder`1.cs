// Decompiled with JetBrains decompiler
// Type: LiteDB.EntityBuilder`1
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Linq.Expressions;

#nullable disable
namespace LiteDB;

public class EntityBuilder<T>
{
  private BsonMapper _mapper;
  private EntityMapper _entity;

  internal EntityBuilder(BsonMapper mapper)
  {
    this._mapper = mapper;
    this._entity = mapper.GetEntityMapper(typeof (T));
  }

  public EntityBuilder<T> Ignore<K>(Expression<Func<T, K>> property)
  {
    return this.GetProperty<T, K>(property, (Action<MemberMapper>) (p => this._entity.Members.Remove(p)));
  }

  public EntityBuilder<T> Field<K>(Expression<Func<T, K>> property, string field)
  {
    if (field.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (field));
    return this.GetProperty<T, K>(property, (Action<MemberMapper>) (p => p.FieldName = field));
  }

  public EntityBuilder<T> Id<K>(Expression<Func<T, K>> property, bool autoId = true)
  {
    return this.GetProperty<T, K>(property, (Action<MemberMapper>) (p =>
    {
      p.FieldName = "_id";
      p.AutoId = autoId;
    }));
  }

  public EntityBuilder<T> DbRef<K>(Expression<Func<T, K>> property, string collection = null)
  {
    return this.GetProperty<T, K>(property, (Action<MemberMapper>) (p => BsonMapper.RegisterDbRef(this._mapper, p, collection ?? this._mapper.ResolveCollectionName(typeof (K)))));
  }

  private EntityBuilder<T> GetProperty<TK, K>(
    Expression<Func<TK, K>> property,
    Action<MemberMapper> action)
  {
    MemberMapper memberMapper = property != null ? this._entity.GetMember((Expression) property) : throw new ArgumentNullException(nameof (property));
    if (memberMapper == null)
      throw new ArgumentNullException(property.GetPath());
    action(memberMapper);
    return this;
  }
}
