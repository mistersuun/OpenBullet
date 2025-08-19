// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MemoryMapping
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security;

#nullable disable
namespace Microsoft.Scripting.Metadata;

[SecurityCritical]
public sealed class MemoryMapping : CriticalFinalizerObject
{
  [SecurityCritical]
  internal unsafe byte* _pointer;
  private SafeMemoryMappedViewHandle _handle;
  internal long _capacity;

  [CLSCompliant(false)]
  public unsafe byte* Pointer
  {
    [SecurityCritical] get
    {
      return (IntPtr) this._pointer != IntPtr.Zero ? this._pointer : throw new ObjectDisposedException(nameof (MemoryMapping));
    }
  }

  public long Capacity => this._capacity;

  public unsafe MemoryBlock GetRange(int start, int length)
  {
    if ((IntPtr) this._pointer == IntPtr.Zero)
      throw new ObjectDisposedException(nameof (MemoryMapping));
    if (start < 0)
      throw new ArgumentOutOfRangeException(nameof (start));
    if (length < 0 || (long) length > this._capacity - (long) start)
      throw new ArgumentOutOfRangeException(nameof (length));
    return new MemoryBlock((object) this, this._pointer + start, length);
  }

  [SecuritySafeCritical]
  public static unsafe MemoryMapping Create(string path)
  {
    MemoryMappedFile memoryMappedFile = (MemoryMappedFile) null;
    MemoryMappedViewAccessor mappedViewAccessor = (MemoryMappedViewAccessor) null;
    MemoryMapping memoryMapping = (MemoryMapping) null;
    FileStream fileStream = (FileStream) null;
    byte* pointer = (byte*) null;
    try
    {
      fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
      memoryMappedFile = MemoryMappedFile.CreateFromFile(fileStream, (string) null, 0L, MemoryMappedFileAccess.Read, (MemoryMappedFileSecurity) null, HandleInheritability.None, true);
      mappedViewAccessor = memoryMappedFile.CreateViewAccessor(0L, 0L, MemoryMappedFileAccess.Read);
      memoryMapping = new MemoryMapping();
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
      }
      finally
      {
        SafeMemoryMappedViewHandle mappedViewHandle = mappedViewAccessor.SafeMemoryMappedViewHandle;
        mappedViewHandle.AcquirePointer(ref pointer);
        if ((IntPtr) pointer == IntPtr.Zero)
          throw new IOException("Cannot create a file mapping");
        memoryMapping._handle = mappedViewHandle;
        memoryMapping._pointer = pointer;
        memoryMapping._capacity = mappedViewAccessor.Capacity;
      }
    }
    finally
    {
      fileStream?.Dispose();
      mappedViewAccessor?.Dispose();
      memoryMappedFile?.Dispose();
    }
    return memoryMapping;
  }

  [SecuritySafeCritical]
  unsafe ~MemoryMapping()
  {
    if ((IntPtr) this._pointer == IntPtr.Zero)
      return;
    this._handle.ReleasePointer();
    this._pointer = (byte*) null;
  }
}
