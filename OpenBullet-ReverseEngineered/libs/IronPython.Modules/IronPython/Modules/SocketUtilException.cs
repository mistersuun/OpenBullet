// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.SocketUtilException
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using System;

#nullable disable
namespace IronPython.Modules;

internal class SocketUtilException : Exception
{
  public SocketUtilException()
  {
  }

  public SocketUtilException(string message)
    : base(message)
  {
  }

  public SocketUtilException(string message, Exception inner)
    : base(message, inner)
  {
  }
}
