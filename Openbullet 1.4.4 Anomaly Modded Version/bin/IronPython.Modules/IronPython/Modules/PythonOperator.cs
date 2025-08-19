// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonOperator
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Modules;

public static class PythonOperator
{
  public const string __doc__ = "Provides programmatic access to various operators (addition, accessing members, etc...)";

  public static object lt(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.LessThan, a, b);
  }

  public static object le(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.LessThanOrEqual, a, b);
  }

  public static object eq(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.Equal, a, b);
  }

  public static object ne(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.NotEqual, a, b);
  }

  public static object ge(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.GreaterThanOrEqual, a, b);
  }

  public static object gt(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.GreaterThan, a, b);
  }

  public static object __lt__(CodeContext context, object a, object b)
  {
    return PythonOperator.lt(context, a, b);
  }

  public static object __le__(CodeContext context, object a, object b)
  {
    return PythonOperator.le(context, a, b);
  }

  public static object __eq__(CodeContext context, object a, object b)
  {
    return PythonOperator.eq(context, a, b);
  }

  public static object __ne__(CodeContext context, object a, object b)
  {
    return PythonOperator.ne(context, a, b);
  }

  public static object __ge__(CodeContext context, object a, object b)
  {
    return PythonOperator.ge(context, a, b);
  }

  public static object __gt__(CodeContext context, object a, object b)
  {
    return PythonOperator.gt(context, a, b);
  }

  public static bool not_(object o) => PythonOps.Not(o);

  public static bool __not__(object o) => PythonOps.Not(o);

  public static bool truth(object o) => PythonOps.IsTrue(o);

  public static object is_(object a, object b) => PythonOps.Is(a, b);

  public static object is_not(object a, object b) => PythonOps.IsNot(a, b);

  public static object abs(CodeContext context, object o) => Builtin.abs(context, o);

  public static object __abs__(CodeContext context, object o) => Builtin.abs(context, o);

  public static object add(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.Add, a, b);
  }

  public static object __add__(CodeContext context, object a, object b)
  {
    return PythonOperator.add(context, a, b);
  }

  public static object and_(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.BitwiseAnd, a, b);
  }

  public static object __and__(CodeContext context, object a, object b)
  {
    return PythonOperator.and_(context, a, b);
  }

  public static object div(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.Divide, a, b);
  }

  public static object __div__(CodeContext context, object a, object b)
  {
    return PythonOperator.div(context, a, b);
  }

  public static object floordiv(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.FloorDivide, a, b);
  }

  public static object __floordiv__(CodeContext context, object a, object b)
  {
    return PythonOperator.floordiv(context, a, b);
  }

  public static object inv(CodeContext context, object o) => PythonOps.OnesComplement(o);

  public static object invert(CodeContext context, object o) => PythonOps.OnesComplement(o);

  public static object __inv__(CodeContext context, object o) => PythonOps.OnesComplement(o);

  public static object __invert__(CodeContext context, object o) => PythonOps.OnesComplement(o);

  public static object lshift(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.LeftShift, a, b);
  }

  public static object __lshift__(CodeContext context, object a, object b)
  {
    return PythonOperator.lshift(context, a, b);
  }

  public static object mod(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.Mod, a, b);
  }

  public static object __mod__(CodeContext context, object a, object b)
  {
    return PythonOperator.mod(context, a, b);
  }

  public static object mul(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.Multiply, a, b);
  }

  public static object __mul__(CodeContext context, object a, object b)
  {
    return PythonOperator.mul(context, a, b);
  }

  public static object neg(object o) => PythonOps.Negate(o);

  public static object __neg__(object o) => PythonOps.Negate(o);

  public static object or_(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.BitwiseOr, a, b);
  }

  public static object __or__(CodeContext context, object a, object b)
  {
    return PythonOperator.or_(context, a, b);
  }

  public static object pos(object o) => PythonOps.Plus(o);

  public static object __pos__(object o) => PythonOps.Plus(o);

  public static object pow(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.Power, a, b);
  }

  public static object __pow__(CodeContext context, object a, object b)
  {
    return PythonOperator.pow(context, a, b);
  }

  public static object rshift(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.RightShift, a, b);
  }

  public static object __rshift__(CodeContext context, object a, object b)
  {
    return PythonOperator.rshift(context, a, b);
  }

  public static object sub(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.Subtract, a, b);
  }

  public static object __sub__(CodeContext context, object a, object b)
  {
    return PythonOperator.sub(context, a, b);
  }

  public static object truediv(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.TrueDivide, a, b);
  }

  public static object __truediv__(CodeContext context, object a, object b)
  {
    return PythonOperator.truediv(context, a, b);
  }

  public static object xor(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.ExclusiveOr, a, b);
  }

  public static object __xor__(CodeContext context, object a, object b)
  {
    return PythonOperator.xor(context, a, b);
  }

  public static object concat(CodeContext context, object a, object b)
  {
    PythonOperator.TestBothSequence(a, b);
    return context.LanguageContext.Operation(PythonOperationKind.Add, a, b);
  }

  public static object __concat__(CodeContext context, object a, object b)
  {
    return PythonOperator.concat(context, a, b);
  }

  public static bool contains(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Contains(b, a);
  }

  public static bool __contains__(CodeContext context, object a, object b)
  {
    return PythonOperator.contains(context, a, b);
  }

  public static int countOf(CodeContext context, object a, object b)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(a);
    int num = 0;
    while (enumerator.MoveNext())
    {
      if (PythonOps.EqualRetBool(context, enumerator.Current, b))
        ++num;
    }
    return num;
  }

  public static void delitem(CodeContext context, object a, object b)
  {
    context.LanguageContext.DelIndex(a, b);
  }

  public static void __delitem__(CodeContext context, object a, object b)
  {
    PythonOperator.delitem(context, a, b);
  }

  public static void delslice(CodeContext context, object a, object b, object c)
  {
    PythonOperator.MakeSlice(b, c);
    context.LanguageContext.DelSlice(a, b, c);
  }

  public static void __delslice__(CodeContext context, object a, object b, object c)
  {
    PythonOperator.delslice(context, a, b, c);
  }

  public static object getitem(CodeContext context, object a, object b)
  {
    return PythonOps.GetIndex(context, a, b);
  }

  public static object __getitem__(CodeContext context, object a, object b)
  {
    return PythonOps.GetIndex(context, a, b);
  }

  public static object getslice(CodeContext context, object a, object b, object c)
  {
    return PythonOps.GetIndex(context, a, PythonOperator.MakeSlice(b, c));
  }

  public static object __getslice__(CodeContext context, object a, object b, object c)
  {
    return PythonOps.GetIndex(context, a, PythonOperator.MakeSlice(b, c));
  }

  public static int indexOf(CodeContext context, object a, object b)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(a);
    int num = 0;
    while (enumerator.MoveNext())
    {
      if (PythonOps.EqualRetBool(context, enumerator.Current, b))
        return num;
      ++num;
    }
    throw PythonOps.ValueError("object not in sequence");
  }

  public static object repeat(CodeContext context, object a, object b)
  {
    try
    {
      PythonOps.GetEnumerator(a);
    }
    catch
    {
      throw PythonOps.TypeError("object can't be repeated");
    }
    try
    {
      Int32Ops.__new__(context, b);
    }
    catch
    {
      throw PythonOps.TypeError("integer required");
    }
    return context.LanguageContext.Operation(PythonOperationKind.Multiply, a, b);
  }

  public static object __repeat__(CodeContext context, object a, object b)
  {
    return PythonOperator.repeat(context, a, b);
  }

  [Python3Warning("operator.sequenceIncludes() is not supported in 3.x. Use operator.contains().")]
  public static object sequenceIncludes(CodeContext context, object a, object b)
  {
    return (object) PythonOperator.contains(context, a, b);
  }

  public static void setitem(CodeContext context, object a, object b, object c)
  {
    context.LanguageContext.SetIndex(a, b, c);
  }

  public static void __setitem__(CodeContext context, object a, object b, object c)
  {
    PythonOperator.setitem(context, a, b, c);
  }

  public static void setslice(CodeContext context, object a, object b, object c, object v)
  {
    context.LanguageContext.SetSlice(a, b, c, v);
  }

  public static void __setslice__(CodeContext context, object a, object b, object c, object v)
  {
    PythonOperator.setslice(context, a, b, c, v);
  }

  [Python3Warning("operator.isCallable() is not supported in 3.x. Use hasattr(obj, '__call__').")]
  public static bool isCallable(CodeContext context, object o) => PythonOps.IsCallable(context, o);

  public static object isMappingType(CodeContext context, object o)
  {
    return PythonOps.IsMappingType(context, o);
  }

  public static bool isNumberType(object o)
  {
    switch (o)
    {
      case int _:
      case long _:
      case double _:
      case float _:
      case short _:
      case uint _:
      case ulong _:
      case ushort _:
      case Decimal _:
      case BigInteger _:
      case Complex _:
        return true;
      default:
        return o is byte;
    }
  }

  public static bool isSequenceType(object o)
  {
    if (CompilerHelpers.GetType(o) != typeof (PythonType))
    {
      switch (o)
      {
        case PythonDictionary _:
          break;
        case ICollection _:
        case IEnumerable _:
        case IEnumerator _:
        case IList _:
          return true;
        default:
          return PythonOps.HasAttr(DefaultContext.Default, o, "__getitem__");
      }
    }
    return false;
  }

  private static int SliceToInt(object o)
  {
    int result;
    if (Converter.TryConvertToInt32(o, out result))
      return result;
    throw PythonOps.TypeError("integer expected");
  }

  private static object MakeSlice(object a, object b)
  {
    return (object) new IronPython.Runtime.Slice((object) PythonOperator.SliceToInt(a), (object) PythonOperator.SliceToInt(b), (object) null);
  }

  public static object iadd(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.InPlaceAdd, a, b);
  }

  public static object iand(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.InPlaceBitwiseAnd, a, b);
  }

  public static object idiv(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.InPlaceDivide, a, b);
  }

  public static object ifloordiv(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.InPlaceFloorDivide, a, b);
  }

  public static object ilshift(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.InPlaceLeftShift, a, b);
  }

  public static object imod(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.InPlaceMod, a, b);
  }

  public static object imul(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.InPlaceMultiply, a, b);
  }

  public static object ior(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.InPlaceBitwiseOr, a, b);
  }

  public static object ipow(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.InPlacePower, a, b);
  }

  public static object irshift(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.InPlaceRightShift, a, b);
  }

  public static object isub(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.InPlaceSubtract, a, b);
  }

  public static object itruediv(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.InPlaceTrueDivide, a, b);
  }

  public static object ixor(CodeContext context, object a, object b)
  {
    return context.LanguageContext.Operation(PythonOperationKind.InPlaceExclusiveOr, a, b);
  }

  public static object iconcat(CodeContext context, object a, object b)
  {
    PythonOperator.TestBothSequence(a, b);
    return context.LanguageContext.Operation(PythonOperationKind.InPlaceAdd, a, b);
  }

  public static object irepeat(CodeContext context, object a, object b)
  {
    if (!PythonOperator.isSequenceType(a))
      throw PythonOps.TypeError("'{0}' object cannot be repeated", (object) PythonTypeOps.GetName(a));
    try
    {
      Int32Ops.__new__(DefaultContext.Default, b);
    }
    catch
    {
      throw PythonOps.TypeError("integer required");
    }
    return context.LanguageContext.Operation(PythonOperationKind.InPlaceMultiply, a, b);
  }

  public static object __iadd__(CodeContext context, object a, object b)
  {
    return PythonOperator.iadd(context, a, b);
  }

  public static object __iand__(CodeContext context, object a, object b)
  {
    return PythonOperator.iand(context, a, b);
  }

  public static object __idiv__(CodeContext context, object a, object b)
  {
    return PythonOperator.idiv(context, a, b);
  }

  public static object __ifloordiv__(CodeContext context, object a, object b)
  {
    return PythonOperator.ifloordiv(context, a, b);
  }

  public static object __ilshift__(CodeContext context, object a, object b)
  {
    return PythonOperator.ilshift(context, a, b);
  }

  public static object __imod__(CodeContext context, object a, object b)
  {
    return PythonOperator.imod(context, a, b);
  }

  public static object __imul__(CodeContext context, object a, object b)
  {
    return PythonOperator.imul(context, a, b);
  }

  public static object __ior__(CodeContext context, object a, object b)
  {
    return PythonOperator.ior(context, a, b);
  }

  public static object __ipow__(CodeContext context, object a, object b)
  {
    return PythonOperator.ipow(context, a, b);
  }

  public static object __irshift__(CodeContext context, object a, object b)
  {
    return PythonOperator.irshift(context, a, b);
  }

  public static object __isub__(CodeContext context, object a, object b)
  {
    return PythonOperator.isub(context, a, b);
  }

  public static object __itruediv__(CodeContext context, object a, object b)
  {
    return PythonOperator.itruediv(context, a, b);
  }

  public static object __ixor__(CodeContext context, object a, object b)
  {
    return PythonOperator.ixor(context, a, b);
  }

  public static object __iconcat__(CodeContext context, object a, object b)
  {
    return PythonOperator.iconcat(context, a, b);
  }

  public static object __irepeat__(CodeContext context, object a, object b)
  {
    return PythonOperator.irepeat(context, a, b);
  }

  public static object index(object a) => (object) PythonOperator.__index__(a);

  public static int __index__(object a) => Converter.ConvertToIndex(a);

  [Documentation("compare_digest(a, b)-> bool\r\n\r\nReturn 'a == b'.  This function uses an approach designed to prevent\r\ntiming analysis, making it appropriate for cryptography.\r\na and b must both be of the same type: either str (ASCII only),\r\nor any type that supports the buffer protocol (e.g. bytes).\r\n\r\nNote: If a and b are of different lengths, or if an error occurs,\r\na timing attack could theoretically reveal information about the\r\ntypes and lengths of a and b--but not their values.")]
  public static bool _compare_digest(object a, object b)
  {
    switch (a)
    {
      case string _ when b is string:
        string s1 = a as string;
        string s2 = b as string;
        return PythonOperator.CompareBytes((IEnumerable<byte>) s1.MakeByteArray(), (IEnumerable<byte>) s2.MakeByteArray());
      case IBufferProtocol _ when b is IBufferProtocol:
        IBufferProtocol bufferProtocol1 = a as IBufferProtocol;
        IBufferProtocol bufferProtocol2 = b as IBufferProtocol;
        if (bufferProtocol1.NumberDimensions > 1L || bufferProtocol2.NumberDimensions > 1L)
          throw PythonOps.BufferError("Buffer must be single dimension");
        return PythonOperator.CompareBytes((IEnumerable<byte>) bufferProtocol1.ToBytes(0, new int?()), (IEnumerable<byte>) bufferProtocol2.ToBytes(0, new int?()));
      default:
        throw PythonOps.TypeError("unsupported operand types(s) or combination of types: '{0}' and '{1}", (object) PythonOps.GetPythonTypeName(a), (object) PythonOps.GetPythonTypeName(b));
    }
  }

  private static bool CompareBytes(IEnumerable<byte> a, IEnumerable<byte> b)
  {
    List<byte> list1 = a.ToList<byte>();
    List<byte> list2 = b.ToList<byte>();
    int count1 = list2.Count;
    int count2 = list1.Count;
    int num1 = count1;
    int num2 = 0;
    List<byte> byteList1 = (List<byte>) null;
    List<byte> byteList2 = list2;
    if (count2 == num1)
    {
      byteList1 = list1;
      num2 = 0;
    }
    if (count2 != num1)
    {
      byteList1 = list2;
      num2 = 1;
    }
    for (int index = 0; index < num1; ++index)
      num2 |= (int) byteList1[index] ^ (int) byteList2[index];
    return num2 == 0;
  }

  private static void TestBothSequence(object a, object b)
  {
    if (!PythonOperator.isSequenceType(a))
      throw PythonOps.TypeError("'{0}' object cannot be concatenated", (object) PythonTypeOps.GetName(a));
    if (!PythonOperator.isSequenceType(b))
      throw PythonOps.TypeError("cannot concatenate '{0}' and '{1} objects", (object) PythonTypeOps.GetName(a), (object) PythonTypeOps.GetName(b));
  }

  public class attrgetter
  {
    private readonly object[] _names;

    public attrgetter(params object[] attrs)
    {
      this._names = attrs.Length != 0 ? attrs : throw PythonOps.TypeError("attrgetter expected 1 arguments, got 0");
    }

    [SpecialName]
    public object Call(CodeContext context, object param)
    {
      if (this._names.Length == 1)
        return PythonOperator.attrgetter.GetOneAttr(context, param, this._names[0]);
      object[] objArray = new object[this._names.Length];
      for (int index = 0; index < this._names.Length; ++index)
        objArray[index] = PythonOperator.attrgetter.GetOneAttr(context, param, this._names[index]);
      return (object) PythonTuple.MakeTuple(objArray);
    }

    private static object GetOneAttr(CodeContext context, object param, object val)
    {
      int length = val is string name ? name.IndexOf('.') : throw PythonOps.TypeError("attribute name must be string");
      if (length < 0)
        return PythonOps.GetBoundAttr(context, param, name);
      object oneAttr = PythonOperator.attrgetter.GetOneAttr(context, param, (object) name.Substring(0, length));
      return PythonOperator.attrgetter.GetOneAttr(context, oneAttr, (object) name.Substring(length + 1, name.Length - length - 1));
    }
  }

  public class itemgetter
  {
    private readonly object[] _items;

    public itemgetter([NotNull] params object[] items)
    {
      this._items = items.Length != 0 ? items : throw PythonOps.TypeError("itemgetter needs at least one argument");
    }

    [SpecialName]
    public object Call(CodeContext context, object param)
    {
      if (this._items.Length == 1)
        return PythonOps.GetIndex(context, param, this._items[0]);
      object[] objArray = new object[this._items.Length];
      for (int index = 0; index < this._items.Length; ++index)
        objArray[index] = PythonOps.GetIndex(context, param, this._items[index]);
      return (object) PythonTuple.MakeTuple(objArray);
    }
  }

  [PythonType]
  public class methodcaller
  {
    private readonly string _name;
    private readonly object[] _args;
    private readonly IDictionary<object, object> _dict;

    public methodcaller(string name, params object[] args)
    {
      this._name = name;
      this._args = args;
    }

    public methodcaller(string name, [ParamDictionary] IDictionary<object, object> kwargs, params object[] args)
    {
      this._name = name;
      this._args = args;
      this._dict = kwargs;
    }

    public override string ToString() => $"<operator.methodcaller: {this._name}>";

    [SpecialName]
    public object Call(CodeContext context, object param)
    {
      object boundAttr = PythonOps.GetBoundAttr(context, param, this._name);
      return this._dict == null ? PythonOps.CallWithContext(context, boundAttr, this._args) : PythonCalls.CallWithKeywordArgs(context, boundAttr, this._args, this._dict);
    }
  }
}
