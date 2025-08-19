// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Profiler
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler;
using Microsoft.Scripting.Ast;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Runtime;

public sealed class Profiler
{
  private readonly Dictionary<MethodBase, int> _methods;
  private readonly Dictionary<string, int> _names;
  private readonly List<string> _counters;
  private readonly List<long[,]> _profiles;
  private long[,] _profileData;
  private const int _initialSize = 100;
  private const int TimeInBody = 0;
  private const int TimeInChildMethods = 1;
  private const int NumberOfCalls = 2;
  private static readonly object _profileKey = new object();

  public static Profiler GetProfiler(PythonContext context)
  {
    return context.GetOrCreateModuleState<Profiler>(Profiler._profileKey, (Func<Profiler>) (() => new Profiler()));
  }

  private Profiler()
  {
    this._methods = new Dictionary<MethodBase, int>();
    this._names = new Dictionary<string, int>();
    this._counters = new List<string>();
    this._profiles = new List<long[,]>();
    this._profileData = new long[100, 3];
  }

  private static string FormatMethodName(MethodBase method)
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (method.DeclaringType != (Type) null)
    {
      stringBuilder.Append("type ");
      stringBuilder.Append(method.DeclaringType.Name);
      stringBuilder.Append(": ");
    }
    stringBuilder.Append("method: ");
    stringBuilder.Append(method.Name);
    stringBuilder.Append('(');
    bool flag = false;
    foreach (ParameterInfo parameter in method.GetParameters())
    {
      if (flag)
        stringBuilder.Append(", ");
      else
        flag = true;
      stringBuilder.Append(parameter.ParameterType.Name);
    }
    stringBuilder.Append(')');
    return stringBuilder.ToString();
  }

  private int GetProfilerIndex(MethodBase method)
  {
    lock (this._methods)
    {
      int newProfilerIndex;
      if (!this._methods.TryGetValue(method, out newProfilerIndex))
      {
        newProfilerIndex = this.GetNewProfilerIndex(Profiler.FormatMethodName(method));
        this._methods[method] = newProfilerIndex;
      }
      return newProfilerIndex;
    }
  }

  private int GetProfilerIndex(string name)
  {
    lock (this._methods)
    {
      int newProfilerIndex;
      if (!this._names.TryGetValue(name, out newProfilerIndex))
      {
        newProfilerIndex = this.GetNewProfilerIndex(name);
        this._names[name] = newProfilerIndex;
      }
      return newProfilerIndex;
    }
  }

  private int GetNewProfilerIndex(string name)
  {
    int count;
    lock (this._counters)
    {
      count = this._counters.Count;
      this._counters.Add(name);
      if (count >= this._profileData.Length / 3)
        this._profiles.Add(Interlocked.Exchange<long[,]>(ref this._profileData, new long[count * 2, 3]));
    }
    return count;
  }

  public List<Profiler.Data> GetProfile(bool includeUnused)
  {
    List<Profiler.Data> profile = new List<Profiler.Data>(this._counters.Count);
    lock (this._counters)
    {
      int length = this._profileData.Length / 3;
      long[,] numArray = Interlocked.Exchange<long[,]>(ref this._profileData, new long[length, 3]);
      for (int index1 = 0; index1 < this._profiles.Count; ++index1)
      {
        for (int index2 = 0; index2 < length; ++index2)
        {
          if (index2 < this._profiles[index1].Length / 3)
          {
            numArray[index2, 0] += this._profiles[index1][index2, 0];
            numArray[index2, 1] += this._profiles[index1][index2, 1];
            numArray[index2, 2] += this._profiles[index1][index2, 2];
          }
        }
      }
      this._profiles.Clear();
      this._profiles.Add(numArray);
      for (int index = 0; index < this._counters.Count; ++index)
      {
        if (includeUnused || numArray[index, 2] > 0L)
          profile.Add(new Profiler.Data(this._counters[index], Profiler.DateTimeTicksFromTimeData(numArray[index, 0] + numArray[index, 1]), Profiler.DateTimeTicksFromTimeData(numArray[index, 0]), (int) numArray[index, 2]));
      }
    }
    return profile;
  }

  public void Reset()
  {
    lock (this._counters)
    {
      Interlocked.Exchange<long[,]>(ref this._profileData, new long[this._profileData.Length / 3, 3]);
      this._profiles.Clear();
    }
  }

  private static long DateTimeTicksFromTimeData(long elapsedStopwatchTicks)
  {
    return Stopwatch.IsHighResolution ? (long) ((double) elapsedStopwatchTicks * 10000000.0 / (double) Stopwatch.Frequency) : elapsedStopwatchTicks;
  }

  public long StartCall(int index)
  {
    Interlocked.Increment(ref this._profileData.Address(index, 2));
    return Stopwatch.GetTimestamp();
  }

  public long StartNestedCall(int index, long timestamp)
  {
    long timestamp1 = Stopwatch.GetTimestamp();
    Interlocked.Add(ref this._profileData.Address(index, 0), timestamp1 - timestamp);
    return timestamp1;
  }

  public long FinishNestedCall(int index, long timestamp)
  {
    long timestamp1 = Stopwatch.GetTimestamp();
    Interlocked.Add(ref this._profileData.Address(index, 1), timestamp1 - timestamp);
    return timestamp1;
  }

  public void FinishCall(int index, long timestamp)
  {
    long timestamp1 = Stopwatch.GetTimestamp();
    Interlocked.Add(ref this._profileData.Address(index, 0), timestamp1 - timestamp);
  }

  internal Expression AddOuterProfiling(
    Expression body,
    ParameterExpression tick,
    int profileIndex)
  {
    return (Expression) Expression.Block((Expression) Expression.Assign((Expression) tick, (Expression) Expression.Call((Expression) Expression.Constant((object) this, typeof (Profiler)), typeof (Profiler).GetMethod("StartCall"), Utils.Constant((object) profileIndex))), (Expression) Utils.Try(body).Finally((Expression) Expression.Call((Expression) Expression.Constant((object) this, typeof (Profiler)), typeof (Profiler).GetMethod("FinishCall"), Utils.Constant((object) profileIndex), (Expression) tick)));
  }

  internal Expression AddInnerProfiling(
    Expression body,
    ParameterExpression tick,
    int profileIndex)
  {
    return (Expression) Expression.Block((Expression) Expression.Assign((Expression) tick, (Expression) Expression.Call((Expression) Expression.Constant((object) this, typeof (Profiler)), typeof (Profiler).GetMethod("StartNestedCall"), Utils.Constant((object) profileIndex), (Expression) tick)), (Expression) Utils.Try(body).Finally((Expression) Expression.Assign((Expression) tick, (Expression) Expression.Call((Expression) Expression.Constant((object) this, typeof (Profiler)), typeof (Profiler).GetMethod("FinishNestedCall"), Utils.Constant((object) profileIndex), (Expression) tick))));
  }

  private static bool IgnoreMethod(MethodBase method)
  {
    return ((IEnumerable<object>) method.GetCustomAttributes(typeof (ProfilerTreatsAsExternalAttribute), false)).Any<object>();
  }

  internal Expression AddProfiling(
    Expression body,
    ParameterExpression tick,
    string name,
    bool unique)
  {
    int profilerIndex = this.GetProfilerIndex(name);
    return this.AddOuterProfiling(((ExpressionVisitor) new Profiler.InnerMethodProfiler(this, tick, profilerIndex)).Visit(body), tick, profilerIndex);
  }

  internal Expression AddProfiling(Expression body, MethodBase method)
  {
    if (method is DynamicMethod || Profiler.IgnoreMethod(method))
      return body;
    int profilerIndex = this.GetProfilerIndex(method);
    ParameterExpression tick = Expression.Variable(typeof (long), "$tick");
    return (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      tick
    }, this.AddOuterProfiling(body, tick, profilerIndex));
  }

  private sealed class InnerMethodProfiler : DynamicExpressionVisitor
  {
    private readonly Profiler _profiler;
    private readonly ParameterExpression _tick;
    private readonly int _profileIndex;

    public InnerMethodProfiler(Profiler profiler, ParameterExpression tick, int profileIndex)
    {
      this._profiler = profiler;
      this._tick = tick;
      this._profileIndex = profileIndex;
    }

    protected virtual Expression VisitDynamic(DynamicExpression node)
    {
      return this._profiler.AddInnerProfiling((Expression) node, this._tick, this._profileIndex);
    }

    protected virtual Expression VisitExtension(Expression node)
    {
      // ISSUE: explicit non-virtual call
      return node is ReducableDynamicExpression ? this._profiler.AddInnerProfiling(node, this._tick, this._profileIndex) : __nonvirtual (((ExpressionVisitor) this).VisitExtension(node));
    }

    protected virtual Expression VisitMethodCall(MethodCallExpression node)
    {
      // ISSUE: explicit non-virtual call
      Expression expression = __nonvirtual (((ExpressionVisitor) this).VisitMethodCall(node));
      return Profiler.IgnoreMethod((MethodBase) node.Method) ? this._profiler.AddInnerProfiling((Expression) node, this._tick, this._profileIndex) : expression;
    }

    protected virtual Expression VisitLambda<T>(Expression<T> node) => (Expression) node;
  }

  public struct Data(string _name, long _inclusive, long _exclusive, int _calls)
  {
    public string Name = _name;
    public long InclusiveTime = _inclusive;
    public long ExclusiveTime = _exclusive;
    public int Calls = _calls;
  }
}
