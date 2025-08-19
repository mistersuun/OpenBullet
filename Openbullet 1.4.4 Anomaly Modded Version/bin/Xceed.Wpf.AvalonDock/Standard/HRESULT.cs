// Decompiled with JetBrains decompiler
// Type: Standard.HRESULT
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[StructLayout(LayoutKind.Explicit)]
internal struct HRESULT(uint i)
{
  [FieldOffset(0)]
  private readonly uint _value = i;
  public static readonly HRESULT S_OK = new HRESULT(0U);
  public static readonly HRESULT S_FALSE = new HRESULT(1U);
  public static readonly HRESULT E_PENDING = new HRESULT(2147483658U /*0x8000000A*/);
  public static readonly HRESULT E_NOTIMPL = new HRESULT(2147500033U /*0x80004001*/);
  public static readonly HRESULT E_NOINTERFACE = new HRESULT(2147500034U /*0x80004002*/);
  public static readonly HRESULT E_POINTER = new HRESULT(2147500035U /*0x80004003*/);
  public static readonly HRESULT E_ABORT = new HRESULT(2147500036U /*0x80004004*/);
  public static readonly HRESULT E_FAIL = new HRESULT(2147500037U /*0x80004005*/);
  public static readonly HRESULT E_UNEXPECTED = new HRESULT(2147549183U);
  public static readonly HRESULT STG_E_INVALIDFUNCTION = new HRESULT(2147680257U /*0x80030001*/);
  public static readonly HRESULT REGDB_E_CLASSNOTREG = new HRESULT(2147746132U);
  public static readonly HRESULT DESTS_E_NO_MATCHING_ASSOC_HANDLER = new HRESULT(2147749635U);
  public static readonly HRESULT DESTS_E_NORECDOCS = new HRESULT(2147749636U);
  public static readonly HRESULT DESTS_E_NOTALLCLEARED = new HRESULT(2147749637U);
  public static readonly HRESULT E_ACCESSDENIED = new HRESULT(2147942405U /*0x80070005*/);
  public static readonly HRESULT E_OUTOFMEMORY = new HRESULT(2147942414U /*0x8007000E*/);
  public static readonly HRESULT E_INVALIDARG = new HRESULT(2147942487U);
  public static readonly HRESULT INTSAFE_E_ARITHMETIC_OVERFLOW = new HRESULT(2147942934U);
  public static readonly HRESULT COR_E_OBJECTDISPOSED = new HRESULT(2148734498U);
  public static readonly HRESULT WC_E_GREATERTHAN = new HRESULT(3222072867U);
  public static readonly HRESULT WC_E_SYNTAX = new HRESULT(3222072877U);

  public static HRESULT Make(bool severe, Facility facility, int code)
  {
    return new HRESULT((uint) ((severe ? int.MinValue : 0) | (int) facility << 16 /*0x10*/ | code));
  }

  public Facility Facility => HRESULT.GetFacility((int) this._value);

  public static Facility GetFacility(int errorCode)
  {
    return (Facility) (errorCode >> 16 /*0x10*/ & 8191 /*0x1FFF*/);
  }

  public int Code => HRESULT.GetCode((int) this._value);

  public static int GetCode(int error) => error & (int) ushort.MaxValue;

  public override string ToString()
  {
    foreach (FieldInfo field in typeof (HRESULT).GetFields(BindingFlags.Static | BindingFlags.Public))
    {
      if (field.FieldType == typeof (HRESULT) && (HRESULT) field.GetValue((object) null) == this)
        return field.Name;
    }
    if (this.Facility == Facility.Win32)
    {
      foreach (FieldInfo field in typeof (Win32Error).GetFields(BindingFlags.Static | BindingFlags.Public))
      {
        if (field.FieldType == typeof (Win32Error) && (HRESULT) (Win32Error) field.GetValue((object) null) == this)
          return $"HRESULT_FROM_WIN32({field.Name})";
      }
    }
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:X8}", (object) this._value);
  }

  public override bool Equals(object obj)
  {
    try
    {
      return (int) ((HRESULT) obj)._value == (int) this._value;
    }
    catch (InvalidCastException ex)
    {
      return false;
    }
  }

  public override int GetHashCode() => this._value.GetHashCode();

  public static bool operator ==(HRESULT hrLeft, HRESULT hrRight)
  {
    return (int) hrLeft._value == (int) hrRight._value;
  }

  public static bool operator !=(HRESULT hrLeft, HRESULT hrRight) => !(hrLeft == hrRight);

  public bool Succeeded => (int) this._value >= 0;

  public bool Failed => (int) this._value < 0;

  public void ThrowIfFailed() => this.ThrowIfFailed((string) null);

  public void ThrowIfFailed(string message)
  {
    if (this.Failed)
    {
      if (string.IsNullOrEmpty(message))
        message = this.ToString();
      Exception exception = Marshal.GetExceptionForHR((int) this._value, new IntPtr(-1));
      if (exception.GetType() == typeof (COMException))
      {
        exception = this.Facility != Facility.Win32 ? (Exception) new COMException(message, (int) this._value) : (Exception) new Win32Exception(this.Code, message);
      }
      else
      {
        ConstructorInfo constructor = exception.GetType().GetConstructor(new Type[1]
        {
          typeof (string)
        });
        if ((ConstructorInfo) null != constructor)
          exception = constructor.Invoke(new object[1]
          {
            (object) message
          }) as Exception;
      }
      throw exception;
    }
  }

  public static void ThrowLastError() => ((HRESULT) Win32Error.GetLastError()).ThrowIfFailed();
}
