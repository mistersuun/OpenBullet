// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.GreaterThanInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal abstract class GreaterThanInstruction : Instruction
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

  private GreaterThanInstruction()
  {
  }

  public static Instruction Create(Type type)
  {
    switch (type.GetTypeCode())
    {
      case TypeCode.Char:
        return GreaterThanInstruction._Char ?? (GreaterThanInstruction._Char = (Instruction) new GreaterThanInstruction.GreaterThanChar());
      case TypeCode.SByte:
        return GreaterThanInstruction._SByte ?? (GreaterThanInstruction._SByte = (Instruction) new GreaterThanInstruction.GreaterThanSByte());
      case TypeCode.Byte:
        return GreaterThanInstruction._Byte ?? (GreaterThanInstruction._Byte = (Instruction) new GreaterThanInstruction.GreaterThanByte());
      case TypeCode.Int16:
        return GreaterThanInstruction._Int16 ?? (GreaterThanInstruction._Int16 = (Instruction) new GreaterThanInstruction.GreaterThanInt16());
      case TypeCode.UInt16:
        return GreaterThanInstruction._UInt16 ?? (GreaterThanInstruction._UInt16 = (Instruction) new GreaterThanInstruction.GreaterThanUInt16());
      case TypeCode.Int32:
        return GreaterThanInstruction._Int32 ?? (GreaterThanInstruction._Int32 = (Instruction) new GreaterThanInstruction.GreaterThanInt32());
      case TypeCode.UInt32:
        return GreaterThanInstruction._UInt32 ?? (GreaterThanInstruction._UInt32 = (Instruction) new GreaterThanInstruction.GreaterThanUInt32());
      case TypeCode.Int64:
        return GreaterThanInstruction._Int64 ?? (GreaterThanInstruction._Int64 = (Instruction) new GreaterThanInstruction.GreaterThanInt64());
      case TypeCode.UInt64:
        return GreaterThanInstruction._UInt64 ?? (GreaterThanInstruction._UInt64 = (Instruction) new GreaterThanInstruction.GreaterThanUInt64());
      case TypeCode.Single:
        return GreaterThanInstruction._Single ?? (GreaterThanInstruction._Single = (Instruction) new GreaterThanInstruction.GreaterThanSingle());
      case TypeCode.Double:
        return GreaterThanInstruction._Double ?? (GreaterThanInstruction._Double = (Instruction) new GreaterThanInstruction.GreaterThanDouble());
      default:
        throw Assert.Unreachable;
    }
  }

  public override string ToString() => "GreaterThan()";

  internal sealed class GreaterThanSByte : GreaterThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      sbyte num = (sbyte) frame.Pop();
      frame.Push((int) (sbyte) frame.Pop() > (int) num);
      return 1;
    }
  }

  internal sealed class GreaterThanInt16 : GreaterThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      short num = (short) frame.Pop();
      frame.Push((int) (short) frame.Pop() > (int) num);
      return 1;
    }
  }

  internal sealed class GreaterThanChar : GreaterThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      char ch = (char) frame.Pop();
      frame.Push((int) (char) frame.Pop() > (int) ch);
      return 1;
    }
  }

  internal sealed class GreaterThanInt32 : GreaterThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int num = (int) frame.Pop();
      frame.Push((int) frame.Pop() > num);
      return 1;
    }
  }

  internal sealed class GreaterThanInt64 : GreaterThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      long num = (long) frame.Pop();
      frame.Push((long) frame.Pop() > num);
      return 1;
    }
  }

  internal sealed class GreaterThanByte : GreaterThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      byte num = (byte) frame.Pop();
      frame.Push((int) (byte) frame.Pop() > (int) num);
      return 1;
    }
  }

  internal sealed class GreaterThanUInt16 : GreaterThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      ushort num = (ushort) frame.Pop();
      frame.Push((int) (ushort) frame.Pop() > (int) num);
      return 1;
    }
  }

  internal sealed class GreaterThanUInt32 : GreaterThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      uint num = (uint) frame.Pop();
      frame.Push((uint) frame.Pop() > num);
      return 1;
    }
  }

  internal sealed class GreaterThanUInt64 : GreaterThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      ulong num = (ulong) frame.Pop();
      frame.Push((ulong) frame.Pop() > num);
      return 1;
    }
  }

  internal sealed class GreaterThanSingle : GreaterThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      float num = (float) frame.Pop();
      frame.Push((double) (float) frame.Pop() > (double) num);
      return 1;
    }
  }

  internal sealed class GreaterThanDouble : GreaterThanInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      double num = (double) frame.Pop();
      frame.Push((double) frame.Pop() > num);
      return 1;
    }
  }
}
