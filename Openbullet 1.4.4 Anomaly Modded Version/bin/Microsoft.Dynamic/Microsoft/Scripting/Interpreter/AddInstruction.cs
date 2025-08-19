// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.AddInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal abstract class AddInstruction : Instruction
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

  private AddInstruction()
  {
  }

  public static Instruction Create(Type type)
  {
    switch (type.GetNonNullableType().GetTypeCode())
    {
      case TypeCode.Int16:
        return AddInstruction._Int16 ?? (AddInstruction._Int16 = (Instruction) new AddInstruction.AddInt16());
      case TypeCode.UInt16:
        return AddInstruction._UInt16 ?? (AddInstruction._UInt16 = (Instruction) new AddInstruction.AddUInt16());
      case TypeCode.Int32:
        return AddInstruction._Int32 ?? (AddInstruction._Int32 = (Instruction) new AddInstruction.AddInt32());
      case TypeCode.UInt32:
        return AddInstruction._UInt32 ?? (AddInstruction._UInt32 = (Instruction) new AddInstruction.AddUInt32());
      case TypeCode.Int64:
        return AddInstruction._Int64 ?? (AddInstruction._Int64 = (Instruction) new AddInstruction.AddInt64());
      case TypeCode.UInt64:
        return AddInstruction._UInt64 ?? (AddInstruction._UInt64 = (Instruction) new AddInstruction.AddUInt64());
      case TypeCode.Single:
        return AddInstruction._Single ?? (AddInstruction._Single = (Instruction) new AddInstruction.AddSingle());
      case TypeCode.Double:
        return AddInstruction._Double ?? (AddInstruction._Double = (Instruction) new AddInstruction.AddDouble());
      default:
        throw Assert.Unreachable;
    }
  }

  public override string ToString() => "Add()";

  private sealed class AddInt16 : AddInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) (short) ((int) (short) obj1 + (int) (short) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class AddInt32 : AddInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : ScriptingRuntimeHelpers.Int32ToObject((int) obj1 + (int) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class AddInt64 : AddInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) ((long) obj1 + (long) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class AddUInt16 : AddInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) (ushort) ((uint) (ushort) obj1 + (uint) (ushort) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class AddUInt32 : AddInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) (uint) ((int) (uint) obj1 + (int) (uint) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class AddUInt64 : AddInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) (ulong) ((long) (ulong) obj1 + (long) (ulong) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class AddSingle : AddInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) (float) ((double) (float) obj1 + (double) (float) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class AddDouble : AddInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) ((double) obj1 + (double) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }
}
