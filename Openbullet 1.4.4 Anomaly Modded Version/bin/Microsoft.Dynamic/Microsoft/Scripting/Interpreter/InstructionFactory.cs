// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.InstructionFactory
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

public abstract class InstructionFactory
{
  private static Dictionary<Type, InstructionFactory> _factories;

  internal static InstructionFactory GetFactory(Type type)
  {
    if (InstructionFactory._factories == null)
      InstructionFactory._factories = new Dictionary<Type, InstructionFactory>()
      {
        {
          typeof (object),
          InstructionFactory<object>.Factory
        },
        {
          typeof (bool),
          InstructionFactory<bool>.Factory
        },
        {
          typeof (byte),
          InstructionFactory<byte>.Factory
        },
        {
          typeof (sbyte),
          InstructionFactory<sbyte>.Factory
        },
        {
          typeof (short),
          InstructionFactory<short>.Factory
        },
        {
          typeof (ushort),
          InstructionFactory<ushort>.Factory
        },
        {
          typeof (int),
          InstructionFactory<int>.Factory
        },
        {
          typeof (uint),
          InstructionFactory<uint>.Factory
        },
        {
          typeof (long),
          InstructionFactory<long>.Factory
        },
        {
          typeof (ulong),
          InstructionFactory<ulong>.Factory
        },
        {
          typeof (float),
          InstructionFactory<float>.Factory
        },
        {
          typeof (double),
          InstructionFactory<double>.Factory
        },
        {
          typeof (char),
          InstructionFactory<char>.Factory
        },
        {
          typeof (string),
          InstructionFactory<string>.Factory
        },
        {
          typeof (BigInteger),
          InstructionFactory<BigInteger>.Factory
        }
      };
    lock (InstructionFactory._factories)
    {
      InstructionFactory factory;
      if (!InstructionFactory._factories.TryGetValue(type, out factory))
      {
        factory = (InstructionFactory) typeof (InstructionFactory<>).MakeGenericType(type).GetDeclaredField("Factory").GetValue((object) null);
        InstructionFactory._factories[type] = factory;
      }
      return factory;
    }
  }

  protected internal abstract Instruction GetArrayItem();

  protected internal abstract Instruction SetArrayItem();

  protected internal abstract Instruction TypeIs();

  protected internal abstract Instruction TypeAs();

  protected internal abstract Instruction DefaultValue();

  protected internal abstract Instruction NewArray();

  protected internal abstract Instruction NewArrayInit(int elementCount);
}
