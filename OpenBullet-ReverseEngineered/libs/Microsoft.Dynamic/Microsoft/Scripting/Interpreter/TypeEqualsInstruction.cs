// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.TypeEqualsInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class TypeEqualsInstruction : Instruction
{
  public static readonly TypeEqualsInstruction Instance = new TypeEqualsInstruction();

  public override int ConsumedStack => 2;

  public override int ProducedStack => 1;

  private TypeEqualsInstruction()
  {
  }

  public override int Run(InterpretedFrame frame)
  {
    object obj1 = frame.Pop();
    object obj2 = frame.Pop();
    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(obj2 != null && (object) obj2.GetType() == obj1));
    return 1;
  }

  public override string InstructionName => "TypeEquals()";
}
