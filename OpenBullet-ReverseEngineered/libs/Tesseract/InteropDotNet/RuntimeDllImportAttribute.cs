// Decompiled with JetBrains decompiler
// Type: InteropDotNet.RuntimeDllImportAttribute
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace InteropDotNet;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
internal sealed class RuntimeDllImportAttribute : Attribute
{
  public string EntryPoint;
  public CallingConvention CallingConvention;
  public CharSet CharSet;
  public bool SetLastError;
  public bool BestFitMapping;
  public bool ThrowOnUnmappableChar;

  public string LibraryFileName { get; private set; }

  public RuntimeDllImportAttribute(string libraryFileName)
  {
    this.LibraryFileName = libraryFileName;
  }
}
