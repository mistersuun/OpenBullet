// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.LightCompiler
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

public sealed class LightCompiler
{
  internal const int DefaultCompilationThreshold = 32 /*0x20*/;
  private readonly int _compilationThreshold;
  private readonly LocalVariables _locals = new LocalVariables();
  private readonly List<ExceptionHandler> _handlers = new List<ExceptionHandler>();
  private readonly List<DebugInfo> _debugInfos = new List<DebugInfo>();
  private readonly HybridReferenceDictionary<LabelTarget, LabelInfo> _treeLabels = new HybridReferenceDictionary<LabelTarget, LabelInfo>();
  private LabelScopeInfo _labelBlock = new LabelScopeInfo((LabelScopeInfo) null, LabelScopeKind.Lambda);
  private readonly Stack<ParameterExpression> _exceptionForRethrowStack = new Stack<ParameterExpression>();
  private bool _forceCompile;
  private readonly LightCompiler _parent;
  private static LocalDefinition[] EmptyLocals = new LocalDefinition[0];

  internal LightCompiler(int compilationThreshold)
  {
    this.Instructions = new InstructionList();
    this._compilationThreshold = compilationThreshold < 0 ? 32 /*0x20*/ : compilationThreshold;
  }

  private LightCompiler(LightCompiler parent)
    : this(parent._compilationThreshold)
  {
    this._parent = parent;
  }

  public InstructionList Instructions { get; }

  public LocalVariables Locals => this._locals;

  internal static Expression Unbox(Expression strongBoxExpression)
  {
    return (Expression) Expression.Field(strongBoxExpression, typeof (StrongBox<object>).GetDeclaredField("Value"));
  }

  internal LightDelegateCreator CompileTop(LambdaExpression node)
  {
    foreach (ParameterExpression parameter in node.Parameters)
      this.Instructions.EmitInitializeParameter(this._locals.DefineLocal(parameter, 0).Index);
    this.Compile(node.Body);
    if (node.Body.Type != typeof (void) && node.ReturnType == typeof (void))
      this.Instructions.EmitPop();
    return new LightDelegateCreator(this.MakeInterpreter(node.Name), node);
  }

  internal LightDelegateCreator CompileTop(LightLambdaExpression node)
  {
    foreach (ParameterExpression parameter in (IEnumerable<ParameterExpression>) node.Parameters)
      this.Instructions.EmitInitializeParameter(this._locals.DefineLocal(parameter, 0).Index);
    this.Compile(node.Body);
    if (node.Body.Type != typeof (void) && node.ReturnType == typeof (void))
      this.Instructions.EmitPop();
    return new LightDelegateCreator(this.MakeInterpreter(node.Name), node);
  }

  private Microsoft.Scripting.Interpreter.Interpreter MakeInterpreter(string lambdaName)
  {
    if (this._forceCompile)
      return (Microsoft.Scripting.Interpreter.Interpreter) null;
    ExceptionHandler[] array1 = this._handlers.ToArray();
    DebugInfo[] array2 = this._debugInfos.ToArray();
    return new Microsoft.Scripting.Interpreter.Interpreter(lambdaName, this._locals, this.GetBranchMapping(), this.Instructions.ToArray(), array1, array2, this._compilationThreshold);
  }

  private void CompileConstantExpression(Expression expr)
  {
    ConstantExpression constantExpression = (ConstantExpression) expr;
    this.Instructions.EmitLoad(constantExpression.Value, constantExpression.Type);
  }

  private void CompileDefaultExpression(Expression expr)
  {
    this.CompileDefaultExpression(expr.Type);
  }

  private void CompileDefaultExpression(Type type)
  {
    if (type == typeof (void))
      return;
    if (type.IsValueType())
    {
      object primitiveDefaultValue = ScriptingRuntimeHelpers.GetPrimitiveDefaultValue(type);
      if (primitiveDefaultValue != null)
        this.Instructions.EmitLoad(primitiveDefaultValue);
      else
        this.Instructions.EmitDefaultValue(type);
    }
    else
      this.Instructions.EmitLoad((object) null);
  }

  private LocalVariable EnsureAvailableForClosure(ParameterExpression expr)
  {
    LocalVariable local;
    if (this._locals.TryGetLocalOrClosure(expr, out local))
    {
      if (!local.InClosure && !local.IsBoxed)
        this._locals.Box(expr, this.Instructions);
      return local;
    }
    if (this._parent == null)
      throw new InvalidOperationException("unbound variable: " + (object) expr);
    this._parent.EnsureAvailableForClosure(expr);
    return this._locals.AddClosureVariable(expr);
  }

  private void EnsureVariable(ParameterExpression variable)
  {
    if (this._locals.ContainsVariable(variable))
      return;
    this.EnsureAvailableForClosure(variable);
  }

  private LocalVariable ResolveLocal(ParameterExpression variable)
  {
    LocalVariable local;
    if (!this._locals.TryGetLocalOrClosure(variable, out local))
      local = this.EnsureAvailableForClosure(variable);
    return local;
  }

  public void CompileGetVariable(ParameterExpression variable)
  {
    LocalVariable localVariable = this.ResolveLocal(variable);
    if (localVariable.InClosure)
      this.Instructions.EmitLoadLocalFromClosure(localVariable.Index);
    else if (localVariable.IsBoxed)
      this.Instructions.EmitLoadLocalBoxed(localVariable.Index);
    else
      this.Instructions.EmitLoadLocal(localVariable.Index);
  }

  public void CompileGetBoxedVariable(ParameterExpression variable)
  {
    LocalVariable localVariable = this.ResolveLocal(variable);
    if (localVariable.InClosure)
      this.Instructions.EmitLoadLocalFromClosureBoxed(localVariable.Index);
    else
      this.Instructions.EmitLoadLocal(localVariable.Index);
  }

  public void CompileSetVariable(ParameterExpression variable, bool isVoid)
  {
    LocalVariable localVariable = this.ResolveLocal(variable);
    if (localVariable.InClosure)
    {
      if (isVoid)
        this.Instructions.EmitStoreLocalToClosure(localVariable.Index);
      else
        this.Instructions.EmitAssignLocalToClosure(localVariable.Index);
    }
    else if (localVariable.IsBoxed)
    {
      if (isVoid)
        this.Instructions.EmitStoreLocalBoxed(localVariable.Index);
      else
        this.Instructions.EmitAssignLocalBoxed(localVariable.Index);
    }
    else if (isVoid)
      this.Instructions.EmitStoreLocal(localVariable.Index);
    else
      this.Instructions.EmitAssignLocal(localVariable.Index);
  }

