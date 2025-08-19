// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.CodeDomCodeGen
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System.CodeDom;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public abstract class CodeDomCodeGen
{
  protected static readonly object SourceSpanKey = (object) typeof (SourceSpan);
  private PositionTrackingWriter _writer;

  protected PositionTrackingWriter Writer => this._writer;

  protected abstract void WriteExpressionStatement(CodeExpressionStatement s);

  protected abstract void WriteFunctionDefinition(CodeMemberMethod func);

  protected abstract string QuoteString(string val);

  public SourceUnit GenerateCode(
    CodeMemberMethod codeDom,
    LanguageContext context,
    string path,
    SourceCodeKind kind)
  {
    ContractUtils.RequiresNotNull((object) codeDom, nameof (codeDom));
    ContractUtils.RequiresNotNull((object) context, nameof (context));
    ContractUtils.Requires(path == null || path.Length > 0, nameof (path));
    this._writer?.Close();
    this._writer = new PositionTrackingWriter();
    this.WriteFunctionDefinition(codeDom);
    return this.CreateSourceUnit(context, path, kind);
  }

  private SourceUnit CreateSourceUnit(LanguageContext context, string path, SourceCodeKind kind)
  {
    string code = this._writer.ToString();
    SourceUnit snippet = context.CreateSnippet(code, path, kind);
    snippet.SetLineMapping(this._writer.GetLineMap());
    return snippet;
  }

  protected virtual void WriteArgumentReferenceExpression(CodeArgumentReferenceExpression e)
  {
    this._writer.Write(e.ParameterName);
  }

  protected virtual void WriteSnippetExpression(CodeSnippetExpression e)
  {
    this._writer.Write(e.Value);
  }

  protected virtual void WriteSnippetStatement(CodeSnippetStatement s)
  {
    this._writer.Write(s.Value);
    this._writer.Write('\n');
  }

  protected void WriteStatement(CodeStatement s)
  {
    if (s.LinePragma != null)
      this._writer.MapLocation(s.LinePragma);
    switch (s)
    {
      case CodeExpressionStatement s1:
        this.WriteExpressionStatement(s1);
        break;
      case CodeSnippetStatement s2:
        this.WriteSnippetStatement(s2);
        break;
    }
  }

  protected void WriteExpression(CodeExpression e)
  {
    switch (e)
    {
      case CodeSnippetExpression e1:
        this.WriteSnippetExpression(e1);
        break;
      case CodePrimitiveExpression e2:
        this.WritePrimitiveExpression(e2);
        break;
      case CodeMethodInvokeExpression m:
        this.WriteCallExpression(m);
        break;
      case CodeArgumentReferenceExpression e3:
        this.WriteArgumentReferenceExpression(e3);
        break;
    }
  }

  protected void WritePrimitiveExpression(CodePrimitiveExpression e)
  {
    object obj = e.Value;
    if (obj is string val)
      this._writer.Write(this.QuoteString(val));
    else
      this._writer.Write(obj);
  }

  protected void WriteCallExpression(CodeMethodInvokeExpression m)
  {
    if (m.Method.TargetObject != null)
    {
      this.WriteExpression(m.Method.TargetObject);
      this._writer.Write(".");
    }
    this._writer.Write(m.Method.MethodName);
    this._writer.Write("(");
    for (int index = 0; index < m.Parameters.Count; ++index)
    {
      if (index != 0)
        this._writer.Write(",");
      this.WriteExpression(m.Parameters[index]);
    }
    this._writer.Write(")");
  }
}
