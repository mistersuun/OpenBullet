// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.TypeCache
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using Microsoft.Scripting.Runtime;
using System;

#nullable disable
namespace IronPython.Runtime.Types;

public static class TypeCache
{
  private static PythonType array;
  private static PythonType builtinfunction;
  private static PythonType pythondictionary;
  private static PythonType frozensetcollection;
  private static PythonType pythonfunction;
  private static PythonType builtin;
  private static PythonType obj;
  private static PythonType setcollection;
  private static PythonType pythontype;
  private static PythonType str;
  private static PythonType pythontuple;
  private static PythonType weakreference;
  private static PythonType list;
  private static PythonType pythonfile;
  private static PythonType pythonmodule;
  private static PythonType method;
  private static PythonType enumerate;
  private static PythonType intType;
  private static PythonType singleType;
  private static PythonType doubleType;
  private static PythonType biginteger;
  private static PythonType complex;
  private static PythonType super;
  private static PythonType oldclass;
  private static PythonType oldinstance;
  private static PythonType nullType;
  private static PythonType boolType;
  private static PythonType baseException;

  public static PythonType Array
  {
    get
    {
      if (TypeCache.array == null)
        TypeCache.array = DynamicHelpers.GetPythonTypeFromType(typeof (System.Array));
      return TypeCache.array;
    }
  }

  public static PythonType BuiltinFunction
  {
    get
    {
      if (TypeCache.builtinfunction == null)
        TypeCache.builtinfunction = DynamicHelpers.GetPythonTypeFromType(typeof (IronPython.Runtime.Types.BuiltinFunction));
      return TypeCache.builtinfunction;
    }
  }

  public static PythonType Dict
  {
    get
    {
      if (TypeCache.pythondictionary == null)
        TypeCache.pythondictionary = DynamicHelpers.GetPythonTypeFromType(typeof (PythonDictionary));
      return TypeCache.pythondictionary;
    }
  }

  public static PythonType FrozenSet
  {
    get
    {
      if (TypeCache.frozensetcollection == null)
        TypeCache.frozensetcollection = DynamicHelpers.GetPythonTypeFromType(typeof (FrozenSetCollection));
      return TypeCache.frozensetcollection;
    }
  }

  public static PythonType Function
  {
    get
    {
      if (TypeCache.pythonfunction == null)
        TypeCache.pythonfunction = DynamicHelpers.GetPythonTypeFromType(typeof (PythonFunction));
      return TypeCache.pythonfunction;
    }
  }

  public static PythonType Builtin
  {
    get
    {
      if (TypeCache.builtin == null)
        TypeCache.builtin = DynamicHelpers.GetPythonTypeFromType(typeof (IronPython.Modules.Builtin));
      return TypeCache.builtin;
    }
  }

  public static PythonType Object
  {
    get
    {
      if (TypeCache.obj == null)
        TypeCache.obj = DynamicHelpers.GetPythonTypeFromType(typeof (object));
      return TypeCache.obj;
    }
  }

  public static PythonType Set
  {
    get
    {
      if (TypeCache.setcollection == null)
        TypeCache.setcollection = DynamicHelpers.GetPythonTypeFromType(typeof (SetCollection));
      return TypeCache.setcollection;
    }
  }

  public static PythonType PythonType
  {
    get
    {
      if (TypeCache.pythontype == null)
        TypeCache.pythontype = DynamicHelpers.GetPythonTypeFromType(typeof (PythonType));
      return TypeCache.pythontype;
    }
  }

  public static PythonType String
  {
    get
    {
      if (TypeCache.str == null)
        TypeCache.str = DynamicHelpers.GetPythonTypeFromType(typeof (string));
      return TypeCache.str;
    }
  }

  public static PythonType PythonTuple
  {
    get
    {
      if (TypeCache.pythontuple == null)
        TypeCache.pythontuple = DynamicHelpers.GetPythonTypeFromType(typeof (IronPython.Runtime.PythonTuple));
      return TypeCache.pythontuple;
    }
  }

  public static PythonType WeakReference
  {
    get
    {
      if (TypeCache.weakreference == null)
        TypeCache.weakreference = DynamicHelpers.GetPythonTypeFromType(typeof (System.WeakReference));
      return TypeCache.weakreference;
    }
  }

