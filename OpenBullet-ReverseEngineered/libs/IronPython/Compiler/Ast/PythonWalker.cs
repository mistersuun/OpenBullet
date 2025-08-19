// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.PythonWalker
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler.Ast;

public class PythonWalker
{
  public virtual bool Walk(AndExpression node) => true;

  public virtual void PostWalk(AndExpression node)
  {
  }

  public virtual bool Walk(BackQuoteExpression node) => true;

  public virtual void PostWalk(BackQuoteExpression node)
  {
  }

  public virtual bool Walk(BinaryExpression node) => true;

  public virtual void PostWalk(BinaryExpression node)
  {
  }

  public virtual bool Walk(CallExpression node) => true;

  public virtual void PostWalk(CallExpression node)
  {
  }

  public virtual bool Walk(ConditionalExpression node) => true;

  public virtual void PostWalk(ConditionalExpression node)
  {
  }

  public virtual bool Walk(ConstantExpression node) => true;

  public virtual void PostWalk(ConstantExpression node)
  {
  }

  public virtual bool Walk(DictionaryComprehension node) => true;

  public virtual void PostWalk(DictionaryComprehension node)
  {
  }

  public virtual bool Walk(DictionaryExpression node) => true;

  public virtual void PostWalk(DictionaryExpression node)
  {
  }

  public virtual bool Walk(ErrorExpression node) => true;

  public virtual void PostWalk(ErrorExpression node)
  {
  }

  public virtual bool Walk(GeneratorExpression node) => true;

  public virtual void PostWalk(GeneratorExpression node)
  {
  }

  public virtual bool Walk(IndexExpression node) => true;

  public virtual void PostWalk(IndexExpression node)
  {
  }

  public virtual bool Walk(LambdaExpression node) => true;

  public virtual void PostWalk(LambdaExpression node)
  {
  }

  public virtual bool Walk(ListComprehension node) => true;

  public virtual void PostWalk(ListComprehension node)
  {
  }

  public virtual bool Walk(ListExpression node) => true;

  public virtual void PostWalk(ListExpression node)
  {
  }

  public virtual bool Walk(MemberExpression node) => true;

  public virtual void PostWalk(MemberExpression node)
  {
  }

  public virtual bool Walk(NameExpression node) => true;

  public virtual void PostWalk(NameExpression node)
  {
  }

  public virtual bool Walk(OrExpression node) => true;

  public virtual void PostWalk(OrExpression node)
  {
  }

  public virtual bool Walk(ParenthesisExpression node) => true;

  public virtual void PostWalk(ParenthesisExpression node)
  {
  }

  public virtual bool Walk(SetComprehension node) => true;

  public virtual void PostWalk(SetComprehension node)
  {
  }

  public virtual bool Walk(SetExpression node) => true;

  public virtual void PostWalk(SetExpression node)
  {
  }

  public virtual bool Walk(SliceExpression node) => true;

  public virtual void PostWalk(SliceExpression node)
  {
  }

  public virtual bool Walk(TupleExpression node) => true;

  public virtual void PostWalk(TupleExpression node)
  {
  }

  public virtual bool Walk(UnaryExpression node) => true;

  public virtual void PostWalk(UnaryExpression node)
  {
  }

  public virtual bool Walk(YieldExpression node) => true;

  public virtual void PostWalk(YieldExpression node)
  {
  }

  public virtual bool Walk(AssertStatement node) => true;

  public virtual void PostWalk(AssertStatement node)
  {
  }

  public virtual bool Walk(AssignmentStatement node) => true;

  public virtual void PostWalk(AssignmentStatement node)
  {
  }

  public virtual bool Walk(AugmentedAssignStatement node) => true;

  public virtual void PostWalk(AugmentedAssignStatement node)
  {
  }

  public virtual bool Walk(BreakStatement node) => true;

  public virtual void PostWalk(BreakStatement node)
  {
  }

  public virtual bool Walk(ClassDefinition node) => true;

  public virtual void PostWalk(ClassDefinition node)
  {
  }

  public virtual bool Walk(ContinueStatement node) => true;

  public virtual void PostWalk(ContinueStatement node)
  {
  }

  public virtual bool Walk(DelStatement node) => true;

  public virtual void PostWalk(DelStatement node)
  {
  }

  public virtual bool Walk(EmptyStatement node) => true;

  public virtual void PostWalk(EmptyStatement node)
  {
  }

  public virtual bool Walk(ExecStatement node) => true;

  public virtual void PostWalk(ExecStatement node)
  {
  }

  public virtual bool Walk(ExpressionStatement node) => true;

  public virtual void PostWalk(ExpressionStatement node)
  {
  }

  public virtual bool Walk(ForStatement node) => true;

  public virtual void PostWalk(ForStatement node)
  {
  }

  public virtual bool Walk(FromImportStatement node) => true;

  public virtual void PostWalk(FromImportStatement node)
  {
  }

  public virtual bool Walk(FunctionDefinition node) => true;

  public virtual void PostWalk(FunctionDefinition node)
  {
  }

  public virtual bool Walk(GlobalStatement node) => true;

  public virtual void PostWalk(GlobalStatement node)
  {
  }

  public virtual bool Walk(IfStatement node) => true;

  public virtual void PostWalk(IfStatement node)
  {
  }

  public virtual bool Walk(ImportStatement node) => true;

  public virtual void PostWalk(ImportStatement node)
  {
  }

  public virtual bool Walk(PrintStatement node) => true;

  public virtual void PostWalk(PrintStatement node)
  {
  }

  public virtual bool Walk(PythonAst node) => true;

  public virtual void PostWalk(PythonAst node)
  {
  }

  public virtual bool Walk(RaiseStatement node) => true;

  public virtual void PostWalk(RaiseStatement node)
  {
  }

  public virtual bool Walk(ReturnStatement node) => true;

  public virtual void PostWalk(ReturnStatement node)
  {
  }

  public virtual bool Walk(SuiteStatement node) => true;

  public virtual void PostWalk(SuiteStatement node)
  {
  }

  public virtual bool Walk(TryStatement node) => true;

  public virtual void PostWalk(TryStatement node)
  {
  }

  public virtual bool Walk(WhileStatement node) => true;

  public virtual void PostWalk(WhileStatement node)
  {
  }

  public virtual bool Walk(WithStatement node) => true;

  public virtual void PostWalk(WithStatement node)
  {
  }

  public virtual bool Walk(Arg node) => true;

  public virtual void PostWalk(Arg node)
  {
  }

  public virtual bool Walk(ComprehensionFor node) => true;

  public virtual void PostWalk(ComprehensionFor node)
  {
  }

  public virtual bool Walk(ComprehensionIf node) => true;

  public virtual void PostWalk(ComprehensionIf node)
  {
  }

  public virtual bool Walk(DottedName node) => true;

  public virtual void PostWalk(DottedName node)
  {
  }

  public virtual bool Walk(IfStatementTest node) => true;

  public virtual void PostWalk(IfStatementTest node)
  {
  }

  public virtual bool Walk(ModuleName node) => true;

  public virtual void PostWalk(ModuleName node)
  {
  }

  public virtual bool Walk(Parameter node) => true;

  public virtual void PostWalk(Parameter node)
  {
  }

  public virtual bool Walk(RelativeModuleName node) => true;

  public virtual void PostWalk(RelativeModuleName node)
  {
  }

  public virtual bool Walk(SublistParameter node) => true;

  public virtual void PostWalk(SublistParameter node)
  {
  }

  public virtual bool Walk(TryStatementHandler node) => true;

  public virtual void PostWalk(TryStatementHandler node)
  {
  }
}
