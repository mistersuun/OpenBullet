// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.FileStreamContentProvider
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;
using System.IO;

#nullable disable
namespace Microsoft.Scripting;

[Serializable]
internal sealed class FileStreamContentProvider : StreamContentProvider
{
  private readonly string _path;
  private readonly FileStreamContentProvider.PALHolder _pal;

  internal string Path => this._path;

  internal FileStreamContentProvider(PlatformAdaptationLayer pal, string path)
  {
    this._path = path;
    this._pal = new FileStreamContentProvider.PALHolder(pal);
  }

  public override Stream GetStream() => this._pal.GetStream(this.Path);

  [Serializable]
  private class PALHolder : MarshalByRefObject
  {
    [NonSerialized]
    private readonly PlatformAdaptationLayer _pal;

    internal PALHolder(PlatformAdaptationLayer pal) => this._pal = pal;

    internal Stream GetStream(string path) => this._pal.OpenInputFileStream(path);

    public override object InitializeLifetimeService() => (object) null;
  }
}
