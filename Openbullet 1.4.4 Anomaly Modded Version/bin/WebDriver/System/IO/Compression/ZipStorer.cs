// Decompiled with JetBrains decompiler
// Type: System.IO.Compression.ZipStorer
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Collections.Generic;
using System.Text;

#nullable disable
namespace System.IO.Compression;

internal sealed class ZipStorer : IDisposable
{
  private static uint[] crcTable = ZipStorer.GenerateCrc32Table();
  private static Encoding defaultEncoding = Encoding.GetEncoding(437);
  private List<ZipStorer.ZipFileEntry> files = new List<ZipStorer.ZipFileEntry>();
  private Stream zipFileStream;
  private string comment = string.Empty;
  private byte[] centralDirectoryImage;
  private ushort existingFileCount;
  private FileAccess access;
  private bool encodeUtf8;
  private bool forceDeflating;

  public bool EncodeUtf8 => this.encodeUtf8;

  public bool ForceDeflating => this.forceDeflating;

  public static ZipStorer Create(Stream zipStream, string fileComment)
  {
    return new ZipStorer()
    {
      comment = fileComment,
      zipFileStream = zipStream,
      access = FileAccess.Write
    };
  }

  public static ZipStorer Open(Stream stream, FileAccess access)
  {
    if (!stream.CanSeek && access != FileAccess.Read)
      throw new InvalidOperationException("Stream cannot seek");
    ZipStorer zipStorer = new ZipStorer();
    zipStorer.zipFileStream = stream;
    zipStorer.access = access;
    return zipStorer.ReadFileInfo() ? zipStorer : throw new InvalidDataException();
  }

  public void AddFile(
    ZipStorer.CompressionMethod compressionMethod,
    string sourceFile,
    string fileNameInZip,
    string fileEntryComment)
  {
    if (this.access == FileAccess.Read)
      throw new InvalidOperationException("Writing is not allowed");
    using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
      this.AddStream(compressionMethod, (Stream) sourceStream, fileNameInZip, File.GetLastWriteTime(sourceFile), fileEntryComment);
  }

  public void AddStream(
    ZipStorer.CompressionMethod compressionMethod,
    Stream sourceStream,
    string fileNameInZip,
    DateTime modificationTimeStamp,
    string fileEntryComment)
  {
    if (this.access == FileAccess.Read)
      throw new InvalidOperationException("Writing is not allowed");
    ZipStorer.ZipFileEntry zipFileEntry = new ZipStorer.ZipFileEntry();
    zipFileEntry.Method = compressionMethod;
    zipFileEntry.EncodeUTF8 = this.EncodeUtf8;
    zipFileEntry.FilenameInZip = ZipStorer.NormalizeFileName(fileNameInZip);
    zipFileEntry.Comment = fileEntryComment == null ? string.Empty : fileEntryComment;
    zipFileEntry.Crc32 = 0U;
    zipFileEntry.HeaderOffset = (uint) this.zipFileStream.Position;
    zipFileEntry.ModifyTime = modificationTimeStamp;
    this.WriteLocalHeader(ref zipFileEntry);
    zipFileEntry.FileOffset = (uint) this.zipFileStream.Position;
    this.Store(ref zipFileEntry, sourceStream);
    sourceStream.Close();
    this.UpdateCrcAndSizes(ref zipFileEntry);
    this.files.Add(zipFileEntry);
  }

  public void Close()
  {
    if (this.access != FileAccess.Read)
    {
      uint position1 = (uint) this.zipFileStream.Position;
      uint size = 0;
      if (this.centralDirectoryImage != null)
        this.zipFileStream.Write(this.centralDirectoryImage, 0, this.centralDirectoryImage.Length);
      for (int index = 0; index < this.files.Count; ++index)
      {
        long position2 = this.zipFileStream.Position;
        this.WriteCentralDirRecord(this.files[index]);
        size += (uint) (this.zipFileStream.Position - position2);
      }
      if (this.centralDirectoryImage != null)
        this.WriteEndRecord(size + (uint) this.centralDirectoryImage.Length, position1);
      else
        this.WriteEndRecord(size, position1);
    }
    if (this.zipFileStream == null)
      return;
    this.zipFileStream.Flush();
    this.zipFileStream.Dispose();
    this.zipFileStream = (Stream) null;
  }

