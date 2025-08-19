// Decompiled with JetBrains decompiler
// Type: Standard.PROPVARIANT
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[StructLayout(LayoutKind.Explicit)]
internal class PROPVARIANT : IDisposable
{
  [FieldOffset(0)]
  private ushort vt;
  [FieldOffset(8)]
  private IntPtr pointerVal;
  [FieldOffset(8)]
  private byte byteVal;
  [FieldOffset(8)]
  private long longVal;
  [FieldOffset(8)]
  private short boolVal;

  public VarEnum VarType => (VarEnum) this.vt;

  public string GetValue()
  {
    return this.vt == (ushort) 31 /*0x1F*/ ? Marshal.PtrToStringUni(this.pointerVal) : (string) null;
  }

  public void SetValue(bool f)
  {
    this.Clear();
    this.vt = (ushort) 11;
    this.boolVal = f ? (short) -1 : (short) 0;
  }

  public void SetValue(string val)
  {
    this.Clear();
    this.vt = (ushort) 31 /*0x1F*/;
    this.pointerVal = Marshal.StringToCoTaskMemUni(val);
  }

  public void Clear() => PROPVARIANT.NativeMethods.PropVariantClear(this);

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  ~PROPVARIANT() => this.Dispose(false);

  private void Dispose(bool disposing) => this.Clear();

  private static class NativeMethods
  {
    [DllImport("ole32.dll")]
    internal static extern HRESULT PropVariantClear(PROPVARIANT pvar);
  }
}