  public void CompileParameterExpression(Expression expr)
  {
    this.CompileGetVariable((ParameterExpression) expr);
  }

  private void CompileBlockExpression(Expression expr, bool asVoid)
  {
    BlockExpression node = (BlockExpression) expr;
    LocalDefinition[] locals = this.CompileBlockStart(node);
    this.Compile(node.Expressions[node.Expressions.Count - 1], asVoid);
    this.CompileBlockEnd(locals);
  }

  private LocalDefinition[] CompileBlockStart(BlockExpression node)
  {
    int count = this.Instructions.Count;
    ReadOnlyCollection<ParameterExpression> variables = node.Variables;
    LocalDefinition[] localDefinitionArray;
    if (variables.Count != 0)
    {
      localDefinitionArray = new LocalDefinition[variables.Count];
      int num = 0;
      foreach (ParameterExpression variable in variables)
      {
        LocalDefinition localDefinition = this._locals.DefineLocal(variable, count);
        localDefinitionArray[num++] = localDefinition;
        this.Instructions.EmitInitializeLocal(localDefinition.Index, variable.Type);
      }
    }
    else
      localDefinitionArray = LightCompiler.EmptyLocals;
    for (int index = 0; index < node.Expressions.Count - 1; ++index)
      this.CompileAsVoid(node.Expressions[index]);
    return localDefinitionArray;
  }

  private void CompileBlockEnd(LocalDefinition[] locals)
  {
    foreach (LocalDefinition local in locals)
      this._locals.UndefineLocal(local, this.Instructions.Count);
  }

  private void CompileIndexExpression(Expression expr)
  {
    IndexExpression indexExpression = (IndexExpression) expr;
    if (indexExpression.Object != null)
      this.Compile(indexExpression.Object);
    foreach (Expression expr1 in indexExpression.Arguments)
      this.Compile(expr1);
    if (indexExpression.Indexer != (PropertyInfo) null)
      this.EmitCall(indexExpression.Indexer.GetGetMethod(true));
    else if (indexExpression.Arguments.Count != 1)
      this.EmitCall(indexExpression.Object.Type.GetMethod("Get", BindingFlags.Instance | BindingFlags.Public));
    else
      this.Instructions.EmitGetArrayItem(indexExpression.Object.Type);
  }

  private void CompileIndexAssignment(BinaryExpression node, bool asVoid)
  {
    IndexExpression left = (IndexExpression) node.Left;
    if (!asVoid)
      throw new NotImplementedException();
    if (left.Object != null)
      this.Compile(left.Object);
    foreach (Expression expr in left.Arguments)
      this.Compile(expr);
    this.Compile(node.Right);
    if (left.Indexer != (PropertyInfo) null)
      this.EmitCall(left.Indexer.GetSetMethod(true));
    else if (left.Arguments.Count != 1)
      this.EmitCall(left.Object.Type.GetMethod("Set", BindingFlags.Instance | BindingFlags.Public));
    else
      this.Instructions.EmitSetArrayItem(left.Object.Type);
  }

  private void CompileMemberAssignment(BinaryExpression node, bool asVoid)
  {
    MemberExpression left = (MemberExpression) node.Left;
    PropertyInfo member1 = left.Member as PropertyInfo;
    if (member1 != (PropertyInfo) null)
    {
      MethodInfo setMethod = member1.GetSetMethod(true);
      this.Compile(left.Expression);
      this.Compile(node.Right);
      int count = this.Instructions.Count;
      if (!asVoid)
      {
        LocalDefinition definition = this._locals.DefineLocal(Expression.Parameter(node.Right.Type), count);
        this.Instructions.EmitAssignLocal(definition.Index);
        this.EmitCall(setMethod);
        this.Instructions.EmitLoadLocal(definition.Index);
        this._locals.UndefineLocal(definition, this.Instructions.Count);
      }
      else
        this.EmitCall(setMethod);
    }
    else
    {
      FieldInfo member2 = left.Member as FieldInfo;
      if (!(member2 != (FieldInfo) null))
        throw new NotImplementedException();
      if (left.Expression != null)
        this.Compile(left.Expression);
      this.Compile(node.Right);
      int count = this.Instructions.Count;
      if (!asVoid)
      {
        LocalDefinition definition = this._locals.DefineLocal(Expression.Parameter(node.Right.Type), count);
        this.Instructions.EmitAssignLocal(definition.Index);
        this.Instructions.EmitStoreField(member2);
        this.Instructions.EmitLoadLocal(definition.Index);
        this._locals.UndefineLocal(definition, this.Instructions.Count);
      }
      else
        this.Instructions.EmitStoreField(member2);
    }
  }

  private void CompileVariableAssignment(BinaryExpression node, bool asVoid)
  {
    this.Compile(node.Right);
    this.CompileSetVariable((ParameterExpression) node.Left, asVoid);
  }

  private void CompileAssignBinaryExpression(Expression expr, bool asVoid)
  {
    BinaryExpression node = (BinaryExpression) expr;
    switch (node.Left.NodeType)
    {
      case ExpressionType.MemberAccess:
        this.CompileMemberAssignment(node, asVoid);
        break;
      case ExpressionType.Parameter:
      case ExpressionType.Extension:
        this.CompileVariableAssignment(node, asVoid);
        break;
      case ExpressionType.Index:
        this.CompileIndexAssignment(node, asVoid);
        break;
      default:
        throw new InvalidOperationException("Invalid lvalue for assignment: " + (object) node.Left.NodeType);
    }
  }

