// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Indentation.CSharp.IndentationReformatter
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#nullable disable
namespace ICSharpCode.AvalonEdit.Indentation.CSharp;

internal sealed class IndentationReformatter
{
  private StringBuilder wordBuilder;
  private Stack<IndentationReformatter.Block> blocks;
  private IndentationReformatter.Block block;
  private bool inString;
  private bool inChar;
  private bool verbatim;
  private bool escape;
  private bool lineComment;
  private bool blockComment;
  private char lastRealChar;

  public void Reformat(IDocumentAccessor doc, IndentationSettings set)
  {
    this.Init();
    while (doc.MoveNext())
      this.Step(doc, set);
  }

  public void Init()
  {
    this.wordBuilder = new StringBuilder();
    this.blocks = new Stack<IndentationReformatter.Block>();
    this.block = new IndentationReformatter.Block();
    this.block.InnerIndent = "";
    this.block.OuterIndent = "";
    this.block.Bracket = '{';
    this.block.Continuation = false;
    this.block.LastWord = "";
    this.block.OneLineBlock = 0;
    this.block.PreviousOneLineBlock = 0;
    this.block.StartLine = 0;
    this.inString = false;
    this.inChar = false;
    this.verbatim = false;
    this.escape = false;
    this.lineComment = false;
    this.blockComment = false;
    this.lastRealChar = ' ';
  }

