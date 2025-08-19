// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.NumericConvertInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal abstract class NumericConvertInstruction : Instruction
{
  internal readonly TypeCode _from;
  internal readonly TypeCode _to;

  protected NumericConvertInstruction(TypeCode from, TypeCode to)
  {
    this._from = from;
    this._to = to;
  }

  public override int ConsumedStack => 1;

  public override int ProducedStack => 1;

  public override string ToString()
  {
    return $"{this.InstructionName}({(object) this._from}->{(object) this._to})";
  }

  public sealed class Unchecked(TypeCode from, TypeCode to) : NumericConvertInstruction(from, to)
  {
    public override string InstructionName => "UncheckedConvert";

    public override int Run(InterpretedFrame frame)
    {
      frame.Push(this.Convert(frame.Pop()));
      return 1;
    }

    private object Convert(object obj)
    {
      switch (this._from)
      {
        case TypeCode.Char:
          return this.ConvertInt32((int) (char) obj);
        case TypeCode.SByte:
          return this.ConvertInt32((int) (sbyte) obj);
        case TypeCode.Byte:
          return this.ConvertInt32((int) (byte) obj);
        case TypeCode.Int16:
          return this.ConvertInt32((int) (short) obj);
        case TypeCode.UInt16:
          return this.ConvertInt32((int) (ushort) obj);
        case TypeCode.Int32:
          return this.ConvertInt32((int) obj);
        case TypeCode.UInt32:
          return this.ConvertInt64((long) (uint) obj);
        case TypeCode.Int64:
          return this.ConvertInt64((long) obj);
        case TypeCode.UInt64:
          return this.ConvertUInt64((ulong) obj);
        case TypeCode.Single:
          return this.ConvertDouble((double) (float) obj);
        case TypeCode.Double:
          return this.ConvertDouble((double) obj);
        default:
          throw Assert.Unreachable;
      }
    }

    private object ConvertInt32(int obj)
    {
      switch (this._to)
      {
        case TypeCode.Char:
          return (object) (char) obj;
        case TypeCode.SByte:
          return (object) (sbyte) obj;
        case TypeCode.Byte:
          return (object) (byte) obj;
        case TypeCode.Int16:
          return (object) (short) obj;
        case TypeCode.UInt16:
          return (object) (ushort) obj;
        case TypeCode.Int32:
          return (object) obj;
        case TypeCode.UInt32:
          return (object) (uint) obj;
        case TypeCode.Int64:
          return (object) (long) obj;
        case TypeCode.UInt64:
          return (object) (ulong) obj;
        case TypeCode.Single:
          return (object) (float) obj;
        case TypeCode.Double:
          return (object) (double) obj;
        default:
          throw Assert.Unreachable;
      }
    }

    private object ConvertInt64(long obj)
    {
      switch (this._to)
      {
        case TypeCode.Char:
          return (object) (char) obj;
        case TypeCode.SByte:
          return (object) (sbyte) obj;
        case TypeCode.Byte:
          return (object) (byte) obj;
        case TypeCode.Int16:
          return (object) (short) obj;
        case TypeCode.UInt16:
          return (object) (ushort) obj;
        case TypeCode.Int32:
          return (object) (int) obj;
        case TypeCode.UInt32:
          return (object) (uint) obj;
        case TypeCode.Int64:
          return (object) obj;
        case TypeCode.UInt64:
          return (object) (ulong) obj;
        case TypeCode.Single:
          return (object) (float) obj;
        case TypeCode.Double:
          return (object) (double) obj;
        default:
          throw Assert.Unreachable;
      }
    }

    private object ConvertUInt64(ulong obj)
    {
      switch (this._to)
      {
        case TypeCode.Char:
          return (object) (char) obj;
        case TypeCode.SByte:
          return (object) (sbyte) obj;
        case TypeCode.Byte:
          return (object) (byte) obj;
        case TypeCode.Int16:
          return (object) (short) obj;
        case TypeCode.UInt16:
          return (object) (ushort) obj;
        case TypeCode.Int32:
          return (object) (int) obj;
        case TypeCode.UInt32:
          return (object) (uint) obj;
        case TypeCode.Int64:
          return (object) (long) obj;
        case TypeCode.UInt64:
          return (object) obj;
        case TypeCode.Single:
          return (object) (float) obj;
        case TypeCode.Double:
          return (object) (double) obj;
        default:
          throw Assert.Unreachable;
      }
    }

    private object ConvertDouble(double obj)
    {
      switch (this._to)
      {
        case TypeCode.Char:
          return (object) (char) obj;
        case TypeCode.SByte:
          return (object) (sbyte) obj;
        case TypeCode.Byte:
          return (object) (byte) obj;
        case TypeCode.Int16:
          return (object) (short) obj;
        case TypeCode.UInt16:
          return (object) (ushort) obj;
        case TypeCode.Int32:
          return (object) (int) obj;
        case TypeCode.UInt32:
          return (object) (uint) obj;
        case TypeCode.Int64:
          return (object) (long) obj;
        case TypeCode.UInt64:
          return (object) (ulong) obj;
        case TypeCode.Single:
          return (object) (float) obj;
        case TypeCode.Double:
          return (object) obj;
        default:
          throw Assert.Unreachable;
      }
    }
  }

  public sealed class Checked(TypeCode from, TypeCode to) : NumericConvertInstruction(from, to)
  {
    public override string InstructionName => "CheckedConvert";

    public override int Run(InterpretedFrame frame)
    {
      frame.Push(this.Convert(frame.Pop()));
      return 1;
    }

    private object Convert(object obj)
    {
      switch (this._from)
      {
        case TypeCode.Char:
          return this.ConvertInt32((int) (char) obj);
        case TypeCode.SByte:
          return this.ConvertInt32((int) (sbyte) obj);
        case TypeCode.Byte:
          return this.ConvertInt32((int) (byte) obj);
        case TypeCode.Int16:
          return this.ConvertInt32((int) (short) obj);
        case TypeCode.UInt16:
          return this.ConvertInt32((int) (ushort) obj);
        case TypeCode.Int32:
          return this.ConvertInt32((int) obj);
        case TypeCode.UInt32:
          return this.ConvertInt64((long) (uint) obj);
        case TypeCode.Int64:
          return this.ConvertInt64((long) obj);
        case TypeCode.UInt64:
          return this.ConvertUInt64((ulong) obj);
        case TypeCode.Single:
          return this.ConvertDouble((double) (float) obj);
        case TypeCode.Double:
          return this.ConvertDouble((double) obj);
        default:
          throw Assert.Unreachable;
      }
    }

    private object ConvertInt32(int obj)
    {
      switch (this._to)
      {
        case TypeCode.Char:
          return (object) checked ((char) obj);
        case TypeCode.SByte:
          return (object) checked ((sbyte) obj);
        case TypeCode.Byte:
          return (object) checked ((byte) obj);
        case TypeCode.Int16:
          return (object) checked ((short) obj);
        case TypeCode.UInt16:
          return (object) checked ((ushort) obj);
        case TypeCode.Int32:
          return (object) obj;
        case TypeCode.UInt32:
          return (object) checked ((uint) obj);
        case TypeCode.Int64:
          return (object) (long) obj;
        case TypeCode.UInt64:
          return (object) checked ((ulong) obj);
        case TypeCode.Single:
          return (object) (float) obj;
        case TypeCode.Double:
          return (object) (double) obj;
        default:
          throw Assert.Unreachable;
      }
    }

    private object ConvertInt64(long obj)
    {
      switch (this._to)
      {
        case TypeCode.Char:
          return (object) checked ((char) obj);
        case TypeCode.SByte:
          return (object) checked ((sbyte) obj);
        case TypeCode.Byte:
          return (object) checked ((byte) obj);
        case TypeCode.Int16:
          return (object) checked ((short) obj);
        case TypeCode.UInt16:
          return (object) checked ((ushort) obj);
        case TypeCode.Int32:
          return (object) checked ((int) obj);
        case TypeCode.UInt32:
          return (object) checked ((uint) obj);
        case TypeCode.Int64:
          return (object) obj;
        case TypeCode.UInt64:
          return (object) checked ((ulong) obj);
        case TypeCode.Single:
          return (object) (float) obj;
        case TypeCode.Double:
          return (object) (double) obj;
        default:
          throw Assert.Unreachable;
      }
    }

    private object ConvertUInt64(ulong obj)
    {
      switch (this._to)
      {
        case TypeCode.Char:
          return (object) checked ((char) obj);
        case TypeCode.SByte:
          return (object) checked ((sbyte) obj);
        case TypeCode.Byte:
          return (object) checked ((byte) obj);
        case TypeCode.Int16:
          return (object) checked ((short) obj);
        case TypeCode.UInt16:
          return (object) checked ((ushort) obj);
        case TypeCode.Int32:
          return (object) checked ((int) obj);
        case TypeCode.UInt32:
          return (object) checked ((uint) obj);
        case TypeCode.Int64:
          return (object) checked ((long) obj);
        case TypeCode.UInt64:
          return (object) obj;
        case TypeCode.Single:
          return (object) (float) obj;
        case TypeCode.Double:
          return (object) (double) obj;
        default:
          throw Assert.Unreachable;
      }
    }

    private object ConvertDouble(double obj)
    {
      switch (this._to)
      {
        case TypeCode.Char:
          return (object) checked ((char) obj);
        case TypeCode.SByte:
          return (object) checked ((sbyte) obj);
        case TypeCode.Byte:
          return (object) checked ((byte) obj);
        case TypeCode.Int16:
          return (object) checked ((short) obj);
        case TypeCode.UInt16:
          return (object) checked ((ushort) obj);
        case TypeCode.Int32:
          return (object) checked ((int) obj);
        case TypeCode.UInt32:
          return (object) checked ((uint) obj);
        case TypeCode.Int64:
          return (object) checked ((long) obj);
        case TypeCode.UInt64:
          return (object) checked ((ulong) obj);
        case TypeCode.Single:
          return (object) (float) obj;
        case TypeCode.Double:
          return (object) obj;
        default:
          throw Assert.Unreachable;
      }
    }
  }
}
