// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.DivInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal abstract class DivInstruction : Instruction
{
  private static Instruction _Int16;
  private static Instruction _Int32;
  private static Instruction _Int64;
  private static Instruction _UInt16;
  private static Instruction _UInt32;
  private static Instruction _UInt64;
  private static Instruction _Single;
  private static Instruction _Double;

  public override int ConsumedStack => 2;

  public override int ProducedStack => 1;

  private DivInstruction()
  {
  }

  public static Instruction Create(Type type)
  {
    switch (type.GetNonNullableType().GetTypeCode())
    {
      case TypeCode.Int16:
        return DivInstruction._Int16 ?? (DivInstruction._Int16 = (Instruction) new DivInstruction.DivInt16());
      case TypeCode.UInt16:
        return DivInstruction._UInt16 ?? (DivInstruction._UInt16 = (Instruction) new DivInstruction.DivUInt16());
      case TypeCode.Int32:
        return DivInstruction._Int32 ?? (DivInstruction._Int32 = (Instruction) new DivInstruction.DivInt32());
      case TypeCode.UInt32:
        return DivInstruction._UInt32 ?? (DivInstruction._UInt32 = (Instruction) new DivInstruction.DivUInt32());
      case TypeCode.Int64:
        return DivInstruction._Int64 ?? (DivInstruction._Int64 = (Instruction) new DivInstruction.DivInt64());
      case TypeCode.UInt64:
        return DivInstruction._UInt64 ?? (DivInstruction._UInt64 = (Instruction) new DivInstruction.DivUInt64());
      case TypeCode.Single:
        return DivInstruction._Single ?? (DivInstruction._Single = (Instruction) new DivInstruction.DivSingle());
      case TypeCode.Double:
        return DivInstruction._Double ?? (DivInstruction._Double = (Instruction) new DivInstruction.DivDouble());
      default:
        throw Assert.Unreachable;
    }
  }

  public override string ToString() => "Div()";

  private sealed class DivInt16 : DivInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) (short) ((int) (short) obj1 / (int) (short) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class DivInt32 : DivInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : ScriptingRuntimeHelpers.Int32ToObject((int) obj1 / (int) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class DivInt64 : DivInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) ((long) obj1 / (long) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class DivUInt16 : DivInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) (ushort) ((uint) (ushort) obj1 / (uint) (ushort) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class DivUInt32 : DivInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) ((uint) obj1 / (uint) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class DivUInt64 : DivInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) ((ulong) obj1 / (ulong) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class DivSingle : DivInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) (float) ((double) (float) obj1 / (double) (float) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class DivDouble : DivInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) ((double) obj1 / (double) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }
}
