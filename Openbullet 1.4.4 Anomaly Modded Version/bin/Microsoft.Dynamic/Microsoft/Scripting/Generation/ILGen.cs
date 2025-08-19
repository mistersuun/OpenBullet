// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Generation.ILGen
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Scripting.Generation;

public class ILGen
{
  private readonly ILGenerator _ilg;
  private readonly KeyedQueue<Type, LocalBuilder> _freeLocals = new KeyedQueue<Type, LocalBuilder>();

  public ILGen(ILGenerator ilg)
  {
    ContractUtils.RequiresNotNull((object) ilg, nameof (ilg));
    this._ilg = ilg;
  }

  public virtual void BeginCatchBlock(Type exceptionType)
  {
    this._ilg.BeginCatchBlock(exceptionType);
  }

  public virtual void BeginExceptFilterBlock() => this._ilg.BeginExceptFilterBlock();

  public virtual Label BeginExceptionBlock() => this._ilg.BeginExceptionBlock();

  public virtual void BeginFaultBlock() => this._ilg.BeginFaultBlock();

  public virtual void BeginFinallyBlock() => this._ilg.BeginFinallyBlock();

  public virtual void EndExceptionBlock() => this._ilg.EndExceptionBlock();

  public virtual void BeginScope() => this._ilg.BeginScope();

  public virtual void EndScope() => this._ilg.EndScope();

  public virtual LocalBuilder DeclareLocal(Type localType) => this._ilg.DeclareLocal(localType);

  public virtual LocalBuilder DeclareLocal(Type localType, bool pinned)
  {
    return this._ilg.DeclareLocal(localType, pinned);
  }

  public virtual Label DefineLabel() => this._ilg.DefineLabel();

  public virtual void MarkLabel(Label loc) => this._ilg.MarkLabel(loc);

  public virtual void Emit(OpCode opcode) => this._ilg.Emit(opcode);

  public virtual void Emit(OpCode opcode, byte arg) => this._ilg.Emit(opcode, arg);

  public virtual void Emit(OpCode opcode, ConstructorInfo con) => this._ilg.Emit(opcode, con);

  public virtual void Emit(OpCode opcode, double arg) => this._ilg.Emit(opcode, arg);

  public virtual void Emit(OpCode opcode, FieldInfo field) => this._ilg.Emit(opcode, field);

  public virtual void Emit(OpCode opcode, float arg) => this._ilg.Emit(opcode, arg);

  public virtual void Emit(OpCode opcode, int arg) => this._ilg.Emit(opcode, arg);

  public virtual void Emit(OpCode opcode, Label label) => this._ilg.Emit(opcode, label);

  public virtual void Emit(OpCode opcode, Label[] labels) => this._ilg.Emit(opcode, labels);

  public virtual void Emit(OpCode opcode, LocalBuilder local) => this._ilg.Emit(opcode, local);

  public virtual void Emit(OpCode opcode, long arg) => this._ilg.Emit(opcode, arg);

  public virtual void Emit(OpCode opcode, MethodInfo meth) => this._ilg.Emit(opcode, meth);

  [CLSCompliant(false)]
  public virtual void Emit(OpCode opcode, sbyte arg) => this._ilg.Emit(opcode, arg);

  public virtual void Emit(OpCode opcode, short arg) => this._ilg.Emit(opcode, arg);

  public virtual void Emit(OpCode opcode, SignatureHelper signature)
  {
    this._ilg.Emit(opcode, signature);
  }

  public virtual void Emit(OpCode opcode, string str) => this._ilg.Emit(opcode, str);

  public virtual void Emit(OpCode opcode, Type cls) => this._ilg.Emit(opcode, cls);

