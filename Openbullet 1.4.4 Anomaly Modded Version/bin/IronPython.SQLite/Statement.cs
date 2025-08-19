// Decompiled with JetBrains decompiler
// Type: IronPython.SQLite.Statement
// Assembly: IronPython.SQLite, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7222B477-FDAF-4AA1-A0E3-CD8AE6ED7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.SQLite.dll

using Community.CsharpSqlite;
using IronPython.Runtime;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;

#nullable disable
namespace IronPython.SQLite;

[DebuggerDisplay("{sql}")]
internal class Statement
{
  private readonly Guid uniqueid;
  private Sqlite3.sqlite3 db;
  internal Sqlite3.Vdbe st;
  private object current;
  private object nextRow;
  private bool started;
  private string sql;
  private bool bound;
  internal bool in_use;
  private StatementType _type;

  public string Tail { get; private set; }

  public Statement(PythonSQLite.Connection connection, string operation)
  {
    this.uniqueid = Guid.NewGuid();
    this.db = connection.db;
    this.sql = operation;
    this.st = (Sqlite3.Vdbe) null;
    string pzTail = (string) null;
    if (Sqlite3.sqlite3_prepare(this.db, this.sql, -1, ref this.st, ref pzTail) != 0)
    {
      Sqlite3.sqlite3_finalize(this.st);
      this.st = (Sqlite3.Vdbe) null;
      throw PythonSQLite.GetSqliteError(this.db, (Sqlite3.Vdbe) null);
    }
    this.Tail = pzTail;
  }

  private Statement(Sqlite3.sqlite3 db, Sqlite3.Vdbe stmt, string operation, string tail)
  {
    this.uniqueid = Guid.NewGuid();
    this.db = db;
    this.sql = operation;
    this.st = stmt;
    this.Tail = tail;
  }

  ~Statement()
  {
    if (this.st != null)
      Sqlite3.sqlite3_finalize(this.st);
    this.st = (Sqlite3.Vdbe) null;
  }

  public StatementType StatementType
  {
    get
    {
      if (this._type != StatementType.Unknown)
        return this._type;
      string source = this.sql.TrimStart();
      this._type = !CultureInfo.InvariantCulture.CompareInfo.IsPrefix(source, "select", CompareOptions.IgnoreCase) ? (!CultureInfo.InvariantCulture.CompareInfo.IsPrefix(source, "insert", CompareOptions.IgnoreCase) ? (!CultureInfo.InvariantCulture.CompareInfo.IsPrefix(source, "update", CompareOptions.IgnoreCase) ? (!CultureInfo.InvariantCulture.CompareInfo.IsPrefix(source, "delete", CompareOptions.IgnoreCase) ? (!CultureInfo.InvariantCulture.CompareInfo.IsPrefix(source, "replace", CompareOptions.IgnoreCase) ? StatementType.Other : StatementType.Replace) : StatementType.Delete) : StatementType.Update) : StatementType.Insert) : StatementType.Select;
      return this._type;
    }
  }

  public void BindParameters(CodeContext context, object parameters)
  {
    if (this.bound)
      this.ClearParameters();
    int num_params_needed = Sqlite3.sqlite3_bind_parameter_count(this.st);
    switch (parameters)
    {
      case null:
        if (num_params_needed <= 0)
          return;
        throw PythonSQLite.MakeProgrammingError((object) "parameters are required but not specified.");
      case IDictionary _:
        this.BindParameters(context, (IDictionary) parameters, num_params_needed);
        break;
      case IList _:
        this.BindParameters(context, (IList) parameters, num_params_needed);
        break;
      default:
        throw PythonSQLite.MakeProgrammingError((object) "unknown parameter type");
    }
    this.bound = true;
  }

  private void BindParameters(CodeContext context, IDictionary args, int num_params_needed)
  {
    for (int index = 1; index <= num_params_needed; ++index)
    {
      string str = Sqlite3.sqlite3_bind_parameter_name(this.st, index);
      string key = !string.IsNullOrEmpty(str) ? str.Substring(1) : throw PythonSQLite.MakeProgrammingError((object) "Binding {0} has no name, but you supplied a dictionary (which has only names).".Format((object) index));
      if (args.Contains((object) key))
        this.BindParameter(context, index, this.maybeAdapt(context, args[(object) key]));
      else
        throw PythonSQLite.MakeProgrammingError((object) "You did not supply a value for binding {0}.".Format((object) index));
    }
  }

  private void BindParameters(CodeContext context, IList args, int num_params_needed)
  {
    if (num_params_needed != args.Count)
      throw PythonSQLite.MakeProgrammingError((object) "Incorrect number of bindings supplied.");
    for (int index = 0; index < args.Count; ++index)
      this.BindParameter(context, index + 1, this.maybeAdapt(context, args[index]));
  }

