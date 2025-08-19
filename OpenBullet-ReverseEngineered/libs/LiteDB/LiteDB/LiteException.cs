// Decompiled with JetBrains decompiler
// Type: LiteDB.LiteException
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Reflection;

#nullable disable
namespace LiteDB;

public class LiteException : Exception
{
  public const int FILE_NOT_FOUND = 101;
  public const int INVALID_DATABASE = 103;
  public const int INVALID_DATABASE_VERSION = 104;
  public const int FILE_SIZE_EXCEEDED = 105;
  public const int COLLECTION_LIMIT_EXCEEDED = 106;
  public const int INDEX_DROP_IP = 108;
  public const int INDEX_LIMIT_EXCEEDED = 109;
  public const int INDEX_DUPLICATE_KEY = 110;
  public const int INDEX_KEY_TOO_LONG = 111;
  public const int INDEX_NOT_FOUND = 112 /*0x70*/;
  public const int INVALID_DBREF = 113;
  public const int LOCK_TIMEOUT = 120;
  public const int INVALID_COMMAND = 121;
  public const int ALREADY_EXISTS_COLLECTION_NAME = 122;
  public const int DATABASE_WRONG_PASSWORD = 123;
  public const int READ_ONLY_DATABASE = 125;
  public const int TRANSACTION_NOT_SUPPORTED = 126;
  public const int SYNTAX_ERROR = 127 /*0x7F*/;
  public const int INVALID_FORMAT = 200;
  public const int DOCUMENT_MAX_DEPTH = 201;
  public const int INVALID_CTOR = 202;
  public const int UNEXPECTED_TOKEN = 203;
  public const int INVALID_DATA_TYPE = 204;
  public const int PROPERTY_NOT_MAPPED = 206;
  public const int INVALID_TYPED_NAME = 207;
  public const int NEED_RECOVER = 208 /*0xD0*/;
  public const int PROPERTY_READ_WRITE = 209;

  public int ErrorCode { get; private set; }

  public string Line { get; private set; }

  public int Position { get; private set; }

  public LiteException(string message)
    : base(message)
  {
  }

  internal LiteException(int code, string message, params object[] args)
    : base(string.Format(message, args))
  {
    this.ErrorCode = code;
  }

  internal LiteException(int code, Exception inner, string message, params object[] args)
    : base(string.Format(message, args), inner)
  {
    this.ErrorCode = code;
  }

  internal static LiteException FileNotFound(string fileId)
  {
    return new LiteException(101, "File '{0}' not found.", new object[1]
    {
      (object) fileId
    });
  }

  internal static LiteException InvalidDatabase()
  {
    return new LiteException(103, "Datafile is not a LiteDB database.", new object[0]);
  }

  internal static LiteException InvalidDatabaseVersion(int version)
  {
    return new LiteException(104, "Invalid database version: {0}", new object[1]
    {
      (object) version
    });
  }

  internal static LiteException FileSizeExceeded(long limit)
  {
    return new LiteException(105, "Database size exceeds limit of {0}.", new object[1]
    {
      (object) StorageUnitHelper.FormatFileSize(limit)
    });
  }

  internal static LiteException CollectionLimitExceeded(int limit)
  {
    return new LiteException(106, "This database exceeded the maximum limit of collection names size: {0} bytes", new object[1]
    {
      (object) limit
    });
  }

  internal static LiteException IndexDropId()
  {
    return new LiteException(108, "Primary key index '_id' can't be dropped.", new object[0]);
  }

  internal static LiteException IndexLimitExceeded(string collection)
  {
    return new LiteException(109, "Collection '{0}' exceeded the maximum limit of indices: {1}", new object[2]
    {
      (object) collection,
      (object) 16 /*0x10*/
    });
  }

  internal static LiteException IndexDuplicateKey(string field, BsonValue key)
  {
    return new LiteException(110, "Cannot insert duplicate key in unique index '{0}'. The duplicate value is '{1}'.", new object[2]
    {
      (object) field,
      (object) key
    });
  }

