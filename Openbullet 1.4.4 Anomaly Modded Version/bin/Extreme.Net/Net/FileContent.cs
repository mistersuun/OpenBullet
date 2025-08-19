// Decompiled with JetBrains decompiler
// Type: Extreme.Net.FileContent
// Assembly: Extreme.Net, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B85A5720-FE8B-4A1B-9FD2-F7651D37B15B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Extreme.Net.dll

using System;
using System.IO;

#nullable disable
namespace Extreme.Net;

public class FileContent : StreamContent
{
  public FileContent(string pathToContent, int bufferSize = 32768 /*0x8000*/)
  {
    switch (pathToContent)
    {
      case null:
        throw new ArgumentNullException(nameof (pathToContent));
      case "":
        throw ExceptionHelper.EmptyString(nameof (pathToContent));
      default:
        if (bufferSize < 1)
          throw ExceptionHelper.CanNotBeLess<int>(nameof (bufferSize), 1);
        this._content = (Stream) new FileStream(pathToContent, FileMode.Open, FileAccess.Read);
        this._bufferSize = bufferSize;
        this._initialStreamPosition = 0L;
        this._contentType = Http.DetermineMediaType(Path.GetExtension(pathToContent));
        break;
    }
  }
}
