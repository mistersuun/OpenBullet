// Decompiled with JetBrains decompiler
// Type: IronPython.Hosting.PythonCodeDomCodeGen
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Hosting;

internal class PythonCodeDomCodeGen : CodeDomCodeGen
{
  private Stack<int> _indents = new Stack<int>((IEnumerable<int>) new int[1]);
  private int _generatedIndent;

  protected override void WriteExpressionStatement(CodeExpressionStatement s)
  {
    this.Writer.Write(new string(' ', this._generatedIndent + this._indents.Peek()));
    this.WriteExpression(s.Expression);
    this.Writer.Write("\n");
  }

  protected override void WriteSnippetExpression(CodeSnippetExpression e)
  {
    this.Writer.Write(this.IndentSnippet(e.Value));
  }

  protected override void WriteSnippetStatement(CodeSnippetStatement s)
  {
    string block = s.Value;
    this.Writer.Write(this.IndentSnippetStatement(block));
    this.Writer.Write('\n');
    string str = block.Substring(block.LastIndexOf('\n') + 1);
    if (!string.IsNullOrEmpty(str.Trim('\t', ' ')))
      return;
    int length = str.Replace("\t", "        ").Length;
    if (length > this._indents.Peek())
    {
      this._indents.Push(length);
    }
    else
    {
      while (length < this._indents.Peek())
        this._indents.Pop();
    }
  }

  protected override void WriteFunctionDefinition(CodeMemberMethod func)
  {
    this.Writer.Write("def ");
    this.Writer.Write(func.Name);
    this.Writer.Write("(");
    for (int index = 0; index < func.Parameters.Count; ++index)
    {
      if (index != 0)
        this.Writer.Write(",");
      this.Writer.Write(func.Parameters[index].Name);
    }
    this.Writer.Write("):\n");
    int num = this._indents.Peek();
    this._generatedIndent += 4;
    foreach (CodeStatement statement in (CollectionBase) func.Statements)
      this.WriteStatement(statement);
    this._generatedIndent -= 4;
    while (this._indents.Peek() > num)
      this._indents.Pop();
  }

  protected override string QuoteString(string val)
  {
    return $"'''{val.Replace("\\", "\\\\").Replace("'''", "\\'''")}'''";
  }

  private string IndentSnippet(string block)
  {
    return block.Replace("\n", "\n" + new string(' ', this._generatedIndent));
  }

  private string IndentSnippetStatement(string block)
  {
    return new string(' ', this._generatedIndent) + this.IndentSnippet(block);
  }
}
