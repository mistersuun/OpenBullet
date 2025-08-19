// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Strings
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;
using System.Globalization;

#nullable disable
namespace Microsoft.Scripting;

internal static class Strings
{
  private static string FormatString(string format, params object[] args)
  {
    return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
  }

  internal static string InvalidOperation_ContainsGenericParameters(object p0, object p1)
  {
    return Strings.FormatString("Cannot access member {1} declared on type {0} because the type contains generic parameters.", p0, p1);
  }

  internal static string MissingType(object p0)
  {
    return Strings.FormatString("Type '{0}' is missing or cannot be loaded.", p0);
  }

  internal static string StaticAccessFromInstanceError(object p0, object p1)
  {
    return Strings.FormatString("static property \"{0}\" of \"{1}\" can only be read through a type, not an instance", p0, p1);
  }

  internal static string StaticAssignmentFromInstanceError(object p0, object p1)
  {
    return Strings.FormatString("static property \"{0}\" of \"{1}\" can only be assigned to through a type, not an instance", p0, p1);
  }

  internal static string MethodPreconditionViolated => "Method precondition violated";

  internal static string InvalidArgumentValue => "Invalid argument value";

  internal static string NonEmptyStringRequired => "Non-empty string required";

  internal static string NonEmptyCollectionRequired => "Non-empty collection required";

  internal static string MustBeExceptionInstance => "must by an Exception instance";

  internal static string TypeOfTestMustBeBool => "Type of test must be bool";

  internal static string TypeOfExpressionMustBeBool => "Type of the expression must be bool";

  internal static string EmptyStringIsInvalidPath => "Empty string is not a valid path.";

  internal static string InvalidDelegate => "Invalid delegate type (Invoke method not found).";

  internal static string ExpectedStaticProperty => "expected only static property";

  internal static string PropertyDoesNotExist => "Property doesn't exist on the provided type";

  internal static string FieldDoesNotExist => "Field doesn't exist on provided type";

  internal static string TypeDoesNotHaveConstructorForTheSignature
  {
    get => "Type doesn't have constructor with a given signature";
  }

  internal static string TypeDoesNotHaveMethodForName
  {
    get => "Type doesn't have a method with a given name.";
  }

  internal static string TypeDoesNotHaveMethodForNameSignature
  {
    get => "Type doesn't have a method with a given name and signature.";
  }

  internal static string CountCannotBeNegative => "Count must be non-negative.";

  internal static string ArrayTypeMustBeArray => "arrayType must be an array type";

  internal static string MustHaveCodeOrTarget => "Either code or target must be specified.";

  internal static string TypeParameterIsNotDelegate(object p0)
  {
    return Strings.FormatString("Type parameter is {0}. Expected a delegate.", p0);
  }

  internal static string InvalidCast(object p0, object p1)
  {
    return Strings.FormatString("Cannot cast from type '{0}' to type '{1}", p0, p1);
  }

  internal static string UnknownMemberType(object p0)
  {
    return Strings.FormatString("unknown member type: '{0}'. ", p0);
  }

  internal static string FirstArgumentMustBeCallSite
  {
    get => "RuleBuilder can only be used with delegates whose first argument is CallSite.";
  }

  internal static string NoInstanceForCall => "no instance for call.";

  internal static string MissingTest => "Missing Test.";

  internal static string MissingTarget => "Missing Target.";

  internal static string NonGenericWithGenericGroup(object p0)
  {
    return Strings.FormatString("The operation requires a non-generic type for {0}, but this represents generic types only", p0);
  }

  internal static string InvalidOperation(object p0)
  {
    return Strings.FormatString("Invalid operation: '{0}'", p0);
  }

  internal static string FinallyAlreadyDefined => "Finally already defined.";

  internal static string CannotHaveFaultAndFinally => "Can not have fault and finally.";

  internal static string FaultAlreadyDefined => "Fault already defined.";

  internal static string CantCreateDefaultTypeFor(object p0)
  {
    return Strings.FormatString("Cannot create default value for type {0}.", p0);
  }

  internal static string UnhandledConvert(object p0)
  {
    return Strings.FormatString("Unhandled convert: {0}", p0);
  }

  internal static string NoCallableMethods(object p0, object p1)
  {
    return Strings.FormatString("{0}.{1} has no publiclly visible method.", p0, p1);
  }

  internal static string GlobalsMustBeUnique
  {
    get => "Global/top-level local variable names must be unique.";
  }

