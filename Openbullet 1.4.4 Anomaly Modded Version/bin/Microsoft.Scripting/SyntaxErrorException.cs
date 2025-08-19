// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.SyntaxErrorException
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security;

#nullable disable
namespace Microsoft.Scripting;

[Serializable]
public class SyntaxErrorException : Exception
{
  private SourceSpan _span;
  private string _sourceCode;
  private string _sourceLine;
  private string _sourcePath;
  private Severity _severity;
  private int _errorCode;

  public SyntaxErrorException()
  {
  }

  public SyntaxErrorException(string message)
    : base(message)
  {
  }

  public SyntaxErrorException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  public SyntaxErrorException(
    string message,
    SourceUnit sourceUnit,
    SourceSpan span,
    int errorCode,
    Severity severity)
    : base(message)
  {
    ContractUtils.RequiresNotNull((object) message, nameof (message));
    this._span = span;
    this._severity = severity;
    this._errorCode = errorCode;
    if (sourceUnit == null)
      return;
    this._sourcePath = sourceUnit.Path;
    try
    {
      this._sourceCode = sourceUnit.GetCode();
      this._sourceLine = sourceUnit.GetCodeLine(this.Line);
    }
    catch (IOException ex)
    {
    }
  }

  public SyntaxErrorException(
    string message,
    string path,
    string code,
    string line,
    SourceSpan span,
    int errorCode,
    Severity severity)
    : base(message)
  {
    ContractUtils.RequiresNotNull((object) message, nameof (message));
    this._span = span;
    this._severity = severity;
    this._errorCode = errorCode;
    this._sourcePath = path;
    this._sourceCode = code;
    this._sourceLine = line;
  }

  protected SyntaxErrorException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
    this._span = (SourceSpan) info.GetValue("Span", typeof (SourceSpan));
    this._sourceCode = info.GetString(nameof (SourceCode));
    this._sourcePath = info.GetString(nameof (SourcePath));
    this._severity = (Severity) info.GetValue(nameof (Severity), typeof (Severity));
    this._errorCode = info.GetInt32(nameof (ErrorCode));
  }

  [SecurityCritical]
  public override void GetObjectData(SerializationInfo info, StreamingContext context)
  {
    ContractUtils.RequiresNotNull((object) info, nameof (info));
    base.GetObjectData(info, context);
    info.AddValue("Span", (object) this._span);
    info.AddValue("SourceCode", (object) this._sourceCode);
    info.AddValue("SourcePath", (object) this._sourcePath);
    info.AddValue("Severity", (object) this._severity);
    info.AddValue("ErrorCode", this._errorCode);
  }

  public SourceSpan RawSpan => this._span;

  public string SourceCode => this._sourceCode;

  public string SourcePath => this._sourcePath;

  public Severity Severity => this._severity;

  public int Line => this._span.Start.Line;

  public int Column => this._span.Start.Column;

  public int ErrorCode => this._errorCode;

  public string GetSymbolDocumentName() => this._sourcePath;

  public string GetCodeLine() => this._sourceLine;
}