  public static PythonType List
  {
    get
    {
      if (TypeCache.list == null)
        TypeCache.list = DynamicHelpers.GetPythonTypeFromType(typeof (IronPython.Runtime.List));
      return TypeCache.list;
    }
  }

  public static PythonType PythonFile
  {
    get
    {
      if (TypeCache.pythonfile == null)
        TypeCache.pythonfile = DynamicHelpers.GetPythonTypeFromType(typeof (IronPython.Runtime.PythonFile));
      return TypeCache.pythonfile;
    }
  }

  public static PythonType Module
  {
    get
    {
      if (TypeCache.pythonmodule == null)
        TypeCache.pythonmodule = DynamicHelpers.GetPythonTypeFromType(typeof (PythonModule));
      return TypeCache.pythonmodule;
    }
  }

  public static PythonType Method
  {
    get
    {
      if (TypeCache.method == null)
        TypeCache.method = DynamicHelpers.GetPythonTypeFromType(typeof (IronPython.Runtime.Method));
      return TypeCache.method;
    }
  }

  public static PythonType Enumerate
  {
    get
    {
      if (TypeCache.enumerate == null)
        TypeCache.enumerate = DynamicHelpers.GetPythonTypeFromType(typeof (IronPython.Runtime.Enumerate));
      return TypeCache.enumerate;
    }
  }

  public static PythonType Int32
  {
    get
    {
      if (TypeCache.intType == null)
        TypeCache.intType = DynamicHelpers.GetPythonTypeFromType(typeof (int));
      return TypeCache.intType;
    }
  }

  public static PythonType Single
  {
    get
    {
      if (TypeCache.singleType == null)
        TypeCache.singleType = DynamicHelpers.GetPythonTypeFromType(typeof (float));
      return TypeCache.singleType;
    }
  }

  public static PythonType Double
  {
    get
    {
      if (TypeCache.doubleType == null)
        TypeCache.doubleType = DynamicHelpers.GetPythonTypeFromType(typeof (double));
      return TypeCache.doubleType;
    }
  }

  public static PythonType BigInteger
  {
    get
    {
      if (TypeCache.biginteger == null)
        TypeCache.biginteger = DynamicHelpers.GetPythonTypeFromType(typeof (System.Numerics.BigInteger));
      return TypeCache.biginteger;
    }
  }

  public static PythonType Complex
  {
    get
    {
      if (TypeCache.complex == null)
        TypeCache.complex = DynamicHelpers.GetPythonTypeFromType(typeof (System.Numerics.Complex));
      return TypeCache.complex;
    }
  }

  public static PythonType Super
  {
    get
    {
      if (TypeCache.super == null)
        TypeCache.super = DynamicHelpers.GetPythonTypeFromType(typeof (IronPython.Runtime.Super));
      return TypeCache.super;
    }
  }

  public static PythonType OldClass
  {
    get
    {
      if (TypeCache.oldclass == null)
        TypeCache.oldclass = DynamicHelpers.GetPythonTypeFromType(typeof (IronPython.Runtime.Types.OldClass));
      return TypeCache.oldclass;
    }
  }

  public static PythonType OldInstance
  {
    get
    {
      if (TypeCache.oldinstance == null)
        TypeCache.oldinstance = DynamicHelpers.GetPythonTypeFromType(typeof (IronPython.Runtime.Types.OldInstance));
      return TypeCache.oldinstance;
    }
  }

  public static PythonType Null
  {
    get
    {
      if (TypeCache.nullType == null)
        TypeCache.nullType = DynamicHelpers.GetPythonTypeFromType(typeof (DynamicNull));
      return TypeCache.nullType;
    }
  }

  public static PythonType Boolean
  {
    get
    {
      if (TypeCache.boolType == null)
        TypeCache.boolType = DynamicHelpers.GetPythonTypeFromType(typeof (bool));
      return TypeCache.boolType;
    }
  }

  public static PythonType BaseException
  {
    get
    {
      if (TypeCache.baseException == null)
        TypeCache.baseException = DynamicHelpers.GetPythonTypeFromType(typeof (PythonExceptions.BaseException));
      return TypeCache.baseException;
    }
  }

  [Obsolete("use Complex instead")]
  public static PythonType Complex64 => TypeCache.Complex;
}
