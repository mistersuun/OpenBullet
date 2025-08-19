// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonGrp
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#nullable disable
namespace IronPython.Modules;

public static class PythonGrp
{
  public const string __doc__ = "Access to the Unix group database.\r\n        \r\nGroup entries are reported as 4-tuples containing the following fields\r\nfrom the group database, in order:\r\n\r\n  gr_name   - name of the group\r\n  gr_passwd - group password (encrypted); often empty\r\n  gr_gid    - numeric ID of the group\r\n  gr_mem    - list of members\r\n  \r\nThe gid is an integer, name and password are strings.  (Note that most\r\nusers are not explicitly listed as members of the groups they are in\r\naccording to the password database.  Check both databases to get\r\ncomplete membership information.)";

  private static PythonGrp.struct_group Make(IntPtr pwd)
  {
    PythonGrp.group structure = (PythonGrp.group) Marshal.PtrToStructure(pwd, typeof (PythonGrp.group));
    return new PythonGrp.struct_group(structure.gr_name, structure.gr_passwd, structure.gr_gid, new IronPython.Runtime.List((object) PythonGrp.MarshalStringArray(structure.gr_mem)));
  }

  private static IEnumerable<string> MarshalStringArray(IntPtr arrayPtr)
  {
    if (arrayPtr != IntPtr.Zero)
    {
      for (IntPtr ptr = Marshal.ReadIntPtr(arrayPtr); ptr != IntPtr.Zero; ptr = Marshal.ReadIntPtr(arrayPtr))
      {
        yield return Marshal.PtrToStringAnsi(ptr);
        arrayPtr = new IntPtr(arrayPtr.ToInt64() + (long) IntPtr.Size);
      }
    }
  }

  public static PythonGrp.struct_group getgrgid(int gid)
  {
    IntPtr pwd = PythonGrp._getgrgid(gid);
    return !(pwd == IntPtr.Zero) ? PythonGrp.Make(pwd) : throw PythonOps.KeyError($"getgrgid(): gid not found: {gid}");
  }

  public static PythonGrp.struct_group getgrnam(string name)
  {
    IntPtr pwd = PythonGrp._getgrnam(name);
    return !(pwd == IntPtr.Zero) ? PythonGrp.Make(pwd) : throw PythonOps.KeyError("getgrnam()): name not found: " + name);
  }

  public static IronPython.Runtime.List getgrall()
  {
    IronPython.Runtime.List list = new IronPython.Runtime.List();
    PythonGrp.setgrent();
    for (IntPtr pwd = PythonGrp.getgrent(); pwd != IntPtr.Zero; pwd = PythonGrp.getgrent())
      list.Add((object) PythonGrp.Make(pwd));
    return list;
  }

  [DllImport("libc", EntryPoint = "getgrgid", CallingConvention = CallingConvention.Cdecl)]
  private static extern IntPtr _getgrgid(int uid);

  [DllImport("libc", EntryPoint = "getgrnam", CallingConvention = CallingConvention.Cdecl)]
  private static extern IntPtr _getgrnam([MarshalAs(UnmanagedType.LPStr)] string name);

  [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
  private static extern void setgrent();

  [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
  private static extern IntPtr getgrent();

  private struct group
  {
    [MarshalAs(UnmanagedType.LPStr)]
    public string gr_name;
    [MarshalAs(UnmanagedType.LPStr)]
    public string gr_passwd;
    public int gr_gid;
    public IntPtr gr_mem;
  }

  [PythonType("struct_group")]
  [Documentation("grp.struct_group: Results from getgr*() routines.\r\n\r\nThis object may be accessed either as a tuple of\r\n  (gr_name,gr_passwd,gr_gid,gr_mem)\r\nor via the object attributes as named in the above tuple.\r\n")]
  public class struct_group : PythonTuple
  {
    internal struct_group(string gr_name, string gr_passwd, int gr_gid, IronPython.Runtime.List gr_mem)
      : base(new object[4]
      {
        (object) gr_name,
        (object) gr_passwd,
        (object) gr_gid,
        (object) gr_mem
      })
    {
    }

    [Documentation("group name")]
    public string gr_name => (string) this._data[0];

    [Documentation("password")]
    public string gr_passwd => (string) this._data[1];

    [Documentation("group id")]
    public int gr_gid => (int) this._data[2];

    [Documentation("group members")]
    public IronPython.Runtime.List gr_mem => (IronPython.Runtime.List) this._data[3];

    public override string __repr__(CodeContext context)
    {
      return $"grp.struct_group(gr_name='{this.gr_name}', gr_passwd='{this.gr_passwd}', gr_gid={this.gr_gid}, gr_mem={this.gr_mem.__repr__(context)})";
    }
  }
}
