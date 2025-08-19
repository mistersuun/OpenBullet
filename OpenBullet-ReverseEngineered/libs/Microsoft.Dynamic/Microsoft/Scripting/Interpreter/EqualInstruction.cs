// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.EqualInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal abstract class EqualInstruction : Instruction
{
  private static Instruction _Reference;
  private static Instruction _Boolean;
  private static Instruction _SByte;
  private static Instruction _Int16;
  private static Instruction _Char;
  private static Instruction _Int32;
  private static Instruction _Int64;
  private static Instruction _Byte;
  private static Instruction _UInt16;
  private static Instruction _UInt32;
  private static Instruction _UInt64;
  private static Instruction _Single;
  private static Instruction _Double;

  public override int ConsumedStack => 2;

  public override int ProducedStack => 1;

  private EqualInstruction()
  {
  }

  public static Instruction Create(Type type)
  {
    switch ((type.IsEnum() ? Enum.GetUnderlyingType(type) : type).GetTypeCode())
    {
      case TypeCode.Object:
        if (!type.IsValueType())
          return EqualInstruction._Reference ?? (EqualInstruction._Reference = (Instruction) new EqualInstruction.EqualReference());
        throw new NotImplementedException();
      case TypeCode.Boolean:
        return EqualInstruction._Boolean ?? (EqualInstruction._Boolean = (Instruction) new EqualInstruction.EqualBoolean());
      case TypeCode.Char:
        return EqualInstruction._Char ?? (EqualInstruction._Char = (Instruction) new EqualInstruction.EqualChar());
      case TypeCode.SByte:
        return EqualInstruction._SByte ?? (EqualInstruction._SByte = (Instruction) new EqualInstruction.EqualSByte());
      case TypeCode.Byte:
        return EqualInstruction._Byte ?? (EqualInstruction._Byte = (Instruction) new EqualInstruction.EqualByte());
      case TypeCode.Int16:
        return EqualInstruction._Int16 ?? (EqualInstruction._Int16 = (Instruction) new EqualInstruction.EqualInt16());
      case TypeCode.UInt16:
        return EqualInstruction._UInt16 ?? (EqualInstruction._UInt16 = (Instruction) new EqualInstruction.EqualInt16());
      case TypeCode.Int32:
        return EqualInstruction._Int32 ?? (EqualInstruction._Int32 = (Instruction) new EqualInstruction.EqualInt32());
      case TypeCode.UInt32:
        return EqualInstruction._UInt32 ?? (EqualInstruction._UInt32 = (Instruction) new EqualInstruction.EqualInt32());
      case TypeCode.Int64:
        return EqualInstruction._Int64 ?? (EqualInstruction._Int64 = (Instruction) new EqualInstruction.EqualInt64());
      case TypeCode.UInt64:
        return EqualInstruction._UInt64 ?? (EqualInstruction._UInt64 = (Instruction) new EqualInstruction.EqualInt64());
      case TypeCode.Single:
        return EqualInstruction._Single ?? (EqualInstruction._Single = (Instruction) new EqualInstruction.EqualSingle());
      case TypeCode.Double:
        return EqualInstruction._Double ?? (EqualInstruction._Double = (Instruction) new EqualInstruction.EqualDouble());
      default:
        throw new NotImplementedException();
    }
  }

  public override string ToString() => "Equal()";

  internal sealed class EqualBoolean : EqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((bool) frame.Pop() == (bool) frame.Pop());
      return 1;
    }
  }

  internal sealed class EqualSByte : EqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((int) (sbyte) frame.Pop() == (int) (sbyte) frame.Pop());
      return 1;
    }
  }

  internal sealed class EqualInt16 : EqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((int) (short) frame.Pop() == (int) (short) frame.Pop());
      return 1;
    }
  }

  internal sealed class EqualChar : EqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((int) (char) frame.Pop() == (int) (char) frame.Pop());
      return 1;
    }
  }

  internal sealed class EqualInt32 : EqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((int) frame.Pop() == (int) frame.Pop());
      return 1;
    }
  }

  internal sealed class EqualInt64 : EqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((long) frame.Pop() == (long) frame.Pop());
      return 1;
    }
  }

  internal sealed class EqualByte : EqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((int) (byte) frame.Pop() == (int) (byte) frame.Pop());
      return 1;
    }
  }

  internal sealed class EqualUInt16 : EqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((int) (ushort) frame.Pop() == (int) (ushort) frame.Pop());
      return 1;
    }
  }

  internal sealed class EqualUInt32 : EqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((int) (uint) frame.Pop() == (int) (uint) frame.Pop());
      return 1;
    }
  }

  internal sealed class EqualUInt64 : EqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((long) (ulong) frame.Pop() == (long) (ulong) frame.Pop());
      return 1;
    }
  }

  internal sealed class EqualSingle : EqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((double) (float) frame.Pop() == (double) (float) frame.Pop());
      return 1;
    }
  }

  internal sealed class EqualDouble : EqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((double) frame.Pop() == (double) frame.Pop());
      return 1;
    }
  }

  internal sealed class EqualReference : EqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push(frame.Pop() == frame.Pop());
      return 1;
    }
  }
}