  private void CompileBinaryExpression(Expression expr)
  {
    BinaryExpression binaryExpression = (BinaryExpression) expr;
    if (binaryExpression.Method != (MethodInfo) null)
    {
      this.Compile(binaryExpression.Left);
      this.Compile(binaryExpression.Right);
      this.EmitCall(binaryExpression.Method);
    }
    else
    {
      switch (binaryExpression.NodeType)
      {
        case ExpressionType.Add:
        case ExpressionType.AddChecked:
        case ExpressionType.Divide:
        case ExpressionType.Multiply:
        case ExpressionType.MultiplyChecked:
        case ExpressionType.Subtract:
        case ExpressionType.SubtractChecked:
          this.CompileArithmetic(binaryExpression.NodeType, binaryExpression.Left, binaryExpression.Right);
          break;
        case ExpressionType.ArrayIndex:
          this.Compile(binaryExpression.Left);
          this.Compile(binaryExpression.Right);
          this.Instructions.EmitGetArrayItem(binaryExpression.Left.Type);
          break;
        case ExpressionType.Equal:
          this.CompileEqual(binaryExpression.Left, binaryExpression.Right);
          break;
        case ExpressionType.GreaterThan:
        case ExpressionType.GreaterThanOrEqual:
        case ExpressionType.LessThan:
        case ExpressionType.LessThanOrEqual:
          this.CompileComparison(binaryExpression.NodeType, binaryExpression.Left, binaryExpression.Right);
          break;
        case ExpressionType.NotEqual:
          this.CompileNotEqual(binaryExpression.Left, binaryExpression.Right);
          break;
        default:
          throw new NotImplementedException(binaryExpression.NodeType.ToString());
      }
    }
  }

  private void CompileEqual(Expression left, Expression right)
  {
    this.Compile(left);
    this.Compile(right);
    this.Instructions.EmitEqual(left.Type);
  }

  private void CompileNotEqual(Expression left, Expression right)
  {
    this.Compile(left);
    this.Compile(right);
    this.Instructions.EmitNotEqual(left.Type);
  }

  private void CompileComparison(ExpressionType nodeType, Expression left, Expression right)
  {
    this.Compile(left);
    this.Compile(right);
    switch (nodeType)
    {
      case ExpressionType.GreaterThan:
        this.Instructions.EmitGreaterThan(left.Type);
        break;
      case ExpressionType.GreaterThanOrEqual:
        this.Instructions.EmitGreaterThanOrEqual(left.Type);
        break;
      case ExpressionType.LessThan:
        this.Instructions.EmitLessThan(left.Type);
        break;
      case ExpressionType.LessThanOrEqual:
        this.Instructions.EmitLessThanOrEqual(left.Type);
        break;
      default:
        throw Assert.Unreachable;
    }
  }

  private void CompileArithmetic(ExpressionType nodeType, Expression left, Expression right)
  {
    this.Compile(left);
    this.Compile(right);
    switch (nodeType)
    {
      case ExpressionType.Add:
        this.Instructions.EmitAdd(left.Type, false);
        break;
      case ExpressionType.AddChecked:
        this.Instructions.EmitAdd(left.Type, true);
        break;
      case ExpressionType.Divide:
        this.Instructions.EmitDiv(left.Type);
        break;
      case ExpressionType.Multiply:
        this.Instructions.EmitMul(left.Type, false);
        break;
      case ExpressionType.MultiplyChecked:
        this.Instructions.EmitMul(left.Type, true);
        break;
      case ExpressionType.Subtract:
        this.Instructions.EmitSub(left.Type, false);
        break;
      case ExpressionType.SubtractChecked:
        this.Instructions.EmitSub(left.Type, true);
        break;
      default:
        throw Assert.Unreachable;
    }
  }

  private void CompileConvertUnaryExpression(Expression expr)
  {
    UnaryExpression unaryExpression = (UnaryExpression) expr;
    if (unaryExpression.Method != (MethodInfo) null)
    {
      this.Compile(unaryExpression.Operand);
      if (!(unaryExpression.Method != ScriptingRuntimeHelpers.Int32ToObjectMethod))
        return;
      this.EmitCall(unaryExpression.Method);
    }
    else if (unaryExpression.Type == typeof (void))
    {
      this.CompileAsVoid(unaryExpression.Operand);
    }
    else
    {
      this.Compile(unaryExpression.Operand);
      this.CompileConvertToType(unaryExpression.Operand.Type, unaryExpression.Type, unaryExpression.NodeType == ExpressionType.ConvertChecked);
    }
  }

  private void CompileConvertToType(Type typeFrom, Type typeTo, bool isChecked)
  {
    if (TypeUtils.AreEquivalent(typeTo, typeFrom))
      return;
    TypeCode typeCode1 = typeFrom.GetTypeCode();
    TypeCode typeCode2 = typeTo.GetTypeCode();
    if (!TypeUtils.IsNumeric(typeCode1) || !TypeUtils.IsNumeric(typeCode2))
      return;
    if (isChecked)
      this.Instructions.EmitNumericConvertChecked(typeCode1, typeCode2);
    else
      this.Instructions.EmitNumericConvertUnchecked(typeCode1, typeCode2);
  }

  private void CompileNotExpression(UnaryExpression node)
  {
    if (!(node.Operand.Type == typeof (bool)))
      throw new NotImplementedException();
    this.Compile(node.Operand);
    this.Instructions.EmitNot();
  }

  private void CompileUnaryExpression(Expression expr)
  {
    UnaryExpression node = (UnaryExpression) expr;
    if (node.Method != (MethodInfo) null)
    {
      this.Compile(node.Operand);
      this.EmitCall(node.Method);
    }
    else
    {
      switch (node.NodeType)
      {
        case ExpressionType.Not:
          this.CompileNotExpression(node);
          break;
        case ExpressionType.TypeAs:
          this.CompileTypeAsExpression(node);
          break;
        default:
          throw new NotImplementedException(node.NodeType.ToString());
      }
    }
  }

  private void CompileAndAlsoBinaryExpression(Expression expr)
  {
    this.CompileLogicalBinaryExpression(expr, true);
  }

  private void CompileOrElseBinaryExpression(Expression expr)
  {
    this.CompileLogicalBinaryExpression(expr, false);
  }

