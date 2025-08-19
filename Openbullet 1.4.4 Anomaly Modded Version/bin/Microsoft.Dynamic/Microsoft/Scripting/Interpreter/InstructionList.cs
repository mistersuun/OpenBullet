// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.InstructionList
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

[DebuggerTypeProxy(typeof (InstructionList.DebugView))]
public sealed class InstructionList
{
  private readonly List<Instruction> _instructions = new List<Instruction>();
  private List<object> _objects;
  private int _currentStackDepth;
  private int _maxStackDepth;
  private int _currentContinuationsDepth;
  private int _maxContinuationDepth;
  private int _runtimeLabelCount;
  private List<BranchLabel> _labels;
  private List<KeyValuePair<int, object>> _debugCookies;
  private const int PushIntMinCachedValue = -100;
  private const int PushIntMaxCachedValue = 100;
  private const int CachedObjectCount = 256 /*0x0100*/;
  private static Instruction _null;
  private static Instruction _true;
  private static Instruction _false;
  private static Instruction[] _ints;
  private static Instruction[] _loadObjectCached;
  private const int LocalInstrCacheSize = 64 /*0x40*/;
  private static Instruction[] _loadLocal;
  private static Instruction[] _loadLocalBoxed;
  private static Instruction[] _loadLocalFromClosure;
  private static Instruction[] _loadLocalFromClosureBoxed;
  private static Instruction[] _assignLocal;
  private static Instruction[] _storeLocal;
  private static Instruction[] _assignLocalBoxed;
  private static Instruction[] _storeLocalBoxed;
  private static Instruction[] _assignLocalToClosure;
  private static Instruction[] _initReference;
  private static Instruction[] _initImmutableRefBox;
  private static Instruction[] _parameterBox;
  private static Instruction[] _parameter;
  private static readonly Dictionary<FieldInfo, Instruction> _loadFields = new Dictionary<FieldInfo, Instruction>();
  private static Dictionary<Type, Func<CallSiteBinder, Instruction>> _factories = new Dictionary<Type, Func<CallSiteBinder, Instruction>>();
  private static readonly RuntimeLabel[] EmptyRuntimeLabels = new RuntimeLabel[1]
  {
    new RuntimeLabel(int.MaxValue, 0, 0)
  };

  public void Emit(Instruction instruction)
  {
    this._instructions.Add(instruction);
    this.UpdateStackDepth(instruction);
  }

  private void UpdateStackDepth(Instruction instruction)
  {
    this._currentStackDepth -= instruction.ConsumedStack;
    this._currentStackDepth += instruction.ProducedStack;
    if (this._currentStackDepth > this._maxStackDepth)
      this._maxStackDepth = this._currentStackDepth;
    this._currentContinuationsDepth -= instruction.ConsumedContinuations;
    this._currentContinuationsDepth += instruction.ProducedContinuations;
    if (this._currentContinuationsDepth <= this._maxContinuationDepth)
      return;
    this._maxContinuationDepth = this._currentContinuationsDepth;
  }

  [Conditional("DEBUG")]
  public void SetDebugCookie(object cookie)
  {
  }

  public int Count => this._instructions.Count;

  public int CurrentStackDepth => this._currentStackDepth;

  public int CurrentContinuationsDepth => this._currentContinuationsDepth;

  public int MaxStackDepth => this._maxStackDepth;

  internal Instruction GetInstruction(int index) => this._instructions[index];

  public InstructionArray ToArray()
  {
    return new InstructionArray(this._maxStackDepth, this._maxContinuationDepth, this._instructions.ToArray(), this._objects?.ToArray(), this.BuildRuntimeLabels(), this._debugCookies);
  }

  public void EmitLoad(object value) => this.EmitLoad(value, (Type) null);

  public void EmitLoad(bool value)
  {
    if (value)
      this.Emit(InstructionList._true ?? (InstructionList._true = (Instruction) new LoadObjectInstruction((object) value)));
    else
      this.Emit(InstructionList._false ?? (InstructionList._false = (Instruction) new LoadObjectInstruction((object) value)));
  }