  internal static string GenNonSerializableBinder
  {
    get => "Generating code from non-serializable CallSiteBinder.";
  }

  internal static string InvalidPath => "Specified path is invalid.";

  internal static string DictionaryNotHashable => "Dictionaries are not hashable.";

  internal static string LanguageRegistered => "language already registered.";

  internal static string MethodOrOperatorNotImplemented
  {
    get => "The method or operation is not implemented.";
  }

  internal static string NoException => "No exception.";

  internal static string ExtensionMustBePublic(object p0)
  {
    return Strings.FormatString("Extension type {0} must be public.", p0);
  }

  internal static string AlreadyInitialized => "Already initialized.";

  internal static string MustReturnScopeExtension
  {
    get => "CreateScopeExtension must return a scope extension.";
  }

  internal static string InvalidParamNumForService
  {
    get => "Invalid number of parameters for the service.";
  }

  internal static string InvalidArgumentType(object p0, object p1)
  {
    return Strings.FormatString("Invalid type of argument {0}; expecting {1}.", p0, p1);
  }

  internal static string CannotChangeNonCachingValue => "Cannot change non-caching value.";

  internal static string FieldReadonly(object p0)
  {
    return Strings.FormatString("Field {0} is read-only", p0);
  }

  internal static string PropertyReadonly(object p0)
  {
    return Strings.FormatString("Property {0} is read-only", p0);
  }

  internal static string UnexpectedEvent(object p0, object p1, object p2, object p3)
  {
    return Strings.FormatString("Expected event from {0}.{1}, got event from {2}.{3}.", p0, p1, p2, p3);
  }

  internal static string ExpectedBoundEvent(object p0)
  {
    return Strings.FormatString("expected bound event, got {0}.", p0);
  }

  internal static string UnexpectedType(object p0, object p1)
  {
    return Strings.FormatString("Expected type {0}, got {1}.", p0, p1);
  }

  internal static string MemberWriteOnly(object p0)
  {
    return Strings.FormatString("can only write to member {0}.", p0);
  }

  internal static string NoCodeToCompile => "No code to compile.";

  internal static string InvalidStreamType(object p0)
  {
    return Strings.FormatString("Invalid stream type: {0}.", p0);
  }

  internal static string QueueEmpty => "Queue empty.";

  internal static string EnumerationNotStarted => "Enumeration has not started. Call MoveNext.";

  internal static string EnumerationFinished => "Enumeration already finished.";

  internal static string CantAddCasing(object p0)
  {
    return Strings.FormatString("can't add another casing for identifier {0}", p0);
  }

  internal static string CantAddIdentifier(object p0)
  {
    return Strings.FormatString("can't add new identifier {0}", p0);
  }

  internal static string InvalidCtorImplementation(object p0, object p1)
  {
    return Strings.FormatString("Type '{0}' doesn't provide a suitable public constructor or its implementation is faulty: {1}", p0, p1);
  }

  internal static string InvalidOutputDir => "Invalid output directory.";

  internal static string InvalidAsmNameOrExtension => "Invalid assembly name or file extension.";

  internal static string CanotEmitConstant(object p0, object p1)
  {
    return Strings.FormatString("Cannot emit constant {0} ({1})", p0, p1);
  }

  internal static string NoImplicitCast(object p0, object p1)
  {
    return Strings.FormatString("No implicit cast from {0} to {1}", p0, p1);
  }

  internal static string NoExplicitCast(object p0, object p1)
  {
    return Strings.FormatString("No explicit cast from {0} to {1}", p0, p1);
  }

  internal static string NameNotDefined(object p0)
  {
    return Strings.FormatString("name '{0}' not defined", p0);
  }

  internal static string NoDefaultValue => "No default value for a given type.";

  internal static string UnknownLanguageProviderType
  {
    get => "Specified language provider type is not registered.";
  }

  internal static string CantReadProperty => "can't read from property";

  internal static string CantWriteProperty => "can't write to property";

  internal static string IllegalNew_GenericParams(object p0)
  {
    return Strings.FormatString("Cannot create instance of {0} because it contains generic parameters", p0);
  }

  internal static string VerificationException(object p0, object p1, object p2)
  {
    return Strings.FormatString("Non-verifiable assembly generated: {0}:\nAssembly preserved as {1}\nError text:\n{2}\n", p0, p1, p2);
  }
}
