// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.DeflateCompressionProvider
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class DeflateCompressionProvider : ICompressionProvider
{
  public DeflateCompressionProvider()
  {
  }

  public DeflateCompressionProvider(CompressionLevel compressionLevel)
  {
    this.CompressionLevel = compressionLevel;
  }

  public string Algorithm => "DEF";

  public CompressionLevel CompressionLevel { get; private set; }

  public byte[] Decompress(byte[] value)
  {
    if (value == null)
      throw LogHelper.LogArgumentNullException(nameof (value));
    using (MemoryStream memoryStream = new MemoryStream(value))
    {
      using (DeflateStream deflateStream = new DeflateStream((Stream) memoryStream, CompressionMode.Decompress))
      {
        using (StreamReader streamReader = new StreamReader((Stream) deflateStream, Encoding.UTF8))
          return Encoding.UTF8.GetBytes(streamReader.ReadToEnd());
      }
    }
  }

  public byte[] Compress(byte[] value)
  {
    if (value == null)
      throw LogHelper.LogArgumentNullException(nameof (value));
    using (MemoryStream memoryStream = new MemoryStream())
    {
      using (DeflateStream deflateStream = new DeflateStream((Stream) memoryStream, this.CompressionLevel))
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) deflateStream, Encoding.UTF8))
          streamWriter.Write(Encoding.UTF8.GetString(value));
      }
      return memoryStream.ToArray();
    }
  }

  public bool IsSupportedAlgorithm(string algorithm)
  {
    return this.Algorithm.Equals(algorithm, StringComparison.Ordinal);
  }
}