  public void EmitLoad(object value, Type type)
  {
    if (value == null)
    {
      this.Emit(InstructionList._null ?? (InstructionList._null = (Instruction) new LoadObjectInstruction((object) null)));
    }
    else
    {
      if (type == (Type) null || type.IsValueType())
      {
        object obj;
        if ((obj = value) is bool)
        {
          this.EmitLoad((bool) obj);
          return;
        }
        if (value is int num && num >= -100 && num <= 100)
        {
          if (InstructionList._ints == null)
            InstructionList._ints = new Instruction[201];
          int index = num - -100;
          this.Emit(InstructionList._ints[index] ?? (InstructionList._ints[index] = (Instruction) new LoadObjectInstruction(value)));
          return;
        }
      }
      if (this._objects == null)
      {
        this._objects = new List<object>();
        if (InstructionList._loadObjectCached == null)
          InstructionList._loadObjectCached = new Instruction[256 /*0x0100*/];
      }
      if (this._objects.Count < InstructionList._loadObjectCached.Length)
      {
        uint count = (uint) this._objects.Count;
        this._objects.Add(value);
        this.Emit(InstructionList._loadObjectCached[(int) count] ?? (InstructionList._loadObjectCached[(int) count] = (Instruction) new LoadCachedObjectInstruction(count)));
      }
      else
        this.Emit((Instruction) new LoadObjectInstruction(value));
    }
  }

  public void EmitDup() => this.Emit((Instruction) DupInstruction.Instance);

  public void EmitPop() => this.Emit((Instruction) PopInstruction.Instance);

  internal void SwitchToBoxed(int index, int instructionIndex)
  {
    if (!(this._instructions[instructionIndex] is IBoxableInstruction instruction1))
      return;
    Instruction instruction2 = instruction1.BoxIfIndexMatches(index);
    if (instruction2 == null)
      return;
    this._instructions[instructionIndex] = instruction2;
  }

  public void EmitLoadLocal(int index)
  {
    if (InstructionList._loadLocal == null)
      InstructionList._loadLocal = new Instruction[64 /*0x40*/];
    if (index < InstructionList._loadLocal.Length)
      this.Emit(InstructionList._loadLocal[index] ?? (InstructionList._loadLocal[index] = (Instruction) new LoadLocalInstruction(index)));
    else
      this.Emit((Instruction) new LoadLocalInstruction(index));
  }

  public void EmitLoadLocalBoxed(int index) => this.Emit(InstructionList.LoadLocalBoxed(index));

  internal static Instruction LoadLocalBoxed(int index)
  {
    if (InstructionList._loadLocalBoxed == null)
      InstructionList._loadLocalBoxed = new Instruction[64 /*0x40*/];
    return index < InstructionList._loadLocalBoxed.Length ? InstructionList._loadLocalBoxed[index] ?? (InstructionList._loadLocalBoxed[index] = (Instruction) new LoadLocalBoxedInstruction(index)) : (Instruction) new LoadLocalBoxedInstruction(index);
  }

  public void EmitLoadLocalFromClosure(int index)
  {
    if (InstructionList._loadLocalFromClosure == null)
      InstructionList._loadLocalFromClosure = new Instruction[64 /*0x40*/];
    if (index < InstructionList._loadLocalFromClosure.Length)
      this.Emit(InstructionList._loadLocalFromClosure[index] ?? (InstructionList._loadLocalFromClosure[index] = (Instruction) new LoadLocalFromClosureInstruction(index)));
    else
      this.Emit((Instruction) new LoadLocalFromClosureInstruction(index));
  }

  public void EmitLoadLocalFromClosureBoxed(int index)
  {
    if (InstructionList._loadLocalFromClosureBoxed == null)
      InstructionList._loadLocalFromClosureBoxed = new Instruction[64 /*0x40*/];
    if (index < InstructionList._loadLocalFromClosureBoxed.Length)
      this.Emit(InstructionList._loadLocalFromClosureBoxed[index] ?? (InstructionList._loadLocalFromClosureBoxed[index] = (Instruction) new LoadLocalFromClosureBoxedInstruction(index)));
    else
      this.Emit((Instruction) new LoadLocalFromClosureBoxedInstruction(index));
  }

  public void EmitAssignLocal(int index)
  {
    if (InstructionList._assignLocal == null)
      InstructionList._assignLocal = new Instruction[64 /*0x40*/];
    if (index < InstructionList._assignLocal.Length)
      this.Emit(InstructionList._assignLocal[index] ?? (InstructionList._assignLocal[index] = (Instruction) new AssignLocalInstruction(index)));
    else
      this.Emit((Instruction) new AssignLocalInstruction(index));
  }

