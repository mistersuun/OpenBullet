// Decompiled with JetBrains decompiler
// Type: LiteDB.BsonMapper
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

#nullable disable
namespace LiteDB;

public class BsonMapper
{
  private const int MAX_DEPTH = 20;
  private Dictionary<Type, EntityMapper> _entities = new Dictionary<Type, EntityMapper>();
  private Dictionary<Type, Func<object, BsonValue>> _customSerializer = new Dictionary<Type, Func<object, BsonValue>>();
  private Dictionary<Type, Func<BsonValue, object>> _customDeserializer = new Dictionary<Type, Func<BsonValue, object>>();
  private readonly Func<Type, object> _typeInstantiator;
  public static BsonMapper Global = new BsonMapper();
  public Func<string, string> ResolveFieldName;
  public Action<Type, MemberInfo, MemberMapper> ResolveMember;
  public Func<Type, string> ResolveCollectionName;
  private Regex _lowerCaseDelimiter = new Regex("(?!(^[A-Z]))([A-Z])", RegexOptions.Compiled);
  private HashSet<Type> _bsonTypes = new HashSet<Type>()
  {
    typeof (string),
    typeof (int),
    typeof (long),
    typeof (bool),
    typeof (Guid),
    typeof (DateTime),
    typeof (byte[]),
    typeof (ObjectId),
    typeof (double),
    typeof (Decimal)
  };
  private HashSet<Type> _basicTypes = new HashSet<Type>()
  {
    typeof (short),
    typeof (ushort),
    typeof (uint),
    typeof (float),
    typeof (char),
    typeof (byte),
    typeof (sbyte)
  };

  public bool SerializeNullValues { get; set; }

  public bool TrimWhitespace { get; set; }

  public bool EmptyStringToNull { get; set; }

  public bool IncludeFields { get; set; }

  public bool IncludeNonPublic { get; set; }

  public BsonMapper(Func<Type, object> customTypeInstantiator = null)
  {
    this.SerializeNullValues = false;
    this.TrimWhitespace = true;
    this.EmptyStringToNull = true;
    this.ResolveFieldName = (Func<string, string>) (s => s);
    this.ResolveMember = (Action<Type, MemberInfo, MemberMapper>) ((t, mi, mm) => { });
    this.ResolveCollectionName = (Func<Type, string>) (t => !LiteDB.Reflection.IsList(t) ? t.Name : LiteDB.Reflection.GetListItemType(t).Name);
    this.IncludeFields = false;
    this._typeInstantiator = customTypeInstantiator ?? new Func<Type, object>(LiteDB.Reflection.CreateInstance);
    this.RegisterType<Uri>((Func<Uri, BsonValue>) (uri => (BsonValue) uri.AbsoluteUri), (Func<BsonValue, Uri>) (bson => new Uri(bson.AsString)));
    this.RegisterType<DateTimeOffset>((Func<DateTimeOffset, BsonValue>) (value => new BsonValue(value.UtcDateTime)), (Func<BsonValue, DateTimeOffset>) (bson => (DateTimeOffset) bson.AsDateTime.ToUniversalTime()));
    this.RegisterType<TimeSpan>((Func<TimeSpan, BsonValue>) (value => new BsonValue(value.Ticks)), (Func<BsonValue, TimeSpan>) (bson => new TimeSpan(bson.AsInt64)));
    this.RegisterType<Regex>((Func<Regex, BsonValue>) (r =>
    {
      if (r.Options == RegexOptions.None)
        return new BsonValue(r.ToString());
      return (BsonValue) new BsonDocument()
      {
        {
          "p",
          (BsonValue) r.ToString()
        },
        {
          "o",
          (BsonValue) (int) r.Options
        }
      };
    }), (Func<BsonValue, Regex>) (value => !value.IsString ? new Regex(value.AsDocument["p"].AsString, (RegexOptions) value.AsDocument["o"].AsInt32) : new Regex((string) value)));
  }

