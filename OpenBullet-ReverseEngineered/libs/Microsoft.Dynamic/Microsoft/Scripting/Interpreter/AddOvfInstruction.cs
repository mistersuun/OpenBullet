// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.AddOvfInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal abstract class AddOvfInstruction : Instruction
{
  private static Instruction _Int16;
  private static Instruction _Int32;
  private static Instruction _Int64;
  private static Instruction _UInt16;
  private static Instruction _UInt32;
  private static Instruction _UInt64;

  public override int ConsumedStack => 2;

  public override int ProducedStack => 1;

  private AddOvfInstruction()
  {
  }

  public static Instruction Create(Type type)
  {
    switch (type.GetNonNullableType().GetTypeCode())
    {
      case TypeCode.Int16:
        return AddOvfInstruction._Int16 ?? (AddOvfInstruction._Int16 = (Instruction) new AddOvfInstruction.AddOvfInt16());
      case TypeCode.UInt16:
        return AddOvfInstruction._UInt16 ?? (AddOvfInstruction._UInt16 = (Instruction) new AddOvfInstruction.AddOvfUInt16());
      case TypeCode.Int32:
        return AddOvfInstruction._Int32 ?? (AddOvfInstruction._Int32 = (Instruction) new AddOvfInstruction.AddOvfInt32());
      case TypeCode.UInt32:
        return AddOvfInstruction._UInt32 ?? (AddOvfInstruction._UInt32 = (Instruction) new AddOvfInstruction.AddOvfUInt32());
      case TypeCode.Int64:
        return AddOvfInstruction._Int64 ?? (AddOvfInstruction._Int64 = (Instruction) new AddOvfInstruction.AddOvfInt64());
      case TypeCode.UInt64:
        return AddOvfInstruction._UInt64 ?? (AddOvfInstruction._UInt64 = (Instruction) new AddOvfInstruction.AddOvfUInt64());
      default:
        return AddInstruction.Create(type);
    }
  }

  public override string ToString() => "AddOvf()";

  private sealed class AddOvfInt16 : AddOvfInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) checked ((short) ((int) (short) obj1 + (int) (short) obj2));
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class AddOvfInt32 : AddOvfInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : ScriptingRuntimeHelpers.Int32ToObject(checked ((int) obj1 + (int) obj2));
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class AddOvfInt64 : AddOvfInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) checked ((long) obj1 + (long) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class AddOvfUInt16 : AddOvfInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) checked ((ushort) ((int) (ushort) obj1 + (int) (ushort) obj2));
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class AddOvfUInt32 : AddOvfInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) checked ((uint) obj1 + (uint) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }

  private sealed class AddOvfUInt64 : AddOvfInstruction
  {
    public override int Run(InterpretedFrame frame)
    {
      int stackIndex = frame.StackIndex;
      object[] data = frame.Data;
      object obj1 = data[stackIndex - 2];
      if (obj1 != null)
      {
        object obj2 = data[stackIndex - 1];
        data[stackIndex - 2] = obj2 == null ? (object) null : (object) checked ((ulong) obj1 + (ulong) obj2);
      }
      frame.StackIndex = stackIndex - 1;
      return 1;
    }
  }
}
