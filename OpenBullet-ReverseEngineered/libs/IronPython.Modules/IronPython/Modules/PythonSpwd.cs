// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonSpwd
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace IronPython.Modules;

public static class PythonSpwd
{
  public const string __doc__ = "This module provides access to the Unix shadow password database.\r\nIt is available on various Unix versions.\r\n\r\nShadow password database entries are reported as 9-tuples of type struct_spwd,\r\ncontaining the following items from the password database (see `<shadow.h>'):\r\nsp_namp, sp_pwdp, sp_lstchg, sp_min, sp_max, sp_warn, sp_inact, sp_expire, sp_flag.\r\nThe sp_namp and sp_pwdp are strings, the rest are integers.\r\nAn exception is raised if the entry asked for cannot be found.\r\nYou have to be root to be able to use this module.";

  private static PythonSpwd.struct_spwd Make(IntPtr pwd)
  {
    PythonSpwd.spwd structure = (PythonSpwd.spwd) Marshal.PtrToStructure(pwd, typeof (PythonSpwd.spwd));
    return new PythonSpwd.struct_spwd(structure.sp_namp, structure.sp_pwdp, structure.sp_lstchg, structure.sp_min, structure.sp_max, structure.sp_warn, structure.sp_inact, structure.sp_expire, structure.sp_flag);
  }

  [Documentation("Return the shadow password database entry for the given user name.")]
  public static PythonSpwd.struct_spwd getspnam(string name)
  {
    IntPtr pwd = PythonSpwd._getspnam(name);
    return !(pwd == IntPtr.Zero) ? PythonSpwd.Make(pwd) : throw PythonOps.KeyError("getspnam(): name not found");
  }

  [Documentation("Return a list of all available shadow password database entries, in arbitrary order.")]
  public static IronPython.Runtime.List getspall()
  {
    IronPython.Runtime.List list = new IronPython.Runtime.List();
    PythonSpwd.setspent();
    for (IntPtr pwd = PythonSpwd.getspent(); pwd != IntPtr.Zero; pwd = PythonSpwd.getspent())
      list.Add((object) PythonSpwd.Make(pwd));
    return list;
  }

  [DllImport("libc", EntryPoint = "getspnam", CallingConvention = CallingConvention.Cdecl)]
  private static extern IntPtr _getspnam([MarshalAs(UnmanagedType.LPStr)] string name);

  [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
  private static extern void setspent();

  [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
  private static extern IntPtr getspent();

  private struct spwd
  {
    [MarshalAs(UnmanagedType.LPStr)]
    public string sp_namp;
    [MarshalAs(UnmanagedType.LPStr)]
    public string sp_pwdp;
    public int sp_lstchg;
    public int sp_min;
    public int sp_max;
    public int sp_warn;
    public int sp_inact;
    public int sp_expire;
    public int sp_flag;
  }

  [PythonType("struct_spwd")]
  [Documentation("spwd.struct_spwd: Results from getsp*() routines.\r\n\r\nThis object may be accessed either as a 9-tuple of\r\n  (sp_namp,sp_pwdp,sp_lstchg,sp_min,sp_max,sp_warn,sp_inact,sp_expire,sp_flag)\r\nor via the object attributes as named in the above tuple.")]
  public class struct_spwd : PythonTuple
  {
    private const int LENGTH = 9;

    internal struct_spwd(
      string sp_nam,
      string sp_pwd,
      int sp_lstchg,
      int sp_min,
      int sp_max,
      int sp_warn,
      int sp_inact,
      int sp_expire,
      int sp_flag)
      : base(new object[9]
      {
        (object) sp_nam,
        (object) sp_pwd,
        (object) sp_lstchg,
        (object) sp_min,
        (object) sp_max,
        (object) sp_warn,
        (object) sp_inact,
        (object) sp_expire,
        (object) sp_flag
      })
    {
    }

    [Documentation("login name")]
    public string sp_nam => (string) this._data[0];

    [Documentation("encrypted password")]
    public string sp_pwd => (string) this._data[1];

    [Documentation("date of last change")]
    public int sp_lstchg => (int) this._data[2];

    [Documentation("min #days between changes")]
    public int sp_min => (int) this._data[3];

    [Documentation("max #days between changes")]
    public int sp_max => (int) this._data[4];

    [Documentation("#days before pw expires to warn user about it")]
    public int sp_warn => (int) this._data[5];

    [Documentation("#days after pw expires until account is disabled")]
    public int sp_inact => (int) this._data[6];

    [Documentation("#days since 1970-01-01 when account expires")]
    public int sp_expire => (int) this._data[7];

    [Documentation("reserved")]
    public int sp_flag => (int) this._data[8];

    public override string __repr__(CodeContext context)
    {
      return $"spwd.struct_spwd(sp_name='{this.sp_nam}', sp_pwd='{this.sp_pwd}', sp_lstchg={this.sp_lstchg}, sp_min={this.sp_min}, sp_max={this.sp_max}, sp_warn={this.sp_warn}, sp_inact={this.sp_inact}, sp_expire={this.sp_expire}, sp_flag={this.sp_flag})";
    }
  }
}
