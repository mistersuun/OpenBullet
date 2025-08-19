// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.EnterExceptionHandlerInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class EnterExceptionHandlerInstruction : Instruction
{
  internal static readonly EnterExceptionHandlerInstruction Void = new EnterExceptionHandlerInstruction(false);
  internal static readonly EnterExceptionHandlerInstruction NonVoid = new EnterExceptionHandlerInstruction(true);
  private readonly bool _hasValue;

  private EnterExceptionHandlerInstruction(bool hasValue) => this._hasValue = hasValue;

  public override int ConsumedStack => !this._hasValue ? 0 : 1;

  public override int ProducedStack => 1;

  public override int Run(InterpretedFrame frame) => 1;
}