  public virtual void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes)
  {
    this._ilg.EmitCall(opcode, methodInfo, optionalParameterTypes);
  }

  public virtual void EmitCalli(
    OpCode opcode,
    CallingConvention unmanagedCallConv,
    Type returnType,
    Type[] parameterTypes)
  {
    this._ilg.EmitCalli(opcode, unmanagedCallConv, returnType, parameterTypes);
  }

  public virtual void EmitCalli(
    OpCode opcode,
    CallingConventions callingConvention,
    Type returnType,
    Type[] parameterTypes,
    Type[] optionalParameterTypes)
  {
    this._ilg.EmitCalli(opcode, callingConvention, returnType, parameterTypes, optionalParameterTypes);
  }

  public virtual void MarkSequencePoint(
    ISymbolDocumentWriter document,
    int startLine,
    int startColumn,
    int endLine,
    int endColumn)
  {
    this._ilg.MarkSequencePoint(document, startLine, startColumn, endLine, endColumn);
  }

  public virtual void UsingNamespace(string usingNamespace)
  {
    this._ilg.UsingNamespace(usingNamespace);
  }

  [Conditional("DEBUG")]
  internal void EmitDebugWriteLine(string message)
  {
    this.EmitString(message);
    this.EmitCall(typeof (Debug), "WriteLine", new Type[1]
    {
      typeof (string)
    });
  }

  internal void Emit(OpCode opcode, MethodBase methodBase)
  {
    ConstructorInfo con = methodBase as ConstructorInfo;
    if (con != (ConstructorInfo) null)
      this.Emit(opcode, con);
    else
      this.Emit(opcode, (MethodInfo) methodBase);
  }

  public void EmitLoadArg(int index)
  {
    ContractUtils.Requires(index >= 0, nameof (index));
    switch (index)
    {
      case 0:
        this.Emit(OpCodes.Ldarg_0);
        break;
      case 1:
        this.Emit(OpCodes.Ldarg_1);
        break;
      case 2:
        this.Emit(OpCodes.Ldarg_2);
        break;
      case 3:
        this.Emit(OpCodes.Ldarg_3);
        break;
      default:
        if (index <= (int) byte.MaxValue)
        {
          this.Emit(OpCodes.Ldarg_S, (byte) index);
          break;
        }
        this.Emit(OpCodes.Ldarg, index);
        break;
    }
  }

  public void EmitLoadArgAddress(int index)
  {
    ContractUtils.Requires(index >= 0, nameof (index));
    if (index <= (int) byte.MaxValue)
      this.Emit(OpCodes.Ldarga_S, (byte) index);
    else
      this.Emit(OpCodes.Ldarga, index);
  }

  public void EmitStoreArg(int index)
  {
    ContractUtils.Requires(index >= 0, nameof (index));
    if (index <= (int) byte.MaxValue)
      this.Emit(OpCodes.Starg_S, (byte) index);
    else
      this.Emit(OpCodes.Starg, index);
  }

  public void EmitLoadValueIndirect(Type type)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    if (type.IsValueType())
    {
      if (type == typeof (int))
        this.Emit(OpCodes.Ldind_I4);
      else if (type == typeof (uint))
        this.Emit(OpCodes.Ldind_U4);
      else if (type == typeof (short))
        this.Emit(OpCodes.Ldind_I2);
      else if (type == typeof (ushort))
        this.Emit(OpCodes.Ldind_U2);
      else if (type == typeof (long) || type == typeof (ulong))
        this.Emit(OpCodes.Ldind_I8);
      else if (type == typeof (char))
        this.Emit(OpCodes.Ldind_I2);
      else if (type == typeof (bool))
        this.Emit(OpCodes.Ldind_I1);
      else if (type == typeof (float))
        this.Emit(OpCodes.Ldind_R4);
      else if (type == typeof (double))
        this.Emit(OpCodes.Ldind_R8);
      else
        this.Emit(OpCodes.Ldobj, type);
    }
    else if (type.IsGenericParameter)
      this.Emit(OpCodes.Ldobj, type);
    else
      this.Emit(OpCodes.Ldind_Ref);
  }

  public void EmitStoreValueIndirect(Type type)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    if (type.IsValueType())
    {
      if (type == typeof (int))
        this.Emit(OpCodes.Stind_I4);
      else if (type == typeof (short))
        this.Emit(OpCodes.Stind_I2);
      else if (type == typeof (long) || type == typeof (ulong))
        this.Emit(OpCodes.Stind_I8);
      else if (type == typeof (char))
        this.Emit(OpCodes.Stind_I2);
      else if (type == typeof (bool))
        this.Emit(OpCodes.Stind_I1);
      else if (type == typeof (float))
        this.Emit(OpCodes.Stind_R4);
      else if (type == typeof (double))
        this.Emit(OpCodes.Stind_R8);
      else
        this.Emit(OpCodes.Stobj, type);
    }
    else if (type.IsGenericParameter)
      this.Emit(OpCodes.Stobj, type);
    else
      this.Emit(OpCodes.Stind_Ref);
  }

  public void EmitLoadElement(Type type)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    if (!type.IsValueType())
      this.Emit(OpCodes.Ldelem_Ref);
    else if (type.IsEnum())
    {
      this.Emit(OpCodes.Ldelem, type);
    }
    else
    {
      switch (type.GetTypeCode())
      {
        case TypeCode.Boolean:
        case TypeCode.SByte:
          this.Emit(OpCodes.Ldelem_I1);
          break;
        case TypeCode.Char:
        case TypeCode.UInt16:
          this.Emit(OpCodes.Ldelem_U2);
          break;
        case TypeCode.Byte:
          this.Emit(OpCodes.Ldelem_U1);
          break;
        case TypeCode.Int16:
          this.Emit(OpCodes.Ldelem_I2);
          break;
        case TypeCode.Int32:
          this.Emit(OpCodes.Ldelem_I4);
          break;
        case TypeCode.UInt32:
          this.Emit(OpCodes.Ldelem_U4);
          break;
        case TypeCode.Int64:
        case TypeCode.UInt64:
          this.Emit(OpCodes.Ldelem_I8);
          break;
        case TypeCode.Single:
          this.Emit(OpCodes.Ldelem_R4);
          break;
        case TypeCode.Double:
          this.Emit(OpCodes.Ldelem_R8);
          break;
        default:
          this.Emit(OpCodes.Ldelem, type);
          break;
      }
    }
  }

  public void EmitStoreElement(Type type)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    if (type.IsEnum())
    {
      this.Emit(OpCodes.Stelem, type);
    }
    else
    {
      switch (type.GetTypeCode())
      {
        case TypeCode.Boolean:
        case TypeCode.SByte:
        case TypeCode.Byte:
          this.Emit(OpCodes.Stelem_I1);
          break;
        case TypeCode.Char:
        case TypeCode.Int16:
        case TypeCode.UInt16:
          this.Emit(OpCodes.Stelem_I2);
          break;
        case TypeCode.Int32:
        case TypeCode.UInt32:
          this.Emit(OpCodes.Stelem_I4);
          break;
        case TypeCode.Int64:
        case TypeCode.UInt64:
          this.Emit(OpCodes.Stelem_I8);
          break;
        case TypeCode.Single:
          this.Emit(OpCodes.Stelem_R4);
          break;
        case TypeCode.Double:
          this.Emit(OpCodes.Stelem_R8);
          break;
        default:
          if (type.IsValueType())
          {
            this.Emit(OpCodes.Stelem, type);
            break;
          }
          this.Emit(OpCodes.Stelem_Ref);
          break;
      }
    }
  }

  public void EmitType(Type type)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    this.Emit(OpCodes.Ldtoken, type);
    this.EmitCall(typeof (Type), "GetTypeFromHandle");
  }

  public void EmitUnbox(Type type)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    this.Emit(OpCodes.Unbox_Any, type);
  }

  public void EmitPropertyGet(PropertyInfo pi)
  {
    ContractUtils.RequiresNotNull((object) pi, nameof (pi));
    if (!pi.CanRead)
      throw Error.CantReadProperty();
    this.EmitCall(pi.GetGetMethod());
  }

  public void EmitPropertySet(PropertyInfo pi)
  {
    ContractUtils.RequiresNotNull((object) pi, nameof (pi));
    if (!pi.CanWrite)
      throw Error.CantWriteProperty();
    this.EmitCall(pi.GetSetMethod());
  }

  public void EmitFieldAddress(FieldInfo fi)
  {
    ContractUtils.RequiresNotNull((object) fi, nameof (fi));
    if (fi.IsStatic)
      this.Emit(OpCodes.Ldsflda, fi);
    else
      this.Emit(OpCodes.Ldflda, fi);
  }

  public void EmitFieldGet(FieldInfo fi)
  {
    ContractUtils.RequiresNotNull((object) fi, nameof (fi));
    if (fi.IsStatic)
      this.Emit(OpCodes.Ldsfld, fi);
    else
      this.Emit(OpCodes.Ldfld, fi);
  }

  public void EmitFieldSet(FieldInfo fi)
  {
    ContractUtils.RequiresNotNull((object) fi, nameof (fi));
    if (fi.IsStatic)
      this.Emit(OpCodes.Stsfld, fi);
    else
      this.Emit(OpCodes.Stfld, fi);
  }

  public void EmitNew(ConstructorInfo ci)
  {
    ContractUtils.RequiresNotNull((object) ci, nameof (ci));
    if (ci.DeclaringType.ContainsGenericParameters())
      throw Error.IllegalNew_GenericParams((object) ci.DeclaringType);
    this.Emit(OpCodes.Newobj, ci);
  }

  public void EmitNew(Type type, Type[] paramTypes)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    ContractUtils.RequiresNotNull((object) paramTypes, nameof (paramTypes));
    ConstructorInfo constructor = type.GetConstructor(paramTypes);
    ContractUtils.Requires(constructor != (ConstructorInfo) null, nameof (type), Strings.TypeDoesNotHaveConstructorForTheSignature);
    this.EmitNew(constructor);
  }

  public void EmitCall(MethodInfo mi)
  {
    ContractUtils.RequiresNotNull((object) mi, nameof (mi));
    if (mi.IsVirtual && !mi.DeclaringType.IsValueType())
      this.Emit(OpCodes.Callvirt, mi);
    else
      this.Emit(OpCodes.Call, mi);
  }

  public void EmitCall(Type type, string name)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    ContractUtils.RequiresNotNull((object) name, nameof (name));
    MethodInfo method = type.GetMethod(name);
    ContractUtils.Requires(method != (MethodInfo) null, nameof (type), Strings.TypeDoesNotHaveMethodForName);
    this.EmitCall(method);
  }

  public void EmitCall(Type type, string name, Type[] paramTypes)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    ContractUtils.RequiresNotNull((object) name, nameof (name));
    ContractUtils.RequiresNotNull((object) paramTypes, nameof (paramTypes));
    MethodInfo method = type.GetMethod(name, paramTypes);
    ContractUtils.Requires(method != (MethodInfo) null, nameof (type), Strings.TypeDoesNotHaveMethodForNameSignature);
    this.EmitCall(method);
  }

  public void EmitNull() => this.Emit(OpCodes.Ldnull);

  public void EmitString(string value)
  {
    ContractUtils.RequiresNotNull((object) value, nameof (value));
    this.Emit(OpCodes.Ldstr, value);
  }

  public void EmitBoolean(bool value)
  {
    if (value)
      this.Emit(OpCodes.Ldc_I4_1);
    else
      this.Emit(OpCodes.Ldc_I4_0);
  }

  public void EmitChar(char value)
  {
    this.EmitInt((int) value);
    this.Emit(OpCodes.Conv_U2);
  }

  public void EmitByte(byte value)
  {
    this.EmitInt((int) value);
    this.Emit(OpCodes.Conv_U1);
  }

  [CLSCompliant(false)]
  public void EmitSByte(sbyte value)
  {
    this.EmitInt((int) value);
    this.Emit(OpCodes.Conv_I1);
  }

  public void EmitShort(short value)
  {
    this.EmitInt((int) value);
    this.Emit(OpCodes.Conv_I2);
  }

  [CLSCompliant(false)]
  public void EmitUShort(ushort value)
  {
    this.EmitInt((int) value);
    this.Emit(OpCodes.Conv_U2);
  }

  public void EmitInt(int value)
  {
    OpCode opcode;
    switch (value)
    {
      case -1:
        opcode = OpCodes.Ldc_I4_M1;
        break;
      case 0:
        opcode = OpCodes.Ldc_I4_0;
        break;
      case 1:
        opcode = OpCodes.Ldc_I4_1;
        break;
      case 2:
        opcode = OpCodes.Ldc_I4_2;
        break;
      case 3:
        opcode = OpCodes.Ldc_I4_3;
        break;
      case 4:
        opcode = OpCodes.Ldc_I4_4;
        break;
      case 5:
        opcode = OpCodes.Ldc_I4_5;
        break;
      case 6:
        opcode = OpCodes.Ldc_I4_6;
        break;
      case 7:
        opcode = OpCodes.Ldc_I4_7;
        break;
      case 8:
        opcode = OpCodes.Ldc_I4_8;
        break;
      default:
        if (value >= (int) sbyte.MinValue && value <= (int) sbyte.MaxValue)
        {
          this.Emit(OpCodes.Ldc_I4_S, (sbyte) value);
          return;
        }
        this.Emit(OpCodes.Ldc_I4, value);
        return;
    }
    this.Emit(opcode);
  }

  [CLSCompliant(false)]
  public void EmitUInt(uint value)
  {
    this.EmitInt((int) value);
    this.Emit(OpCodes.Conv_U4);
  }

  public void EmitLong(long value) => this.Emit(OpCodes.Ldc_I8, value);

  [CLSCompliant(false)]
  public void EmitULong(ulong value)
  {
    this.Emit(OpCodes.Ldc_I8, (long) value);
    this.Emit(OpCodes.Conv_U8);
  }

  public void EmitDouble(double value) => this.Emit(OpCodes.Ldc_R8, value);

  public void EmitSingle(float value) => this.Emit(OpCodes.Ldc_R4, value);

  private void EmitSimpleConstant(object value)
  {
    object obj = value;
    Type type = value?.GetType();
    if ((object) type == null)
      type = typeof (object);
    if (!this.TryEmitConstant(obj, type))
      throw Error.CanotEmitConstant(value, (object) value.GetType());
  }

  internal bool TryEmitConstant(object value, Type type)
  {
    if (value == null)
    {
      this.EmitDefault(type);
      return true;
    }
    if (this.TryEmitILConstant(value, type))
      return true;
    Type type1 = value as Type;
    if (type1 != (Type) null && ILGen.ShouldLdtoken(type1))
    {
      this.EmitType(type1);
      return true;
    }
    MethodBase methodBase = value as MethodBase;
    if (!(methodBase != (MethodBase) null) || !ILGen.ShouldLdtoken(methodBase))
      return false;
    this.Emit(OpCodes.Ldtoken, methodBase);
    Type declaringType = methodBase.DeclaringType;
    if (declaringType != (Type) null && declaringType.IsGenericType())
    {
      this.Emit(OpCodes.Ldtoken, declaringType);
      this.EmitCall(typeof (MethodBase).GetMethod("GetMethodFromHandle", new Type[2]
      {
        typeof (RuntimeMethodHandle),
        typeof (RuntimeTypeHandle)
      }));
    }
    else
      this.EmitCall(typeof (MethodBase).GetMethod("GetMethodFromHandle", new Type[1]
      {
        typeof (RuntimeMethodHandle)
      }));
    type = TypeUtils.GetConstantType(type);
    if (type != typeof (MethodBase))
      this.Emit(OpCodes.Castclass, type);
    return true;
  }

  public static bool ShouldLdtoken(Type t)
  {
    return t is TypeBuilder || t.IsGenericParameter || t.IsVisible();
  }

  public static bool ShouldLdtoken(MethodBase mb)
  {
    if (mb is DynamicMethod)
      return false;
    Type declaringType = mb.DeclaringType;
    return declaringType == (Type) null || ILGen.ShouldLdtoken(declaringType);
  }

  private bool TryEmitILConstant(object value, Type type)
  {
    switch (type.GetTypeCode())
    {
      case TypeCode.Boolean:
        this.EmitBoolean((bool) value);
        return true;
      case TypeCode.Char:
        this.EmitChar((char) value);
        return true;
      case TypeCode.SByte:
        this.EmitSByte((sbyte) value);
        return true;
      case TypeCode.Byte:
        this.EmitByte((byte) value);
        return true;
      case TypeCode.Int16:
        this.EmitShort((short) value);
        return true;
      case TypeCode.UInt16:
        this.EmitUShort((ushort) value);
        return true;
      case TypeCode.Int32:
        this.EmitInt((int) value);
        return true;
      case TypeCode.UInt32:
        this.EmitUInt((uint) value);
        return true;
      case TypeCode.Int64:
        this.EmitLong((long) value);
        return true;
      case TypeCode.UInt64:
        this.EmitULong((ulong) value);
        return true;
      case TypeCode.Single:
        this.EmitSingle((float) value);
        return true;
      case TypeCode.Double:
        this.EmitDouble((double) value);
        return true;
      case TypeCode.Decimal:
        this.EmitDecimal((Decimal) value);
        return true;
      case TypeCode.String:
        this.EmitString((string) value);
        return true;
      default:
        return false;
    }
  }

  public void EmitImplicitCast(Type from, Type to)
  {
    if (!this.TryEmitCast(from, to, true))
      throw Error.NoImplicitCast((object) from, (object) to);
  }

  public void EmitExplicitCast(Type from, Type to)
  {
    if (!this.TryEmitCast(from, to, false))
      throw Error.NoExplicitCast((object) from, (object) to);
  }

  public bool TryEmitImplicitCast(Type from, Type to) => this.TryEmitCast(from, to, true);

  public bool TryEmitExplicitCast(Type from, Type to) => this.TryEmitCast(from, to, false);

  private bool TryEmitCast(Type from, Type to, bool implicitOnly)
  {
    ContractUtils.RequiresNotNull((object) from, nameof (from));
    ContractUtils.RequiresNotNull((object) to, nameof (to));
    if (from == to)
      return true;
    if (to.IsAssignableFrom(from))
    {
      if (to.IsNullableType())
      {
        Type nonNullableType = to.GetNonNullableType();
        if (!this.TryEmitCast(from, nonNullableType, true))
          return false;
        this.EmitNew(to.GetConstructor(new Type[1]
        {
          nonNullableType
        }));
      }
      if (from.IsValueType() && to == typeof (object))
      {
        this.EmitBoxing(from);
        return true;
      }
      if (to.IsInterface())
      {
        this.Emit(OpCodes.Box, from);
        return true;
      }
      if (!from.IsEnum() || !(to == typeof (Enum)))
        return true;
      this.Emit(OpCodes.Box, from);
      return true;
    }
    if (to == typeof (void))
    {
      this.Emit(OpCodes.Pop);
      return true;
    }
    if (to.IsValueType() && from == typeof (object))
    {
      if (implicitOnly)
        return false;
      this.Emit(OpCodes.Unbox_Any, to);
      return true;
    }
    if (to.IsValueType() != from.IsValueType())
      return false;
    if (!to.IsValueType())
    {
      if (implicitOnly)
        return false;
      this.Emit(OpCodes.Castclass, to);
      return true;
    }
    if (to.IsEnum())
      to = Enum.GetUnderlyingType(to);
    if (from.IsEnum())
      from = Enum.GetUnderlyingType(from);
    return to == from || this.EmitNumericCast(from, to, implicitOnly);
  }

  public bool EmitNumericCast(Type from, Type to, bool implicitOnly)
  {
    int typeCode1 = (int) from.GetTypeCode();
    TypeCode typeCode2 = to.GetTypeCode();
    int fromX;
    ref int local1 = ref fromX;
    int fromY;
    ref int local2 = ref fromY;
    int x;
    int y;
    if (!TypeUtils.GetNumericConversionOrder((TypeCode) typeCode1, out local1, out local2) || !TypeUtils.GetNumericConversionOrder(typeCode2, out x, out y))
      return false;
    bool flag = TypeUtils.IsImplicitlyConvertible(fromX, fromY, x, y);
    if (implicitOnly && !flag)
      return false;
    if (!flag || y == 2 || x == 2)
    {
      switch (typeCode2)
      {
        case TypeCode.SByte:
          this.Emit(OpCodes.Conv_I1);
          break;
        case TypeCode.Byte:
          this.Emit(OpCodes.Conv_U1);
          break;
        case TypeCode.Int16:
          this.Emit(OpCodes.Conv_I2);
          break;
        case TypeCode.UInt16:
          this.Emit(OpCodes.Conv_U1);
          break;
        case TypeCode.Int32:
          this.Emit(OpCodes.Conv_I4);
          break;
        case TypeCode.UInt32:
          this.Emit(OpCodes.Conv_U2);
          break;
        case TypeCode.Int64:
          this.Emit(OpCodes.Conv_I8);
          break;
        case TypeCode.UInt64:
          this.Emit(OpCodes.Conv_U4);
          break;
        case TypeCode.Single:
          this.Emit(OpCodes.Conv_R4);
          break;
        case TypeCode.Double:
          this.Emit(OpCodes.Conv_R8);
          break;
        default:
          throw Microsoft.Scripting.Utils.Assert.Unreachable;
      }
    }
    return true;
  }

  public void EmitBoxing(Type type)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    if (type.IsValueType())
    {
      if (type == typeof (void))
        this.Emit(OpCodes.Ldnull);
      else if (type == typeof (int))
        this.EmitCall(typeof (ScriptingRuntimeHelpers), "Int32ToObject");
      else if (type == typeof (bool))
      {
        Label label1 = this.DefineLabel();
        Label label2 = this.DefineLabel();
        this.Emit(OpCodes.Brtrue_S, label1);
        this.Emit(OpCodes.Ldsfld, typeof (ScriptingRuntimeHelpers).GetDeclaredField("False"));
        this.Emit(OpCodes.Br_S, label2);
        this.MarkLabel(label1);
        this.Emit(OpCodes.Ldsfld, typeof (ScriptingRuntimeHelpers).GetDeclaredField("True"));
        this.MarkLabel(label2);
      }
      else
        this.Emit(OpCodes.Box, type);
    }
    else
    {
      if (!type.IsGenericParameter)
        return;
      this.EmitCall(typeof (GeneratorOps).GetMethod("BoxGeneric").MakeGenericMethod(type));
    }
  }

  internal void EmitConvertToType(Type typeFrom, Type typeTo, bool isChecked)
  {
    if (typeFrom == typeof (DynamicNull))
      typeFrom = typeof (object);
    if (typeFrom == typeTo)
      return;
    if (typeFrom == typeof (void))
      this.EmitDefault(typeTo);
    else if (typeTo == typeof (void))
    {
      this.Emit(OpCodes.Pop);
    }
    else
    {
      bool flag1 = typeFrom.IsNullableType();
      bool flag2 = typeTo.IsNullableType();
      Type nonNullableType1 = typeFrom.GetNonNullableType();
      Type nonNullableType2 = typeTo.GetNonNullableType();
      if (typeFrom.IsInterface() || typeTo.IsInterface() || typeFrom == typeof (object) || typeTo == typeof (object))
        this.EmitCastToType(typeFrom, typeTo);
      else if (flag1 | flag2)
        this.EmitNullableConversion(typeFrom, typeTo, isChecked);
      else if ((!TypeUtils.IsConvertible(typeFrom) || !TypeUtils.IsConvertible(typeTo)) && (nonNullableType1.IsAssignableFrom(nonNullableType2) || nonNullableType2.IsAssignableFrom(nonNullableType1)))
        this.EmitCastToType(typeFrom, typeTo);
      else if (typeFrom.IsArray && typeTo.IsArray)
        this.EmitCastToType(typeFrom, typeTo);
      else
        this.EmitNumericConversion(typeFrom, typeTo, isChecked);
    }
  }

  private void EmitCastToType(Type typeFrom, Type typeTo)
  {
    if (!typeFrom.IsValueType() && typeTo.IsValueType())
      this.Emit(OpCodes.Unbox_Any, typeTo);
    else if (typeFrom.IsValueType() && !typeTo.IsValueType())
    {
      this.EmitBoxing(typeFrom);
      if (!(typeTo != typeof (object)))
        return;
      this.Emit(OpCodes.Castclass, typeTo);
    }
    else
    {
      if (typeFrom.IsValueType() || typeTo.IsValueType())
        throw Error.InvalidCast((object) typeFrom, (object) typeTo);
      this.Emit(OpCodes.Castclass, typeTo);
    }
  }

  private void EmitNumericConversion(Type typeFrom, Type typeTo, bool isChecked)
  {
    bool flag = typeFrom.IsUnsignedInt();
    if (typeTo == typeof (float))
    {
      if (flag)
        this.Emit(OpCodes.Conv_R_Un);
      this.Emit(OpCodes.Conv_R4);
    }
    else if (typeTo == typeof (double))
    {
      if (flag)
        this.Emit(OpCodes.Conv_R_Un);
      this.Emit(OpCodes.Conv_R8);
    }
    else
    {
      TypeCode typeCode = typeTo.GetTypeCode();
      if (isChecked)
      {
        if (flag)
        {
          switch (typeCode)
          {
            case TypeCode.Char:
            case TypeCode.UInt16:
              this.Emit(OpCodes.Conv_Ovf_U2_Un);
              break;
            case TypeCode.SByte:
              this.Emit(OpCodes.Conv_Ovf_I1_Un);
              break;
            case TypeCode.Byte:
              this.Emit(OpCodes.Conv_Ovf_U1_Un);
              break;
            case TypeCode.Int16:
              this.Emit(OpCodes.Conv_Ovf_I2_Un);
              break;
            case TypeCode.Int32:
              this.Emit(OpCodes.Conv_Ovf_I4_Un);
              break;
            case TypeCode.UInt32:
              this.Emit(OpCodes.Conv_Ovf_U4_Un);
              break;
            case TypeCode.Int64:
              this.Emit(OpCodes.Conv_Ovf_I8_Un);
              break;
            case TypeCode.UInt64:
              this.Emit(OpCodes.Conv_Ovf_U8_Un);
              break;
            default:
              throw Error.UnhandledConvert((object) typeTo);
          }
        }
        else
        {
          switch (typeCode)
          {
            case TypeCode.Char:
            case TypeCode.UInt16:
              this.Emit(OpCodes.Conv_Ovf_U2);
              break;
            case TypeCode.SByte:
              this.Emit(OpCodes.Conv_Ovf_I1);
              break;
            case TypeCode.Byte:
              this.Emit(OpCodes.Conv_Ovf_U1);
              break;
            case TypeCode.Int16:
              this.Emit(OpCodes.Conv_Ovf_I2);
              break;
            case TypeCode.Int32:
              this.Emit(OpCodes.Conv_Ovf_I4);
              break;
            case TypeCode.UInt32:
              this.Emit(OpCodes.Conv_Ovf_U4);
              break;
            case TypeCode.Int64:
              this.Emit(OpCodes.Conv_Ovf_I8);
              break;
            case TypeCode.UInt64:
              this.Emit(OpCodes.Conv_Ovf_U8);
              break;
            default:
              throw Error.UnhandledConvert((object) typeTo);
          }
        }
      }
      else if (flag)
      {
        switch (typeCode)
        {
          case TypeCode.Char:
          case TypeCode.Int16:
          case TypeCode.UInt16:
            this.Emit(OpCodes.Conv_U2);
            break;
          case TypeCode.SByte:
          case TypeCode.Byte:
            this.Emit(OpCodes.Conv_U1);
            break;
          case TypeCode.Int32:
          case TypeCode.UInt32:
            this.Emit(OpCodes.Conv_U4);
            break;
          case TypeCode.Int64:
          case TypeCode.UInt64:
            this.Emit(OpCodes.Conv_U8);
            break;
          default:
            throw Error.UnhandledConvert((object) typeTo);
        }
      }
      else
      {
        switch (typeCode)
        {
          case TypeCode.Char:
          case TypeCode.Int16:
          case TypeCode.UInt16:
            this.Emit(OpCodes.Conv_I2);
            break;
          case TypeCode.SByte:
          case TypeCode.Byte:
            this.Emit(OpCodes.Conv_I1);
            break;
          case TypeCode.Int32:
          case TypeCode.UInt32:
            this.Emit(OpCodes.Conv_I4);
            break;
          case TypeCode.Int64:
          case TypeCode.UInt64:
            this.Emit(OpCodes.Conv_I8);
            break;
          default:
            throw Error.UnhandledConvert((object) typeTo);
        }
      }
    }
  }

  private void EmitNullableToNullableConversion(Type typeFrom, Type typeTo, bool isChecked)
  {
    Label label1 = new Label();
    Label label2 = new Label();
    LocalBuilder local1 = this.DeclareLocal(typeFrom);
    this.Emit(OpCodes.Stloc, local1);
    LocalBuilder local2 = this.DeclareLocal(typeTo);
    this.Emit(OpCodes.Ldloca, local1);
    this.EmitHasValue(typeFrom);
    Label label3 = this.DefineLabel();
    this.Emit(OpCodes.Brfalse_S, label3);
    this.Emit(OpCodes.Ldloca, local1);
    this.EmitGetValueOrDefault(typeFrom);
    Type nonNullableType1 = typeFrom.GetNonNullableType();
    Type nonNullableType2 = typeTo.GetNonNullableType();
    this.EmitConvertToType(nonNullableType1, nonNullableType2, isChecked);
    ConstructorInfo constructor = typeTo.GetConstructor(new Type[1]
    {
      nonNullableType2
    });
    this.Emit(OpCodes.Newobj, constructor);
    this.Emit(OpCodes.Stloc, local2);
    Label label4 = this.DefineLabel();
    this.Emit(OpCodes.Br_S, label4);
    this.MarkLabel(label3);
    this.Emit(OpCodes.Ldloca, local2);
    this.Emit(OpCodes.Initobj, typeTo);
    this.MarkLabel(label4);
    this.Emit(OpCodes.Ldloc, local2);
  }

  private void EmitNonNullableToNullableConversion(Type typeFrom, Type typeTo, bool isChecked)
  {
    LocalBuilder local = this.DeclareLocal(typeTo);
    Type nonNullableType = typeTo.GetNonNullableType();
    this.EmitConvertToType(typeFrom, nonNullableType, isChecked);
    ConstructorInfo constructor = typeTo.GetConstructor(new Type[1]
    {
      nonNullableType
    });
    this.Emit(OpCodes.Newobj, constructor);
    this.Emit(OpCodes.Stloc, local);
    this.Emit(OpCodes.Ldloc, local);
  }

  private void EmitNullableToNonNullableConversion(Type typeFrom, Type typeTo, bool isChecked)
  {
    if (typeTo.IsValueType())
      this.EmitNullableToNonNullableStructConversion(typeFrom, typeTo, isChecked);
    else
      this.EmitNullableToReferenceConversion(typeFrom);
  }

  private void EmitNullableToNonNullableStructConversion(
    Type typeFrom,
    Type typeTo,
    bool isChecked)
  {
    LocalBuilder local = this.DeclareLocal(typeFrom);
    this.Emit(OpCodes.Stloc, local);
    this.Emit(OpCodes.Ldloca, local);
    this.EmitGetValue(typeFrom);
    this.EmitConvertToType(typeFrom.GetNonNullableType(), typeTo, isChecked);
  }

  private void EmitNullableToReferenceConversion(Type typeFrom) => this.Emit(OpCodes.Box, typeFrom);

  private void EmitNullableConversion(Type typeFrom, Type typeTo, bool isChecked)
  {
    bool flag1 = typeFrom.IsNullableType();
    bool flag2 = typeTo.IsNullableType();
    if (flag1 & flag2)
      this.EmitNullableToNullableConversion(typeFrom, typeTo, isChecked);
    else if (flag1)
      this.EmitNullableToNonNullableConversion(typeFrom, typeTo, isChecked);
    else
      this.EmitNonNullableToNullableConversion(typeFrom, typeTo, isChecked);
  }

  internal void EmitHasValue(Type nullableType)
  {
    MethodInfo method = nullableType.GetMethod("get_HasValue", BindingFlags.Instance | BindingFlags.Public);
    this.Emit(OpCodes.Call, method);
  }

  internal void EmitGetValue(Type nullableType)
  {
    MethodInfo method = nullableType.GetMethod("get_Value", BindingFlags.Instance | BindingFlags.Public);
    this.Emit(OpCodes.Call, method);
  }

  internal void EmitGetValueOrDefault(Type nullableType)
  {
    MethodInfo method = nullableType.GetMethod("GetValueOrDefault", ReflectionUtils.EmptyTypes);
    this.Emit(OpCodes.Call, method);
  }

  public void EmitArray<T>(IList<T> items)
  {
    ContractUtils.RequiresNotNull((object) items, nameof (items));
    this.EmitInt(items.Count);
    this.Emit(OpCodes.Newarr, typeof (T));
    for (int index = 0; index < items.Count; ++index)
    {
      this.Emit(OpCodes.Dup);
      this.EmitInt(index);
      this.EmitSimpleConstant((object) items[index]);
      this.EmitStoreElement(typeof (T));
    }
  }

  public void EmitArray(Type elementType, int count, EmitArrayHelper emit)
  {
    ContractUtils.RequiresNotNull((object) elementType, nameof (elementType));
    ContractUtils.RequiresNotNull((object) emit, nameof (emit));
    ContractUtils.Requires(count >= 0, nameof (count), Strings.CountCannotBeNegative);
    this.EmitInt(count);
    this.Emit(OpCodes.Newarr, elementType);
    for (int index = 0; index < count; ++index)
    {
      this.Emit(OpCodes.Dup);
      this.EmitInt(index);
      emit(index);
      this.EmitStoreElement(elementType);
    }
  }

  public void EmitArray(Type arrayType)
  {
    ContractUtils.RequiresNotNull((object) arrayType, nameof (arrayType));
    ContractUtils.Requires(arrayType.IsArray, nameof (arrayType), Strings.ArrayTypeMustBeArray);
    int arrayRank = arrayType.GetArrayRank();
    if (arrayRank == 1)
    {
      this.Emit(OpCodes.Newarr, arrayType.GetElementType());
    }
    else
    {
      Type[] paramTypes = new Type[arrayRank];
      for (int index = 0; index < arrayRank; ++index)
        paramTypes[index] = typeof (int);
      this.EmitNew(arrayType, paramTypes);
    }
  }

  public void EmitDecimal(Decimal value)
  {
    if (Decimal.Truncate(value) == value)
    {
      if (-2147483648M <= value && value <= 2147483647M)
      {
        this.EmitInt(Decimal.ToInt32(value));
        this.EmitNew(typeof (Decimal).GetConstructor(new Type[1]
        {
          typeof (int)
        }));
      }
      else if (-9223372036854775808M <= value && value <= 9223372036854775807M)
      {
        this.EmitLong(Decimal.ToInt64(value));
        this.EmitNew(typeof (Decimal).GetConstructor(new Type[1]
        {
          typeof (long)
        }));
      }
      else
        this.EmitDecimalBits(value);
    }
    else
      this.EmitDecimalBits(value);
  }

  private void EmitDecimalBits(Decimal value)
  {
    int[] bits = Decimal.GetBits(value);
    this.EmitInt(bits[0]);
    this.EmitInt(bits[1]);
    this.EmitInt(bits[2]);
    this.EmitBoolean(((ulong) bits[3] & 2147483648UL /*0x80000000*/) > 0UL);
    this.EmitByte((byte) (bits[3] >> 16 /*0x10*/));
    this.EmitNew(typeof (Decimal).GetConstructor(new Type[5]
    {
      typeof (int),
      typeof (int),
      typeof (int),
      typeof (bool),
      typeof (byte)
    }));
  }

  internal void EmitDefault(Type type)
  {
    switch (type.GetTypeCode())
    {
      case TypeCode.Empty:
      case TypeCode.DBNull:
      case TypeCode.String:
        this.Emit(OpCodes.Ldnull);
        break;
      case TypeCode.Object:
      case TypeCode.DateTime:
        if (type.IsValueType())
        {
          LocalBuilder local = this.GetLocal(type);
          this.Emit(OpCodes.Ldloca, local);
          this.Emit(OpCodes.Initobj, type);
          this.Emit(OpCodes.Ldloc, local);
          this.FreeLocal(local);
          break;
        }
        this.Emit(OpCodes.Ldnull);
        break;
      case TypeCode.Boolean:
      case TypeCode.Char:
      case TypeCode.SByte:
      case TypeCode.Byte:
      case TypeCode.Int16:
      case TypeCode.UInt16:
      case TypeCode.Int32:
      case TypeCode.UInt32:
        this.Emit(OpCodes.Ldc_I4_0);
        break;
      case TypeCode.Int64:
      case TypeCode.UInt64:
        this.Emit(OpCodes.Ldc_I4_0);
        this.Emit(OpCodes.Conv_I8);
        break;
      case TypeCode.Single:
        this.Emit(OpCodes.Ldc_R4, 0.0f);
        break;
      case TypeCode.Double:
        this.Emit(OpCodes.Ldc_R8, 0.0);
        break;
      case TypeCode.Decimal:
        this.Emit(OpCodes.Ldc_I4_0);
        this.Emit(OpCodes.Newobj, typeof (Decimal).GetConstructor(new Type[1]
        {
          typeof (int)
        }));
        break;
      default:
        throw Microsoft.Scripting.Utils.Assert.Unreachable;
    }
  }

  public void EmitMissingValue(Type type)
  {
    switch (type.GetTypeCode())
    {
      case TypeCode.Empty:
      case TypeCode.DBNull:
        this.EmitNull();
        break;
      case TypeCode.Boolean:
      case TypeCode.Char:
      case TypeCode.SByte:
      case TypeCode.Byte:
      case TypeCode.Int16:
      case TypeCode.UInt16:
      case TypeCode.Int32:
      case TypeCode.UInt32:
        this.EmitInt(0);
        break;
      case TypeCode.Int64:
      case TypeCode.UInt64:
        this.EmitLong(0L);
        break;
      case TypeCode.Single:
        this.EmitSingle(0.0f);
        break;
      case TypeCode.Double:
        this.Emit(OpCodes.Ldc_R8, 0.0);
        break;
      case TypeCode.Decimal:
        this.EmitFieldGet(typeof (Decimal).GetDeclaredField("Zero"));
        break;
      case TypeCode.DateTime:
        LocalBuilder local1 = this.DeclareLocal(typeof (DateTime));
        this.Emit(OpCodes.Ldloca, local1);
        this.Emit(OpCodes.Initobj, typeof (DateTime));
        this.Emit(OpCodes.Ldloc, local1);
        break;
      case TypeCode.String:
        this.EmitNull();
        break;
      default:
        if (type == typeof (object))
        {
          this.Emit(OpCodes.Ldsfld, typeof (Missing).GetDeclaredField("Value"));
          break;
        }
        if (!type.IsValueType())
        {
          this.EmitNull();
          break;
        }
        LocalBuilder local2 = type.IsSealed() && !type.IsEnum() ? this.DeclareLocal(type) : throw Error.NoDefaultValue();
        this.Emit(OpCodes.Ldloca, local2);
        this.Emit(OpCodes.Initobj, type);
        this.Emit(OpCodes.Ldloc, local2);
        break;
    }
  }

  internal LocalBuilder GetLocal(Type type)
  {
    LocalBuilder localBuilder;
    return this._freeLocals.TryDequeue(type, out localBuilder) ? localBuilder : this.DeclareLocal(type);
  }

  internal void FreeLocal(LocalBuilder local)
  {
    if (local == null)
      return;
    this._freeLocals.Enqueue(local.LocalType, local);
  }
}
