// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.FileContent
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.IO;

#nullable disable
namespace Leaf.xNet;

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
        this.ContentStream = (Stream) new FileStream(pathToContent, FileMode.Open, FileAccess.Read);
        this.BufferSize = bufferSize;
        this.InitialStreamPosition = 0L;
        this.MimeContentType = Http.DetermineMediaType(Path.GetExtension(pathToContent));
        break;
    }
  }
}
