// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonWinReg
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Text;

#nullable disable
namespace IronPython.Modules;

public static class PythonWinReg
{
  public const string __doc__ = "Provides access to the Windows registry.";
  public static PythonType error = PythonExceptions.WindowsError;
  public static BigInteger HKEY_CLASSES_ROOT = (BigInteger) 2147483648L /*0x80000000*/;
  public static BigInteger HKEY_CURRENT_USER = (BigInteger) 2147483649L /*0x80000001*/;
  public static BigInteger HKEY_LOCAL_MACHINE = (BigInteger) 2147483650L /*0x80000002*/;
  public static BigInteger HKEY_USERS = (BigInteger) 2147483651L /*0x80000003*/;
  public static BigInteger HKEY_PERFORMANCE_DATA = (BigInteger) 2147483652L /*0x80000004*/;
  public static BigInteger HKEY_CURRENT_CONFIG = (BigInteger) 2147483653L /*0x80000005*/;
  public static BigInteger HKEY_DYN_DATA = (BigInteger) 2147483654L /*0x80000006*/;
  public const int KEY_QUERY_VALUE = 1;
  public const int KEY_SET_VALUE = 2;
  public const int KEY_CREATE_SUB_KEY = 4;
  public const int KEY_ENUMERATE_SUB_KEYS = 8;
  public const int KEY_NOTIFY = 16 /*0x10*/;
  public const int KEY_CREATE_LINK = 32 /*0x20*/;
  public const int KEY_ALL_ACCESS = 983103;
  public const int KEY_EXECUTE = 131097;
  public const int KEY_READ = 131097;
  public const int KEY_WRITE = 131078 /*0x020006*/;
  public const int REG_CREATED_NEW_KEY = 1;
  public const int REG_OPENED_EXISTING_KEY = 2;
  public const int REG_NONE = 0;
  public const int REG_SZ = 1;
  public const int REG_EXPAND_SZ = 2;
  public const int REG_BINARY = 3;
  public const int REG_DWORD = 4;
  public const int REG_DWORD_LITTLE_ENDIAN = 4;
  public const int REG_DWORD_BIG_ENDIAN = 5;
  public const int REG_LINK = 6;
  public const int REG_MULTI_SZ = 7;
  public const int REG_RESOURCE_LIST = 8;
  public const int REG_FULL_RESOURCE_DESCRIPTOR = 9;
  public const int REG_RESOURCE_REQUIREMENTS_LIST = 10;
  public const int REG_NOTIFY_CHANGE_NAME = 1;
  public const int REG_NOTIFY_CHANGE_ATTRIBUTES = 2;
  public const int REG_NOTIFY_CHANGE_LAST_SET = 4;
  public const int REG_NOTIFY_CHANGE_SECURITY = 8;
  public const int REG_OPTION_RESERVED = 0;
  public const int REG_OPTION_NON_VOLATILE = 0;
  public const int REG_OPTION_VOLATILE = 1;
  public const int REG_OPTION_CREATE_LINK = 2;
  public const int REG_OPTION_BACKUP_RESTORE = 4;
  public const int REG_OPTION_OPEN_LINK = 8;
  public const int REG_NO_LAZY_FLUSH = 4;
  public const int REG_REFRESH_HIVE = 2;
  public const int REG_LEGAL_CHANGE_FILTER = 15;
  public const int REG_LEGAL_OPTION = 15;
  public const int REG_WHOLE_HIVE_VOLATILE = 1;
  private const int ERROR_NO_MORE_ITEMS = 259;
  private const int ERROR_MORE_DATA = 234;
  private const int ERROR_SUCCESS = 0;

  public static void CloseKey(PythonWinReg.HKEYType key) => key.Close();

  public static PythonWinReg.HKEYType CreateKey(object key, string subKeyName)
  {
    if (subKeyName.Length == 256 /*0x0100*/)
      return PythonWinReg.CreateKeyEx(key, subKeyName, 0, 983103);
    PythonWinReg.HKEYType rootKey = PythonWinReg.GetRootKey(key);
    return key is BigInteger && string.IsNullOrEmpty(subKeyName) ? rootKey : new PythonWinReg.HKEYType(rootKey.GetKey().CreateSubKey(subKeyName));
  }

  private static string FormatError(int errorCode) => new Win32Exception(errorCode).Message;

