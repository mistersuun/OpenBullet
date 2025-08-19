// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.NotInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class NotInstruction : Instruction
{
  public static readonly Instruction Instance = (Instruction) new NotInstruction();

  private NotInstruction()
  {
  }

  public override int ConsumedStack => 1;

  public override int ProducedStack => 1;

  public override int Run(InterpretedFrame frame)
  {
    frame.Push((bool) frame.Pop() ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True);
    return 1;
  }
}
