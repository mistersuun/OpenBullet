// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.LambdaBuilder
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
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Ast;

public class LambdaBuilder
{
  private readonly List<ParameterExpression> _locals = new List<ParameterExpression>();
  private List<ParameterExpression> _params = new List<ParameterExpression>();
  private readonly List<KeyValuePair<ParameterExpression, bool>> _visibleVars = new List<KeyValuePair<ParameterExpression, bool>>();
  private string _name;
  private Type _returnType;
  private Expression _body;
  private bool _visible = true;
  private bool _completed;
  private static int _lambdaId;

  internal LambdaBuilder(string name, Type returnType)
  {
    this._name = name;
    this._returnType = returnType;
  }

  public string Name
  {
    get => this._name;
    set
    {
      ContractUtils.RequiresNotNull((object) value, nameof (value));
      this._name = value;
    }
  }

  public Type ReturnType
  {
    get => this._returnType;
    set
    {
      ContractUtils.RequiresNotNull((object) value, nameof (value));
      this._returnType = value;
    }
  }

  public List<ParameterExpression> Locals => this._locals;

  public List<ParameterExpression> Parameters => this._params;

  public ParameterExpression ParamsArray { get; private set; }

  public Expression Body
  {
    get => this._body;
    set
    {
      ContractUtils.RequiresNotNull((object) value, nameof (value));
      this._body = value;
    }
  }

  public bool Dictionary { get; set; }

  public bool Visible
  {
    get => this._visible;
    set => this._visible = value;
  }

  public List<ParameterExpression> GetVisibleVariables()
  {
    List<ParameterExpression> visibleVariables = new List<ParameterExpression>(this._visibleVars.Count);
    foreach (KeyValuePair<ParameterExpression, bool> visibleVar in this._visibleVars)
    {
      if (this.EmitDictionary || visibleVar.Value)
        visibleVariables.Add(visibleVar.Key);
    }
    return visibleVariables;
  }

