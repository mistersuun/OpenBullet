// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Generation.ToDiskRewriter
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Generation;

internal sealed class ToDiskRewriter : DynamicExpressionVisitor
{
  private static int _uniqueNameId;
  private List<Expression> _constants;
  private Dictionary<object, Expression> _constantCache;
  private ParameterExpression _constantPool;
  private Dictionary<Type, Type> _delegateTypes;
  private int _depth;
  private readonly TypeGen _typeGen;

  internal ToDiskRewriter(TypeGen typeGen) => this._typeGen = typeGen;

  public LambdaExpression RewriteLambda(LambdaExpression lambda)
  {
    return (LambdaExpression) ((ExpressionVisitor) this).Visit((Expression) lambda);
  }

  protected virtual Expression VisitLambda<T>(Expression<T> node)
  {
    ++this._depth;
    try
    {
      // ISSUE: explicit non-virtual call
      node = (Expression<T>) __nonvirtual (((ExpressionVisitor) this).VisitLambda<T>(node));
      if (this._depth != 1)
        return (Expression) node;
      Expression body = node.Body;
      if (this._constants != null)
      {
        for (int index = 0; index < this._constants.Count; ++index)
          this._constants[index] = ((ExpressionVisitor) this).Visit(this._constants[index]);
        ReadOnlyCollectionBuilder<Expression> collectionBuilder = new ReadOnlyCollectionBuilder<Expression>(this._constants.Count + 2);
        collectionBuilder.Add((Expression) Expression.Assign((Expression) this._constantPool, (Expression) Expression.NewArrayBounds(typeof (object), (Expression) Expression.Constant((object) this._constants.Count))));
        for (int index = this._constants.Count - 1; index >= 0; --index)
          collectionBuilder.Add((Expression) Expression.Assign((Expression) Expression.ArrayAccess((Expression) this._constantPool, (Expression) Expression.Constant((object) index)), this._constants[index]));
        collectionBuilder.Add(body);
        body = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
        {
          this._constantPool
        }, (IEnumerable<Expression>) collectionBuilder);
      }
      return (Expression) Expression.Lambda<T>(body, $"{node.Name}${(object) Interlocked.Increment(ref ToDiskRewriter._uniqueNameId)}", node.TailCall, (IEnumerable<ParameterExpression>) node.Parameters);
    }
    finally
    {
      --this._depth;
    }
  }

  protected virtual Expression VisitExtension(Expression node)
  {
    return node.NodeType == ExpressionType.Dynamic ? ((ExpressionVisitor) this).VisitDynamic((DynamicExpression) node) : ((ExpressionVisitor) this).Visit(node.Reduce());
  }

  protected virtual Expression VisitConstant(ConstantExpression node)
  {
    if (node.Value is CallSite site)
      return this.RewriteCallSite(site);
    if (node.Value is IExpressionSerializable expressionSerializable)
    {
      this.EnsureConstantPool();
      Expression expression1;
      if (!this._constantCache.TryGetValue(node.Value, out expression1))
      {
        Expression expression2 = expressionSerializable.CreateExpression();
        this._constants.Add(expression2);
        Dictionary<object, Expression> constantCache = this._constantCache;
        object key = node.Value;
        ParameterExpression constantPool = this._constantPool;
        Expression[] expressionArray = new Expression[1]
        {
          Microsoft.Scripting.Ast.Utils.Constant((object) (this._constants.Count - 1))
        };
        Expression expression3;
        expression1 = expression3 = Microsoft.Scripting.Ast.Utils.Convert((Expression) Expression.ArrayAccess((Expression) constantPool, expressionArray), expression2.Type);
        constantCache[key] = expression3;
      }
      return expression1;
    }
    if (!(node.Value is string[] collection))
    {
      // ISSUE: explicit non-virtual call
      return __nonvirtual (((ExpressionVisitor) this).VisitConstant(node));
    }
    if (collection.Length == 0)
      return (Expression) Expression.Field((Expression) null, typeof (ArrayUtils).GetDeclaredField("EmptyStrings"));
    this._constants.Add((Expression) Expression.NewArrayInit(typeof (string), (IEnumerable<Expression>) new ReadOnlyCollection<Expression>((IList<Expression>) ((ICollection<string>) collection).Map<string, ConstantExpression>((Func<string, ConstantExpression>) (s => Expression.Constant((object) s, typeof (string)))))));
    return Microsoft.Scripting.Ast.Utils.Convert((Expression) Expression.ArrayAccess((Expression) this._constantPool, Microsoft.Scripting.Ast.Utils.Constant((object) (this._constants.Count - 1))), typeof (string[]));
  }

  protected virtual Expression VisitDynamic(DynamicExpression node)
  {
    Type newDelegateType;
    if (this.RewriteDelegate(node.DelegateType, out newDelegateType))
      node = Expression.MakeDynamic(newDelegateType, node.Binder, (IEnumerable<Expression>) node.Arguments);
    return ((ExpressionVisitor) this).Visit(CompilerHelpers.Reduce(node));
  }

  private bool RewriteDelegate(Type delegateType, out Type newDelegateType)
  {
    if (!this.ShouldRewriteDelegate(delegateType))
    {
      newDelegateType = (Type) null;
      return false;
    }
    if (this._delegateTypes == null)
      this._delegateTypes = new Dictionary<Type, Type>();
    if (!this._delegateTypes.TryGetValue(delegateType, out newDelegateType))
    {
      MethodInfo method = delegateType.GetMethod("Invoke");
      newDelegateType = this._typeGen.AssemblyGen.MakeDelegateType(delegateType.Name, ((ICollection<ParameterInfo>) method.GetParameters()).Map<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType)), method.ReturnType);
      this._delegateTypes[delegateType] = newDelegateType;
    }
    return true;
  }

  private bool ShouldRewriteDelegate(Type delegateType)
  {
    ModuleBuilder module = delegateType.Module as ModuleBuilder;
    if ((Module) module == (Module) null)
    {
      if (!(delegateType.Module.GetType() == typeof (ModuleBuilder).Assembly.GetType("System.Reflection.Emit.InternalModuleBuilder")))
        return false;
      if ((bool) delegateType.Module.GetType().InvokeMember("IsTransientInternal", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, (Binder) null, (object) delegateType.Module, (object[]) null))
        return true;
    }
    else if (module.IsTransient())
      return true;
    return Snippets.Shared.SaveSnippets && module.Assembly != (Assembly) this._typeGen.AssemblyGen.AssemblyBuilder;
  }

  private Expression RewriteCallSite(CallSite site)
  {
    if (!(site.Binder is IExpressionSerializable binder))
      throw Microsoft.Scripting.Error.GenNonSerializableBinder();
    this.EnsureConstantPool();
    Type type = site.GetType();
    this._constants.Add((Expression) Expression.Call(type.GetMethod("Create"), binder.CreateExpression()));
    return ((ExpressionVisitor) this).Visit(Microsoft.Scripting.Ast.Utils.Convert((Expression) Expression.ArrayAccess((Expression) this._constantPool, Microsoft.Scripting.Ast.Utils.Constant((object) (this._constants.Count - 1))), type));
  }

  private void EnsureConstantPool()
  {
    if (this._constantPool != null)
      return;
    this._constantPool = Expression.Variable(typeof (object[]), "$constantPool");
    this._constants = new List<Expression>();
    this._constantCache = new Dictionary<object, Expression>();
  }
}
