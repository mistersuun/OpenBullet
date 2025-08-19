// Decompiled with JetBrains decompiler
// Type: Tesseract.Interop.MarshalHelper
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace Tesseract.Interop;

internal static class MarshalHelper
{
  public static IntPtr StringToPtr(string value, Encoding encoding)
  {
    encoding.GetEncoder();
    byte[] numArray = new byte[encoding.GetByteCount(value) + 1];
    encoding.GetBytes(value, 0, value.Length, numArray, 0);
    IntPtr destination = Marshal.AllocHGlobal(new IntPtr(numArray.Length));
    Marshal.Copy(numArray, 0, destination, numArray.Length);
    return destination;
  }

  public static unsafe string PtrToString(IntPtr handle, Encoding encoding)
  {
    int length = MarshalHelper.StrLength(handle);
    return new string((sbyte*) handle.ToPointer(), 0, length, encoding);
  }

  public static unsafe int StrLength(IntPtr handle)
  {
    byte* pointer = (byte*) handle.ToPointer();
    int index = 0;
    while (pointer[index] != (byte) 0)
      ++index;
    return index;
  }
}
