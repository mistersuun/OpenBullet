// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonPwd
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

#nullable disable
namespace IronPython.Modules;

public static class PythonPwd
{
  public const string __doc__ = "This module provides access to the Unix password database.\r\nIt is available on all Unix versions.\r\n\r\nPassword database entries are reported as 7-tuples containing the following\r\nitems from the password database (see `<pwd.h>'), in order:\r\npw_name, pw_passwd, pw_uid, pw_gid, pw_gecos, pw_dir, pw_shell.\r\nThe uid and gid items are integers, all others are strings. An\r\nexception is raised if the entry asked for cannot be found.";

  private static PythonPwd.struct_passwd Make(IntPtr pwd)
  {
    PythonPwd.struct_passwd structPasswd;
    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
      PythonPwd.passwd_osx structure = (PythonPwd.passwd_osx) Marshal.PtrToStructure(pwd, typeof (PythonPwd.passwd_osx));
      structPasswd = new PythonPwd.struct_passwd(structure.pw_name, structure.pw_passwd, structure.pw_uid, structure.pw_gid, structure.pw_gecos, structure.pw_dir, structure.pw_shell);
    }
    else
    {
      PythonPwd.passwd_linux structure = (PythonPwd.passwd_linux) Marshal.PtrToStructure(pwd, typeof (PythonPwd.passwd_linux));
      structPasswd = new PythonPwd.struct_passwd(structure.pw_name, structure.pw_passwd, structure.pw_uid, structure.pw_gid, structure.pw_gecos, structure.pw_dir, structure.pw_shell);
    }
    return structPasswd;
  }

  [Documentation("Return the password database entry for the given numeric user ID.")]
  public static PythonPwd.struct_passwd getpwuid(object uid)
  {
    switch (uid)
    {
      case int uid1:
        IntPtr pwd = PythonPwd._getpwuid(uid1);
        return !(pwd == IntPtr.Zero) ? PythonPwd.Make(pwd) : throw PythonOps.KeyError($"getpwuid(): uid not found: {uid1}");
      case long _:
      case BigInteger _:
        throw PythonOps.KeyError("getpwuid(): uid not found");
      default:
        throw PythonOps.TypeError("integer argument expected, got " + PythonOps.GetPythonTypeName(uid));
    }
  }

  [Documentation("Return the password database entry for the given user name.")]
  public static PythonPwd.struct_passwd getpwnam(string name)
  {
    IntPtr pwd = PythonPwd._getpwnam(name);
    return !(pwd == IntPtr.Zero) ? PythonPwd.Make(pwd) : throw PythonOps.KeyError("getpwname(): name not found: " + name);
  }

  [Documentation("Return a list of all available password database entries, in arbitrary order.")]
  public static List getpwall()
  {
    List list = new List();
    PythonPwd.setpwent();
    for (IntPtr pwd = PythonPwd.getpwent(); pwd != IntPtr.Zero; pwd = PythonPwd.getpwent())
      list.Add((object) PythonPwd.Make(pwd));
    return list;
  }

  [DllImport("libc", EntryPoint = "getpwuid", CallingConvention = CallingConvention.Cdecl)]
  private static extern IntPtr _getpwuid(int uid);

  [DllImport("libc", EntryPoint = "getpwnam", CallingConvention = CallingConvention.Cdecl)]
  private static extern IntPtr _getpwnam([MarshalAs(UnmanagedType.LPStr)] string name);

  [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
  private static extern void setpwent();

  [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
  private static extern IntPtr getpwent();

  private struct passwd_linux
  {
    [MarshalAs(UnmanagedType.LPStr)]
    public string pw_name;
    [MarshalAs(UnmanagedType.LPStr)]
    public string pw_passwd;
    public int pw_uid;
    public int pw_gid;
    [MarshalAs(UnmanagedType.LPStr)]
    public string pw_gecos;
    [MarshalAs(UnmanagedType.LPStr)]
    public string pw_dir;
    [MarshalAs(UnmanagedType.LPStr)]
    public string pw_shell;
  }

  private struct passwd_osx
  {
    [MarshalAs(UnmanagedType.LPStr)]
    public string pw_name;
    [MarshalAs(UnmanagedType.LPStr)]
    public string pw_passwd;
    public int pw_uid;
    public int pw_gid;
    public ulong pw_change;
    [MarshalAs(UnmanagedType.LPStr)]
    public string pw_class;
    [MarshalAs(UnmanagedType.LPStr)]
    public string pw_gecos;
    [MarshalAs(UnmanagedType.LPStr)]
    public string pw_dir;
    [MarshalAs(UnmanagedType.LPStr)]
    public string pw_shell;
    public ulong pw_expire;
  }

  [PythonType("struct_passwd")]
  [Documentation("pwd.struct_passwd: Results from getpw*() routines.\r\n\r\nThis object may be accessed either as a tuple of\r\n  (pw_name,pw_passwd,pw_uid,pw_gid,pw_gecos,pw_dir,pw_shell)\r\nor via the object attributes as named in the above tuple.")]
  public class struct_passwd : PythonTuple
  {
    internal struct_passwd(
      string pw_name,
      string pw_passwd,
      int pw_uid,
      int pw_gid,
      string pw_gecos,
      string pw_dir,
      string pw_shell)
      : base(new object[7]
      {
        (object) pw_name,
        (object) pw_passwd,
        (object) pw_uid,
        (object) pw_gid,
        (object) pw_gecos,
        (object) pw_dir,
        (object) pw_shell
      })
    {
    }

    [Documentation("user name")]
    public string pw_name => (string) this._data[0];

    [Documentation("password")]
    public string pw_passwd => (string) this._data[1];

    [Documentation("user id")]
    public int pw_uid => (int) this._data[2];

    [Documentation("group id")]
    public int pw_gid => (int) this._data[3];

    [Documentation("real name")]
    public string pw_gecos => (string) this._data[4];

    [Documentation("home directory")]
    public string pw_dir => (string) this._data[5];

    [Documentation("shell program")]
    public string pw_shell => (string) this._data[6];

    public override string __repr__(CodeContext context)
    {
      return $"pwd.struct_passwd(pw_name='{this.pw_name}', pw_passwd='{this.pw_passwd}', pw_uid={this.pw_uid}, pw_gid={this.pw_gid}, pw_gecos='{this.pw_gecos}', pw_dir='{this.pw_dir}', pw_shell='{this.pw_shell}')";
    }
  }
}