  public static PythonWinReg.HKEYType CreateKeyEx(object key, string subKeyName, int res = 0, int sam = 983103)
  {
    PythonWinReg.HKEYType rootKey = PythonWinReg.GetRootKey(key);
    if (key is BigInteger && string.IsNullOrEmpty(subKeyName))
      return rootKey;
    SafeRegistryHandle phkResult;
    int keyEx = PythonWinReg.RegCreateKeyEx(rootKey.GetKey().Handle, subKeyName, 0, (string) null, RegistryOptions.None, (RegistryRights) sam, IntPtr.Zero, out phkResult, out int _);
    if (keyEx != 0)
      throw PythonExceptions.CreateThrowable(PythonWinReg.error, (object) keyEx, (object) PythonWinReg.FormatError(keyEx));
    return new PythonWinReg.HKEYType(RegistryKey.FromHandle(phkResult));
  }

  [DllImport("advapi32.dll", SetLastError = true)]
  private static extern int RegCreateKeyEx(
    SafeRegistryHandle hKey,
    string lpSubKey,
    int Reserved,
    string lpClass,
    RegistryOptions dwOptions,
    RegistryRights samDesired,
    IntPtr lpSecurityAttributes,
    out SafeRegistryHandle phkResult,
    out int lpdwDisposition);