  private void CompileLogicalBinaryExpression(Expression expr, bool andAlso)
  {
    BinaryExpression binaryExpression = (BinaryExpression) expr;
    if (binaryExpression.Method != (MethodInfo) null)
      throw new NotImplementedException();
    if (!(binaryExpression.Left.Type == typeof (bool)))
      throw new NotImplementedException();
    BranchLabel branchLabel = this.Instructions.MakeLabel();
    BranchLabel label = this.Instructions.MakeLabel();
    this.Compile(binaryExpression.Left);
    if (andAlso)
      this.Instructions.EmitBranchFalse(branchLabel);
    else
      this.Instructions.EmitBranchTrue(branchLabel);
    this.Compile(binaryExpression.Right);
    this.Instructions.EmitBranch(label, false, true);
    this.Instructions.MarkLabel(branchLabel);
    this.Instructions.EmitLoad(!andAlso);
    this.Instructions.MarkLabel(label);
  }

  private void CompileConditionalExpression(Expression expr, bool asVoid)
  {
    ConditionalExpression conditionalExpression = (ConditionalExpression) expr;
    this.Compile(conditionalExpression.Test);
    if (conditionalExpression.IfTrue == Microsoft.Scripting.Ast.Utils.Empty())
    {
      BranchLabel branchLabel = this.Instructions.MakeLabel();
      this.Instructions.EmitBranchTrue(branchLabel);
      this.Compile(conditionalExpression.IfFalse, asVoid);
      this.Instructions.MarkLabel(branchLabel);
    }
    else
    {
      BranchLabel branchLabel = this.Instructions.MakeLabel();
      this.Instructions.EmitBranchFalse(branchLabel);
      this.Compile(conditionalExpression.IfTrue, asVoid);
      if (conditionalExpression.IfFalse != Microsoft.Scripting.Ast.Utils.Empty())
      {
        BranchLabel label = this.Instructions.MakeLabel();
        this.Instructions.EmitBranch(label, false, !asVoid);
        this.Instructions.MarkLabel(branchLabel);
        this.Compile(conditionalExpression.IfFalse, asVoid);
        this.Instructions.MarkLabel(label);
      }
      else
        this.Instructions.MarkLabel(branchLabel);
    }
  }

  private void CompileLoopExpression(Expression expr)
  {
    LoopExpression loop = (LoopExpression) expr;
    EnterLoopInstruction enterLoopInstruction = new EnterLoopInstruction(loop, this._locals, this._compilationThreshold, this.Instructions.Count);
    this.PushLabelBlock(LabelScopeKind.Statement);
    LabelInfo labelInfo1 = this.DefineLabel(loop.BreakLabel);
    LabelInfo labelInfo2 = this.DefineLabel(loop.ContinueLabel);
    this.Instructions.MarkLabel(labelInfo2.GetLabel(this));
    this.Instructions.Emit((Instruction) enterLoopInstruction);
    this.CompileAsVoid(loop.Body);
    this.Instructions.EmitBranch(labelInfo2.GetLabel(this), expr.Type != typeof (void), false);
    this.Instructions.MarkLabel(labelInfo1.GetLabel(this));
    this.PopLabelBlock(LabelScopeKind.Statement);
    enterLoopInstruction.FinishLoop(this.Instructions.Count);
  }

  private void CompileSwitchExpression(Expression expr)
  {
    SwitchExpression switchExpression = (SwitchExpression) expr;
    if (switchExpression.SwitchValue.Type != typeof (int) || switchExpression.Comparison != (MethodInfo) null)
      throw new NotImplementedException();
    if (!switchExpression.Cases.All<SwitchCase>((Func<SwitchCase, bool>) (c => c.TestValues.All<Expression>((Func<Expression, bool>) (t => t is ConstantExpression)))))
      throw new NotImplementedException();
    LabelInfo labelInfo = this.DefineLabel((LabelTarget) null);
    bool hasValue = switchExpression.Type != typeof (void);
    this.Compile(switchExpression.SwitchValue);
    Dictionary<int, int> cases = new Dictionary<int, int>();
    int count = this.Instructions.Count;
    this.Instructions.EmitSwitch(cases);
    if (switchExpression.DefaultBody != null)
      this.Compile(switchExpression.DefaultBody);
    this.Instructions.EmitBranch(labelInfo.GetLabel(this), false, hasValue);
    for (int index = 0; index < switchExpression.Cases.Count; ++index)
    {
      SwitchCase switchCase = switchExpression.Cases[index];
      int num = this.Instructions.Count - count;
      foreach (ConstantExpression testValue in switchCase.TestValues)
        cases[(int) testValue.Value] = num;
      this.Compile(switchCase.Body);
      if (index < switchExpression.Cases.Count - 1)
        this.Instructions.EmitBranch(labelInfo.GetLabel(this), false, hasValue);
    }
    this.Instructions.MarkLabel(labelInfo.GetLabel(this));
  }

  private void CompileLabelExpression(Expression expr)
  {
    LabelExpression labelExpression = (LabelExpression) expr;
    LabelInfo info = (LabelInfo) null;
    if (this._labelBlock.Kind == LabelScopeKind.Block)
    {
      this._labelBlock.TryGetLabelInfo(labelExpression.Target, out info);
      if (info == null && this._labelBlock.Parent.Kind == LabelScopeKind.Switch)
        this._labelBlock.Parent.TryGetLabelInfo(labelExpression.Target, out info);
    }
    if (info == null)
      info = this.DefineLabel(labelExpression.Target);
    if (labelExpression.DefaultValue != null)
    {
      if (labelExpression.Target.Type == typeof (void))
        this.CompileAsVoid(labelExpression.DefaultValue);
      else
        this.Compile(labelExpression.DefaultValue);
    }
    this.Instructions.MarkLabel(info.GetLabel(this));
  }

  private void CompileGotoExpression(Expression expr)
  {
    GotoExpression gotoExpression = (GotoExpression) expr;
    LabelInfo labelInfo = this.ReferenceLabel(gotoExpression.Target);
    if (gotoExpression.Value != null)
      this.Compile(gotoExpression.Value);
    this.Instructions.EmitGoto(labelInfo.GetLabel(this), gotoExpression.Type != typeof (void), gotoExpression.Value != null && gotoExpression.Value.Type != typeof (void));
  }

  public BranchLabel GetBranchLabel(LabelTarget target)
  {
    return this.ReferenceLabel(target).GetLabel(this);
  }

  public void PushLabelBlock(LabelScopeKind type)
  {
    this._labelBlock = new LabelScopeInfo(this._labelBlock, type);
  }

  public void PopLabelBlock(LabelScopeKind kind) => this._labelBlock = this._labelBlock.Parent;

