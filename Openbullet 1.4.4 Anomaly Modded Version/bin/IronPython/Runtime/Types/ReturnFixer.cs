// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.ReturnFixer
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Utils;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Types;

internal sealed class ReturnFixer
{
  private readonly ParameterInfo _parameter;
  private readonly LocalBuilder _reference;
  private readonly int _index;

  private ReturnFixer(LocalBuilder reference, ParameterInfo parameter, int index)
  {
    this._parameter = parameter;
    this._reference = reference;
    this._index = index;
  }

  public void FixReturn(ILGen il)
  {
    il.EmitLoadArg(this._index);
    il.Emit(OpCodes.Ldloc, this._reference);
    il.EmitFieldGet(this._reference.LocalType.GetDeclaredField("Value"));
    il.EmitStoreValueIndirect(this._parameter.ParameterType.GetElementType());
  }

  public static ReturnFixer EmitArgument(ILGen il, ParameterInfo parameter, int index)
  {
    il.EmitLoadArg(index);
    if (parameter.ParameterType.IsByRef)
    {
      Type elementType = parameter.ParameterType.GetElementType();
      Type localType = typeof (StrongBox<>).MakeGenericType(elementType);
      LocalBuilder localBuilder = il.DeclareLocal(localType);
      il.EmitLoadValueIndirect(elementType);
      ConstructorInfo constructor = localType.GetConstructor(new Type[1]
      {
        elementType
      });
      il.Emit(OpCodes.Newobj, constructor);
      il.Emit(OpCodes.Stloc, localBuilder);
      il.Emit(OpCodes.Ldloc, localBuilder);
      return new ReturnFixer(localBuilder, parameter, index);
    }
    il.EmitBoxing(parameter.ParameterType);
    return (ReturnFixer) null;
  }
}
