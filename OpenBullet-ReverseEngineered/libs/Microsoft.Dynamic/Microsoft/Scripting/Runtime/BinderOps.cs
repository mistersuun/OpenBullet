// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.BinderOps
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public static class BinderOps
{
  public static object[] GetCombinedParameters(object[] initialArgs, object additionalArgs)
  {
    switch (additionalArgs)
    {
      case IList list:
label_9:
        object[] destinationArray = new object[initialArgs.Length + list.Count];
        Array.Copy((Array) initialArgs, (Array) destinationArray, initialArgs.Length);
        list.CopyTo((Array) destinationArray, initialArgs.Length);
        return destinationArray;
      case IEnumerable enumerable:
        list = (IList) new List<object>();
        IEnumerator enumerator = enumerable.GetEnumerator();
        try
        {
          while (enumerator.MoveNext())
          {
            object current = enumerator.Current;
            list.Add(current);
          }
          goto label_9;
        }
        finally
        {
          if (enumerator is IDisposable disposable)
            disposable.Dispose();
        }
      default:
        throw new InvalidOperationException("args must be iterable");
    }
  }

  public static Dictionary<TKey, TValue> MakeDictionary<TKey, TValue>(
    string[] names,
    object[] values)
  {
    Dictionary<TKey, TValue> dictionary1 = new Dictionary<TKey, TValue>();
    IDictionary dictionary2 = (IDictionary) dictionary1;
    for (int index = 0; index < names.Length; ++index)
      dictionary2[(object) names[index]] = values[index];
    return dictionary1;
  }

  public static ArgumentTypeException BadArgumentsForOperation(
    ExpressionType op,
    params object[] args)
  {
    StringBuilder stringBuilder = new StringBuilder("unsupported operand type(s) for operation ");
    stringBuilder.Append(op.ToString());
    stringBuilder.Append(": ");
    string str = "";
    foreach (object obj in args)
    {
      stringBuilder.Append(str);
      stringBuilder.Append((object) CompilerHelpers.GetType(obj));
      str = ", ";
    }
    throw new ArgumentTypeException(stringBuilder.ToString());
  }

  public static ArgumentTypeException TypeErrorForIncorrectArgumentCount(
    string methodName,
    int formalNormalArgumentCount,
    int defaultArgumentCount,
    int providedArgumentCount,
    bool hasArgList,
    bool keywordArgumentsProvided)
  {
    return BinderOps.TypeErrorForIncorrectArgumentCount(methodName, formalNormalArgumentCount, formalNormalArgumentCount, defaultArgumentCount, providedArgumentCount, hasArgList, keywordArgumentsProvided);
  }

  public static ArgumentTypeException TypeErrorForIncorrectArgumentCount(
    string methodName,
    int minFormalNormalArgumentCount,
    int maxFormalNormalArgumentCount,
    int defaultArgumentCount,
    int providedArgumentCount,
    bool hasArgList,
    bool keywordArgumentsProvided)
  {
    string str1 = keywordArgumentsProvided ? "non-keyword " : "";
    string str2;
    int num;
    if (defaultArgumentCount > 0 | hasArgList || minFormalNormalArgumentCount != maxFormalNormalArgumentCount)
    {
      if (providedArgumentCount < minFormalNormalArgumentCount || maxFormalNormalArgumentCount == int.MaxValue)
      {
        str2 = "at least";
        num = minFormalNormalArgumentCount - defaultArgumentCount;
      }
      else
      {
        str2 = "at most";
        num = maxFormalNormalArgumentCount;
      }
    }
    else
    {
      if (minFormalNormalArgumentCount == 0)
        return ScriptingRuntimeHelpers.SimpleTypeError($"{methodName}() takes no arguments ({providedArgumentCount} given)");
      str2 = "exactly";
      num = minFormalNormalArgumentCount;
    }
    return new ArgumentTypeException($"{methodName}() takes {str2} {num} {str1}argument{(num == 1 ? (object) "" : (object) "s")} ({providedArgumentCount} given)");
  }

  public static ArgumentTypeException TypeErrorForIncorrectArgumentCount(
    string name,
    int formalNormalArgumentCount,
    int defaultArgumentCount,
    int providedArgumentCount)
  {
    return BinderOps.TypeErrorForIncorrectArgumentCount(name, formalNormalArgumentCount, defaultArgumentCount, providedArgumentCount, false, false);
  }

  public static ArgumentTypeException TypeErrorForIncorrectArgumentCount(
    string name,
    int expected,
    int received)
  {
    return BinderOps.TypeErrorForIncorrectArgumentCount(name, expected, 0, received);
  }

  public static ArgumentTypeException TypeErrorForExtraKeywordArgument(
    string name,
    string argumentName)
  {
    return new ArgumentTypeException($"{name}() got an unexpected keyword argument '{argumentName}'");
  }

  public static ArgumentTypeException TypeErrorForDuplicateKeywordArgument(
    string name,
    string argumentName)
  {
    return new ArgumentTypeException($"{name}() got multiple values for keyword argument '{argumentName}'");
  }

  public static ArgumentTypeException TypeErrorForNonInferrableMethod(string name)
  {
    return new ArgumentTypeException($"The type arguments for method '{name}' cannot be inferred from the usage. Try specifying the type arguments explicitly.");
  }

  public static ArgumentTypeException SimpleTypeError(string message)
  {
    return new ArgumentTypeException(message);
  }

  public static ArgumentTypeException InvalidSplatteeError(string name, string typeName)
  {
    return new ArgumentTypeException($"{name}() argument after * must be a sequence, not {typeName}");
  }

  public static object InvokeMethod(MethodBase mb, object obj, object[] args)
  {
    try
    {
      return mb.Invoke(obj, args);
    }
    catch (TargetInvocationException ex)
    {
      throw ex.InnerException;
    }
  }

  public static object InvokeConstructor(ConstructorInfo ci, object[] args)
  {
    try
    {
      return ci.Invoke(args);
    }
    catch (TargetInvocationException ex)
    {
      throw ex.InnerException;
    }
  }

  public static bool CheckDictionaryMembers(IDictionary dict, string[] names, Type[] types)
  {
    if (dict.Count != names.Length)
      return false;
    for (int index = 0; index < names.Length; ++index)
    {
      string name = names[index];
      if (!dict.Contains((object) name) || types != null && CompilerHelpers.GetType(dict[(object) name]) != types[index])
        return false;
    }
    return true;
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

  public static Type GetEventHandlerType(EventInfo eventInfo)
  {
    ContractUtils.RequiresNotNull((object) eventInfo, nameof (eventInfo));
    return eventInfo.EventHandlerType;
  }

  public static void SetEvent(EventTracker eventTracker, object value)
  {
    switch (value)
    {
      case EventTracker eventTracker1:
        if (eventTracker1 == eventTracker)
          break;
        throw new ArgumentException($"expected event from {eventTracker.DeclaringType.Name}.{eventTracker.Name}, got event from {eventTracker1.DeclaringType.Name}.{eventTracker1.Name}");
      case BoundMemberTracker boundMemberTracker:
        if (boundMemberTracker.BoundTo.MemberType != TrackerTypes.Event)
          throw new ArgumentTypeException("expected bound event, got " + boundMemberTracker.BoundTo.MemberType.ToString());
        if (boundMemberTracker.BoundTo == eventTracker)
          break;
        throw new ArgumentException($"expected event from {eventTracker.DeclaringType.Name}.{eventTracker.Name}, got event from {boundMemberTracker.BoundTo.DeclaringType.Name}.{boundMemberTracker.BoundTo.Name}");
      default:
        throw new ArgumentTypeException("expected bound event, got " + CompilerHelpers.GetType(value).Name);
    }
  }
}