  public void EmitStoreLocal(int index)
  {
    if (InstructionList._storeLocal == null)
      InstructionList._storeLocal = new Instruction[64 /*0x40*/];
    if (index < InstructionList._storeLocal.Length)
      this.Emit(InstructionList._storeLocal[index] ?? (InstructionList._storeLocal[index] = (Instruction) new StoreLocalInstruction(index)));
    else
      this.Emit((Instruction) new StoreLocalInstruction(index));
  }

  public void EmitAssignLocalBoxed(int index) => this.Emit(InstructionList.AssignLocalBoxed(index));

  internal static Instruction AssignLocalBoxed(int index)
  {
    if (InstructionList._assignLocalBoxed == null)
      InstructionList._assignLocalBoxed = new Instruction[64 /*0x40*/];
    return index < InstructionList._assignLocalBoxed.Length ? InstructionList._assignLocalBoxed[index] ?? (InstructionList._assignLocalBoxed[index] = (Instruction) new AssignLocalBoxedInstruction(index)) : (Instruction) new AssignLocalBoxedInstruction(index);
  }

  public void EmitStoreLocalBoxed(int index) => this.Emit(InstructionList.StoreLocalBoxed(index));

  internal static Instruction StoreLocalBoxed(int index)
  {
    if (InstructionList._storeLocalBoxed == null)
      InstructionList._storeLocalBoxed = new Instruction[64 /*0x40*/];
    return index < InstructionList._storeLocalBoxed.Length ? InstructionList._storeLocalBoxed[index] ?? (InstructionList._storeLocalBoxed[index] = (Instruction) new StoreLocalBoxedInstruction(index)) : (Instruction) new StoreLocalBoxedInstruction(index);
  }

  public void EmitAssignLocalToClosure(int index)
  {
    if (InstructionList._assignLocalToClosure == null)
      InstructionList._assignLocalToClosure = new Instruction[64 /*0x40*/];
    if (index < InstructionList._assignLocalToClosure.Length)
      this.Emit(InstructionList._assignLocalToClosure[index] ?? (InstructionList._assignLocalToClosure[index] = (Instruction) new AssignLocalToClosureInstruction(index)));
    else
      this.Emit((Instruction) new AssignLocalToClosureInstruction(index));
  }

  public void EmitStoreLocalToClosure(int index)
  {
    this.EmitAssignLocalToClosure(index);
    this.EmitPop();
  }

  public void EmitInitializeLocal(int index, Type type)
  {
    object primitiveDefaultValue = ScriptingRuntimeHelpers.GetPrimitiveDefaultValue(type);
    if (primitiveDefaultValue != null)
      this.Emit((Instruction) new InitializeLocalInstruction.ImmutableValue(index, primitiveDefaultValue));
    else if (type.IsValueType())
      this.Emit((Instruction) new InitializeLocalInstruction.MutableValue(index, type));
    else
      this.Emit(InstructionList.InitReference(index));
  }

  internal void EmitInitializeParameter(int index) => this.Emit(InstructionList.Parameter(index));

  internal static Instruction Parameter(int index)
  {
    if (InstructionList._parameter == null)
      InstructionList._parameter = new Instruction[64 /*0x40*/];
    return index < InstructionList._parameter.Length ? InstructionList._parameter[index] ?? (InstructionList._parameter[index] = (Instruction) new InitializeLocalInstruction.Parameter(index)) : (Instruction) new InitializeLocalInstruction.Parameter(index);
  }

  internal static Instruction ParameterBox(int index)
  {
    if (InstructionList._parameterBox == null)
      InstructionList._parameterBox = new Instruction[64 /*0x40*/];
    return index < InstructionList._parameterBox.Length ? InstructionList._parameterBox[index] ?? (InstructionList._parameterBox[index] = (Instruction) new InitializeLocalInstruction.ParameterBox(index)) : (Instruction) new InitializeLocalInstruction.ParameterBox(index);
  }

  internal static Instruction InitReference(int index)
  {
    if (InstructionList._initReference == null)
      InstructionList._initReference = new Instruction[64 /*0x40*/];
    return index < InstructionList._initReference.Length ? InstructionList._initReference[index] ?? (InstructionList._initReference[index] = (Instruction) new InitializeLocalInstruction.Reference(index)) : (Instruction) new InitializeLocalInstruction.Reference(index);
  }

