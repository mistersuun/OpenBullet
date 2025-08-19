// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ExcepInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Reflection;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal struct ExcepInfo
{
  private short wCode;
  private short wReserved;
  private IntPtr bstrSource;
  private IntPtr bstrDescription;
  private IntPtr bstrHelpFile;
  private int dwHelpContext;
  private IntPtr pvReserved;
  private IntPtr pfnDeferredFillIn;
  private int scode;

  private static string ConvertAndFreeBstr(ref IntPtr bstr)
  {
    if (bstr == IntPtr.Zero)
      return (string) null;
    string stringBstr = Marshal.PtrToStringBSTR(bstr);
    Marshal.FreeBSTR(bstr);
    bstr = IntPtr.Zero;
    return stringBstr;
  }

  internal void Dummy()
  {
    this.wCode = (short) 0;
    this.wReserved = (short) 0;
    ++this.wReserved;
    this.bstrSource = IntPtr.Zero;
    this.bstrDescription = IntPtr.Zero;
    this.bstrHelpFile = IntPtr.Zero;
    this.dwHelpContext = 0;
    this.pfnDeferredFillIn = IntPtr.Zero;
    this.pvReserved = IntPtr.Zero;
    this.scode = 0;
    throw Error.MethodShouldNotBeCalled();
  }

  internal Exception GetException()
  {
    int errorCode = this.scode != 0 ? this.scode : (int) this.wCode;
    Exception exception = Marshal.GetExceptionForHR(errorCode);
    string message = ExcepInfo.ConvertAndFreeBstr(ref this.bstrDescription);
    if (message != null)
    {
      if (exception is COMException)
      {
        exception = (Exception) new COMException(message, errorCode);
      }
      else
      {
        ConstructorInfo constructor = exception.GetType().GetConstructor(new Type[1]
        {
          typeof (string)
        });
        if (constructor != (ConstructorInfo) null)
          exception = (Exception) constructor.Invoke(new object[1]
          {
            (object) message
          });
      }
    }
    exception.Source = ExcepInfo.ConvertAndFreeBstr(ref this.bstrSource);
    string str = ExcepInfo.ConvertAndFreeBstr(ref this.bstrHelpFile);
    if (str != null && this.dwHelpContext != 0)
      str = $"{str}#{(object) this.dwHelpContext}";
    exception.HelpLink = str;
    return exception;
  }
}
