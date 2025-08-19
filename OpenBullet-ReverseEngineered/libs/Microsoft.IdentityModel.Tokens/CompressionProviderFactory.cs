// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.CompressionProviderFactory
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using System;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class CompressionProviderFactory
{
  private static CompressionProviderFactory _default;

  static CompressionProviderFactory()
  {
    CompressionProviderFactory.Default = new CompressionProviderFactory();
  }

  public CompressionProviderFactory()
  {
  }

  public CompressionProviderFactory(CompressionProviderFactory other)
  {
    this.CustomCompressionProvider = other != null ? other.CustomCompressionProvider : throw LogHelper.LogArgumentNullException(nameof (other));
  }

  public static CompressionProviderFactory Default
  {
    get => CompressionProviderFactory._default;
    set
    {
      CompressionProviderFactory._default = value ?? throw LogHelper.LogArgumentNullException(nameof (Default));
    }
  }

  public ICompressionProvider CustomCompressionProvider { get; set; }

  public virtual bool IsSupportedAlgorithm(string algorithm)
  {
    return this.CustomCompressionProvider != null && this.CustomCompressionProvider.IsSupportedAlgorithm(algorithm) || this.IsSupportedCompressionAlgorithm(algorithm);
  }

  private bool IsSupportedCompressionAlgorithm(string algorithm)
  {
    return "DEF".Equals(algorithm, StringComparison.Ordinal);
  }

  public ICompressionProvider CreateCompressionProvider(string algorithm)
  {
    if (string.IsNullOrEmpty(algorithm))
      throw LogHelper.LogArgumentNullException(nameof (algorithm));
    if (this.CustomCompressionProvider != null && this.CustomCompressionProvider.IsSupportedAlgorithm(algorithm))
      return this.CustomCompressionProvider;
    if (algorithm.Equals("DEF", StringComparison.Ordinal))
      return (ICompressionProvider) new DeflateCompressionProvider();
    throw LogHelper.LogExceptionMessage((Exception) new NotSupportedException(LogHelper.FormatInvariant("IDX10652: The algorithm '{0}' is not supported.", (object) algorithm)));
  }
}