  private LabelInfo EnsureLabel(LabelTarget node)
  {
    LabelInfo labelInfo;
    if (!this._treeLabels.TryGetValue(node, out labelInfo))
      this._treeLabels[node] = labelInfo = new LabelInfo(node);
    return labelInfo;
  }

  private LabelInfo ReferenceLabel(LabelTarget node)
  {
    LabelInfo labelInfo = this.EnsureLabel(node);
    labelInfo.Reference(this._labelBlock);
    return labelInfo;
  }

  internal LabelInfo DefineLabel(LabelTarget node)
  {
    if (node == null)
      return new LabelInfo((LabelTarget) null);
    LabelInfo labelInfo = this.EnsureLabel(node);
    labelInfo.Define(this._labelBlock);
    return labelInfo;
  }

  private bool TryPushLabelBlock(Expression node)
  {
    switch (node.NodeType)
    {
      case ExpressionType.Conditional:
      case ExpressionType.Goto:
      case ExpressionType.Loop:
        this.PushLabelBlock(LabelScopeKind.Statement);
        return true;
      case ExpressionType.Convert:
        if (!(node.Type != typeof (void)))
        {
          this.PushLabelBlock(LabelScopeKind.Statement);
          return true;
        }
        break;
      case ExpressionType.Block:
        this.PushLabelBlock(LabelScopeKind.Block);
        if (this._labelBlock.Parent.Kind != LabelScopeKind.Switch)
          this.DefineBlockLabels(node);
        return true;
      case ExpressionType.Label:
        if (this._labelBlock.Kind == LabelScopeKind.Block)
        {
          LabelTarget target = ((LabelExpression) node).Target;
          if (this._labelBlock.ContainsTarget(target) || this._labelBlock.Parent.Kind == LabelScopeKind.Switch && this._labelBlock.Parent.ContainsTarget(target))
            return false;
        }
        this.PushLabelBlock(LabelScopeKind.Statement);
        return true;
      case ExpressionType.Switch:
        this.PushLabelBlock(LabelScopeKind.Switch);
        SwitchExpression switchExpression = (SwitchExpression) node;
        foreach (SwitchCase switchCase in switchExpression.Cases)
          this.DefineBlockLabels(switchCase.Body);
        this.DefineBlockLabels(switchExpression.DefaultBody);
        return true;
    }
    if (this._labelBlock.Kind == LabelScopeKind.Expression)
      return false;
    this.PushLabelBlock(LabelScopeKind.Expression);
    return true;
  }

  private void DefineBlockLabels(Expression node)
  {
    if (!(node is BlockExpression blockExpression))
      return;
    int index = 0;
    for (int count = blockExpression.Expressions.Count; index < count; ++index)
    {
      if (blockExpression.Expressions[index] is LabelExpression expression)
        this.DefineLabel(expression.Target);
    }
  }

  private HybridReferenceDictionary<LabelTarget, BranchLabel> GetBranchMapping()
  {
    HybridReferenceDictionary<LabelTarget, BranchLabel> branchMapping = new HybridReferenceDictionary<LabelTarget, BranchLabel>(this._treeLabels.Count);
    foreach (KeyValuePair<LabelTarget, LabelInfo> treeLabel in this._treeLabels)
      branchMapping[treeLabel.Key] = treeLabel.Value.GetLabel(this);
    return branchMapping;
  }

  private void CompileThrowUnaryExpression(Expression expr, bool asVoid)
  {
    UnaryExpression unaryExpression = (UnaryExpression) expr;
    if (unaryExpression.Operand == null)
    {
      this.CompileParameterExpression((Expression) this._exceptionForRethrowStack.Peek());
      if (asVoid)
        this.Instructions.EmitRethrowVoid();
      else
        this.Instructions.EmitRethrow();
    }
    else
    {
      this.Compile(unaryExpression.Operand);
      if (asVoid)
        this.Instructions.EmitThrowVoid();
      else
        this.Instructions.EmitThrow();
    }
  }

  private bool EndsWithRethrow(Expression expr)
  {
    if (expr.NodeType == ExpressionType.Throw)
      return ((UnaryExpression) expr).Operand == null;
    return expr is BlockExpression blockExpression && this.EndsWithRethrow(blockExpression.Expressions[blockExpression.Expressions.Count - 1]);
  }

  private void CompileAsVoidRemoveRethrow(Expression expr)
  {
    int currentStackDepth = this.Instructions.CurrentStackDepth;
    if (expr.NodeType == ExpressionType.Throw)
      return;
    BlockExpression node = (BlockExpression) expr;
    LocalDefinition[] locals = this.CompileBlockStart(node);
    this.CompileAsVoidRemoveRethrow(node.Expressions[node.Expressions.Count - 1]);
    this.CompileBlockEnd(locals);
  }

