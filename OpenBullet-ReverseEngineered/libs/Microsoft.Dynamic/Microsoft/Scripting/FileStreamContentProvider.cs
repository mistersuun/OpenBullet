// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.FileStreamContentProvider
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

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
