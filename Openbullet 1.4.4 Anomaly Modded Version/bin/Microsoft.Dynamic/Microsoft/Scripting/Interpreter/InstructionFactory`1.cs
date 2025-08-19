// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.InstructionFactory`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Interpreter;

public sealed class InstructionFactory<T> : InstructionFactory
{
  public static readonly InstructionFactory Factory = (InstructionFactory) new InstructionFactory<T>();
  private Instruction _getArrayItem;
  private Instruction _setArrayItem;
  private Instruction _typeIs;
  private Instruction _defaultValue;
  private Instruction _newArray;
  private Instruction _typeAs;

  private InstructionFactory()
  {
  }

  protected internal override Instruction GetArrayItem()
  {
    return this._getArrayItem ?? (this._getArrayItem = (Instruction) new GetArrayItemInstruction<T>());
  }

  protected internal override Instruction SetArrayItem()
  {
    return this._setArrayItem ?? (this._setArrayItem = (Instruction) new SetArrayItemInstruction<T>());
  }

  protected internal override Instruction TypeIs()
  {
    return this._typeIs ?? (this._typeIs = (Instruction) new TypeIsInstruction<T>());
  }

  protected internal override Instruction TypeAs()
  {
    return this._typeAs ?? (this._typeAs = (Instruction) new TypeAsInstruction<T>());
  }

  protected internal override Instruction DefaultValue()
  {
    return this._defaultValue ?? (this._defaultValue = (Instruction) new DefaultValueInstruction<T>());
  }

  protected internal override Instruction NewArray()
  {
    return this._newArray ?? (this._newArray = (Instruction) new NewArrayInstruction<T>());
  }

  protected internal override Instruction NewArrayInit(int elementCount)
  {
    return (Instruction) new NewArrayInitInstruction<T>(elementCount);
  }
}
