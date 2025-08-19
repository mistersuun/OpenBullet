// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.LessThanInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

public abstract class LessThanInstruction : Instruction
{
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

  private LessThanInstruction()
  {
  }

  public static Instruction Create(Type type)
  {
    switch (type.GetTypeCode())
    {
      case TypeCode.Char:
        return LessThanInstruction._Char ?? (LessThanInstruction._Char = (Instruction) new LessThanInstruction.LessThanChar());
      case TypeCode.SByte:
        return LessThanInstruction._SByte ?? (LessThanInstruction._SByte = (Instruction) new LessThanInstruction.LessThanSByte());
      case TypeCode.Byte:
        return LessThanInstruction._Byte ?? (LessThanInstruction._Byte = (Instruction) new LessThanInstruction.LessThanByte());
      case TypeCode.Int16:
        return LessThanInstruction._Int16 ?? (LessThanInstruction._Int16 = (Instruction) new LessThanInstruction.LessThanInt16());
      case TypeCode.UInt16:
        return LessThanInstruction._UInt16 ?? (LessThanInstruction._UInt16 = (Instruction) new LessThanInstruction.LessThanUInt16());
      case TypeCode.Int32:
        return LessThanInstruction._Int32 ?? (LessThanInstruction._Int32 = (Instruction) new LessThanInstruction.LessThanInt32());
      case TypeCode.UInt32:
        return LessThanInstruction._UInt32 ?? (LessThanInstruction._UInt32 = (Instruction) new LessThanInstruction.LessThanUInt32());
      case TypeCode.Int64:
        return LessThanInstruction._Int64 ?? (LessThanInstruction._Int64 = (Instruction) new LessThanInstruction.LessThanInt64());
      case TypeCode.UInt64:
        return LessThanInstruction._UInt64 ?? (LessThanInstruction._UInt64 = (Instruction) new LessThanInstruction.LessThanUInt64());
      case TypeCode.Single:
        return LessThanInstruction._Single ?? (LessThanInstruction._Single = (Instruction) new LessThanInstruction.LessThanSingle());
      case TypeCode.Double:
        return LessThanInstruction._Double ?? (LessThanInstruction._Double = (Instruction) new LessThanInstruction.LessThanDouble());
      default:
        throw Assert.Unreachable;
    }
  }

  public override string ToString() => "LessThan()";

  internal sealed class LessThanSByte : LessThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      sbyte num = (sbyte) frame.Pop();
      frame.Push((int) (sbyte) frame.Pop() < (int) num);
      return 1;
    }
  }

  internal sealed class LessThanInt16 : LessThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      short num = (short) frame.Pop();
      frame.Push((int) (short) frame.Pop() < (int) num);
      return 1;
    }
  }

  internal sealed class LessThanChar : LessThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      char ch = (char) frame.Pop();
      frame.Push((int) (char) frame.Pop() < (int) ch);
      return 1;
    }
  }

  internal sealed class LessThanInt32 : LessThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int num = (int) frame.Pop();
      frame.Push((int) frame.Pop() < num);
      return 1;
    }
  }

  internal sealed class LessThanInt64 : LessThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      long num = (long) frame.Pop();
      frame.Push((long) frame.Pop() < num);
      return 1;
    }
  }

  internal sealed class LessThanByte : LessThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      byte num = (byte) frame.Pop();
      frame.Push((int) (byte) frame.Pop() < (int) num);
      return 1;
    }
  }

  internal sealed class LessThanUInt16 : LessThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      ushort num = (ushort) frame.Pop();
      frame.Push((int) (ushort) frame.Pop() < (int) num);
      return 1;
    }
  }

  internal sealed class LessThanUInt32 : LessThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      uint num = (uint) frame.Pop();
      frame.Push((uint) frame.Pop() < num);
      return 1;
    }
  }

  internal sealed class LessThanUInt64 : LessThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      ulong num = (ulong) frame.Pop();
      frame.Push((ulong) frame.Pop() < num);
      return 1;
    }
  }

  internal sealed class LessThanSingle : LessThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      float num = (float) frame.Pop();
      frame.Push((double) (float) frame.Pop() < (double) num);
      return 1;
    }
  }

  internal sealed class LessThanDouble : LessThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      double num = (double) frame.Pop();
      frame.Push((double) frame.Pop() < num);
      return 1;
    }
  }
}