  internal static Instruction InitImmutableRefBox(int index)
  {
    if (InstructionList._initImmutableRefBox == null)
      InstructionList._initImmutableRefBox = new Instruction[64 /*0x40*/];
    return index < InstructionList._initImmutableRefBox.Length ? InstructionList._initImmutableRefBox[index] ?? (InstructionList._initImmutableRefBox[index] = (Instruction) new InitializeLocalInstruction.ImmutableBox(index, (object) null)) : (Instruction) new InitializeLocalInstruction.ImmutableBox(index, (object) null);
  }

  public void EmitNewRuntimeVariables(int count)
  {
    this.Emit((Instruction) new RuntimeVariablesInstruction(count));
  }

  public void EmitGetArrayItem(Type arrayType)
  {
    Type elementType = arrayType.GetElementType();
    if (elementType.IsClass() || elementType.IsInterface())
      this.Emit(InstructionFactory<object>.Factory.GetArrayItem());
    else
      this.Emit(InstructionFactory.GetFactory(elementType).GetArrayItem());
  }

  public void EmitSetArrayItem(Type arrayType)
  {
    Type elementType = arrayType.GetElementType();
    if (elementType.IsClass() || elementType.IsInterface())
      this.Emit(InstructionFactory<object>.Factory.SetArrayItem());
    else
      this.Emit(InstructionFactory.GetFactory(elementType).SetArrayItem());
  }

  public void EmitNewArray(Type elementType)
  {
    this.Emit(InstructionFactory.GetFactory(elementType).NewArray());
  }

  public void EmitNewArrayBounds(Type elementType, int rank)
  {
    this.Emit((Instruction) new NewArrayBoundsInstruction(elementType, rank));
  }

  public void EmitNewArrayInit(Type elementType, int elementCount)
  {
    this.Emit(InstructionFactory.GetFactory(elementType).NewArrayInit(elementCount));
  }

  public void EmitAdd(Type type, bool @checked)
  {
    if (@checked)
      this.Emit(AddOvfInstruction.Create(type));
    else
      this.Emit(AddInstruction.Create(type));
  }

  public void EmitSub(Type type, bool @checked) => throw new NotSupportedException();

  public void EmitMul(Type type, bool @checked) => throw new NotSupportedException();

  public void EmitDiv(Type type) => this.Emit(DivInstruction.Create(type));

  public void EmitEqual(Type type) => this.Emit(EqualInstruction.Create(type));

  public void EmitNotEqual(Type type) => this.Emit(NotEqualInstruction.Create(type));

  public void EmitLessThan(Type type) => this.Emit(LessThanInstruction.Create(type));

  public void EmitLessThanOrEqual(Type type) => throw new NotSupportedException();

  public void EmitGreaterThan(Type type) => this.Emit(GreaterThanInstruction.Create(type));

  public void EmitGreaterThanOrEqual(Type type) => throw new NotSupportedException();

  public void EmitNumericConvertChecked(TypeCode from, TypeCode to)
  {
    this.Emit((Instruction) new NumericConvertInstruction.Checked(from, to));
  }

  public void EmitNumericConvertUnchecked(TypeCode from, TypeCode to)
  {
    this.Emit((Instruction) new NumericConvertInstruction.Unchecked(from, to));
  }

  public void EmitNot() => this.Emit(NotInstruction.Instance);

  public void EmitDefaultValue(Type type)
  {
    this.Emit(InstructionFactory.GetFactory(type).DefaultValue());
  }

  public void EmitNew(ConstructorInfo constructorInfo)
  {
    this.Emit((Instruction) new NewInstruction(constructorInfo));
  }

  internal void EmitCreateDelegate(LightDelegateCreator creator)
  {
    this.Emit((Instruction) new CreateDelegateInstruction(creator));
  }

  public void EmitTypeEquals() => this.Emit((Instruction) TypeEqualsInstruction.Instance);

  public void EmitTypeIs(Type type) => this.Emit(InstructionFactory.GetFactory(type).TypeIs());

  public void EmitTypeAs(Type type) => this.Emit(InstructionFactory.GetFactory(type).TypeAs());

