// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Error
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting;

internal static class Error
{
  internal static Exception COMObjectDoesNotSupportEvents()
  {
    return (Exception) new ArgumentException(Strings.COMObjectDoesNotSupportEvents);
  }

  internal static Exception COMObjectDoesNotSupportSourceInterface()
  {
    return (Exception) new ArgumentException(Strings.COMObjectDoesNotSupportSourceInterface);
  }

  internal static Exception SetComObjectDataFailed()
  {
    return (Exception) new InvalidOperationException(Strings.SetComObjectDataFailed);
  }

  internal static Exception MethodShouldNotBeCalled()
  {
    return (Exception) new InvalidOperationException(Strings.MethodShouldNotBeCalled);
  }

  internal static Exception UnexpectedVarEnum(object p0)
  {
    return (Exception) new InvalidOperationException(Strings.UnexpectedVarEnum(p0));
  }

  internal static Exception DispBadParamCount(object p0)
  {
    return (Exception) new TargetParameterCountException(Strings.DispBadParamCount(p0));
  }

  internal static Exception DispMemberNotFound(object p0)
  {
    return (Exception) new MissingMemberException(Strings.DispMemberNotFound(p0));
  }

  internal static Exception DispNoNamedArgs(object p0)
  {
    return (Exception) new ArgumentException(Strings.DispNoNamedArgs(p0));
  }

  internal static Exception DispOverflow(object p0)
  {
    return (Exception) new OverflowException(Strings.DispOverflow(p0));
  }

  internal static Exception DispTypeMismatch(object p0, object p1)
  {
    return (Exception) new ArgumentException(Strings.DispTypeMismatch(p0, p1));
  }

  internal static Exception DispParamNotOptional(object p0)
  {
    return (Exception) new ArgumentException(Strings.DispParamNotOptional(p0));
  }

  internal static Exception CannotRetrieveTypeInformation()
  {
    return (Exception) new InvalidOperationException(Strings.CannotRetrieveTypeInformation);
  }

  internal static Exception GetIDsOfNamesInvalid(object p0)
  {
    return (Exception) new ArgumentException(Strings.GetIDsOfNamesInvalid(p0));
  }

  internal static Exception UnsupportedEnumType()
  {
    return (Exception) new InvalidOperationException(Strings.UnsupportedEnumType);
  }

  internal static Exception UnsupportedHandlerType()
  {
    return (Exception) new InvalidOperationException(Strings.UnsupportedHandlerType);
  }

  internal static Exception CouldNotGetDispId(object p0, object p1)
  {
    return (Exception) new MissingMemberException(Strings.CouldNotGetDispId(p0, p1));
  }

  internal static Exception AmbiguousConversion(object p0, object p1)
  {
    return (Exception) new AmbiguousMatchException(Strings.AmbiguousConversion(p0, p1));
  }

  internal static Exception VariantGetAccessorNYI(object p0)
  {
    return (Exception) new NotImplementedException(Strings.VariantGetAccessorNYI(p0));
  }

  internal static Exception MustHaveCodeOrTarget()
  {
    return (Exception) new ArgumentException(Strings.MustHaveCodeOrTarget);
  }

  internal static Exception TypeParameterIsNotDelegate(object p0)
  {
    return (Exception) new InvalidOperationException(Strings.TypeParameterIsNotDelegate(p0));
  }

  internal static Exception InvalidCast(object p0, object p1)
  {
    return (Exception) new InvalidOperationException(Strings.InvalidCast(p0, p1));
  }

  internal static Exception UnknownMemberType(object p0)
  {
    return (Exception) new InvalidOperationException(Strings.UnknownMemberType(p0));
  }

  internal static Exception FirstArgumentMustBeCallSite()
  {
    return (Exception) new InvalidOperationException(Strings.FirstArgumentMustBeCallSite);
  }

  internal static Exception NoInstanceForCall()
  {
    return (Exception) new InvalidOperationException(Strings.NoInstanceForCall);
  }

  internal static Exception MissingTest()
  {
    return (Exception) new InvalidOperationException(Strings.MissingTest);
  }

  internal static Exception MissingTarget()
  {
    return (Exception) new InvalidOperationException(Strings.MissingTarget);
  }

  internal static Exception NonGenericWithGenericGroup(object p0)
  {
    return (Exception) new TypeLoadException(Strings.NonGenericWithGenericGroup(p0));
  }

  internal static Exception InvalidOperation(object p0)
  {
    return (Exception) new ArgumentException(Strings.InvalidOperation(p0));
  }

  internal static Exception FinallyAlreadyDefined()
  {
    return (Exception) new InvalidOperationException(Strings.FinallyAlreadyDefined);
  }

  internal static Exception CannotHaveFaultAndFinally()
  {
    return (Exception) new InvalidOperationException(Strings.CannotHaveFaultAndFinally);
  }

  internal static Exception FaultAlreadyDefined()
  {
    return (Exception) new InvalidOperationException(Strings.FaultAlreadyDefined);
  }

  internal static Exception CantCreateDefaultTypeFor(object p0)
  {
    return (Exception) new ArgumentException(Strings.CantCreateDefaultTypeFor(p0));
  }

  internal static Exception UnhandledConvert(object p0)
  {
    return (Exception) new ArgumentException(Strings.UnhandledConvert(p0));
  }

  internal static Exception NoCallableMethods(object p0, object p1)
  {
    return (Exception) new InvalidOperationException(Strings.NoCallableMethods(p0, p1));
  }

  internal static Exception GlobalsMustBeUnique()
  {
    return (Exception) new ArgumentException(Strings.GlobalsMustBeUnique);
  }

