// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Generation.CompilerHelpers
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Interpreter;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Generation;

public static class CompilerHelpers
{
  public static readonly MethodAttributes PublicStatic = MethodAttributes.Public | MethodAttributes.Static;
  private static readonly MethodInfo _CreateInstanceMethod = typeof (ScriptingRuntimeHelpers).GetMethod("CreateInstance");
  private static int _Counter;

  public static object GetMissingValue(Type type)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    if (type.IsByRef)
      type = type.GetElementType();
    if (type.IsEnum())
      return Activator.CreateInstance(type);
    switch (type.GetTypeCode())
    {
      case TypeCode.Empty:
      case TypeCode.DBNull:
      case TypeCode.String:
        return (object) null;
      case TypeCode.Boolean:
        return (object) false;
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
        return (object) 0;
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
        return (object) DateTime.MinValue;
      default:
        if (type.IsSealed() && type.IsValueType())
          return Activator.CreateInstance(type);
        if (type == typeof (object))
          return (object) Missing.Value;
        if (!type.IsValueType())
          return (object) null;
        throw Microsoft.Scripting.Error.CantCreateDefaultTypeFor((object) type);
    }
  }

  public static bool IsStatic(MethodBase mi) => mi.IsConstructor || mi.IsStatic;

  public static bool IsConstructor(MethodBase mb)
  {
    return mb.IsConstructor || mb.IsGenericMethod && (mb as MethodInfo).GetGenericMethodDefinition() == CompilerHelpers._CreateInstanceMethod;
  }

  public static T[] MakeRepeatedArray<T>(T item, int count)
  {
    T[] objArray = new T[count];
    for (int index = 0; index < count; ++index)
      objArray[index] = item;
    return objArray;
  }

  public static bool IsComparisonOperator(ExpressionType op)
  {
    switch (op)
    {
      case ExpressionType.Equal:
        return true;
      case ExpressionType.GreaterThan:
        return true;
      case ExpressionType.GreaterThanOrEqual:
        return true;
      case ExpressionType.LessThan:
        return true;
      case ExpressionType.LessThanOrEqual:
        return true;
      case ExpressionType.NotEqual:
        return true;
      default:
        return false;
    }
  }

  public static Type GetType(object obj) => obj == null ? typeof (DynamicNull) : obj.GetType();

  public static Type[] GetTypes(object[] args)
  {
    Type[] types = new Type[args.Length];
    for (int index = 0; index < args.Length; ++index)
      types[index] = CompilerHelpers.GetType(args[index]);
    return types;
  }

  public static bool TypesEqual(IList args, int start, Type[] types)
  {
    for (int index = 0; index < types.Length; ++index)
    {
      object obj = args[start + index];
      if (types[index] != obj?.GetType())
        return false;
    }
    return true;
  }

  public static bool CanOptimizeMethod(MethodBase method)
  {
    return !method.ContainsGenericParameters && !method.IsProtected() && !method.IsPrivate && method.DeclaringType.IsVisible();
  }

  public static MethodInfo TryGetCallableMethod(Type targetType, MethodInfo method)
  {
    if (method.DeclaringType == (Type) null || method.DeclaringType.IsVisible())
      return method;
    MethodInfo callableMethod = RuntimeReflectionExtensions.GetRuntimeBaseDefinition(method);
    if (callableMethod.DeclaringType.IsVisible() || callableMethod.DeclaringType.IsInterface())
    {
      if (callableMethod.IsGenericMethodDefinition)
        callableMethod = callableMethod.MakeGenericMethod(method.GetGenericArguments());
      return callableMethod;
    }
    foreach (Type implementedInterface in targetType.GetImplementedInterfaces())
    {
      if (implementedInterface.IsPublic())
      {
        InterfaceMapping interfaceMap = targetType.GetInterfaceMap(implementedInterface);
        for (int index = 0; index < interfaceMap.TargetMethods.Length; ++index)
        {
          MethodInfo targetMethod = interfaceMap.TargetMethods[index];
          if (targetMethod != (MethodInfo) null && targetMethod.MethodHandle == method.MethodHandle)
            return interfaceMap.InterfaceMethods[index];
        }
      }
    }
    return method;
  }

  public static IEnumerable<MemberInfo> FilterNonVisibleMembers(
    Type targetType,
    IEnumerable<MemberInfo> members)
  {
    return targetType.IsVisible() ? members : CompilerHelpers.FilterNonVisibleMembersIterator(targetType, members);
  }

  public static IEnumerable<MemberInfo> FilterNonVisibleMembersIterator(
    Type targetType,
    IEnumerable<MemberInfo> members)
  {
    foreach (MemberInfo member in members)
    {
      MemberInfo visibleMember = CompilerHelpers.TryGetVisibleMember(targetType, member);
      if (visibleMember != (MemberInfo) null)
        yield return visibleMember;
    }
  }

  public static MemberInfo TryGetVisibleMember(Type targetType, MemberInfo member)
  {
    MemberInfo visibleMember = (MemberInfo) null;
    MethodInfo method1;
    if ((method1 = member as MethodInfo) != (MethodInfo) null)
    {
      MethodInfo callableMethod = CompilerHelpers.TryGetCallableMethod(targetType, method1);
      if (CompilerHelpers.IsVisible((MethodBase) callableMethod))
        visibleMember = (MemberInfo) callableMethod;
    }
    else
    {
      PropertyInfo propertyInfo;
      if ((propertyInfo = member as PropertyInfo) != (PropertyInfo) null)
      {
        Type targetType1 = targetType;
        MethodInfo method2 = propertyInfo.GetGetMethod();
        if ((object) method2 == null)
          method2 = propertyInfo.GetSetMethod();
        MethodInfo callableMethod = CompilerHelpers.TryGetCallableMethod(targetType1, method2);
        if (CompilerHelpers.IsVisible((MethodBase) callableMethod))
          visibleMember = (MemberInfo) callableMethod.DeclaringType.GetDeclaredProperty(propertyInfo.Name);
      }
      else
      {
        EventInfo eventInfo;
        if ((eventInfo = member as EventInfo) != (EventInfo) null)
        {
          Type targetType2 = targetType;
          MethodInfo method3 = eventInfo.GetAddMethod();
          if ((object) method3 == null)
            method3 = eventInfo.GetRemoveMethod() ?? eventInfo.GetRaiseMethod();
          MethodInfo callableMethod = CompilerHelpers.TryGetCallableMethod(targetType2, method3);
          if (CompilerHelpers.IsVisible((MethodBase) callableMethod))
            visibleMember = (MemberInfo) callableMethod.DeclaringType.GetDeclaredEvent(eventInfo.Name);
        }
      }
    }
    return visibleMember;
  }

  public static bool MemberEquals(this MemberInfo self, MemberInfo other)
  {
    if (self == (MemberInfo) null != (other == (MemberInfo) null))
      return false;
    if (self == (MemberInfo) null)
      return true;
    if (self.MemberType != other.MemberType)
      return false;
    switch (self.MemberType)
    {
      case MemberTypes.Constructor:
        return ((MethodBase) self).MethodHandle.Equals(((MethodBase) other).MethodHandle);
      case MemberTypes.Field:
        return ((FieldInfo) self).FieldHandle.Equals(((FieldInfo) other).FieldHandle);
      case MemberTypes.Method:
        return ((MethodBase) self).MethodHandle.Equals(((MethodBase) other).MethodHandle);
      case MemberTypes.TypeInfo:
      case MemberTypes.NestedType:
        return ((Type) self).TypeHandle.Equals(((Type) other).TypeHandle);
      default:
        return self.Module == other.Module && self.MetadataToken == other.MetadataToken;
    }
  }

  public static bool IsVisible(MethodBase info)
  {
    if (!info.IsPublic)
      return false;
    return info.DeclaringType == (Type) null || info.DeclaringType.IsVisible();
  }

  public static bool IsVisible(FieldInfo info)
  {
    if (!info.IsPublic)
      return false;
    return info.DeclaringType == (Type) null || info.DeclaringType.IsVisible();
  }

  public static bool IsProtected(this MethodBase info) => info.IsFamily || info.IsFamilyOrAssembly;

  public static bool IsProtected(this FieldInfo info) => info.IsFamily || info.IsFamilyOrAssembly;

  public static bool IsProtected(this Type type) => type.IsNestedFamily || type.IsNestedFamORAssem;

  public static Type GetVisibleType(object value)
  {
    return CompilerHelpers.GetVisibleType(CompilerHelpers.GetType(value));
  }

  public static Type GetVisibleType(Type t)
  {
    while (!t.IsVisible())
      t = t.GetBaseType();
    return t;
  }

  public static MethodBase[] GetConstructors(Type t, bool privateBinding)
  {
    return CompilerHelpers.GetConstructors(t, privateBinding, false);
  }

  public static MethodBase[] GetConstructors(Type t, bool privateBinding, bool includeProtected)
  {
    if (t.IsArray)
      return new MethodBase[1]
      {
        CompilerHelpers.GetArrayCtor(t)
      };
    BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
    if (privateBinding | includeProtected)
      flags |= BindingFlags.NonPublic;
    IEnumerable<ConstructorInfo> constructorInfos = t.GetDeclaredConstructors().WithBindingFlags(flags);
    if (!privateBinding & includeProtected)
      constructorInfos = CompilerHelpers.FilterConstructorsToPublicAndProtected(constructorInfos);
    if (t.IsValueType())
    {
      try
      {
        MethodBase structDefaultCtor = CompilerHelpers.GetStructDefaultCtor(t);
        List<MethodBase> methodBaseList = new List<MethodBase>();
        methodBaseList.Add(structDefaultCtor);
        methodBaseList.AddRange(constructorInfos.Cast<ConstructorInfo, MethodBase>());
        return methodBaseList.ToArray();
      }
      catch (BadImageFormatException ex)
      {
      }
    }
    return (MethodBase[]) constructorInfos.ToArray<ConstructorInfo>();
  }

  public static IEnumerable<ConstructorInfo> FilterConstructorsToPublicAndProtected(
    IEnumerable<ConstructorInfo> ctors)
  {
    foreach (ConstructorInfo ctor in ctors)
    {
      if (ctor.IsPublic || ctor.IsProtected())
        yield return ctor;
    }
  }

  private static MethodBase GetStructDefaultCtor(Type t)
  {
    return (MethodBase) ReflectionUtils.GetDeclaredMethods(typeof (ScriptingRuntimeHelpers), "CreateInstance").Single<MethodInfo>().MakeGenericMethod(t);
  }

  private static MethodBase GetArrayCtor(Type t)
  {
    return (MethodBase) ReflectionUtils.GetDeclaredMethods(typeof (ScriptingRuntimeHelpers), "CreateArray").Single<MethodInfo>().MakeGenericMethod(t.GetElementType());
  }

  public static MethodInfo GetImplicitConverter(Type fromType, Type toType)
  {
    MethodInfo converter = CompilerHelpers.GetConverter(fromType, fromType, toType, "op_Implicit");
    return (object) converter != null ? converter : CompilerHelpers.GetConverter(toType, fromType, toType, "op_Implicit");
  }

  public static MethodInfo GetExplicitConverter(Type fromType, Type toType)
  {
    MethodInfo converter = CompilerHelpers.GetConverter(fromType, fromType, toType, "op_Explicit");
    return (object) converter != null ? converter : CompilerHelpers.GetConverter(toType, fromType, toType, "op_Explicit");
  }

  private static MethodInfo GetConverter(
    Type type,
    Type fromType,
    Type toType,
    string opMethodName)
  {
    foreach (MethodInfo withBindingFlag in type.GetInheritedMembers(opMethodName).WithBindingFlags(BindingFlags.Static | BindingFlags.Public))
    {
      if ((withBindingFlag.DeclaringType == (Type) null || withBindingFlag.DeclaringType.IsVisible()) && withBindingFlag.IsPublic && withBindingFlag.ReturnType == toType && withBindingFlag.GetParameters()[0].ParameterType.IsAssignableFrom(fromType))
        return withBindingFlag;
    }
    return (MethodInfo) null;
  }

  public static bool TryImplicitConversion(object value, Type to, out object result)
  {
    if (CompilerHelpers.TryImplicitConvert(value, to, to.GetInheritedMethods("op_Implicit").WithBindingFlags(BindingFlags.Static | BindingFlags.Public), out result))
      return true;
    Type type = CompilerHelpers.GetType(value);
    while (!CompilerHelpers.TryImplicitConvert(value, to, type.GetInheritedMethods("op_Implicit").WithBindingFlags(BindingFlags.Static | BindingFlags.Public), out result))
    {
      type = type.GetBaseType();
      if (!(type != (Type) null))
        return false;
    }
    return true;
  }

  private static bool TryImplicitConvert(
    object value,
    Type to,
    IEnumerable<MethodInfo> implicitConv,
    out object result)
  {
    foreach (MethodInfo methodInfo in implicitConv)
    {
      if (to.IsValueType() == methodInfo.ReturnType.IsValueType() && to.IsAssignableFrom(methodInfo.ReturnType))
      {
        if (methodInfo.IsStatic)
          result = methodInfo.Invoke((object) null, new object[1]
          {
            value
          });
        else
          result = methodInfo.Invoke(value, ArrayUtils.EmptyObjects);
        return true;
      }
    }
    result = (object) null;
    return false;
  }

  public static bool IsStrongBox(object target)
  {
    return CompilerHelpers.IsStrongBox(CompilerHelpers.GetType(target));
  }

  public static bool IsStrongBox(Type t)
  {
    return t.IsGenericType() && t.GetGenericTypeDefinition() == typeof (StrongBox<>);
  }

  public static Expression GetTryConvertReturnValue(Type type)
  {
    Type type1 = type;
    return type1.IsInterface || type1.IsClass || type1.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>) ? (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) null, type) : Microsoft.Scripting.Ast.Utils.Constant(Activator.CreateInstance(type));
  }

  public static bool HasTypeConverter(Type fromType, Type toType)
  {
    return CompilerHelpers.TryGetTypeConverter(fromType, toType, out TypeConverter _);
  }

  public static bool TryApplyTypeConverter(object value, Type toType, out object result)
  {
    TypeConverter converter;
    if (value != null && CompilerHelpers.TryGetTypeConverter(value.GetType(), toType, out converter))
    {
      result = converter.ConvertFrom(value);
      return true;
    }
    result = value;
    return false;
  }

  public static bool TryGetTypeConverter(Type fromType, Type toType, out TypeConverter converter)
  {
    ContractUtils.RequiresNotNull((object) fromType, nameof (fromType));
    ContractUtils.RequiresNotNull((object) toType, nameof (toType));
    foreach (TypeConverterAttribute customAttribute in toType.GetCustomAttributes(typeof (TypeConverterAttribute), true))
    {
      try
      {
        converter = Activator.CreateInstance(Type.GetType(customAttribute.ConverterTypeName)) as TypeConverter;
      }
      catch (Exception ex)
      {
        converter = (TypeConverter) null;
      }
      if (converter != null && converter.CanConvertFrom(fromType))
        return true;
    }
    converter = (TypeConverter) null;
    return false;
  }

  public static MethodBase[] GetMethodTargets(object obj)
  {
    Type type = CompilerHelpers.GetType(obj);
    if (typeof (Delegate).IsAssignableFrom(type))
      return new MethodBase[1]
      {
        (MethodBase) type.GetMethod("Invoke")
      };
    if (typeof (BoundMemberTracker).IsAssignableFrom(type))
    {
      if ((obj as BoundMemberTracker).BoundTo.MemberType != TrackerTypes.Method)
        ;
    }
    else if (!typeof (MethodGroup).IsAssignableFrom(type) && !typeof (MemberGroup).IsAssignableFrom(type))
      return CompilerHelpers.MakeCallSignatureForCallableObject(type);
    return (MethodBase[]) null;
  }

  private static MethodBase[] MakeCallSignatureForCallableObject(Type t)
  {
    List<MethodBase> methodBaseList = new List<MethodBase>();
    foreach (MethodInfo inheritedMethod in t.GetInheritedMethods("Call"))
    {
      if (inheritedMethod.IsSpecialName)
        methodBaseList.Add((MethodBase) inheritedMethod);
    }
    return methodBaseList.ToArray();
  }

  public static Type[] GetSiteTypes(IList<Expression> arguments, Type returnType)
  {
    int count = arguments.Count;
    Type[] siteTypes = new Type[count + 1];
    for (int index = 0; index < count; ++index)
      siteTypes[index] = arguments[index].Type;
    siteTypes[count] = returnType;
    return siteTypes;
  }

  public static Type[] GetExpressionTypes(Expression[] expressions)
  {
    ContractUtils.RequiresNotNull((object) expressions, nameof (expressions));
    Type[] expressionTypes = new Type[expressions.Length];
    for (int index = 0; index < expressionTypes.Length; ++index)
    {
      ContractUtils.RequiresNotNull((object) expressions[index], "expressions[i]");
      expressionTypes[index] = expressions[index].Type;
    }
    return expressionTypes;
  }

  public static Type MakeCallSiteType(params Type[] types)
  {
    return typeof (CallSite<>).MakeGenericType(DelegateHelpers.MakeDelegate(types));
  }

  public static Type MakeCallSiteDelegateType(Type[] types) => DelegateHelpers.MakeDelegate(types);

  public static Delegate LightCompile(this LambdaExpression lambda)
  {
    return new LightCompiler(-1).CompileTop(lambda).CreateDelegate();
  }

  public static Delegate LightCompile(this LambdaExpression lambda, int compilationThreshold)
  {
    return new LightCompiler(compilationThreshold).CompileTop(lambda).CreateDelegate();
  }

  public static T LightCompile<T>(this Expression<T> lambda) => (T) lambda.LightCompile();

  public static T LightCompile<T>(this Expression<T> lambda, int compilationThreshold)
  {
    return (T) lambda.LightCompile(compilationThreshold);
  }

  public static void CompileToMethod(
    this LambdaExpression lambda,
    MethodBuilder method,
    bool emitDebugSymbols)
  {
    if (emitDebugSymbols)
    {
      ContractUtils.Requires((Module) (method.Module as ModuleBuilder) != (Module) null, nameof (method), "MethodBuilder does not have a valid ModuleBuilder");
      lambda.CompileToMethod(method, DebugInfoGenerator.CreatePdbGenerator());
    }
    else
      lambda.CompileToMethod(method);
  }

  public static T Compile<T>(this Expression<T> lambda, bool emitDebugSymbols)
  {
    return emitDebugSymbols ? CompilerHelpers.CompileToMethod<T>(lambda, DebugInfoGenerator.CreatePdbGenerator(), true) : lambda.Compile();
  }

  public static T CompileToMethod<T>(
    Expression<T> lambda,
    DebugInfoGenerator debugInfoGenerator,
    bool emitDebugSymbols)
  {
    return (T) CompilerHelpers.CompileToMethod((LambdaExpression) lambda, debugInfoGenerator, emitDebugSymbols);
  }

  public static Delegate CompileToMethod(
    LambdaExpression lambda,
    DebugInfoGenerator debugInfoGenerator,
    bool emitDebugSymbols)
  {
    string name = string.IsNullOrEmpty(lambda.Name) ? CompilerHelpers.GetUniqueMethodName() : lambda.Name;
    TypeBuilder typeBuilder = Snippets.Shared.DefineType(name, typeof (object), false, emitDebugSymbols).TypeBuilder;
    CompilerHelpers.DebuggableCodeRewriter debuggableCodeRewriter = new CompilerHelpers.DebuggableCodeRewriter(typeBuilder);
    lambda = (LambdaExpression) ((ExpressionVisitor) debuggableCodeRewriter).Visit((Expression) lambda);
    MethodBuilder method = typeBuilder.DefineMethod(name, CompilerHelpers.PublicStatic);
    lambda.CompileToMethod(method, debugInfoGenerator);
    Type type = typeBuilder.CreateType();
    debuggableCodeRewriter.InitializeFields(type);
    return type.GetMethod(method.Name).CreateDelegate(lambda.Type);
  }

  public static string GetUniqueMethodName()
  {
    return "lambda_method$" + (object) Interlocked.Increment(ref CompilerHelpers._Counter);
  }

  public static bool CanEmitConstant(object value, Type type)
  {
    if (value == null || CompilerHelpers.CanEmitILConstant(type))
      return true;
    Type t = value as Type;
    if (t != (Type) null && ILGen.ShouldLdtoken(t))
      return true;
    MethodBase mb = value as MethodBase;
    return mb != (MethodBase) null && ILGen.ShouldLdtoken(mb);
  }

  internal static bool CanEmitILConstant(Type type)
  {
    TypeCode typeCode = type.GetTypeCode();
    return (uint) (typeCode - 3) <= 12U || typeCode == TypeCode.String;
  }

  public static Expression Reduce(DynamicExpression node)
  {
    Expression right = Microsoft.Scripting.Ast.Utils.Constant((object) CallSite.Create(node.DelegateType, node.Binder));
    ParameterExpression left = Expression.Variable(right.Type, "$site");
    return (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      left
    }, (Expression) Expression.Call((Expression) Expression.Field((Expression) Expression.Assign((Expression) left, right), right.Type.GetDeclaredField("Target")), node.DelegateType.GetMethod("Invoke"), ArrayUtils.Insert<Expression>((Expression) left, (IList<Expression>) node.Arguments)));
  }

  [Obsolete("Use CreateBigInteger instead.")]
  public static BigInteger CreateBigInt(int value) => CompilerHelpers.CreateBigInteger(value);

  public static BigInteger CreateBigInteger(int value) => (BigInteger) value;

  [Obsolete("Use CreateBigInteger instead.")]
  public static BigInteger CreateBigInt(long value) => CompilerHelpers.CreateBigInteger(value);

  public static BigInteger CreateBigInteger(long value) => (BigInteger) value;

  [Obsolete("Use CreateBigInteger instead.")]
  public static BigInteger CreateBigInt(bool isNegative, byte[] data)
  {
    return CompilerHelpers.CreateBigInteger(isNegative, data);
  }

  public static BigInteger CreateBigInteger(bool isNegative, byte[] data)
  {
    BigInteger bigInteger = new BigInteger(data);
    return !isNegative ? bigInteger : -bigInteger;
  }

  private sealed class DebuggableCodeRewriter : DynamicExpressionVisitor
  {
    private readonly Dictionary<object, FieldBuilder> _fields = new Dictionary<object, FieldBuilder>((IEqualityComparer<object>) ReferenceEqualityComparer<object>.Instance);
    private readonly TypeBuilder _type;
    private readonly HashSet<string> _methodNames = new HashSet<string>();

    internal DebuggableCodeRewriter(TypeBuilder type) => this._type = type;

    internal void InitializeFields(Type type)
    {
      foreach (KeyValuePair<object, FieldBuilder> field in this._fields)
        type.GetInheritedFields(field.Value.Name).First<FieldInfo>().SetValue((object) null, field.Key);
    }

    protected virtual Expression VisitLambda<T>(Expression<T> node)
    {
      if (this._methodNames.Contains(node.Name))
      {
        int count = this._methodNames.Count;
        string name;
        do
        {
          name = $"{node.Name}${(object) count++}";
        }
        while (this._methodNames.Contains(name));
        this._methodNames.Add(name);
        return (Expression) Expression.Lambda<T>(((ExpressionVisitor) this).Visit(node.Body), name, node.TailCall, (IEnumerable<ParameterExpression>) node.Parameters);
      }
      this._methodNames.Add(node.Name);
      // ISSUE: explicit non-virtual call
      return __nonvirtual (((ExpressionVisitor) this).VisitLambda<T>(node));
    }

    protected virtual Expression VisitExtension(Expression node)
    {
      return node is LightDynamicExpression dynamicExpression ? ((ExpressionVisitor) this).Visit(dynamicExpression.Reduce()) : ((ExpressionVisitor) this).Visit(node.Reduce());
    }

    protected virtual Expression VisitConstant(ConstantExpression node)
    {
      if (CompilerHelpers.CanEmitConstant(node.Value, node.Type))
        return (Expression) node;
      FieldBuilder field;
      if (!this._fields.TryGetValue(node.Value, out field))
      {
        field = this._type.DefineField("$constant" + (object) this._fields.Count, CompilerHelpers.GetVisibleType(node.Value.GetType()), FieldAttributes.Public | FieldAttributes.Static);
        this._fields.Add(node.Value, field);
      }
      Expression expression = (Expression) Expression.Field((Expression) null, (FieldInfo) field);
      if (expression.Type != node.Type)
        expression = (Expression) Expression.Convert(expression, node.Type);
      return expression;
    }

    protected virtual Expression VisitDynamic(DynamicExpression node)
    {
      return ((ExpressionVisitor) this).Visit(CompilerHelpers.Reduce(node));
    }
  }
}