  public ParameterExpression Parameter(Type type, string name)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    ParameterExpression key = Expression.Parameter(type, name);
    this._params.Add(key);
    this._visibleVars.Add(new KeyValuePair<ParameterExpression, bool>(key, false));
    return key;
  }

  public ParameterExpression ClosedOverParameter(Type type, string name)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    ParameterExpression key = Expression.Parameter(type, name);
    this._params.Add(key);
    this._visibleVars.Add(new KeyValuePair<ParameterExpression, bool>(key, true));
    return key;
  }

  public void AddParameters(params ParameterExpression[] parameters)
  {
    this._params.AddRange((IEnumerable<ParameterExpression>) parameters);
  }

  public ParameterExpression CreateHiddenParameter(string name, Type type)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    ParameterExpression hiddenParameter = Expression.Parameter(type, name);
    this._params.Add(hiddenParameter);
    return hiddenParameter;
  }

  public ParameterExpression CreateParamsArray(Type type, string name)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    ContractUtils.Requires(type.IsArray, nameof (type));
    ContractUtils.Requires(type.GetArrayRank() == 1, nameof (type));
    ContractUtils.Requires(this.ParamsArray == null, nameof (type), "Already have parameter array");
    return this.ParamsArray = this.Parameter(type, name);
  }

  public Expression ClosedOverVariable(Type type, string name)
  {
    ParameterExpression key = Expression.Variable(type, name);
    this._locals.Add(key);
    this._visibleVars.Add(new KeyValuePair<ParameterExpression, bool>(key, true));
    return (Expression) key;
  }

  public Expression Variable(Type type, string name)
  {
    ParameterExpression key = Expression.Variable(type, name);
    this._locals.Add(key);
    this._visibleVars.Add(new KeyValuePair<ParameterExpression, bool>(key, false));
    return (Expression) key;
  }

  public ParameterExpression HiddenVariable(Type type, string name)
  {
    ParameterExpression parameterExpression = Expression.Variable(type, name);
    this._locals.Add(parameterExpression);
    return parameterExpression;
  }

  public void AddHiddenVariable(ParameterExpression temp)
  {
    ContractUtils.RequiresNotNull((object) temp, nameof (temp));
    this._locals.Add(temp);
  }

  public LambdaExpression MakeLambda(Type lambdaType)
  {
    this.Validate();
    this.EnsureSignature(lambdaType);
    LambdaExpression lambdaExpression = Expression.Lambda(lambdaType, this.AddDefaultReturn(this.MakeBody()), $"{this._name}${(object) Interlocked.Increment(ref LambdaBuilder._lambdaId)}", (IEnumerable<ParameterExpression>) new ReadOnlyCollectionBuilder<ParameterExpression>((IEnumerable<ParameterExpression>) this._params));
    this._completed = true;
    return lambdaExpression;
  }

  public LambdaExpression MakeLambda()
  {
    ContractUtils.Requires(this.ParamsArray == null, "Paramarray lambdas require explicit delegate type");
    this.Validate();
    LambdaExpression lambdaExpression = Expression.Lambda(LambdaBuilder.GetLambdaType(this._returnType, (IList<ParameterExpression>) this._params), this.AddDefaultReturn(this.MakeBody()), $"{this._name}${(object) Interlocked.Increment(ref LambdaBuilder._lambdaId)}", (IEnumerable<ParameterExpression>) this._params);
    this._completed = true;
    return lambdaExpression;
  }

  public LambdaExpression MakeGenerator(LabelTarget label, Type lambdaType)
  {
    this.Validate();
    this.EnsureSignature(lambdaType);
    LambdaExpression lambdaExpression = Microsoft.Scripting.Ast.Utils.GeneratorLambda(lambdaType, label, this.MakeBody(), $"{this._name}${(object) Interlocked.Increment(ref LambdaBuilder._lambdaId)}", (IEnumerable<ParameterExpression>) this._params);
    this._completed = true;
    return lambdaExpression;
  }

  private void EnsureSignature(Type delegateType)
  {
    ParameterInfo[] parameters = delegateType.GetMethod("Invoke").GetParameters();
    bool flag1 = parameters.Length != 0 && parameters[parameters.Length - 1].IsDefined(typeof (ParamArrayAttribute), false);
    bool flag2 = this.ParamsArray != null;
    if (flag2 && !flag1)
      throw new ArgumentException("paramarray lambdas must have paramarray delegate type");
    int num1 = flag1 ? parameters.Length - 1 : parameters.Length;
    int num2 = this._params.Count - num1;
    if (flag2)
      --num2;
    if (num2 < 0)
      throw new ArgumentException("lambda does not have enough parameters");
    if (!flag1)
    {
      bool flag3 = false;
      for (int index = 0; index < num1; ++index)
      {
        if (this._params[index].Type != parameters[index].ParameterType)
          flag3 = true;
      }
      if (!flag3)
        return;
    }
    List<ParameterExpression> parameterExpressionList = new List<ParameterExpression>(parameters.Length);
    List<ParameterExpression> collection = new List<ParameterExpression>();
    List<Expression> expressionList = new List<Expression>();
    System.Collections.Generic.Dictionary<ParameterExpression, ParameterExpression> map = new System.Collections.Generic.Dictionary<ParameterExpression, ParameterExpression>();
    for (int index = 0; index < num1; ++index)
    {
      if (this._params[index].Type != parameters[index].ParameterType)
      {
        ParameterExpression parameterExpression = Expression.Parameter(parameters[index].ParameterType, parameters[index].Name);
        ParameterExpression key = this._params[index];
        ParameterExpression left = Expression.Variable(key.Type, key.Name);
        parameterExpressionList.Add(parameterExpression);
        collection.Add(left);
        map.Add(key, left);
        expressionList.Add((Expression) Expression.Assign((Expression) left, (Expression) Expression.Convert((Expression) parameterExpression, key.Type)));
      }
      else
      {
        parameterExpressionList.Add(this._params[index]);
        map.Add(this._params[index], this._params[index]);
      }
    }
    if (flag1)
    {
      ParameterInfo parameterInfo = parameters[parameters.Length - 1];
      ParameterExpression array = Expression.Parameter(parameterInfo.ParameterType, parameterInfo.Name);
      parameterExpressionList.Add(array);
      for (int index = 0; index < num2; ++index)
      {
        ParameterExpression key = this._params[num1 + index];
        ParameterExpression left = Expression.Variable(key.Type, key.Name);
        collection.Add(left);
        map.Add(key, left);
        expressionList.Add((Expression) Expression.Assign((Expression) left, Microsoft.Scripting.Ast.Utils.Convert((Expression) Expression.ArrayAccess((Expression) array, Microsoft.Scripting.Ast.Utils.Constant((object) index)), key.Type)));
      }
      if (flag2)
      {
        ParameterExpression paramsArray = this.ParamsArray;
        ParameterExpression left = Expression.Variable(paramsArray.Type, paramsArray.Name);
        collection.Add(left);
        map.Add(paramsArray, left);
        MethodInfo method = typeof (ScriptingRuntimeHelpers).GetMethod("ShiftParamsArray").MakeGenericMethod(parameterInfo.ParameterType.GetElementType());
        expressionList.Add((Expression) Expression.Assign((Expression) left, Microsoft.Scripting.Ast.Utils.Convert((Expression) Expression.Call(method, (Expression) array, Microsoft.Scripting.Ast.Utils.Constant((object) num2)), paramsArray.Type)));
      }
    }
    Expression expression = new LambdaParameterRewriter(map).Visit(this._body);
    expressionList.Add(expression);
    this._body = (Expression) Expression.Block((IEnumerable<Expression>) expressionList);
    this.ParamsArray = (ParameterExpression) null;
    this._locals.AddRange((IEnumerable<ParameterExpression>) collection);
    this._params = parameterExpressionList;
    for (int index1 = 0; index1 < this._visibleVars.Count; ++index1)
    {
      KeyValuePair<ParameterExpression, bool> visibleVar = this._visibleVars[index1];
      ParameterExpression key1 = visibleVar.Key;
      ParameterExpression parameterExpression;
      if (key1 != null && map.TryGetValue(key1, out parameterExpression))
      {
        List<KeyValuePair<ParameterExpression, bool>> visibleVars = this._visibleVars;
        int index2 = index1;
        ParameterExpression key2 = parameterExpression;
        visibleVar = this._visibleVars[index1];
        int num3 = visibleVar.Value ? 1 : 0;
        KeyValuePair<ParameterExpression, bool> keyValuePair = new KeyValuePair<ParameterExpression, bool>(key2, num3 != 0);
        visibleVars[index2] = keyValuePair;
      }
    }
  }

  private void Validate()
  {
    if (this._completed)
      throw new InvalidOperationException("The builder is closed");
    if (this._returnType == (Type) null)
      throw new InvalidOperationException("Return type is missing");
    if (this._name == null)
      throw new InvalidOperationException("Name is missing");
    if (this._body == null)
      throw new InvalidOperationException("Body is missing");
    if (this.ParamsArray != null && (this._params.Count == 0 || this._params[this._params.Count - 1] != this.ParamsArray))
      throw new InvalidOperationException("The params array parameter is not last in the parameter list");
  }

  private bool EmitDictionary => this.Dictionary;

  private Expression MakeBody()
  {
    Expression expression = this._body;
    if (this._locals != null && this._locals.Count > 0)
      expression = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ReadOnlyCollection<ParameterExpression>((IList<ParameterExpression>) this._locals.ToArray()), expression);
    return expression;
  }

  private Expression AddDefaultReturn(Expression body)
  {
    if (body.Type == typeof (void) && this._returnType != typeof (void))
      body = (Expression) Expression.Block(body, (Expression) Microsoft.Scripting.Ast.Utils.Default(this._returnType));
    return body;
  }

  private static Type GetLambdaType(Type returnType, IList<ParameterExpression> parameterList)
  {
    ContractUtils.RequiresNotNull((object) returnType, nameof (returnType));
    bool flag = returnType == typeof (void);
    int count = parameterList != null ? parameterList.Count : 0;
    Type[] typeArray = new Type[count + (flag ? 0 : 1)];
    for (int index = 0; index < count; ++index)
    {
      ContractUtils.RequiresNotNull((object) parameterList[index], "parameter");
      typeArray[index] = parameterList[index].Type;
    }
    Type lambdaType;
    if (flag)
    {
      lambdaType = Expression.GetActionType(typeArray);
    }
    else
    {
      typeArray[count] = returnType;
      lambdaType = Expression.GetFuncType(typeArray);
    }
    return lambdaType;
  }
}