  public void EmitLoadField(FieldInfo field) => this.Emit(this.GetLoadField(field));

  private Instruction GetLoadField(FieldInfo field)
  {
    lock (InstructionList._loadFields)
    {
      Instruction loadField1;
      if (InstructionList._loadFields.TryGetValue(field, out loadField1))
        return loadField1;
      Instruction loadField2 = !field.IsStatic ? (Instruction) new LoadFieldInstruction(field) : (Instruction) new LoadStaticFieldInstruction(field);
      InstructionList._loadFields.Add(field, loadField2);
      return loadField2;
    }
  }

  public void EmitStoreField(FieldInfo field)
  {
    if (field.IsStatic)
      this.Emit((Instruction) new StoreStaticFieldInstruction(field));
    else
      this.Emit((Instruction) new StoreFieldInstruction(field));
  }

  public void EmitDynamic(Type type, CallSiteBinder binder)
  {
    this.Emit(InstructionList.CreateDynamicInstruction(type, binder));
  }

  public void EmitDynamic<T0, TRet>(CallSiteBinder binder)
  {
    this.Emit(DynamicInstruction<T0, TRet>.Factory(binder));
  }

  public void EmitDynamic<T0, T1, TRet>(CallSiteBinder binder)
  {
    this.Emit(DynamicInstruction<T0, T1, TRet>.Factory(binder));
  }

  public void EmitDynamic<T0, T1, T2, TRet>(CallSiteBinder binder)
  {
    this.Emit(DynamicInstruction<T0, T1, T2, TRet>.Factory(binder));
  }

  public void EmitDynamic<T0, T1, T2, T3, TRet>(CallSiteBinder binder)
  {
    this.Emit(DynamicInstruction<T0, T1, T2, T3, TRet>.Factory(binder));
  }

  public void EmitDynamic<T0, T1, T2, T3, T4, TRet>(CallSiteBinder binder)
  {
    this.Emit(DynamicInstruction<T0, T1, T2, T3, T4, TRet>.Factory(binder));
  }

  public void EmitDynamic<T0, T1, T2, T3, T4, T5, TRet>(CallSiteBinder binder)
  {
    this.Emit(DynamicInstruction<T0, T1, T2, T3, T4, T5, TRet>.Factory(binder));
  }

  public void EmitDynamic<T0, T1, T2, T3, T4, T5, T6, TRet>(CallSiteBinder binder)
  {
    this.Emit(DynamicInstruction<T0, T1, T2, T3, T4, T5, T6, TRet>.Factory(binder));
  }

  public void EmitDynamic<T0, T1, T2, T3, T4, T5, T6, T7, TRet>(CallSiteBinder binder)
  {
    this.Emit(DynamicInstruction<T0, T1, T2, T3, T4, T5, T6, T7, TRet>.Factory(binder));
  }

  public void EmitDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>(CallSiteBinder binder)
  {
    this.Emit(DynamicInstruction<T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>.Factory(binder));
  }

  public void EmitDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>(CallSiteBinder binder)
  {
    this.Emit(DynamicInstruction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>.Factory(binder));
  }

  public void EmitDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet>(CallSiteBinder binder)
  {
    this.Emit(DynamicInstruction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet>.Factory(binder));
  }

  public void EmitDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRet>(
    CallSiteBinder binder)
  {
    this.Emit(DynamicInstruction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRet>.Factory(binder));
  }

  public void EmitDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet>(
    CallSiteBinder binder)
  {
    this.Emit(DynamicInstruction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet>.Factory(binder));
  }

  public void EmitDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRet>(
    CallSiteBinder binder)
  {
    this.Emit(DynamicInstruction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRet>.Factory(binder));
  }

  public void EmitDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRet>(
    CallSiteBinder binder)
  {
    this.Emit(DynamicInstruction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRet>.Factory(binder));
  }

