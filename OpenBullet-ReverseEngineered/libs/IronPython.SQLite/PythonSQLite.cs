// Decompiled with JetBrains decompiler
// Type: IronPython.SQLite.PythonSQLite
// Assembly: IronPython.SQLite, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7222B477-FDAF-4AA1-A0E3-CD8AE6ED7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.SQLite.dll

using Community.CsharpSqlite;
using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

#nullable disable
namespace IronPython.SQLite;

public static class PythonSQLite
{
  public const int SQLITE_OK = 0;
  public const int SQLITE_DENY = 1;
  public const int SQLITE_IGNORE = 2;
  public const int SQLITE_CREATE_INDEX = 1;
  public const int SQLITE_CREATE_TABLE = 2;
  public const int SQLITE_CREATE_TEMP_INDEX = 3;
  public const int SQLITE_CREATE_TEMP_TABLE = 4;
  public const int SQLITE_CREATE_TEMP_TRIGGER = 5;
  public const int SQLITE_CREATE_TEMP_VIEW = 6;
  public const int SQLITE_CREATE_TRIGGER = 7;
  public const int SQLITE_CREATE_VIEW = 8;
  public const int SQLITE_DELETE = 9;
  public const int SQLITE_DROP_INDEX = 10;
  public const int SQLITE_DROP_TABLE = 11;
  public const int SQLITE_DROP_TEMP_INDEX = 12;
  public const int SQLITE_DROP_TEMP_TABLE = 13;
  public const int SQLITE_DROP_TEMP_TRIGGER = 14;
  public const int SQLITE_DROP_TEMP_VIEW = 15;
  public const int SQLITE_DROP_TRIGGER = 16 /*0x10*/;
  public const int SQLITE_DROP_VIEW = 17;
  public const int SQLITE_INSERT = 18;
  public const int SQLITE_PRAGMA = 19;
  public const int SQLITE_READ = 20;
  public const int SQLITE_SELECT = 21;
  public const int SQLITE_TRANSACTION = 22;
  public const int SQLITE_UPDATE = 23;
  public const int SQLITE_ATTACH = 24;
  public const int SQLITE_DETACH = 25;
  public const int SQLITE_ALTER_TABLE = 26;
  public const int SQLITE_REINDEX = 27;
  public const int SQLITE_ANALYZE = 28;
  public const int PARSE_DECLTYPES = 1;
  public const int PARSE_COLNAMES = 2;
  public static PythonType Warning;
  public static PythonType Error;
  public static PythonType InterfaceError;
  public static PythonType DatabaseError;
  public static PythonType DataError;
  public static PythonType OperationalError;
  public static PythonType IntegrityError;
  public static PythonType InternalError;
  public static PythonType ProgrammingError;
  public static PythonType NotSupportedError;
  public static readonly string version = $"{2}.{7}.{9}";
  public static readonly string sqlite_version = Sqlite3.sqlite3_version.Replace("(C#)", "");
  public static PythonDictionary converters = new PythonDictionary();
  public static PythonDictionary adapters = new PythonDictionary();
  public static readonly Type OptimizedUnicode = typeof (string);
  internal static Encoding Latin1 = Encoding.GetEncoding("iso-8859-1");

  private static void InitModuleExceptions(PythonContext context, PythonDictionary dict)
  {
    PythonSQLite.Warning = context.EnsureModuleException((object) "sqlite.Warning", PythonExceptions.StandardError, dict, "Warning", "_sqlite3");
    PythonSQLite.Error = context.EnsureModuleException((object) "sqlite.Error", PythonExceptions.StandardError, dict, "Error", "_sqlite3");
    PythonSQLite.InterfaceError = context.EnsureModuleException((object) "sqlite.InterfaceError", PythonSQLite.Error, dict, "InterfaceError", "_sqlite3");
    PythonSQLite.DatabaseError = context.EnsureModuleException((object) "sqlite.DatabaseError", PythonSQLite.Error, dict, "DatabaseError", "_sqlite3");
    PythonSQLite.DataError = context.EnsureModuleException((object) "sqlite.DataError", PythonSQLite.DatabaseError, dict, "DataError", "_sqlite3");
    PythonSQLite.OperationalError = context.EnsureModuleException((object) "sqlite.OperationalError", PythonSQLite.DatabaseError, dict, "OperationalError", "_sqlite3");
    PythonSQLite.IntegrityError = context.EnsureModuleException((object) "sqlite.IntegrityError", PythonSQLite.DatabaseError, dict, "IntegrityError", "_sqlite3");
    PythonSQLite.InternalError = context.EnsureModuleException((object) "sqlite.InternalError", PythonSQLite.DatabaseError, dict, "InternalError", "_sqlite3");
    PythonSQLite.ProgrammingError = context.EnsureModuleException((object) "sqlite.ProgrammingError", PythonSQLite.DatabaseError, dict, "ProgrammingError", "_sqlite3");
    PythonSQLite.NotSupportedError = context.EnsureModuleException((object) "sqlite.NotSupportedError", PythonSQLite.DatabaseError, dict, "NotSupportedError", "_sqlite3");
  }

