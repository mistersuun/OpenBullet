// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Generation.ReturnFixer
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Generation;

internal sealed class ReturnFixer
{
  private readonly LocalBuilder _refSlot;
  private readonly int _argIndex;
  private readonly Type _argType;

  private ReturnFixer(LocalBuilder refSlot, int argIndex, Type argType)
  {
    this._refSlot = refSlot;
    this._argIndex = argIndex;
    this._argType = argType;
  }

  internal static ReturnFixer EmitArgument(ILGen cg, int argIndex, Type argType)
  {
    cg.EmitLoadArg(argIndex);
    if (!argType.IsByRef)
    {
      cg.EmitBoxing(argType);
      return (ReturnFixer) null;
    }
    Type elementType = argType.GetElementType();
    cg.EmitLoadValueIndirect(elementType);
    Type type = typeof (StrongBox<>).MakeGenericType(elementType);
    cg.EmitNew(type, new Type[1]{ elementType });
    LocalBuilder localBuilder = cg.DeclareLocal(type);
    cg.Emit(OpCodes.Dup);
    cg.Emit(OpCodes.Stloc, localBuilder);
    return new ReturnFixer(localBuilder, argIndex, argType);
  }

  internal void FixReturn(ILGen cg)
  {
    cg.EmitLoadArg(this._argIndex);
    cg.Emit(OpCodes.Ldloc, this._refSlot);
    cg.Emit(OpCodes.Ldfld, this._refSlot.LocalType.GetDeclaredField("Value"));
    cg.EmitStoreValueIndirect(this._argType.GetElementType());
  }
}