  private void BindParameter(CodeContext context, int index, object arg)
  {
    int num;
    switch (arg)
    {
      case null:
        num = Sqlite3.sqlite3_bind_null(this.st, index);
        break;
      case int iValue1:
        num = Sqlite3.sqlite3_bind_int(this.st, index, iValue1);
        break;
      case bool flag:
        num = Sqlite3.sqlite3_bind_int(this.st, index, flag ? 1 : 0);
        break;
      case long iValue2:
        num = Sqlite3.sqlite3_bind_int64(this.st, index, iValue2);
        break;
      case BigInteger iValue3:
        num = Sqlite3.sqlite3_bind_int64(this.st, index, (long) iValue3);
        break;
      case float rValue1:
        num = Sqlite3.sqlite3_bind_double(this.st, index, (double) rValue1);
        break;
      case double rValue2:
        num = Sqlite3.sqlite3_bind_double(this.st, index, rValue2);
        break;
      case string _:
        num = Sqlite3.sqlite3_bind_text(this.st, index, (string) arg, -1, Sqlite3.SQLITE_TRANSIENT);
        break;
      case byte[] _:
        num = Sqlite3.sqlite3_bind_blob(this.st, index, (byte[]) arg, -1, Sqlite3.SQLITE_TRANSIENT);
        break;
      case PythonBuffer _:
        string s = ((PythonBuffer) arg).__getslice__((object) 0, (object) null).ToString();
        byte[] bytes = PythonSQLite.Latin1.GetBytes(s);
        num = Sqlite3.sqlite3_bind_blob(this.st, index, bytes, -1, Sqlite3.SQLITE_TRANSIENT);
        break;
      default:
        throw PythonSQLite.MakeInterfaceError((object) "Unable to bind parameter {0} - unsupported type {1}".Format((object) index, (object) arg.GetType()));
    }
    if (num != 0)
      throw PythonSQLite.MakeInterfaceError((object) "Unable to bind parameter {0}: {1}".Format((object) index, (object) Sqlite3.sqlite3_errmsg(this.db)));
  }

  private object maybeAdapt(CodeContext context, object value)
  {
    return !this.needsAdaptation(context, value) ? value : this.adaptValue(context, value);
  }

  private bool needsAdaptation(CodeContext context, object value)
  {
    switch (value)
    {
      case null:
      case int _:
      case bool _:
      case long _:
      case BigInteger _:
      case float _:
      case double _:
      case string _:
      case byte[] _:
      case PythonBuffer _:
        object pythonTypeFromType = (object) DynamicHelpers.GetPythonTypeFromType(typeof (PythonSQLite.PrepareProtocol));
        object key = (object) new PythonTuple((object) new object[2]
        {
          (object) DynamicHelpers.GetPythonType(value),
          pythonTypeFromType
        });
        return PythonSQLite.adapters.ContainsKey(key);
      default:
        return true;
    }
  }

  private object adaptValue(CodeContext context, object value)
  {
    object pythonTypeFromType = (object) DynamicHelpers.GetPythonTypeFromType(typeof (PythonSQLite.PrepareProtocol));
    object key = (object) new PythonTuple((object) new object[2]
    {
      (object) DynamicHelpers.GetPythonType(value),
      pythonTypeFromType
    });
    object func1;
    if (PythonSQLite.adapters.TryGetValue(key, out func1))
      return PythonCalls.Call(context, func1, value);
    object func2;
    if (context.LanguageContext.Operations.TryGetMember(value, "__conform__", out func2))
    {
      object obj = PythonCalls.Call(context, func2, pythonTypeFromType);
      if (obj != null)
        return obj;
    }
    return value;
  }

  public int RawStep() => Util.Step(this.st);

  public int SqliteFinalize()
  {
    int num = 0;
    if (this.st != null)
    {
      num = Sqlite3.sqlite3_finalize(this.st);
      this.st = (Sqlite3.Vdbe) null;
    }
    this.in_use = false;
    return num;
  }

  public int Reset()
  {
    int num = 0;
    if (this.in_use && this.st != null)
    {
      num = Sqlite3.sqlite3_reset(this.st);
      if (num == 0)
        this.in_use = false;
    }
    return num;
  }

  private void ClearParameters()
  {
    if (Sqlite3.sqlite3_clear_bindings(this.st) != 0)
      throw PythonSQLite.GetSqliteError(this.db, (Sqlite3.Vdbe) null);
  }

  internal void MarkDirty() => this.in_use = true;

  internal int Recompile(CodeContext context, object parameters)
  {
    Sqlite3.Vdbe ppStmt = (Sqlite3.Vdbe) null;
    string pzTail = (string) null;
    int num = Sqlite3.sqlite3_prepare(this.db, this.sql, -1, ref ppStmt, ref pzTail);
    if (num != 0)
      return num;
    new Statement(this.st.db, ppStmt, this.sql, pzTail).BindParameters(context, parameters);
    Sqlite3.sqlite3_finalize(this.st);
    this.st = ppStmt;
    return num;
  }
}