  internal static Exception MakeWarning(params object[] args)
  {
    return PythonSQLite.CreateThrowable(PythonSQLite.Warning, args);
  }

  internal static Exception MakeError(params object[] args)
  {
    return PythonSQLite.CreateThrowable(PythonSQLite.Error, args);
  }

  internal static Exception MakeInterfaceError(params object[] args)
  {
    return PythonSQLite.CreateThrowable(PythonSQLite.InterfaceError, args);
  }

  internal static Exception MakeDatabaseError(params object[] args)
  {
    return PythonSQLite.CreateThrowable(PythonSQLite.DatabaseError, args);
  }

  internal static Exception MakeDataError(params object[] args)
  {
    return PythonSQLite.CreateThrowable(PythonSQLite.DataError, args);
  }

  internal static Exception MakeOperationalError(params object[] args)
  {
    return PythonSQLite.CreateThrowable(PythonSQLite.OperationalError, args);
  }

  internal static Exception MakeIntegrityError(params object[] args)
  {
    return PythonSQLite.CreateThrowable(PythonSQLite.IntegrityError, args);
  }

  internal static Exception MakeInternalError(params object[] args)
  {
    return PythonSQLite.CreateThrowable(PythonSQLite.InternalError, args);
  }

  internal static Exception MakeProgrammingError(params object[] args)
  {
    return PythonSQLite.CreateThrowable(PythonSQLite.ProgrammingError, args);
  }

  internal static Exception MakeNotSupportedError(params object[] args)
  {
    return PythonSQLite.CreateThrowable(PythonSQLite.NotSupportedError, args);
  }

  internal static Exception GetSqliteError(Sqlite3.sqlite3 db, Sqlite3.Vdbe st)
  {
    if (st != null)
      Sqlite3.sqlite3_reset(st);
    int num = Sqlite3.sqlite3_errcode(db);
    string str = Sqlite3.sqlite3_errmsg(db);
    switch (num)
    {
      case 0:
        return (Exception) null;
      case 1:
      case 3:
      case 4:
      case 5:
      case 6:
      case 8:
      case 9:
      case 10:
      case 13:
      case 14:
      case 15:
      case 16 /*0x10*/:
      case 17:
        return PythonSQLite.MakeOperationalError((object) str);
      case 2:
      case 12:
        return PythonSQLite.MakeInternalError((object) str);
      case 7:
        return (Exception) new OutOfMemoryException();
      case 11:
        return PythonSQLite.MakeDatabaseError((object) str);
      case 18:
        return PythonSQLite.MakeDataError((object) str);
      case 19:
      case 20:
        return PythonSQLite.MakeIntegrityError((object) str);
      case 21:
        return PythonSQLite.MakeProgrammingError((object) str);
      default:
        return PythonSQLite.MakeDatabaseError((object) str);
    }
  }

