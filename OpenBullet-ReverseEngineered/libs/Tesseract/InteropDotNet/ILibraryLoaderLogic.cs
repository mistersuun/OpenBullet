// Decompiled with JetBrains decompiler
// Type: InteropDotNet.ILibraryLoaderLogic
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;

#nullable disable
namespace InteropDotNet;

internal interface ILibraryLoaderLogic
{
  IntPtr LoadLibrary(string fileName);

  bool FreeLibrary(IntPtr libraryHandle);

  IntPtr GetProcAddress(IntPtr libraryHandle, string functionName);

  string FixUpLibraryName(string fileName);
}