  internal static Instruction CreateDynamicInstruction(Type delegateType, CallSiteBinder binder)
  {
    Func<CallSiteBinder, Instruction> func;
    lock (InstructionList._factories)
    {
      if (!InstructionList._factories.TryGetValue(delegateType, out func))
      {
        if (delegateType.GetMethod("Invoke").ReturnType == typeof (void))
          return (Instruction) new DynamicInstructionN(delegateType, CallSite.Create(delegateType, binder), true);
        Type dynamicInstructionType = DynamicInstructionN.GetDynamicInstructionType(delegateType);
        if (dynamicInstructionType == (Type) null)
          return (Instruction) new DynamicInstructionN(delegateType, CallSite.Create(delegateType, binder));
        func = (Func<CallSiteBinder, Instruction>) dynamicInstructionType.GetMethod("Factory").CreateDelegate(typeof (Func<CallSiteBinder, Instruction>));
        InstructionList._factories[delegateType] = func;
      }
    }
    return func(binder);
  }

  private RuntimeLabel[] BuildRuntimeLabels()
  {
    if (this._runtimeLabelCount == 0)
      return InstructionList.EmptyRuntimeLabels;
    RuntimeLabel[] runtimeLabelArray = new RuntimeLabel[this._runtimeLabelCount + 1];
    foreach (BranchLabel label in this._labels)
    {
      if (label.HasRuntimeLabel)
        runtimeLabelArray[label.LabelIndex] = label.ToRuntimeLabel();
    }
    runtimeLabelArray[runtimeLabelArray.Length - 1] = new RuntimeLabel(int.MaxValue, 0, 0);
    return runtimeLabelArray;
  }

  public BranchLabel MakeLabel()
  {
    if (this._labels == null)
      this._labels = new List<BranchLabel>();
    BranchLabel branchLabel = new BranchLabel();
    this._labels.Add(branchLabel);
    return branchLabel;
  }

  internal void FixupBranch(int branchIndex, int offset)
  {
    this._instructions[branchIndex] = ((OffsetInstruction) this._instructions[branchIndex]).Fixup(offset);
  }

  private int EnsureLabelIndex(BranchLabel label)
  {
    if (label.HasRuntimeLabel)
      return label.LabelIndex;
    label.LabelIndex = this._runtimeLabelCount;
    ++this._runtimeLabelCount;
    return label.LabelIndex;
  }

  public int MarkRuntimeLabel()
  {
    BranchLabel label = this.MakeLabel();
    this.MarkLabel(label);
    return this.EnsureLabelIndex(label);
  }

  public void MarkLabel(BranchLabel label) => label.Mark(this);

  public void EmitGoto(BranchLabel label, bool hasResult, bool hasValue)
  {
    this.Emit((Instruction) GotoInstruction.Create(this.EnsureLabelIndex(label), hasResult, hasValue));
  }

  private void EmitBranch(OffsetInstruction instruction, BranchLabel label)
  {
    this.Emit((Instruction) instruction);
    label.AddBranch(this, this.Count - 1);
  }

  public void EmitBranch(BranchLabel label)
  {
    this.EmitBranch((OffsetInstruction) new BranchInstruction(), label);
  }

  public void EmitBranch(BranchLabel label, bool hasResult, bool hasValue)
  {
    this.EmitBranch((OffsetInstruction) new BranchInstruction(hasResult, hasValue), label);
  }

  public void EmitCoalescingBranch(BranchLabel leftNotNull)
  {
    this.EmitBranch((OffsetInstruction) new CoalescingBranchInstruction(), leftNotNull);
  }

  public void EmitBranchTrue(BranchLabel elseLabel)
  {
    this.EmitBranch((OffsetInstruction) new BranchTrueInstruction(), elseLabel);
  }

  public void EmitBranchFalse(BranchLabel elseLabel)
  {
    this.EmitBranch((OffsetInstruction) new BranchFalseInstruction(), elseLabel);
  }

  public void EmitThrow() => this.Emit((Instruction) ThrowInstruction.Throw);

  public void EmitThrowVoid() => this.Emit((Instruction) ThrowInstruction.VoidThrow);

  public void EmitRethrow() => this.Emit((Instruction) ThrowInstruction.Rethrow);

  public void EmitRethrowVoid() => this.Emit((Instruction) ThrowInstruction.VoidRethrow);

  public void EmitEnterTryFinally(BranchLabel finallyStartLabel)
  {
    this.Emit((Instruction) EnterTryFinallyInstruction.Create(this.EnsureLabelIndex(finallyStartLabel)));
  }

  public void EmitEnterFinally() => this.Emit(EnterFinallyInstruction.Instance);

  public void EmitLeaveFinally() => this.Emit(LeaveFinallyInstruction.Instance);

