// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.SourceFileInformation
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace Microsoft.Scripting;

public sealed class SourceFileInformation
{
  public SourceFileInformation(string fileName) => this.FileName = fileName;

  public SourceFileInformation(string fileName, Guid language)
  {
    this.FileName = fileName;
    this.LanguageGuid = language;
  }

  public SourceFileInformation(string fileName, Guid language, Guid vendor)
  {
    this.FileName = fileName;
    this.LanguageGuid = language;
    this.VendorGuid = vendor;
  }

  public string FileName { get; }

  public Guid LanguageGuid { get; }

  public Guid VendorGuid { get; }
}