  private void CompileTryExpression(Expression expr)
  {
    TryExpression tryExpression = (TryExpression) expr;
    BranchLabel label = this.Instructions.MakeLabel();
    BranchLabel branchLabel1 = this.Instructions.MakeLabel();
    int count1 = this.Instructions.Count;
    BranchLabel branchLabel2 = (BranchLabel) null;
    if (tryExpression.Finally != null)
    {
      branchLabel2 = this.Instructions.MakeLabel();
      this.Instructions.EmitEnterTryFinally(branchLabel2);
    }
    this.PushLabelBlock(LabelScopeKind.Try);
    this.Compile(tryExpression.Body);
    bool flag = tryExpression.Body.Type != typeof (void);
    int count2 = this.Instructions.Count;
    this.Instructions.MarkLabel(branchLabel1);
    this.Instructions.EmitGoto(label, flag, flag);
    if (tryExpression.Handlers.Count > 0)
    {
      if (tryExpression.Finally == null && tryExpression.Handlers.Count == 1)
      {
        CatchBlock handler = tryExpression.Handlers[0];
        if (handler.Filter == null && handler.Test == typeof (Exception) && handler.Variable == null && this.EndsWithRethrow(handler.Body))
        {
          if (flag)
            this.Instructions.EmitEnterExceptionHandlerNonVoid();
          else
            this.Instructions.EmitEnterExceptionHandlerVoid();
          int labelIndex = this.Instructions.MarkRuntimeLabel();
          int count3 = this.Instructions.Count;
          this.CompileAsVoidRemoveRethrow(handler.Body);
          this.Instructions.EmitLeaveFault(flag);
          this.Instructions.MarkLabel(label);
          this._handlers.Add(new ExceptionHandler(count1, count2, labelIndex, count3, (Type) null));
          this.PopLabelBlock(LabelScopeKind.Try);
          return;
        }
      }
      foreach (CatchBlock handler in tryExpression.Handlers)
      {
        this.PushLabelBlock(LabelScopeKind.Catch);
        if (handler.Filter != null)
          throw new NotImplementedException();
        ParameterExpression variable = handler.Variable ?? Expression.Parameter(handler.Test);
        LocalDefinition definition = this._locals.DefineLocal(variable, this.Instructions.Count);
        this._exceptionForRethrowStack.Push(variable);
        if (flag)
          this.Instructions.EmitEnterExceptionHandlerNonVoid();
        else
          this.Instructions.EmitEnterExceptionHandlerVoid();
        int labelIndex = this.Instructions.MarkRuntimeLabel();
        int count4 = this.Instructions.Count;
        this.CompileSetVariable(variable, true);
        this.Compile(handler.Body);
        this._exceptionForRethrowStack.Pop();
        this.Instructions.EmitLeaveExceptionHandler(flag, branchLabel1);
        this._handlers.Add(new ExceptionHandler(count1, count2, labelIndex, count4, handler.Test));
        this.PopLabelBlock(LabelScopeKind.Catch);
        this._locals.UndefineLocal(definition, this.Instructions.Count);
      }
      if (tryExpression.Fault != null)
        throw new NotImplementedException();
    }
    if (tryExpression.Finally != null)
    {
      this.PushLabelBlock(LabelScopeKind.Finally);
      this.Instructions.MarkLabel(branchLabel2);
      this.Instructions.EmitEnterFinally();
      this.CompileAsVoid(tryExpression.Finally);
      this.Instructions.EmitLeaveFinally();
      this.PopLabelBlock(LabelScopeKind.Finally);
    }
    this.Instructions.MarkLabel(label);
    this.PopLabelBlock(LabelScopeKind.Try);
  }

  private void CompileDynamicExpression(Expression expr)
  {
    DynamicExpression dynamicExpression = (DynamicExpression) expr;
    foreach (Expression expr1 in dynamicExpression.Arguments)
      this.Compile(expr1);
    this.Instructions.EmitDynamic(dynamicExpression.DelegateType, dynamicExpression.Binder);
  }

  private void CompileMethodCallExpression(Expression expr)
  {
    MethodCallExpression methodCallExpression = (MethodCallExpression) expr;
    ParameterInfo[] parameters = methodCallExpression.Method.GetParameters();
    if (!CollectionUtils.TrueForAll<ParameterInfo>((IEnumerable<ParameterInfo>) parameters, (Predicate<ParameterInfo>) (p => !p.ParameterType.IsByRef)) || !methodCallExpression.Method.IsStatic && methodCallExpression.Method.DeclaringType.IsValueType() && !methodCallExpression.Method.DeclaringType.IsPrimitive())
      this._forceCompile = true;
    if (!methodCallExpression.Method.IsStatic)
      this.Compile(methodCallExpression.Object);
    foreach (Expression expr1 in methodCallExpression.Arguments)
      this.Compile(expr1);
    this.EmitCall(methodCallExpression.Method, parameters);
  }

  public void EmitCall(MethodInfo method) => this.EmitCall(method, method.GetParameters());

  public void EmitCall(MethodInfo method, ParameterInfo[] parameters)
  {
    Instruction instruction;
    try
    {
      instruction = (Instruction) CallInstruction.Create(method, parameters);
    }
    catch (SecurityException ex)
    {
      this._forceCompile = true;
      this.Instructions.Emit((Instruction) new PopNInstruction((method.IsStatic ? 0 : 1) + parameters.Length));
      if (!(method.ReturnType != typeof (void)))
        return;
      this.Instructions.EmitLoad((object) null);
      return;
    }
    this.Instructions.Emit(instruction);
  }

  private void CompileNewExpression(Expression expr)
  {
    NewExpression newExpression = (NewExpression) expr;
    if (newExpression.Constructor != (ConstructorInfo) null && (!CollectionUtils.TrueForAll<ParameterInfo>((IEnumerable<ParameterInfo>) newExpression.Constructor.GetParameters(), (Predicate<ParameterInfo>) (p => !p.ParameterType.IsByRef)) || newExpression.Constructor.DeclaringType == typeof (DynamicMethod)))
      this._forceCompile = true;
    if (newExpression.Constructor != (ConstructorInfo) null)
    {
      foreach (Expression expr1 in newExpression.Arguments)
        this.Compile(expr1);
      this.Instructions.EmitNew(newExpression.Constructor);
    }
    else
      this.Instructions.EmitDefaultValue(newExpression.Type);
  }

  private void CompileMemberExpression(Expression expr)
  {
    MemberExpression memberExpression = (MemberExpression) expr;
    MemberInfo member = memberExpression.Member;
    FieldInfo field = member as FieldInfo;
    if (field != (FieldInfo) null)
    {
      if (field.IsLiteral)
        this.Instructions.EmitLoad(field.GetRawConstantValue(), field.FieldType);
      else if (field.IsStatic)
      {
        if (field.IsInitOnly)
          this.Instructions.EmitLoad(field.GetValue((object) null), field.FieldType);
        else
          this.Instructions.EmitLoadField(field);
      }
      else
      {
        this.Compile(memberExpression.Expression);
        this.Instructions.EmitLoadField(field);
      }
    }
    else
    {
      PropertyInfo propertyInfo = member as PropertyInfo;
      MethodInfo method = propertyInfo != (PropertyInfo) null ? propertyInfo.GetGetMethod(true) : throw new NotImplementedException();
      if (memberExpression.Expression != null)
        this.Compile(memberExpression.Expression);
      this.EmitCall(method);
    }
  }

