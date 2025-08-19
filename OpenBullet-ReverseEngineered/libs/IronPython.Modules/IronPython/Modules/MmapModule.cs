// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.MmapModule
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Modules;

public static class MmapModule
{
  public const int ACCESS_READ = 1;
  public const int ACCESS_WRITE = 2;
  public const int ACCESS_COPY = 3;
  private const int SEEK_SET = 0;
  private const int SEEK_CUR = 1;
  private const int SEEK_END = 2;
  [PythonHidden(PlatformsAttribute.PlatformFamily.Windows)]
  public const int MAP_SHARED = 1;
  [PythonHidden(PlatformsAttribute.PlatformFamily.Windows)]
  public const int MAP_PRIVATE = 2;
  [PythonHidden(PlatformsAttribute.PlatformFamily.Windows)]
  public const int PROT_NONE = 0;
  [PythonHidden(PlatformsAttribute.PlatformFamily.Windows)]
  public const int PROT_READ = 1;
  [PythonHidden(PlatformsAttribute.PlatformFamily.Windows)]
  public const int PROT_WRITE = 2;
  [PythonHidden(PlatformsAttribute.PlatformFamily.Windows)]
  public const int PROT_EXEC = 4;
  public static readonly int ALLOCATIONGRANULARITY = MmapModule.GetAllocationGranularity();
  public static readonly int PAGESIZE = Environment.SystemPageSize;
  public static readonly string __doc__ = (string) null;
  private static readonly object _mmapErrorKey = new object();

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    context.EnsureModuleException(MmapModule._mmapErrorKey, PythonExceptions.EnvironmentError, dict, "error", "mmap");
  }

  private static Exception Error(CodeContext context, string message)
  {
    return PythonExceptions.CreateThrowable((PythonType) context.LanguageContext.GetModuleState(MmapModule._mmapErrorKey), (object) message);
  }

  private static Exception Error(CodeContext context, int errno, string message)
  {
    return PythonExceptions.CreateThrowable((PythonType) context.LanguageContext.GetModuleState(MmapModule._mmapErrorKey), (object) errno, (object) message);
  }

  private static string FormatError(int errorCode) => new Win32Exception(errorCode).Message;

  private static Exception WindowsError(int code)
  {
    return PythonExceptions.CreateThrowable(PythonExceptions.WindowsError, (object) code, (object) MmapModule.FormatError(code));
  }

  public static PythonType mmap
  {
    get
    {
      return IronPython.RuntimeInformation.IsOSPlatform(IronPython.OSPlatform.Windows) ? DynamicHelpers.GetPythonTypeFromType(typeof (MmapModule.MmapDefault)) : DynamicHelpers.GetPythonTypeFromType(typeof (MmapModule.MmapUnix));
    }
  }

  [DllImport("kernel32", SetLastError = true)]
  private static extern void GetSystemInfo(ref MmapModule.SYSTEM_INFO lpSystemInfo);

  private static int GetAllocationGranularity()
  {
    try
    {
      return MmapModule.GetAllocationGranularityWorker();
    }
    catch
    {
      return Environment.SystemPageSize;
    }
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static int GetAllocationGranularityWorker()
  {
    MmapModule.SYSTEM_INFO lpSystemInfo = new MmapModule.SYSTEM_INFO();
    MmapModule.GetSystemInfo(ref lpSystemInfo);
    return lpSystemInfo.dwAllocationGranularity;
  }

  private static MemoryMappedFile CreateFromFile(
    FileStream fileStream,
    string mapName,
    long capacity,
    MemoryMappedFileAccess access,
    HandleInheritability inheritability,
    bool leaveOpen)
  {
    return MemoryMappedFile.CreateFromFile(fileStream, mapName, capacity, access, (MemoryMappedFileSecurity) null, inheritability, leaveOpen);
  }

  [PythonType("mmap.mmap")]
  [PythonHidden(new PlatformID[] {})]
  public class MmapUnix(
    CodeContext context,
    int fileno,
    long length,
    string tagname = null,
    int access = 2,
    long offset = 0,
    int flags = 1,
    int prot = 3) : MmapModule.MmapDefault(context, fileno, length, tagname, access, offset)
  {
  }

  [PythonType("mmap.mmap")]
  [PythonHidden(new PlatformID[] {})]
  public class MmapDefault
  {
    private MemoryMappedFile _file;
    private MemoryMappedViewAccessor _view;
    private long _position;
    private FileStream _sourceStream;
    private readonly long _offset;
    private readonly string _mapName;
    private readonly MemoryMappedFileAccess _fileAccess;
    private volatile bool _isClosed;
    private int _refCount = 1;

    public MmapDefault(
      CodeContext context,
      int fileno,
      long length,
      string tagname = null,
      int access = 2,
      long offset = 0)
    {
      switch (access)
      {
        case 1:
          this._fileAccess = MemoryMappedFileAccess.Read;
          break;
        case 2:
          this._fileAccess = MemoryMappedFileAccess.ReadWrite;
          break;
        case 3:
          this._fileAccess = MemoryMappedFileAccess.CopyOnWrite;
          break;
        default:
          throw PythonOps.ValueError("mmap invalid access parameter");
      }
      if (length < 0L)
        throw PythonOps.OverflowError("memory mapped size must be positive");
      if (offset < 0L)
        throw PythonOps.OverflowError("memory mapped offset must be positive");
      if (length > (long) int.MaxValue)
        throw PythonOps.OverflowError("cannot fit 'long' into an index-sized integer");
      if (offset % (long) MmapModule.ALLOCATIONGRANULARITY != 0L)
        throw MmapModule.WindowsError(1132);
      this._mapName = tagname == "" ? (string) null : tagname;
      if (fileno == -1 || fileno == 0)
      {
        this._offset = 0L;
        this._sourceStream = (FileStream) null;
        if (this._mapName == null)
          this._mapName = Guid.NewGuid().ToString();
        this._file = MemoryMappedFile.CreateOrOpen(this._mapName, length, this._fileAccess);
      }
      else
      {
        this._offset = offset;
        PythonContext languageContext = context.LanguageContext;
        PythonFile pf;
        if (languageContext.FileManager.TryGetFileFromId(languageContext, fileno, out pf))
        {
          if ((this._sourceStream = pf._stream as FileStream) == null)
            throw MmapModule.WindowsError(6);
        }
        else
        {
          object o;
          if (!languageContext.FileManager.TryGetObjectFromId(languageContext, fileno, out o) || !(o is PythonIOModule.FileIO fileIo))
            throw MmapModule.Error(context, 9, "Bad file descriptor");
          if ((this._sourceStream = fileIo._readStream as FileStream) == null)
            throw MmapModule.WindowsError(6);
        }
        if (this._fileAccess == MemoryMappedFileAccess.ReadWrite && !this._sourceStream.CanWrite)
          throw MmapModule.WindowsError(5);
        if (length == 0L)
        {
          length = this._sourceStream.Length;
          if (length == 0L)
            throw PythonOps.ValueError("cannot mmap an empty file");
          if (this._offset >= length)
            throw PythonOps.ValueError("mmap offset is greater than file size");
          length -= this._offset;
        }
        long num = checked (this._offset + length);
        if (num > this._sourceStream.Length)
        {
          if (!this._sourceStream.CanWrite)
            throw MmapModule.WindowsError(8);
          this._sourceStream.SetLength(num);
        }
        this._file = MmapModule.CreateFromFile(this._sourceStream, this._mapName, this._sourceStream.Length, this._fileAccess, HandleInheritability.None, true);
      }
      try
      {
        this._view = this._file.CreateViewAccessor(this._offset, length, this._fileAccess);
      }
      catch
      {
        this._file.Dispose();
        this._file = (MemoryMappedFile) null;
        throw;
      }
      this._position = 0L;
    }

    public object __len__()
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
        return MmapModule.MmapDefault.ReturnLong(this._view.Capacity);
    }

    public string this[long index]
    {
      get
      {
        using (new MmapModule.MmapDefault.MmapLocker(this))
        {
          this.CheckIndex(index);
          return ((char) this._view.ReadByte(index)).ToString();
        }
      }
      set
      {
        using (new MmapModule.MmapDefault.MmapLocker(this))
        {
          if (value == null || value.Length != 1)
            throw PythonOps.IndexError("mmap assignment must be a single-character string");
          this.EnsureWritable();
          this.CheckIndex(index);
          this._view.Write(index, (byte) value[0]);
        }
      }
    }

    public string this[IronPython.Runtime.Slice slice]
    {
      get
      {
        using (new MmapModule.MmapDefault.MmapLocker(this))
        {
          long ostart;
          long ostep;
          long ocount;
          PythonOps.FixSlice(this._view.Capacity, MmapModule.MmapDefault.GetLong(slice.start), MmapModule.MmapDefault.GetLong(slice.stop), MmapModule.MmapDefault.GetLong(slice.step), out ostart, out long _, out ostep, out ocount);
          int capacity = (int) ocount;
          if (capacity == 0)
            return "";
          StringBuilder stringBuilder = new StringBuilder(capacity);
          for (; capacity > 0; --capacity)
          {
            stringBuilder.Append((char) this._view.ReadByte(ostart));
            ostart += ostep;
          }
          return stringBuilder.ToString();
        }
      }
      set
      {
        using (new MmapModule.MmapDefault.MmapLocker(this))
        {
          if (value == null)
            throw PythonOps.TypeError("mmap slice assignment must be a string");
          this.EnsureWritable();
          long ostart;
          long ostep;
          long ocount;
          PythonOps.FixSlice(this._view.Capacity, MmapModule.MmapDefault.GetLong(slice.start), MmapModule.MmapDefault.GetLong(slice.stop), MmapModule.MmapDefault.GetLong(slice.step), out ostart, out long _, out ostep, out ocount);
          int num1 = (int) ocount;
          if (value.Length != num1)
            throw PythonOps.IndexError("mmap slice assignment is wrong size");
          if (num1 == 0)
            return;
          byte[] array = value.MakeByteArray();
          if (ostep == 1L)
          {
            this._view.WriteArray<byte>(ostart, array, 0, value.Length);
          }
          else
          {
            foreach (byte num2 in array)
            {
              this._view.Write(ostart, num2);
              ostart += ostep;
            }
          }
        }
      }
    }

    public void __delitem__(long index)
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
      {
        this.CheckIndex(index);
        throw PythonOps.TypeError("mmap object doesn't support item deletion");
      }
    }

    public void __delslice__(IronPython.Runtime.Slice slice)
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
        throw PythonOps.TypeError("mmap object doesn't support slice deletion");
    }

    public void close()
    {
      if (this._isClosed)
        return;
      lock (this)
      {
        if (this._isClosed)
          return;
        this._isClosed = true;
        this.CloseWorker();
      }
    }

    private void CloseWorker()
    {
      if (Interlocked.Decrement(ref this._refCount) != 0)
        return;
      this._view.Flush();
      this._view.Dispose();
      this._file.Dispose();
      this._sourceStream = (FileStream) null;
      this._view = (MemoryMappedViewAccessor) null;
      this._file = (MemoryMappedFile) null;
    }

    public object find([NotNull] string s)
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
        return this.FindWorker(s, this.Position, this._view.Capacity);
    }

    public object find([NotNull] string s, long start)
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
        return this.FindWorker(s, start, this._view.Capacity);
    }

    public object find([NotNull] string s, long start, long end)
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
        return this.FindWorker(s, start, end);
    }

    private object FindWorker(string s, long start, long end)
    {
      ContractUtils.RequiresNotNull((object) s, nameof (s));
      start = PythonOps.FixSliceIndex(start, this._view.Capacity);
      end = PythonOps.FixSliceIndex(end, this._view.Capacity);
      if (s == "")
        return start > end ? (object) -1 : MmapModule.MmapDefault.ReturnLong(start);
      long count1 = end - start;
      if ((long) s.Length > count1)
        return (object) -1;
      int num1 = -1;
      int count2 = Math.Max(s.Length, MmapModule.PAGESIZE);
      CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
      if (count1 <= (long) (count2 * 2))
      {
        byte[] numArray = new byte[count1];
        this._view.ReadArray<byte>(start, numArray, 0, (int) count1);
        string source = ((IList<byte>) numArray).MakeString();
        num1 = compareInfo.IndexOf(source, s, CompareOptions.Ordinal);
      }
      else
      {
        byte[] numArray1 = new byte[count2];
        byte[] numArray2 = new byte[count2];
        this._view.ReadArray<byte>(start, numArray1, 0, count2);
        int length1 = this._view.ReadArray<byte>(start + (long) count2, numArray2, 0, count2);
        start += (long) (count2 * 2);
        long num2 = count1 - (long) (count2 * 2);
        while (num2 > 0L && length1 > 0)
        {
          string source = MmapModule.MmapDefault.GetString(numArray1, numArray2, length1);
          num1 = compareInfo.IndexOf(source, s, CompareOptions.Ordinal);
          if (num1 != -1)
            return MmapModule.MmapDefault.ReturnLong(start - (long) (2 * count2) + (long) num1);
          byte[] numArray3 = numArray1;
          numArray1 = numArray2;
          numArray2 = numArray3;
          int count3 = num2 < (long) count2 ? (int) num2 : count2;
          num2 -= (long) length1;
          length1 = this._view.ReadArray<byte>(start, numArray2, 0, count3);
          start += (long) length1;
        }
      }
      return num1 != -1 ? MmapModule.MmapDefault.ReturnLong(start + (long) num1) : (object) -1;
    }

    public int flush()
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
      {
        this._view.Flush();
        return 1;
      }
    }

    public int flush(long offset, long size)
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
      {
        this.CheckIndex(offset, false);
        this.CheckIndex(checked (offset + size), false);
        this._view.Flush();
        return 1;
      }
    }

    public void move(long dest, long src, long count)
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
      {
        this.EnsureWritable();
        if (dest < 0L || src < 0L || count < 0L || checked (Math.Max(src, dest) + count) > this._view.Capacity)
          throw PythonOps.ValueError("source or destination out of range");
        if (src == dest || count == 0L)
          return;
        if (count <= (long) MmapModule.PAGESIZE)
          this.MoveWorker(new byte[count], src, dest, (int) count);
        else if (src < dest)
        {
          byte[] buffer = new byte[MmapModule.PAGESIZE];
          for (; count >= (long) MmapModule.PAGESIZE; count -= (long) MmapModule.PAGESIZE)
          {
            this.MoveWorker(buffer, src, dest, MmapModule.PAGESIZE);
            src += (long) MmapModule.PAGESIZE;
            dest += (long) MmapModule.PAGESIZE;
          }
          if (count <= 0L)
            return;
          this.MoveWorker(buffer, src, dest, (int) count);
        }
        else
        {
          byte[] buffer = new byte[MmapModule.PAGESIZE];
          src += count;
          dest += count;
          int count1 = (int) (count % (long) MmapModule.PAGESIZE);
          if (count1 != 0)
          {
            src -= (long) count1;
            dest -= (long) count1;
            count -= (long) count1;
            this.MoveWorker(buffer, src, dest, count1);
          }
          while (count > 0L)
          {
            src -= (long) MmapModule.PAGESIZE;
            dest -= (long) MmapModule.PAGESIZE;
            count -= (long) MmapModule.PAGESIZE;
            this.MoveWorker(buffer, src, dest, MmapModule.PAGESIZE);
          }
        }
      }
    }

    private void MoveWorker(byte[] buffer, long src, long dest, int count)
    {
      this._view.ReadArray<byte>(src, buffer, 0, count);
      this._view.WriteArray<byte>(dest, buffer, 0, count);
    }

    public string read(int len)
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
      {
        long position = this.Position;
        if (len < 0)
          len = checked ((int) (this._view.Capacity - position));
        else if ((long) len > this._view.Capacity - position)
          len = checked ((int) (this._view.Capacity - position));
        if (len == 0)
          return "";
        byte[] numArray = new byte[len];
        len = this._view.ReadArray<byte>(position, numArray, 0, len);
        this.Position = position + (long) len;
        return ((IList<byte>) numArray).MakeString(len);
      }
    }

    public string read_byte()
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
      {
        long position = this.Position;
        if (position >= this._view.Capacity)
          throw PythonOps.ValueError("read byte out of range");
        int num = (int) this._view.ReadByte(position);
        this.Position = position + 1L;
        return ((char) num).ToString();
      }
    }

    public string readline()
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
      {
        StringBuilder stringBuilder = new StringBuilder();
        long position = this.Position;
        for (char ch = char.MinValue; ch != '\n' && position < this._view.Capacity; ++position)
        {
          ch = (char) this._view.ReadByte(position);
          stringBuilder.Append(ch);
        }
        this.Position = position;
        return stringBuilder.ToString();
      }
    }

    public void resize(long newsize)
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
      {
        if (this._fileAccess != MemoryMappedFileAccess.ReadWrite)
          throw PythonOps.TypeError("mmap can't resize a readonly or copy-on-write memory map.");
        if (this._sourceStream == null)
          throw MmapModule.WindowsError(87);
        if (newsize == 0L)
          throw MmapModule.WindowsError(this._offset == 0L ? 5 : 1006);
        if (this._view.Capacity == newsize)
          return;
        long num = checked (this._offset + newsize);
        try
        {
          this._view.Flush();
          this._view.Dispose();
          this._file.Dispose();
          bool leaveOpen = true;
          if (!this._sourceStream.CanWrite)
          {
            this._sourceStream = new FileStream(this._sourceStream.Name, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            leaveOpen = false;
          }
          if (num != this._sourceStream.Length)
            this._sourceStream.SetLength(num);
          this._file = MmapModule.CreateFromFile(this._sourceStream, this._mapName, this._sourceStream.Length, this._fileAccess, HandleInheritability.None, leaveOpen);
          this._view = this._file.CreateViewAccessor(this._offset, newsize, this._fileAccess);
        }
        catch
        {
          this.close();
          throw;
        }
      }
    }

    public object rfind([NotNull] string s)
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
        return this.RFindWorker(s, this.Position, this._view.Capacity);
    }

    public object rfind([NotNull] string s, long start)
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
        return this.RFindWorker(s, start, this._view.Capacity);
    }

    public object rfind([NotNull] string s, long start, long end)
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
        return this.RFindWorker(s, start, end);
    }

    private object RFindWorker(string s, long start, long end)
    {
      ContractUtils.RequiresNotNull((object) s, nameof (s));
      start = PythonOps.FixSliceIndex(start, this._view.Capacity);
      end = PythonOps.FixSliceIndex(end, this._view.Capacity);
      if (s == "")
        return start > end ? (object) -1 : MmapModule.MmapDefault.ReturnLong(start);
      long count1 = end - start;
      if ((long) s.Length > count1)
        return (object) -1;
      int num1 = -1;
      int count2 = Math.Max(s.Length, MmapModule.PAGESIZE);
      CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
      if (count1 <= (long) (count2 * 2))
      {
        byte[] numArray = new byte[count1];
        long num2 = (long) this._view.ReadArray<byte>(start, numArray, 0, (int) count1);
        string source = ((IList<byte>) numArray).MakeString();
        num1 = compareInfo.LastIndexOf(source, s, CompareOptions.Ordinal);
      }
      else
      {
        byte[] numArray1 = new byte[count2];
        byte[] numArray2 = new byte[count2];
        int count3 = (int) ((end - start) % (long) count2);
        if (count3 == 0)
          count3 = count2;
        start = end - (long) count2 - (long) count3;
        long num3 = count1 - (long) (count2 + count3);
        this._view.ReadArray<byte>(start, numArray1, 0, count2);
        int length1 = this._view.ReadArray<byte>(start + (long) count2, numArray2, 0, count3);
        for (; num3 >= 0L; num3 -= (long) length1)
        {
          string source = MmapModule.MmapDefault.GetString(numArray1, numArray2, length1);
          num1 = compareInfo.LastIndexOf(source, s, CompareOptions.Ordinal);
          if (num1 != -1)
            return MmapModule.MmapDefault.ReturnLong((long) num1 + start);
          byte[] numArray3 = numArray1;
          numArray1 = numArray2;
          numArray2 = numArray3;
          start -= (long) count2;
          length1 = this._view.ReadArray<byte>(start, numArray1, 0, count2);
        }
      }
      return num1 != -1 ? MmapModule.MmapDefault.ReturnLong((long) num1 + start) : (object) -1;
    }

    public void seek(long pos, int whence = 0)
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
      {
        switch (whence)
        {
          case 0:
            this.CheckSeekIndex(pos);
            this.Position = pos;
            break;
          case 1:
            checked { pos += this.Position; }
            goto case 0;
          case 2:
            checked { pos += this._view.Capacity; }
            goto case 0;
          default:
            throw PythonOps.ValueError("unknown seek type");
        }
      }
    }

    public object size()
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
        return this._sourceStream == null ? MmapModule.MmapDefault.ReturnLong(this._view.Capacity) : MmapModule.MmapDefault.ReturnLong(new FileInfo(this._sourceStream.Name).Length);
    }

    public object tell()
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
        return MmapModule.MmapDefault.ReturnLong(this.Position);
    }

    public void write(string s)
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
      {
        this.EnsureWritable();
        long position = this.Position;
        if (this._view.Capacity - position < (long) s.Length)
          throw PythonOps.ValueError("data out of range");
        byte[] array = s.MakeByteArray();
        this._view.WriteArray<byte>(position, array, 0, s.Length);
        this.Position = position + (long) s.Length;
      }
    }

    public void write_byte(string s)
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
      {
        if (s.Length != 1)
          throw PythonOps.TypeError("write_byte() argument 1 must be char, not str");
        this.EnsureWritable();
        long position = this.Position;
        if (this.Position >= this._view.Capacity)
          throw PythonOps.ValueError("write byte out of range");
        this._view.Write(position, (byte) s[0]);
        this.Position = position + 1L;
      }
    }

    private long Position
    {
      get => Interlocked.Read(ref this._position);
      set => Interlocked.Exchange(ref this._position, value);
    }

    private void EnsureWritable()
    {
      if (this._fileAccess == MemoryMappedFileAccess.Read)
        throw PythonOps.TypeError("mmap can't modify a read-only memory map.");
    }

    private void CheckIndex(long index) => this.CheckIndex(index, true);

    private void CheckIndex(long index, bool inclusive)
    {
      if (index > this._view.Capacity || index < 0L || inclusive && index == this._view.Capacity)
        throw PythonOps.IndexError("mmap index out of range");
    }

    private void CheckSeekIndex(long index)
    {
      if (index > this._view.Capacity || index < 0L)
        throw PythonOps.ValueError("seek out of range");
    }

    private static long? GetLong(object o)
    {
      switch (o)
      {
        case null:
          return new long?();
        case int num1:
          return new long?((long) num1);
        case BigInteger bigInteger:
          return new long?((long) bigInteger);
        case long num2:
          return new long?(num2);
        default:
          return new long?((long) Converter.ConvertToBigInteger(o));
      }
    }

    private static object ReturnLong(long l)
    {
      return l <= (long) int.MaxValue && l >= (long) int.MinValue ? (object) (int) l : (object) (BigInteger) l;
    }

    private static string GetString(byte[] buffer0, byte[] buffer1, int length1)
    {
      StringBuilder stringBuilder = new StringBuilder(buffer0.Length + length1);
      foreach (byte num in buffer0)
        stringBuilder.Append((char) num);
      for (int index = 0; index < length1; ++index)
        stringBuilder.Append((char) buffer1[index]);
      return stringBuilder.ToString();
    }

    internal string GetSearchString()
    {
      using (new MmapModule.MmapDefault.MmapLocker(this))
        return this[new IronPython.Runtime.Slice((object) 0, (object) null)];
    }

    private void EnsureOpen()
    {
      if (this._isClosed)
        throw PythonOps.ValueError("mmap closed or invalid");
    }

    private struct MmapLocker : IDisposable
    {
      private readonly MmapModule.MmapDefault _mmap;

      public MmapLocker(MmapModule.MmapDefault mmap)
      {
        this._mmap = mmap;
        Interlocked.Increment(ref this._mmap._refCount);
        this._mmap.EnsureOpen();
      }

      public void Dispose() => this._mmap.CloseWorker();
    }
  }

  private struct SYSTEM_INFO
  {
    internal int dwOemId;
    internal int dwPageSize;
    internal IntPtr lpMinimumApplicationAddress;
    internal IntPtr lpMaximumApplicationAddress;
    internal IntPtr dwActiveProcessorMask;
    internal int dwNumberOfProcessors;
    internal int dwProcessorType;
    internal int dwAllocationGranularity;
    internal short wProcessorLevel;
    internal short wProcessorRevision;
  }
}
