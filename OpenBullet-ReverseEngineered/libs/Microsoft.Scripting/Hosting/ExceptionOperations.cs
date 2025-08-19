// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.ExceptionOperations
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;

#nullable disable
namespace Microsoft.Scripting.Hosting;

public sealed class ExceptionOperations : MarshalByRefObject
{
  private readonly LanguageContext _context;

  internal ExceptionOperations(LanguageContext context) => this._context = context;

  public string FormatException(Exception exception) => this._context.FormatException(exception);

  public void GetExceptionMessage(
    Exception exception,
    out string message,
    out string errorTypeName)
  {
    this._context.GetExceptionMessage(exception, out message, out errorTypeName);
  }

  public bool HandleException(Exception exception)
  {
    ContractUtils.RequiresNotNull((object) exception, nameof (exception));
    return false;
  }

  public IList<DynamicStackFrame> GetStackFrames(Exception exception)
  {
    ContractUtils.RequiresNotNull((object) exception, nameof (exception));
    return this._context.GetStackFrames(exception);
  }

  public string FormatException(ObjectHandle exception)
  {
    ContractUtils.RequiresNotNull((object) exception, nameof (exception));
    Exception exception1 = exception.Unwrap() as Exception;
    ContractUtils.Requires(exception1 != null, nameof (exception), "ObjectHandle must be to Exception object");
    return this._context.FormatException(exception1);
  }

  public void GetExceptionMessage(
    ObjectHandle exception,
    out string message,
    out string errorTypeName)
  {
    ContractUtils.RequiresNotNull((object) exception, nameof (exception));
    Exception exception1 = exception.Unwrap() as Exception;
    ContractUtils.Requires(exception1 != null, nameof (exception), "ObjectHandle must be to Exception object");
    this._context.GetExceptionMessage(exception1, out message, out errorTypeName);
  }

  public bool HandleException(ObjectHandle exception)
  {
    ContractUtils.RequiresNotNull((object) exception, nameof (exception));
    ContractUtils.Requires(exception.Unwrap() is Exception, nameof (exception), "ObjectHandle must be to Exception object");
    return false;
  }

  public IList<DynamicStackFrame> GetStackFrames(ObjectHandle exception)
  {
    ContractUtils.RequiresNotNull((object) exception, nameof (exception));
    Exception exception1 = exception.Unwrap() as Exception;
    ContractUtils.Requires(exception1 != null, nameof (exception), "ObjectHandle must be to Exception object");
    return this._context.GetStackFrames(exception1);
  }

  public override object InitializeLifetimeService() => (object) null;
}