  public void RegisterType<T>(Func<T, BsonValue> serialize, Func<BsonValue, T> deserialize)
  {
    this._customSerializer[typeof (T)] = (Func<object, BsonValue>) (o => serialize((T) o));
    this._customDeserializer[typeof (T)] = (Func<BsonValue, object>) (b => (object) deserialize(b));
  }

  public void RegisterType(
    Type type,
    Func<object, BsonValue> serialize,
    Func<BsonValue, object> deserialize)
  {
    this._customSerializer[type] = (Func<object, BsonValue>) (o => serialize(o));
    this._customDeserializer[type] = (Func<BsonValue, object>) (b => deserialize(b));
  }

  public EntityBuilder<T> Entity<T>() => new EntityBuilder<T>(this);

  public string GetPath<T>(Expression<Func<T, object>> property)
  {
    return new QueryVisitor<T>(this).GetPath((Expression) property);
  }

  public string GetField<T>(Expression<Func<T, object>> property)
  {
    return new QueryVisitor<T>(this).GetField((Expression) property);
  }

  public Query GetQuery<T>(Expression<Func<T, bool>> predicate)
  {
    return new QueryVisitor<T>(this).Visit(predicate);
  }

  public BsonMapper UseCamelCase()
  {
    this.ResolveFieldName = (Func<string, string>) (s => char.ToLower(s[0]).ToString() + s.Substring(1));
    return this;
  }

  public BsonMapper UseLowerCaseDelimiter(char delimiter = '_')
  {
    this.ResolveFieldName = (Func<string, string>) (s => this._lowerCaseDelimiter.Replace(s, delimiter.ToString() + "$2").ToLower());
    return this;
  }

  internal EntityMapper GetEntityMapper(Type type)
  {
    EntityMapper entityMapper;
    if (!this._entities.TryGetValue(type, out entityMapper))
    {
      lock (this._entities)
      {
        if (!this._entities.TryGetValue(type, out entityMapper))
          return this._entities[type] = this.BuildEntityMapper(type);
      }
    }
    return entityMapper;
  }

  protected virtual EntityMapper BuildEntityMapper(Type type)
  {
    EntityMapper entityMapper = new EntityMapper()
    {
      Members = new List<MemberMapper>(),
      ForType = type
    };
    Type attributeType1 = typeof (BsonIdAttribute);
    Type attributeType2 = typeof (BsonIgnoreAttribute);
    Type attributeType3 = typeof (BsonFieldAttribute);
    Type attributeType4 = typeof (BsonRefAttribute);
    IEnumerable<MemberInfo> typeMembers = this.GetTypeMembers(type);
    MemberInfo idMember = this.GetIdMember(typeMembers);
    foreach (MemberInfo memberInfo in typeMembers)
    {
      if (!memberInfo.IsDefined(attributeType2, true))
      {
        string name = this.ResolveFieldName(memberInfo.Name);
        BsonFieldAttribute bsonFieldAttribute = (BsonFieldAttribute) ((IEnumerable<object>) memberInfo.GetCustomAttributes(attributeType3, false)).FirstOrDefault<object>();
        if (bsonFieldAttribute != null && bsonFieldAttribute.Name != null)
          name = bsonFieldAttribute.Name;
        if (memberInfo == idMember)
          name = "_id";
        if (!BsonDocument.IsValidFieldName(name))
          throw LiteException.InvalidFormat(memberInfo.Name);
        GenericGetter genericGetter = LiteDB.Reflection.CreateGenericGetter(type, memberInfo);
        GenericSetter genericSetter = LiteDB.Reflection.CreateGenericSetter(type, memberInfo);
        BsonIdAttribute bsonIdAttribute = (BsonIdAttribute) ((IEnumerable<object>) memberInfo.GetCustomAttributes(attributeType1, false)).FirstOrDefault<object>();
        Type type1 = (object) (memberInfo as PropertyInfo) != null ? (memberInfo as PropertyInfo).PropertyType : (memberInfo as FieldInfo).FieldType;
        bool flag = LiteDB.Reflection.IsList(type1);
        MemberMapper member = new MemberMapper()
        {
          AutoId = bsonIdAttribute == null || bsonIdAttribute.AutoId,
          FieldName = name,
          MemberName = memberInfo.Name,
          DataType = type1,
          IsList = flag,
          UnderlyingType = flag ? LiteDB.Reflection.GetListItemType(type1) : type1,
          Getter = genericGetter,
          Setter = genericSetter
        };
        BsonRefAttribute bsonRefAttribute = (BsonRefAttribute) ((IEnumerable<object>) memberInfo.GetCustomAttributes(attributeType4, false)).FirstOrDefault<object>();
        if (bsonRefAttribute != null && (object) (memberInfo as PropertyInfo) != null)
          BsonMapper.RegisterDbRef(this, member, bsonRefAttribute.Collection ?? this.ResolveCollectionName((memberInfo as PropertyInfo).PropertyType));
        if (this.ResolveMember != null)
          this.ResolveMember(type, memberInfo, member);
        if (member.FieldName != null && !entityMapper.Members.Any<MemberMapper>((Func<MemberMapper, bool>) (x => x.FieldName == name)))
          entityMapper.Members.Add(member);
      }
    }
    return entityMapper;
  }

