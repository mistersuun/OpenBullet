// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.DebugFrame
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Debugging.CompilerServices;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Debugging;

[DebuggerDisplay("FunctionInfo = {_funcInfo.Name}, CurrentSequencePointIndex = {CurrentSequencePointIndex}")]
public sealed class DebugFrame
{
  private FunctionInfo _funcInfo;
  private Exception _thrownException;
  private IRuntimeVariables _liftedLocals;
  private IDebuggableGenerator _generator;
  private int _lastKnownGeneratorYieldMarker = int.MaxValue;
  private bool _inTraceBack;
  private bool _inGeneratorLoop;
  private bool _forceToGeneratorLoop;
  private Dictionary<IList<VariableInfo>, DebugFrame.ScopeData> _variables;
  private const string _exceptionVariableSymbol = "$debugException";

  internal DebugFrame(DebugThread thread, FunctionInfo funcInfo)
  {
    this.Thread = thread;
    this._funcInfo = funcInfo;
    this._variables = new Dictionary<IList<VariableInfo>, DebugFrame.ScopeData>();
  }

  internal DebugFrame(
    DebugThread thread,
    FunctionInfo funcInfo,
    IRuntimeVariables liftedLocals,
    int frameOrder)
    : this(thread, funcInfo)
  {
    this._liftedLocals = liftedLocals;
    this.StackDepth = frameOrder;
  }

  internal DebugThread Thread { get; }

  internal int StackDepth { get; set; }

  internal VariableInfo[] Variables
  {
    get
    {
      DebugFrame.ScopeData currentScopeData = this.CurrentScopeData;
      VariableInfo[] variables = this._thrownException != null ? currentScopeData.VarInfosWithException : currentScopeData.VarInfos;
      if (variables == null)
      {
        List<VariableInfo> collection = new List<VariableInfo>();
        foreach (VariableInfo variable in (IEnumerable<VariableInfo>) this._funcInfo.Variables)
        {
          if (variable.IsParameter && !variable.Hidden)
            collection.Add(variable);
        }
        foreach (VariableInfo variableInfo in (IEnumerable<VariableInfo>) this.LocalsInCurrentScope)
        {
          if (!variableInfo.Hidden)
            collection.Add(variableInfo);
        }
        List<VariableInfo> variableInfoList = new List<VariableInfo>((IEnumerable<VariableInfo>) collection);
        variableInfoList.Add(new VariableInfo("$debugException", typeof (Exception), false, false, false));
        currentScopeData.VarInfos = collection.ToArray();
        currentScopeData.VarInfosWithException = variableInfoList.ToArray();
        variables = this._thrownException == null ? currentScopeData.VarInfos : currentScopeData.VarInfosWithException;
      }
      return variables;
    }
  }

  internal int CurrentSequencePointIndex
  {
    get
    {
      int sequencePointIndex = this.CurrentLocationCookie;
      if (sequencePointIndex >= this._funcInfo.SequencePoints.Length)
        sequencePointIndex = 0;
      return sequencePointIndex;
    }
    set
    {
      if (value < 0 || value >= this._funcInfo.SequencePoints.Length)
        throw new ArgumentOutOfRangeException(nameof (value));
      if (!this._inTraceBack)
        throw new InvalidOperationException(ErrorStrings.JumpNotAllowedInNonLeafFrames);
      bool flag = value != this.CurrentLocationCookie || this._thrownException != null;
      if (this._generator == null & flag)
        this.RemapToGenerator(this._funcInfo.Version);
      if (value != this.CurrentLocationCookie)
        this._generator.YieldMarkerLocation = value;
      this.ThrownException = (Exception) null;
      if (!(!this._inGeneratorLoop & flag))
        return;
      this._forceToGeneratorLoop = true;
    }
  }

  internal void RemapToLatestVersion()
  {
    this.RemapToGenerator(int.MaxValue);
    if (this._inGeneratorLoop)
      return;
    this._forceToGeneratorLoop = true;
  }

  internal FunctionInfo FunctionInfo => this._funcInfo;

  internal Exception ThrownException
  {
    get => this._thrownException;
    set
    {
      if (this._thrownException != null && value == null)
      {
        this._thrownException = (Exception) null;
        this.GetLocalsScope().Remove((object) "$debugException");
      }
      else
      {
        if (value == null || this.GetLocalsScope().ContainsKey((object) "$debugException"))
          return;
        this._thrownException = value;
        this.GetLocalsScope()[(object) "$debugException"] = (object) this._thrownException;
      }
    }
  }

  internal IDebuggableGenerator Generator => this._generator;

  internal bool IsInTraceback
  {
    get => this._inTraceBack;
    set => this._inTraceBack = value;
  }

  internal bool InGeneratorLoop
  {
    get => this._inGeneratorLoop;
    set => this._inGeneratorLoop = value;
  }

  internal bool ForceSwitchToGeneratorLoop
  {
    get => this._forceToGeneratorLoop;
    set => this._forceToGeneratorLoop = value;
  }

  internal DebugContext DebugContext => this.Thread.DebugContext;