  [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
  private static extern int RegQueryValueEx(
    SafeRegistryHandle hKey,
    string lpValueName,
    IntPtr lpReserved,
    out int lpType,
    byte[] lpData,
    ref uint lpcbData);

  [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
  private static extern int RegEnumKeyEx(
    SafeRegistryHandle hKey,
    int dwIndex,
    StringBuilder lpName,
    ref int lpcbName,
    IntPtr lpReserved,
    IntPtr lpClass,
    IntPtr lpcbClass,
    IntPtr lpftLastWriteTime);

  [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
  private static extern int RegSetValueEx(
    SafeRegistryHandle hKey,
    string lpValueName,
    int Reserved,
    int dwType,
    byte[] lpData,
    int cbData);

  [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
  internal static extern int RegDeleteKey(SafeRegistryHandle hKey, string lpSubKey);

  [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
  internal static extern int RegDisableReflectionKey(SafeRegistryHandle hKey);

  [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
  internal static extern int RegEnableReflectionKey(SafeRegistryHandle hKey);

  [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
  internal static extern int RegQueryReflectionKey(
    SafeRegistryHandle hBase,
    out bool bIsReflectionDisabled);

  public static void DeleteKey(object key, string subKeyName)
  {
    PythonWinReg.HKEYType rootKey = PythonWinReg.GetRootKey(key);
    if (key is BigInteger && string.IsNullOrEmpty(subKeyName))
      throw new InvalidCastException("DeleteKey() argument 2 must be string, not None");
    if (subKeyName.Length == 256 /*0x0100*/)
    {
      PythonWinReg.RegDeleteKey(rootKey.GetKey().Handle, subKeyName);
    }
    else
    {
      try
      {
        rootKey.GetKey().DeleteSubKey(subKeyName);
      }
      catch (ArgumentException ex)
      {
        throw new ExternalException(ex.Message);
      }
    }
  }

  public static void DeleteValue(object key, string value)
  {
    PythonWinReg.GetRootKey(key).GetKey().DeleteValue(value, true);
  }

  public static string EnumKey(object key, int index)
  {
    PythonWinReg.HKEYType rootKey = PythonWinReg.GetRootKey(key);
    int lpcbName = 257;
    StringBuilder lpName = new StringBuilder(lpcbName);
    if (PythonWinReg.RegEnumKeyEx(rootKey.GetKey().Handle, index, lpName, ref lpcbName, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) != 0)
      throw PythonExceptions.CreateThrowable(PythonExceptions.WindowsError, (object) 22, (object) "No more data is available");
    return lpName.ToString();
  }

  public static PythonTuple EnumValue(object key, int index)
  {
    PythonWinReg.HKEYType rootKey = PythonWinReg.GetRootKey(key);
    if (index >= rootKey.GetKey().ValueCount)
      throw PythonExceptions.CreateThrowable(PythonExceptions.WindowsError, (object) 22, (object) "No more data is available");
    RegistryKey key1 = rootKey.GetKey();
    string valueName = key1.GetValueNames()[index];
    int valueKind;
    object obj;
    PythonWinReg.QueryValueExImpl(rootKey.hkey == PythonWinReg.HKEY_PERFORMANCE_DATA ? new SafeRegistryHandle(new IntPtr(-2147483644 /*0x80000004*/), true) : key1.Handle, valueName, out valueKind, out obj);
    return PythonTuple.MakeTuple((object) valueName, obj, (object) valueKind);
  }

  private static void QueryValueExImpl(
    SafeRegistryHandle handle,
    string valueName,
    out int valueKind,
    out object value)
  {
    valueName = valueName ?? "";
    valueKind = 0;
    byte[] numArray = new byte[128 /*0x80*/];
    uint length = (uint) numArray.Length;
    int num = PythonWinReg.RegQueryValueEx(handle, valueName, IntPtr.Zero, out valueKind, numArray, ref length);
    while (true)
    {
      switch (num)
      {
        case 0:
          goto label_5;
        case 2:
          goto label_3;
        case 234:
          numArray = new byte[numArray.Length * 2];
          length = (uint) numArray.Length;
          num = PythonWinReg.RegQueryValueEx(handle, valueName, IntPtr.Zero, out valueKind, numArray, ref length);
          continue;
        default:
          goto label_4;
      }
    }
label_3:
    throw PythonExceptions.CreateThrowable(PythonExceptions.WindowsError, (object) 2, (object) "The system cannot find the file specified");
label_4:
    throw PythonExceptions.CreateThrowable(PythonExceptions.WindowsError, (object) num);
label_5:
    switch (valueKind)
    {
      case 1:
      case 2:
        if (length >= 2U && numArray[(int) length - 1] == (byte) 0 && numArray[(int) length - 2] == (byte) 0)
        {
          value = (object) PythonWinReg.ExtractString(numArray, 0, (int) length - 2);
          break;
        }
        value = (object) PythonWinReg.ExtractString(numArray, 0, (int) length);
        break;
      case 3:
        value = length == 0U ? (object) (string) null : (object) ((IList<byte>) numArray).MakeString((int) length);
        break;
      case 4:
        if (BitConverter.IsLittleEndian)
        {
          value = (object) (uint) ((int) numArray[3] << 24 | (int) numArray[2] << 16 /*0x10*/ | (int) numArray[1] << 8 | (int) numArray[0]);
          break;
        }
        value = (object) (uint) ((int) numArray[0] << 24 | (int) numArray[1] << 16 /*0x10*/ | (int) numArray[2] << 8 | (int) numArray[3]);
        break;
      case 7:
        IronPython.Runtime.List list = new IronPython.Runtime.List();
        int start = 0;
        while ((long) start < (long) length)
        {
          for (int end = start; (long) end < (long) length; end += 2)
          {
            if (numArray[end] == (byte) 0 && numArray[end + 1] == (byte) 0)
            {
              list.Add((object) PythonWinReg.ExtractString(numArray, start, end));
              start = end + 2;
              if ((long) (start + 2) <= (long) length && numArray[start] == (byte) 0 && numArray[start + 1] == (byte) 0)
              {
                start = numArray.Length;
                break;
              }
            }
          }
          if (start != numArray.Length)
            list.Add((object) PythonWinReg.ExtractString(numArray, start, numArray.Length));
        }
        value = (object) list;
        break;
      default:
        value = (object) null;
        break;
    }
  }

  public static string ExpandEnvironmentStrings(string value)
  {
    return value != null ? Environment.ExpandEnvironmentVariables(value) : throw PythonExceptions.CreateThrowable(PythonExceptions.TypeError, (object) "must be unicode, not None");
  }

  private static string ExtractString(byte[] data, int start, int end)
  {
    if (end <= start)
      return string.Empty;
    char[] chArray = new char[(end - start) / 2];
    for (int index = 0; index < chArray.Length; ++index)
      chArray[index] = (char) ((uint) data[index * 2 + start] | (uint) data[index * 2 + start + 1] << 8);
    return new string(chArray);
  }

  public static void FlushKey(object key) => PythonWinReg.GetRootKey(key).GetKey().Flush();

  public static PythonWinReg.HKEYType OpenKey(object key, string subKeyName)
  {
    return PythonWinReg.OpenKey(key, subKeyName, 0, 131097);
  }

  public static PythonWinReg.HKEYType OpenKey(object key, string subKeyName, int res = 0, int sam = 131097)
  {
    RegistryKey key1 = PythonWinReg.GetRootKey(key).GetKey();
    RegistryKey key2;
    try
    {
      if ((sam & 2) == 2 || (sam & 4) == 4)
      {
        key2 = res == 0 ? key1.OpenSubKey(subKeyName, true) : key1.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.Default, (RegistryRights) res);
      }
      else
      {
        if ((sam & 1) != 1 && (sam & 8) != 8 && (sam & 16 /*0x10*/) != 16 /*0x10*/)
          throw new Win32Exception("Unexpected mode");
        key2 = res == 0 ? key1.OpenSubKey(subKeyName, false) : key1.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.ReadSubTree, (RegistryRights) res);
      }
    }
    catch (SecurityException ex)
    {
      throw PythonExceptions.CreateThrowable(PythonExceptions.WindowsError, (object) 5, (object) "Access is denied");
    }
    return key2 != null ? new PythonWinReg.HKEYType(key2) : throw PythonExceptions.CreateThrowable(PythonExceptions.WindowsError, (object) 2, (object) "The system cannot find the file specified");
  }

  public static PythonWinReg.HKEYType OpenKeyEx(object key, string subKeyName, int res = 0, int sam = 131097)
  {
    return PythonWinReg.OpenKey(key, subKeyName, res, sam);
  }

  public static PythonTuple QueryInfoKey(object key)
  {
    PythonWinReg.HKEYType hkeyType = (PythonWinReg.HKEYType) null;
    if (key is int key1)
    {
      if (HKeyHandleCache.cache.ContainsKey(key1) && HKeyHandleCache.cache[(int) key].IsAlive)
        hkeyType = HKeyHandleCache.cache[(int) key].Target as PythonWinReg.HKEYType;
    }
    else
      hkeyType = PythonWinReg.GetRootKey(key);
    if (hkeyType == null)
      throw PythonExceptions.CreateThrowable(PythonExceptions.EnvironmentError, (object) "key has been closed");
    try
    {
      RegistryKey key2 = hkeyType.GetKey();
      return PythonTuple.MakeTuple((object) key2.SubKeyCount, (object) key2.ValueCount, (object) 0);
    }
    catch (ObjectDisposedException ex)
    {
      throw new ExternalException(ex.Message);
    }
  }

  public static object QueryValue(object key, string subKeyName)
  {
    return PythonWinReg.OpenKey(key, subKeyName).GetKey().GetValue((string) null);
  }

  public static PythonTuple QueryValueEx(object key, string valueName)
  {
    PythonWinReg.HKEYType rootKey = PythonWinReg.GetRootKey(key);
    int valueKind;
    object obj;
    PythonWinReg.QueryValueExImpl(rootKey.hkey == PythonWinReg.HKEY_PERFORMANCE_DATA ? new SafeRegistryHandle(new IntPtr(-2147483644 /*0x80000004*/), true) : rootKey.GetKey().Handle, valueName, out valueKind, out obj);
    return PythonTuple.MakeTuple(obj, (object) valueKind);
  }

  public static void SetValue(object key, string subKeyName, int type, string value)
  {
    PythonWinReg.CreateKey(key, subKeyName).GetKey().SetValue((string) null, (object) value);
  }

  public static void SetValueEx(
    object key,
    string valueName,
    object reserved,
    int type,
    object value)
  {
    PythonWinReg.HKEYType rootKey = PythonWinReg.GetRootKey(key);
    RegistryValueKind valueKind = (RegistryValueKind) type;
    if (value == null)
    {
      PythonWinReg.RegSetValueEx(rootKey.GetKey().Handle, valueName, 0, type, (byte[]) null, 0);
    }
    else
    {
      switch (valueKind)
      {
        case RegistryValueKind.Binary:
          byte[] numArray = (byte[]) null;
          if (value is string)
            numArray = new ASCIIEncoding().GetBytes(value as string);
          rootKey.GetKey().SetValue(valueName, (object) numArray, valueKind);
          break;
        case RegistryValueKind.DWord:
          if (value is BigInteger)
            value = (object) (int) (uint) (BigInteger) value;
          rootKey.GetKey().SetValue(valueName, value, valueKind);
          break;
        case RegistryValueKind.MultiString:
          int size = ((IronPython.Runtime.List) value)._size;
          string[] destinationArray = new string[size];
          Array.Copy((Array) ((IronPython.Runtime.List) value)._data, (Array) destinationArray, size);
          rootKey.GetKey().SetValue(valueName, (object) destinationArray, valueKind);
          break;
        case RegistryValueKind.QWord:
          if (value is BigInteger)
            value = (object) (long) (ulong) (BigInteger) value;
          rootKey.GetKey().SetValue(valueName, value, valueKind);
          break;
        default:
          rootKey.GetKey().SetValue(valueName, value, valueKind);
          break;
      }
    }
  }

  public static PythonWinReg.HKEYType ConnectRegistry(string computerName, BigInteger key)
  {
    if (string.IsNullOrEmpty(computerName))
      computerName = string.Empty;
    RegistryKey key1;
    try
    {
      key1 = !(computerName == string.Empty) ? RegistryKey.OpenRemoteBaseKey(PythonWinReg.MapSystemKey(key), computerName) : RegistryKey.OpenBaseKey(PythonWinReg.MapSystemKey(key), RegistryView.Default);
    }
    catch (IOException ex)
    {
      throw PythonExceptions.CreateThrowable(PythonExceptions.WindowsError, (object) 53, (object) ex.Message);
    }
    catch (Exception ex)
    {
      throw new ExternalException(ex.Message);
    }
    return new PythonWinReg.HKEYType(key1);
  }

  public static void DisableReflectionKey(object key)
  {
    PythonWinReg.HKEYType rootKey = PythonWinReg.GetRootKey(key);
    if (!Environment.Is64BitOperatingSystem)
      throw new NotImplementedException("not implemented on this platform");
    int num = PythonWinReg.RegDisableReflectionKey(rootKey.GetKey().Handle);
    if (num != 0)
      throw PythonExceptions.CreateThrowable(PythonExceptions.WindowsError, (object) num);
  }

  public static void EnableReflectionKey(object key)
  {
    PythonWinReg.HKEYType rootKey = PythonWinReg.GetRootKey(key);
    if (!Environment.Is64BitOperatingSystem)
      throw new NotImplementedException("not implemented on this platform");
    int num = PythonWinReg.RegEnableReflectionKey(rootKey.GetKey().Handle);
    if (num != 0)
      throw PythonExceptions.CreateThrowable(PythonExceptions.WindowsError, (object) num);
  }

  public static bool QueryReflectionKey(object key)
  {
    PythonWinReg.HKEYType rootKey = PythonWinReg.GetRootKey(key);
    if (!Environment.Is64BitOperatingSystem)
      throw new NotImplementedException("not implemented on this platform");
    bool bIsReflectionDisabled;
    int num = PythonWinReg.RegQueryReflectionKey(rootKey.GetKey().Handle, out bIsReflectionDisabled);
    if (num != 0)
      throw PythonExceptions.CreateThrowable(PythonExceptions.WindowsError, (object) num);
    return bIsReflectionDisabled;
  }

  private static PythonWinReg.HKEYType GetRootKey(object key)
  {
    switch (key)
    {
      case PythonWinReg.HKEYType rootKey:
label_3:
        return rootKey;
      case BigInteger _:
        rootKey = new PythonWinReg.HKEYType(RegistryKey.OpenBaseKey(PythonWinReg.MapSystemKey((BigInteger) key), RegistryView.Default), (BigInteger) key);
        goto label_3;
      default:
        throw new InvalidCastException("The object is not a PyHKEY object");
    }
  }

  private static RegistryHive MapSystemKey(BigInteger hKey)
  {
    if (hKey == PythonWinReg.HKEY_CLASSES_ROOT)
      return RegistryHive.ClassesRoot;
    if (hKey == PythonWinReg.HKEY_CURRENT_CONFIG)
      return RegistryHive.CurrentConfig;
    if (hKey == PythonWinReg.HKEY_CURRENT_USER)
      return RegistryHive.CurrentUser;
    if (hKey == PythonWinReg.HKEY_DYN_DATA)
      return RegistryHive.DynData;
    if (hKey == PythonWinReg.HKEY_LOCAL_MACHINE)
      return RegistryHive.LocalMachine;
    if (hKey == PythonWinReg.HKEY_PERFORMANCE_DATA)
      return RegistryHive.PerformanceData;
    if (hKey == PythonWinReg.HKEY_USERS)
      return RegistryHive.Users;
    throw new ValueErrorException("Unknown system key");
  }

  private static int MapRegistryValueKind(RegistryValueKind registryValueKind)
  {
    return (int) registryValueKind;
  }

  [PythonType]
  public class HKEYType : IDisposable
  {
    private RegistryKey key;
    internal readonly BigInteger hkey = (BigInteger) 0;

    internal HKEYType(RegistryKey key)
    {
      this.key = key;
      HKeyHandleCache.cache[key.GetHashCode()] = new WeakReference((object) this);
    }

    internal HKEYType(RegistryKey key, BigInteger hkey)
      : this(key)
    {
      this.hkey = hkey;
    }

    public void Close()
    {
      lock (this)
      {
        if (this.key == null)
          return;
        HKeyHandleCache.cache.Remove(this.key.GetHashCode());
        this.key.Dispose();
        this.key = (RegistryKey) null;
      }
    }

    public int Detach() => 0;

    public int handle
    {
      get
      {
        lock (this)
          return this.key == null ? 0 : this.key.GetHashCode();
      }
    }

    public static implicit operator int(PythonWinReg.HKEYType hKey) => hKey.handle;

    [PythonHidden(new PlatformID[] {})]
    public RegistryKey GetKey()
    {
      lock (this)
        return this.key != null ? this.key : throw PythonExceptions.CreateThrowable(PythonExceptions.EnvironmentError, (object) "key has been closed");
    }

    void IDisposable.Dispose() => this.Close();
  }
}