  private void CompileNewArrayExpression(Expression expr)
  {
    NewArrayExpression newArrayExpression = (NewArrayExpression) expr;
    foreach (Expression expression in newArrayExpression.Expressions)
      this.Compile(expression);
    Type elementType = newArrayExpression.Type.GetElementType();
    int count = newArrayExpression.Expressions.Count;
    if (newArrayExpression.NodeType == ExpressionType.NewArrayInit)
    {
      this.Instructions.EmitNewArrayInit(elementType, count);
    }
    else
    {
      if (newArrayExpression.NodeType != ExpressionType.NewArrayBounds)
        throw new NotImplementedException();
      if (count == 1)
        this.Instructions.EmitNewArray(elementType);
      else
        this.Instructions.EmitNewArrayBounds(elementType, count);
    }
  }

  private void CompileExtensionExpression(Expression expr)
  {
    if (expr is IInstructionProvider instructionProvider)
    {
      instructionProvider.AddInstructions(this);
    }
    else
    {
      if (!expr.CanReduce)
        throw new NotImplementedException();
      this.Compile(expr.Reduce());
    }
  }

  private void CompileDebugInfoExpression(Expression expr)
  {
    DebugInfoExpression debugInfoExpression = (DebugInfoExpression) expr;
    int count = this.Instructions.Count;
    this._debugInfos.Add(new DebugInfo()
    {
      Index = count,
      FileName = debugInfoExpression.Document.FileName,
      StartLine = debugInfoExpression.StartLine,
      EndLine = debugInfoExpression.EndLine,
      IsClear = debugInfoExpression.IsClear
    });
  }

  private void CompileRuntimeVariablesExpression(Expression expr)
  {
    RuntimeVariablesExpression variablesExpression = (RuntimeVariablesExpression) expr;
    foreach (ParameterExpression variable in variablesExpression.Variables)
    {
      this.EnsureAvailableForClosure(variable);
      this.CompileGetBoxedVariable(variable);
    }
    this.Instructions.EmitNewRuntimeVariables(variablesExpression.Variables.Count);
  }

  private void CompileLambdaExpression(Expression expr)
  {
    LambdaExpression node = (LambdaExpression) expr;
    LightCompiler lightCompiler = new LightCompiler(this);
    LightDelegateCreator creator = lightCompiler.CompileTop(node);
    if (lightCompiler._locals.ClosureVariables != null)
    {
      foreach (ParameterExpression key in lightCompiler._locals.ClosureVariables.Keys)
        this.CompileGetBoxedVariable(key);
    }
    this.Instructions.EmitCreateDelegate(creator);
  }

  private void CompileCoalesceBinaryExpression(Expression expr)
  {
    BinaryExpression binaryExpression = (BinaryExpression) expr;
    if (binaryExpression.Left.Type.IsNullableType())
      throw new NotImplementedException();
    if (binaryExpression.Conversion != null)
      throw new NotImplementedException();
    BranchLabel branchLabel = this.Instructions.MakeLabel();
    this.Compile(binaryExpression.Left);
    this.Instructions.EmitCoalescingBranch(branchLabel);
    this.Instructions.EmitPop();
    this.Compile(binaryExpression.Right);
    this.Instructions.MarkLabel(branchLabel);
  }

  private void CompileInvocationExpression(Expression expr)
  {
    InvocationExpression invocationExpression = (InvocationExpression) expr;
    if (typeof (LambdaExpression).IsAssignableFrom(invocationExpression.Expression.Type))
      throw new NotImplementedException();
    this.CompileMethodCallExpression((Expression) Expression.Call(invocationExpression.Expression, invocationExpression.Expression.Type.GetMethod("Invoke"), (IEnumerable<Expression>) invocationExpression.Arguments));
  }

  private void CompileListInitExpression(Expression expr) => throw new NotImplementedException();

  private void CompileMemberInitExpression(Expression expr) => throw new NotImplementedException();

  private void CompileQuoteUnaryExpression(Expression expr) => throw new NotImplementedException();

  private void CompileUnboxUnaryExpression(Expression expr)
  {
    this.Compile(((UnaryExpression) expr).Operand);
  }

  private void CompileTypeEqualExpression(Expression expr)
  {
    TypeBinaryExpression binaryExpression = (TypeBinaryExpression) expr;
    this.Compile(binaryExpression.Expression);
    this.Instructions.EmitLoad((object) binaryExpression.TypeOperand);
    this.Instructions.EmitTypeEquals();
  }

  private void CompileTypeAsExpression(UnaryExpression node)
  {
    this.Compile(node.Operand);
    this.Instructions.EmitTypeAs(node.Type);
  }

  private void CompileTypeIsExpression(Expression expr)
  {
    TypeBinaryExpression binaryExpression = (TypeBinaryExpression) expr;
    this.Compile(binaryExpression.Expression);
    if (binaryExpression.TypeOperand.IsSealed())
    {
      this.Instructions.EmitLoad((object) binaryExpression.TypeOperand);
      this.Instructions.EmitTypeEquals();
    }
    else
      this.Instructions.EmitTypeIs(binaryExpression.TypeOperand);
  }

  private void CompileReducibleExpression(Expression expr) => throw new NotImplementedException();

  internal void Compile(Expression expr, bool asVoid)
  {
    if (asVoid)
      this.CompileAsVoid(expr);
    else
      this.Compile(expr);
  }

  internal void CompileAsVoid(Expression expr)
  {
    bool flag = this.TryPushLabelBlock(expr);
    int currentStackDepth = this.Instructions.CurrentStackDepth;
    switch (expr.NodeType)
    {
      case ExpressionType.Constant:
      case ExpressionType.Parameter:
      case ExpressionType.Default:
        if (!flag)
          break;
        this.PopLabelBlock(this._labelBlock.Kind);
        break;
      case ExpressionType.Assign:
        this.CompileAssignBinaryExpression(expr, true);
        goto case ExpressionType.Constant;
      case ExpressionType.Block:
        this.CompileBlockExpression(expr, true);
        goto case ExpressionType.Constant;
      case ExpressionType.Throw:
        this.CompileThrowUnaryExpression(expr, true);
        goto case ExpressionType.Constant;
      default:
        this.CompileNoLabelPush(expr);
        if (expr.Type != typeof (void))
        {
          this.Instructions.EmitPop();
          goto case ExpressionType.Constant;
        }
        goto case ExpressionType.Constant;
    }
  }

