// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.ScriptingRuntimeHelpers
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public static class ScriptingRuntimeHelpers
{
  private const int MIN_CACHE = -100;
  private const int MAX_CACHE = 1000;
  private static readonly object[] cache = ScriptingRuntimeHelpers.MakeCache();
  public static readonly object True = (object) true;
  public static readonly object False = (object) false;
  internal static readonly MethodInfo BooleanToObjectMethod = typeof (ScriptingRuntimeHelpers).GetMethod("BooleanToObject");
  internal static readonly MethodInfo Int32ToObjectMethod = typeof (ScriptingRuntimeHelpers).GetMethod("Int32ToObject");
  private static readonly string[] chars = ScriptingRuntimeHelpers.MakeSingleCharStrings();

  public static T[] ShiftParamsArray<T>(T[] array, int count)
  {
    return array != null && array.Length > count ? ArrayUtils.ShiftLeft<T>(array, count) : new T[0];
  }

  [Obsolete("do not call this method", true)]
  public static IEnumerable<T> MakeGenerator<T>(Func<GeneratorNext<T>> next)
  {
    return (IEnumerable<T>) new GeneratorEnumerable<T>(next);
  }

  [Obsolete("do not call this method", true)]
  public static IEnumerator<T> MakeGenerator<T>(GeneratorNext<T> next)
  {
    return (IEnumerator<T>) new GeneratorEnumerator<T>(next);
  }

  [Obsolete("do not call this method", true)]
  public static IEnumerable<T> MakeGenerator<T>(Func<GeneratorNext<T>> next, int[] yieldMarkers)
  {
    return (IEnumerable<T>) new DebugGeneratorEnumerable<T>(next, yieldMarkers);
  }

  [Obsolete("do not call this method", true)]
  public static IEnumerator<T> MakeGenerator<T>(GeneratorNext<T> next, int[] yieldMarkers)
  {
    return (IEnumerator<T>) new DebugGeneratorEnumerator<T>(next, yieldMarkers);
  }

  private static object[] MakeCache()
  {
    object[] objArray = new object[1100];
    for (int index = 0; index < objArray.Length; ++index)
      objArray[index] = (object) (index - 100);
    return objArray;
  }

  public static object Int32ToObject(int value)
  {
    return value < 1000 && value >= -100 ? ScriptingRuntimeHelpers.cache[value - -100] : (object) value;
  }

  private static string[] MakeSingleCharStrings()
  {
    string[] strArray = new string[(int) byte.MaxValue];
    for (char minValue = char.MinValue; (int) minValue < strArray.Length; ++minValue)
      strArray[(int) minValue] = new string(minValue, 1);
    return strArray;
  }

  public static object BooleanToObject(bool value)
  {
    return !value ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  public static string CharToString(char ch)
  {
    return ch < 'ÿ' ? ScriptingRuntimeHelpers.chars[(int) ch] : new string(ch, 1);
  }

  internal static object GetPrimitiveDefaultValue(Type type)
  {
    switch (type.GetTypeCode())
    {
      case TypeCode.DBNull:
        return (object) null;
      case TypeCode.Boolean:
        return ScriptingRuntimeHelpers.False;
      case TypeCode.Char:
        return (object) char.MinValue;
      case TypeCode.SByte:
        return (object) (sbyte) 0;
      case TypeCode.Byte:
        return (object) (byte) 0;
      case TypeCode.Int16:
        return (object) (short) 0;
      case TypeCode.UInt16:
        return (object) (ushort) 0;
      case TypeCode.Int32:
        return ScriptingRuntimeHelpers.Int32ToObject(0);
      case TypeCode.UInt32:
        return (object) 0U;
      case TypeCode.Int64:
        return (object) 0L;
      case TypeCode.UInt64:
        return (object) 0UL;
      case TypeCode.Single:
        return (object) 0.0f;
      case TypeCode.Double:
        return (object) 0.0;
      case TypeCode.Decimal:
        return (object) 0M;
      case TypeCode.DateTime:
        return (object) new DateTime();
      default:
        return (object) null;
    }
  }

  public static ArgumentTypeException SimpleTypeError(string message)
  {
    return new ArgumentTypeException(message);
  }

  public static Exception CannotConvertError(Type toType, object value)
  {
    return (Exception) ScriptingRuntimeHelpers.SimpleTypeError($"Cannot convert {CompilerHelpers.GetType(value).Name}({value}) to {toType.Name}");
  }

  public static Exception SimpleAttributeError(string message)
  {
    return (Exception) new MissingMemberException(message);
  }

  public static object ReadOnlyAssignError(bool field, string fieldName)
  {
    if (field)
      throw Error.FieldReadonly((object) fieldName);
    throw Error.PropertyReadonly((object) fieldName);
  }

  public static T CreateInstance<T>() => default (T);

  public static T[] CreateArray<T>(int args) => new T[args];

  public static Type GetEventHandlerType(EventInfo eventInfo)
  {
    ContractUtils.RequiresNotNull((object) eventInfo, nameof (eventInfo));
    return eventInfo.EventHandlerType;
  }

  public static IList<string> GetStringMembers(IList<object> members)
  {
    List<string> stringMembers = new List<string>();
    foreach (object member in (IEnumerable<object>) members)
    {
      if (member is string str)
        stringMembers.Add(str);
    }
    return (IList<string>) stringMembers;
  }

  public static void SetEvent(EventTracker eventTracker, object value)
  {
    switch (value)
    {
      case EventTracker eventTracker1:
        if (eventTracker1 == eventTracker)
          break;
        throw Error.UnexpectedEvent((object) eventTracker.DeclaringType.Name, (object) eventTracker.Name, (object) eventTracker1.DeclaringType.Name, (object) eventTracker1.Name);
      case BoundMemberTracker boundMemberTracker:
        if (boundMemberTracker.BoundTo.MemberType != TrackerTypes.Event)
          throw Error.ExpectedBoundEvent((object) boundMemberTracker.BoundTo.MemberType.ToString());
        if (boundMemberTracker.BoundTo == eventTracker)
          break;
        throw Error.UnexpectedEvent((object) eventTracker.DeclaringType.Name, (object) eventTracker.Name, (object) boundMemberTracker.BoundTo.DeclaringType.Name, (object) boundMemberTracker.BoundTo.Name);
      default:
        throw Error.ExpectedBoundEvent((object) CompilerHelpers.GetType(value).Name);
    }
  }

  public static bool CheckDictionaryMembers(IDictionary dict, string[] names)
  {
    if (dict.Count != names.Length)
      return false;
    foreach (string name in names)
    {
      if (!dict.Contains((object) name))
        return false;
    }
    return true;
  }

  [Obsolete("use MakeIncorrectBoxTypeError instead")]
  public static T IncorrectBoxType<T>(object received)
  {
    throw Error.UnexpectedType((object) $"StrongBox<{typeof (T).Name}>", (object) CompilerHelpers.GetType(received).Name);
  }

  public static Exception MakeIncorrectBoxTypeError(Type type, object received)
  {
    return Error.UnexpectedType((object) $"StrongBox<{type.Name}>", (object) CompilerHelpers.GetType(received).Name);
  }

  public static bool InterpretedCallSiteTest(bool restrictionResult, object bindingInfo)
  {
    if (!restrictionResult)
      return false;
    CachedBindingInfo cachedBindingInfo = (CachedBindingInfo) bindingInfo;
    if (cachedBindingInfo.CompilationThreshold < 0)
      return cachedBindingInfo.CheckCompiled();
    --cachedBindingInfo.CompilationThreshold;
    return true;
  }
}
