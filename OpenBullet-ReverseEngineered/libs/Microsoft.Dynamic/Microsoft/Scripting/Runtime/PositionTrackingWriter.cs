// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.PositionTrackingWriter
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public class PositionTrackingWriter : StringWriter
{
  private List<KeyValuePair<int, int>> _lineMap = new List<KeyValuePair<int, int>>();
  private List<KeyValuePair<int, string>> _fileMap = new List<KeyValuePair<int, string>>();
  private int _line = 1;
  private int _column = 1;

  public void MapLocation(CodeLinePragma linePragma)
  {
    this._lineMap.Add(new KeyValuePair<int, int>(this._line, linePragma.LineNumber));
    this._fileMap.Add(new KeyValuePair<int, string>(this._line, linePragma.FileName));
  }

  public KeyValuePair<int, int>[] GetLineMap() => this._lineMap.ToArray();

  public KeyValuePair<int, string>[] GetFileMap() => this._fileMap.ToArray();

  public override void Write(char value)
  {
    if (value != '\n')
    {
      ++this._column;
    }
    else
    {
      this._column = 1;
      ++this._line;
    }
    base.Write(value);
  }

  public override void Write(string value)
  {
    this.UpdateLineColumn(value);
    base.Write(value);
  }

  public override void Write(char[] buffer, int index, int count)
  {
    this.UpdateLineColumn(buffer, index, count);
    base.Write(buffer, index, count);
  }

  private void UpdateLineColumn(string value)
  {
    int startIndex;
    int num;
    for (startIndex = 0; (num = 1 + value.IndexOf('\n', startIndex)) > 0; startIndex = num)
      ++this._line;
    if (startIndex > 0)
      this._column = value.Length - startIndex + 1;
    else
      this._column += value.Length;
  }

  private void UpdateLineColumn(char[] buffer, int index, int count)
  {
    int num1 = index + count;
    int startIndex;
    int num2;
    for (startIndex = index; (num2 = 1 + Array.IndexOf<char>(buffer, '\n', startIndex, num1 - startIndex)) > 0; startIndex = num2)
      ++this._line;
    if (startIndex > 0)
      this._column = count - startIndex + 1;
    else
      this._column += count;
  }
}
