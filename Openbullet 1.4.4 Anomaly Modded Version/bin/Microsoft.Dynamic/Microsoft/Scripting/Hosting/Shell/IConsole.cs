// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.Shell.IConsole
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.IO;

#nullable disable
namespace Microsoft.Scripting.Hosting.Shell;

public interface IConsole
{
  string ReadLine(int autoIndentSize);

  void Write(string text, Style style);

  void WriteLine(string text, Style style);

  void WriteLine();

  TextWriter Output { get; set; }

  TextWriter ErrorOutput { get; set; }
}