  public List<ZipStorer.ZipFileEntry> ReadCentralDirectory()
  {
    if (this.centralDirectoryImage == null)
      throw new InvalidOperationException("Central directory currently does not exist");
    List<ZipStorer.ZipFileEntry> zipFileEntryList = new List<ZipStorer.ZipFileEntry>();
    ushort uint16_1;
    ushort uint16_2;
    ushort uint16_3;
    for (int startIndex = 0; startIndex < this.centralDirectoryImage.Length && BitConverter.ToUInt32(this.centralDirectoryImage, startIndex) == 33639248U; startIndex += 46 + (int) uint16_1 + (int) uint16_2 + (int) uint16_3)
    {
      int num1 = ((uint) BitConverter.ToUInt16(this.centralDirectoryImage, startIndex + 8) & 2048U /*0x0800*/) > 0U ? 1 : 0;
      ushort uint16_4 = BitConverter.ToUInt16(this.centralDirectoryImage, startIndex + 10);
      uint uint32_1 = BitConverter.ToUInt32(this.centralDirectoryImage, startIndex + 12);
      uint uint32_2 = BitConverter.ToUInt32(this.centralDirectoryImage, startIndex + 16 /*0x10*/);
      uint uint32_3 = BitConverter.ToUInt32(this.centralDirectoryImage, startIndex + 20);
      uint uint32_4 = BitConverter.ToUInt32(this.centralDirectoryImage, startIndex + 24);
      uint16_1 = BitConverter.ToUInt16(this.centralDirectoryImage, startIndex + 28);
      uint16_2 = BitConverter.ToUInt16(this.centralDirectoryImage, startIndex + 30);
      uint16_3 = BitConverter.ToUInt16(this.centralDirectoryImage, startIndex + 32 /*0x20*/);
      uint uint32_5 = BitConverter.ToUInt32(this.centralDirectoryImage, startIndex + 42);
      uint num2 = 46U + (uint) uint16_1 + (uint) uint16_2 + (uint) uint16_3;
      Encoding encoding = num1 != 0 ? Encoding.UTF8 : ZipStorer.defaultEncoding;
      ZipStorer.ZipFileEntry zipFileEntry = new ZipStorer.ZipFileEntry();
      zipFileEntry.Method = (ZipStorer.CompressionMethod) uint16_4;
      zipFileEntry.FilenameInZip = encoding.GetString(this.centralDirectoryImage, startIndex + 46, (int) uint16_1);
      zipFileEntry.FileOffset = this.GetFileOffset(uint32_5);
      zipFileEntry.FileSize = uint32_4;
      zipFileEntry.CompressedSize = uint32_3;
      zipFileEntry.HeaderOffset = uint32_5;
      zipFileEntry.HeaderSize = num2;
      zipFileEntry.Crc32 = uint32_2;
      zipFileEntry.ModifyTime = ZipStorer.DosTimeToDateTime(uint32_1);
      if (uint16_3 > (ushort) 0)
        zipFileEntry.Comment = encoding.GetString(this.centralDirectoryImage, startIndex + 46 + (int) uint16_1 + (int) uint16_2, (int) uint16_3);
      zipFileEntryList.Add(zipFileEntry);
    }
    return zipFileEntryList;
  }

  public bool ExtractFile(ZipStorer.ZipFileEntry zipFileEntry, string destinationFileName)
  {
    string directoryName = Path.GetDirectoryName(destinationFileName);
    if (!Directory.Exists(directoryName))
      Directory.CreateDirectory(directoryName);
    if (Directory.Exists(destinationFileName))
      return true;
    bool file = false;
    using (Stream destinationStream = (Stream) new FileStream(destinationFileName, FileMode.Create, FileAccess.Write))
      file = this.ExtractFile(zipFileEntry, destinationStream);
    File.SetCreationTime(destinationFileName, zipFileEntry.ModifyTime);
    File.SetLastWriteTime(destinationFileName, zipFileEntry.ModifyTime);
    return file;
  }

