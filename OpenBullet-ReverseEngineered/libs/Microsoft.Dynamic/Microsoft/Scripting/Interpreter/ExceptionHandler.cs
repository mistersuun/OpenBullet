// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.ExceptionHandler
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

public sealed class ExceptionHandler
{
  public readonly Type ExceptionType;
  public readonly int StartIndex;
  public readonly int EndIndex;
  public readonly int LabelIndex;
  public readonly int HandlerStartIndex;

  public bool IsFault => this.ExceptionType == (Type) null;

  internal ExceptionHandler(
    int start,
    int end,
    int labelIndex,
    int handlerStartIndex,
    Type exceptionType)
  {
    this.StartIndex = start;
    this.EndIndex = end;
    this.LabelIndex = labelIndex;
    this.ExceptionType = exceptionType;
    this.HandlerStartIndex = handlerStartIndex;
  }

  public bool Matches(Type exceptionType, int index)
  {
    return index >= this.StartIndex && index < this.EndIndex && (this.ExceptionType == (Type) null || this.ExceptionType.IsAssignableFrom(exceptionType));
  }

  public bool IsBetterThan(ExceptionHandler other)
  {
    if (other == null)
      return true;
    if (this.StartIndex == other.StartIndex && this.EndIndex == other.EndIndex)
      return this.HandlerStartIndex < other.HandlerStartIndex;
    return this.StartIndex > other.StartIndex || this.EndIndex < other.EndIndex;
  }

  internal bool IsInside(int index) => index >= this.StartIndex && index < this.EndIndex;

  public override string ToString()
  {
    return $"{(this.IsFault ? (object) "fault" : (object) $"catch({this.ExceptionType.Name})")} [{this.StartIndex}-{this.EndIndex}] [{this.HandlerStartIndex}->]";
  }
}