  private void CompileNoLabelPush(Expression expr)
  {
    int currentStackDepth = this.Instructions.CurrentStackDepth;
    switch (expr.NodeType)
    {
      case ExpressionType.Add:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.AddChecked:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.And:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.AndAlso:
        this.CompileAndAlsoBinaryExpression(expr);
        break;
      case ExpressionType.ArrayLength:
        this.CompileUnaryExpression(expr);
        break;
      case ExpressionType.ArrayIndex:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.Call:
        this.CompileMethodCallExpression(expr);
        break;
      case ExpressionType.Coalesce:
        this.CompileCoalesceBinaryExpression(expr);
        break;
      case ExpressionType.Conditional:
        this.CompileConditionalExpression(expr, expr.Type == typeof (void));
        break;
      case ExpressionType.Constant:
        this.CompileConstantExpression(expr);
        break;
      case ExpressionType.Convert:
        this.CompileConvertUnaryExpression(expr);
        break;
      case ExpressionType.ConvertChecked:
        this.CompileConvertUnaryExpression(expr);
        break;
      case ExpressionType.Divide:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.Equal:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.ExclusiveOr:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.GreaterThan:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.GreaterThanOrEqual:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.Invoke:
        this.CompileInvocationExpression(expr);
        break;
      case ExpressionType.Lambda:
        this.CompileLambdaExpression(expr);
        break;
      case ExpressionType.LeftShift:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.LessThan:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.LessThanOrEqual:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.ListInit:
        this.CompileListInitExpression(expr);
        break;
      case ExpressionType.MemberAccess:
        this.CompileMemberExpression(expr);
        break;
      case ExpressionType.MemberInit:
        this.CompileMemberInitExpression(expr);
        break;
      case ExpressionType.Modulo:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.Multiply:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.MultiplyChecked:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.Negate:
        this.CompileUnaryExpression(expr);
        break;
      case ExpressionType.UnaryPlus:
        this.CompileUnaryExpression(expr);
        break;
      case ExpressionType.NegateChecked:
        this.CompileUnaryExpression(expr);
        break;
      case ExpressionType.New:
        this.CompileNewExpression(expr);
        break;
      case ExpressionType.NewArrayInit:
        this.CompileNewArrayExpression(expr);
        break;
      case ExpressionType.NewArrayBounds:
        this.CompileNewArrayExpression(expr);
        break;
      case ExpressionType.Not:
        this.CompileUnaryExpression(expr);
        break;
      case ExpressionType.NotEqual:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.Or:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.OrElse:
        this.CompileOrElseBinaryExpression(expr);
        break;
      case ExpressionType.Parameter:
        this.CompileParameterExpression(expr);
        break;
      case ExpressionType.Power:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.Quote:
        this.CompileQuoteUnaryExpression(expr);
        break;
      case ExpressionType.RightShift:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.Subtract:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.SubtractChecked:
        this.CompileBinaryExpression(expr);
        break;
      case ExpressionType.TypeAs:
        this.CompileUnaryExpression(expr);
        break;
      case ExpressionType.TypeIs:
        this.CompileTypeIsExpression(expr);
        break;
      case ExpressionType.Assign:
        this.CompileAssignBinaryExpression(expr, expr.Type == typeof (void));
        break;
      case ExpressionType.Block:
        this.CompileBlockExpression(expr, expr.Type == typeof (void));
        break;
      case ExpressionType.DebugInfo:
        this.CompileDebugInfoExpression(expr);
        break;
      case ExpressionType.Decrement:
        this.CompileUnaryExpression(expr);
        break;
      case ExpressionType.Dynamic:
        this.CompileDynamicExpression(expr);
        break;
      case ExpressionType.Default:
        this.CompileDefaultExpression(expr);
        break;
      case ExpressionType.Extension:
        this.CompileExtensionExpression(expr);
        break;
      case ExpressionType.Goto:
        this.CompileGotoExpression(expr);
        break;
      case ExpressionType.Increment:
        this.CompileUnaryExpression(expr);
        break;
      case ExpressionType.Index:
        this.CompileIndexExpression(expr);
        break;
      case ExpressionType.Label:
        this.CompileLabelExpression(expr);
        break;
      case ExpressionType.RuntimeVariables:
        this.CompileRuntimeVariablesExpression(expr);
        break;
      case ExpressionType.Loop:
        this.CompileLoopExpression(expr);
        break;
      case ExpressionType.Switch:
        this.CompileSwitchExpression(expr);
        break;
      case ExpressionType.Throw:
        this.CompileThrowUnaryExpression(expr, expr.Type == typeof (void));
        break;
      case ExpressionType.Try:
        this.CompileTryExpression(expr);
        break;
      case ExpressionType.Unbox:
        this.CompileUnboxUnaryExpression(expr);
        break;
      case ExpressionType.AddAssign:
      case ExpressionType.AndAssign:
      case ExpressionType.DivideAssign:
      case ExpressionType.ExclusiveOrAssign:
      case ExpressionType.LeftShiftAssign:
      case ExpressionType.ModuloAssign:
      case ExpressionType.MultiplyAssign:
      case ExpressionType.OrAssign:
      case ExpressionType.PowerAssign:
      case ExpressionType.RightShiftAssign:
      case ExpressionType.SubtractAssign:
      case ExpressionType.AddAssignChecked:
      case ExpressionType.MultiplyAssignChecked:
      case ExpressionType.SubtractAssignChecked:
      case ExpressionType.PreIncrementAssign:
      case ExpressionType.PreDecrementAssign:
      case ExpressionType.PostIncrementAssign:
      case ExpressionType.PostDecrementAssign:
        this.CompileReducibleExpression(expr);
        break;
      case ExpressionType.TypeEqual:
        this.CompileTypeEqualExpression(expr);
        break;
      case ExpressionType.OnesComplement:
        this.CompileUnaryExpression(expr);
        break;
      case ExpressionType.IsTrue:
        this.CompileUnaryExpression(expr);
        break;
      case ExpressionType.IsFalse:
        this.CompileUnaryExpression(expr);
        break;
      default:
        throw Assert.Unreachable;
    }
  }

  public void Compile(Expression expr)
  {
    int num = this.TryPushLabelBlock(expr) ? 1 : 0;
    this.CompileNoLabelPush(expr);
    if (num == 0)
      return;
    this.PopLabelBlock(this._labelBlock.Kind);
  }
}