  public bool ExtractFile(ZipStorer.ZipFileEntry zipFileEntry, Stream destinationStream)
  {
    if (!destinationStream.CanWrite)
      throw new InvalidOperationException("Stream cannot be written");
    byte[] buffer1 = new byte[4];
    this.zipFileStream.Seek((long) zipFileEntry.HeaderOffset, SeekOrigin.Begin);
    this.zipFileStream.Read(buffer1, 0, 4);
    if (BitConverter.ToUInt32(buffer1, 0) != 67324752U)
      return false;
    Stream stream;
    if (zipFileEntry.Method == ZipStorer.CompressionMethod.Store)
    {
      stream = this.zipFileStream;
    }
    else
    {
      if (zipFileEntry.Method != ZipStorer.CompressionMethod.Deflate)
        return false;
      stream = (Stream) new DeflateStream(this.zipFileStream, CompressionMode.Decompress, true);
    }
    byte[] buffer2 = new byte[16384 /*0x4000*/];
    this.zipFileStream.Seek((long) zipFileEntry.FileOffset, SeekOrigin.Begin);
    int count;
    for (uint fileSize = zipFileEntry.FileSize; fileSize > 0U; fileSize -= (uint) count)
    {
      count = stream.Read(buffer2, 0, (int) Math.Min((long) fileSize, (long) buffer2.Length));
      destinationStream.Write(buffer2, 0, count);
    }
    destinationStream.Flush();
    if (zipFileEntry.Method == ZipStorer.CompressionMethod.Deflate)
      stream.Dispose();
    return true;
  }

  public void Dispose() => this.Close();

  private static uint[] GenerateCrc32Table()
  {
    uint[] crc32Table = new uint[256 /*0x0100*/];
    for (int index1 = 0; index1 < crc32Table.Length; ++index1)
    {
      uint num = (uint) index1;
      for (int index2 = 0; index2 < 8; ++index2)
      {
        if (((int) num & 1) != 0)
          num = 3988292384U ^ num >> 1;
        else
          num >>= 1;
      }
      crc32Table[index1] = num;
    }
    return crc32Table;
  }

  private static uint DateTimeToDosTime(DateTime dateTime)
  {
    return (uint) (dateTime.Second / 2 | dateTime.Minute << 5 | dateTime.Hour << 11 | dateTime.Day << 16 /*0x10*/ | dateTime.Month << 21 | dateTime.Year - 1980 << 25);
  }

  private static DateTime DosTimeToDateTime(uint dosTime)
  {
    return new DateTime((int) (dosTime >> 25) + 1980, (int) (dosTime >> 21) & 15, (int) (dosTime >> 16 /*0x10*/) & 31 /*0x1F*/, (int) (dosTime >> 11) & 31 /*0x1F*/, (int) (dosTime >> 5) & 63 /*0x3F*/, ((int) dosTime & 31 /*0x1F*/) * 2);
  }

  private static string NormalizeFileName(string fileNameToNormalize)
  {
    string str = fileNameToNormalize.Replace('\\', '/');
    int num = str.IndexOf(':');
    if (num >= 0)
      str = str.Remove(0, num + 1);
    return str.Trim('/');
  }

  private uint GetFileOffset(uint headerOffset)
  {
    byte[] buffer = new byte[2];
    this.zipFileStream.Seek((long) (headerOffset + 26U), SeekOrigin.Begin);
    this.zipFileStream.Read(buffer, 0, 2);
    ushort uint16_1 = BitConverter.ToUInt16(buffer, 0);
    this.zipFileStream.Read(buffer, 0, 2);
    ushort uint16_2 = BitConverter.ToUInt16(buffer, 0);
    return (uint) ((ulong) (30 + (int) uint16_1 + (int) uint16_2) + (ulong) headerOffset);
  }