  internal static LiteException IndexKeyTooLong()
  {
    return new LiteException(111, "Index key must be less than {0} bytes.", new object[1]
    {
      (object) 512 /*0x0200*/
    });
  }

  internal static LiteException IndexNotFound(string collection, string field)
  {
    return new LiteException(112 /*0x70*/, "Index not found on '{0}.{1}'.", new object[2]
    {
      (object) collection,
      (object) field
    });
  }

  internal static LiteException LockTimeout(TimeSpan ts)
  {
    return new LiteException(120, "Timeout. Database is locked for more than {0}.", new object[1]
    {
      (object) ts.ToString()
    });
  }

  internal static LiteException InvalidCommand(string command)
  {
    return new LiteException(121, "Command '{0}' is not a valid shell command.", new object[1]
    {
      (object) command
    });
  }

  internal static LiteException AlreadyExistsCollectionName(string newName)
  {
    return new LiteException(122, "New collection name '{0}' already exists.", new object[1]
    {
      (object) newName
    });
  }

  internal static LiteException DatabaseWrongPassword()
  {
    return new LiteException(123, "Invalid database password.", new object[0]);
  }

  internal static LiteException ReadOnlyDatabase()
  {
    return new LiteException(125, "This action are not supported because database was opened in read only mode.", new object[0]);
  }

  internal static LiteException InvalidDbRef(string path)
  {
    return new LiteException(113, "Invalid value for DbRef in path \"{0}\". Value must be document like {{ $ref: \"?\", $id: ? }}", new object[1]
    {
      (object) path
    });
  }

  internal static LiteException TransactionNotSupported(string method)
  {
    return new LiteException(126, "Transactions are not supported here: " + method, new object[0]);
  }

  internal static LiteException NeedRecover()
  {
    return new LiteException(208 /*0xD0*/, "Your datafile did not terminate properly during the writing process. Reopen the file", new object[0]);
  }

  internal static LiteException InvalidFormat(string field)
  {
    return new LiteException(200, "Invalid format: {0}", new object[1]
    {
      (object) field
    });
  }

  internal static LiteException DocumentMaxDepth(int depth, Type type)
  {
    return new LiteException(201, "Document has more than {0} nested documents in '{1}'. Check for circular references (use DbRef).", new object[2]
    {
      (object) depth,
      type == (Type) null ? (object) "-" : (object) type.Name
    });
  }

  internal static LiteException InvalidCtor(Type type, Exception inner)
  {
    return new LiteException(202, inner, "Failed to create instance for type '{0}' from assembly '{1}'. Checks if the class has a public constructor with no parameters.", new object[2]
    {
      (object) type.FullName,
      (object) type.AssemblyQualifiedName
    });
  }

  internal static LiteException UnexpectedToken(string token)
  {
    return new LiteException(203, "Unexpected JSON token: {0}", new object[1]
    {
      (object) token
    });
  }

  internal static LiteException InvalidDataType(string field, BsonValue value)
  {
    return new LiteException(204, "Invalid BSON data type '{0}' on field '{1}'.", new object[2]
    {
      (object) value.Type,
      (object) field
    });
  }

  internal static LiteException PropertyReadWrite(PropertyInfo prop)
  {
    return new LiteException(209, "'{0}' property must have public getter and setter.", new object[1]
    {
      (object) prop.Name
    });
  }

  internal static LiteException PropertyNotMapped(string name)
  {
    return new LiteException(206, "Property '{0}' was not mapped into BsonDocument.", new object[1]
    {
      (object) name
    });
  }

  internal static LiteException InvalidTypedName(string type)
  {
    return new LiteException(207, "Type '{0}' not found in current domain (_type format is 'Type.FullName, AssemblyName').", new object[1]
    {
      (object) type
    });
  }

  internal static LiteException SyntaxError(StringScanner s, string message = "Unexpected token")
  {
    return new LiteException((int) sbyte.MaxValue, message, new object[0])
    {
      Line = s.Source,
      Position = s.Index
    };
  }
}