  protected virtual MemberInfo GetIdMember(IEnumerable<MemberInfo> members)
  {
    return LiteDB.Reflection.SelectMember(members, (Func<MemberInfo, bool>) (x => Attribute.IsDefined(x, typeof (BsonIdAttribute), true)), (Func<MemberInfo, bool>) (x => x.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)), (Func<MemberInfo, bool>) (x => x.Name.Equals(x.DeclaringType.Name + "Id", StringComparison.OrdinalIgnoreCase)));
  }

  protected virtual IEnumerable<MemberInfo> GetTypeMembers(Type type)
  {
    List<MemberInfo> typeMembers = new List<MemberInfo>();
    BindingFlags bindingAttr = this.IncludeNonPublic ? BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic : BindingFlags.Instance | BindingFlags.Public;
    typeMembers.AddRange(((IEnumerable<PropertyInfo>) type.GetProperties(bindingAttr)).Where<PropertyInfo>((Func<PropertyInfo, bool>) (x => x.CanRead && x.GetIndexParameters().Length == 0)).Select<PropertyInfo, MemberInfo>((Func<PropertyInfo, MemberInfo>) (x => (MemberInfo) x)));
    if (this.IncludeFields)
      typeMembers.AddRange(((IEnumerable<FieldInfo>) type.GetFields(bindingAttr)).Where<FieldInfo>((Func<FieldInfo, bool>) (x => !x.Name.EndsWith("k__BackingField") && !x.IsStatic)).Select<FieldInfo, MemberInfo>((Func<FieldInfo, MemberInfo>) (x => (MemberInfo) x)));
    return (IEnumerable<MemberInfo>) typeMembers;
  }

  internal static void RegisterDbRef(BsonMapper mapper, MemberMapper member, string collection)
  {
    member.IsDbRef = true;
    if (member.IsList)
      BsonMapper.RegisterDbRefList(mapper, member, collection);
    else
      BsonMapper.RegisterDbRefItem(mapper, member, collection);
  }

  private static void RegisterDbRefItem(BsonMapper mapper, MemberMapper member, string collection)
  {
    EntityMapper entity = mapper.GetEntityMapper(member.DataType);
    member.Serialize = (Func<object, BsonMapper, BsonValue>) ((obj, m) =>
    {
      if (obj == null)
        return BsonValue.Null;
      MemberMapper id = entity.Id;
      if (id == null)
        throw new LiteException("There is no _id field mapped in your type: " + member.DataType.FullName);
      object obj1 = id.Getter(obj);
      return (BsonValue) new BsonDocument()
      {
        {
          "$id",
          m.Serialize(obj1.GetType(), obj1, 0)
        },
        {
          "$ref",
          (BsonValue) collection
        }
      };
    });
    member.Deserialize = (Func<BsonValue, BsonMapper, object>) ((bson, m) =>
    {
      BsonValue bsonValue1 = bson.AsDocument["$id"];
      BsonMapper bsonMapper = m;
      Type forType = entity.ForType;
      BsonValue bsonValue2;
      if (!bsonValue1.IsNull)
        bsonValue2 = (BsonValue) new BsonDocument()
        {
          {
            "_id",
            bsonValue1
          }
        };
      else
        bsonValue2 = bson;
      return bsonMapper.Deserialize(forType, bsonValue2);
    });
  }

  private static void RegisterDbRefList(BsonMapper mapper, MemberMapper member, string collection)
  {
    EntityMapper entity = mapper.GetEntityMapper(member.UnderlyingType);
    member.Serialize = (Func<object, BsonMapper, BsonValue>) ((list, m) =>
    {
      if (list == null)
        return BsonValue.Null;
      BsonArray bsonArray = new BsonArray();
      MemberMapper id = entity.Id;
      foreach (object obj1 in (IEnumerable) list)
      {
        if (obj1 != null)
        {
          object obj2 = id.Getter(obj1);
          bsonArray.Add((BsonValue) new BsonDocument()
          {
            {
              "$id",
              m.Serialize(obj2.GetType(), obj2, 0)
            },
            {
              "$ref",
              (BsonValue) collection
            }
          });
        }
      }
      return (BsonValue) bsonArray;
    });
    member.Deserialize = (Func<BsonValue, BsonMapper, object>) ((bson, m) =>
    {
      BsonArray asArray = bson.AsArray;
      if (asArray.Count == 0)
        return m.Deserialize(member.DataType, (BsonValue) asArray);
      BsonArray bsonArray = new BsonArray();
      foreach (BsonValue bsonValue1 in asArray)
      {
        BsonValue bsonValue2 = bsonValue1.AsDocument["$id"];
        if (bsonValue2.IsNull)
          bsonArray.Add(bsonValue1);
        else
          bsonArray.Add((BsonValue) new BsonDocument()
          {
            {
              "_id",
              bsonValue2
            }
          });
      }
      return m.Deserialize(member.DataType, (BsonValue) bsonArray);
    });
  }

  public virtual object ToObject(Type type, BsonDocument doc)
  {
    if ((BsonValue) doc == (BsonValue) null)
      throw new ArgumentNullException(nameof (doc));
    return type == typeof (BsonDocument) ? (object) doc : this.Deserialize(type, (BsonValue) doc);
  }

  public virtual T ToObject<T>(BsonDocument doc) => (T) this.ToObject(typeof (T), doc);

  internal T Deserialize<T>(BsonValue value)
  {
    return value == (BsonValue) null ? default (T) : (T) this.Deserialize(typeof (T), value);
  }

  internal object Deserialize(Type type, BsonValue value)
  {
    if (value.IsNull)
      return (object) null;
    if (LiteDB.Reflection.IsNullable(type))
      type = LiteDB.Reflection.UnderlyingTypeOf(type);
    if (type == typeof (BsonValue))
      return (object) new BsonValue(value);
    if (type == typeof (BsonDocument))
      return (object) value.AsDocument;
    if (type == typeof (BsonArray))
      return (object) value.AsArray;
    if (this._bsonTypes.Contains(type))
      return value.RawValue;
    if (this._basicTypes.Contains(type))
      return Convert.ChangeType(value.RawValue, type);
    if (type == typeof (ulong))
      return (object) (ulong) (long) value.RawValue;
    if (type.GetTypeInfo().IsEnum)
      return Enum.Parse(type, value.AsString);
    Func<BsonValue, object> func;
    if (this._customDeserializer.TryGetValue(type, out func))
      return func(value);
    if (value.IsArray)
    {
      if (type == typeof (object))
        return this.DeserializeArray(typeof (object), value.AsArray);
      return type.IsArray ? this.DeserializeArray(type.GetElementType(), value.AsArray) : this.DeserializeList(type, value.AsArray);
    }
    if (!value.IsDocument)
      return value.RawValue;
    BsonDocument asDocument = value.AsDocument;
    BsonValue bsonValue;
    if (asDocument.RawValue.TryGetValue("_type", out bsonValue))
    {
      type = Type.GetType(bsonValue.AsString);
      if (type == (Type) null)
        throw LiteException.InvalidTypedName(bsonValue.AsString);
    }
    else if (type == typeof (object))
      type = typeof (Dictionary<string, object>);
    object dict = this._typeInstantiator(type);
    if (dict is IDictionary && type.GetTypeInfo().IsGenericType)
      this.DeserializeDictionary(type.GetTypeInfo().GetGenericArguments()[0], type.GetTypeInfo().GetGenericArguments()[1], (IDictionary) dict, value.AsDocument);
    else
      this.DeserializeObject(type, dict, asDocument);
    return dict;
  }

  private object DeserializeArray(Type type, BsonArray array)
  {
    Array instance = Array.CreateInstance(type, array.Count);
    int num = 0;
    foreach (BsonValue bsonValue in array)
      instance.SetValue(this.Deserialize(type, bsonValue), num++);
    return (object) instance;
  }

  private object DeserializeList(Type type, BsonArray value)
  {
    Type listItemType = LiteDB.Reflection.GetListItemType(type);
    IEnumerable instance = (IEnumerable) LiteDB.Reflection.CreateInstance(type);
    if (instance is IList list)
    {
      foreach (BsonValue bsonValue in value)
        list.Add(this.Deserialize(listItemType, bsonValue));
    }
    else
    {
      MethodInfo method = type.GetMethod("Add");
      foreach (BsonValue bsonValue in value)
        method.Invoke((object) instance, new object[1]
        {
          this.Deserialize(listItemType, bsonValue)
        });
    }
    return (object) instance;
  }

  private void DeserializeDictionary(Type K, Type T, IDictionary dict, BsonDocument value)
  {
    foreach (string key1 in (IEnumerable<string>) value.Keys)
    {
      object key2 = K.GetTypeInfo().IsEnum ? Enum.Parse(K, key1) : Convert.ChangeType((object) key1, K);
      object obj = this.Deserialize(T, value[key1]);
      dict.Add(key2, obj);
    }
  }

  private void DeserializeObject(Type type, object obj, BsonDocument value)
  {
    foreach (MemberMapper memberMapper in this.GetEntityMapper(type).Members.Where<MemberMapper>((Func<MemberMapper, bool>) (x => x.Setter != null)))
    {
      BsonValue bsonValue = value[memberMapper.FieldName];
      if (!bsonValue.IsNull)
      {
        if (memberMapper.Deserialize != null)
          memberMapper.Setter(obj, memberMapper.Deserialize(bsonValue, this));
        else
          memberMapper.Setter(obj, this.Deserialize(memberMapper.DataType, bsonValue));
      }
    }
  }

  public virtual BsonDocument ToDocument(Type type, object entity)
  {
    if (entity == null)
      throw new ArgumentNullException(nameof (entity));
    return entity is BsonDocument ? (BsonDocument) entity : this.Serialize(type, entity, 0).AsDocument;
  }

  public virtual BsonDocument ToDocument<T>(T entity)
  {
    return this.ToDocument(typeof (T), (object) entity).AsDocument;
  }

  internal BsonValue Serialize(Type type, object obj, int depth)
  {
    if (++depth > 20)
      throw LiteException.DocumentMaxDepth(20, type);
    if (obj == null)
      return BsonValue.Null;
    if ((object) (obj as BsonValue) != null)
      return new BsonValue((BsonValue) obj);
    switch (obj)
    {
      case string _:
        string str = this.TrimWhitespace ? (obj as string).Trim() : (string) obj;
        return this.EmptyStringToNull && str.Length == 0 ? BsonValue.Null : new BsonValue(str);
      case int num2:
        return new BsonValue(num2);
      case long num3:
        return new BsonValue(num3);
      case double num4:
        return new BsonValue(num4);
      case Decimal num5:
        return new BsonValue(num5);
      case byte[] _:
        return new BsonValue((byte[]) obj);
      default:
        if ((object) (obj as ObjectId) != null)
          return new BsonValue((ObjectId) obj);
        switch (obj)
        {
          case Guid guid:
            return new BsonValue(guid);
          case bool flag:
            return new BsonValue(flag);
          case DateTime dateTime:
            return new BsonValue(dateTime);
          case short _:
          case ushort _:
          case byte _:
          case sbyte _:
            return new BsonValue(Convert.ToInt32(obj));
          case uint _:
            return new BsonValue(Convert.ToInt64(obj));
          case ulong num1:
            return new BsonValue((long) num1);
          case float _:
            return new BsonValue(Convert.ToDouble(obj));
          case char _:
          case Enum _:
            return new BsonValue(obj.ToString());
          default:
            Func<object, BsonValue> func;
            if (this._customSerializer.TryGetValue(type, out func) || this._customSerializer.TryGetValue(obj.GetType(), out func))
              return func(obj);
            switch (obj)
            {
              case IDictionary _:
                if (type == typeof (object))
                  type = obj.GetType();
                return (BsonValue) this.SerializeDictionary(type.GetTypeInfo().GetGenericArguments()[1], obj as IDictionary, depth);
              case IEnumerable _:
                return (BsonValue) this.SerializeArray(LiteDB.Reflection.GetListItemType(obj.GetType()), obj as IEnumerable, depth);
              default:
                return (BsonValue) this.SerializeObject(type, obj, depth);
            }
        }
    }
  }

  private BsonArray SerializeArray(Type type, IEnumerable array, int depth)
  {
    BsonArray bsonArray = new BsonArray();
    foreach (object obj in array)
      bsonArray.Add(this.Serialize(type, obj, depth));
    return bsonArray;
  }

  private BsonDocument SerializeDictionary(Type type, IDictionary dict, int depth)
  {
    BsonDocument bsonDocument = new BsonDocument();
    foreach (object key in (IEnumerable) dict.Keys)
    {
      object obj = dict[key];
      bsonDocument.RawValue[key.ToString()] = this.Serialize(type, obj, depth);
    }
    return bsonDocument;
  }

  private BsonDocument SerializeObject(Type type, object obj, int depth)
  {
    BsonDocument bsonDocument = new BsonDocument();
    Type type1 = obj.GetType();
    EntityMapper entityMapper = this.GetEntityMapper(type1);
    Dictionary<string, BsonValue> rawValue = bsonDocument.RawValue;
    if (type != type1)
      rawValue["_type"] = new BsonValue($"{type1.FullName}, {type1.GetTypeInfo().Assembly.GetName().Name}");
    foreach (MemberMapper memberMapper in entityMapper.Members.Where<MemberMapper>((Func<MemberMapper, bool>) (x => x.Getter != null)))
    {
      object obj1 = memberMapper.Getter(obj);
      if (obj1 != null || this.SerializeNullValues || !(memberMapper.FieldName != "_id"))
        rawValue[memberMapper.FieldName] = memberMapper.Serialize == null ? this.Serialize(memberMapper.DataType, obj1, depth) : memberMapper.Serialize(obj1, this);
    }
    return bsonDocument;
  }
}
