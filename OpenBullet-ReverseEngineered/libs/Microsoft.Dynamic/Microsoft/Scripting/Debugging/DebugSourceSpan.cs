// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.DebugSourceSpan
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Diagnostics;

#nullable disable
namespace Microsoft.Scripting.Debugging;

[DebuggerDisplay("{LineStart}, {ColumnStart} - {LineEnd}, {ColumnEnd}")]
internal sealed class DebugSourceSpan
{
  private readonly DebugSourceFile _sourceFile;
  private readonly int _lineStart;
  private readonly int _columnStart;
  private readonly int _lineEnd;
  private readonly int _columnEnd;

  internal DebugSourceSpan(
    DebugSourceFile sourceFile,
    int lineStart,
    int columnStart,
    int lineEnd,
    int columnEnd)
  {
    this._sourceFile = sourceFile;
    this._lineStart = lineStart;
    this._columnStart = columnStart;
    this._lineEnd = lineEnd;
    this._columnEnd = columnEnd;
  }

  internal DebugSourceSpan(DebugSourceFile sourceFile, SourceSpan dlrSpan)
  {
    DebugSourceFile sourceFile1 = sourceFile;
    SourceLocation sourceLocation = dlrSpan.Start;
    int line1 = sourceLocation.Line;
    sourceLocation = dlrSpan.Start;
    int column1 = sourceLocation.Column;
    sourceLocation = dlrSpan.End;
    int line2 = sourceLocation.Line;
    sourceLocation = dlrSpan.End;
    int column2 = sourceLocation.Column;
    // ISSUE: explicit constructor call
    this.\u002Ector(sourceFile1, line1, column1, line2, column2);
  }

  internal DebugSourceFile SourceFile => this._sourceFile;

  internal int LineStart => this._lineStart;

  internal int ColumnStart => this._columnStart;

  internal int LineEnd => this._lineEnd;

  internal int ColumnEnd => this._columnEnd;

  internal SourceSpan ToDlrSpan()
  {
    return new SourceSpan(new SourceLocation(0, this._lineStart, this._columnStart), new SourceLocation(0, this._lineEnd, this._columnEnd == -1 ? int.MaxValue : this._columnEnd));
  }

  internal bool Contains(DebugSourceSpan candidateSpan)
  {
    return candidateSpan._sourceFile == this._sourceFile && candidateSpan._lineStart >= this._lineStart && candidateSpan._lineEnd <= this._lineEnd && (candidateSpan._lineStart != this._lineStart || candidateSpan._columnStart >= this._columnStart) && (candidateSpan._lineEnd != this._lineEnd || candidateSpan._columnEnd <= this._columnEnd);
  }

  internal bool Intersects(DebugSourceSpan candidateSpan)
  {
    return candidateSpan._sourceFile == this._sourceFile && candidateSpan._lineEnd >= this._lineStart && candidateSpan._lineStart <= this._lineEnd && (candidateSpan._lineStart != this._lineEnd || candidateSpan._columnStart <= this._columnEnd) && (candidateSpan._lineEnd != this._lineStart || this._columnStart <= candidateSpan._columnEnd);
  }

  internal int GetSequencePointIndex(FunctionInfo funcInfo)
  {
    DebugSourceSpan[] sequencePoints = funcInfo.SequencePoints;
    for (int sequencePointIndex = 0; sequencePointIndex < sequencePoints.Length; ++sequencePointIndex)
    {
      if (this.Intersects(sequencePoints[sequencePointIndex]))
        return sequencePointIndex;
    }
    return int.MaxValue;
  }
}