  public void Step(IDocumentAccessor doc, IndentationSettings set)
  {
    string text1 = doc.Text;
    if (set.LeaveEmptyLines && text1.Length == 0)
      return;
    string str = text1.TrimStart();
    StringBuilder stringBuilder = new StringBuilder();
    if (str.Length == 0)
    {
      if (this.blockComment || this.inString && this.verbatim)
        return;
      stringBuilder.Append(this.block.InnerIndent);
      stringBuilder.Append(IndentationReformatter.Repeat(set.IndentString, this.block.OneLineBlock));
      if (this.block.Continuation)
        stringBuilder.Append(set.IndentString);
      if (!(doc.Text != stringBuilder.ToString()))
        return;
      doc.Text = stringBuilder.ToString();
    }
    else
    {
      if (IndentationReformatter.TrimEnd(doc))
        str = doc.Text.TrimStart();
      IndentationReformatter.Block block = this.block;
      bool blockComment = this.blockComment;
      bool flag = this.inString && this.verbatim;
      this.lineComment = false;
      this.inChar = false;
      this.escape = false;
      if (!this.verbatim)
        this.inString = false;
      this.lastRealChar = '\n';
      char c = ' ';
      char ch1 = str[0];
      for (int index = 0; index < str.Length && !this.lineComment; ++index)
      {
        char ch2 = c;
        c = ch1;
        ch1 = index + 1 >= str.Length ? '\n' : str[index + 1];
        if (this.escape)
        {
          this.escape = false;
        }
        else
        {
          switch (c)
          {
            case '"':
              if (!this.inChar && !this.lineComment && !this.blockComment)
              {
                this.inString = !this.inString;
                if (!this.inString && this.verbatim)
                {
                  if (ch1 == '"')
                  {
                    this.escape = true;
                    this.inString = true;
                    break;
                  }
                  this.verbatim = false;
                  break;
                }
                if (this.inString && ch2 == '@')
                {
                  this.verbatim = true;
                  break;
                }
                break;
              }
              break;
            case '#':
              if (!this.inChar && !this.blockComment && !this.inString)
              {
                this.lineComment = true;
                break;
              }
              break;
            case '\'':
              if (!this.inString && !this.lineComment && !this.blockComment)
              {
                this.inChar = !this.inChar;
                break;
              }
              break;
            case '/':
              if (this.blockComment && ch2 == '*')
                this.blockComment = false;
              if (!this.inString && !this.inChar)
              {
                if (!this.blockComment && ch1 == '/')
                  this.lineComment = true;
                if (!this.lineComment && ch1 == '*')
                {
                  this.blockComment = true;
                  break;
                }
                break;
              }
              break;
            case '\\':
              if (this.inString && !this.verbatim || this.inChar)
              {
                this.escape = true;
                break;
              }
              break;
          }
          if (this.lineComment || this.blockComment || this.inString || this.inChar)
          {
            if (this.wordBuilder.Length > 0)
              this.block.LastWord = this.wordBuilder.ToString();
            this.wordBuilder.Length = 0;
          }
          else
          {
            if (!char.IsWhiteSpace(c) && c != '[' && c != '/' && this.block.Bracket == '{')
              this.block.Continuation = true;
            if (char.IsLetterOrDigit(c))
            {
              this.wordBuilder.Append(c);
            }
            else
            {
              if (this.wordBuilder.Length > 0)
                this.block.LastWord = this.wordBuilder.ToString();
              this.wordBuilder.Length = 0;
            }
            switch (c)
            {
              case '(':
              case '[':
                this.blocks.Push(this.block);
                if (this.block.StartLine == doc.LineNumber)
                  this.block.InnerIndent = this.block.OuterIndent;
                else
                  this.block.StartLine = doc.LineNumber;
                this.block.Indent(IndentationReformatter.Repeat(set.IndentString, block.OneLineBlock) + (block.Continuation ? set.IndentString : "") + (index == str.Length - 1 ? set.IndentString : new string(' ', index + 1)));
                this.block.Bracket = c;
                break;
              case ')':
                if (this.blocks.Count != 0 && this.block.Bracket == '(')
                {
                  this.block = this.blocks.Pop();
                  if (IndentationReformatter.IsSingleStatementKeyword(this.block.LastWord))
                  {
                    this.block.Continuation = false;
                    break;
                  }
                  break;
                }
                break;
              case ',':
              case ';':
                this.block.Continuation = false;
                this.block.ResetOneLineBlock();
                break;
              case ':':
                if (this.block.LastWord == "case" || str.StartsWith("case ", StringComparison.Ordinal) || str.StartsWith(this.block.LastWord + ":", StringComparison.Ordinal))
                {
                  this.block.Continuation = false;
                  this.block.ResetOneLineBlock();
                  break;
                }
                break;
              case ']':
                if (this.blocks.Count != 0 && this.block.Bracket == '[')
                {
                  this.block = this.blocks.Pop();
                  break;
                }
                break;
              case '{':
                this.block.ResetOneLineBlock();
                this.blocks.Push(this.block);
                this.block.StartLine = doc.LineNumber;
                if (this.block.LastWord == "switch")
                  this.block.Indent(set.IndentString + set.IndentString);
                else
                  this.block.Indent(set);
                this.block.Bracket = '{';
                break;
              case '}':
                while (this.block.Bracket != '{' && this.blocks.Count != 0)
                  this.block = this.blocks.Pop();
                if (this.blocks.Count != 0)
                {
                  this.block = this.blocks.Pop();
                  this.block.Continuation = false;
                  this.block.ResetOneLineBlock();
                  break;
                }
                break;
            }
            if (!char.IsWhiteSpace(c))
              this.lastRealChar = c;
          }
        }
      }
      if (this.wordBuilder.Length > 0)
        this.block.LastWord = this.wordBuilder.ToString();
      this.wordBuilder.Length = 0;
      if (flag || blockComment && str[0] != '*' || doc.Text.StartsWith("//\t", StringComparison.Ordinal) || doc.Text == "//")
        return;
      if (str[0] == '}')
      {
        stringBuilder.Append(block.OuterIndent);
        block.ResetOneLineBlock();
        block.Continuation = false;
      }
      else
        stringBuilder.Append(block.InnerIndent);
      if (stringBuilder.Length > 0 && block.Bracket == '(' && str[0] == ')')
        stringBuilder.Remove(stringBuilder.Length - 1, 1);
      else if (stringBuilder.Length > 0 && block.Bracket == '[' && str[0] == ']')
        stringBuilder.Remove(stringBuilder.Length - 1, 1);
      if (str[0] == ':')
        block.Continuation = true;
      else if (this.lastRealChar == ':' && stringBuilder.Length >= set.IndentString.Length)
      {
        if (this.block.LastWord == "case" || str.StartsWith("case ", StringComparison.Ordinal) || str.StartsWith(this.block.LastWord + ":", StringComparison.Ordinal))
          stringBuilder.Remove(stringBuilder.Length - set.IndentString.Length, set.IndentString.Length);
      }
      else if (this.lastRealChar == ')')
      {
        if (IndentationReformatter.IsSingleStatementKeyword(this.block.LastWord))
          ++this.block.OneLineBlock;
      }
      else if (this.lastRealChar == 'e' && this.block.LastWord == "else")
      {
        this.block.OneLineBlock = Math.Max(1, this.block.PreviousOneLineBlock);
        this.block.Continuation = false;
        block.OneLineBlock = this.block.OneLineBlock - 1;
      }
      if (doc.IsReadOnly)
      {
        if (block.Continuation || block.OneLineBlock != 0 || block.StartLine != this.block.StartLine || this.block.StartLine >= doc.LineNumber || this.lastRealChar == ':')
          return;
        stringBuilder.Length = 0;
        string text2 = doc.Text;
        for (int index = 0; index < text2.Length && char.IsWhiteSpace(text2[index]); ++index)
          stringBuilder.Append(text2[index]);
        if (blockComment && stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == ' ')
          --stringBuilder.Length;
        this.block.InnerIndent = stringBuilder.ToString();
      }
      else
      {
        if (str[0] != '{')
        {
          if (str[0] != ')' && block.Continuation && block.Bracket == '{')
            stringBuilder.Append(set.IndentString);
          stringBuilder.Append(IndentationReformatter.Repeat(set.IndentString, block.OneLineBlock));
        }
        if (blockComment)
          stringBuilder.Append(' ');
        if (stringBuilder.Length == doc.Text.Length - str.Length && doc.Text.StartsWith(stringBuilder.ToString(), StringComparison.Ordinal) && !char.IsWhiteSpace(doc.Text[stringBuilder.Length]))
          return;
        doc.Text = stringBuilder.ToString() + str;
      }
    }
  }

