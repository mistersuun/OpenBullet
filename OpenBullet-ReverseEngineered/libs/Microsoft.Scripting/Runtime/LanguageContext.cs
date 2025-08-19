// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.LanguageContext
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public abstract class LanguageContext
{
  private DynamicOperations _operations;

  protected LanguageContext(ScriptDomainManager domainManager)
  {
    ContractUtils.RequiresNotNull((object) domainManager, nameof (domainManager));
    this.DomainManager = domainManager;
    this.ContextId = domainManager.GenerateContextId();
  }

  public ContextId ContextId { get; }

  public ScriptDomainManager DomainManager { get; }

  public virtual bool CanCreateSourceCode => true;

  public virtual Scope GetScope(string path) => (Scope) null;

  public virtual Scope CreateScope() => new Scope();

  public virtual Scope CreateScope(IDictionary<string, object> dictionary) => new Scope(dictionary);

  public virtual Scope CreateScope(IDynamicMetaObjectProvider storage) => new Scope(storage);

  public ScopeExtension EnsureScopeExtension(Scope scope)
  {
    ContractUtils.RequiresNotNull((object) scope, nameof (scope));
    ScopeExtension extension = scope.GetExtension(this.ContextId);
    if (extension != null)
      return extension;
    return scope.SetExtension(this.ContextId, this.CreateScopeExtension(scope) ?? throw Microsoft.Scripting.Error.MustReturnScopeExtension());
  }

  public virtual ScopeExtension CreateScopeExtension(Scope scope) => new ScopeExtension(scope);

  public virtual void ScopeSetVariable(Scope scope, string name, object value)
  {
    this.Operations.SetMember((object) scope, name, value);
  }

  public virtual bool ScopeTryGetVariable(Scope scope, string name, out object value)
  {
    return this.Operations.TryGetMember((object) scope, name, out value);
  }

  public virtual T ScopeGetVariable<T>(Scope scope, string name)
  {
    return this.Operations.GetMember<T>((object) scope, name);
  }

  public virtual object ScopeGetVariable(Scope scope, string name)
  {
    return this.Operations.GetMember((object) scope, name);
  }

  public virtual SourceCodeReader GetSourceReader(
    Stream stream,
    Encoding defaultEncoding,
    string path)
  {
    ContractUtils.RequiresNotNull((object) stream, nameof (stream));
    ContractUtils.RequiresNotNull((object) defaultEncoding, nameof (defaultEncoding));
    ContractUtils.Requires(stream.CanRead && stream.CanSeek, nameof (stream), "The stream must support reading and seeking");
    StreamReader streamReader = new StreamReader(stream, defaultEncoding, true);
    streamReader.Peek();
    return new SourceCodeReader((TextReader) streamReader, streamReader.CurrentEncoding);
  }

  public virtual CompilerOptions GetCompilerOptions() => new CompilerOptions();

  public virtual CompilerOptions GetCompilerOptions(Scope scope) => this.GetCompilerOptions();

  public abstract ScriptCode CompileSourceCode(
    SourceUnit sourceUnit,
    CompilerOptions options,
    ErrorSink errorSink);

  public virtual ScriptCode LoadCompiledCode(Delegate method, string path, string customData)
  {
    throw new NotSupportedException();
  }

  public virtual int ExecuteProgram(SourceUnit program)
  {
    ContractUtils.RequiresNotNull((object) program, nameof (program));
    object obj = program.Execute();
    if (obj == null)
      return 0;
    CallSite<Func<CallSite, object, int>> callSite = CallSite<Func<CallSite, object, int>>.Create((CallSiteBinder) this.CreateConvertBinder(typeof (int), new bool?(true)));
    return callSite.Target((CallSite) callSite, obj);
  }

  public virtual Version LanguageVersion => new Version(0, 0);

  public virtual void SetSearchPaths(ICollection<string> paths)
  {
    throw new NotSupportedException();
  }

  public virtual ICollection<string> GetSearchPaths()
  {
    return (ICollection<string>) this.Options.SearchPaths;
  }

  public virtual SourceUnit GenerateSourceCode(
    CodeObject codeDom,
    string path,
    SourceCodeKind kind)
  {
    throw new NotImplementedException();
  }

  public virtual TService GetService<TService>(params object[] args) where TService : class
  {
    return default (TService);
  }

  public virtual Guid LanguageGuid => Guid.Empty;

  public virtual Guid VendorGuid => Guid.Empty;

  public virtual void Shutdown()
  {
  }

  public virtual string FormatException(Exception exception) => exception.ToString();

  public virtual IList<DynamicStackFrame> GetStackFrames(Exception exception)
  {
    return (IList<DynamicStackFrame>) new DynamicStackFrame[0];
  }

  public virtual LanguageOptions Options => new LanguageOptions();

  public SourceUnit CreateSnippet(string code, SourceCodeKind kind)
  {
    return this.CreateSnippet(code, (string) null, kind);
  }

  public SourceUnit CreateSnippet(string code, string id, SourceCodeKind kind)
  {
    ContractUtils.RequiresNotNull((object) code, nameof (code));
    return this.CreateSourceUnit((TextContentProvider) new SourceStringContentProvider(code), id, kind);
  }

  public SourceUnit CreateFileUnit(string path)
  {
    return this.CreateFileUnit(path, StringUtils.DefaultEncoding);
  }

  public SourceUnit CreateFileUnit(string path, Encoding encoding)
  {
    return this.CreateFileUnit(path, encoding, SourceCodeKind.File);
  }

  public SourceUnit CreateFileUnit(string path, Encoding encoding, SourceCodeKind kind)
  {
    ContractUtils.RequiresNotNull((object) path, nameof (path));
    ContractUtils.RequiresNotNull((object) encoding, nameof (encoding));
    return this.CreateSourceUnit((TextContentProvider) new LanguageBoundTextContentProvider(this, (StreamContentProvider) new FileStreamContentProvider(this.DomainManager.Platform, path), encoding, path), path, kind);
  }

  public SourceUnit CreateFileUnit(string path, string content)
  {
    ContractUtils.RequiresNotNull((object) path, nameof (path));
    ContractUtils.RequiresNotNull((object) content, nameof (content));
    return this.CreateSourceUnit((TextContentProvider) new SourceStringContentProvider(content), path, SourceCodeKind.File);
  }

  public SourceUnit CreateSourceUnit(
    StreamContentProvider contentProvider,
    string path,
    Encoding encoding,
    SourceCodeKind kind)
  {
    ContractUtils.RequiresNotNull((object) contentProvider, nameof (contentProvider));
    ContractUtils.RequiresNotNull((object) encoding, nameof (encoding));
    ContractUtils.Requires(kind.IsValid(), nameof (kind));
    ContractUtils.Requires(this.CanCreateSourceCode);
    return new SourceUnit(this, (TextContentProvider) new LanguageBoundTextContentProvider(this, contentProvider, encoding, path), path, kind);
  }

  public SourceUnit CreateSourceUnit(
    TextContentProvider contentProvider,
    string path,
    SourceCodeKind kind)
  {
    ContractUtils.RequiresNotNull((object) contentProvider, nameof (contentProvider));
    ContractUtils.Requires(kind.IsValid(), nameof (kind));
    ContractUtils.Requires(this.CanCreateSourceCode);
    return new SourceUnit(this, contentProvider, path, kind);
  }

  public virtual ErrorSink GetCompilerErrorSink() => ErrorSink.Null;

  internal static DynamicMetaObject ErrorMetaObject(
    Type resultType,
    DynamicMetaObject target,
    DynamicMetaObject[] args,
    DynamicMetaObject errorSuggestion)
  {
    return errorSuggestion ?? new DynamicMetaObject((Expression) Expression.Throw((Expression) Expression.New(typeof (NotImplementedException)), resultType), target.Restrictions.Merge(BindingRestrictions.Combine((IList<DynamicMetaObject>) args)));
  }

  public virtual UnaryOperationBinder CreateUnaryOperationBinder(ExpressionType operation)
  {
    return (UnaryOperationBinder) new LanguageContext.DefaultUnaryOperationBinder(operation);
  }

  public virtual BinaryOperationBinder CreateBinaryOperationBinder(ExpressionType operation)
  {
    return (BinaryOperationBinder) new LanguageContext.DefaultBinaryOperationBinder(operation);
  }

  public virtual ConvertBinder CreateConvertBinder(Type toType, bool? explicitCast)
  {
    return (ConvertBinder) new LanguageContext.DefaultConvertAction(toType, ((int) explicitCast ?? 0) != 0);
  }

  public virtual GetMemberBinder CreateGetMemberBinder(string name, bool ignoreCase)
  {
    return (GetMemberBinder) new LanguageContext.DefaultGetMemberAction(name, ignoreCase);
  }

  public virtual SetMemberBinder CreateSetMemberBinder(string name, bool ignoreCase)
  {
    return (SetMemberBinder) new LanguageContext.DefaultSetMemberAction(name, ignoreCase);
  }

  public virtual DeleteMemberBinder CreateDeleteMemberBinder(string name, bool ignoreCase)
  {
    return (DeleteMemberBinder) new LanguageContext.DefaultDeleteMemberAction(name, ignoreCase);
  }

  public virtual InvokeMemberBinder CreateCallBinder(
    string name,
    bool ignoreCase,
    CallInfo callInfo)
  {
    return (InvokeMemberBinder) new LanguageContext.DefaultCallAction(this, name, ignoreCase, callInfo);
  }

  public virtual InvokeBinder CreateInvokeBinder(CallInfo callInfo)
  {
    return (InvokeBinder) new LanguageContext.DefaultInvokeAction(callInfo);
  }

  public virtual CreateInstanceBinder CreateCreateBinder(CallInfo callInfo)
  {
    return (CreateInstanceBinder) new LanguageContext.DefaultCreateAction(callInfo);
  }

  public DynamicOperations Operations
  {
    get
    {
      if (this._operations == null)
        Interlocked.CompareExchange<DynamicOperations>(ref this._operations, new DynamicOperations(this), (DynamicOperations) null);
      return this._operations;
    }
  }

  public virtual IList<string> GetMemberNames(object obj)
  {
    return obj is IDynamicMetaObjectProvider metaObjectProvider ? (IList<string>) metaObjectProvider.GetMetaObject((Expression) Expression.Parameter(typeof (object), (string) null)).GetDynamicMemberNames().ToReadOnly<string>() : (IList<string>) EmptyArray<string>.Instance;
  }

  public virtual string GetDocumentation(object obj) => string.Empty;

  public virtual IList<string> GetCallSignatures(object obj) => (IList<string>) new string[0];

  public virtual bool IsCallable(object obj)
  {
    return obj != null && typeof (Delegate).IsAssignableFrom(obj.GetType());
  }

  public virtual string FormatObject(DynamicOperations operations, object obj)
  {
    return obj != null ? obj.ToString() : "null";
  }

  public virtual void GetExceptionMessage(
    Exception exception,
    out string message,
    out string errorTypeName)
  {
    message = exception.Message;
    errorTypeName = exception.GetType().Name;
  }

  private sealed class DefaultUnaryOperationBinder : UnaryOperationBinder
  {
    internal DefaultUnaryOperationBinder(ExpressionType operation)
      : base(operation)
    {
    }

    public override DynamicMetaObject FallbackUnaryOperation(
      DynamicMetaObject target,
      DynamicMetaObject errorSuggestion)
    {
      return LanguageContext.ErrorMetaObject(this.ReturnType, target, new DynamicMetaObject[1]
      {
        target
      }, errorSuggestion);
    }
  }

  private sealed class DefaultBinaryOperationBinder : BinaryOperationBinder
  {
    internal DefaultBinaryOperationBinder(ExpressionType operation)
      : base(operation)
    {
    }

    public override DynamicMetaObject FallbackBinaryOperation(
      DynamicMetaObject target,
      DynamicMetaObject arg,
      DynamicMetaObject errorSuggestion)
    {
      return LanguageContext.ErrorMetaObject(this.ReturnType, target, new DynamicMetaObject[2]
      {
        target,
        arg
      }, errorSuggestion);
    }
  }

  private class DefaultConvertAction : ConvertBinder
  {
    internal DefaultConvertAction(Type type, bool @explicit)
      : base(type, @explicit)
    {
    }

    public override DynamicMetaObject FallbackConvert(
      DynamicMetaObject self,
      DynamicMetaObject errorSuggestion)
    {
      if (this.Type.IsAssignableFrom(self.LimitType))
        return new DynamicMetaObject((Expression) Expression.Convert(self.Expression, this.Type), BindingRestrictions.GetTypeRestriction(self.Expression, self.LimitType));
      return errorSuggestion != null ? errorSuggestion : new DynamicMetaObject((Expression) Expression.Throw((Expression) Expression.Constant((object) new ArgumentTypeException($"Expected {this.Type.FullName}, got {self.LimitType.FullName}")), this.ReturnType), BindingRestrictions.GetTypeRestriction(self.Expression, self.LimitType));
    }
  }

  private class DefaultGetMemberAction : GetMemberBinder
  {
    internal DefaultGetMemberAction(string name, bool ignoreCase)
      : base(name, ignoreCase)
    {
    }

    public override DynamicMetaObject FallbackGetMember(
      DynamicMetaObject self,
      DynamicMetaObject errorSuggestion)
    {
      DynamicMetaObject member = errorSuggestion;
      if (member != null)
        return member;
      return new DynamicMetaObject((Expression) Expression.Throw((Expression) Expression.New(typeof (MissingMemberException).GetConstructor(new Type[1]
      {
        typeof (string)
      }), (Expression) Expression.Constant((object) ("unknown member: " + this.Name))), typeof (object)), self.Value == null ? BindingRestrictions.GetExpressionRestriction((Expression) Expression.Equal(self.Expression, (Expression) Expression.Constant((object) null))) : BindingRestrictions.GetTypeRestriction(self.Expression, self.Value.GetType()));
    }
  }

  private class DefaultSetMemberAction : SetMemberBinder
  {
    internal DefaultSetMemberAction(string name, bool ignoreCase)
      : base(name, ignoreCase)
    {
    }

    public override DynamicMetaObject FallbackSetMember(
      DynamicMetaObject self,
      DynamicMetaObject value,
      DynamicMetaObject errorSuggestion)
    {
      return LanguageContext.ErrorMetaObject(this.ReturnType, self, new DynamicMetaObject[1]
      {
        value
      }, errorSuggestion);
    }
  }

  private class DefaultDeleteMemberAction : DeleteMemberBinder
  {
    internal DefaultDeleteMemberAction(string name, bool ignoreCase)
      : base(name, ignoreCase)
    {
    }

    public override DynamicMetaObject FallbackDeleteMember(
      DynamicMetaObject self,
      DynamicMetaObject errorSuggestion)
    {
      return LanguageContext.ErrorMetaObject(this.ReturnType, self, DynamicMetaObject.EmptyMetaObjects, errorSuggestion);
    }
  }

  private class DefaultCallAction : InvokeMemberBinder
  {
    private readonly LanguageContext _context;

    internal DefaultCallAction(
      LanguageContext context,
      string name,
      bool ignoreCase,
      CallInfo callInfo)
      : base(name, ignoreCase, callInfo)
    {
      this._context = context;
    }

    public override DynamicMetaObject FallbackInvokeMember(
      DynamicMetaObject target,
      DynamicMetaObject[] args,
      DynamicMetaObject errorSuggestion)
    {
      return LanguageContext.ErrorMetaObject(this.ReturnType, target, ((IList<DynamicMetaObject>) args).AddFirst<DynamicMetaObject>(target), errorSuggestion);
    }

    private static Expression[] GetArgs(DynamicMetaObject target, DynamicMetaObject[] args)
    {
      Expression[] args1 = new Expression[args.Length + 1];
      args1[0] = target.Expression;
      for (int index = 0; index < args.Length; ++index)
        args1[1 + index] = args[index].Expression;
      return args1;
    }

    public override DynamicMetaObject FallbackInvoke(
      DynamicMetaObject target,
      DynamicMetaObject[] args,
      DynamicMetaObject errorSuggestion)
    {
      return new DynamicMetaObject((Expression) Expression.Dynamic((CallSiteBinder) this._context.CreateInvokeBinder(this.CallInfo), typeof (object), LanguageContext.DefaultCallAction.GetArgs(target, args)), target.Restrictions.Merge(BindingRestrictions.Combine((IList<DynamicMetaObject>) args)));
    }
  }

  private class DefaultInvokeAction : InvokeBinder
  {
    internal DefaultInvokeAction(CallInfo callInfo)
      : base(callInfo)
    {
    }

    public override DynamicMetaObject FallbackInvoke(
      DynamicMetaObject target,
      DynamicMetaObject[] args,
      DynamicMetaObject errorSuggestion)
    {
      return LanguageContext.ErrorMetaObject(this.ReturnType, target, args, errorSuggestion);
    }
  }

  private class DefaultCreateAction : CreateInstanceBinder
  {
    internal DefaultCreateAction(CallInfo callInfo)
      : base(callInfo)
    {
    }

    public override DynamicMetaObject FallbackCreateInstance(
      DynamicMetaObject target,
      DynamicMetaObject[] args,
      DynamicMetaObject errorSuggestion)
    {
      return LanguageContext.ErrorMetaObject(this.ReturnType, target, args, errorSuggestion);
    }
  }
}
