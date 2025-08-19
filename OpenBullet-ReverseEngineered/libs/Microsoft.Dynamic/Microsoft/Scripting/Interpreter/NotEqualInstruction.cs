// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.NotEqualInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal abstract class NotEqualInstruction : Instruction
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

  private NotEqualInstruction()
  {
  }

  public static Instruction Create(Type type)
  {
    switch ((type.IsEnum() ? Enum.GetUnderlyingType(type) : type).GetTypeCode())
    {
      case TypeCode.Object:
        if (!type.IsValueType())
          return NotEqualInstruction._Reference ?? (NotEqualInstruction._Reference = (Instruction) new NotEqualInstruction.NotEqualReference());
        throw new NotImplementedException();
      case TypeCode.Boolean:
        return NotEqualInstruction._Boolean ?? (NotEqualInstruction._Boolean = (Instruction) new NotEqualInstruction.NotEqualBoolean());
      case TypeCode.Char:
        return NotEqualInstruction._Char ?? (NotEqualInstruction._Char = (Instruction) new NotEqualInstruction.NotEqualChar());
      case TypeCode.SByte:
        return NotEqualInstruction._SByte ?? (NotEqualInstruction._SByte = (Instruction) new NotEqualInstruction.NotEqualSByte());
      case TypeCode.Byte:
        return NotEqualInstruction._Byte ?? (NotEqualInstruction._Byte = (Instruction) new NotEqualInstruction.NotEqualByte());
      case TypeCode.Int16:
        return NotEqualInstruction._Int16 ?? (NotEqualInstruction._Int16 = (Instruction) new NotEqualInstruction.NotEqualInt16());
      case TypeCode.UInt16:
        return NotEqualInstruction._UInt16 ?? (NotEqualInstruction._UInt16 = (Instruction) new NotEqualInstruction.NotEqualInt16());
      case TypeCode.Int32:
        return NotEqualInstruction._Int32 ?? (NotEqualInstruction._Int32 = (Instruction) new NotEqualInstruction.NotEqualInt32());
      case TypeCode.UInt32:
        return NotEqualInstruction._UInt32 ?? (NotEqualInstruction._UInt32 = (Instruction) new NotEqualInstruction.NotEqualInt32());
      case TypeCode.Int64:
        return NotEqualInstruction._Int64 ?? (NotEqualInstruction._Int64 = (Instruction) new NotEqualInstruction.NotEqualInt64());
      case TypeCode.UInt64:
        return NotEqualInstruction._UInt64 ?? (NotEqualInstruction._UInt64 = (Instruction) new NotEqualInstruction.NotEqualInt64());
      case TypeCode.Single:
        return NotEqualInstruction._Single ?? (NotEqualInstruction._Single = (Instruction) new NotEqualInstruction.NotEqualSingle());
      case TypeCode.Double:
        return NotEqualInstruction._Double ?? (NotEqualInstruction._Double = (Instruction) new NotEqualInstruction.NotEqualDouble());
      default:
        throw new NotImplementedException();
    }
  }

  public override string ToString() => "NotEqual()";

  internal sealed class NotEqualBoolean : NotEqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((bool) frame.Pop() != (bool) frame.Pop());
      return 1;
    }
  }

  internal sealed class NotEqualSByte : NotEqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((int) (sbyte) frame.Pop() != (int) (sbyte) frame.Pop());
      return 1;
    }
  }

  internal sealed class NotEqualInt16 : NotEqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((int) (short) frame.Pop() != (int) (short) frame.Pop());
      return 1;
    }
  }

  internal sealed class NotEqualChar : NotEqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((int) (char) frame.Pop() != (int) (char) frame.Pop());
      return 1;
    }
  }

  internal sealed class NotEqualInt32 : NotEqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((int) frame.Pop() != (int) frame.Pop());
      return 1;
    }
  }

  internal sealed class NotEqualInt64 : NotEqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((long) frame.Pop() != (long) frame.Pop());
      return 1;
    }
  }

  internal sealed class NotEqualByte : NotEqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((int) (byte) frame.Pop() != (int) (byte) frame.Pop());
      return 1;
    }
  }

  internal sealed class NotEqualUInt16 : NotEqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((int) (ushort) frame.Pop() != (int) (ushort) frame.Pop());
      return 1;
    }
  }

  internal sealed class NotEqualUInt32 : NotEqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((int) (uint) frame.Pop() != (int) (uint) frame.Pop());
      return 1;
    }
  }

  internal sealed class NotEqualUInt64 : NotEqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((long) (ulong) frame.Pop() != (long) (ulong) frame.Pop());
      return 1;
    }
  }

  internal sealed class NotEqualSingle : NotEqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((double) (float) frame.Pop() != (double) (float) frame.Pop());
      return 1;
    }
  }

  internal sealed class NotEqualDouble : NotEqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push((double) frame.Pop() != (double) frame.Pop());
      return 1;
    }
  }

  internal sealed class NotEqualReference : NotEqualInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      frame.Push(frame.Pop() != frame.Pop());
      return 1;
    }
  }
}