  internal int CurrentLocationCookie
  {
    get
    {
      if (this._generator == null)
        return ((IDebugRuntimeVariables) this._liftedLocals).DebugMarker;
      return this._generator.YieldMarkerLocation == int.MaxValue ? this._lastKnownGeneratorYieldMarker : this._generator.YieldMarkerLocation;
    }
  }

  internal int LastKnownGeneratorYieldMarker
  {
    get => this._lastKnownGeneratorYieldMarker;
    set => this._lastKnownGeneratorYieldMarker = value;
  }

  internal void ReplaceLiftedLocals(IRuntimeVariables liftedLocals)
  {
    IRuntimeVariables liftedLocals1 = this._liftedLocals;
    this._liftedLocals = liftedLocals;
    if (liftedLocals1 != null)
    {
      for (int index = 0; index < liftedLocals1.Count; ++index)
      {
        if (!this._funcInfo.Variables[index].IsParameter && index < this._liftedLocals.Count)
          this._liftedLocals[index] = liftedLocals1[index];
      }
    }
    this._variables.Clear();
  }

  internal void RemapToGenerator(int version)
  {
    this.CreateGenerator(this.GetFunctionInfo(version) ?? throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ErrorStrings.InvalidFunctionVersion, (object) version)));
    ((IEnumerator) this._generator).MoveNext();
  }

  internal IDictionary<object, object> GetLocalsScope()
  {
    DebugFrame.ScopeData currentScopeData = this.CurrentScopeData;
    IDictionary<object, object> localsScope = currentScopeData.Scope;
    if (localsScope == null)
    {
      List<string> stringList = new List<string>();
      List<VariableInfo> variableInfos = new List<VariableInfo>();
      for (int index = 0; index < this._funcInfo.Variables.Count; ++index)
      {
        if (this._funcInfo.Variables[index].IsParameter && !this._funcInfo.Variables[index].Hidden)
        {
          stringList.Add(this._funcInfo.Variables[index].Name);
          variableInfos.Add(this._funcInfo.Variables[index]);
        }
      }
      foreach (VariableInfo variableInfo in (IEnumerable<VariableInfo>) this.LocalsInCurrentScope)
      {
        if (!variableInfo.Hidden)
        {
          stringList.Add(variableInfo.Name);
          variableInfos.Add(variableInfo);
        }
      }
      localsScope = (IDictionary<object, object>) new LocalsDictionary((IRuntimeVariables) new ScopedRuntimeVariables((IList<VariableInfo>) variableInfos, this._liftedLocals), stringList.ToArray());
      currentScopeData.Scope = localsScope;
    }
    return localsScope;
  }

  private void CreateGenerator(FunctionInfo targetFuncInfo)
  {
    object[] valuesForGenerator = this.GetParamValuesForGenerator();
    this._generator = (IDebuggableGenerator) targetFuncInfo.GeneratorFactory.GetType().GetMethod("Invoke").Invoke((object) targetFuncInfo.GeneratorFactory, valuesForGenerator);
    if (this._funcInfo == targetFuncInfo)
      return;
    this._funcInfo = targetFuncInfo;
  }

  private object[] GetParamValuesForGenerator()
  {
    List<object> objectList = new List<object>();
    objectList.Add((object) this);
    for (int index = 0; index < this._funcInfo.Variables.Count; ++index)
    {
      if (this._funcInfo.Variables[index].IsParameter)
        objectList.Add(this._liftedLocals[index]);
    }
    return objectList.ToArray();
  }

  private FunctionInfo GetFunctionInfo(int version)
  {
    if (version == this._funcInfo.Version)
      return this._funcInfo;
    FunctionInfo functionInfo1 = this._funcInfo;
    FunctionInfo functionInfo2 = (FunctionInfo) null;
    for (; functionInfo1 != null; functionInfo1 = version <= functionInfo1.Version ? functionInfo1.PreviousVersion : functionInfo1.NextVersion)
    {
      if (functionInfo1.Version == version)
        return functionInfo1;
      functionInfo2 = functionInfo1;
    }
    return version == int.MaxValue ? functionInfo2 : (FunctionInfo) null;
  }

  private DebugFrame.ScopeData CurrentScopeData
  {
    get
    {
      IList<VariableInfo> key = (this.CurrentLocationCookie < this._funcInfo.VariableScopeMap.Length ? this._funcInfo.VariableScopeMap[this.CurrentLocationCookie] : (IList<VariableInfo>) null) ?? this._funcInfo.VariableScopeMap[0];
      DebugFrame.ScopeData currentScopeData;
      if (!this._variables.TryGetValue(key, out currentScopeData))
      {
        currentScopeData = new DebugFrame.ScopeData();
        this._variables.Add(key, currentScopeData);
      }
      return currentScopeData;
    }
  }

  private IList<VariableInfo> LocalsInCurrentScope
  {
    get
    {
      return (this.CurrentLocationCookie < this._funcInfo.VariableScopeMap.Length ? this._funcInfo.VariableScopeMap[this.CurrentLocationCookie] : (IList<VariableInfo>) null) ?? this._funcInfo.VariableScopeMap[0];
    }
  }

  private class ScopeData
  {
    public VariableInfo[] VarInfos;
    public VariableInfo[] VarInfosWithException;
    public IDictionary<object, object> Scope;
  }
}