  private static string Repeat(string text, int count)
  {
    if (count == 0)
      return string.Empty;
    if (count == 1)
      return text;
    StringBuilder stringBuilder = new StringBuilder(text.Length * count);
    for (int index = 0; index < count; ++index)
      stringBuilder.Append(text);
    return stringBuilder.ToString();
  }

  private static bool IsSingleStatementKeyword(string keyword)
  {
    switch (keyword)
    {
      case "if":
      case "for":
      case "while":
      case "do":
      case "foreach":
      case "using":
      case "lock":
        return true;
      default:
        return false;
    }
  }

  private static bool TrimEnd(IDocumentAccessor doc)
  {
    string text = doc.Text;
    if (!char.IsWhiteSpace(text[text.Length - 1]) || text.EndsWith("// ", StringComparison.Ordinal) || text.EndsWith("* ", StringComparison.Ordinal))
      return false;
    doc.Text = text.TrimEnd();
    return true;
  }

  private struct Block
  {
    public string OuterIndent;
    public string InnerIndent;
    public string LastWord;
    public char Bracket;
    public bool Continuation;
    public int OneLineBlock;
    public int PreviousOneLineBlock;
    public int StartLine;

    public void ResetOneLineBlock()
    {
      this.PreviousOneLineBlock = this.OneLineBlock;
      this.OneLineBlock = 0;
    }

    public void Indent(IndentationSettings set) => this.Indent(set.IndentString);

    public void Indent(string indentationString)
    {
      this.OuterIndent = this.InnerIndent;
      this.InnerIndent += indentationString;
      this.Continuation = false;
      this.ResetOneLineBlock();
      this.LastWord = "";
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[Block StartLine={0}, LastWord='{1}', Continuation={2}, OneLineBlock={3}, PreviousOneLineBlock={4}]", (object) this.StartLine, (object) this.LastWord, (object) this.Continuation, (object) this.OneLineBlock, (object) this.PreviousOneLineBlock);
    }
  }
}
