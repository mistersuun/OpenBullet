// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.InitializeLocalInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal abstract class InitializeLocalInstruction : LocalAccessInstruction
{
  internal InitializeLocalInstruction(int index)
    : base(index)
  {
  }

  internal sealed class Reference : InitializeLocalInstruction, IBoxableInstruction
  {
    internal Reference(int index)
      : base(index)
    {
    }

    public override int Run(InterpretedFrame frame)
    {
      frame.Data[this._index] = (object) null;
      return 1;
    }

    public Instruction BoxIfIndexMatches(int index)
    {
      return index != this._index ? (Instruction) null : InstructionList.InitImmutableRefBox(index);
    }

    public override string InstructionName => "InitRef";
  }

  internal sealed class ImmutableValue : InitializeLocalInstruction, IBoxableInstruction
  {
    private readonly object _defaultValue;

    internal ImmutableValue(int index, object defaultValue)
      : base(index)
    {
      this._defaultValue = defaultValue;
    }

    public override int Run(InterpretedFrame frame)
    {
      frame.Data[this._index] = this._defaultValue;
      return 1;
    }

    public Instruction BoxIfIndexMatches(int index)
    {
      return index != this._index ? (Instruction) null : (Instruction) new InitializeLocalInstruction.ImmutableBox(index, this._defaultValue);
    }

    public override string InstructionName => "InitImmutableValue";
  }

  internal sealed class ImmutableBox : InitializeLocalInstruction
  {
    private readonly object _defaultValue;

    internal ImmutableBox(int index, object defaultValue)
      : base(index)
    {
      this._defaultValue = defaultValue;
    }

    public override int Run(InterpretedFrame frame)
    {
      frame.Data[this._index] = (object) new StrongBox<object>(this._defaultValue);
      return 1;
    }

    public override string InstructionName => "InitImmutableBox";
  }

  internal sealed class ParameterBox(int index) : InitializeLocalInstruction(index)
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Data[this._index] = (object) new StrongBox<object>(frame.Data[this._index]);
      return 1;
    }
  }

  internal sealed class Parameter : InitializeLocalInstruction, IBoxableInstruction
  {
    internal Parameter(int index)
      : base(index)
    {
    }

    public override int Run(InterpretedFrame frame) => 1;

    public Instruction BoxIfIndexMatches(int index)
    {
      return index == this._index ? InstructionList.ParameterBox(index) : (Instruction) null;
    }

    public override string InstructionName => "InitParameter";
  }

  internal sealed class MutableValue : InitializeLocalInstruction, IBoxableInstruction
  {
    private readonly Type _type;

    internal MutableValue(int index, Type type)
      : base(index)
    {
      this._type = type;
    }

    public override int Run(InterpretedFrame frame)
    {
      try
      {
        frame.Data[this._index] = Activator.CreateInstance(this._type);
      }
      catch (TargetInvocationException ex)
      {
        ExceptionHelpers.UpdateForRethrow(ex.InnerException);
        throw ex.InnerException;
      }
      return 1;
    }

    public Instruction BoxIfIndexMatches(int index)
    {
      return index != this._index ? (Instruction) null : (Instruction) new InitializeLocalInstruction.MutableBox(index, this._type);
    }

    public override string InstructionName => "InitMutableValue";
  }

  internal sealed class MutableBox : InitializeLocalInstruction
  {
    private readonly Type _type;

    internal MutableBox(int index, Type type)
      : base(index)
    {
      this._type = type;
    }

    public override int Run(InterpretedFrame frame)
    {
      frame.Data[this._index] = (object) new StrongBox<object>(Activator.CreateInstance(this._type));
      return 1;
    }

    public override string InstructionName => "InitMutableBox";
  }
}