  internal static Exception GenNonSerializableBinder()
  {
    return (Exception) new ArgumentException(Strings.GenNonSerializableBinder);
  }

  internal static Exception InvalidPath() => (Exception) new ArgumentException(Strings.InvalidPath);

  internal static Exception DictionaryNotHashable()
  {
    return (Exception) new ArgumentTypeException(Strings.DictionaryNotHashable);
  }

  internal static Exception LanguageRegistered()
  {
    return (Exception) new InvalidOperationException(Strings.LanguageRegistered);
  }

  internal static Exception MethodOrOperatorNotImplemented()
  {
    return (Exception) new NotImplementedException(Strings.MethodOrOperatorNotImplemented);
  }

  internal static Exception NoException()
  {
    return (Exception) new InvalidOperationException(Strings.NoException);
  }

  internal static Exception ExtensionMustBePublic(object p0)
  {
    return (Exception) new ArgumentException(Strings.ExtensionMustBePublic(p0));
  }

  internal static Exception AlreadyInitialized()
  {
    return (Exception) new InvalidOperationException(Strings.AlreadyInitialized);
  }

  internal static Exception MustReturnScopeExtension()
  {
    return (Exception) new InvalidImplementationException(Strings.MustReturnScopeExtension);
  }

  internal static Exception InvalidParamNumForService()
  {
    return (Exception) new ArgumentException(Strings.InvalidParamNumForService);
  }

  internal static Exception InvalidArgumentType(object p0, object p1)
  {
    return (Exception) new ArgumentException(Strings.InvalidArgumentType(p0, p1));
  }

  internal static Exception CannotChangeNonCachingValue()
  {
    return (Exception) new ArgumentException(Strings.CannotChangeNonCachingValue);
  }

  internal static Exception FieldReadonly(object p0)
  {
    return (Exception) new MissingMemberException(Strings.FieldReadonly(p0));
  }

  internal static Exception PropertyReadonly(object p0)
  {
    return (Exception) new MissingMemberException(Strings.PropertyReadonly(p0));
  }

  internal static Exception UnexpectedEvent(object p0, object p1, object p2, object p3)
  {
    return (Exception) new ArgumentException(Strings.UnexpectedEvent(p0, p1, p2, p3));
  }

  internal static Exception ExpectedBoundEvent(object p0)
  {
    return (Exception) new ArgumentTypeException(Strings.ExpectedBoundEvent(p0));
  }

  internal static Exception UnexpectedType(object p0, object p1)
  {
    return (Exception) new ArgumentTypeException(Strings.UnexpectedType(p0, p1));
  }

  internal static Exception MemberWriteOnly(object p0)
  {
    return (Exception) new MemberAccessException(Strings.MemberWriteOnly(p0));
  }

  internal static Exception NoCodeToCompile()
  {
    return (Exception) new InvalidOperationException(Strings.NoCodeToCompile);
  }

  internal static Exception InvalidStreamType(object p0)
  {
    return (Exception) new ArgumentException(Strings.InvalidStreamType(p0));
  }

  internal static Exception QueueEmpty()
  {
    return (Exception) new InvalidOperationException(Strings.QueueEmpty);
  }

  internal static Exception EnumerationNotStarted()
  {
    return (Exception) new InvalidOperationException(Strings.EnumerationNotStarted);
  }

  internal static Exception EnumerationFinished()
  {
    return (Exception) new InvalidOperationException(Strings.EnumerationFinished);
  }

  internal static Exception CantAddCasing(object p0)
  {
    return (Exception) new InvalidOperationException(Strings.CantAddCasing(p0));
  }

  internal static Exception CantAddIdentifier(object p0)
  {
    return (Exception) new InvalidOperationException(Strings.CantAddIdentifier(p0));
  }

  internal static Exception InvalidOutputDir()
  {
    return (Exception) new ArgumentException(Strings.InvalidOutputDir);
  }

  internal static Exception InvalidAsmNameOrExtension()
  {
    return (Exception) new ArgumentException(Strings.InvalidAsmNameOrExtension);
  }

  internal static Exception CanotEmitConstant(object p0, object p1)
  {
    return (Exception) new ArgumentException(Strings.CanotEmitConstant(p0, p1));
  }

  internal static Exception NoImplicitCast(object p0, object p1)
  {
    return (Exception) new ArgumentException(Strings.NoImplicitCast(p0, p1));
  }

  internal static Exception NoExplicitCast(object p0, object p1)
  {
    return (Exception) new ArgumentException(Strings.NoExplicitCast(p0, p1));
  }

  internal static Exception NameNotDefined(object p0)
  {
    return (Exception) new MissingMemberException(Strings.NameNotDefined(p0));
  }

  internal static Exception NoDefaultValue()
  {
    return (Exception) new ArgumentException(Strings.NoDefaultValue);
  }

  internal static Exception UnknownLanguageProviderType()
  {
    return (Exception) new ArgumentException(Strings.UnknownLanguageProviderType);
  }

  internal static Exception CantReadProperty()
  {
    return (Exception) new InvalidOperationException(Strings.CantReadProperty);
  }

  internal static Exception CantWriteProperty()
  {
    return (Exception) new InvalidOperationException(Strings.CantWriteProperty);
  }

  internal static Exception IllegalNew_GenericParams(object p0)
  {
    return (Exception) new ArgumentException(Strings.IllegalNew_GenericParams(p0));
  }

  internal static Exception VerificationException(object p0, object p1, object p2)
  {
    return (Exception) new System.Security.VerificationException(Strings.VerificationException(p0, p1, p2));
  }
}