  private static Exception CreateThrowable(PythonType type, params object[] args)
  {
    return PythonOps.CreateThrowable(type, args);
  }

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    PythonSQLite.InitModuleExceptions(context, dict);
  }

  public static object connect(
    CodeContext context,
    string database,
    double timeout = 0.0,
    int detect_types = 0,
    string isolation_level = null,
    bool check_same_thread = true,
    object factory = null,
    int cached_statements = 0)
  {
    if (factory == null)
      return (object) new PythonSQLite.Connection(database, timeout, detect_types, isolation_level, check_same_thread, factory, cached_statements);
    return PythonCalls.Call(context, factory, (object) database, (object) timeout, (object) detect_types, (object) isolation_level, (object) check_same_thread, factory, (object) cached_statements);
  }

  [Documentation("register_adapter(type, callable)\r\n\r\nRegisters an adapter with pysqlite's adapter registry. Non-standard.")]
  public static void register_adapter(CodeContext context, PythonType type, object adapter)
  {
    object pythonTypeFromType = (object) DynamicHelpers.GetPythonTypeFromType(typeof (PythonSQLite.PrepareProtocol));
    object key = (object) new PythonTuple((object) new object[2]
    {
      (object) type,
      pythonTypeFromType
    });
    PythonSQLite.adapters[key] = adapter;
  }

  [Documentation("register_converter(typename, callable)\r\n\r\nRegisters a converter with pysqlite. Non-standard.")]
  public static void register_converter(CodeContext context, string type, object converter)
  {
    PythonSQLite.converters[(object) type.ToUpperInvariant()] = converter;
  }

  [PythonType]
  public class Connection
  {
    public bool autocommit;
    private double _timeout;
    private string _isolation_level;
    public IDictionary collations = (IDictionary) new PythonDictionary();
    private List<WeakReference> statements = new List<WeakReference>();
    private int created_statements;
    private Dictionary<object, object> function_pinboard = new Dictionary<object, object>();
    internal Sqlite3.sqlite3 db;
    internal bool inTransaction;
    internal int thread_ident = Thread.CurrentThread.ManagedThreadId;
    private static readonly Dictionary<object, object> emptyKwargs = new Dictionary<object, object>();
    public PythonType Warning = PythonSQLite.Warning;
    public PythonType Error = PythonSQLite.Error;
    public PythonType InterfaceError = PythonSQLite.InterfaceError;
    public PythonType DataError = PythonSQLite.DataError;
    public PythonType DatabaseError = PythonSQLite.DatabaseError;
    public PythonType OperationalError = PythonSQLite.OperationalError;
    public PythonType InternalError = PythonSQLite.InternalError;
    public PythonType IntegrityError = PythonSQLite.IntegrityError;
    public PythonType ProgrammingError = PythonSQLite.ProgrammingError;
    public PythonType NotSupportedError = PythonSQLite.NotSupportedError;

    public int total_changes => Sqlite3.sqlite3_total_changes(this.db);

    public int detect_types { get; set; }

    public bool check_same_thread { get; set; }

    public double timeout
    {
      get => this._timeout;
      set
      {
        this._timeout = value;
        Sqlite3.sqlite3_busy_timeout(this.db, (int) (this._timeout * 1000.0));
      }
    }

    public string isolation_level
    {
      get => this._isolation_level;
      set => this.setIsolationLevel(value);
    }

    public string begin_statement { get; private set; }

    public object row_factory { get; set; }

    public object text_factory { get; set; }

    public Connection(
      string database,
      double timeout = 0.0,
      int detect_types = 0,
      string isolation_level = null,
      bool check_same_thread = true,
      object factory = null,
      int cached_statements = 0)
    {
      this.text_factory = (object) typeof (string);
      if (Sqlite3.sqlite3_open(database, out this.db) != 0)
        throw PythonSQLite.GetSqliteError(this.db, (Sqlite3.Vdbe) null);
      this.setIsolationLevel(isolation_level ?? "");
      this.detect_types = detect_types;
      this.timeout = timeout;
      this.check_same_thread = check_same_thread;
    }

    ~Connection()
    {
      if (this.db == null)
        return;
      Sqlite3.sqlite3_close(this.db);
    }

    [Documentation("Closes the connection.")]
    public void close()
    {
      this.checkThread();
      this.doAllStatements(PythonSQLite.Connection.AllStatmentsAction.Finalize);
      if (this.db == null)
        return;
      int num1 = 0;
      int num2;
      do
      {
        num2 = Sqlite3.sqlite3_close(this.db);
        if (num2 == 5)
        {
          GC.Collect();
          GC.WaitForPendingFinalizers();
        }
      }
      while (num2 == 5 && num1++ < 3);
      if (num2 != 0)
        throw PythonSQLite.GetSqliteError(this.db, (Sqlite3.Vdbe) null);
      this.db = (Sqlite3.sqlite3) null;
    }

    internal void begin()
    {
      Sqlite3.Vdbe ppStmt = (Sqlite3.Vdbe) null;
      string pzTail = (string) null;
      if (Sqlite3.sqlite3_prepare(this.db, this.begin_statement, -1, ref ppStmt, ref pzTail) != 0)
        throw PythonSQLite.GetSqliteError(this.db, ppStmt);
      if (Util.Step(ppStmt) != 101)
        throw PythonSQLite.GetSqliteError(this.db, ppStmt);
      this.inTransaction = true;
      if (Sqlite3.sqlite3_finalize(ppStmt) == 0)
        return;
      PythonSQLite.GetSqliteError(this.db, (Sqlite3.Vdbe) null);
    }

    [Documentation("Commit the current transaction.")]
    public void commit()
    {
      this.checkThread();
      this.checkConnection();
      if (!this.inTransaction)
        return;
      Sqlite3.Vdbe ppStmt = (Sqlite3.Vdbe) null;
      string pzTail = (string) null;
      if (Sqlite3.sqlite3_prepare(this.db, "COMMIT", -1, ref ppStmt, ref pzTail) != 0)
        throw PythonSQLite.GetSqliteError(this.db, (Sqlite3.Vdbe) null);
      if (Util.Step(ppStmt) != 101)
        throw PythonSQLite.GetSqliteError(this.db, ppStmt);
      this.inTransaction = false;
      if (Sqlite3.sqlite3_finalize(ppStmt) == 0)
        return;
      PythonSQLite.GetSqliteError(this.db, (Sqlite3.Vdbe) null);
    }

    [Documentation("Roll back the current transaction.")]
    public void rollback()
    {
      this.checkThread();
      this.checkConnection();
      if (!this.inTransaction)
        return;
      this.doAllStatements(PythonSQLite.Connection.AllStatmentsAction.Reset);
      Sqlite3.Vdbe ppStmt = (Sqlite3.Vdbe) null;
      string pzTail = (string) null;
      if (Sqlite3.sqlite3_prepare(this.db, "ROLLBACK", -1, ref ppStmt, ref pzTail) != 0)
        throw PythonSQLite.GetSqliteError(this.db, (Sqlite3.Vdbe) null);
      if (Util.Step(ppStmt) != 101)
        throw PythonSQLite.GetSqliteError(this.db, ppStmt);
      this.inTransaction = false;
      if (Sqlite3.sqlite3_finalize(ppStmt) == 0)
        return;
      PythonSQLite.GetSqliteError(this.db, (Sqlite3.Vdbe) null);
    }

    [Documentation("Return a cursor for the connection.")]
    public object cursor(CodeContext context, object factory = null)
    {
      this.checkThread();
      this.checkConnection();
      object obj = factory == null ? (object) new PythonSQLite.Cursor(context, this) : PythonCalls.Call(context, factory, (object) this);
      if (this.row_factory != null)
        context.LanguageContext.Operations.SetMember(obj, "row_factory", this.row_factory);
      return obj;
    }

    [Documentation("Executes a SQL statement. Non-standard.")]
    public object execute(
      CodeContext context,
      [ParamDictionary] IDictionary<object, object> kwargs,
      params object[] args)
    {
      object obj = this.cursor(context);
      object member = context.LanguageContext.Operations.GetMember(obj, nameof (execute));
      return PythonCalls.CallWithKeywordArgs(context, member, args, kwargs);
    }

    [Documentation("Repeatedly executes a SQL statement. Non-standard.")]
    public object executemany(
      CodeContext context,
      [ParamDictionary] IDictionary<object, object> kwargs,
      params object[] args)
    {
      object obj = this.cursor(context);
      object member = context.LanguageContext.Operations.GetMember(obj, nameof (executemany));
      return PythonCalls.CallWithKeywordArgs(context, member, args, kwargs);
    }

    [Documentation("Executes a multiple SQL statements at once. Non-standard.")]
    public object executescript(
      CodeContext context,
      [ParamDictionary] IDictionary<object, object> kwargs,
      params object[] args)
    {
      object obj = this.cursor(context);
      object member = context.LanguageContext.Operations.GetMember(obj, nameof (executescript));
      return PythonCalls.CallWithKeywordArgs(context, member, args, kwargs);
    }

    public object __call__(string sql)
    {
      this.dropUnusedStatementReferences();
      Statement target = new Statement(this, sql);
      this.statements.Add(new WeakReference((object) target));
      return (object) target;
    }

    private void dropUnusedStatementReferences()
    {
      if (this.created_statements++ < 200)
        return;
      this.created_statements = 0;
      List<WeakReference> weakReferenceList = new List<WeakReference>();
      foreach (WeakReference statement in this.statements)
      {
        if (statement.IsAlive)
          weakReferenceList.Add(statement);
      }
      this.statements = weakReferenceList;
    }

    [Documentation("Creates a new function. Non-standard.")]
    public void create_function(CodeContext context, string name, int narg, object func)
    {
      if (Sqlite3.sqlite3_create_function(this.db, name, narg, (byte) 5, (object) new object[2]
      {
        (object) context,
        func
      }, new Sqlite3.dxFunc(PythonSQLite.Connection.callUserFunction), (Sqlite3.dxStep) null, (Sqlite3.dxFinal) null) != 0)
        throw PythonSQLite.MakeOperationalError((object) "Error creating function");
      this.function_pinboard[func] = (object) null;
    }

    private static void callUserFunction(Sqlite3.sqlite3_context ctx, int argc, Sqlite3.Mem[] argv)
    {
      object[] objArray = (object[]) Sqlite3.sqlite3_user_data(ctx);
      CodeContext context = (CodeContext) objArray[0];
      object func = objArray[1];
      object[] args = PythonSQLite.Connection.buildPyParams(context, ctx, argc, argv);
      try
      {
        object result = PythonCalls.CallWithKeywordArgs(context, func, args, (IDictionary<object, object>) PythonSQLite.Connection.emptyKwargs);
        PythonSQLite.Connection.setResult(ctx, result);
      }
      catch (Exception ex)
      {
        Sqlite3.sqlite3_result_error(ctx, "user-defined function raised exception", -1);
      }
    }

    private static object[] buildPyParams(
      CodeContext context,
      Sqlite3.sqlite3_context ctx,
      int argc,
      Sqlite3.Mem[] argv)
    {
      object[] objArray = new object[argc];
      for (int index = 0; index < argc; ++index)
      {
        Sqlite3.Mem mem = argv[index];
        object obj;
        switch (Sqlite3.sqlite3_value_type(mem))
        {
          case 1:
            obj = (object) (int) Sqlite3.sqlite3_value_int64(mem);
            break;
          case 2:
            obj = (object) Sqlite3.sqlite3_value_double(mem);
            break;
          case 3:
            obj = (object) Sqlite3.sqlite3_value_text(mem);
            break;
          case 4:
            byte[] numArray = Sqlite3.sqlite3_value_blob(mem);
            obj = (object) new PythonBuffer(context, (object) numArray);
            break;
          default:
            obj = (object) null;
            break;
        }
        objArray[index] = obj;
      }
      return objArray;
    }

    private static void setResult(Sqlite3.sqlite3_context ctx, object result)
    {
      switch (result)
      {
        case null:
          Sqlite3.sqlite3_result_null(ctx);
          break;
        case bool flag:
          Sqlite3.sqlite3_result_int64(ctx, flag ? 1L : 0L);
          break;
        case int iVal1:
          Sqlite3.sqlite3_result_int64(ctx, (long) iVal1);
          break;
        case long iVal2:
          Sqlite3.sqlite3_result_int64(ctx, iVal2);
          break;
        case BigInteger iVal3:
          Sqlite3.sqlite3_result_int64(ctx, (long) iVal3);
          break;
        case float rVal1:
          Sqlite3.sqlite3_result_double(ctx, (double) rVal1);
          break;
        case double rVal2:
          Sqlite3.sqlite3_result_double(ctx, rVal2);
          break;
        case string _:
          Sqlite3.sqlite3_result_text(ctx, (string) result, -1, Sqlite3.SQLITE_TRANSIENT);
          break;
        case byte[] _:
          byte[] bytes = (byte[]) result;
          string z1 = PythonSQLite.Latin1.GetString(bytes, 0, bytes.Length);
          Sqlite3.sqlite3_result_blob(ctx, z1, z1.Length, Sqlite3.SQLITE_TRANSIENT);
          break;
        case PythonBuffer _:
          string z2 = ((PythonBuffer) result).__getslice__((object) 0, (object) null).ToString();
          Sqlite3.sqlite3_result_blob(ctx, z2, z2.Length, Sqlite3.SQLITE_TRANSIENT);
          break;
      }
    }

    [Documentation("Creates a new aggregate. Non-standard.")]
    public void create_aggregate(
      CodeContext context,
      string name,
      int n_arg,
      object aggregate_class)
    {
      PythonSQLite.Connection.UserAggregateThunk p = new PythonSQLite.Connection.UserAggregateThunk(context, name, aggregate_class);
      if (Sqlite3.sqlite3_create_function(this.db, name, n_arg, (byte) 5, (object) p, (Sqlite3.dxFunc) null, new Sqlite3.dxStep(p.stepCallback), new Sqlite3.dxFinal(p.finalCallback)) != 0)
        throw PythonSQLite.MakeOperationalError((object) "Error creating aggregate");
      this.function_pinboard[aggregate_class] = (object) null;
    }

    [Documentation("Creates a collation function. Non-standard.")]
    public void create_collation(params object[] args) => throw new NotImplementedException();

    [Documentation("Sets progress handler callback. Non-standard.")]
    public void set_progress_handler(params object[] args) => throw new NotImplementedException();

    [Documentation("Sets authorizer callback. Non-standard.")]
    public void set_authorizer(params object[] args) => throw new NotImplementedException();

    [Documentation("For context manager. Non-standard.")]
    public object __enter__() => (object) this;

    [Documentation("For context manager. Non-standard.")]
    public object __exit__(CodeContext context, object exc_type, object exc_value, object exc_tb)
    {
      DynamicOperations operations = context.LanguageContext.Operations;
      if (exc_type == null && exc_value == null && exc_tb == null)
      {
        object obj;
        if (operations.TryGetMember((object) this, "commit", out obj))
          operations.Invoke(obj);
        else
          this.commit();
      }
      else
      {
        object obj;
        if (operations.TryGetMember((object) this, "rollback", out obj))
          operations.Invoke(obj);
        else
          this.rollback();
      }
      return (object) false;
    }

    public object iterdump(CodeContext context)
    {
      throw new NotImplementedException("Not supported with C#-sqlite for unknown reasons.");
    }

    internal void checkConnection()
    {
      if (this.db == null)
        throw PythonSQLite.MakeProgrammingError((object) "Cannot operate on a closed database.");
    }

    internal void checkThread()
    {
      if (this.check_same_thread && this.thread_ident != Thread.CurrentThread.ManagedThreadId)
        throw PythonSQLite.MakeProgrammingError((object) ("SQLite objects created in a thread can only be used in that same thread." + "The object was created in thread id {0} and this is thread id {1}".Format((object) this.thread_ident, (object) Thread.CurrentThread.ManagedThreadId)));
    }

    internal static void verify(PythonSQLite.Connection connection)
    {
      PythonSQLite.Connection.verify(connection, false);
    }

    internal static void verify(PythonSQLite.Connection connection, bool closed)
    {
      if (!closed && (connection == null || connection.db == null))
        throw PythonSQLite.MakeProgrammingError((object) "Cannot operate on a closed database.");
      connection.checkThread();
    }

    private void setIsolationLevel(string isolation_level)
    {
      this.begin_statement = (string) null;
      if (isolation_level == null)
      {
        this._isolation_level = (string) null;
        this.commit();
        this.inTransaction = false;
      }
      else
      {
        this._isolation_level = isolation_level;
        this.begin_statement = "BEGIN " + isolation_level;
      }
    }

    private void doAllStatements(PythonSQLite.Connection.AllStatmentsAction action)
    {
      foreach (WeakReference statement in this.statements)
      {
        if (statement.IsAlive && statement.Target is Statement target)
        {
          if (action == PythonSQLite.Connection.AllStatmentsAction.Reset)
            target.Reset();
          else
            target.SqliteFinalize();
        }
      }
    }

    private enum AllStatmentsAction
    {
      Reset,
      Finalize,
    }

    private class UserAggregateThunk
    {
      private CodeContext context;
      private string name;
      private object aggregate_class;
      private object instance;

      public UserAggregateThunk(CodeContext context, string name, object aggregate_class)
      {
        this.context = context;
        this.aggregate_class = aggregate_class;
        this.name = name;
      }

      public void stepCallback(Sqlite3.sqlite3_context ctx, int argc, Sqlite3.Mem[] param)
      {
        if (this.instance == null)
        {
          try
          {
            this.instance = PythonCalls.Call(this.context, this.aggregate_class);
          }
          catch (Exception ex)
          {
            Sqlite3.sqlite3_result_error(ctx, "user-defined aggregate's '__init__' method raised error", -1);
            return;
          }
        }
        try
        {
          PythonCalls.CallWithKeywordArgs(this.context, this.context.LanguageContext.Operations.GetMember(this.instance, "step"), PythonSQLite.Connection.buildPyParams(this.context, ctx, argc, param), (IDictionary<object, object>) new Dictionary<object, object>());
        }
        catch (Exception ex)
        {
          if (ex is MissingMemberException)
            throw;
          Sqlite3.sqlite3_result_error(ctx, "user-defined aggregate's 'step' method raised error", -1);
        }
      }

      public void finalCallback(Sqlite3.sqlite3_context ctx)
      {
        if (this.instance == null)
          return;
        try
        {
          object result = this.context.LanguageContext.Operations.InvokeMember(this.instance, "finalize");
          PythonSQLite.Connection.setResult(ctx, result);
        }
        catch (Exception ex)
        {
          Sqlite3.sqlite3_result_error(ctx, "user-defined aggregate's 'finalize' method raised error", -1);
        }
      }
    }
  }

  [PythonType]
  public class Cursor : IEnumerable
  {
    public const string __doc__ = "SQLite database cursor class.";
    private Statement statement;
    private object next_row;
    private bool resultsDone;
    private int last_step_rc;
    private List<object> row_cast_map = new List<object>();
    private CodeContext context;

    public PythonTuple description { get; private set; }

    public int rowcount { get; private set; }

    public int? rownumber => new int?();

    public long? lastrowid { get; private set; }

    public object row_factory { get; set; }

    public int arraysize { get; set; }

    public PythonSQLite.Connection connection { get; private set; }

    public object callproc(string procname) => throw PythonSQLite.MakeNotSupportedError();

    public Cursor(CodeContext context, PythonSQLite.Connection connection)
    {
      this.context = context;
      this.connection = connection;
      this.arraysize = 1;
      this.rowcount = -1;
      if (this.connection == null)
        return;
      this.connection.checkThread();
    }

    ~Cursor()
    {
      if (this.statement == null)
        return;
      this.statement.Reset();
    }

    [Documentation("Closes the cursor.")]
    public void close()
    {
      this.connection.checkThread();
      this.connection.checkConnection();
      if (this.statement == null)
        return;
      this.statement.Reset();
    }

    [Documentation("Executes a SQL statement.")]
    public object execute(CodeContext context, object operation, object args = null)
    {
      return this.queryExecute(context, false, operation, args);
    }

    [Documentation("Repeatedly executes a SQL statement.")]
    public object executemany(CodeContext context, object operation, object args)
    {
      return this.queryExecute(context, true, operation, args);
    }

    private object queryExecute(
      CodeContext context,
      bool multiple,
      object operation_obj,
      object args)
    {
      string str1 = operation_obj is string ? (string) operation_obj : throw PythonSQLite.CreateThrowable(PythonExceptions.ValueError, (object) "operation parameter must be str or unicode");
      if (string.IsNullOrEmpty(str1))
        return (object) null;
      this.connection.checkThread();
      this.connection.checkConnection();
      this.next_row = (object) null;
      IEnumerator enumerator = (IEnumerator) null;
      if (multiple)
      {
        if (args != null)
          enumerator = PythonOps.CreatePythonEnumerator(args);
      }
      else
      {
        object[] objArray = new object[1]{ args };
        if (objArray[0] == null)
          objArray[0] = (object) new PythonTuple();
        enumerator = objArray.GetEnumerator();
      }
      int num1;
      if (this.statement != null)
        num1 = this.statement.Reset();
      this.description = (PythonTuple) null;
      this.rowcount = -1;
      this.statement = (Statement) this.connection.__call__(str1);
      if (this.statement.in_use)
        this.statement = new Statement(this.connection, str1);
      this.statement.Reset();
      this.statement.MarkDirty();
      if (!string.IsNullOrEmpty(this.connection.begin_statement))
      {
        switch (this.statement.StatementType)
        {
          case StatementType.Select:
            if (multiple)
              throw PythonSQLite.MakeProgrammingError((object) "You cannot execute SELECT statements in executemany().");
            break;
          case StatementType.Insert:
          case StatementType.Update:
          case StatementType.Delete:
          case StatementType.Replace:
            if (!this.connection.inTransaction)
            {
              this.connection.begin();
              break;
            }
            break;
          case StatementType.Other:
            if (this.connection.inTransaction)
            {
              this.connection.commit();
              break;
            }
            break;
        }
      }
      while (enumerator.MoveNext())
      {
        object current = enumerator.Current;
        this.statement.MarkDirty();
        this.statement.BindParameters(context, current);
        int num2;
        do
        {
          num2 = this.statement.RawStep();
          switch (num2)
          {
            case 100:
            case 101:
              goto label_29;
            default:
              if (this.statement.Reset() == 17)
                continue;
              goto label_28;
          }
        }
        while (this.statement.Recompile(context, current) == 0);
        this.statement.Reset();
        throw PythonSQLite.GetSqliteError(this.connection.db, (Sqlite3.Vdbe) null);
label_28:
        this.statement.Reset();
        throw PythonSQLite.GetSqliteError(this.connection.db, (Sqlite3.Vdbe) null);
label_29:
        if (!this.buildRowCastMap())
          throw PythonSQLite.MakeOperationalError((object) "Error while building row_cast_map");
        if ((num2 == 100 || num2 == 101 && this.statement.StatementType == StatementType.Select) && this.description == null)
        {
          int length = Sqlite3.sqlite3_column_count(this.statement.st);
          object[] o1 = new object[length];
          for (int N = 0; N < length; ++N)
          {
            string str2 = this.buildColumnName(Sqlite3.sqlite3_column_name(this.statement.st, N));
            object[] objArray = new object[7];
            objArray[0] = (object) str2;
            object o2 = (object) objArray;
            o1[N] = (object) new PythonTuple(o2);
          }
          this.description = new PythonTuple((object) o1);
        }
        if (num2 == 100)
        {
          if (multiple)
            throw PythonSQLite.MakeProgrammingError((object) "executemany() can only execute DML statements.");
          this.next_row = this.fetchOneRow(context);
        }
        else if (num2 == 101 && !multiple)
          this.statement.Reset();
        switch (this.statement.StatementType)
        {
          case StatementType.Insert:
          case StatementType.Update:
          case StatementType.Delete:
          case StatementType.Replace:
            if (this.rowcount == -1)
              this.rowcount = 0;
            this.rowcount += Sqlite3.sqlite3_changes(this.connection.db);
            break;
        }
        this.lastrowid = multiple || this.statement.StatementType != StatementType.Insert ? new long?() : new long?(Sqlite3.sqlite3_last_insert_rowid(this.connection.db));
        if (multiple)
          num1 = this.statement.Reset();
      }
      return (object) this;
    }

    private string buildColumnName(string colname)
    {
      int length = colname.IndexOf('[');
      return length >= 0 ? colname.Substring(0, length).Trim() : colname;
    }

    private object fetchOneRow(CodeContext context)
    {
      int length = Sqlite3.sqlite3_data_count(this.statement.st);
      object[] o = new object[length];
      for (int index = 0; index < length; ++index)
      {
        object rowCast = this.connection.detect_types == 0 ? (object) null : this.row_cast_map[index];
        object obj;
        if (rowCast != null)
        {
          byte[] bytes = Sqlite3.sqlite3_column_blob(this.statement.st, index);
          if (bytes == null)
          {
            obj = (object) null;
          }
          else
          {
            string str = PythonSQLite.Latin1.GetString(bytes, 0, bytes.Length);
            obj = PythonCalls.Call(context, rowCast, (object) str);
          }
        }
        else
        {
          switch (Sqlite3.sqlite3_column_type(this.statement.st, index))
          {
            case 1:
              long num = Sqlite3.sqlite3_column_int64(this.statement.st, index);
              obj = num < (long) int.MinValue || num > (long) int.MaxValue ? (object) num : (object) (int) num;
              break;
            case 2:
              obj = (object) Sqlite3.sqlite3_column_double(this.statement.st, index);
              break;
            case 3:
              obj = (object) Sqlite3.sqlite3_column_text(this.statement.st, index);
              break;
            case 5:
              obj = (object) null;
              break;
            default:
              byte[] numArray = Sqlite3.sqlite3_column_blob(this.statement.st, index) ?? new byte[0];
              obj = (object) new PythonBuffer(context, (object) numArray);
              break;
          }
        }
        o[index] = obj;
      }
      return (object) new PythonTuple((object) o);
    }

    public PythonSQLite.Cursor executescript(string operation)
    {
      this.connection.checkThread();
      this.connection.checkConnection();
      this.connection.commit();
      Sqlite3.Vdbe ppStmt = (Sqlite3.Vdbe) null;
      string pzTail = operation;
      bool flag = false;
      while (Sqlite3.sqlite3_complete(operation) != 0)
      {
        flag = true;
        if (Sqlite3.sqlite3_prepare(this.connection.db, operation, -1, ref ppStmt, ref pzTail) != 0)
          throw PythonSQLite.GetSqliteError(this.connection.db, (Sqlite3.Vdbe) null);
        int num = 100;
        while (true)
        {
          switch (num)
          {
            case 100:
              num = Sqlite3.sqlite3_step(ppStmt);
              continue;
            case 101:
              goto label_8;
            default:
              goto label_7;
          }
        }
label_7:
        Sqlite3.sqlite3_finalize(ppStmt);
        throw PythonSQLite.GetSqliteError(this.connection.db, (Sqlite3.Vdbe) null);
label_8:
        if (Sqlite3.sqlite3_finalize(ppStmt) != 0)
          throw PythonSQLite.GetSqliteError(this.connection.db, (Sqlite3.Vdbe) null);
      }
      if (!flag)
        throw PythonSQLite.MakeProgrammingError((object) "you did not provide a complete SQL statement");
      return this;
    }

    public object __iter__() => (object) this;

    public object next(CodeContext context)
    {
      this.connection.checkThread();
      this.connection.checkConnection();
      if (this.next_row == null)
      {
        if (this.statement != null)
        {
          this.statement.Reset();
          this.statement = (Statement) null;
        }
        throw new StopIterationException();
      }
      object nextRow = this.next_row;
      this.next_row = (object) null;
      object obj = this.row_factory == null ? nextRow : PythonCalls.Call(context, this.row_factory, (object) this, nextRow);
      if (this.statement != null)
      {
        int num = this.statement.RawStep();
        switch (num)
        {
          case 100:
          case 101:
            if (num == 100)
            {
              this.next_row = this.fetchOneRow(context);
              break;
            }
            break;
          default:
            this.statement.Reset();
            throw PythonSQLite.GetSqliteError(this.connection.db, this.statement.st);
        }
      }
      return obj;
    }

    [Documentation("Fetches one row from the resultset.")]
    public object fetchone(CodeContext context)
    {
      try
      {
        return this.next(context);
      }
      catch (StopIterationException ex)
      {
        return (object) null;
      }
    }

    public object fetchmany(CodeContext context) => this.fetchmany(context, this.arraysize);

    [Documentation("Fetches several rows from the resultset.")]
    public object fetchmany(CodeContext context, int size)
    {
      List list = new List();
      object obj = this.fetchone(context);
      for (int index = 0; index < size && obj != null; obj = this.fetchone(context))
      {
        list.Add(obj);
        ++index;
      }
      return (object) list;
    }

    [Documentation("Fetches all rows from the resultset.")]
    public object fetchall(CodeContext context)
    {
      List list = new List();
      for (object obj = this.fetchone(context); obj != null; obj = this.fetchone(context))
        list.Add(obj);
      return (object) list;
    }

    public object nextset() => (object) null;

    [Documentation("Required by DB-API. Does nothing in IronPython.Sqlite3.")]
    public void setinputsizes(object sizes)
    {
    }

    [Documentation("Required by DB-API. Does nothing in IronPython.Sqlite3.")]
    public void setoutputsize(params object[] args)
    {
    }

    private bool buildRowCastMap()
    {
      if (this.connection.detect_types == 0)
        return true;
      this.row_cast_map = new List<object>();
      for (int N = 0; N < Sqlite3.sqlite3_column_count(this.statement.st); ++N)
      {
        object obj = (object) null;
        if ((this.connection.detect_types & 2) != 0)
        {
          string input = Sqlite3.sqlite3_column_name(this.statement.st, N);
          if (input != null)
          {
            Match match = new Regex("\\[(\\w+)\\]").Match(input);
            if (match.Success)
              obj = this.getConverter(match.Groups[1].ToString());
          }
        }
        if (obj == null && (this.connection.detect_types & 1) != 0)
        {
          string input = Sqlite3.sqlite3_column_decltype(this.statement.st, N);
          if (input != null)
          {
            Match match = new Regex("\\b(\\w+)\\b").Match(input);
            if (match.Success)
              obj = this.getConverter(match.Groups[1].ToString());
          }
        }
        this.row_cast_map.Add(obj);
      }
      return true;
    }

    private object getConverter(string key)
    {
      object obj;
      return !PythonSQLite.converters.TryGetValue((object) key.ToUpperInvariant(), out obj) ? (object) null : obj;
    }

    public IEnumerator GetEnumerator()
    {
      List list = new List();
      try
      {
        while (true)
          list.append(this.next(this.context));
      }
      catch (StopIterationException ex)
      {
      }
      return list.GetEnumerator();
    }
  }

  [PythonType]
  public class Row : IEnumerable
  {
    private PythonTuple data;
    private PythonTuple description;

    public Row(PythonSQLite.Cursor cursor, PythonTuple data)
    {
      this.data = data;
      this.description = cursor.description;
    }

    public override bool Equals(object obj)
    {
      if (!(obj is PythonSQLite.Row row))
        return false;
      if (this == row)
        return true;
      return this.description.Equals((object) row.description) && this.data.Equals((object) row.data);
    }

    public override int GetHashCode() => this.description.GetHashCode() ^ this.data.GetHashCode();

    public object __iter__() => (object) this.data;

    public object this[long i] => this.data[(BigInteger) i];

    public object this[string s]
    {
      get
      {
        for (int index = 0; index < this.data.Count; ++index)
        {
          PythonTuple pythonTuple = (PythonTuple) this.description[index];
          if (CultureInfo.InvariantCulture.CompareInfo.Compare(s, (string) pythonTuple[0], CompareOptions.IgnoreCase) == 0)
            return this.data[index];
        }
        throw PythonSQLite.CreateThrowable(PythonExceptions.IndexError, (object) "No item with that key");
      }
    }

    public List keys()
    {
      List list = new List();
      for (int index = 0; index < this.data.Count; ++index)
        list.append(((PythonTuple) this.description[index])[0]);
      return list;
    }

    public IEnumerator GetEnumerator() => this.data.GetEnumerator();
  }

  public class PrepareProtocol
  {
  }
}