  private void WriteLocalHeader(ref ZipStorer.ZipFileEntry zipFileEntry)
  {
    long position = this.zipFileStream.Position;
    byte[] bytes = (zipFileEntry.EncodeUTF8 ? Encoding.UTF8 : ZipStorer.defaultEncoding).GetBytes(zipFileEntry.FilenameInZip);
    this.zipFileStream.Write(new byte[6]
    {
      (byte) 80 /*0x50*/,
      (byte) 75,
      (byte) 3,
      (byte) 4,
      (byte) 20,
      (byte) 0
    }, 0, 6);
    this.zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.EncodeUTF8 ? (ushort) 2048 /*0x0800*/ : (ushort) 0), 0, 2);
    this.zipFileStream.Write(BitConverter.GetBytes((ushort) zipFileEntry.Method), 0, 2);
    this.zipFileStream.Write(BitConverter.GetBytes(ZipStorer.DateTimeToDosTime(zipFileEntry.ModifyTime)), 0, 4);
    this.zipFileStream.Write(new byte[12], 0, 12);
    this.zipFileStream.Write(BitConverter.GetBytes((ushort) bytes.Length), 0, 2);
    this.zipFileStream.Write(BitConverter.GetBytes((ushort) 0), 0, 2);
    this.zipFileStream.Write(bytes, 0, bytes.Length);
    zipFileEntry.HeaderSize = (uint) (this.zipFileStream.Position - position);
  }

  private void WriteCentralDirRecord(ZipStorer.ZipFileEntry zipFileEntry)
  {
    Encoding encoding = zipFileEntry.EncodeUTF8 ? Encoding.UTF8 : ZipStorer.defaultEncoding;
    byte[] bytes1 = encoding.GetBytes(zipFileEntry.FilenameInZip);
    byte[] bytes2 = encoding.GetBytes(zipFileEntry.Comment);
    this.zipFileStream.Write(new byte[8]
    {
      (byte) 80 /*0x50*/,
      (byte) 75,
      (byte) 1,
      (byte) 2,
      (byte) 23,
      (byte) 11,
      (byte) 20,
      (byte) 0
    }, 0, 8);
    this.zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.EncodeUTF8 ? (ushort) 2048 /*0x0800*/ : (ushort) 0), 0, 2);
    this.zipFileStream.Write(BitConverter.GetBytes((ushort) zipFileEntry.Method), 0, 2);
    this.zipFileStream.Write(BitConverter.GetBytes(ZipStorer.DateTimeToDosTime(zipFileEntry.ModifyTime)), 0, 4);
    this.zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.Crc32), 0, 4);
    this.zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.CompressedSize), 0, 4);
    this.zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.FileSize), 0, 4);
    this.zipFileStream.Write(BitConverter.GetBytes((ushort) bytes1.Length), 0, 2);
    this.zipFileStream.Write(BitConverter.GetBytes((ushort) 0), 0, 2);
    this.zipFileStream.Write(BitConverter.GetBytes((ushort) bytes2.Length), 0, 2);
    this.zipFileStream.Write(BitConverter.GetBytes((ushort) 0), 0, 2);
    this.zipFileStream.Write(BitConverter.GetBytes((ushort) 0), 0, 2);
    this.zipFileStream.Write(BitConverter.GetBytes((ushort) 0), 0, 2);
    this.zipFileStream.Write(BitConverter.GetBytes((ushort) 33024), 0, 2);
    this.zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.HeaderOffset), 0, 4);
    this.zipFileStream.Write(bytes1, 0, bytes1.Length);
    this.zipFileStream.Write(bytes2, 0, bytes2.Length);
  }

  private void WriteEndRecord(uint size, uint offset)
  {
    byte[] bytes = (this.EncodeUtf8 ? Encoding.UTF8 : ZipStorer.defaultEncoding).GetBytes(this.comment);
    this.zipFileStream.Write(new byte[8]
    {
      (byte) 80 /*0x50*/,
      (byte) 75,
      (byte) 5,
      (byte) 6,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0
    }, 0, 8);
    this.zipFileStream.Write(BitConverter.GetBytes((int) (ushort) this.files.Count + (int) this.existingFileCount), 0, 2);
    this.zipFileStream.Write(BitConverter.GetBytes((int) (ushort) this.files.Count + (int) this.existingFileCount), 0, 2);
    this.zipFileStream.Write(BitConverter.GetBytes(size), 0, 4);
    this.zipFileStream.Write(BitConverter.GetBytes(offset), 0, 4);
    this.zipFileStream.Write(BitConverter.GetBytes((ushort) bytes.Length), 0, 2);
    this.zipFileStream.Write(bytes, 0, bytes.Length);
  }

  private void Store(ref ZipStorer.ZipFileEntry zipFileEntry, Stream sourceStream)
  {
    byte[] buffer = new byte[16384 /*0x4000*/];
    uint num = 0;
    long position1 = this.zipFileStream.Position;
    long position2 = sourceStream.Position;
    Stream stream = zipFileEntry.Method != ZipStorer.CompressionMethod.Store ? (Stream) new DeflateStream(this.zipFileStream, CompressionMode.Compress, true) : this.zipFileStream;
    zipFileEntry.Crc32 = uint.MaxValue;
    int count;
    do
    {
      count = sourceStream.Read(buffer, 0, buffer.Length);
      num += (uint) count;
      if (count > 0)
      {
        stream.Write(buffer, 0, count);
        for (uint index = 0; (long) index < (long) count; ++index)
          zipFileEntry.Crc32 = ZipStorer.crcTable[((int) zipFileEntry.Crc32 ^ (int) buffer[(int) index]) & (int) byte.MaxValue] ^ zipFileEntry.Crc32 >> 8;
      }
    }
    while (count == buffer.Length);
    stream.Flush();
    if (zipFileEntry.Method == ZipStorer.CompressionMethod.Deflate)
      stream.Dispose();
    zipFileEntry.Crc32 ^= uint.MaxValue;
    zipFileEntry.FileSize = num;
    zipFileEntry.CompressedSize = (uint) (this.zipFileStream.Position - position1);
    if (zipFileEntry.Method != ZipStorer.CompressionMethod.Deflate || this.ForceDeflating || !sourceStream.CanSeek || zipFileEntry.CompressedSize <= zipFileEntry.FileSize)
      return;
    zipFileEntry.Method = ZipStorer.CompressionMethod.Store;
    this.zipFileStream.Position = position1;
    this.zipFileStream.SetLength(position1);
    sourceStream.Position = position2;
    this.Store(ref zipFileEntry, sourceStream);
  }

  private void UpdateCrcAndSizes(ref ZipStorer.ZipFileEntry zipFileEntry)
  {
    long position = this.zipFileStream.Position;
    this.zipFileStream.Position = (long) (zipFileEntry.HeaderOffset + 8U);
    this.zipFileStream.Write(BitConverter.GetBytes((ushort) zipFileEntry.Method), 0, 2);
    this.zipFileStream.Position = (long) (zipFileEntry.HeaderOffset + 14U);
    this.zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.Crc32), 0, 4);
    this.zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.CompressedSize), 0, 4);
    this.zipFileStream.Write(BitConverter.GetBytes(zipFileEntry.FileSize), 0, 4);
    this.zipFileStream.Position = position;
  }

  private bool ReadFileInfo()
  {
    if (this.zipFileStream.Length < 22L)
      return false;
    try
    {
      this.zipFileStream.Seek(-17L, SeekOrigin.End);
      BinaryReader binaryReader = new BinaryReader(this.zipFileStream);
      do
      {
        this.zipFileStream.Seek(-5L, SeekOrigin.Current);
        if (binaryReader.ReadUInt32() == 101010256U)
        {
          this.zipFileStream.Seek(6L, SeekOrigin.Current);
          ushort num = binaryReader.ReadUInt16();
          int count = binaryReader.ReadInt32();
          uint offset = binaryReader.ReadUInt32();
          if (this.zipFileStream.Position + (long) binaryReader.ReadUInt16() != this.zipFileStream.Length)
            return false;
          this.existingFileCount = num;
          this.centralDirectoryImage = new byte[count];
          this.zipFileStream.Seek((long) offset, SeekOrigin.Begin);
          this.zipFileStream.Read(this.centralDirectoryImage, 0, count);
          this.zipFileStream.Seek((long) offset, SeekOrigin.Begin);
          return true;
        }
      }
      while (this.zipFileStream.Position > 0L);
    }
    catch (IOException ex)
    {
    }
    return false;
  }

  public enum CompressionMethod : ushort
  {
    Store = 0,
    Deflate = 8,
  }

  public struct ZipFileEntry
  {
    public ZipStorer.CompressionMethod Method;
    public string FilenameInZip;
    public uint FileSize;
    public uint CompressedSize;
    public uint HeaderOffset;
    public uint FileOffset;
    public uint HeaderSize;
    public uint Crc32;
    public DateTime ModifyTime;
    public string Comment;
    public bool EncodeUTF8;

    public override string ToString() => this.FilenameInZip;
  }
}