  public void EmitLeaveFault(bool hasValue)
  {
    this.Emit(hasValue ? LeaveFaultInstruction.NonVoid : LeaveFaultInstruction.Void);
  }

  public void EmitEnterExceptionHandlerNonVoid()
  {
    this.Emit((Instruction) EnterExceptionHandlerInstruction.NonVoid);
  }

  public void EmitEnterExceptionHandlerVoid()
  {
    this.Emit((Instruction) EnterExceptionHandlerInstruction.Void);
  }

  public void EmitLeaveExceptionHandler(bool hasValue, BranchLabel tryExpressionEndLabel)
  {
    this.Emit((Instruction) LeaveExceptionHandlerInstruction.Create(this.EnsureLabelIndex(tryExpressionEndLabel), hasValue));
  }

  public void EmitSwitch(Dictionary<int, int> cases)
  {
    this.Emit((Instruction) new SwitchInstruction(cases));
  }

  internal sealed class DebugView
  {
    private readonly InstructionList _list;

    public DebugView(InstructionList list) => this._list = list;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public InstructionList.DebugView.InstructionView[] A0
    {
      get
      {
        return InstructionList.DebugView.GetInstructionViews((IList<Instruction>) this._list._instructions, (IList<object>) this._list._objects, (Func<int, int>) (index => this._list._labels[index].TargetIndex), (IList<KeyValuePair<int, object>>) this._list._debugCookies);
      }
    }

    internal static InstructionList.DebugView.InstructionView[] GetInstructionViews(
      IList<Instruction> instructions,
      IList<object> objects,
      Func<int, int> labelIndexer,
      IList<KeyValuePair<int, object>> debugCookies)
    {
      List<InstructionList.DebugView.InstructionView> instructionViewList = new List<InstructionList.DebugView.InstructionView>();
      int stackDepth = 0;
      int continuationsDepth = 0;
      using (IEnumerator<KeyValuePair<int, object>> enumerator = ((IEnumerable<KeyValuePair<int, object>>) ((object) debugCookies ?? (object) new KeyValuePair<int, object>[0])).GetEnumerator())
      {
        bool flag = enumerator.MoveNext();
        for (int index = 0; index < instructions.Count; ++index)
        {
          object cookie = (object) null;
          for (; flag; flag = enumerator.MoveNext())
          {
            KeyValuePair<int, object> current = enumerator.Current;
            if (current.Key == index)
            {
              current = enumerator.Current;
              cookie = current.Value;
            }
            else
              break;
          }
          int stackBalance = instructions[index].StackBalance;
          int continuationsBalance = instructions[index].ContinuationsBalance;
          string debugString = instructions[index].ToDebugString(index, cookie, labelIndexer, objects);
          instructionViewList.Add(new InstructionList.DebugView.InstructionView(instructions[index], debugString, index, stackDepth, continuationsDepth));
          stackDepth += stackBalance;
          continuationsDepth += continuationsBalance;
        }
      }
      return instructionViewList.ToArray();
    }

    [DebuggerDisplay("{GetValue(),nq}", Name = "{GetName(),nq}", Type = "{GetDisplayType(), nq}")]
    internal struct InstructionView(
      Instruction instruction,
      string name,
      int index,
      int stackDepth,
      int continuationsDepth)
    {
      private readonly int _index = index;
      private readonly int _stackDepth = stackDepth;
      private readonly int _continuationsDepth = continuationsDepth;
      private readonly string _name = name;
      private readonly Instruction _instruction = instruction;

      internal string GetName()
      {
        int num = this._index;
        string str1 = num.ToString();
        string str2;
        if (this._continuationsDepth != 0)
        {
          num = this._continuationsDepth;
          str2 = $" C({num.ToString()})";
        }
        else
          str2 = "";
        string str3;
        if (this._stackDepth != 0)
        {
          num = this._stackDepth;
          str3 = $" S({num.ToString()})";
        }
        else
          str3 = "";
        return str1 + str2 + str3;
      }

      internal string GetValue() => this._name;

      internal string GetDisplayType()
      {
        int num = this._instruction.ContinuationsBalance;
        string str1 = num.ToString();
        num = this._instruction.StackBalance;
        string str2 = num.ToString();
        return $"{str1}/{str2}";
      }
    }
  }
}
